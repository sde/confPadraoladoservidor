using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CSAS
{
    public partial class ReescritorCamada : UserControl
    {
        public ReescritorCamada()
        {
            InitializeComponent();
        }

        public string ns { set { lblTitulo.Text = value; txtCS.Text = value; } get { return txtCS.Text; } }

        private void ReescritorCamada_Load(object sender, EventArgs e)
        {

        }
    }
}
