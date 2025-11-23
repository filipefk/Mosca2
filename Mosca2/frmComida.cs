namespace Mosca2;

public partial class frmComida : Form
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Random _random = new();
    private Point _dragOffset;
    private bool _dragging = false;

    public frmComida()
    {
        InitializeComponent();
        _timer = new System.Windows.Forms.Timer();
        // Eventos para arrastar o form pelo picComida
        picComida.MouseDown += PicComida_MouseDown;
        picComida.MouseMove += PicComida_MouseMove;
        picComida.MouseUp += PicComida_MouseUp;
    }

    private void MoscasTopMost()
    {
        foreach (Form mosca in Application.OpenForms.OfType<frmMosca>())
        {
            mosca.TopMost = false;
            mosca.TopMost = true;
        }
    }

    public async Task Mostrar()
    {
        this.BackColor = Color.DarkGray;
        this.TransparencyKey = this.BackColor;
        this.ShowInTaskbar = false;
        this.Show();
        this.TopMost = true;

        // Sorteia imagem
        if (_random.Next(2) == 0)
            picComida.Image = Properties.Resources.Merda;
        else
            picComida.Image = Properties.Resources.Lixo;

        // Sorteia tempo de exibição
        int tempoMs = _random.Next(10, 31) * 1000;
        
        _timer.Interval = tempoMs;
        _timer.Tick += (s, e) => { _timer.Stop(); this.Close(); };
        _timer.Start();

        // Sorteia posição em uma das telas, evitando bordas (20px)
        var screens = Screen.AllScreens;
        var screen = screens[_random.Next(screens.Length)];
        int margem = 20;
        int maxX = screen.WorkingArea.Right - this.Width - margem;
        int maxY = screen.WorkingArea.Bottom - this.Height - margem;
        int minX = screen.WorkingArea.Left + margem;
        int minY = screen.WorkingArea.Top + margem;
        int x = _random.Next(minX, Math.Max(minX + 1, maxX));
        int y = _random.Next(minY, Math.Max(minY + 1, maxY));
        this.StartPosition = FormStartPosition.Manual;
        this.Location = new Point(x, y);

        MoscasTopMost();
    }

    private void PicComida_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragging = true;
            _dragOffset = new Point(e.X, e.Y);
        }
    }

    private void PicComida_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_dragging)
        {
            var mousePos = MousePosition;
            this.Location = new Point(mousePos.X - _dragOffset.X, mousePos.Y - _dragOffset.Y);
            MoscasTopMost();
        }
    }

    private void PicComida_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragging = false;
        }
    }
}
