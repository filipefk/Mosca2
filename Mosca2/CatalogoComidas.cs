using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mosca2;

internal sealed class ComidaGerada
{
    public required Bitmap Imagem { get; init; }
    public required Bitmap Sombra { get; init; }
    public required int MargemSombra { get; init; }
    public required bool ProjetaSombra { get; init; }
    public required PointF[] RegiaoPouso { get; init; }
    public required PointF[] OrigensFumaca { get; init; }
    public required bool Compartilhada { get; init; }

    public void Liberar()
    {
        if (Compartilhada)
            return;
        Imagem.Dispose();
        Sombra.Dispose();
    }
}

internal static class CatalogoComidas
{
    public const float TamanhoComidaPx = 90f;

    private const double PesoLixo = 25;
    private const double PesoMerda = 25;
    private const double PesoMel = 25;
    private const double PesoRefrigerante = 25;

    private static readonly Dictionary<(string, float), ComidaGerada> _cache = [];
    private static readonly object _lock = new();

    public static ComidaGerada Gerar(float escala, Random random)
    {
        double total = PesoLixo + PesoMerda + PesoMel + PesoRefrigerante;
        double sorteio = random.NextDouble() * total;

        if ((sorteio -= PesoLixo) < 0)
            return ObterPng("Lixo", escala);
        if ((sorteio -= PesoMerda) < 0)
            return ObterPng("Merda", escala);
        if ((sorteio -= PesoMel) < 0)
            return ComidasProcedurais.PocaMel(escala, random);
        return ComidasProcedurais.PocaRefrigerante(escala, random);
    }

    public static PointF SortearPonto(PointF[] regiao, Random random)
    {
        float minX = regiao.Min(p => p.X), maxX = regiao.Max(p => p.X);
        float minY = regiao.Min(p => p.Y), maxY = regiao.Max(p => p.Y);
        for (int tentativa = 0; tentativa < 40; tentativa++)
        {
            var ponto = new PointF(
                minX + (float)random.NextDouble() * (maxX - minX),
                minY + (float)random.NextDouble() * (maxY - minY));
            if (DentroDoPoligono(regiao, ponto))
                return ponto;
        }
        return new PointF(regiao.Average(p => p.X), regiao.Average(p => p.Y));
    }

    public static ComidaGerada Finalizar(Bitmap imagem, float escala, bool projetaSombra, PointF[] regiaoPouso, PointF[] origensFumaca, bool compartilhada)
    {
        int raio = Math.Max(1, (int)Math.Round(Math.Max(imagem.Width, imagem.Height) / 22.0));
        return new ComidaGerada
        {
            Imagem = imagem,
            Sombra = EfeitosImagem.SombraDesfocada(imagem, imagem.Width, imagem.Height, raio),
            MargemSombra = EfeitosImagem.MargemSombra(raio),
            ProjetaSombra = projetaSombra,
            RegiaoPouso = regiaoPouso,
            OrigensFumaca = origensFumaca,
            Compartilhada = compartilhada
        };
    }

    public static PointF[] Elipse(float cx, float cy, float rx, float ry, int pontos = 16)
    {
        var resultado = new PointF[pontos];
        for (int i = 0; i < pontos; i++)
        {
            double angulo = 2 * Math.PI * i / pontos;
            resultado[i] = new PointF(cx + (float)Math.Cos(angulo) * rx, cy + (float)Math.Sin(angulo) * ry);
        }
        return resultado;
    }

    private static bool DentroDoPoligono(PointF[] poligono, PointF ponto)
    {
        bool dentro = false;
        for (int i = 0, j = poligono.Length - 1; i < poligono.Length; j = i++)
        {
            var a = poligono[i];
            var b = poligono[j];
            if ((a.Y > ponto.Y) != (b.Y > ponto.Y)
                && ponto.X < (b.X - a.X) * (ponto.Y - a.Y) / (b.Y - a.Y) + a.X)
                dentro = !dentro;
        }
        return dentro;
    }

    private static ComidaGerada ObterPng(string nome, float escala)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue((nome, escala), out var comida))
                return comida;

            var original = (Image?)Properties.Resources.ResourceManager.GetObject(nome)
                ?? throw new ArgumentException($"Imagem {nome} não está nos resources");
            using var limpa = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(limpa))
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height));

            if (nome == "Merda")
                RemoverFumacaDesenhada(limpa);

            var recorte = AreaOcupada(limpa);
            float fator = TamanhoComidaPx * escala / Math.Max(recorte.Width, recorte.Height);
            int largura = Math.Max(1, (int)Math.Round(recorte.Width * fator));
            int altura = Math.Max(1, (int)Math.Round(recorte.Height * fator));

            var imagem = new Bitmap(largura, altura, PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(imagem))
            {
                EfeitosImagem.ConfigurarQualidade(g);
                g.Clear(Color.Transparent);
                g.DrawImage(limpa, new Rectangle(0, 0, largura, altura), recorte, GraphicsUnit.Pixel);
            }

            PointF[] regiao;
            PointF[] fumaca;
            if (nome == "Merda")
            {
                regiao = Elipse(0.5f, 0.58f, 0.3f, 0.27f);
                fumaca = [new PointF(0.38f, 0.3f), new PointF(0.54f, 0.05f), new PointF(0.7f, 0.28f)];
            }
            else
            {
                regiao = [new PointF(0.52f, 0.18f), new PointF(0.22f, 0.82f), new PointF(0.82f, 0.82f)];
                fumaca = [new PointF(0.42f, 0.22f), new PointF(0.52f, 0.06f), new PointF(0.62f, 0.22f)];
            }

            comida = Finalizar(imagem, escala, true, regiao, fumaca, true);
            _cache[(nome, escala)] = comida;
            return comida;
        }
    }

    private static void RemoverFumacaDesenhada(Bitmap bmp)
    {
        var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
        var dados = bmp.LockBits(area, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        var pixels = new byte[dados.Stride * bmp.Height];
        Marshal.Copy(dados.Scan0, pixels, 0, pixels.Length);

        int limite = bmp.Height / 2;
        for (int y = 0; y < limite; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                int i = y * dados.Stride + x * 4;
                if (pixels[i + 3] == 0)
                    continue;
                int b = pixels[i], g = pixels[i + 1], r = pixels[i + 2];
                int saturacao = Math.Max(r, Math.Max(g, b)) - Math.Min(r, Math.Min(g, b));
                if (saturacao < 25)
                    pixels[i + 3] = 0;
            }
        }

        Marshal.Copy(pixels, 0, dados.Scan0, pixels.Length);
        bmp.UnlockBits(dados);
    }

    private static Rectangle AreaOcupada(Bitmap bmp)
    {
        var area = new Rectangle(0, 0, bmp.Width, bmp.Height);
        var dados = bmp.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var pixels = new byte[dados.Stride * bmp.Height];
        Marshal.Copy(dados.Scan0, pixels, 0, pixels.Length);
        bmp.UnlockBits(dados);

        int minX = bmp.Width, minY = bmp.Height, maxX = -1, maxY = -1;
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                if (pixels[y * dados.Stride + x * 4 + 3] <= 40)
                    continue;
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }
        }
        return maxX < 0 ? area : Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
    }
}
