namespace Mosca2;

public partial class frmPrincipal : Form
{
    private frmMosca[] forms = new frmMosca[5];

    public frmPrincipal()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < forms.Length; i++)
        {
            forms[i] = new frmMosca();
            //forms[i].ComSom = true;
            forms[i].Show();
        }
    }

    public async void button2_Click(object sender, EventArgs e)
    {
        frmComida comida = new frmComida();
        await comida.Mostrar();
    }

    private async void button3_Click(object sender, EventArgs e)
    {
        await Task.Delay(3000);
        await forms[0].RoubarMouse();
    }
}
