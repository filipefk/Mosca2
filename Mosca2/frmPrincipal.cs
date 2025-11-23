using System.Threading.Tasks;

namespace Mosca2;

public partial class frmPrincipal : Form
{
    public frmPrincipal()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        frmMosca[] forms = new frmMosca[5];
        for (int i = 0; i < forms.Length; i++)
        {
            forms[i] = new frmMosca();
            forms[i].Show();
        }
    }

    public async void button2_Click(object sender, EventArgs e)
    {
        frmComida comida = new frmComida();
        await comida.Mostrar();
    }
}
