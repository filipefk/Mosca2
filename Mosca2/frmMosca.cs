using NAudio.Wave;
using System.Diagnostics;
using System.Numerics;

namespace Mosca2;

public partial class frmMosca : JanelaCamada
{
    private readonly System.Windows.Forms.Timer _timerDecisao;
    private bool _permitirPernas = true;
    private bool _permitirRotacao = true;
    private bool _permitirCaminhar = true;
    private bool _permitirVoo = true;
    private bool _executandoComportamento = false;
    private int _geracao = 0;
    private double _proximoVooEm = 0;
    private float _escalaComprimento = 1f;
    private const double PesoPousada = 45;
    private const double PesoLimpando = 20;
    private const double PesoAndando = 20;
    private const double PesoVirando = 15;
    private const double VelocidadeAndarPxS = 40;
    private const double PassoTrocaPernaPx = 4;
    private const int RajadasLimparMin = 1;
    private const int RajadasLimparMax = 2;
    private const int TrocasPorRajadaMin = 2;
    private const int TrocasPorRajadaMax = 4;
    private const double IntervaloTrocaLimparMinS = 0.08;
    private const double IntervaloTrocaLimparMaxS = 0.13;
    private const double PausaEntreRajadasMinS = 0.3;
    private const double PausaEntreRajadasMaxS = 0.7;
    private const double VelocidadeGiroRepousoGrausS = 180;
    private static readonly Random _random = new();
    private float _escala;
    private int _tamanho;
    private int _margem;
    private int _dpiAtual;
    private RenderizadorMosca _renderizador;
    private float _anguloAtual = 0;
    private int _indiceSprite = 0;
    private float _altura = 0f;
    private Point _tremor = Point.Empty;
    private static readonly Stopwatch _relogio = Stopwatch.StartNew();
    private const int DuracaoDecolagemMs = 80;
    private const int DuracaoDecolagemFugaMs = 30;
    private const int QuadrosTremorDecolagem = 3;
    private const int QuadrosTremorFuga = 1;
    private const int DuracaoPousoMs = 80;
    private const float EscalaVoo = 0.08f;
    private const float AmplitudeOscilacaoVoo = 0.015f;
    private const double FrequenciaOscilacaoVoo = 6.0;
    private const double FatorVelocidade = 64.0;
    private const double TempoAceleracaoS = 0.12;
    private const double TaxaGiroGrausS = 900;
    private const double DesvioRuidoMaxGraus = 35;
    private const double IntervaloRajadaMinS = 0.12;
    private const double IntervaloRajadaMaxS = 0.4;
    private const double ChanceGuinada = 0.5;
    private const double ChancePairar = 0.12;
    private const int QuadroMs = 16;
    private const float RaioPanicoPx = 15;
    private const float RaioPercepcaoPx = 120;
    private const float VelocidadeAmeacaPxS = 600;
    private const float SuavizacaoVelocidadeCursor = 0.3f;
    private const double DesvioFugaGraus = 35;
    private const double DistanciaFugaMinPx = 150;
    private const double DistanciaFugaMaxPx = 450;
    private const double ChanceFugaLonge = 0.2;
    private Point _ultimoCursor;
    private double _ultimoCursorEm = -1;
    private Vector2 _velocidadeCursor = Vector2.Zero;
    private Point _janela;
    private Point _destinoVoar;
    private bool _emVoo = false;
    private WaveOutEvent? _waveOut;
    private ZumbidoMosca? _zumbido;
    private frmComida? _comidaAtual;
    private float _volumeBase = 1f;
    private const float VolumeMinimoVoo = 0.35f;
    private const float VariacaoTomVelocidade = 0.06f;
    private bool _dragging = false;
    private Point _dragOffset;
    private bool _mostrarIndice = false;

    public enum Direcao
    {
        Cima,
        Baixo,
        Esquerda,
        Direita
    }

    public enum ComportamentoMouseEnum
    {
        FugirMouse,
        SeguirMouse,
        IgnorarMouse
    }

    public frmMosca()
    {
        InitializeComponent();

        _renderizador = CriarRenderizador(DeviceDpi);
        this.ClientSize = new Size(_renderizador.Lado, _renderizador.Lado);

        this.ShowInTaskbar = false;
        this.TopMost = true;

        _timerDecisao = new System.Windows.Forms.Timer();
        _timerDecisao.Interval = 100;
        _timerDecisao.Tick += TimerDecisao_Tick;
        ProximoIntervaloVoar();
        _timerDecisao.Start();

        this.MouseEnter += Mosca_MouseEnter;
        this.MouseWheel += Mosca_MouseWheel;
        this.MouseClick += Mosca_MouseClick;
        this.MouseDown += Mosca_MouseDown;
        this.MouseMove += Mosca_MouseMove;
        this.MouseUp += Mosca_MouseUp;

        _zumbido = new ZumbidoMosca(0.95f + (float)_random.NextDouble() * 0.1f);
        _waveOut = new WaveOutEvent { DesiredLatency = 120 };
        _waveOut.Init(_zumbido);
        _waveOut.PlaybackStopped += WaveOut_PlaybackStopped;

        this.StartPosition = FormStartPosition.Manual;
        _janela = ForaDeTodasTelasAleatorio();
        this.Location = _janela;
    }

    public ComportamentoMouseEnum ComportamentoMouse { get; set; } = ComportamentoMouseEnum.FugirMouse;
    public bool ComSom { get; set; } = false;

    public bool MostrarIndice
    {
        get { return _mostrarIndice; }
        set
        {
            _mostrarIndice = value;
            Renderizar();
        }
    }

    public bool PermitirAgarrarSoltar { get; set; } = false;
    public int Indice { get; set; } = -1;

    public Point PosicaoAtual { get => Origem; }

    public uint VelocidadePadrao { get; set; } = 0;

    private Point Origem => new Point(_janela.X + _margem, _janela.Y + _margem);

    private Point CursorRelativo => new Point((int)Math.Round(-23 * _escala), (int)Math.Round(13 * _escala));

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        AjustarEscalaAoMonitor();
        Renderizar();
    }

    private void Renderizar()
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(Renderizar));
            return;
        }
        float oscilacao = _altura > 0f
            ? (float)Math.Sin(_relogio.Elapsed.TotalSeconds * Math.PI * 2 * FrequenciaOscilacaoVoo) * AmplitudeOscilacaoVoo * _altura
            : 0f;
        float escalaCorpo = 1f + EscalaVoo * _altura + oscilacao;
        var estado = new EstadoVisualMosca(_indiceSprite, _anguloAtual, _altura, escalaCorpo, _escalaComprimento, _altura, _mostrarIndice ? Indice.ToString() : null);
        AtualizarCamada(_renderizador.Desenhar(estado), new Point(_janela.X + _tremor.X, _janela.Y + _tremor.Y));
    }

    private async Task Decolar(bool fuga = false)
    {
        int amplitude = Math.Max(1, (int)Math.Round(1.5 * _escala));
        int quadrosTremor = fuga ? QuadrosTremorFuga : QuadrosTremorDecolagem;
        for (int i = 0; i < quadrosTremor; i++)
        {
            _tremor = new Point(_random.Next(-amplitude, amplitude + 1), _random.Next(-amplitude, amplitude + 1));
            Renderizar();
            await Task.Delay(16);
        }
        _tremor = Point.Empty;
        await AnimarAltura(1f, fuga ? DuracaoDecolagemFugaMs : DuracaoDecolagemMs);
    }

    private async Task Pousar()
    {
        await AnimarAltura(0f, DuracaoPousoMs);
    }

    private async Task AnimarAltura(float alturaDestino, int duracaoMs)
    {
        float alturaInicial = _altura;
        var inicio = _relogio.Elapsed;
        while (true)
        {
            double t = Math.Min(1.0, (_relogio.Elapsed - inicio).TotalMilliseconds / duracaoMs);
            double suavizado = 1 - (1 - t) * (1 - t);
            _altura = (float)(alturaInicial + (alturaDestino - alturaInicial) * suavizado);
            Renderizar();
            if (t >= 1.0)
                break;
            await Task.Delay(8);
        }
    }

    private void PosicionarOrigem(Point origem)
    {
        PosicionarJanela(new Point(origem.X - _margem, origem.Y - _margem));
    }

    private void PosicionarJanela(Point janela)
    {
        _janela = janela;
        AjustarEscalaAoMonitor();
        Renderizar();
    }

    private RenderizadorMosca CriarRenderizador(int dpi)
    {
        _dpiAtual = dpi;
        _escala = dpi / 96f;
        _tamanho = (int)Math.Round(45 * _escala);
        _margem = (int)Math.Ceiling(_tamanho * 0.6);
        return new RenderizadorMosca(_tamanho, _tamanho + _margem * 2, _escala);
    }

    private void AjustarEscalaAoMonitor()
    {
        var centro = CentroMosca;
        int dpi = DpiNoPonto(centro);
        if (dpi == _dpiAtual)
            return;

        var anterior = _renderizador;
        _renderizador = CriarRenderizador(dpi);
        anterior.Dispose();
        _janela = new Point(centro.X - _tamanho / 2 - _margem, centro.Y - _tamanho / 2 - _margem);
    }

    private void LevarCursor()
    {
        var origem = Origem;
        var relativo = CursorRelativo;
        Cursor.Position = new Point(origem.X + relativo.X, origem.Y + relativo.Y);
    }

    private Point OrigemParaCursorEm(Point cursor)
    {
        var relativo = CursorRelativo;
        return new Point(cursor.X - relativo.X, cursor.Y - relativo.Y);
    }

    private async void Mosca_MouseEnter(object? sender, EventArgs e)
    {
        if (ComportamentoMouse == ComportamentoMouseEnum.SeguirMouse
            || ComportamentoMouse == ComportamentoMouseEnum.IgnorarMouse
            || _emVoo)
            return;
        await Fugir();
    }

    private async void Mosca_MouseClick(object? sender, EventArgs e)
    {
        if (_emVoo) return;
        if (!PermitirAgarrarSoltar && ComportamentoMouse != ComportamentoMouseEnum.IgnorarMouse)
            await Fugir();
    }

    public async Task Fugir()
    {
        await VoarParaDestino(DestinoFuga(Cursor.Position), 0, false, true);
    }

    public void CursorMoveu(Point cursor)
    {
        double agora = _relogio.Elapsed.TotalSeconds;
        double dt = agora - _ultimoCursorEm;
        if (_ultimoCursorEm >= 0 && dt > 0 && dt < 0.1)
        {
            var instantanea = new Vector2((cursor.X - _ultimoCursor.X) / (float)dt, (cursor.Y - _ultimoCursor.Y) / (float)dt);
            _velocidadeCursor = Vector2.Lerp(_velocidadeCursor, instantanea, SuavizacaoVelocidadeCursor);
        }
        else
        {
            _velocidadeCursor = Vector2.Zero;
        }
        _ultimoCursor = cursor;
        _ultimoCursorEm = agora;

        if (ComportamentoMouse != ComportamentoMouseEnum.FugirMouse || _emVoo || _dragging || IsDisposed)
            return;

        var centro = CentroMosca;
        var paraMosca = new Vector2(centro.X - cursor.X, centro.Y - cursor.Y);
        float distancia = paraMosca.Length();
        float raioCorpo = _tamanho / 2f;

        bool pertoDemais = distancia < raioCorpo + RaioPanicoPx * _escala;
        bool aproximandoRapido = false;
        if (distancia < raioCorpo + RaioPercepcaoPx * _escala && distancia > 0)
        {
            float aproximacao = Vector2.Dot(_velocidadeCursor, paraMosca / distancia);
            aproximandoRapido = aproximacao > VelocidadeAmeacaPxS * _escala;
        }

        if (pertoDemais || aproximandoRapido)
            _ = VoarParaDestino(DestinoFuga(cursor), 0, false, true);
    }

    private Point CentroMosca
    {
        get
        {
            var origem = Origem;
            return new Point(origem.X + _tamanho / 2, origem.Y + _tamanho / 2);
        }
    }

    private Point DestinoFuga(Point cursor)
    {
        if (_random.NextDouble() < ChanceFugaLonge)
            return DestinoVoarAleatorio();

        var centro = CentroMosca;
        double anguloBase = Math.Atan2(centro.Y - cursor.Y, centro.X - cursor.X);
        if (centro == cursor)
            anguloBase = _random.NextDouble() * Math.PI * 2;

        var tela = Screen.FromPoint(centro).WorkingArea;
        int margem = 10;
        for (int tentativa = 0; tentativa < 6; tentativa++)
        {
            double desvio = (_random.NextDouble() * 2 - 1) * DesvioFugaGraus * Math.PI / 180;
            double distancia = (DistanciaFugaMinPx + _random.NextDouble() * (DistanciaFugaMaxPx - DistanciaFugaMinPx)) * _escala;
            double angulo = anguloBase + desvio;
            int x = (int)Math.Round(centro.X + Math.Cos(angulo) * distancia - _tamanho / 2.0);
            int y = (int)Math.Round(centro.Y + Math.Sin(angulo) * distancia - _tamanho / 2.0);
            x = Math.Clamp(x, tela.Left + margem, Math.Max(tela.Left + margem, tela.Right - _tamanho - margem));
            y = Math.Clamp(y, tela.Top + margem, Math.Max(tela.Top + margem, tela.Bottom - _tamanho - margem));

            var destino = new Point(x, y);
            double afastamento = Math.Sqrt(Math.Pow(x + _tamanho / 2.0 - cursor.X, 2) + Math.Pow(y + _tamanho / 2.0 - cursor.Y, 2));
            if (afastamento >= DistanciaFugaMinPx * _escala)
                return destino;
        }
        return DestinoVoarAleatorio();
    }

    public async Task VoarPara(Point destino, uint velocidade = 0, bool levarMouse = false)
    {
        await VoarParaDestino(destino, velocidade, levarMouse, false);
    }

    private async Task VoarParaDestino(Point destino, uint velocidade, bool levarMouse, bool fuga)
    {
        if (_dragging) return;
        _destinoVoar = destino;
        velocidade = velocidade == 0 ? VelocidadePadrao : velocidade;
        await VoarParaDestinoAtual(velocidade, levarMouse, fuga);
    }

    public async Task VoarPara(uint velocidade = 0, bool levarMouse = false)
    {
        await VoarParaDestinoAtual(velocidade, levarMouse, false);
    }

    private async Task VoarParaDestinoAtual(uint velocidade, bool levarMouse, bool fuga)
    {
        if (_dragging) return;
        _emVoo = true;
        _geracao++;
        SairDaComida();
        _escalaComprimento = 1f;
        _tremor = Point.Empty;
        await VoarComFisica(velocidade, levarMouse, fuga);
    }

    public async Task RoubarMouse(int esperarMilissegundos = 0)
    {
        await Task.Delay(esperarMilissegundos);
        var destinoMouse = OrigemParaCursorEm(Cursor.Position);
        await VoarPara(destinoMouse, 150);
        await VoarPara(DestinoVoarAleatorio(), 80, true);
    }

    private Point DestinoVoarAleatorio()
    {
        var screens = Screen.AllScreens;
        var screen = screens[_random.Next(screens.Length)];
        int margem = 10;
        int maxX = screen.WorkingArea.Right - _tamanho - margem;
        int maxY = screen.WorkingArea.Bottom - _tamanho - margem;
        int minX = screen.WorkingArea.Left + margem;
        int minY = screen.WorkingArea.Top + margem;
        int x = _random.Next(minX, Math.Max(minX + 1, maxX));
        int y = _random.Next(minY, Math.Max(minY + 1, maxY));
        return new Point(x, y);
    }

    private Point ForaDeTodasTelasAleatorio()
    {
        var screens = Screen.AllScreens;
        int margem = 100; // quanto "fora" da tela
        int tentativas = 0;
        while (tentativas < 20)
        {
            var screen = screens[_random.Next(screens.Length)];
            int borda = _random.Next(4); // 0: cima, 1: baixo, 2: esquerda, 3: direita
            int x = 0, y = 0;
            switch (borda)
            {
                case 0: // cima
                    x = _random.Next(screen.WorkingArea.Left, screen.WorkingArea.Right - this.Width);
                    y = screen.WorkingArea.Top - this.Height - margem;
                    break;
                case 1: // baixo
                    x = _random.Next(screen.WorkingArea.Left, screen.WorkingArea.Right - this.Width);
                    y = screen.WorkingArea.Bottom + margem;
                    break;
                case 2: // esquerda
                    x = screen.WorkingArea.Left - this.Width - margem;
                    y = _random.Next(screen.WorkingArea.Top, screen.WorkingArea.Bottom - this.Height);
                    break;
                case 3: // direita
                    x = screen.WorkingArea.Right + margem;
                    y = _random.Next(screen.WorkingArea.Top, screen.WorkingArea.Bottom - this.Height);
                    break;
            }
            var pos = new Point(x, y);
            var rect = new Rectangle(pos, this.Size);
            bool fora = true;
            foreach (var s in screens)
            {
                if (rect.IntersectsWith(s.WorkingArea))
                {
                    fora = false;
                    break;
                }
            }
            if (fora)
                return pos;
            tentativas++;
        }
        // fallback: retorna posição fora da primeira tela
        var fallback = new Point(screens[0].WorkingArea.Left - this.Width - margem, screens[0].WorkingArea.Top);
        return fallback;
    }

    private async void TimerDecisao_Tick(object? sender, EventArgs e)
    {
        if (_executandoComportamento || _emVoo || _dragging || IsDisposed)
            return;
        if (!_permitirPernas && !_permitirRotacao && !_permitirCaminhar && !_permitirVoo)
            return;

        _executandoComportamento = true;
        int geracao = _geracao;
        try
        {
            if (_permitirVoo && _relogio.Elapsed.TotalSeconds >= _proximoVooEm)
            {
                await VoarEspontaneo();
                return;
            }

            switch (SortearComportamento())
            {
                case ComportamentoRepouso.Limpando:
                    await Limpar(geracao);
                    break;
                case ComportamentoRepouso.Andando:
                    await Andar(geracao);
                    break;
                case ComportamentoRepouso.Virando:
                    await Virar(geracao);
                    break;
                default:
                    await FicarPousada(geracao);
                    break;
            }
        }
        finally
        {
            _escalaComprimento = 1f;
            _tremor = Point.Empty;
            if (!_emVoo)
                Renderizar();
            _executandoComportamento = false;
        }
    }

    private enum ComportamentoRepouso
    {
        Pousada,
        Limpando,
        Andando,
        Virando
    }

    private ComportamentoRepouso SortearComportamento()
    {
        double pesoLimpando = _permitirPernas ? PesoLimpando : 0;
        double pesoAndando = _permitirCaminhar ? PesoAndando : 0;
        double pesoVirando = _permitirRotacao ? PesoVirando : 0;
        double total = PesoPousada + pesoLimpando + pesoAndando + pesoVirando;
        double sorteio = _random.NextDouble() * total;

        if ((sorteio -= PesoPousada) < 0) return ComportamentoRepouso.Pousada;
        if ((sorteio -= pesoLimpando) < 0) return ComportamentoRepouso.Limpando;
        if ((sorteio -= pesoAndando) < 0) return ComportamentoRepouso.Andando;
        return ComportamentoRepouso.Virando;
    }

    private bool Interrompido(int geracao)
    {
        return geracao != _geracao || _emVoo || _dragging || IsDisposed;
    }

    private async Task FicarPousada(int geracao)
    {
        double fim = _relogio.Elapsed.TotalSeconds + 0.4 + _random.NextDouble() * 2.1;
        while (!Interrompido(geracao) && _relogio.Elapsed.TotalSeconds < fim)
        {
            await Task.Delay(_random.Next(250, 700));
            if (Interrompido(geracao))
                return;

            double sorteio = _random.NextDouble();
            if (_permitirRotacao && sorteio < 0.3)
            {
                float giro = (float)((_random.Next(2) == 0 ? -1 : 1) * (0.5 + _random.NextDouble() * 1.5));
                AplicarAngulo(_anguloAtual + giro);
            }
            else if (_permitirPernas && sorteio < 0.4)
            {
                _indiceSprite = _random.Next(SpritesMosca.Quantidade);
                Renderizar();
            }
            else if (sorteio < 0.5)
            {
                _tremor = new Point(_random.Next(-1, 2), _random.Next(-1, 2));
                Renderizar();
                await Task.Delay(QuadroMs * 2);
                _tremor = Point.Empty;
                Renderizar();
            }
        }
    }

    private async Task Limpar(int geracao)
    {
        int perna1 = _random.Next(SpritesMosca.Quantidade);
        int perna2;
        do
        {
            perna2 = _random.Next(SpritesMosca.Quantidade);
        } while (perna2 == perna1);

        var trocas = new Queue<(double Momento, int Sprite)>();
        double momento = 0;
        int rajadas = _random.Next(RajadasLimparMin, RajadasLimparMax + 1);
        for (int rajada = 0; rajada < rajadas; rajada++)
        {
            if (rajada > 0)
                momento += Sortear(PausaEntreRajadasMinS, PausaEntreRajadasMaxS);
            int quantidade = _random.Next(TrocasPorRajadaMin, TrocasPorRajadaMax + 1);
            for (int i = 0; i < quantidade; i++)
            {
                trocas.Enqueue((momento, i % 2 == 0 ? perna1 : perna2));
                momento += Sortear(IntervaloTrocaLimparMinS, IntervaloTrocaLimparMaxS);
            }
        }
        double duracao = momento;

        double inicio = _relogio.Elapsed.TotalSeconds;
        double frequenciaInclinacao = 1.5 + _random.NextDouble() * 1.5;

        while (!Interrompido(geracao))
        {
            double t = _relogio.Elapsed.TotalSeconds - inicio;
            if (t >= duracao)
                break;
            while (trocas.Count > 0 && t >= trocas.Peek().Momento)
                _indiceSprite = trocas.Dequeue().Sprite;
            _escalaComprimento = (float)(0.985 + 0.015 * Math.Sin(t * Math.PI * 2 * frequenciaInclinacao));
            Renderizar();
            await Task.Delay(QuadroMs);
        }
    }

    private static double Sortear(double minimo, double maximo) => minimo + _random.NextDouble() * (maximo - minimo);

    private async Task Andar(int geracao)
    {
        int trechos = _random.Next(1, 4);
        for (int trecho = 0; trecho < trechos && !Interrompido(geracao); trecho++)
        {
            if (trecho > 0)
            {
                await Task.Delay(_random.Next(150, 400));
                if (Interrompido(geracao))
                    return;
                if (_permitirRotacao)
                    AplicarAngulo(_anguloAtual + (float)((_random.NextDouble() * 2 - 1) * 10));
            }

            double distancia = (5 + _random.NextDouble() * 25) * _escala;
            double radianos = (_anguloAtual - 90) * Math.PI / 180;
            double dirX = Math.Cos(radianos);
            double dirY = Math.Sin(radianos);
            var inicio = Origem;

            var destino = new Point(
                (int)Math.Round(inicio.X + dirX * distancia),
                (int)Math.Round(inicio.Y + dirY * distancia));
            if (!DentroDaAreaDeTrabalho(destino))
                return;

            double velocidade = VelocidadeAndarPxS * _escala;
            double passoTroca = PassoTrocaPernaPx * _escala;
            double percorrido = 0;
            double ultimaTroca = 0;
            double anterior = _relogio.Elapsed.TotalSeconds;

            while (percorrido < distancia && !Interrompido(geracao))
            {
                await Task.Delay(QuadroMs);
                double agora = _relogio.Elapsed.TotalSeconds;
                percorrido = Math.Min(distancia, percorrido + velocidade * Math.Min(0.05, agora - anterior));
                anterior = agora;

                if (percorrido - ultimaTroca >= passoTroca)
                {
                    _indiceSprite = (_indiceSprite + 1) % SpritesMosca.Quantidade;
                    ultimaTroca = percorrido;
                }
                PosicionarOrigem(new Point(
                    (int)Math.Round(inicio.X + dirX * percorrido),
                    (int)Math.Round(inicio.Y + dirY * percorrido)));
            }
        }
    }

    private async Task Virar(int geracao)
    {
        float giro = (float)((_random.Next(2) == 0 ? -1 : 1) * (15 + _random.NextDouble() * 45));
        float alvo = ((_anguloAtual + giro) % 360 + 360) % 360;
        bool trocouPerna = false;
        double anterior = _relogio.Elapsed.TotalSeconds;

        while (!Interrompido(geracao))
        {
            await Task.Delay(QuadroMs);
            double agora = _relogio.Elapsed.TotalSeconds;
            float passo = (float)(VelocidadeGiroRepousoGrausS * Math.Min(0.05, agora - anterior));
            anterior = agora;

            _anguloAtual = AproximarAngulo(_anguloAtual, alvo, passo);
            float restante = Math.Abs(((alvo - _anguloAtual) % 360 + 540) % 360 - 180);
            if (_permitirPernas && !trocouPerna && restante < Math.Abs(giro) / 2)
            {
                _indiceSprite = _random.Next(SpritesMosca.Quantidade);
                trocouPerna = true;
            }
            Renderizar();
            if (restante < 0.01f)
                break;
        }
    }

    private bool DentroDaAreaDeTrabalho(Point origem)
    {
        var caixa = new Rectangle(origem, new Size(_tamanho, _tamanho));
        return Screen.AllScreens.Any(s => s.WorkingArea.Contains(caixa));
    }

    private bool TemComida()
    {
        return Application.OpenForms.OfType<frmComida>().Count() > 0;
    }

    private List<frmComida> ListaComidas()
    {
        return Application.OpenForms.OfType<frmComida>().ToList();
    }

    private async Task VoarEspontaneo()
    {
        if (_emVoo)
            return;
        Point destino;
        frmComida? comidaAlvo = null;
        if (ComportamentoMouse == ComportamentoMouseEnum.SeguirMouse && _random.NextDouble() < 0.95)
        {
            destino = OrigemParaCursorEm(Cursor.Position);
        }
        else if (ComportamentoMouse != ComportamentoMouseEnum.SeguirMouse && TemComida() && _random.NextDouble() < 0.95)
        {
            var comidas = ListaComidas();
            comidaAlvo = comidas[_random.Next(comidas.Count)];
            var ponto = comidaAlvo.PontoDePouso();
            destino = new Point(ponto.X - _tamanho / 2, ponto.Y - _tamanho / 2);
        }
        else
        {
            destino = DestinoVoarAleatorio();
        }
        await VoarPara(destino);

        if (comidaAlvo != null && !comidaAlvo.IsDisposed && !_emVoo && Origem == destino)
            EntrarNaComida(comidaAlvo);
    }

    private void EntrarNaComida(frmComida comida)
    {
        SairDaComida();
        _comidaAtual = comida;
        comida.ComecouArrastar += Comida_ComecouArrastar;
    }

    private void SairDaComida()
    {
        if (_comidaAtual == null)
            return;
        _comidaAtual.ComecouArrastar -= Comida_ComecouArrastar;
        _comidaAtual = null;
    }

    private void Comida_ComecouArrastar(object? sender, EventArgs e)
    {
        if (sender == _comidaAtual && !_emVoo && !_dragging && !IsDisposed)
            _ = Fugir();
    }

    private async Task VoarComFisica(uint velocidade = 0, bool levarMouse = false, bool fuga = false)
    {
        if (ComSom && _waveOut != null && _zumbido != null)
        {
            float[] volumes = { 1.0f, 0.8f, 0.5f, 0.3f, 0.1f };
            _volumeBase = volumes[_random.Next(volumes.Length)];
            AtualizarSom(0f);
            _zumbido.Iniciar();
            if (_waveOut.PlaybackState != PlaybackState.Playing)
                _waveOut.Play();
            if (!fuga)
                await Task.Delay(100);
        }
        velocidade = velocidade == 0 ? (uint)_random.Next(80, 201) : velocidade;

        await Decolar(fuga);

        double velMax = velocidade * FatorVelocidade;
        double aceleracao = velMax / TempoAceleracaoS;
        double raioFrenagem = velMax * velMax / (2 * aceleracao);
        double tolerancia = Math.Max(1.5, 3 * _escala);

        var destino = new Vector2(_destinoVoar.X, _destinoVoar.Y);
        var inicio = Origem;
        var pos = new Vector2(inicio.X, inicio.Y);
        var vel = Vector2.Zero;

        double[] fases = [_random.NextDouble() * Math.PI * 2, _random.NextDouble() * Math.PI * 2, _random.NextDouble() * Math.PI * 2];
        double[] frequencias = [0.7, 1.9, 3.7];
        double desvioRajada = 0;
        double proximaRajada = 0;
        double pairarAte = -1;

        double limiteTempo = 2.0 + 4.0 * (destino - pos).Length() / velMax;
        var cronometro = Stopwatch.StartNew();
        double anterior = 0;

        while (!IsDisposed)
        {
            double agora = cronometro.Elapsed.TotalSeconds;
            double dt = Math.Clamp(agora - anterior, 0.001, 0.05);
            anterior = agora;

            var paraDestino = destino - pos;
            double dist = paraDestino.Length();
            if (dist <= tolerancia || agora > limiteTempo)
                break;

            double fatorLonge = Math.Clamp(dist / (raioFrenagem * 1.5), 0, 1);

            if (agora >= proximaRajada)
            {
                proximaRajada = agora + IntervaloRajadaMinS + _random.NextDouble() * (IntervaloRajadaMaxS - IntervaloRajadaMinS);
                desvioRajada = _random.NextDouble() < ChanceGuinada
                    ? (_random.Next(2) == 0 ? -1 : 1) * (30 + _random.NextDouble() * 60)
                    : 0;
                if (dist > raioFrenagem * 2 && _random.NextDouble() < ChancePairar)
                    pairarAte = agora + 0.05 + _random.NextDouble() * 0.1;
            }

            double ruido = 0;
            for (int i = 0; i < fases.Length; i++)
                ruido += Math.Sin(2 * Math.PI * frequencias[i] * agora + fases[i]) / fases.Length;

            double desvioGraus = (ruido * DesvioRuidoMaxGraus + desvioRajada) * fatorLonge;
            double anguloDesejado = Math.Atan2(paraDestino.Y, paraDestino.X) + desvioGraus * Math.PI / 180;
            double velDesejada = agora < pairarAte ? 0 : Math.Min(velMax, Math.Sqrt(2 * aceleracao * dist));
            var desejada = new Vector2((float)Math.Cos(anguloDesejado), (float)Math.Sin(anguloDesejado)) * (float)velDesejada;

            var dv = desejada - vel;
            float maxDv = (float)(aceleracao * dt);
            if (dv.Length() > maxDv)
                dv = Vector2.Normalize(dv) * maxDv;
            vel += dv;

            if (dist <= vel.Length() * dt + tolerancia)
                break;

            pos += vel * (float)dt;

            if (vel.Length() > 1)
            {
                float anguloVoo = (float)(Math.Atan2(vel.Y, vel.X) * 180 / Math.PI + 90);
                _anguloAtual = AproximarAngulo(_anguloAtual, anguloVoo, (float)(TaxaGiroGrausS * dt));
            }

            PosicionarOrigem(new Point((int)Math.Round(pos.X), (int)Math.Round(pos.Y)));
            if (levarMouse)
                LevarCursor();
            AtualizarSom((float)Math.Clamp(vel.Length() / velMax, 0, 1));

            int gasto = (int)((cronometro.Elapsed.TotalSeconds - agora) * 1000);
            await Task.Delay(Math.Max(1, QuadroMs - gasto));
        }

        if (IsDisposed)
            return;

        // Garante que termina exatamente no destino
        PosicionarOrigem(_destinoVoar);
        if (levarMouse)
            LevarCursor();
        _zumbido?.Parar();
        await Pousar();
        _emVoo = false;
        ProximoIntervaloVoar();
    }

    private void AtualizarSom(float velocidadeNormalizada)
    {
        if (_zumbido == null)
            return;
        _zumbido.Volume = _volumeBase * (VolumeMinimoVoo + (1f - VolumeMinimoVoo) * velocidadeNormalizada);
        _zumbido.Taxa = 1f - VariacaoTomVelocidade / 2 + VariacaoTomVelocidade * velocidadeNormalizada;

        var telaVirtual = SystemInformation.VirtualScreen;
        if (telaVirtual.Width > 0)
            _zumbido.Pan = 2f * (CentroMosca.X - telaVirtual.Left) / telaVirtual.Width - 1f;
    }

    private void WaveOut_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        if (_zumbido != null && _zumbido.Tocando && _waveOut != null && !IsDisposed)
            _waveOut.Play();
    }

    private static float AproximarAngulo(float atual, float alvo, float passoMax)
    {
        float diferenca = ((alvo - atual) % 360 + 540) % 360 - 180;
        diferenca = Math.Clamp(diferenca, -passoMax, passoMax);
        return ((atual + diferenca) % 360 + 360) % 360;
    }

    public async Task ApontarPara(int posX, int posY)
    {
        var point = new Point(posX, posY);
        await ApontarPara(point);
    }

    public Task ApontarPara(Point point)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => AplicarAngulo(AnguloPara(point))));
            return Task.CompletedTask;
        }
        AplicarAngulo(AnguloPara(point));
        return Task.CompletedTask;
    }

    private float AnguloPara(Point point)
    {
        var centro = CentroMosca;
        int dx = point.X - centro.X;
        int dy = point.Y - centro.Y;
        if (dx == 0 && dy == 0)
            return _anguloAtual;
        // ângulo em radianos, 0 é para a direita, positivo sentido anti-horário
        double angRad = Math.Atan2(dy, dx);
        // converte para graus e ajusta para que 0 seja para cima (em vez de direita)
        double angDeg = angRad * 180 / Math.PI + 90;
        return (float)(((angDeg % 360) + 360) % 360);
    }

    public void AplicarAngulo(float angulo)
    {
        _anguloAtual = ((angulo % 360) + 360) % 360;
        Renderizar();
    }

    private void ProximoIntervaloVoar()
    {
        int nextInterval = _random.Next(500, 10001);
        _proximoVooEm = _relogio.Elapsed.TotalSeconds + nextInterval / 1000.0;
        _emVoo = false;
    }

    private async Task RotacionarSuaveAte(int anguloDestino, int passo = 30, int delayMs = 10)
    {
        int atual = (int)Math.Round(_anguloAtual);
        anguloDestino = ((anguloDestino % 360) + 360) % 360;
        atual = ((atual % 360) + 360) % 360;
        if (atual == anguloDestino) return;

        int diff = (anguloDestino - atual + 360) % 360;
        int sentido = diff <= 180 ? 1 : -1; // 1: horário, -1: anti-horário
        int passos = 0;
        while (atual != anguloDestino && passos < (360 / passo))
        {
            int prox = (atual + sentido * passo + 360) % 360;
            // Se passar do destino, ajusta para o destino
            if ((sentido == 1 && ((prox > anguloDestino && atual < anguloDestino) || (atual > anguloDestino && prox < atual))) ||
                (sentido == -1 && ((prox < anguloDestino && atual > anguloDestino) || (atual < anguloDestino && prox > atual))))
            {
                prox = anguloDestino;
            }
            AplicarAngulo(prox);
            atual = prox;
            await Task.Delay(delayMs);
            passos++;
        }
    }

    public async Task OlharParaRotacionarSuave(Direcao onde)
    {
        switch (onde)
        {
            case Direcao.Cima:
                await RotacionarSuaveAte(0);
                break;
            case Direcao.Direita:
                await RotacionarSuaveAte(90);
                break;
            case Direcao.Baixo:
                await RotacionarSuaveAte(180);
                break;
            case Direcao.Esquerda:
                await RotacionarSuaveAte(270);
                break;
        }
    }


    private void Mosca_MouseWheel(object? sender, MouseEventArgs e)
    {
        int quantosGraus = 40;
        int delta = e.Delta > 0 ? quantosGraus * -1 : quantosGraus;
        AplicarAngulo(_anguloAtual + delta);
    }

    private void Mosca_MouseDown(object? sender, MouseEventArgs e)
    {
        if (PermitirAgarrarSoltar && e.Button == MouseButtons.Left)
        {
            _dragging = true;
            _dragOffset = new Point(e.X, e.Y);
        }
    }

    private void Mosca_MouseMove(object? sender, MouseEventArgs e)
    {
        if (PermitirAgarrarSoltar && _dragging)
        {
            var mousePos = MousePosition;
            PosicionarJanela(new Point(mousePos.X - _dragOffset.X, mousePos.Y - _dragOffset.Y));
        }
    }

    private void Mosca_MouseUp(object? sender, MouseEventArgs e)
    {
        if (PermitirAgarrarSoltar && e.Button == MouseButtons.Left)
        {
            _dragging = false;
        }
    }

    public void Morrer()
    {
        // Para os timers
        _timerDecisao.Stop();
        _timerDecisao.Dispose();
        _geracao++;
        SairDaComida();

        // Para o áudio
        _zumbido?.Parar();
        _zumbido = null;
        if (_waveOut != null)
        {
            _waveOut.PlaybackStopped -= WaveOut_PlaybackStopped;
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }

        // Fecha o formulário
        this.Close();
    }

    public async Task DancaLoca()
    {
        AtivarTimers(false);

        AplicarAngulo(35);
        await Task.Delay(200);

        await PatinhasBoomerang(10);

        await Rodopio(5, true);

        // 5 - Mover um tanto para a esquerda
        var origem = Origem;
        PosicionarOrigem(new Point(origem.X - 100, origem.Y));
        await Task.Delay(150);

        AplicarAngulo(215);

        await PatinhasBoomerang(10);

        await Rodopio(5, false);

        AtivarTimers(true);
    }

    public async Task Rodopio(int numeroVoltas, bool sentidoHorario)
    {
        int passo = 120;
        int direcao = sentidoHorario ? 1 : -1;

        for (int i = 0; i < numeroVoltas * 3; i++)
        {
            int novoAngulo = ((int)Math.Round(_anguloAtual) + direcao * passo + 360) % 360;
            await RotacionarSuaveAte(novoAngulo);
            Application.DoEvents();
        }
    }

    public async Task PatinhasBoomerang(int quantasVezes)
    {
        int idx1 = _random.Next(SpritesMosca.Quantidade);
        int idx2;
        do
        {
            idx2 = _random.Next(SpritesMosca.Quantidade);
        } while (idx2 == idx1);
        for (int i = 0; i < quantasVezes; i++)
        {
            foreach (var idx in new[] { idx1, idx2 })
            {
                _indiceSprite = idx;
                Renderizar();
                Application.DoEvents();
                await Task.Delay(150);
            }
        }
    }

    public async Task FilaIndiana()
    {
        AtivarTimers(false);

        if (Indice < 0)
            return;

        var screen = Screen.PrimaryScreen?.WorkingArea ?? Screen.AllScreens[0].WorkingArea;
        int centerX = screen.Left + (screen.Width - _tamanho) / 2;
        int centerY = screen.Top + (screen.Height - _tamanho) / 2;
        int spacing = 30;

        int x;
        if (Indice == 0)
        {
            x = centerX;
        }
        else if (Indice % 2 == 0)
        {
            // Par diferente de zero: esquerda
            x = centerX - (spacing * Indice);
        }
        else
        {
            // Ímpar: direita
            x = centerX + (spacing * Indice);
        }

        var destino = new Point(x, centerY);
        await VoarPara(destino, 120);
        await OlharParaRotacionarSuave(Direcao.Direita);
        await Task.Delay(1000);

        AtivarTimers(true);
    }

    public async Task RodaGigante()
    {
        AtivarTimers(false);
        for (int i = 0; i < 4; i++)
        {
            await VoarPara(BordaMaisProxima(Direcao.Cima), 50);
            Application.DoEvents();
            await VoarPara(BordaMaisProxima(Direcao.Direita), 50);
            Application.DoEvents();
            await VoarPara(BordaMaisProxima(Direcao.Baixo), 50);
            Application.DoEvents();
            await VoarPara(BordaMaisProxima(Direcao.Esquerda), 50);
            Application.DoEvents();
        }
        AtivarTimers(true);
    }

    public void AtivarTimers(bool ativar)
    {
        AtivarTimerVoar(ativar);
        AtivarTimerRotacao(ativar);
        AtivarTimerMoverPernas(ativar);
        AtivarTimerCaminhar(ativar);
    }

    public void AtivarTimerCaminhar(bool ativar)
    {
        _permitirCaminhar = ativar;
        if (!ativar)
            _geracao++;
    }

    public void AtivarTimerVoar(bool ativar)
    {
        _permitirVoo = ativar;
        if (ativar)
            _proximoVooEm = _relogio.Elapsed.TotalSeconds + _random.Next(500, 10001) / 1000.0;
        else
            _geracao++;
    }

    public void AtivarTimerRotacao(bool ativar)
    {
        _permitirRotacao = ativar;
        if (!ativar)
            _geracao++;
    }

    public void AtivarTimerMoverPernas(bool ativar)
    {
        _permitirPernas = ativar;
        if (!ativar)
            _geracao++;
    }

    private Point BordaMaisProxima(Direcao? direcao = null)
    {
        // Descobre a tela onde o ponto está
        Screen? screen = Screen.AllScreens.FirstOrDefault(s => s.WorkingArea.Contains(PosicaoAtual));
        if (screen == null)
        {
            // Se não estiver em nenhuma tela, usa a principal
            screen = Screen.PrimaryScreen;
        }
        var area = screen!.WorkingArea;

        if (direcao != null)
        {
            switch (direcao.Value)
            {
                case Direcao.Cima:
                    return new Point(PosicaoAtual.X, area.Top);
                case Direcao.Baixo:
                    return new Point(PosicaoAtual.X, area.Bottom - 30);
                case Direcao.Esquerda:
                    return new Point(area.Left, PosicaoAtual.Y);
                case Direcao.Direita:
                    return new Point(area.Right - 30, PosicaoAtual.Y);
            }
        }

        // Calcula as distâncias até cada borda
        int distTop = Math.Abs(PosicaoAtual.Y - area.Top);
        int distBottom = Math.Abs(PosicaoAtual.Y - area.Bottom);
        int distLeft = Math.Abs(PosicaoAtual.X - area.Left);
        int distRight = Math.Abs(PosicaoAtual.X - area.Right);

        int minDist = Math.Min(Math.Min(distTop, distBottom), Math.Min(distLeft, distRight));

        if (minDist == distTop)
            return new Point(PosicaoAtual.X, area.Top);
        if (minDist == distBottom)
            return new Point(PosicaoAtual.X, area.Bottom);
        if (minDist == distLeft)
            return new Point(area.Left, PosicaoAtual.Y);
        // minDist == distRight
        return new Point(area.Right, PosicaoAtual.Y);
    }

    private Point CentroTelaPrincipal()
    {
        var screen = Screen.PrimaryScreen?.WorkingArea ?? Screen.AllScreens[0].WorkingArea;
        int centroX = screen.Left + screen.Width / 2 - _tamanho / 2;
        int centroY = screen.Top + screen.Height / 2 - _tamanho / 2;
        return new Point(centroX, centroY);
    }

    public async Task FormacaoQuadrada(double fator = 1.5)
    {
        if (Indice < 0) return;

        var destino = CalcularDeslocamentoQuadrado(fator);
        await VoarPara(destino, 120);
    }

    private Point CalcularDeslocamentoQuadrado(double fator = 1.5)
    {
        if (Indice < 0) return new Point(0, 0);
        int tamanho = _tamanho;
        var deslocs = new (int dx, int dy)[]
        {
            (0, 0),                                         // 0 centro
            ((int)(+fator*tamanho), 0),                     // 1 direita
            ((int)(-fator*tamanho), 0),                     // 2 esquerda
            (0, (int)(-fator*tamanho)),                     // 3 cima
            ((int)(+fator*tamanho), (int)(-fator*tamanho)), // 4 cima-direita
            ((int)(-fator*tamanho), (int)(-fator*tamanho)), // 5 cima-esquerda
            (0, (int)(+fator*tamanho)),                     // 6 baixo
            ((int)(+fator*tamanho), (int)(+fator*tamanho)), // 7 baixo-direita
            ((int)(-fator*tamanho), (int)(+fator*tamanho)), // 8 baixo-esquerda

            ((int)(+fator*(tamanho*2)), (int)(-fator*tamanho)), // 9
            ((int)(+fator*(tamanho*2)), 0),                     // 10
            ((int)(+fator*(tamanho*2)), (int)(+fator*tamanho)), // 11

            ((int)(-fator*(tamanho*2)), (int)(-fator*tamanho)), // 12
            ((int)(-fator*(tamanho*2)), 0),                     // 13
            ((int)(-fator*(tamanho*2)), (int)(+fator*tamanho)), // 14

            (0, (int)(-fator*(tamanho*2))),                     // 15
            ((int)(+fator*tamanho), (int)(-fator*(tamanho*2))), // 16
            ((int)(-fator*tamanho), (int)(-fator*(tamanho*2))), // 17

            ((int)(+fator*(tamanho*2)), (int)(-fator*(tamanho*2))), // 18
            ((int)(-fator*(tamanho*2)), (int)(-fator*(tamanho*2))), // 19

            (0, (int)(+fator*(tamanho*2))),                         // 20
            ((int)(+fator*tamanho), (int)(+fator*(tamanho*2))),     // 21
            ((int)(-fator*tamanho), (int)(+fator*(tamanho*2))),     // 22
            ((int)(+fator*(tamanho*2)), (int)(+fator*(tamanho*2))), // 23
            ((int)(-fator*(tamanho*2)), (int)(+fator*(tamanho*2))), // 24

            ((int)(+fator*(tamanho*3)), (int)(-fator*(tamanho*2))), // 25
            ((int)(+fator*(tamanho*3)), (int)(-fator*tamanho)),     // 26
            ((int)(+fator*(tamanho*3)), 0),                         // 27
            ((int)(+fator*(tamanho*3)), (int)(+fator*tamanho)),     // 28
            ((int)(+fator*(tamanho*3)), (int)(+fator*(tamanho*2))), // 29

            ((int)(-fator*(tamanho*3)), (int)(-fator*(tamanho*2))), // 30
            ((int)(-fator*(tamanho*3)), (int)(-fator*tamanho)),     // 31
            ((int)(-fator*(tamanho*3)), 0),                         // 32
            ((int)(-fator*(tamanho*3)), (int)(+fator*tamanho)),     // 33
            ((int)(-fator*(tamanho*3)), (int)(+fator*(tamanho*2))), // 34

            (0, (int)(-fator*(tamanho*3))),                         // 35
            ((int)(+fator*tamanho), (int)(-fator*(tamanho*3))),     // 36
            ((int)(-fator*tamanho), (int)(-fator*(tamanho*3))),     // 37
            ((int)(+fator*(tamanho*2)), (int)(-fator*(tamanho*3))), // 38
            ((int)(-fator*(tamanho*2)), (int)(-fator*(tamanho*3))), // 39
            ((int)(+fator*(tamanho*3)), (int)(-fator*(tamanho*3))), // 40
            ((int)(-fator*(tamanho*3)), (int)(-fator*(tamanho*3))), // 41

            ((int)(+fator*(tamanho*4)), (int)(-fator*(tamanho*3))), // 42
            ((int)(+fator*(tamanho*4)), (int)(-fator*(tamanho*2))), // 43
            ((int)(+fator*(tamanho*4)), (int)(-fator*tamanho)),     // 44
            ((int)(+fator*(tamanho*4)), 0),                         // 45
            ((int)(+fator*(tamanho*4)), (int)(+fator*tamanho)),     // 46
            ((int)(+fator*(tamanho*4)), (int)(+fator*(tamanho*2))), // 47

            ((int)(-fator*(tamanho*4)), (int)(-fator*(tamanho*3))), // 48
            ((int)(-fator*(tamanho*4)), (int)(-fator*(tamanho*2))), // 49
            ((int)(-fator*(tamanho*4)), (int)(-fator*tamanho)),     // 50
            ((int)(-fator*(tamanho*4)), 0),                         // 51
            ((int)(-fator*(tamanho*4)), (int)(+fator*tamanho)),     // 52
            ((int)(-fator*(tamanho*4)), (int)(+fator*(tamanho*2))), // 53

        };

        var centroTelaPrincipal = CentroTelaPrincipal();

        var indiceDesloc = Indice % deslocs.Length;
        return new Point(centroTelaPrincipal.X + deslocs[indiceDesloc].dx, centroTelaPrincipal.Y + deslocs[indiceDesloc].dy);

    }
}
