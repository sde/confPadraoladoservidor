using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SDE.Desktop
{
    static class Program
    {
        public const string SENHA_FTP = "paoc1710";
        //public const string SENHA_FTP = "123";
        public const string SDE_PRODUCAO = "ftp://sistemadaempresa.com.br/ftp/documentos/";
        //public const string SDE_DEV = "C:\\Documentos\\"; 
        public const string SDE_EMERGENCIA = "ftp://sistemadaempresa.com/ftp/documentos/";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmPrincipal = new FrmPrincipal();
            Application.Run(frmPrincipal);
        }

        public static bool verificaExecucao()
        {
            bool retorno = false;
            DateTime data = new DateTime();
            data = DateTime.Now;
            StreamWriter sw;

            if (!Directory.Exists("C:\\Documentos SDE\\Arquivos\\"))
                Directory.CreateDirectory("C:\\Documentos SDE\\Arquivos\\");

            string path = "C:\\Documentos SDE\\Arquivos\\vrfcexcco.txt";
            if (!File.Exists(path))
            {
                sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.WriteLine(data.ToString("dd/MM/yyyy"));
                sw.Close();
                retorno = true;
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                using (StreamReader sr = fi.OpenText())
                {
                    retorno = (sr.ReadLine() == data.ToString("dd/MM/yyyy"));
                }
                sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.WriteLine(data.ToString("dd/MM/yyyy"));
                sw.Close();
            }
            return retorno;
        }

        public static FrmPrincipal frmPrincipal;
    }
}
