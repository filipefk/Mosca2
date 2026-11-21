using System.Diagnostics;

namespace Mosca2;

public partial class frmComida : JanelaCamada
{
    private const double DuracaoEntradaS = 0.3;
    private const double DuracaoSaidaS = 2.0;
    private const double TransicaoArrastarS = 0.1;
    private const float EscalaInicial = 0.9f;
    private const float EscalaArrastando = 1.06f;
    private const int QuadroMs = 33;

    private readonly System.Windows.Forms.Timer _timerAnimacao;
    private readonly Random _random = new();
    private readonly Stopwatch _relogio = new();
    private float _escala;
    private ComidaGerada? _comida;
    private RenderizadorComida? _renderizador;
    private Point _janela;
    private double _duracaoS;
    private double _ultimoQuadro;
    private float _altura;
    private Point _dragOffset;
    private bool _dragging = false;

    public event EventHandler? ComecouArrastar;

    public frmComida()
    {
        InitializeComponent();
        _escala = DeviceDpi / 96f;
        this.ShowInTaskbar = false;

        _timerAnimacao = new System.Windows.Forms.Timer { Interval = QuadroMs };
        _timerAnimacao.Tick += TimerAnimacao_Tick;

        this.MouseDown += Comida_MouseDown;
        this.MouseMove += Comida_MouseMove;
        this.MouseUp += Comida_MouseUp;
    }

    private void MoscasTopMost()
    {
        foreach (Form mosca in Application.OpenForms.OfType<frmMosca>())
        {
            mosca.TopMost = false;
            mosca.TopMost = true;
        }
    }

    public Task Mostrar()
    {
        // Sorteia a tela e usa a escala do monitor escolhido
        var screens = Screen.AllScreens;
        var area = screens[_random.Next(screens.Length)].WorkingArea;
        _escala = DpiNoPonto(new Point(area.Left + area.Width / 2, area.Top + area.Height / 2)) / 96f;

        _comida = CatalogoComidas.Gerar(_escala, _random);
        _renderizador = new RenderizadorComida(_comida, _escala, _random);
        this.ClientSize = _renderizador.TamanhoJanela;

        // Sorteia tempo de exibição
        _duracaoS = _random.Next(10, 31);

        // Sorteia posição na tela escolhida, mantendo a comida a 20px das bordas
        var comida = _renderizador.AreaComida;
        int margem = 20;
        int minX = area.Left + margem - comida.Left;
        int maxX = area.Right - margem - comida.Right;
        int minY = area.Top + margem - comida.Top;
        int maxY = area.Bottom - margem - comida.Bottom;
        int x = _random.Next(minX, Math.Max(minX + 1, maxX));
        int y = _random.Next(minY, Math.Max(minY + 1, maxY));
        _janela = new Point(x, y);
        this.StartPosition = FormStartPosition.Manual;
        this.Location = _janela;

        _relogio.Restart();
        _ultimoQuadro = 0;
        this.Show();
        this.TopMost = true;
        Renderizar();
        _timerAnimacao.Start();

        MoscasTopMost();
        return Task.CompletedTask;
    }

    public Point PontoDePouso()
    {
        if (_comida == null || _renderizador == null)
            return new Point(_janela.X + Width / 2, _janela.Y + Height / 2);

        var area = _renderizador.AreaComida;
        var ponto = CatalogoComidas.SortearPonto(_comida.RegiaoPouso, _random);
        return new Point(
            _janela.X + area.Left + (int)Math.Round(ponto.X * area.Width),
            _janela.Y + area.Top + (int)Math.Round(ponto.Y * area.Height));
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Renderizar();
    }

    private void TimerAnimacao_Tick(object? sender, EventArgs e)
    {
        double agora = _relogio.Elapsed.TotalSeconds;
        double dt = Math.Clamp(agora - _ultimoQuadro, 0, 0.1);
        _ultimoQuadro = agora;

        if (agora >= _duracaoS && !_dragging)
        {
            _timerAnimacao.Stop();
            this.Close();
            return;
        }

        float alvo = _dragging ? 1f : 0f;
        float passo = (float)(dt / TransicaoArrastarS);
        _altura = _altura < alvo ? Math.Min(alvo, _altura + passo) : Math.Max(alvo, _altura - passo);

        Renderizar();
    }

    private void Renderizar()
    {
        if (_renderizador == null || IsDisposed || !IsHandleCreated)
            return;

        double agora = _relogio.Elapsed.TotalSeconds;
        double entrada = Math.Clamp(agora / DuracaoEntradaS, 0, 1);
        double entradaSuave = 1 - (1 - entrada) * (1 - entrada);
        double saida = _dragging ? 1 : Math.Clamp((_duracaoS - agora) / DuracaoSaidaS, 0, 1);
        double opacidade = Math.Min(entradaSuave, saida);

        float escalaCorpo = (float)(EscalaInicial + (1 - EscalaInicial) * entradaSuave) * (1f + (EscalaArrastando - 1f) * _altura);
        var quadro = _renderizador.Desenhar(escalaCorpo, _altura, agora);
        AtualizarCamada(quadro, _janela, (byte)Math.Round(255 * opacidade));
    }

    private void Comida_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragging = true;
            _dragOffset = new Point(e.X, e.Y);
            ComecouArrastar?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Comida_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_dragging)
        {
            var mousePos = MousePosition;
            _janela = new Point(mousePos.X - _dragOffset.X, mousePos.Y - _dragOffset.Y);
            Renderizar();
        }
    }

    private void Comida_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragging = false;
            double agora = _relogio.Elapsed.TotalSeconds;
            if (_duracaoS - agora < DuracaoSaidaS)
                _duracaoS = agora + DuracaoSaidaS;
            MoscasTopMost();
        }
    }

    private void LiberarRecursos()
    {
        _timerAnimacao.Stop();
        _timerAnimacao.Dispose();
        _renderizador?.Dispose();
        _renderizador = null;
        _comida?.Liberar();
        _comida = null;
    }
}
