using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace SDE.Desktop.ConexaoWeb
{
    class GerenteBaixaRelatorioOrdemServico
    {
        public void baixaRelatorioOrdemServico(int idCorp, int idOrdemServico)
        {
            String diretorio = @"C:\Documentos SDE\Relatório\";
            String nomeArquivo = @"\Ordem de Servico - " + idOrdemServico + ".pdf";

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
            byte[] fileData = webClient.DownloadData(Program.SDE_PRODUCAO + idCorp + @"/OrdemServico.pdf");

            FileStream fileStream = File.Create(diretorio + nomeArquivo);
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
            Process.Start(diretorio + nomeArquivo);
        }
    }
}
