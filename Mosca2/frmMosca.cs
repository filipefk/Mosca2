using NAudio.Wave;

namespace Mosca2;

public partial class frmMosca : Form
{
    private readonly System.Windows.Forms.Timer _timerMoverPernas;
    private readonly System.Windows.Forms.Timer _timerRotacao;
    private readonly System.Windows.Forms.Timer _timerVoar;
    private static readonly Random _random = new();
    private readonly string[] _moscaImages = { "Mosca1", "Mosca2", "Mosca3", "Mosca4", "Mosca5" };
    private int _anguloAtual = 0;
    private Point _destinoVoar;
    private bool _emVoo = false;
    private IWavePlayer? _waveOut;
    private WaveStream? _mp3Reader;
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

        this.BackColor = Color.FromArgb(50, 50, 50);
        this.TransparencyKey = this.BackColor;
        this.ShowInTaskbar = false;
        this.TopMost = true;

        _timerMoverPernas = new System.Windows.Forms.Timer();
        _timerMoverPernas.Tick += TimerMoverPernas_Tick;
        ProximoIntervaloMoverPernas();
        _timerMoverPernas.Start();

        _timerRotacao = new System.Windows.Forms.Timer();
        _timerRotacao.Tick += TimerRotacao_Tick;
        ProximoIntervaloRotacao();
        _timerRotacao.Start();

        _timerVoar = new System.Windows.Forms.Timer();
        _timerVoar.Tick += async (s, e) => await TimerVoar_Tick(s, e);
        ProximoIntervaloVoar();
        _timerVoar.Start();

        if (picMosca != null)
        {
            picMosca.MouseEnter += PicMosca_MouseEnter;
            picMosca.MouseWheel += PicMosca_MouseWheel;
            picMosca.MouseClick += PicMosca_MouseClick;
            picMosca.MouseDown += PicMosca_MouseDown;
            picMosca.MouseMove += PicMosca_MouseMove;
            picMosca.MouseUp += PicMosca_MouseUp;
        }

        var mp3Stream = new MemoryStream();
        Properties.Resources.Som_de_Mosca.CopyTo(mp3Stream);
        mp3Stream.Position = 0;
        _mp3Reader = new Mp3FileReader(mp3Stream);
        _waveOut = new WaveOutEvent();
        _waveOut.Init(_mp3Reader);

        this.StartPosition = FormStartPosition.Manual;
        this.Location = ForaDeTodasTelasAleatorio();
    }

    public ComportamentoMouseEnum ComportamentoMouse { get; set; } = ComportamentoMouseEnum.FugirMouse;
    public bool ComSom { get; set; } = false;
    
    public bool MostrarIndice 
    {
        get { return _mostrarIndice; }
        set 
        { 
            _mostrarIndice = value;
            lblIndice.Text = Indice.ToString();
            lblIndice.Visible = _mostrarIndice;
        }
    }

    public bool PermitirAgarrarSoltar { get; set; } = false;
    public int Indice { get; set; } = -1;

    public Point PosicaoAtual { get => new Point(this.Left, this.Top); }

    private async void PicMosca_MouseEnter(object? sender, EventArgs e)
    {
        if (ComportamentoMouse == ComportamentoMouseEnum.SeguirMouse
            || ComportamentoMouse == ComportamentoMouseEnum.IgnorarMouse
            || _emVoo)
            return;
        await Fugir();
    }

    private async void PicMosca_MouseClick(object? sender, EventArgs e)
    {
        if (_emVoo) return;
        if (!PermitirAgarrarSoltar && ComportamentoMouse != ComportamentoMouseEnum.IgnorarMouse)
            await Fugir();
    }

    public async Task Fugir()
    {
        await VoarPara(DestinoVoarAleatorio());
    }

    public async Task VoarPara(Point destino, int velocidade = 0, bool levarMouse = false)
    {
        if (_dragging) return;
        _destinoVoar = destino;
        await VoarPara(velocidade, levarMouse);
    }

    public async Task VoarPara(int velocidade = 0, bool levarMouse = false)
    {
        if (_dragging) return;
        _emVoo = true;
        var rotacaoAtiva = _timerRotacao.Enabled;
        if (rotacaoAtiva)
            _timerRotacao.Enabled = false;
        await Voar(velocidade, levarMouse);
        if (rotacaoAtiva)
            _timerRotacao.Enabled = true;
    }

    public async Task RoubarMouse(int esperarMilissegundos = 0)
    {
        await Task.Delay(esperarMilissegundos);
        var mousePos = Cursor.Position;
        var destinoMouse = new Point(mousePos.X - (this.Width / 2) + 50, mousePos.Y - (this.Height / 2) + 10);
        await VoarPara(destinoMouse, 150);
        await VoarPara(DestinoVoarAleatorio(), 80, true);
    }

    private Point DestinoVoarAleatorio()
    {
        var screens = Screen.AllScreens;
        var screen = screens[_random.Next(screens.Length)];
        int margem = 10;
        int maxX = screen.WorkingArea.Right - this.Width - margem;
        int maxY = screen.WorkingArea.Bottom - this.Height - margem;
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

    private void TimerMoverPernas_Tick(object? sender, EventArgs e)
    {
        MoverPatinhas();
        ProximoIntervaloMoverPernas();
    }

    private void TimerRotacao_Tick(object? sender, EventArgs e)
    {
        RotacionarMoscaAleatorio();
        ProximoIntervaloRotacao();
    }

    private bool TemComida()
    {
        return Application.OpenForms.OfType<frmComida>().Count() > 0;
    }

    private List<frmComida> ListaComidas()
    {
        return Application.OpenForms.OfType<frmComida>().ToList();
    }

    private async Task TimerVoar_Tick(object? sender, EventArgs e)
    {
        if (_emVoo)
            return;
        Point destino;
        if (ComportamentoMouse == ComportamentoMouseEnum.SeguirMouse && _random.NextDouble() < 0.95)
        {
            var mousePos = Cursor.Position;
            destino = new Point(mousePos.X - (this.Width / 2) + 50, mousePos.Y - (this.Height / 2) + 10);
        }
        else if (ComportamentoMouse != ComportamentoMouseEnum.SeguirMouse && TemComida() && _random.NextDouble() < 0.95)
        {
            var comidas = ListaComidas();
            var comida = comidas[_random.Next(comidas.Count)];
            var offsetX = _random.Next(-9, 10);
            var offsetY = _random.Next(-9, 10);
            var comidaCentro = new Point(
                comida.Left + comida.Width / 2 - this.Width / 2 + 15 + offsetX,
                comida.Top + comida.Height / 2 - this.Height / 2 + 5 + offsetY
            );
            destino = comidaCentro;
        }
        else
        {
            destino = DestinoVoarAleatorio();
        }
        await VoarPara(destino);
    }

    private async Task Voar(int velocidade = 0, bool levarMouse = false)
    {
        if (ComSom && _waveOut != null && _mp3Reader != null)
        {
            _mp3Reader.Position = 0;
            float[] volumes = { 1.0f, 0.8f, 0.5f, 0.3f, 0.1f };
            _waveOut.Volume = volumes[_random.Next(volumes.Length)];
            _waveOut.Play();
            await Task.Delay(100);
        }
        velocidade = velocidade == 0 ? _random.Next(80, 201) : velocidade;
        while (true)
        {
            int dx = _destinoVoar.X - this.Left;
            int dy = _destinoVoar.Y - this.Top;
            int dist = (int)Math.Sqrt(dx * dx + dy * dy);
            if (dist < velocidade)
            {
                this.Left = _destinoVoar.X;
                this.Top = _destinoVoar.Y;
                if (levarMouse)
                    Cursor.Position = new Point(this.Left + this.Width / 2 - 50, this.Top + this.Height / 2 - 10);
                break;
            }
            await ApontarParaDestinoVoar();
            double ang = Math.Atan2(dy, dx);
            int nx = this.Left + (int)(velocidade * Math.Cos(ang));
            int ny = this.Top + (int)(velocidade * Math.Sin(ang));
            this.Left = nx;
            this.Top = ny;
            if (levarMouse)
                Cursor.Position = new Point(this.Left + this.Width / 2 - 50, this.Top + this.Height / 2 - 10);
            await Task.Delay(8);
        }
        if (_waveOut != null)
            _waveOut.Stop();
        _emVoo = false;
        ProximoIntervaloVoar();
    }

    private async Task ApontarParaDestinoVoar()
    {
        await ApontarPara(_destinoVoar.X, _destinoVoar.Y);
    }

    public async Task ApontarPara(int posX, int posY)
    {
        var point = new Point(posX, posY);
        await ApontarPara(point);
    }

    public async Task ApontarPara(Point point)
    {
        int dx = point.X - this.Left;
        int dy = point.Y - this.Top;
        // ângulo em radianos, 0 é para a direita, positivo sentido anti-horário
        double angRad = Math.Atan2(dy, dx);
        // converte para graus
        int angDeg = (int)(angRad * 180 / Math.PI);
        // ajusta para que 0 seja para cima (em vez de direita)
        int anguloMosca = (angDeg + 90 + 360) % 360;
        AplicarAngulo(anguloMosca);
        Application.DoEvents();
    }

    public void AplicarAngulo(int angulo)
    {
        if (picMosca?.Image != null)
        {
            _anguloAtual = angulo;
            picMosca.Image = RotacionarImagem(_anguloAtual);
            Application.DoEvents();
        }
    }

    private void ProximoIntervaloMoverPernas()
    {
        int nextInterval = _random.Next(500, 1500);
        _timerMoverPernas.Interval = nextInterval;
    }

    private void ProximoIntervaloRotacao()
    {
        int nextInterval = _random.Next(2000, 5000);
        _timerRotacao.Interval = nextInterval;
    }

    private void ProximoIntervaloVoar()
    {
        int nextInterval = _random.Next(500, 10001);
        _timerVoar.Interval = nextInterval;
        _emVoo = false;
    }

    private void MoverPatinhas()
    {
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (img != null && picMosca != null)
        {
            if (_anguloAtual != 0)
                picMosca.Image = RotacionarImagem(_anguloAtual, img);
            else
                picMosca.Image = img;
        }
    }

    private async Task RotacionarSuaveAte(int anguloDestino, int passo = 30, int delayMs = 10)
    {
        int atual = _anguloAtual;
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

    private async void RotacionarMoscaAleatorio()
    {
        if (picMosca?.Image == null) return;

        // 5% chance de rodopio
        if (_random.NextDouble() < 0.03)
        {
            int numeroVoltas = _random.Next(1, 3); // 2, 3 ou 4
            bool sentidoHorario = _random.Next(2) == 0;
            await Rodopio(numeroVoltas, sentidoHorario);
        }
        else
        {
            int angle = _random.Next(0, 360);
            bool esquerda = _random.Next(2) == 0;
            if (!esquerda) angle = 360 - angle;
            var novoAngulo = (_anguloAtual + angle) % 360;
            await RotacionarSuaveAte(novoAngulo);
        }
    }

    private Image RotacionarImagem(float angle, Image? img = null)
    {
        if (img == null)
        {
            string imgName = _moscaImages[0];
            img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
            if (img == null)
                throw new ArgumentException($"Imagem {imgName} não está nos resources");
        }
        Bitmap bmp = new Bitmap(img.Width, img.Height);
        bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
            g.DrawImage(img, new Point(0, 0));
        }
        return bmp;
    }

    private void PicMosca_MouseWheel(object? sender, MouseEventArgs e)
    {
        if (picMosca?.Image == null) return;
        int quantosGraus = 40;
        int delta = e.Delta > 0 ? quantosGraus * -1 : quantosGraus;
        var novoAngulo = (_anguloAtual + delta + 360) % 360;
        AplicarAngulo(novoAngulo);
    }

    private void PicMosca_MouseDown(object? sender, MouseEventArgs e)
    {
        if (PermitirAgarrarSoltar && e.Button == MouseButtons.Left)
        {
            _dragging = true;
            _dragOffset = new Point(e.X, e.Y);
        }
    }

    private void PicMosca_MouseMove(object? sender, MouseEventArgs e)
    {
        if (PermitirAgarrarSoltar && _dragging)
        {
            var mousePos = MousePosition;
            this.Location = new Point(mousePos.X - _dragOffset.X, mousePos.Y - _dragOffset.Y);
        }
    }

    private void PicMosca_MouseUp(object? sender, MouseEventArgs e)
    {
        if (PermitirAgarrarSoltar && e.Button == MouseButtons.Left)
        {
            _dragging = false;
        }
    }

    public void Morrer()
    {
        // Para os timers
        _timerMoverPernas.Stop();
        _timerMoverPernas.Dispose();
        _timerRotacao.Stop();
        _timerRotacao.Dispose();
        _timerVoar.Stop();
        _timerVoar.Dispose();

        // Para o áudio
        if (_waveOut != null)
        {
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }
        if (_mp3Reader != null)
        {
            _mp3Reader.Dispose();
            _mp3Reader = null;
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
        this.Left -= 100;
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
            int novoAngulo = (_anguloAtual + direcao * passo + 360) % 360;
            await RotacionarSuaveAte(novoAngulo);
            Application.DoEvents();
        }
    }

    public async Task PatinhasBoomerang(int quantasVezes)
    {
        int idx1 = _random.Next(_moscaImages.Length);
        int idx2;
        do
        {
            idx2 = _random.Next(_moscaImages.Length);
        } while (idx2 == idx1);
        for (int i = 0; i < quantasVezes; i++)
        {
            foreach (var idx in new[] { idx1, idx2 })
            {
                var img = (Image?)Properties.Resources.ResourceManager.GetObject(_moscaImages[idx]);
                if (img != null && picMosca != null)
                    picMosca.Image = RotacionarImagem(_anguloAtual, img);
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
        int centerX = screen.Left + (screen.Width - this.Width) / 2;
        int centerY = screen.Top + (screen.Height - this.Height) / 2;
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
    }

    public void AtivarTimerVoar(bool ativar)
    {
        if (ativar)
        {
            ProximoIntervaloVoar();
            _timerVoar.Start();
        }
        else
        {
            _timerVoar.Stop();
        }
    }

    public void AtivarTimerRotacao(bool ativar)
    {
        if (ativar)
        {
            ProximoIntervaloRotacao();
            _timerRotacao.Start();
        }
        else
        {
            _timerRotacao.Stop();
        }
    }

    public void AtivarTimerMoverPernas(bool ativar)
    {
        if (ativar)
        {
            ProximoIntervaloMoverPernas();
            _timerMoverPernas.Start();
        }
        else
        {
            _timerMoverPernas.Stop();
        }
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

}