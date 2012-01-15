using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace SDE.Desktop.ConexaoWeb
{
    public class GerenteProcessos
    {
        
        public void iniciaProcesso(string executavel, string parametros)
        {
            Console.Beep();
            Process.Start(executavel, parametros);
            Console.Beep();
        }
    }
}
