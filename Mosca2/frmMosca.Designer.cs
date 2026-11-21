namespace Mosca2
{
    partial class frmMosca
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                _renderizador?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            //
            // frmMosca
            //
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(72, 72);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmMosca";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmMosca";
            ResumeLayout(false);
        }

        #endregion
    }
}
