using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace CSAS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.Controls.Add(c1);
            panel1.Controls.Add(c2);
            panel1.Controls.Add(c3);
            panel1.Controls.Add(c4);
            panel1.Controls.Add(c5);
        }

        GeradorAS gerador = new GeradorAS();

        ReescritorCamada c1 = new ReescritorCamada();
        ReescritorCamada c2 = new ReescritorCamada();
        ReescritorCamada c3 = new ReescritorCamada();
        ReescritorCamada c4 = new ReescritorCamada();
        ReescritorCamada c5 = new ReescritorCamada();

        private void Form1_Load(object sender, EventArgs e)
        {
            //txtAssembly.Text = @"C:\sistemadaempresa\fontes\servidor1\SDE\bin\SDE.dll";
            txtDestino.Text = @"C:\dev\sistemadaempresa\ambiente_desenvolvimento\principal\lado_cliente\src\";
            c1.ns = "SDE.Entidade";
            c2.ns = "SDE.Parametro";
            c3.ns = "SDE.Enumerador";
            c4.ns = "SDE.CamadaServico";
            c5.ns = "SDE.Outros";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            string aFile = txtAssembly.Text;
            if (!File.Exists(aFile))
            {
                MessageBox.Show("ASSEMBLY não existe");
                return;
            }
            Assembly a = Assembly.LoadFile(aFile);
            */
            Assembly a = Assembly.Load("SDE");
            
            

            Type[] tipos = a.GetTypes();//referenciar as dlls neste projeto

            //gerador.addEntidade(filtraTipos(tipos, c1.ns));
            gerador.addParametro(filtraTipos(tipos, c2.ns));
            gerador.addOutro(filtraTipos(tipos, c5.ns));
            //gerador.addEnum(filtraTipos(tipos, c3.ns));
            gerador.addServico(filtraTipos(tipos, c4.ns));

            gerador.gerar(txtDestino.Text);

            /*
            renderizaEnum(tipos,
                c2.diretorioRaiz, c2.namespaceCS, c2.packageAS
                );
            renderizaServicos(tipos,
                c4.diretorioRaiz, c4.namespaceCS,
                txtNSPersistencia.Text, txtNSEntidades.Text,
                txtPKGPersistencia.Text, txtPKGEntidades.Text
                );
             * */
            //MessageBox.Show("ok");
            this.Close();
        }

        private List<Type> filtraTipos(Type[] tipos, string ns)
        {
            List<Type> ls = new List<Type>();
            foreach (Type t in tipos)
                if (t.Namespace == ns)
                    ls.Add(t);
            return ls;
        }
    }
}
