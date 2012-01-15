using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SDE.Desktop.ConexaoWeb
{
    public class GerenteTEF
    {
        public delegate void EventoRetornoTEF(int coo);
        public event EventoRetornoTEF eventoRetornoTEF;
        /*
        public GerenteTEF(string conteudo, string complemento)
        {

        }
        */
        FileSystemWatcher fWatcher;

        string pastaTEF = "C:\\MP20FIII\\";
        string arqNum = "C:\\MP20FIII\\NUMERO.TXT";

        int numero = 0;
        string arqTXT_conteudo
        {
            get
            {
                return string.Format("C:\\MP20FIII\\{0}.TXT", numero);
            }
        }
        string arqPRN_retorno
        {
            get
            {
                return string.Format("C:\\MP20FIII\\{0}.PRN", numero);
            }
        }
        string arqPRZ_complemento
        {
            get
            {
                return string.Format("C:\\MP20FIII\\{0}.PRZ", numero);
            }
        }

        public void escreveArquivoTEF(string conteudo, string complemento)
        {
            //cria C:\\MP20FIII\\NUMERO.TXT
            if (!Directory.Exists(pastaTEF))
                Directory.CreateDirectory(pastaTEF);
            if (!File.Exists(arqNum))
                File.WriteAllText(arqNum,"1");
            //
            //gera arquivos
            numero = int.Parse(File.ReadAllText(arqNum));
            numero++;
            File.WriteAllText(arqTXT_conteudo, conteudo, Encoding.Default);
            File.WriteAllText(arqPRZ_complemento, complemento, Encoding.Default);
            File.WriteAllText(arqNum, numero.ToString());
            //
            //espera arquivos
            fWatcher = new FileSystemWatcher(pastaTEF);
            fWatcher.Created += new FileSystemEventHandler(fWatcher_Created);
            fWatcher.EnableRaisingEvents = true;
        }

        void fWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath==arqPRN_retorno)
            {
                int coo = int.Parse(File.ReadAllText(arqPRN_retorno));
                this.eventoRetornoTEF.Invoke(coo);
                fWatcher.Dispose();
            }
        }


    }
}
