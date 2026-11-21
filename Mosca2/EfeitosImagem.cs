using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mosca2;

internal static class EfeitosImagem
{
    public static void ConfigurarQualidade(Graphics g)
    {
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.SmoothingMode = SmoothingMode.AntiAlias;
    }

    public static int MargemSombra(int raio) => raio * 3;

    public static Bitmap SombraDesfocada(Image original, int largura, int altura, int raio)
    {
        int margem = MargemSombra(raio);
        int larguraTotal = largura + margem * 2;
        int alturaTotal = altura + margem * 2;

        using var silhueta = new Bitmap(larguraTotal, alturaTotal, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(silhueta))
        {
            ConfigurarQualidade(g);
            g.Clear(Color.Transparent);
            g.DrawImage(original, new Rectangle(margem, margem, largura, altura));
        }

        var area = new Rectangle(0, 0, larguraTotal, alturaTotal);
        var dados = silhueta.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var pixels = new byte[dados.Stride * alturaTotal];
        Marshal.Copy(dados.Scan0, pixels, 0, pixels.Length);
        silhueta.UnlockBits(dados);

        var alfa = new float[larguraTotal * alturaTotal];
        for (int y = 0; y < alturaTotal; y++)
            for (int x = 0; x < larguraTotal; x++)
                alfa[y * larguraTotal + x] = pixels[y * dados.Stride + x * 4 + 3];

        for (int passada = 0; passada < 3; passada++)
        {
            alfa = DesfocarHorizontal(alfa, larguraTotal, alturaTotal, raio);
            alfa = DesfocarVertical(alfa, larguraTotal, alturaTotal, raio);
        }

        var sombra = new Bitmap(larguraTotal, alturaTotal, PixelFormat.Format32bppArgb);
        var dadosSombra = sombra.LockBits(area, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        var saida = new byte[dadosSombra.Stride * alturaTotal];
        for (int y = 0; y < alturaTotal; y++)
            for (int x = 0; x < larguraTotal; x++)
                saida[y * dadosSombra.Stride + x * 4 + 3] = (byte)Math.Clamp((int)Math.Round(alfa[y * larguraTotal + x]), 0, 255);
        Marshal.Copy(saida, 0, dadosSombra.Scan0, saida.Length);
        sombra.UnlockBits(dadosSombra);
        return sombra;
    }

    private static float[] DesfocarHorizontal(float[] origem, int largura, int altura, int raio)
    {
        var destino = new float[origem.Length];
        float divisor = raio * 2 + 1;
        for (int y = 0; y < altura; y++)
        {
            int linha = y * largura;
            float soma = 0;
            for (int x = -raio; x <= raio; x++)
                soma += origem[linha + Math.Clamp(x, 0, largura - 1)];
            for (int x = 0; x < largura; x++)
            {
                destino[linha + x] = soma / divisor;
                soma += origem[linha + Math.Min(x + raio + 1, largura - 1)];
                soma -= origem[linha + Math.Max(x - raio, 0)];
            }
        }
        return destino;
    }

    private static float[] DesfocarVertical(float[] origem, int largura, int altura, int raio)
    {
        var destino = new float[origem.Length];
        float divisor = raio * 2 + 1;
        for (int x = 0; x < largura; x++)
        {
            float soma = 0;
            for (int y = -raio; y <= raio; y++)
                soma += origem[Math.Clamp(y, 0, altura - 1) * largura + x];
            for (int y = 0; y < altura; y++)
            {
                destino[y * largura + x] = soma / divisor;
                soma += origem[Math.Min(y + raio + 1, altura - 1) * largura + x];
                soma -= origem[Math.Max(y - raio, 0) * largura + x];
            }
        }
        return destino;
    }
}
