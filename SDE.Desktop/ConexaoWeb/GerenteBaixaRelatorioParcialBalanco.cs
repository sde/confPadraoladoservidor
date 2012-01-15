using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SDE.Desktop.ConexaoWeb
{
    class GerenteBaixaRelatorioParcialBalanco
    {
        public void baixaRelatorioParcialBalanco(int idCorp)
        {
            string diretorio = @"C:\Documentos SDE\";
            diretorio += "Relatório" + @"\";
            string fileName = "Relatório Parcial Balanço";

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            WebClient request = new WebClient();

            request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
            byte[] fileData = request.DownloadData(Program.SDE_PRODUCAO + idCorp + @"/relParcialBalanco.pdf");
            FileStream file = File.Create(diretorio + @"\" + fileName + ".pdf");
            file.Write(fileData, 0, fileData.Length);
            file.Close();
            System.Diagnostics.Process.Start(diretorio + @"\" + fileName + ".pdf");
        }
    }
}
