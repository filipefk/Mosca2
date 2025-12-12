namespace Mosca2;

public partial class frmPrincipal : Form
{
    private List<frmMosca> moscas = [];
    private NotifyIcon? notifyIcon;
    private ContextMenuStrip? trayMenu;
    private int quantasMoscas = 1;
    private GlobalMouseHook? globalMouseHook;


    // AQUI ATIVA E DESATIVA O CONTROLE REMOTO
    private static readonly bool ModoControleRemoto = true;

    public frmPrincipal()
    {
        InitializeComponent();
        notifyIcon = new();
        notifyIcon.Icon = this.Icon;
        notifyIcon.Text = "Bzzzzzz";
        notifyIcon.Visible = true;

        if (!ModoControleRemoto)
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Dar comida", null, async (s, e) => await MenuDarComida_Click());
            trayMenu.Items.Add("Mais mosca", null, (s, e) => MenuMaisMosca_Click());
            trayMenu.Items.Add("Menos mosca", null, (s, e) => MenuMenosMosca_Click());
            trayMenu.Items.Add("Roubar o mouse", null, async (s, e) => await MenuRoubarOMouse_Click());
            trayMenu.Items.Add("Com som", null, (s, e) => MenuComSom_Click());
            trayMenu.Items.Add("Sem som", null, (s, e) => MenuSemSom_Click());
            trayMenu.Items.Add("Seguir o mouse", null, (s, e) => MenuSeguirMouse_Click());
            trayMenu.Items.Add("Fugir do mouse", null, (s, e) => MenuFugirDoMouse_Click());
            trayMenu.Items.Add("Ignorar o mouse", null, (s, e) => MenuIgnorarMouse_Click());
            trayMenu.Items.Add("Iniciar comando aleatórios", null, (s, e) => MenuIniciarComandosAleatorios_Click());
            trayMenu.Items.Add("Parar comando aleatórios", null, (s, e) => MenuPararComandosAleatorios_Click());
            trayMenu.Items.Add("Dança loca", null, async (s, e) => await DancaLoca());
            trayMenu.Items.Add("Fila indiana", null, async (s, e) => await FilaIndiana());
            trayMenu.Items.Add("Roda gigante", null, async (s, e) => await RodaGigante());
            trayMenu.Items.Add("Matar moscas", null, (s, e) => MatarMoscas());

            notifyIcon.ContextMenuStrip = trayMenu;
            notifyIcon.MouseUp += NotifyIcon_MouseUp;
        }

        globalMouseHook = new GlobalMouseHook();
        globalMouseHook.MouseMoved += async (s, e) => await GlobalMouseHook_MouseMoved(s, e);
    }

    private async Task GlobalMouseHook_MouseMoved(object? sender, Point e)
    {
        if (lblPosicaoMouse.InvokeRequired)
        {
            lblPosicaoMouse.Invoke(new Action(() =>
            {
                lblPosicaoMouse.Text = $"Posição do mouse: X = {e.X}, Y = {e.Y}";
            }));
        }
        else
        {
            lblPosicaoMouse.Text = $"Posição do mouse: X = {e.X}, Y = {e.Y}";
        }
        if (chkApontarParaMouse.Checked)
        {
            Parallel.ForEach(moscas, async mosca => await mosca.ApontarPara(e));
        }
    }

    private void frmPrincipal_Load(object sender, EventArgs e)
    {
        if (ModoControleRemoto)
        {
            this.ShowInTaskbar = true;
            PreencheComboComportamento();
            MostrarMoscas(1);
            AjustaPropriedades();
            this.TopMost = true;
            Application.DoEvents();
            this.TopMost = false;
        }
        else
        {
            this.ShowInTaskbar = false;
            FormTransparente();
            MostrarMoscas(quantasMoscas);
            this.Hide();
        }
    }

    private void PreencheComboComportamento()
    {
        cboComportamentoMouse.Items.Clear();
        cboComportamentoMouse.Items.Add("Fugir do mouse");
        cboComportamentoMouse.Items.Add("Seguir o mouse");
        cboComportamentoMouse.Items.Add("Ignorar o mouse");
        cboComportamentoMouse.SelectedIndex = 0;
    }

    private void AjustaPropriedades()
    {
        if (ModoControleRemoto)
        {
            foreach (var mosca in moscas)
            {
                mosca.AtivarTimerMoverPernas(chkTimerMoverPernas.Checked);
                mosca.AtivarTimerRotacao(chkTimerRotacao.Checked);
                mosca.AtivarTimerVoar(chkTimerVoar.Checked);
                mosca.ComportamentoMouse = (frmMosca.ComportamentoMouseEnum)cboComportamentoMouse.SelectedIndex;
                mosca.ComSom = chkComSom.Checked;
                mosca.PermitirAgarrarSoltar = chkPermitirAgarrarSoltar.Checked;
                mosca.MostrarIndice = chkMostrarIndice.Checked;
            }
            lblTotalDeMoscas.Text = $"Total de moscas: {moscas.Count}";
        }
    }

    private void NotifyIcon_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && trayMenu != null)
        {
            trayMenu.Show(Cursor.Position.X - trayMenu.Width, Cursor.Position.Y - trayMenu.Height);
        }
    }

    private async Task MenuDarComida_Click()
    {
        var comida = new frmComida();
        await comida.Mostrar();
    }

    private void MenuMaisMosca_Click()
    {
        AdicionarNovaMosca();
    }

    private void MenuMenosMosca_Click()
    {
        RemoverUmaMosca();
    }

    private async Task MenuRoubarOMouse_Click()
    {
        if (moscas.Count > 0)
        {
            await moscas[0].RoubarMouse(3000);
        }
    }

    private void MenuComSom_Click()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComSom = true;
        }
    }

    private void MenuSemSom_Click()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComSom = false;
        }
    }

    private void MenuSeguirMouse_Click()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComportamentoMouse = frmMosca.ComportamentoMouseEnum.SeguirMouse;
        }
    }

    private void MenuFugirDoMouse_Click()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComportamentoMouse = frmMosca.ComportamentoMouseEnum.FugirMouse;
        }
    }

    private void MenuIgnorarMouse_Click()
    {
        foreach (var mosca in moscas)
        {
            mosca.ComportamentoMouse = frmMosca.ComportamentoMouseEnum.IgnorarMouse;
        }
    }



    private void MenuIniciarComandosAleatorios_Click() { }
    private void MenuPararComandosAleatorios_Click() { }

    private async Task DancaLoca()
    {
        var tasks = moscas.Select(mosca => mosca.DancaLoca());
        await Task.WhenAll(tasks);
    }

    private async Task FilaIndiana()
    {
        var tasks = moscas.Select(mosca => mosca.FilaIndiana());
        await Task.WhenAll(tasks);
        AjustaPropriedades();
    }

    private async Task FormacaoQuadrada()
    {
        var tasks = moscas.Select(mosca => mosca.FormacaoQuadrada());
        await Task.WhenAll(tasks);
    }

    private async Task RodaGigante()
    {
        var tasks = moscas.Select(mosca => mosca.RodaGigante());
        await Task.WhenAll(tasks);
    }

    private void MatarMoscas()
    {
        Application.Exit();
    }

    private async Task EntrarEmForma()
    {
        cboComportamentoMouse.SelectedIndex = 2;
        chkTimerMoverPernas.Checked = false;
        await FormacaoQuadrada();
        AjustaPropriedades();
        chkTimerRotacao.Checked = false;
        chkTimerVoar.Checked = false;
        chkComSom.Checked = false;
        chkPermitirAgarrarSoltar.Checked = true;
        chkApontarParaMouse.Checked = true;
        AjustaPropriedades();
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

    public void RemoverUmaMosca()
    {
        if (moscas.Count > 0)
        {
            var mosca = moscas[0];
            moscas.RemoveAt(0);
            mosca.Morrer();
            for (int i = 0; i < moscas.Count; i++)
            {
                moscas[i].Indice = i;
            }
        }
    }

    private void FormTransparente()
    {
        this.TransparencyKey = Color.White;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;
    }

    private void chkTimers_CheckedChanged(object sender, EventArgs e)
    {
        AjustaPropriedades();
    }

    private void btMaisUmaMosca_Click(object sender, EventArgs e)
    {
        MenuMaisMosca_Click();
        AjustaPropriedades();
    }

    private void btMenosUmaMosca_Click(object sender, EventArgs e)
    {
        MenuMenosMosca_Click();
        AjustaPropriedades();
    }

    private async void hsbAngulo_Scroll(object sender, ScrollEventArgs e)
    {
        lblAngulo.Text = $"{hsbAngulo.Value} °";
        foreach (var mosca in moscas)
        {
            mosca.AplicarAngulo(hsbAngulo.Value);
        }
    }

    private async void btEstrarEmForma_Click(object sender, EventArgs e)
    {
        await EntrarEmForma();
    }

    private async void btFilaIndiana_Click(object sender, EventArgs e)
    {
        await FilaIndiana();
    }

    private async void btRodaGigante_Click(object sender, EventArgs e)
    {
        await RodaGigante();
    }

    private async void btDancaLoca_Click(object sender, EventArgs e)
    {
        await DancaLoca();
    }

    private async void btDarComida_Click(object sender, EventArgs e)
    {
        var comida = new frmComida();
        await comida.Mostrar();
    }

    private async void btFormacaoQuadrada_Click(object sender, EventArgs e)
    {
        await FormacaoQuadrada();
    }

    private void nudVelocidade_ValueChanged(object sender, EventArgs e)
    {
        foreach (var mosca in moscas)
        {
            mosca.VelocidadePadrao = (uint)nudVelocidade.Value;
        }
    }

    private void nudVelocidade_KeyPress(object sender, KeyPressEventArgs e)
    {

    }
}
