using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mosca2;

internal static class ComidasProcedurais
{
    private readonly record struct CoresPoca(Color Centro, Color Borda, Color Menisco, int AlfaReflexo);

    public static ComidaGerada PocaMel(float escala, Random random)
    {
        var cores = new CoresPoca(
            Color.FromArgb(220, 176, 96, 8),
            Color.FromArgb(140, 232, 165, 40),
            Color.FromArgb(150, 120, 60, 5),
            190);
        return Poca(escala, random, cores, bolhas: false);
    }

    public static ComidaGerada PocaRefrigerante(float escala, Random random)
    {
        var cores = new CoresPoca(
            Color.FromArgb(205, 58, 20, 10),
            Color.FromArgb(105, 120, 50, 25),
            Color.FromArgb(150, 35, 10, 5),
            160);
        return Poca(escala, random, cores, bolhas: true);
    }

    private static ComidaGerada Poca(float escala, Random random, CoresPoca cores, bool bolhas)
    {
        float tamanho = CatalogoComidas.TamanhoComidaPx * escala;
        int largura = (int)Math.Round(tamanho * (0.8 + random.NextDouble() * 0.2));
        int altura = (int)Math.Round(largura * (0.55 + random.NextDouble() * 0.2));
        float cx = largura / 2f;
        float cy = altura / 2f;
        const float FolgaMaxima = 1.27f;
        float rx = (largura / 2f - 2) / FolgaMaxima;
        float ry = (altura / 2f - 2) / FolgaMaxima;

        var contorno = ContornoIrregular(random, cx, cy, rx, ry);
        var imagem = new Bitmap(largura, altura, PixelFormat.Format32bppPArgb);
        using (var g = Graphics.FromImage(imagem))
        {
            EfeitosImagem.ConfigurarQualidade(g);
            g.Clear(Color.Transparent);

            using var caminho = new GraphicsPath();
            caminho.AddClosedCurve(contorno, 0.5f);

            using (var preenchimento = new PathGradientBrush(caminho)
            {
                CenterPoint = new PointF(cx - rx * 0.1f, cy - ry * 0.1f),
                CenterColor = cores.Centro,
                SurroundColors = [cores.Borda]
            })
            {
                g.FillPath(preenchimento, caminho);
            }

            using (var menisco = new Pen(cores.Menisco, 1.6f * escala) { LineJoin = LineJoin.Round })
                g.DrawPath(menisco, caminho);

            g.SetClip(caminho);
            if (bolhas)
                DesenharBolhas(g, random, cx, cy, rx, ry, escala);
            DesenharReflexo(g, new PointF(cx - rx * 0.35f, cy - ry * 0.4f), rx * 0.5f, ry * 0.2f, cores.AlfaReflexo, -12f);
            DesenharReflexo(g, new PointF(cx + rx * 0.3f, cy + ry * 0.35f), rx * 0.18f, ry * 0.08f, cores.AlfaReflexo / 2, -8f);
            g.ResetClip();
        }

        var regiao = contorno
            .Select(p => new PointF((cx + (p.X - cx) * 0.85f) / largura, (cy + (p.Y - cy) * 0.85f) / altura))
            .ToArray();
        return CatalogoComidas.Finalizar(imagem, escala, false, regiao, [], false);
    }

    private static PointF[] ContornoIrregular(Random random, float cx, float cy, float rx, float ry, int pontos = 28)
    {
        double[] amplitudes = [0.10, 0.07, 0.05, 0.03];
        var fases = amplitudes.Select(_ => random.NextDouble() * Math.PI * 2).ToArray();
        var resultado = new PointF[pontos];
        for (int i = 0; i < pontos; i++)
        {
            double angulo = 2 * Math.PI * i / pontos;
            double fator = 1;
            for (int k = 0; k < amplitudes.Length; k++)
                fator += amplitudes[k] * Math.Sin((k + 2) * angulo + fases[k]);
            fator *= 1 + (random.NextDouble() - 0.5) * 0.04;
            resultado[i] = new PointF(cx + (float)(Math.Cos(angulo) * rx * fator), cy + (float)(Math.Sin(angulo) * ry * fator));
        }
        return resultado;
    }

    private static void DesenharReflexo(Graphics g, PointF centro, float rx, float ry, int alfa, float rotacao)
    {
        if (rx < 0.5f || ry < 0.5f)
            return;
        var estado = g.Save();
        g.TranslateTransform(centro.X, centro.Y);
        g.RotateTransform(rotacao);
        using var caminho = new GraphicsPath();
        caminho.AddEllipse(-rx, -ry, rx * 2, ry * 2);
        using var pincel = new PathGradientBrush(caminho)
        {
            CenterColor = Color.FromArgb(alfa, 255, 255, 255),
            SurroundColors = [Color.FromArgb(0, 255, 255, 255)]
        };
        g.FillPath(pincel, caminho);
        g.Restore(estado);
    }

    private static void DesenharBolhas(Graphics g, Random random, float cx, float cy, float rx, float ry, float escala)
    {
        int quantidade = random.Next(3, 7);
        using var contorno = new Pen(Color.FromArgb(140, 255, 255, 255), 0.8f * escala);
        using var interior = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
        using var brilho = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
        for (int i = 0; i < quantidade; i++)
        {
            double angulo = random.NextDouble() * Math.PI * 2;
            double distancia = random.NextDouble() * 0.6;
            float x = cx + (float)(Math.Cos(angulo) * distancia * rx);
            float y = cy + (float)(Math.Sin(angulo) * distancia * ry);
            float raio = (float)(1.5 + random.NextDouble() * 1.5) * escala;
            g.FillEllipse(interior, x - raio, y - raio, raio * 2, raio * 2);
            g.DrawEllipse(contorno, x - raio, y - raio, raio * 2, raio * 2);
            float ponto = Math.Max(0.6f, raio * 0.35f);
            g.FillEllipse(brilho, x - raio * 0.45f - ponto / 2, y - raio * 0.45f - ponto / 2, ponto, ponto);
        }
    }
}
