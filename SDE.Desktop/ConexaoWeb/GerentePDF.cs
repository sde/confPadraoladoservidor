using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SDE.Desktop.ConexaoWeb
{
    class GerentePDF
    {
        public void escreveArquivoPDF(int idCorp, int idEmp, int idMov, string tipo)
        {
            string diretorio = @"C:\Documentos SDE\";
            diretorio += tipo + @"\";
            string fileName = tipo + "-" + idMov;
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            WebClient request = new WebClient();

            request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
            byte[] fileData = request.DownloadData(Program.SDE_PRODUCAO + idCorp + @"/mov.pdf");
            FileStream file = File.Create(diretorio +@"\"+ fileName + ".pdf");
            file.Write(fileData, 0, fileData.Length);
            file.Close();
            System.Diagnostics.Process.Start(diretorio + @"\" + fileName + ".pdf");
            //File.Open(diretorio +@"\"+ fileName + ".pdf", FileMode.Open);
        }
    }
}
