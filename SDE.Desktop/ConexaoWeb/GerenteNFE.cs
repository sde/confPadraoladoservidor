using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SDE.Desktop.ConexaoWeb
{
    public class GerenteNFE
    {
        public void escreveArquivoNFExml(int idCorp, int idEmp, string conteudo, string chaveAcessoNFE)
        {
            if ((idCorp != 12 && idEmp != 6) || (idCorp == 12 && idEmp != 6))
            {
                string pastaNFE = "C:\\nfe_geradas\\xml\\individual\\";
                if (!Directory.Exists(pastaNFE))
                    Directory.CreateDirectory(pastaNFE);

                string nomeArquivo = chaveAcessoNFE + "-nfe.xml";

                File.WriteAllText(pastaNFE + nomeArquivo, conteudo, Encoding.Default);

                //grava os arquivos xml gerados na pasta backup
                string pastaNFEBkp = "C:\\nfe_geradas\\xml\\backup\\";
                if (!Directory.Exists(pastaNFEBkp))
                    Directory.CreateDirectory(pastaNFEBkp);

                File.WriteAllText(pastaNFEBkp + nomeArquivo, conteudo, Encoding.Default);

            }
            else
            {
                string pastaNFE = "C:\\nfe_geradas_"+ idCorp +"_"+ idEmp +"\\xml\\individual\\";
                if (!Directory.Exists(pastaNFE))
                    Directory.CreateDirectory(pastaNFE);

                string nomeArquivo = chaveAcessoNFE + "-nfe.xml";

                File.WriteAllText(pastaNFE + nomeArquivo, conteudo, Encoding.Default);

                //grava os arquivos xml gerados na pasta backup
                string pastaNFEBkp = "C:\\nfe_geradas_" + idCorp + "_" + idEmp + "\\xml\\backup\\";
                if (!Directory.Exists(pastaNFEBkp))
                    Directory.CreateDirectory(pastaNFEBkp);

                File.WriteAllText(pastaNFEBkp + nomeArquivo, conteudo, Encoding.Default);

            }
        }

        public void escreveArquivoNFE(string conteudo, string chaveAcessoNFE)
        {
            //cria C:\\MP20FIII\\NUMERO.TXT
            string pastaNFE = "C:\\nfe_geradas\\";
            if (!Directory.Exists(pastaNFE))
                Directory.CreateDirectory(pastaNFE);

            string nomeArquivo = chaveAcessoNFE + "-nfe.txt";

            //
            //gera arquivos
            File.WriteAllText(pastaNFE+nomeArquivo, conteudo, Encoding.Default);
            System.Diagnostics.Process.Start("explorer.exe", pastaNFE);
        }
    }
}
