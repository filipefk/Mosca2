namespace Mosca2;

public partial class frmPrincipal : Form
{
    private List<frmMosca> moscas = [];
    private NotifyIcon? notifyIcon;
    private ContextMenuStrip? trayMenu;
    private int quantasMoscas = 1;

    public frmPrincipal()
    {
        InitializeComponent();
        // Inicializa o menu de contexto
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Dar comida", null, (s, e) => DarComida());
        trayMenu.Items.Add("Mais mosca", null, (s, e) => MaisMosca());
        trayMenu.Items.Add("Menos mosca", null, (s, e) => MenosMosca());
        trayMenu.Items.Add("Roubar o mouse", null, (s, e) => RoubarOMouse());
        trayMenu.Items.Add("Com som", null, (s, e) => ComSom());
        trayMenu.Items.Add("Sem som", null, (s, e) => SemSom());
        trayMenu.Items.Add("Seguir o mouse", null, (s, e) => SeguirOMouse());
        trayMenu.Items.Add("Fugir do mouse", null, (s, e) => FugirDoMouse());
        trayMenu.Items.Add("Iniciar comando aleatórios", null, (s, e) => IniciarComandosAleatorios());
        trayMenu.Items.Add("Parar comando aleatórios", null, (s, e) => PararComandosAleatorios());
        trayMenu.Items.Add("Dança loca", null, (s, e) => DancaLoca());
        trayMenu.Items.Add("Fila indiana", null, (s, e) => FilaIndiana());
        trayMenu.Items.Add("Matar moscas", null, (s, e) => MatarMoscas());

        // Inicializa o NotifyIcon com o menu de contexto
        notifyIcon = new();
        notifyIcon.Icon = this.Icon;
        notifyIcon.Text = "Bzzzzzz";
        notifyIcon.ContextMenuStrip = trayMenu;
        notifyIcon.Visible = true;
        notifyIcon.MouseUp += NotifyIcon_MouseUp;
    }

    private void NotifyIcon_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && trayMenu != null)
        {
            trayMenu.Show(Cursor.Position.X - trayMenu.Width, Cursor.Position.Y - trayMenu.Height);
        }
    }

    private async void DarComida()
    {
        var comida = new frmComida();
        await comida.Mostrar();
    }

    private void MaisMosca()
    {
        AdicionarNovaMosca();
    }

    private void MenosMosca()
    {
        if (moscas.Count > 0)
        {
            var mosca = moscas[0];
            moscas.RemoveAt(0);
            mosca.Morrer();
        }
    }

    private async void RoubarOMouse()
    {
        if (moscas.Count > 0)
        {
            await moscas[0].RoubarMouse(3000);
        }
    }

    private void ComSom()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComSom = true;
        }
    }

    private void SemSom()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComSom = false;
        }
    }

    private void SeguirOMouse()
    {
        foreach (var mosca in moscas)
        {
            mosca.SeguirMouse = true;
        }
    }

    private void FugirDoMouse()
    {
        foreach (var mosca in moscas)
        {
            mosca.SeguirMouse = false;
        }
    }

    private void IniciarComandosAleatorios() { }
    private void PararComandosAleatorios() { }

    private async void DancaLoca()
    {
        var tasks = moscas.Select(mosca => mosca.DancaLoca());
        await Task.WhenAll(tasks);
    }

    private async void FilaIndiana()
    {
        //var tasks = moscas.Select(mosca => mosca.FilaIndiana());
        foreach (var mosca in moscas)
        {
            mosca.AtivarTimers(false);
        }
        //var tasks = moscas.Select(mosca => mosca.AtivarTimers(false));
        //await Task.WhenAll(tasks);
    }

    private void MatarMoscas()
    {
        Application.Exit();
    }

    private void MostrarMoscas(int quantas)
    {
        while (moscas.Count < quantas)
        {
            AdicionarNovaMosca();
        }
        while (moscas.Count > quantas)
        {
            var mosca = moscas[0];
            moscas.RemoveAt(0);
            mosca.Morrer();
        }
    }

    public void AdicionarNovaMosca()
    {
        var mosca = new frmMosca();
        mosca.Indice = moscas.Count;
        mosca.Show();
        moscas.Add(mosca);
    }

    private void frmPrincipal_Load(object sender, EventArgs e)
    {
        FormTransparente();
        MostrarMoscas(quantasMoscas);
        this.Hide();
    }

    private void FormTransparente()
    {
        this.TransparencyKey = Color.White;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;
    }
}
