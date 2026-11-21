using NAudio.Wave;

namespace Mosca2;

internal sealed class ZumbidoMosca : ISampleProvider
{
    private const float DuracaoFadeInS = 0.08f;
    private const float DuracaoFadeOutS = 0.15f;
    private const float LimiarSilencio = 0.01f;
    private const float SuavizacaoPan = 0.0005f;

    private static readonly Lazy<(float[] Amostras, int Taxa)> _som = new(Carregar);

    private readonly float[] _amostras;
    private readonly float _passoFadeIn;
    private readonly float _passoFadeOut;
    private double _posicao;
    private float _volumeAtual;
    private float _panAtual;
    private volatile float _volumeAlvo;
    private volatile float _panAlvo;
    private volatile float _taxa = 1f;
    private volatile bool _tocando;

    public ZumbidoMosca(float tom)
    {
        var som = _som.Value;
        _amostras = som.Amostras;
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(som.Taxa, 2);
        Tom = tom;
        _passoFadeIn = 1f / (som.Taxa * DuracaoFadeInS);
        _passoFadeOut = 1f / (som.Taxa * DuracaoFadeOutS);
        _posicao = Random.Shared.Next(_amostras.Length);
    }

    public WaveFormat WaveFormat { get; }

    public float Tom { get; }

    public bool Tocando => _tocando;

    public float Volume
    {
        set => _volumeAlvo = Math.Clamp(value, 0f, 1f);
    }

    public float Pan
    {
        set => _panAlvo = Math.Clamp(value, -1f, 1f);
    }

    public float Taxa
    {
        set => _taxa = Math.Clamp(value, 0.5f, 2f);
    }

    public void Iniciar()
    {
        if (_volumeAtual <= 0f)
            _panAtual = _panAlvo;
        _tocando = true;
    }

    public void Parar()
    {
        _tocando = false;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        if (!_tocando && _volumeAtual <= 0f)
            return 0;

        int quadros = count / 2;
        int total = _amostras.Length;
        float alvo = _tocando ? _volumeAlvo : 0f;
        float panAlvo = _panAlvo;
        double passo = Tom * _taxa;

        for (int i = 0; i < quadros; i++)
        {
            if (_volumeAtual < alvo)
                _volumeAtual = Math.Min(alvo, _volumeAtual + _passoFadeIn);
            else if (_volumeAtual > alvo)
                _volumeAtual = Math.Max(alvo, _volumeAtual - _passoFadeOut);
            _panAtual += (panAlvo - _panAtual) * SuavizacaoPan;

            int indice = (int)_posicao;
            float fracao = (float)(_posicao - indice);
            float a = _amostras[indice];
            float b = _amostras[(indice + 1) % total];
            float amostra = (a + (b - a) * fracao) * _volumeAtual;

            double angulo = (_panAtual + 1) * Math.PI / 4;
            buffer[offset + i * 2] = amostra * (float)Math.Cos(angulo);
            buffer[offset + i * 2 + 1] = amostra * (float)Math.Sin(angulo);

            _posicao += passo;
            if (_posicao >= total)
                _posicao -= total;
        }

        return quadros * 2;
    }

    private static (float[] Amostras, int Taxa) Carregar()
    {
        using var mp3 = new MemoryStream();
        Properties.Resources.Som_de_Mosca.CopyTo(mp3);
        mp3.Position = 0;

        using var leitor = new Mp3FileReader(mp3);
        var fonte = leitor.ToSampleProvider();
        int canais = fonte.WaveFormat.Channels;
        var mono = new List<float>();
        var bloco = new float[fonte.WaveFormat.SampleRate * canais];
        int lidos;
        while ((lidos = fonte.Read(bloco, 0, bloco.Length)) > 0)
        {
            for (int i = 0; i + canais <= lidos; i += canais)
            {
                float soma = 0;
                for (int c = 0; c < canais; c++)
                    soma += bloco[i + c];
                mono.Add(soma / canais);
            }
        }

        int inicio = mono.FindIndex(s => Math.Abs(s) > LimiarSilencio);
        int fim = mono.FindLastIndex(s => Math.Abs(s) > LimiarSilencio);
        float[] amostras = inicio >= 0 && fim > inicio
            ? mono.GetRange(inicio, fim - inicio + 1).ToArray()
            : mono.ToArray();
        if (amostras.Length < 2)
            amostras = new float[fonte.WaveFormat.SampleRate];

        return (amostras, fonte.WaveFormat.SampleRate);
    }
}
