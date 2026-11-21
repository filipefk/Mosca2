using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Mosca2;

internal static class MixerZumbido
{
    private const int LatenciaMs = 120;

    private static readonly object _trava = new();
    private static MixingSampleProvider? _mixer;
    private static WaveOutEvent? _saida;

    public static void Adicionar(ZumbidoMosca zumbido)
    {
        MixingSampleProvider mixer;
        lock (_trava)
        {
            if (_mixer == null)
            {
                _mixer = new MixingSampleProvider(zumbido.WaveFormat) { ReadFully = true };
                _saida = new WaveOutEvent { DesiredLatency = LatenciaMs };
                _saida.Init(new LimitadorSuave(_mixer));
                _saida.Play();
                Application.ApplicationExit += (s, e) => Encerrar();
            }
            mixer = _mixer;
        }
        mixer.AddMixerInput(zumbido);
    }

    public static void Remover(ZumbidoMosca zumbido)
    {
        _mixer?.RemoveMixerInput(zumbido);
    }

    private static void Encerrar()
    {
        lock (_trava)
        {
            _saida?.Stop();
            _saida?.Dispose();
            _saida = null;
            _mixer = null;
        }
    }

    private sealed class LimitadorSuave(ISampleProvider fonte) : ISampleProvider
    {
        public WaveFormat WaveFormat => fonte.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int lidos = fonte.Read(buffer, offset, count);
            for (int i = offset; i < offset + lidos; i++)
                buffer[i] = MathF.Tanh(buffer[i]);
            return lidos;
        }
    }
}
