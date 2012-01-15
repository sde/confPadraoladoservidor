using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;


namespace SDE.Desktop.ConexaoWeb
{
    class GerenteLoginRVMangueiras
    {
        public void LoginRVMangueiras(int idCorp)
        {
            if (idCorp == 96)
            {
                if (File.Exists(@"C:\Loja\Progs\USER.SEG"))
                {
                    File.Delete(@"C:\Loja\Progs\USER.SEG");
                    File.Create(@"C:\Loja\Progs\USER.SEG");
                }

                if (File.Exists(@"C:\Loja\Progs\VERIF.SEG"))
                {
                    File.Delete(@"C:\Loja\Progs\VERIF.SEG");
                    File.Create(@"C:\Loja\Progs\VERIF.SEG");
                }

                if (File.Exists(@"C:\Loja\Progs\EST001.exe"))
                {
                    File.Delete(@"C:\Loja\Progs\EST001.exe");
                    File.Create(@"C:\Loja\Progs\EST001.exe");
                }
            }
        }
    }
}
