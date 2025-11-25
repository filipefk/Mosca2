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
        if (SeguirMouse || _emVoo) return;
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

    private void TimerMoverPernas_Tick(object? sender, EventArgs e)
    {
        TrocarImagemMosca();
        ProximoIntervaloMoverPernas();
    }

    private void TimerRotacao_Tick(object? sender, EventArgs e)
    {
        RotacionarImagemMosca();
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
        if (SeguirMouse && _random.NextDouble() < 0.95)
        {
            var mousePos = Cursor.Position;
            destino = new Point(mousePos.X - (this.Width / 2) + 50, mousePos.Y - (this.Height / 2) + 10);
        }
        else if (!SeguirMouse && TemComida() && _random.NextDouble() < 0.95)
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
            int anguloMosca = (int)(ang * 180 / Math.PI) + 135;
            _anguloAtual = ((anguloMosca % 360) + 360) % 360;
            if (picMosca?.Image != null)
            {
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
        ProximoIntervaloVoar();
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

    private void TrocarImagemMosca()
    {
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (img != null && picMosca != null)
        {
            if (_anguloAtual != 0)
                picMosca.Image = RotacionarImagem(img, _anguloAtual);
            else
                picMosca.Image = img;
        }
    }

    private void RotacionarImagemMosca()
    {
        if (picMosca?.Image == null) return;
        int[] angles = { 30, 50, 90 };
        int angle = angles[_random.Next(angles.Length)];
        bool esquerda = _random.Next(2) == 0;
        if (!esquerda) angle = 360 - angle;
        _anguloAtual = (_anguloAtual + angle) % 360;
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (picMosca.Image != null)
            img = picMosca.Image;
        if (img != null)
            picMosca.Image = RotacionarImagem(img, _anguloAtual);
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
        string imgName = _moscaImages[_random.Next(_moscaImages.Length)];
        var img = (Image?)Properties.Resources.ResourceManager.GetObject(imgName);
        if (img != null)
            picMosca.Image = RotacionarImagem(img, _anguloAtual);
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
        // 1 - Desativa todos os timers de movimento
        _timerMoverPernas.Stop();
        _timerRotacao.Stop();
        _timerVoar.Stop();

        // 2 - Posicionar com a cabeça apontando pra cima
        _anguloAtual = 35;
        picMosca.Image = RotacionarImagem(picMosca.Image!, _anguloAtual);

        await Task.Delay(200);

        await PatinhasBoomerang(10);

        await Rodopio(5, true);

        // 5 - Mover um tanto para a esquerda
        this.Left -= 100;
        await Task.Delay(150);

        _anguloAtual = 215;
        picMosca.Image = RotacionarImagem(picMosca.Image!, _anguloAtual);

        await PatinhasBoomerang(10);

        await Rodopio(5, false);

        // 8 - Voltar tudo ao normal, reativar todos os timers
        ProximoIntervaloMoverPernas();
        ProximoIntervaloRotacao();
        ProximoIntervaloVoar();
        _timerMoverPernas.Start();
        _timerRotacao.Start();
        _timerVoar.Start();
    }

    public async Task Rodopio(int numeroVoltas, bool sentidoHorario)
    {
        var grausIncremento = 20;
        int incremento = sentidoHorario ? grausIncremento : -grausIncremento;
        int passosPorVolta = 360 / grausIncremento;
        int totalPassos = numeroVoltas * passosPorVolta;
        int delayMs = 800 / passosPorVolta; // para ~1 volta por segundo
        for (int i = 0; i < totalPassos; i++)
        {
            _anguloAtual = (_anguloAtual + incremento + 360) % 360;
            if (picMosca != null && picMosca.Image != null)
                picMosca.Image = RotacionarImagem(picMosca.Image, _anguloAtual);
            await Task.Delay(delayMs);
        }
    }

    public async Task PatinhasBoomerang(int quantasVezes)
    {
        int idx1 = _random.Next(_moscaImages.Length);
        int idx2;
        do {
            idx2 = _random.Next(_moscaImages.Length);
        } while (idx2 == idx1);
        for (int i = 0; i < quantasVezes; i++)
        {
            foreach (var idx in new[] { idx1, idx2 })
            {
                var img = (Image?)Properties.Resources.ResourceManager.GetObject(_moscaImages[idx]);
                if (img != null && picMosca != null)
                    picMosca.Image = RotacionarImagem(img, _anguloAtual);
                await Task.Delay(150);
            }
        }
    }
}