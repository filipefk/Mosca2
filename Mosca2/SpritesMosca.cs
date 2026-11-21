using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mosca2;

internal static class SpritesMosca
{
    private static readonly string[] _nomes = ["Mosca1", "Mosca2", "Mosca3", "Mosca4", "Mosca5"];
    private static readonly Dictionary<int, Conjunto> _cache = [];
    private static readonly object _lock = new();

    public static int Quantidade => _nomes.Length;

    public sealed class Conjunto
    {
        public required Bitmap[] Corpos { get; init; }
        public required Bitmap[] CorposSemAsas { get; init; }
        public required Bitmap[] Sombras { get; init; }
        public required int Tamanho { get; init; }
    }

    public static Conjunto Obter(int tamanho)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(tamanho, out var conjunto))
                return conjunto;

            int raioDesfoque = Math.Max(1, (int)Math.Round(tamanho / 15.0));
            var corpos = new Bitmap[_nomes.Length];
            var corposSemAsas = new Bitmap[_nomes.Length];
            var sombras = new Bitmap[_nomes.Length];
            for (int i = 0; i < _nomes.Length; i++)
            {
                var original = (Image?)Properties.Resources.ResourceManager.GetObject(_nomes[i])
                    ?? throw new ArgumentException($"Imagem {_nomes[i]} não está nos resources");
                corpos[i] = Reduzir(original, tamanho);
                corposSemAsas[i] = AtenuarAsas(corpos[i]);
                sombras[i] = EfeitosImagem.SombraDesfocada(original, tamanho, tamanho, raioDesfoque);
            }

            conjunto = new Conjunto { Corpos = corpos, CorposSemAsas = corposSemAsas, Sombras = sombras, Tamanho = tamanho };
            _cache[tamanho] = conjunto;
            return conjunto;
        }
    }

    private static Bitmap Reduzir(Image original, int tamanho)
    {
        var bmp = new Bitmap(tamanho, tamanho, PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(bmp);
        EfeitosImagem.ConfigurarQualidade(g);
        g.Clear(Color.Transparent);
        g.DrawImage(original, new Rectangle(0, 0, tamanho, tamanho));
        return bmp;
    }

    private static Bitmap AtenuarAsas(Bitmap corpo)
    {
        const double InicioAsas = 0.42;
        const double FatorAlfaAsas = 0.2;

        var area = new Rectangle(0, 0, corpo.Width, corpo.Height);
        using var copia = corpo.Clone(area, PixelFormat.Format32bppArgb);
        var dados = copia.LockBits(area, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        var pixels = new byte[dados.Stride * corpo.Height];
        Marshal.Copy(dados.Scan0, pixels, 0, pixels.Length);

        int linhaInicial = (int)(corpo.Height * InicioAsas);
        for (int y = linhaInicial; y < corpo.Height; y++)
        {
            for (int x = 0; x < corpo.Width; x++)
            {
                int i = y * dados.Stride + x * 4;
                byte alfa = pixels[i + 3];
                if (alfa == 0)
                    continue;
                int luminancia = (pixels[i] + pixels[i + 1] + pixels[i + 2]) / 3;
                bool corpoEscuro = alfa >= 230 && luminancia < 110;
                if (!corpoEscuro)
                    pixels[i + 3] = (byte)(alfa * FatorAlfaAsas);
            }
        }

        Marshal.Copy(pixels, 0, dados.Scan0, pixels.Length);
        copia.UnlockBits(dados);
        return copia.Clone(area, PixelFormat.Format32bppPArgb);
    }
}
