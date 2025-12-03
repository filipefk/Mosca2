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
            if (disposing && globalMouseHook != null)
            {
                globalMouseHook.Dispose();
                globalMouseHook = null;
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
            chkComSom = new CheckBox();
            btMaisUmaMosca = new Button();
            btMenosUmaMosca = new Button();
            lblTotalDeMoscas = new Label();
            hsbAngulo = new HScrollBar();
            lblAngulo = new Label();
            chkPermitirAgarrarSoltar = new CheckBox();
            chkApontarParaMouse = new CheckBox();
            lblPosicaoMouse = new Label();
            cboComportamentoMouse = new ComboBox();
            label1 = new Label();
            btEstrarEmForma = new Button();
            btFilaIndiana = new Button();
            btRodaGigante = new Button();
            btDancaLoca = new Button();
            btDarComida = new Button();
            chkMostrarIndice = new CheckBox();
            btFormacaoQuadrada = new Button();
            nudVelocidade = new NumericUpDown();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)nudVelocidade).BeginInit();
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
            chkTimerRotacao.Size = new Size(118, 19);
            chkTimerRotacao.TabIndex = 1;
            chkTimerRotacao.Text = "Rotação aleatório";
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
            chkTimerVoar.Size = new Size(98, 19);
            chkTimerVoar.TabIndex = 2;
            chkTimerVoar.Text = "Voar aleatório";
            chkTimerVoar.UseVisualStyleBackColor = true;
            chkTimerVoar.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // chkComSom
            // 
            chkComSom.AutoSize = true;
            chkComSom.Location = new Point(12, 87);
            chkComSom.Name = "chkComSom";
            chkComSom.Size = new Size(78, 19);
            chkComSom.TabIndex = 4;
            chkComSom.Text = "Com som";
            chkComSom.UseVisualStyleBackColor = true;
            chkComSom.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // btMaisUmaMosca
            // 
            btMaisUmaMosca.Location = new Point(218, 7);
            btMaisUmaMosca.Name = "btMaisUmaMosca";
            btMaisUmaMosca.Size = new Size(25, 26);
            btMaisUmaMosca.TabIndex = 5;
            btMaisUmaMosca.Text = "+";
            btMaisUmaMosca.UseVisualStyleBackColor = true;
            btMaisUmaMosca.Click += btMaisUmaMosca_Click;
            // 
            // btMenosUmaMosca
            // 
            btMenosUmaMosca.Location = new Point(249, 7);
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
            lblTotalDeMoscas.Location = new Point(291, 13);
            lblTotalDeMoscas.Name = "lblTotalDeMoscas";
            lblTotalDeMoscas.Size = new Size(104, 15);
            lblTotalDeMoscas.TabIndex = 7;
            lblTotalDeMoscas.Text = "Total de moscas: 0";
            // 
            // hsbAngulo
            // 
            hsbAngulo.Location = new Point(206, 62);
            hsbAngulo.Maximum = 369;
            hsbAngulo.Name = "hsbAngulo";
            hsbAngulo.Size = new Size(245, 33);
            hsbAngulo.TabIndex = 8;
            hsbAngulo.Scroll += hsbAngulo_Scroll;
            // 
            // lblAngulo
            // 
            lblAngulo.AutoSize = true;
            lblAngulo.Location = new Point(307, 41);
            lblAngulo.Name = "lblAngulo";
            lblAngulo.Size = new Size(21, 15);
            lblAngulo.TabIndex = 9;
            lblAngulo.Text = "0 º";
            // 
            // chkPermitirAgarrarSoltar
            // 
            chkPermitirAgarrarSoltar.AutoSize = true;
            chkPermitirAgarrarSoltar.Location = new Point(12, 112);
            chkPermitirAgarrarSoltar.Name = "chkPermitirAgarrarSoltar";
            chkPermitirAgarrarSoltar.Size = new Size(149, 19);
            chkPermitirAgarrarSoltar.TabIndex = 10;
            chkPermitirAgarrarSoltar.Text = "Permitir agarrar e soltar";
            chkPermitirAgarrarSoltar.UseVisualStyleBackColor = true;
            chkPermitirAgarrarSoltar.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // chkApontarParaMouse
            // 
            chkApontarParaMouse.AutoSize = true;
            chkApontarParaMouse.Location = new Point(12, 137);
            chkApontarParaMouse.Name = "chkApontarParaMouse";
            chkApontarParaMouse.Size = new Size(144, 19);
            chkApontarParaMouse.TabIndex = 11;
            chkApontarParaMouse.Text = "Apontar para o mouse";
            chkApontarParaMouse.UseVisualStyleBackColor = true;
            chkApontarParaMouse.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // lblPosicaoMouse
            // 
            lblPosicaoMouse.AutoSize = true;
            lblPosicaoMouse.Location = new Point(206, 144);
            lblPosicaoMouse.Name = "lblPosicaoMouse";
            lblPosicaoMouse.Size = new Size(110, 15);
            lblPosicaoMouse.TabIndex = 12;
            lblPosicaoMouse.Text = "Posição do mouse: ";
            // 
            // cboComportamentoMouse
            // 
            cboComportamentoMouse.DropDownStyle = ComboBoxStyle.DropDownList;
            cboComportamentoMouse.FormattingEnabled = true;
            cboComportamentoMouse.Location = new Point(12, 217);
            cboComportamentoMouse.Name = "cboComportamentoMouse";
            cboComportamentoMouse.Size = new Size(162, 23);
            cboComportamentoMouse.TabIndex = 13;
            cboComportamentoMouse.SelectedIndexChanged += chkTimers_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 199);
            label1.Name = "label1";
            label1.Size = new Size(99, 15);
            label1.TabIndex = 14;
            label1.Text = "Comportamento:";
            // 
            // btEstrarEmForma
            // 
            btEstrarEmForma.Location = new Point(341, 169);
            btEstrarEmForma.Name = "btEstrarEmForma";
            btEstrarEmForma.Size = new Size(129, 26);
            btEstrarEmForma.TabIndex = 15;
            btEstrarEmForma.Text = "Em forma!!!";
            btEstrarEmForma.UseVisualStyleBackColor = true;
            btEstrarEmForma.Click += btEstrarEmForma_Click;
            // 
            // btFilaIndiana
            // 
            btFilaIndiana.Location = new Point(206, 201);
            btFilaIndiana.Name = "btFilaIndiana";
            btFilaIndiana.Size = new Size(129, 26);
            btFilaIndiana.TabIndex = 16;
            btFilaIndiana.Text = "Fila indiana";
            btFilaIndiana.UseVisualStyleBackColor = true;
            btFilaIndiana.Click += btFilaIndiana_Click;
            // 
            // btRodaGigante
            // 
            btRodaGigante.Location = new Point(206, 233);
            btRodaGigante.Name = "btRodaGigante";
            btRodaGigante.Size = new Size(129, 26);
            btRodaGigante.TabIndex = 17;
            btRodaGigante.Text = "Roda gigante";
            btRodaGigante.UseVisualStyleBackColor = true;
            btRodaGigante.Click += btRodaGigante_Click;
            // 
            // btDancaLoca
            // 
            btDancaLoca.Location = new Point(341, 201);
            btDancaLoca.Name = "btDancaLoca";
            btDancaLoca.Size = new Size(129, 26);
            btDancaLoca.TabIndex = 18;
            btDancaLoca.Text = "Dança loca";
            btDancaLoca.UseVisualStyleBackColor = true;
            btDancaLoca.Click += btDancaLoca_Click;
            // 
            // btDarComida
            // 
            btDarComida.Location = new Point(206, 169);
            btDarComida.Name = "btDarComida";
            btDarComida.Size = new Size(129, 26);
            btDarComida.TabIndex = 19;
            btDarComida.Text = "Dar comida";
            btDarComida.UseVisualStyleBackColor = true;
            btDarComida.Click += btDarComida_Click;
            // 
            // chkMostrarIndice
            // 
            chkMostrarIndice.AutoSize = true;
            chkMostrarIndice.Location = new Point(12, 162);
            chkMostrarIndice.Name = "chkMostrarIndice";
            chkMostrarIndice.Size = new Size(102, 19);
            chkMostrarIndice.TabIndex = 20;
            chkMostrarIndice.Text = "Mostrar indice";
            chkMostrarIndice.UseVisualStyleBackColor = true;
            chkMostrarIndice.CheckedChanged += chkTimers_CheckedChanged;
            // 
            // btFormacaoQuadrada
            // 
            btFormacaoQuadrada.Location = new Point(341, 233);
            btFormacaoQuadrada.Name = "btFormacaoQuadrada";
            btFormacaoQuadrada.Size = new Size(129, 26);
            btFormacaoQuadrada.TabIndex = 21;
            btFormacaoQuadrada.Text = "Formação quadrada";
            btFormacaoQuadrada.UseVisualStyleBackColor = true;
            btFormacaoQuadrada.Click += btFormacaoQuadrada_Click;
            // 
            // nudVelocidade
            // 
            nudVelocidade.Increment = new decimal(new int[] { 30, 0, 0, 0 });
            nudVelocidade.Location = new Point(358, 111);
            nudVelocidade.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudVelocidade.Name = "nudVelocidade";
            nudVelocidade.Size = new Size(46, 23);
            nudVelocidade.TabIndex = 22;
            nudVelocidade.ValueChanged += nudVelocidade_ValueChanged;
            nudVelocidade.KeyPress += nudVelocidade_KeyPress;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(206, 113);
            label2.Name = "label2";
            label2.Size = new Size(146, 15);
            label2.TabIndex = 23;
            label2.Text = "Velocidade padrão de voo:";
            // 
            // frmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(487, 275);
            Controls.Add(label2);
            Controls.Add(nudVelocidade);
            Controls.Add(btFormacaoQuadrada);
            Controls.Add(chkMostrarIndice);
            Controls.Add(btDarComida);
            Controls.Add(btDancaLoca);
            Controls.Add(btRodaGigante);
            Controls.Add(btFilaIndiana);
            Controls.Add(btEstrarEmForma);
            Controls.Add(label1);
            Controls.Add(cboComportamentoMouse);
            Controls.Add(lblPosicaoMouse);
            Controls.Add(chkApontarParaMouse);
            Controls.Add(chkPermitirAgarrarSoltar);
            Controls.Add(lblAngulo);
            Controls.Add(hsbAngulo);
            Controls.Add(lblTotalDeMoscas);
            Controls.Add(btMenosUmaMosca);
            Controls.Add(btMaisUmaMosca);
            Controls.Add(chkComSom);
            Controls.Add(chkTimerVoar);
            Controls.Add(chkTimerRotacao);
            Controls.Add(chkTimerMoverPernas);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmPrincipal";
            ShowInTaskbar = false;
            Text = "Controle remoto";
            Load += frmPrincipal_Load;
            ((System.ComponentModel.ISupportInitialize)nudVelocidade).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkTimerMoverPernas;
        private CheckBox chkTimerRotacao;
        private CheckBox chkTimerVoar;
        private CheckBox chkComSom;
        private Button btMaisUmaMosca;
        private Button btMenosUmaMosca;
        private Label lblTotalDeMoscas;
        private HScrollBar hsbAngulo;
        private Label lblAngulo;
        private CheckBox chkPermitirAgarrarSoltar;
        private CheckBox chkApontarParaMouse;
        private Label lblPosicaoMouse;
        private ComboBox cboComportamentoMouse;
        private Label label1;
        private Button btEstrarEmForma;
        private Button btFilaIndiana;
        private Button btRodaGigante;
        private Button btDancaLoca;
        private Button btDarComida;
        private CheckBox chkMostrarIndice;
        private Button btFormacaoQuadrada;
        private NumericUpDown nudVelocidade;
        private Label label2;
    }
}
