using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SDE.Desktop.ConexaoWeb
{
    class GerenteBaixaInventario
    {
        public void baixaInventario(int idCorp, int idEmp, string tipoPreco, double pctSobreValor, string dataInventario, string textoCabecalho, bool mostraZerados, string tipoDocumento)
        {
            string diretorio = @"C:\Documentos SDE\Relatório de Inventário";
            diretorio += tipoDocumento + @"\";
            string fileName = tipoDocumento + "-" + dataInventario.Replace('/','-').Replace('/','-').Replace('/','-');

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            WebClient request = new WebClient();

            request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
            byte[] fileData = request.DownloadData(Program.SDE_PRODUCAO + idCorp + @"/inventario.pdf");
            FileStream file = File.Create(diretorio + @"\" + fileName + ".pdf");
            file.Write(fileData, 0, fileData.Length);
            file.Close();
            System.Diagnostics.Process.Start(diretorio + @"\" + fileName + ".pdf");
        }
    }
}
