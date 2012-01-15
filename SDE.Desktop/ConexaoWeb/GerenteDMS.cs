using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SDE.Desktop.ConexaoWeb
{
    public class GerenteDMS
    {
        string pastaDMS = "C:\\notas_prefeitura_rioverde\\";

        public void escreveArquivoDMS(int idMov, string conteudo)
        {
            if (!Directory.Exists(pastaDMS))
                Directory.CreateDirectory(pastaDMS);
            string nomeArquivo =
                string.Format(
                    "{0}_mov_{1}_data_{2:yyMMdd}_hora_{2:HHmmss}.xml",
                    pastaDMS, idMov, DateTime.Now
                );
            File.WriteAllText(nomeArquivo, conteudo, Encoding.Default);
            System.Diagnostics.Process.Start("explorer.exe", pastaDMS);
        }
    }
}
