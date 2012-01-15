using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SDE.Desktop.ConexaoWeb
{
    class GerenteEtiquetas
    {
        private const string ARQUIVO_BAT = @"C:\SDE\enviaImpressao.bat";
        private const string ARQUIVO_TXT = @"C:\SDE\confImpressao.txt";
        private string STX = char.ConvertFromUtf32(002);

        public void enviaImpressao(int idCorp)
        {
            if (idCorp == 44 || idCorp == 53)
            {
                string diretorio = @"C:\SDE\";

                if (!Directory.Exists(diretorio))
                    Directory.CreateDirectory(diretorio);

                WebClient request = new WebClient();

                request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
                byte[] fileDataBat = request.DownloadData(Program.SDE_PRODUCAO + "argox/" + idCorp + "/enviaImpressao.bat");
                FileStream fileBat = File.Create(ARQUIVO_BAT);
                fileBat.Write(fileDataBat, 0, fileDataBat.Length);
                fileBat.Close();

                request = new WebClient();

                request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
                byte[] fileDataTxt = request.DownloadData(Program.SDE_PRODUCAO + "argox/" + idCorp + "/confImpressao.txt");
                FileStream fileTxt = File.Create(ARQUIVO_TXT);
                fileTxt.Write(fileDataTxt, 0, fileDataTxt.Length);
                fileTxt.Close();

                System.Diagnostics.Process.Start(ARQUIVO_BAT);
            }
            else if (idCorp == 56 || idCorp == 68 || idCorp == 20 || idCorp == 64 || idCorp == 76 || idCorp == 83 || 
                idCorp == 70 || idCorp == 90 || idCorp == 108 || idCorp == 128)
            {
                string diretorio = @"C:\Documentos SDE\Etiqueta\";
                string fileName = "Etiqueta";

                if (!Directory.Exists(diretorio))
                    Directory.CreateDirectory(diretorio);

                WebClient request = new WebClient();
                

                request.Credentials = new NetworkCredential("Administrator", Program.SENHA_FTP);
                //request.Credentials = new NetworkCredential("marcos", Program.SENHA_FTP); //teste localhost
                byte[] fileData = request.DownloadData(Program.SDE_PRODUCAO + idCorp + "/etiqueta.pdf");
                //byte[] fileData = request.DownloadData(Program.SDE_DEV + idCorp + "/etiqueta.pdf"); //teste localhost
                FileStream file = File.Create(diretorio + @"\" + fileName + ".pdf");
                file.Write(fileData, 0, fileData.Length);
                file.Close();
                System.Diagnostics.Process.Start(diretorio + @"\" + fileName + ".pdf");
            }
        }
    }
}
