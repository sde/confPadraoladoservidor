namespace Exportador_SDE
{
    partial class FormPrincipal
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
            this.btnExportaLoja = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExportaLoja
            // 
            this.btnExportaLoja.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportaLoja.Location = new System.Drawing.Point(12, 12);
            this.btnExportaLoja.Name = "btnExportaLoja";
            this.btnExportaLoja.Size = new System.Drawing.Size(268, 39);
            this.btnExportaLoja.TabIndex = 0;
            this.btnExportaLoja.Text = "Exporta LOJA";
            this.btnExportaLoja.UseVisualStyleBackColor = true;
            this.btnExportaLoja.Click += new System.EventHandler(this.btnExportaLoja_Click);
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.btnExportaLoja);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormPrincipal";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exportador SDE";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExportaLoja;
    }
}