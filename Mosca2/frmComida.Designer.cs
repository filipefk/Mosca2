namespace Mosca2
{
    partial class frmComida
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picComida = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picComida).BeginInit();
            SuspendLayout();
            // 
            // picComida
            // 
            picComida.Image = Properties.Resources.Merda;
            picComida.Location = new Point(0, -1);
            picComida.Name = "picComida";
            picComida.Size = new Size(72, 65);
            picComida.SizeMode = PictureBoxSizeMode.StretchImage;
            picComida.TabIndex = 0;
            picComida.TabStop = false;
            // 
            // frmComida
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(74, 66);
            Controls.Add(picComida);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmComida";
            Text = "frmComida";
            ((System.ComponentModel.ISupportInitialize)picComida).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox picComida;
    }
}