using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Mosca2;

internal readonly record struct EstadoVisualMosca(int IndiceSprite, float Angulo, float Altura, float Escala, float EscalaComprimento, float IntensidadeAsas, string? TextoIndice);

internal sealed class RenderizadorMosca : IDisposable
{
    private readonly SpritesMosca.Conjunto _sprites;
    private readonly Bitmap _quadro;
    private readonly Font _fonteIndice;
    private readonly SolidBrush _pincelIndice;
    private readonly ImageAttributes _atributosSombra = new();
    private readonly ColorMatrix _matrizSombra = new();
    private readonly ImageAttributes _atributosCorpo = new();
    private readonly ColorMatrix _matrizCorpo = new();
    private readonly Random _random = new();
    private readonly float _escala;
    private const float RaizAsasY = -0.08f;
    private const float ComprimentoAsas = 0.42f;
    private const float AberturaAsasGraus = 95f;
    private const float AberturaAsasMinGraus = 25f;
    private const float VariacaoAberturaGraus = 10f;
    private const float AlfaBorraoAsas = 0.35f;

    public RenderizadorMosca(int tamanhoSprite, int lado, float escala)
    {
        _sprites = SpritesMosca.Obter(tamanhoSprite);
        _quadro = new Bitmap(lado, lado, PixelFormat.Format32bppPArgb);
        _fonteIndice = new Font("Segoe UI", 9F * escala, FontStyle.Bold, GraphicsUnit.Pixel);
        _pincelIndice = new SolidBrush(Color.OrangeRed);
        _escala = escala;
        Lado = lado;
    }

    public int Lado { get; }

    public int TamanhoSprite => _sprites.Tamanho;

    public Bitmap Desenhar(EstadoVisualMosca estado)
    {
        int indice = Math.Clamp(estado.IndiceSprite, 0, _sprites.Corpos.Length - 1);
        float centro = Lado / 2f;
        float tamanho = _sprites.Tamanho;
        float tamanhoCorpo = tamanho * estado.Escala;

        using var g = Graphics.FromImage(_quadro);
        g.Clear(Color.Transparent);
        g.CompositingMode = CompositingMode.SourceOver;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        DesenharSombra(g, indice, estado, centro);

        g.TranslateTransform(centro, centro);
        g.RotateTransform(estado.Angulo);
        float comprimentoCorpo = tamanhoCorpo * estado.EscalaComprimento;
        var destinoCorpo = new RectangleF(-tamanhoCorpo / 2f, -comprimentoCorpo / 2f, tamanhoCorpo, comprimentoCorpo);
        float asas = Math.Clamp(estado.IntensidadeAsas, 0f, 1f);
        if (asas <= 0f)
        {
            g.DrawImage(_sprites.Corpos[indice], destinoCorpo);
        }
        else
        {
            g.DrawImage(_sprites.CorposSemAsas[indice], destinoCorpo);
            if (asas < 1f)
                DesenharComOpacidade(g, _sprites.Corpos[indice], destinoCorpo, 1f - asas);
            DesenharBorraoAsas(g, tamanhoCorpo, asas);
        }
        g.ResetTransform();

        if (estado.TextoIndice != null)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            float x = centro - tamanho / 2f + 37 * _escala;
            float y = centro - tamanho / 2f;
            g.DrawString(estado.TextoIndice, _fonteIndice, _pincelIndice, x, y);
        }

        return _quadro;
    }

    private void DesenharSombra(Graphics g, int indice, EstadoVisualMosca estado, float centro)
    {
        float altura = Math.Clamp(estado.Altura, 0f, 1f);
        float deslocamento = (2f + 10f * altura) * _escala;
        _matrizSombra.Matrix33 = 0.45f - 0.25f * altura;
        _atributosSombra.SetColorMatrix(_matrizSombra);

        var sombra = _sprites.Sombras[indice];
        float escalaSombra = estado.Escala * (1f + 0.12f * altura);
        float largura = sombra.Width * escalaSombra;
        float alturaSombra = sombra.Height * escalaSombra;
        g.TranslateTransform(centro + deslocamento, centro + deslocamento);
        g.RotateTransform(estado.Angulo);
        g.DrawImage(sombra,
            [new PointF(-largura / 2f, -alturaSombra / 2f), new PointF(largura / 2f, -alturaSombra / 2f), new PointF(-largura / 2f, alturaSombra / 2f)],
            new RectangleF(0, 0, sombra.Width, sombra.Height), GraphicsUnit.Pixel, _atributosSombra);
        g.ResetTransform();
    }

    private void DesenharComOpacidade(Graphics g, Bitmap imagem, RectangleF destino, float opacidade)
    {
        _matrizCorpo.Matrix33 = opacidade;
        _atributosCorpo.SetColorMatrix(_matrizCorpo);
        g.DrawImage(imagem,
            [new PointF(destino.Left, destino.Top), new PointF(destino.Right, destino.Top), new PointF(destino.Left, destino.Bottom)],
            new RectangleF(0, 0, imagem.Width, imagem.Height), GraphicsUnit.Pixel, _atributosCorpo);
    }

    private void DesenharBorraoAsas(Graphics g, float tamanhoCorpo, float intensidade)
    {
        float raio = tamanhoCorpo * ComprimentoAsas;
        var raiz = new PointF(0, tamanhoCorpo * RaizAsasY);
        var caixa = new RectangleF(raiz.X - raio, raiz.Y - raio, raio * 2, raio * 2);
        int alfa = (int)(255 * AlfaBorraoAsas * intensidade);

        foreach (int lado in new[] { 1, -1 })
        {
            float abertura = AberturaAsasGraus + (float)((_random.NextDouble() * 2 - 1) * VariacaoAberturaGraus);
            float varredura = abertura - AberturaAsasMinGraus;
            // Ângulos do GDI+: 90° aponta para trás (+Y local); lado 1 abre para a direita, -1 para a esquerda
            float inicio = lado == 1 ? 90f - abertura : 90f + AberturaAsasMinGraus;

            using var caminho = new GraphicsPath();
            caminho.AddPie(caixa.X, caixa.Y, caixa.Width, caixa.Height, inicio, varredura);
            using var pincel = new PathGradientBrush(caminho)
            {
                CenterPoint = raiz,
                CenterColor = Color.FromArgb(alfa, 225, 230, 235),
                SurroundColors = [Color.FromArgb(0, 225, 230, 235)]
            };
            g.FillPath(pincel, caminho);
        }
    }

    public void Dispose()
    {
        _quadro.Dispose();
        _fonteIndice.Dispose();
        _pincelIndice.Dispose();
        _atributosSombra.Dispose();
        _atributosCorpo.Dispose();
    }
}
