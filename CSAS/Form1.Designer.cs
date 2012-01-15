namespace CSAS
{
    partial class Form1
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
            this.btnGera = new System.Windows.Forms.Button();
            this.txtAssembly = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDestino = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // btnGera
            // 
            this.btnGera.Location = new System.Drawing.Point(310, 395);
            this.btnGera.Name = "btnGera";
            this.btnGera.Size = new System.Drawing.Size(176, 37);
            this.btnGera.TabIndex = 0;
            this.btnGera.Text = "Gerar Classes AS";
            this.btnGera.UseVisualStyleBackColor = true;
            this.btnGera.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtAssembly
            // 
            this.txtAssembly.Location = new System.Drawing.Point(12, 27);
            this.txtAssembly.Name = "txtAssembly";
            this.txtAssembly.Size = new System.Drawing.Size(476, 20);
            this.txtAssembly.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(9, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "ASSEMBLY origem";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "PASTA destino";
            // 
            // txtDestino
            // 
            this.txtDestino.Location = new System.Drawing.Point(12, 71);
            this.txtDestino.Name = "txtDestino";
            this.txtDestino.Size = new System.Drawing.Size(476, 20);
            this.txtDestino.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 335);
            this.panel1.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 442);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDestino);
            this.Controls.Add(this.btnGera);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtAssembly);
            this.Name = "Form1";
            this.Text = "CSAS - SDE Sistema de Gestão 2009®";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGera;
        private System.Windows.Forms.TextBox txtAssembly;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDestino;
        private System.Windows.Forms.FlowLayoutPanel panel1;
    }
}

