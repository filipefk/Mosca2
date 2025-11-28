namespace Mosca2
{
    partial class frmPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // Dispose do NotifyIcon
            if (disposing && notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            chkTimerMoverPernas = new CheckBox();
            chkTimerRotacao = new CheckBox();
            chkTimerVoar = new CheckBox();
            chkSeguirMouse = new CheckBox();
            chkComSom = new CheckBox();
            SuspendLayout();
            // 
            // chkTimerMoverPernas
            // 
            chkTimerMoverPernas.AutoSize = true;
            chkTimerMoverPernas.Checked = true;
            chkTimerMoverPernas.CheckState = CheckState.Checked;
            chkTimerMoverPernas.Location = new Point(12, 12);
            chkTimerMoverPernas.Name = "chkTimerMoverPernas";
            chkTimerMoverPernas.Size = new Size(98, 19);
            chkTimerMoverPernas.TabIndex = 0;
            chkTimerMoverPernas.Text = "Mover pernas";
            chkTimerMoverPernas.UseVisualStyleBackColor = true;
            chkTimerMoverPernas.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // chkTimerRotacao
            // 
            chkTimerRotacao.AutoSize = true;
            chkTimerRotacao.Checked = true;
            chkTimerRotacao.CheckState = CheckState.Checked;
            chkTimerRotacao.Location = new Point(12, 37);
            chkTimerRotacao.Name = "chkTimerRotacao";
            chkTimerRotacao.Size = new Size(69, 19);
            chkTimerRotacao.TabIndex = 1;
            chkTimerRotacao.Text = "Rotação";
            chkTimerRotacao.UseVisualStyleBackColor = true;
            chkTimerRotacao.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // chkTimerVoar
            // 
            chkTimerVoar.AutoSize = true;
            chkTimerVoar.Checked = true;
            chkTimerVoar.CheckState = CheckState.Checked;
            chkTimerVoar.Location = new Point(12, 62);
            chkTimerVoar.Name = "chkTimerVoar";
            chkTimerVoar.Size = new Size(49, 19);
            chkTimerVoar.TabIndex = 2;
            chkTimerVoar.Text = "Voar";
            chkTimerVoar.UseVisualStyleBackColor = true;
            chkTimerVoar.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // chkSeguirMouse
            // 
            chkSeguirMouse.AutoSize = true;
            chkSeguirMouse.Location = new Point(12, 87);
            chkSeguirMouse.Name = "chkSeguirMouse";
            chkSeguirMouse.Size = new Size(108, 19);
            chkSeguirMouse.TabIndex = 3;
            chkSeguirMouse.Text = "Seguir o mouse";
            chkSeguirMouse.UseVisualStyleBackColor = true;
            chkSeguirMouse.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // chkComSom
            // 
            chkComSom.AutoSize = true;
            chkComSom.Location = new Point(12, 112);
            chkComSom.Name = "chkComSom";
            chkComSom.Size = new Size(78, 19);
            chkComSom.TabIndex = 4;
            chkComSom.Text = "Com som";
            chkComSom.UseVisualStyleBackColor = true;
            chkComSom.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // frmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(413, 151);
            Controls.Add(chkComSom);
            Controls.Add(chkSeguirMouse);
            Controls.Add(chkTimerVoar);
            Controls.Add(chkTimerRotacao);
            Controls.Add(chkTimerMoverPernas);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmPrincipal";
            ShowInTaskbar = false;
            Text = "Principal";
            Load += frmPrincipal_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkTimerMoverPernas;
        private CheckBox chkTimerRotacao;
        private CheckBox chkTimerVoar;
        private CheckBox chkSeguirMouse;
        private CheckBox chkComSom;
    }
}
