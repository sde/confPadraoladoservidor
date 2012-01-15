using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SDE.Desktop.ConexaoWeb
{
    class GerenteBaixaDuplicata
    {
        public void baixaDuplicata(int idCorp, int idEmp, string numeroTitulo, string tipoDocumento)
        {
            string diretorio = @"C:\Documentos SDE\";
            diretorio += tipoDocumento + @"\";
            string fileName = tipoDocumento + "-" + numeroTitulo;

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            WebClient request = new WebClient();

            request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
            byte[] fileData = request.DownloadData(Program.SDE_PRODUCAO + idCorp + @"/duplicata.pdf");
            FileStream file = File.Create(diretorio + @"\" + fileName + ".pdf");
            file.Write(fileData, 0, fileData.Length);
            file.Close();
            System.Diagnostics.Process.Start(diretorio + @"\" + fileName + ".pdf");
        }
    }
}
