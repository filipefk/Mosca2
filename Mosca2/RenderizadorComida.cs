using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mosca2;

internal sealed class RenderizadorComida : IDisposable
{
    private const int SegmentosFumaca = 18;
    private const float AlfaFumaca = 0.35f;

    private readonly ComidaGerada _comida;
    private readonly Bitmap _quadro;
    private readonly ImageAttributes _atributosSombra = new();
    private readonly ColorMatrix _matrizSombra = new();
    private readonly float _escala;
    private readonly double[] _fasesFumaca;

    public RenderizadorComida(ComidaGerada comida, float escala, Random random)
    {
        _comida = comida;
        _escala = escala;
        _fasesFumaca = comida.OrigensFumaca.Select(_ => random.NextDouble() * Math.PI * 2).ToArray();

        int largura = comida.Imagem.Width;
        int altura = comida.Imagem.Height;
        int margem = (int)Math.Ceiling(Math.Max(largura, altura) * 0.12f) + comida.MargemSombra + (int)Math.Ceiling(10 * escala);
        int alturaFumaca = comida.OrigensFumaca.Length > 0 ? (int)Math.Ceiling(altura * 0.9f + 10 * escala) : 0;

        AreaComida = new Rectangle(margem, margem + alturaFumaca, largura, altura);
        TamanhoJanela = new Size(largura + margem * 2, altura + margem * 2 + alturaFumaca);
        _quadro = new Bitmap(TamanhoJanela.Width, TamanhoJanela.Height, PixelFormat.Format32bppPArgb);
    }

    public Rectangle AreaComida { get; }

    public Size TamanhoJanela { get; }

    public Bitmap Desenhar(float escalaCorpo, float altura, double tempo)
    {
        using var g = Graphics.FromImage(_quadro);
        g.Clear(Color.Transparent);
        EfeitosImagem.ConfigurarQualidade(g);
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;

        float alt = Math.Clamp(altura, 0f, 1f);
        float cx = AreaComida.Left + AreaComida.Width / 2f;
        float cy = AreaComida.Top + AreaComida.Height / 2f;

        DesenharSombra(g, cx, cy, escalaCorpo, alt);

        float largura = AreaComida.Width * escalaCorpo;
        float alturaCorpo = AreaComida.Height * escalaCorpo;
        g.DrawImage(_comida.Imagem, new RectangleF(cx - largura / 2f, cy - alturaCorpo / 2f, largura, alturaCorpo));

        for (int i = 0; i < _comida.OrigensFumaca.Length; i++)
        {
            var origem = _comida.OrigensFumaca[i];
            var base_ = new PointF(cx + (origem.X - 0.5f) * largura, cy + (origem.Y - 0.5f) * alturaCorpo);
            DesenharFumaca(g, base_, tempo, _fasesFumaca[i]);
        }

        return _quadro;
    }

    private void DesenharSombra(Graphics g, float cx, float cy, float escalaCorpo, float altura)
    {
        float opacidade = _comida.ProjetaSombra ? 0.4f - 0.18f * altura : 0.25f * altura;
        if (opacidade <= 0.005f)
            return;

        _matrizSombra.Matrix33 = opacidade;
        _atributosSombra.SetColorMatrix(_matrizSombra);

        var sombra = _comida.Sombra;
        float escalaSombra = escalaCorpo * (1f + 0.1f * altura);
        float largura = sombra.Width * escalaSombra;
        float alturaSombra = sombra.Height * escalaSombra;
        float deslocamento = (2f + 8f * altura) * _escala;
        float x = cx + deslocamento - largura / 2f;
        float y = cy + deslocamento - alturaSombra / 2f;

        g.DrawImage(sombra,
            [new PointF(x, y), new PointF(x + largura, y), new PointF(x, y + alturaSombra)],
            new RectangleF(0, 0, sombra.Width, sombra.Height), GraphicsUnit.Pixel, _atributosSombra);
    }

    private void DesenharFumaca(Graphics g, PointF base_, double tempo, double fase)
    {
        float comprimento = base_.Y - 4 * _escala;
        if (comprimento <= 0)
            return;

        double pulso = 0.6 + 0.4 * Math.Sin(tempo * 1.3 + fase);
        PointF anterior = default;
        for (int s = 0; s <= SegmentosFumaca; s++)
        {
            double f = (double)s / SegmentosFumaca;
            double amplitude = (2 + 7 * f) * _escala;
            double deriva = f * 4 * _escala * Math.Sin(tempo * 0.7 + fase);
            var ponto = new PointF(
                base_.X + (float)(amplitude * Math.Sin(f * 5.5 - tempo * 2.2 + fase) + deriva),
                base_.Y - (float)(f * comprimento));

            if (s > 0)
            {
                double entrada = Math.Min(1, f / 0.15);
                double saida = Math.Pow(1 - f, 1.3);
                double alfa = 255 * AlfaFumaca * pulso * entrada * saida;
                using var halo = new Pen(Color.FromArgb((int)(alfa * 0.35), 215, 215, 205), 4f * _escala) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                using var nucleo = new Pen(Color.FromArgb((int)alfa, 215, 215, 205), 1.6f * _escala) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(halo, anterior, ponto);
                g.DrawLine(nucleo, anterior, ponto);
            }
            anterior = ponto;
        }
    }

    public void Dispose()
    {
        _quadro.Dispose();
        _atributosSombra.Dispose();
    }
}
