using System.Runtime.InteropServices;

namespace Mosca2;

public class JanelaCamada : Form
{
    private const int WS_EX_TOPMOST = 0x00000008;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int ULW_ALPHA = 0x00000002;
    private const byte AC_SRC_OVER = 0x00;
    private const byte AC_SRC_ALPHA = 0x01;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TOPMOST;
            return cp;
        }
    }

    protected override bool ShowWithoutActivation => true;

    protected override void WndProc(ref Message m)
    {
        // A posição e o tamanho são controlados pelo UpdateLayeredWindow; ignora o redimensionamento sugerido ao trocar de monitor
        const int WM_DPICHANGED = 0x02E0;
        if (m.Msg == WM_DPICHANGED)
        {
            m.Result = IntPtr.Zero;
            return;
        }
        base.WndProc(ref m);
    }

    protected static int DpiNoPonto(Point ponto)
    {
        const uint MONITOR_DEFAULTTONEAREST = 2;
        const int MDT_EFFECTIVE_DPI = 0;
        var monitor = MonitorFromPoint(new POINT { x = ponto.X, y = ponto.Y }, MONITOR_DEFAULTTONEAREST);
        if (monitor != IntPtr.Zero && GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out uint dpiX, out _) == 0 && dpiX > 0)
            return (int)dpiX;
        return 96;
    }

    protected void AtualizarCamada(Bitmap bmp, Point posicaoTela, byte opacidade = 255)
    {
        if (!IsHandleCreated || IsDisposed)
            return;

        IntPtr telaDc = GetDC(IntPtr.Zero);
        IntPtr memDc = CreateCompatibleDC(telaDc);
        IntPtr hBitmap = IntPtr.Zero;
        IntPtr bitmapAnterior = IntPtr.Zero;
        try
        {
            hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
            bitmapAnterior = SelectObject(memDc, hBitmap);

            var tamanho = new SIZE { cx = bmp.Width, cy = bmp.Height };
            var origem = new POINT { x = 0, y = 0 };
            var destino = new POINT { x = posicaoTela.X, y = posicaoTela.Y };
            var blend = new BLENDFUNCTION
            {
                BlendOp = AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = opacidade,
                AlphaFormat = AC_SRC_ALPHA
            };

            UpdateLayeredWindow(Handle, telaDc, ref destino, ref tamanho, memDc, ref origem, 0, ref blend, ULW_ALPHA);
        }
        finally
        {
            if (hBitmap != IntPtr.Zero)
            {
                SelectObject(memDc, bitmapAnterior);
                DeleteObject(hBitmap);
            }
            DeleteDC(memDc);
            ReleaseDC(IntPtr.Zero, telaDc);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize,
        IntPtr hdcSrc, ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

    [DllImport("shcore.dll")]
    private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);
}
