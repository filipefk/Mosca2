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
            btMaisUmaMosca = new Button();
            btMenosUmaMosca = new Button();
            lblTotalDeMoscas = new Label();
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
            // btMaisUmaMosca
            // 
            btMaisUmaMosca.Location = new Point(144, 7);
            btMaisUmaMosca.Name = "btMaisUmaMosca";
            btMaisUmaMosca.Size = new Size(25, 26);
            btMaisUmaMosca.TabIndex = 5;
            btMaisUmaMosca.Text = "+";
            btMaisUmaMosca.UseVisualStyleBackColor = true;
            btMaisUmaMosca.Click += btMaisUmaMosca_Click;
            // 
            // btMenosUmaMosca
            // 
            btMenosUmaMosca.Location = new Point(175, 7);
            btMenosUmaMosca.Name = "btMenosUmaMosca";
            btMenosUmaMosca.Size = new Size(25, 26);
            btMenosUmaMosca.TabIndex = 6;
            btMenosUmaMosca.Text = "-";
            btMenosUmaMosca.UseVisualStyleBackColor = true;
            btMenosUmaMosca.Click += btMenosUmaMosca_Click;
            // 
            // lblTotalDeMoscas
            // 
            lblTotalDeMoscas.AutoSize = true;
            lblTotalDeMoscas.Location = new Point(217, 13);
            lblTotalDeMoscas.Name = "lblTotalDeMoscas";
            lblTotalDeMoscas.Size = new Size(104, 15);
            lblTotalDeMoscas.TabIndex = 7;
            lblTotalDeMoscas.Text = "Total de moscas: 0";
            // 
            // frmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(386, 153);
            Controls.Add(lblTotalDeMoscas);
            Controls.Add(btMenosUmaMosca);
            Controls.Add(btMaisUmaMosca);
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
        private Button btMaisUmaMosca;
        private Button btMenosUmaMosca;
        private Label lblTotalDeMoscas;
    }
}
