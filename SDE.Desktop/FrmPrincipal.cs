using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flash.External;
using SDE.Desktop.ConexaoWeb;
using System.IO;
using System.Diagnostics;

namespace SDE.Desktop
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
            iniciaFlashExternal();
            BackColor = Color.LightBlue;
            escondeSDEDesktop();
            notifyIcon.ShowBalloonTip(0);
        }
        
        private void iniciaFlashExternal()
        {
            if (!Program.verificaExecucao())
            {
                Inicializar();
                foreach (Process processo in Process.GetProcesses())
                {
                    if (processo.ProcessName == "SDE.Desktop")
                    {
                        Close();
                        processo.Kill();
                    }
                }
            }
        

            int receiverweb = new Random().Next(1000, 2000);
            int receiverproxy = new Random().Next(3000, 4000);

            String urlServidor = @"http://localhost:2050/";
            //String urlServidor = @"http://sistemadaempresa.com/sde/";
            //String urlServidor = @"http://sistemadaempresa.com.br/sde/";
            String queryString = string.Format("?proxy=1&receiverweb={0}&receiverproxy={1}", receiverweb, receiverproxy);

            //String swfIndex = string.Concat("TesteLocalConnection.swf", queryString);
            String swfProxy = string.Concat("DesktopProxy.swf", queryString);
            String swfIndex = string.Concat("index.swf", queryString);
            String urlSwfQueryString = "\"" + urlServidor + swfIndex + "\"";
            
            String executavelFirefox = @"*c:\Arquivos de programas\Mozilla Firefox\firefox.exe";
            String executavelChrome1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"*\Google\Chrome\Application\chrome.exe";
            String executavelChrome2 = @"*C:\Arquivos de programas\Google\Chrome\Application\chrome.exe";
            String navegador = "explorer";

            /*Este bloco de código apresentou problemas em execução no Win7 devido
             * a direfença na estrutura de diretórios entre o WinXP e seus sucessores.
             * Com esta verificação de diretórios existentes o problema foi sanado.*/
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\Inicializar\\"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\SDE\\SDE.Desktop.appref-ms"))
                {
                    File.Copy
                        (Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\SDE\\SDE.Desktop.appref-ms",
                        Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\Inicializar\\SDE.Desktop.appref-ms", true);
                }
            }
            else if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\Startup\\"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\SDE\\SDE.Desktop.appref-ms"))
                {
                    File.Copy
                        (Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\SDE\\SDE.Desktop.appref-ms",
                        Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\Startup\\SDE.Desktop.appref-ms", true);
                }
            }

            if (File.Exists(executavelChrome1))
                navegador = executavelChrome1;
            else if (File.Exists(executavelChrome2))
                navegador = executavelChrome2;
            else if (File.Exists(executavelFirefox))
                navegador = executavelFirefox;
            System.Diagnostics.Process.Start(navegador, urlSwfQueryString);
            //System.Threading.Thread.Sleep(5000);
            //"C:\Documents and Settings\Thiago\Configurações locais\Dados de aplicativos\Google\Chrome\Application\chrome.exe"


            this.IntrovertIMApp.LoadMovie(0, urlServidor + swfProxy);
            connWeb = new ConnWeb(this.IntrovertIMApp);
        }
        private void iniciaFlashSite()
        {
            string flex4 = "http://www.sistemadaempresa.com/sde/";
            IntrovertIMApp.LoadMovie(0, flex4);

        }
        private void escondeSDEDesktop()
        {
            WindowState = FormWindowState.Maximized;
        }

        private ConnWeb connWeb;

        private void Form1_Load(object sender, EventArgs e)
        {
            connWeb.eventoEscreveArquivoTEF += new ConnWeb.EventoEscreveArquivoTEF(connWeb_eventoEscreveArquivoTEF);
            connWeb.eventoEscreveArquivoNFE += new ConnWeb.EventoEscreveArquivoNFE(connWeb_eventoEscreveArquivoNFE);
			connWeb.eventoEscreveArquivoNFExml += new ConnWeb.EventoEscreveArquivoNFExml(connWeb_eventoEscreveArquivoNFExml);
            connWeb.eventoEscreveArquivoDMS += new ConnWeb.EventoEscreveArquivoDMS(connWeb_eventoEscreveArquivoDMS);
            connWeb.eventoImprimeEtiquetas += new ConnWeb.EventoImprimeEtiquetas(connWeb_eventoImprimeEtiquetas);
            connWeb.eventoEscreveArquivoPDF += new ConnWeb.EventoEscreveArquivoPDF(connWeb_eventoEscreveArquivoPDF);
            connWeb.eventoBaixaDuplicata += new ConnWeb.EventoBaixaDuplicata(connWeb_eventoBaixaDuplicata);
            connWeb.eventoBaixaInventario += new ConnWeb.EventoBaixaInventario(connWeb_eventoBaixaInventario);
            connWeb.eventoBaixaRelatorioCliente += new ConnWeb.EventoBaixaRelatorioCliente(connWeb_eventoBaixaRelatorioCliente);
            connWeb.eventoBaixaRelatorioParcialBalanco += new ConnWeb.EventoBaixaRelatorioParcialBalanco(connWeb_eventoBaixaRelatorioParcialBalanco);
            connWeb.eventoBaixaCarne += new ConnWeb.EventoBaixaCarne(connWeb_eventoBaixaCarne);

            connWeb.eventoLoginRVMangueias += new ConnWeb.EventoLoginRVMangueiras(connWeb_eventoLoginRVMangueiras);

            connWeb.eventoBaixaRelatorioOrdemServico += new ConnWeb.EventoBaixaRelatorioOrdemServico(connWeb_eventoBaixaRelatorioOrdemServico);
            //connWeb.eventoIniciaProcesso += new ConnWeb.EventoIniciaProcesso(connWeb_eventoIniciaProcesso);
            string url = "http://200.98.204.144";
            webBrowser1.Navigate(url);
        }

        void connWeb_eventoEscreveArquivoDMS(int idMov, string conteudo)
        {
            GerenteDMS dms = new GerenteDMS();
            dms.escreveArquivoDMS(idMov, conteudo);
        }
        /*
        void connWeb_eventoIniciaProcesso(string executavel, string parametros)
        {
            Console.Beep();
            GerenteProcessos proc = new GerenteProcessos();
            proc.iniciaProcesso(executavel, parametros);
        }
        */
        void connWeb_eventoEscreveArquivoNFE(string conteudo, string chaveAcessoNFE)
        {
            GerenteNFE nfe = new GerenteNFE();
            //nfe +=
            nfe.escreveArquivoNFE(conteudo, chaveAcessoNFE);
        }
		
		void connWeb_eventoEscreveArquivoNFExml(int idCorp, int idEmp, string conteudo, string chaveAcessoNFE)
        {
            GerenteNFE nfe = new GerenteNFE();
            //nfe +=
            nfe.escreveArquivoNFExml(idCorp, idEmp, conteudo, chaveAcessoNFE);
        }

        void connWeb_eventoEscreveArquivoTEF(bool isMultiTef, string conteudo, string complemento)
        {
            if (isMultiTef)
            {
                foreach (Process processo in Process.GetProcesses())
                {
                    if (processo.ProcessName == "MultiTef" || processo.ProcessName == "fbguard")
                    {
                        processo.Kill();
                        //System.Threading.Thread.Sleep(1000);

                    }
                }

                GerenteTEF tef = new GerenteTEF();
                tef.eventoRetornoTEF += new GerenteTEF.EventoRetornoTEF(tef_eventoRetornoTEF);
                tef.escreveArquivoTEF(conteudo, complemento);

                Process.Start(@"C:\Documents and Settings\All Users\Desktop\Firebird 2.0\Firebird Guardian.lnk");
                System.Threading.Thread.Sleep(2500);
                Process.Start(@"C:\Documentos SDE\multitef.bat");
            }
            else
            {
                GerenteTEF tef = new GerenteTEF();
                tef.eventoRetornoTEF += new GerenteTEF.EventoRetornoTEF(tef_eventoRetornoTEF);
                tef.escreveArquivoTEF(conteudo, complemento);
            }
        }

        void tef_eventoRetornoTEF(int coo)
        {
            connWeb.enviaweb_retornoTEF(coo);
        }

        void connWeb_eventoImprimeEtiquetas(int idCorp)
        {
            GerenteEtiquetas barras = new GerenteEtiquetas();
            barras.enviaImpressao(idCorp);
        }

        void connWeb_eventoEscreveArquivoPDF(int idCorp, int idEmp, int idMov, string tipo)
        {
            GerentePDF PDF = new GerentePDF();
            PDF.escreveArquivoPDF(idCorp, idEmp, idMov, tipo);
        }

        void connWeb_eventoBaixaDuplicata(int idCorp, int idEmp, string numeroTitulo, string tipoDocumento)
        {
            GerenteBaixaDuplicata baixaDuplicata = new GerenteBaixaDuplicata();
            baixaDuplicata.baixaDuplicata(idCorp, idEmp, numeroTitulo, tipoDocumento);
        }

        void connWeb_eventoBaixaInventario(int idCorp, int idEmp, string tipoPreco, double pctSobreValor, string dataInventario, string textoCabecalho, bool mostraZerados, string tipoDocumento)
        {
            GerenteBaixaInventario baixaInventario = new GerenteBaixaInventario();
            baixaInventario.baixaInventario(idCorp, idEmp, tipoPreco, pctSobreValor, dataInventario, textoCabecalho, mostraZerados, tipoDocumento);
        }
		
        void connWeb_eventoBaixaRelatorioCliente(int idCorp, int idEmp)
        {
            GerenteBaixaRelatorioCliente baixaRelaorioCliente = new GerenteBaixaRelatorioCliente();
            baixaRelaorioCliente.baixaRelatorioCliente(idCorp, idEmp);
        }
        void connWeb_eventoBaixaRelatorioParcialBalanco(int idCorp)
        {
            GerenteBaixaRelatorioParcialBalanco baixaRelatorioParcialBalanco = new GerenteBaixaRelatorioParcialBalanco();
            baixaRelatorioParcialBalanco.baixaRelatorioParcialBalanco(idCorp);
        }
        void connWeb_eventoBaixaCarne(int idCorp)
        {
            GerenteCarne carne = new GerenteCarne();
            carne.baixaCarne(idCorp);
        }
        void connWeb_eventoBaixaRelatorioOrdemServico(int idCorp, int idOrdemServico)
        {
            GerenteBaixaRelatorioOrdemServico baixaRelatorioOrdemServico = new GerenteBaixaRelatorioOrdemServico();
            baixaRelatorioOrdemServico.baixaRelatorioOrdemServico(idCorp, idOrdemServico);
        }

        void connWeb_eventoLoginRVMangueiras(int idCorp)
        {
            GerenteLoginRVMangueiras loginRVMangueiras = new GerenteLoginRVMangueiras();
            loginRVMangueiras.LoginRVMangueiras(idCorp);
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FrmPrincipal_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void mostrarSDEDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon_DoubleClick(sender, e);
        }

        private void sairDoSDEDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Inicializar()
        {
            if (File.Exists("C:\\Ativa\\CyprusPlus\\fbclient.dll"))
                File.Delete("C:\\Ativa\\CyprusPlus\\fbclient.dll");
            if (File.Exists("C:\\Ativa\\CyprusPlus\\cyprus.sql"))
                File.Delete("C:\\Ativa\\CyprusPlus\\cyprus.sql");
            if (File.Exists("C:\\Ativa\\CyprusPlus\\CYP.FDB"))
                File.Delete("C:\\Ativa\\CyprusPlus\\CYP.FDB");
            if (File.Exists("C:\\Ativa\\CyprusPlus\\dbexpint.dll"))
                File.Delete("C:\\Ativa\\CyprusPlus\\dbexpint.dll");
            foreach (Process processo in Process.GetProcesses())
            {
                if (processo.ProcessName == "fbserver")
                {
                    processo.Kill();
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "http://www.sistemadaempresa.com";
            webBrowser1.Navigate(url);
        }

        

    }
}
