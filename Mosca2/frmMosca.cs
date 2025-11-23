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
    private bool _SeguirMouse = false;

    public frmMosca()
    {
        InitializeComponent();

        this.BackColor = Color.DarkGray;
        this.TransparencyKey = this.BackColor;
        this.ShowInTaskbar = true;
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

    private async Task TimerVoar_Tick(object? sender, EventArgs e)
    {
        if (_emVoo)
            return;
        // Sorteia uma tela e uma posição visível
        var screens = Screen.AllScreens;
        var screen = screens[_random.Next(screens.Length)];
        int maxX = screen.WorkingArea.Right - this.Width;
        int maxY = screen.WorkingArea.Bottom - this.Height;
        int minX = screen.WorkingArea.Left;
        int minY = screen.WorkingArea.Top;
        int x = _random.Next(minX, maxX > minX ? maxX : minX + 1);
        int y = _random.Next(minY, maxY > minY ? maxY : minY + 1);
        _destinoVoar = new Point(x, y);
        _emVoo = true;
        _timerRotacao.Enabled = false; // Desativa rotação
        await Voar();
        _timerRotacao.Enabled = true; // Reativa rotação
    }

    private async Task Voar()
    {
        const int velocidade = 150;
        while (true)
        {
            int dx = _destinoVoar.X - this.Left;
            int dy = _destinoVoar.Y - this.Top;
            int dist = (int)Math.Sqrt(dx * dx + dy * dy);
            if (dist < velocidade)
            {
                this.Left = _destinoVoar.X;
                this.Top = _destinoVoar.Y;
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
            await Task.Delay(8);
        }
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
}