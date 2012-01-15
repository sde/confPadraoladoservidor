using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Flash.External;
using System.Threading;

namespace SDE.Desktop.ConexaoWeb
{
    public class ConnWeb
    {

        private ExternalInterfaceProxy proxy;
        public bool proxyReady { get; private set; }

        public delegate void EventoEscreveArquivoTEF(bool isMultiTef, string conteudo, string complemento);
        public event EventoEscreveArquivoTEF eventoEscreveArquivoTEF;

        public delegate void EventoEscreveArquivoNFE(string conteudo, string chaveAcessoNFE);
        public event EventoEscreveArquivoNFE eventoEscreveArquivoNFE;

		public delegate void EventoEscreveArquivoNFExml(int idCorp, int idEmp, string conteudo, string chaveAcessoNFE);
        public event EventoEscreveArquivoNFExml eventoEscreveArquivoNFExml;

        public delegate void EventoImprimeDanfe(string danfe);
        public event EventoImprimeDanfe eventoImprimeDanfe;

        public delegate void EventoEscreveArquivoDMS(int idMov, string conteudo);
        public event EventoEscreveArquivoDMS eventoEscreveArquivoDMS;

        public delegate void EventoImprimeEtiquetas(int idCorp);
        public event EventoImprimeEtiquetas eventoImprimeEtiquetas;

        public delegate void EventoEscreveArquivoPDF(int idCorp, int idEmp, int idMov, string tipo);
        public event EventoEscreveArquivoPDF eventoEscreveArquivoPDF;

        public delegate void EventoBaixaListaCasamento(int idCorp);
        public event EventoBaixaListaCasamento eventoBaixaListaCasamento;

        public delegate void EventoBaixaDuplicata(int idCorp, int idEmp, string numeroTitulo, string tipoDocumento);
        public event EventoBaixaDuplicata eventoBaixaDuplicata;

        public delegate void EventoBaixaInventario(int idCorp, int idEmp, string tipoPreco, double pctSobreValor, string dataInventario, string textoCabecalho, bool mostraZerados, string tipoDocumento);
        public event EventoBaixaInventario eventoBaixaInventario;

        public delegate void EventoBaixaRelatorioCliente(int idCorp, int idEmp);
        public event EventoBaixaRelatorioCliente eventoBaixaRelatorioCliente;

        public delegate void EventoBaixaRelatorioParcialBalanco(int idCorp);
        public event EventoBaixaRelatorioParcialBalanco eventoBaixaRelatorioParcialBalanco;

        public delegate void EventoBaixaCarne(int idCorp);
        public event EventoBaixaCarne eventoBaixaCarne;

        public delegate void EventoBaixaRelatorioOrdemServico(int idCorp, int idOrdemServico);
        public event EventoBaixaRelatorioOrdemServico eventoBaixaRelatorioOrdemServico;

        public delegate void EventoBaixaRelatorio(int idCorp, string relatorio, string nomeRelatorio);
        public event EventoBaixaRelatorio eventoBaixaRelatorio;

		public delegate void EventoLoginRVMangueiras(int idCorp);
        public event EventoLoginRVMangueiras eventoLoginRVMangueias;

        public ConnWeb(AxShockwaveFlashObjects.AxShockwaveFlash IntrovertIMApp)
        {
            proxy = new ExternalInterfaceProxy(IntrovertIMApp);
            proxy.ExternalInterfaceCall += new ExternalInterfaceCallEventHandler(proxy_ExternalInterfaceCall);
        }

        public void enviaweb_retornoTEF(int coo)
        {
            proxyEnvia("desktop_chama_proxy_executar", "desktop_chama_web_retornotef", coo);
        }
        /*
        public void Fecha()
        {
            proxyEnvia("desktop_chama_proxy_fechar");
        }
        */
        private void proxyEnvia(string fName, params object[] args)
        {
            if (proxyReady)
                proxy.Call(fName, args);
            else
                System.Windows.Forms.MessageBox.Show("proxy inacessivel");
        }



        /*
        void proxyMatouDesktop()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
         * */
        void proxyPingouDesktop()
        {
            //System.Windows.Forms.MessageBox.Show("proxy pingou desktop");
            proxyReady = true;
        }




        object proxy_ExternalInterfaceCall(object sender, ExternalInterfaceCallEventArgs e)
        {
            //MessageBox.Show(e.FunctionCall.FunctionName);
            IList args = e.FunctionCall.Arguments[0] as IList;
            while (args[0] is IList)
                args = args[0] as IList;

            if (e.FunctionCall.FunctionName != "proxy_chama_desktop_executar")
            {
                System.Windows.Forms.MessageBox.Show("não tratado fname == "+e.FunctionCall.FunctionName);
            }

            string comando = args[0] as string;

            //Console.Beep(4000,300);

            /**
             * no caso da web, se o proxy estiver aberto, o desktop também estará
             * no caso do desktop, se o proxy estiver aberto, a web talvez não estará
             * */
            switch (comando)
            {
                case "ping":
                    proxyPingouDesktop();
                    break;
                case "web_chama_desktop_enviatef":
                    this.eventoEscreveArquivoTEF.Invoke(bool.Parse(args[1].ToString()), args[2].ToString(), args[3].ToString());
                    break;
                case "web_chama_desktop_envianfe":
                    this.eventoEscreveArquivoNFE.Invoke(args[1].ToString(), args[2].ToString());
                    break;
				case "web_chama_desktop_envianfe_xml":
                    this.eventoEscreveArquivoNFExml.Invoke(int.Parse(args[1].ToString()), int.Parse(args[2].ToString()),args[3].ToString(), args[4].ToString());
                    break;
                case "web_chama_desktop_imprime_danfe":
                    this.eventoImprimeDanfe.Invoke(Convert.ToString(args[1]));
                    break;
                case "web_chama_desktop_envianfe_prefeitura":
                    this.eventoEscreveArquivoDMS.Invoke(int.Parse(args[1].ToString()), args[2].ToString());
                    break;
                case "web_chama_desktop_imprime_etiquetas":
                    this.eventoImprimeEtiquetas(int.Parse(args[1].ToString()));
                    break;
                case "web_chama_desktop_constroipdf":
                    this.eventoEscreveArquivoPDF.Invoke(int.Parse(args[1].ToString()), int.Parse(args[2].ToString()), int.Parse(args[3].ToString()), args[4].ToString());
                    break;
                case "web_chama_desktop_baixa_listaCasamento":
                    this.eventoBaixaListaCasamento.Invoke(Convert.ToInt32(args[1].ToString()));
                    break;
                case "web_chama_desktop_baixa_duplicata":
                    this.eventoBaixaDuplicata.Invoke(int.Parse(args[1].ToString()), int.Parse(args[2].ToString()), args[3].ToString(), args[4].ToString());
                    break;
                case "web_chama_desktop_baixa_inventario":
                    this.eventoBaixaInventario.Invoke(int.Parse(args[1].ToString()), int.Parse(args[2].ToString()), args[3].ToString(), double.Parse(args[4].ToString()), args[5].ToString(), args[6].ToString(), bool.Parse(args[7].ToString()), args[8].ToString());
                    break;
                case "web_chama_desktop_baixa_relCliente":
                    this.eventoBaixaRelatorioCliente.Invoke(int.Parse(args[1].ToString()), int.Parse(args[2].ToString()));
                    break;
                case "web_chama_desktop_baixa_relParcialBalanco":
                    this.eventoBaixaRelatorioParcialBalanco.Invoke(int.Parse(args[1].ToString()));
                    break;
                case "web_chama_desktop_baixa_carne":
                    this.eventoBaixaCarne.Invoke(int.Parse(args[1].ToString()));
                    break;
                case "web_chama_desktop_baixa_relOrdemServico":
                    this.eventoBaixaRelatorioOrdemServico.Invoke(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                    break;
                case "web_chama_desktop_baixa_relatorio":
                    this.eventoBaixaRelatorio.Invoke(Convert.ToInt32(args[1]), Convert.ToString(args[2]), Convert.ToString(args[3]));
                    break;
                case "web_chama_desktop_login_rvmangueiras":
                    this.eventoLoginRVMangueias.Invoke(Convert.ToInt32(args[1].ToString()));
                    break;
                case "web_chama_desktop_processo":
                    //System.Windows.Forms.MessageBox.Show(comando, "tratei");
                    System.Diagnostics.Process.Start(args[1].ToString(), args[2].ToString());
                    //this.eventoIniciaProcesso.Invoke(args[1].ToString(), args[2].ToString());
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show(comando, "MÉTODO NÃO TRATADO");
                    break;
            }
            return null;
        }




    }
}
