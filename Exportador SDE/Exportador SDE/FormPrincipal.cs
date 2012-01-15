using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Exportador_SDE
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void btnExportaLoja_Click(object sender, EventArgs e)
        {
            new FormExportaLoja().ShowDialog();
            this.Close();
        }
    }
}
