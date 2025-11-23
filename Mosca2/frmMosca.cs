using NAudio.Wave;

namespace Mosca2;

public partial class frmMosca : Form
{
    private readonly System.Windows.Forms.Timer _timerMoverPernas;
    private readonly System.Windows.Forms.Timer _timerRotacao;
    private readonly System.Windows.Forms.Timer _timerVoar;
    private readonly Random _random = new();
    private readonly string[] _moscaImages = { "Mosca1", "Mosca2", "Mosca3", "Mosca4", "Mosca5" };
    private int _anguloAtual = 0;
    private Point _destinoVoar;
    private bool _emVoo = false;
    private IWavePlayer? _waveOut;
    private WaveStream? _mp3Reader;

    public frmMosca()
    {
        InitializeComponent();

        this.BackColor = Color.FromArgb(128, 128, 128);
        this.TransparencyKey = this.BackColor;
        this.ShowInTaskbar = false;
        this.TopMost = true;

        _timerMoverPernas = new System.Windows.Forms.Timer();
        _timerMoverPernas.Tick += async (s, e) => await TimerMoverPernas_Tick(s, e);
        ProximoIntervaloMoverPernasAsync().GetAwaiter().GetResult();
        _timerMoverPernas.Start();

        _timerRotacao = new System.Windows.Forms.Timer();
        _timerRotacao.Tick += async (s, e) => await TimerRotacao_Tick(s, e);
        ProximoIntervaloRotacaoAsync().GetAwaiter().GetResult();
        _timerRotacao.Start();

        _timerVoar = new System.Windows.Forms.Timer();
        _timerVoar.Tick += async (s, e) => await TimerVoar_Tick(s, e);
        ProximoIntervaloVoarAsync().GetAwaiter().GetResult();
        _timerVoar.Start();

        if (picMosca != null)
        {
            picMosca.MouseEnter += PicMosca_MouseEnter;
            picMosca.MouseWheel += PicMosca_MouseWheel;
            picMosca.MouseClick += PicMosca_MouseClick;
        }

        var mp3Stream = new MemoryStream();
        Properties.Resources.Som_de_Mosca.CopyTo(mp3Stream);
        mp3Stream.Position = 0;
        _mp3Reader = new Mp3FileReader(mp3Stream);
        _waveOut = new WaveOutEvent();
        _waveOut.Init(_mp3Reader);

        this.StartPosition = FormStartPosition.Manual;
        this.Location = DestinoVoarAleatorio();
    }

    public bool SeguirMouse = false;

    public bool ComSom = false;

    private async void PicMosca_MouseEnter(object? sender, EventArgs e)
    {
        if (SeguirMouse) return;
        if (_emVoo) return;
        await Fugir();
    }

    private async void PicMosca_MouseClick(object? sender, EventArgs e)
    {
        if (_emVoo) return;
        await Fugir();
    }

    public async Task Fugir()
    {
        await VoarPara(DestinoVoarAleatorio());
    }

    public async Task VoarPara(Point destino, int velocidade = 0, bool levarMouse = false)
    {
        _destinoVoar = destino;
        await VoarPara(velocidade, levarMouse);
    }

    public async Task VoarPara(int velocidade = 0, bool levarMouse = false)
    {
        _emVoo = true;
        _timerRotacao.Enabled = false;
        await Voar(velocidade, levarMouse);
        _timerRotacao.Enabled = true;
    }

    public async Task RoubarMouse()
    {
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

    private async Task TimerMoverPernas_Tick(object? sender, EventArgs e)
    {
        await TrocarImagemMoscaAsync();
        await ProximoIntervaloMoverPernasAsync();
    }

    private async Task TimerRotacao_Tick(object? sender, EventArgs e)
    {
        await RotacionarImagemMoscaAsync();
        await ProximoIntervaloRotacaoAsync();
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
        if (SeguirMouse && _random.NextDouble() < 0.95)
        {
            var mousePos = Cursor.Position;
            // Centro da mosca exatamente na ponta do mouse
            destino = new Point(mousePos.X - (this.Width / 2) + 50, mousePos.Y - (this.Height / 2) + 10);
        }
        else if (!SeguirMouse && TemComida() && _random.NextDouble() < 0.95)
        {
            var comidas = ListaComidas();
            // Sorteia um dos frmComida abertos
            var comida = comidas[_random.Next(comidas.Count)];
            // Sorteia variações de -5 a +5 pixels
            var offsetX = _random.Next(-9, 10);
            var offsetY = _random.Next(-9, 10);
            // Define o destino para o centro do frmComida sorteado, com variação
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
        int[] velocidades = { 80, 100, 130, 150 };
        velocidade = velocidade == 0 ? velocidades[_random.Next(velocidades.Length)] : velocidade;
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
            double ang = Math.Atan2(dy, dx);
            // Ajuste: imagem original aponta para 135 graus (diagonal esquerda/cima)
            int anguloMosca = (int)(ang * 180 / Math.PI) + 135;
            _anguloAtual = ((anguloMosca % 360) + 360) % 360;
            if (picMosca?.Image != null)
            {
                // Sempre rotaciona a imagem base
                string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
                var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
                if (img != null)
                    picMosca.Image = RotacionarImagem(img, _anguloAtual);
            }
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
        await ProximoIntervaloVoarAsync();
    }

    private async Task ProximoIntervaloMoverPernasAsync()
    {
        int[] intervals = { 200, 300, 500, 800 };
        int nextInterval = intervals[_random.Next(intervals.Length)];
        _timerMoverPernas.Interval = nextInterval;
        await Task.CompletedTask;
    }

    private async Task ProximoIntervaloRotacaoAsync()
    {
        int[] intervals = { 1000, 2000, 3000 };
        int nextInterval = intervals[_random.Next(intervals.Length)];
        _timerRotacao.Interval = nextInterval;
        await Task.CompletedTask;
    }

    private async Task ProximoIntervaloVoarAsync()
    {
        int[] intervals = { 2000, 3000, 4000 };
        int nextInterval = intervals[_random.Next(intervals.Length)];
        _timerVoar.Interval = nextInterval;
        _emVoo = false;
        await Task.CompletedTask;
    }

    private async Task TrocarImagemMoscaAsync()
    {
        // Sorteia uma das imagens
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (img != null && picMosca != null)
        {
            if (_anguloAtual != 0)
                picMosca.Image = RotacionarImagem(img, _anguloAtual);
            else
                picMosca.Image = img;
        }
        await Task.CompletedTask;
    }

    private async Task RotacionarImagemMoscaAsync()
    {
        if (picMosca?.Image == null) { await Task.CompletedTask; return; }
        int[] angles = { 30, 50, 90 };
        int angle = angles[_random.Next(angles.Length)];
        bool esquerda = _random.Next(2) == 0;
        if (!esquerda) angle = 360 - angle;
        _anguloAtual = (_anguloAtual + angle) % 360;
        // Sempre rotaciona a imagem base
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (picMosca.Image != null)
            img = picMosca.Image;
        if (img != null)
            picMosca.Image = RotacionarImagem(img, _anguloAtual);
        await Task.CompletedTask;
    }

    private static Image RotacionarImagem(Image img, float angle)
    {
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
        _anguloAtual = (_anguloAtual + delta + 360) % 360;
        // Sempre rotaciona a imagem base
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (img != null)
            picMosca.Image = RotacionarImagem(img, _anguloAtual);
    }
}