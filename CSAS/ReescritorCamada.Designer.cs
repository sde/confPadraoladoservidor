namespace CSAS
{
    partial class ReescritorCamada
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCS = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(0, 5);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(52, 13);
            this.lblTitulo.TabIndex = 12;
            this.lblTitulo.Text = "TITULO";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Namespace CS:";
            // 
            // txtCS
            // 
            this.txtCS.Location = new System.Drawing.Point(3, 34);
            this.txtCS.Name = "txtCS";
            this.txtCS.Size = new System.Drawing.Size(254, 20);
            this.txtCS.TabIndex = 7;
            this.txtCS.Text = "Multisoft.Sistema";
            // 
            // ReescritorCamada
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCS);
            this.Name = "ReescritorCamada";
            this.Size = new System.Drawing.Size(266, 63);
            this.Load += new System.EventHandler(this.ReescritorCamada_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCS;

    }
}
