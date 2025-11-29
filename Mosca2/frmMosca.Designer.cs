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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picMosca = new PictureBox();
            lblIndice = new Label();
            ((System.ComponentModel.ISupportInitialize)picMosca).BeginInit();
            SuspendLayout();
            // 
            // picMosca
            // 
            picMosca.Image = Properties.Resources.Mosca1;
            picMosca.Location = new Point(0, 0);
            picMosca.Name = "picMosca";
            picMosca.Size = new Size(45, 45);
            picMosca.SizeMode = PictureBoxSizeMode.StretchImage;
            picMosca.TabIndex = 0;
            picMosca.TabStop = false;
            // 
            // lblIndice
            // 
            lblIndice.AutoSize = true;
            lblIndice.BackColor = Color.Transparent;
            lblIndice.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblIndice.ForeColor = Color.OrangeRed;
            lblIndice.Location = new Point(37, 0);
            lblIndice.Name = "lblIndice";
            lblIndice.Size = new Size(14, 15);
            lblIndice.TabIndex = 1;
            lblIndice.Text = "0";
            // 
            // frmMosca
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(54, 47);
            Controls.Add(lblIndice);
            Controls.Add(picMosca);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmMosca";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmMosca";
            ((System.ComponentModel.ISupportInitialize)picMosca).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picMosca;
        private Label lblIndice;
    }
}