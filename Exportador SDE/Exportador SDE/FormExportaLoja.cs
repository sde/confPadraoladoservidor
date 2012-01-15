using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using SDE.Entidade;
using System.IO;
using System.Xml.Serialization;
using Ionic.Utils.Zip;
using Db4objects.Db4o;
using SDE.Enumerador;
using System.Reflection;
using System.Collections;
using Db4objects.Db4o.Query;

namespace Exportador_SDE
{
    public partial class FormExportaLoja : Form
    {
        List<Cliente> novosCli = new List<Cliente>();
        List<Cliente> reprovadosCli = new List<Cliente>();
        List<Item> novosItem = new List<Item>();
        List<Item> reprovadosItem = new List<Item>();

        OdbcConnection oConn;

        DataTable dtProp;
        Dictionary<int, List<DataRow>> dicPropri;

        IObjectContainer db0;

        public FormExportaLoja()
        {
            InitializeComponent();
            db0 = Db4oFactory.OpenFile("C:\\banco_dados_db4o\\sde\\Banco\\0.dbf");
        }

        private void Escreve(string str)
        {
            txtRelatorio.Text += str + "\r\n";
        }
        private void txtPastaLoja_KeyUp(object sender, KeyEventArgs e)
        {
            Program.raizExportacao = txtPastaLoja.Text;
        }
        private IDataReader readOdbc(string pasta, string arquivoDbf)
        {
            if (oConn != null)
            {
                oConn.Close();
                oConn = null;
            }
            oConn = new OdbcConnection();

            string connStr = string.Concat(
                @"Driver={Microsoft dBase Driver (*.dbf)};SourceType=DBF;SourceDB=",
                pasta,
                "Exclusive=No; Collate=Machine;NULL=NO;DELETED=NO;BACKGROUNDFETCH=NO;"
                );
            string cmdSelect = string.Concat("SELECT * FROM ", pasta, "\\", arquivoDbf);

            oConn.ConnectionString = connStr;
            oConn.Open();
            OdbcCommand oCmd = oConn.CreateCommand();
            oCmd.CommandText = cmdSelect;
            IDataReader dr = oCmd.ExecuteReader();
            return dr;
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            string pastaExportacao = Program.raizExportacao + "\\Exportados";
            if (!Directory.Exists(pastaExportacao))
                Directory.CreateDirectory(pastaExportacao);

            if (txtCnpj.Text.Length < 10 || txtRazaoSocial.Text.Length < 5 || txtNomeFantasia.Text.Length < 5 || txtUsuario.Text.Length < 2)
            {
                MessageBox.Show("Digite dados válidos");
                return;
            }
            bool valido = Utils.ValidaCNPJ(txtCnpj.Text);

            if (!valido)
            {
                MessageBox.Show("O Cpnj não é válido");
                return;
            }

            IDataReader dr;
            string arquivo;

            try
            {
                txtCnpj.Text = txtCnpj.Text.ToUpper();
                txtNomeFantasia.Text = txtNomeFantasia.Text.ToUpper();
                txtRazaoSocial.Text = txtRazaoSocial.Text.ToUpper();
                txtUsuario.Text = txtUsuario.Text.ToUpper();

                arquivo = "c:\\DadosExportados "+ txtNomeFantasia.Text +".dbf";

                if (File.Exists(arquivo))
                    File.Delete(arquivo);

                IObjectContainer dbXXX = Db4oFactory.OpenFile(arquivo);

                #region Marca e Seção

                Cad_Marca marca = new Cad_Marca();
                marca.id = AppFacade.get.reaproveitamento.getIncremento(dbXXX, typeof(Cad_Marca), 0);
                marca.idClienteFuncionarioLogado = 1;
                marca.marca = "GERAL";
                dbXXX.Store(marca);

                Cad_Secao secao = new Cad_Secao();
                secao.id = AppFacade.get.reaproveitamento.getIncremento(dbXXX, typeof(Cad_Secao), 0);
                secao.idClienteFuncionarioLogado = 1;
                secao.secao = "GERAL";
                secao.grupo = "GERAL";
                dbXXX.Store(secao);
                #endregion

                #region Pessoas

                //exportador normal
                /*
                Escreve("Escrevendo Pessoas");
                {
                    Empresa emp = ExportaEmpresas(dbXXX);
                    dbXXX.Store(emp);
                }
                 * */

                {
                    //propriedades
                    dr = readOdbc(Program.raizExportacao + "\\COMUM", "PROPRIED.DBF");
                    DataSet dtstProp = new DataSet();
                    dtstProp.Load(dr, LoadOption.OverwriteChanges, "tbl_propriedades");
                    dtProp = dtstProp.Tables[0];

                    dicPropri = new Dictionary<int, List<DataRow>>();
                    foreach (DataRow drow in dtProp.Rows)
                    {
                        if (drow["CLIE_PRO"] is Double)
                        {
                            int codigoCli = Convert.ToInt32(drow["CLIE_PRO"]);
                            if (!dicPropri.ContainsKey(codigoCli))
                                dicPropri[codigoCli] = new List<DataRow>();
                            dicPropri[codigoCli].Add(drow);
                        }
                    }
                }
                
                //exportador normal
                /*
                #region Clientes
                Escreve("Escrevendo Clientes");
                dr = readOdbc(Program.raizExportacao + "\\COMUM", "CLIENTES.DBF");
                ExportaClientes(dr, dbXXX);
                #endregion
                #region Fornecedores
                Escreve("Escrevendo Fornecedores");
                dr = readOdbc(Program.raizExportacao + "\\COMUM", "FORNECED.DBF");
                ExportaFornecedores(dr, dbXXX);
                #endregion
                 * */

                //exportador obra densa
                #region Fornecedores
                Escreve("Escrevendo Fornecedores");
                dr = readOdbc(Program.raizExportacao + "\\COMUM", "FORNECED.DBF");
                ExportaFornecedores(dr, dbXXX);
                #endregion
                #region Clientes
                Escreve("Escrevendo Clientes");
                dr = readOdbc(Program.raizExportacao + "\\COMUM", "CLIENTES.DBF");
                ExportaClientes(dr, dbXXX);
                #endregion

                Escreve("Escrevendo Pessoas");
                {
                    Empresa emp = ExportaEmpresas(dbXXX);
                    dbXXX.Store(emp);
                }

                /*
                //fiz isso pra inverter 1(UM) id do SDE para o cliente consumidor funcionar corretamente
                IQuery query = dbXXX.Query();
                query.Constrain(typeof(Cliente));
                IObjectSet rs_listaClientes = query.Execute();

                int indice = rs_listaClientes.Count - 2;

                Cliente clienteConsumidor = rs_listaClientes[indice] as Cliente;
                clienteConsumidor.id = 1;
                dbXXX.Store(clienteConsumidor);

                Cliente fornecedorBastardo = rs_listaClientes[0] as Cliente;
                clienteConsumidor.id = indice;
                dbXXX.Store(fornecedorBastardo);
                 * */

                #endregion

                #region Itens
                Escreve("Escrevendo Itens");
                dr = readOdbc(Program.raizExportacao + "\\DADOS1", "PRODUTOS.DBF");
                ExportaItens(dr, dbXXX);
                #endregion

                string cpf_cnpj_restringido = (novosCli[1] as Cliente).cpf_cnpj;
                int index = 0;

                List<Cliente> cEndList = new List<Cliente>();

                //retirei para testes na exportação obra densa
                /*
                foreach (Cliente cli in novosCli)
                {
                    if (cli.cpf_cnpj == cpf_cnpj_restringido && cli.id != 1)
                        index = novosCli.IndexOf(cli);

                    if (cli.__enderecos != null)
                    {
                        if (cli.id != 1 && cli.id != 2)
                            if (cli.__enderecos.Count == 0)
                            {
                                cEndList.Add(cli);
                                reprovadosCli.Add(cli);
                            }
                    }
                    else if (cli.id != 1 && cli.id != 2)
                    {
                        cEndList.Add(cli);
                        reprovadosCli.Add(cli);
                    }
                }
                 * */
                

                novosCli.RemoveAt(index);

                foreach (Cliente cEnd in cEndList)
                    novosCli.Remove(cEnd);

                foreach (Object o in novosCli)
                    dbXXX.Store(o);
                foreach (Object o in novosItem)
                    dbXXX.Store(o);

                Escreve("exportados:");
                Escreve("clientes: " + novosCli.Count.ToString());
                Escreve("produtos:" + novosItem.Count.ToString());
                Escreve("falhos:");
                Escreve("clientes: " + reprovadosCli.Count.ToString());
                Escreve("produtos:" + reprovadosItem.Count.ToString());

                dbXXX.Commit();
                dbXXX.Dispose();

                ZipFile zip = new ZipFile();
                zip.AddFile(arquivo);
                zip.Save(pastaExportacao + "\\dados_exportacao.zip");
                File.Delete(arquivo);

                arquivo = pastaExportacao + "\\1.Clientes.xml";
                using (TextWriter tw = new StreamWriter(arquivo + "_reprovados.xml"))
                {
                    new XmlSerializer(novosCli.GetType()).Serialize(tw, reprovadosCli);
                }
                arquivo = pastaExportacao + "\\2.Produtos.xml";
                using (TextWriter tw = new StreamWriter(arquivo + "_reprovados.xml"))
                {
                    new XmlSerializer(reprovadosItem.GetType()).Serialize(tw, reprovadosItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Close();
            }
        }

        #region Exporta Empresa
        private Empresa ExportaEmpresas(IObjectContainer db)
        {
            Cliente cAdmin = new Cliente()
            {
                id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(Cliente), 0),
                cpf_cnpj = "00000000000",
                nome = "CLIENTE CONSUMIDOR",
                apelido_razsoc = "CLIENTE CONSUMIDOR",
                dtNasc = "01/01/1900",
                tipo = SDE.Enumerador.EPesTipo.Fisica,
                loginUsuario = "ADMIN",
                loginSenha = "ADMIN"
            };
            Cliente cEmp = new Cliente()
            {
                id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(Cliente), 0),
                cpf_cnpj = txtCnpj.Text,
                nome = txtNomeFantasia.Text,
                apelido_razsoc = txtRazaoSocial.Text,
                dtNasc = "01/01/1900",
                tipo = SDE.Enumerador.EPesTipo.Juridica
            };

            Empresa n = new Empresa();
            n.id = 1;
            n.idCliente = cEmp.id;
            n.idClienteAdmin = cAdmin.id;
            n.usuario = txtUsuario.Text;
            //novosCli.Add(cAdmin);
            //novosCli.Add(cEmp);
            db.Store(cAdmin);
            db.Store(cEmp);

            return n;
        }
        #endregion

        #region Exporta Clientes
        private void ExportaClientes(IDataReader dr, IObjectContainer db)
        {
            while (dr.Read())
            {
                string ehContribuinte = dr["CONTRIB"].ToString();

                Cliente c = new Cliente();

                c.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(Cliente), 0);
                c.nome = dr["CLIENTE"].ToString().ToUpper();
                c.nomePai = dr["PAI"].ToString().ToUpper();
                c.nomeMae = dr["MAE"].ToString().ToUpper();
                c.tipo = (dr["TIPO_CLI"].ToString() == "F") ? EPesTipo.Fisica : EPesTipo.Juridica;
                c.apelido_razsoc = dr["FANTASIA"].ToString().ToUpper();
                c.cpf_cnpj = dr["CGC"].ToString();
                try
                {
                    object o = dr["NASCIMENTO"];
                    if (o is DateTime)
                        c.dtNasc = Utils.getDateString((DateTime)o);
                    else
                        c.dtNasc = "01/01/1900";
                }
                catch
                {
                    c.dtNasc = "01/01/1900";
                    Console.Beep();
                }
                c.dtNascTicks = Utils.DateParseBR(c.dtNasc).Ticks;

                c.__contatos = new List<ClienteContato>();
                object obj;

                obj = dr["TELEFONE"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "TELEFONE";
                    pc.tipo = EContatoTipo.fone_fixo;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["FAX"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "FAX";
                    pc.tipo = EContatoTipo.fax;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["CELULAR"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "CELULAR";
                    pc.tipo = EContatoTipo.celular;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["EMAIL"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "EMAIL";
                    pc.tipo = EContatoTipo.email;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["HOMEPAGE"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "HOMEPAGE";
                    pc.tipo = EContatoTipo.homepage;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }

                StringBuilder sb = new StringBuilder();

                obj = dr["CADASTRO"];
                if (obj is DateTime)
                {
                    sb.AppendFormat("cadastrado no MULTISOFT LOJA em: {0}", Utils.getDateString((DateTime)obj));
                    sb.AppendLine();
                }

                obj = dr["ULT_COMPRA"];
                if (obj is DateTime)
                {
                    sb.AppendFormat("ultima compra em: {0}", Utils.getDateString((DateTime)obj));
                    sb.AppendLine();
                }

                #region Endereço
                {
                    c.__enderecos = new List<ClienteEndereco>();

                    string rua = (dr["ENDERECO"] is String) ? dr["ENDERECO"].ToString() : "";
                    string bairro = (dr["BAIRRO"] is String) ? dr["BAIRRO"].ToString() : "";
                    string cidade = (dr["CIDADE"] is String) ? dr["CIDADE"].ToString() : "";
                    string uf = (dr["ESTADO"] is String) ? dr["ESTADO"].ToString() : "";
                    string cep = (dr["CEP"] is String) ? dr["CEP"].ToString() : "";
                    if (string.Concat(rua, bairro, cidade, uf, cep).Length > 11)//cep+estado nao conta
                    {
                        IQuery query;

                        ClienteEndereco cEnd = new ClienteEndereco();
                        cEnd.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteEndereco), 0);
                        cEnd.idCliente = c.id;
                        cEnd.logradouro = rua;
                        cEnd.bairro = bairro;
                        cEnd.cidade = cidade;
                        cEnd.uf = uf;
                        cEnd.cep = cep.Replace("-", "").Trim();

                        if (cEnd.uf != null && cEnd.uf != "")
                        {
                            query = db0.Query();
                            query.Constrain(typeof(IBGEEstado));
                            query.Descend("sigla").Constrain(cEnd.uf.ToUpper());
                            if (query.Execute().Count > 0)
                                cEnd.ufIBGE = (query.Execute()[0] as IBGEEstado).codigo;

                            query = db0.Query();
                            query.Constrain(typeof(IBGEMunicipio));
                            query.Descend("nome").Constrain(cEnd.cidade.ToUpper());
                            if (query.Execute().Count > 0)
                                foreach (IBGEMunicipio munIBGE in query.Execute())
                                    if (munIBGE.codigoEstado == cEnd.ufIBGE)
                                        cEnd.cidadeIBGE = (query.Execute()[0] as IBGEMunicipio).codigo;
                        }

                        if ((cEnd.uf != null && cEnd.cidade != null && cEnd.cep != null && cEnd.bairro != null
                            && cEnd.ufIBGE != null && cEnd.cidadeIBGE != null) &&
                            (cEnd.uf != "" && cEnd.cidade != "" && cEnd.cep != "" && cEnd.bairro != ""
                            && cEnd.ufIBGE != "" && cEnd.cidadeIBGE != ""))
                            c.__enderecos.Add(cEnd);
                    }


                }
                #endregion

                #region Propriedades
                {
                    int codigoCli = Convert.ToInt32(dr["CODIGO"]);

                    if (dicPropri.ContainsKey(codigoCli))
                    {
                        Escreve("escrevendo props do cliente " + codigoCli.ToString() + " qtd de props: " + dicPropri[codigoCli].Count.ToString());
                        foreach (DataRow drow in dicPropri[codigoCli])
                        {
                            IQuery query;

                            ClienteEndereco cPr = new ClienteEndereco();
                            cPr.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteEndereco), 0);
                            cPr.idCliente = c.id;
                            obj = drow["INSE_PRO"];
                            if (obj is string)
                                cPr.inscr = obj.ToString();

                            obj = drow["DESC_PRO"];
                            if (obj is string)
                                cPr.campo = obj.ToString();
                            obj = drow["ENDE_PRO"];
                            if (obj is string)
                                cPr.logradouro = obj.ToString();
                            obj = drow["MUNI_PRO"];
                            if (obj is string)
                                cPr.cidade = obj.ToString();
                            obj = drow["EST_PRO"];
                            if (obj is string)
                                cPr.uf = obj.ToString();
                            obj = drow["CEP_PRO"];
                            if (obj is string)
                                cPr.cep = obj.ToString().Replace("-", "").Trim();
                            obj = drow["FONE_PRO"];
                            if (obj is string)
                                cPr.fone = obj.ToString();
                            obj = drow["TAMA_PRO"];
                            if (obj is string)
                                cPr.tamanho = obj.ToString();


                            obj = drow["GERE_PRO"];
                            if (obj is string)
                                cPr.falarCom = obj.ToString();

                            cPr.complemento = "";
                            obj = drow["LOC1_PRO"];
                            if (obj is string)
                                cPr.complemento += obj.ToString();
                            obj = drow["LOC2_PRO"];
                            if (obj is string)
                                cPr.complemento += obj.ToString();
                            obj = drow["LOC3_PRO"];
                            if (obj is string)
                                cPr.complemento += obj.ToString();

                            cPr.obs = "";
                            obj = drow["OBS1_PRO"];
                            if (obj is string)
                                cPr.obs += obj.ToString();
                            obj = drow["OBS2_PRO"];
                            if (obj is string)
                                cPr.obs += obj.ToString();

                            cPr.bairro = "ZONA RURAL";

                            if (cPr.uf != null && cPr.uf != "")
                            {
                                query = db0.Query();
                                query.Constrain(typeof(IBGEEstado));
                                query.Descend("sigla").Constrain(cPr.uf.ToUpper());
                                if (query.Execute().Count > 0)
                                    cPr.ufIBGE = (query.Execute()[0] as IBGEEstado).codigo;

                                query = db0.Query();
                                query.Constrain(typeof(IBGEMunicipio));
                                query.Descend("nome").Constrain(cPr.cidade.ToUpper());
                                if (query.Execute().Count > 0)
                                    foreach (IBGEMunicipio munIBGE in query.Execute())
                                        if (munIBGE.codigoEstado == cPr.ufIBGE)
                                            cPr.cidadeIBGE = (query.Execute()[0] as IBGEMunicipio).codigo;
                            }

                            if ((cPr.uf != null && cPr.cidade != null && cPr.cep != null && cPr.bairro != null
                                && cPr.ufIBGE != null && cPr.cidadeIBGE != null) &&
                                (cPr.uf != "" && cPr.cidade != "" && cPr.cep != "" && cPr.bairro != ""
                                && cPr.ufIBGE != "" && cPr.cidadeIBGE != ""))
                                c.__enderecos.Add(cPr);
                        }
                    }
                }
                #endregion

                if (dr["CX_POSTAL"] is String)
                    sb.AppendFormat("CAIXA POSTAL: {0}\r\n", dr["CX_POSTAL"]);
                if (dr["INSCRICAO"] is String)
                    sb.AppendFormat("INSCRICAO: {0}\r\n", dr["INSCRICAO"]);
                if (dr["INSC_MUN"] is String)
                    sb.AppendFormat("INSCR MUN: {0}\r\n", dr["INSC_MUN"]);

                c.obs = sb.ToString();

                bool valido = false;

                if (c.tipo == EPesTipo.Fisica)
                    valido = Utils.ValidaCPF(c.cpf_cnpj);
                else if (c.tipo == EPesTipo.Juridica)
                {
                    if (ehContribuinte == "S")
                    {
                        foreach (ClienteEndereco cEnd in c.__enderecos)
                            if (cEnd.inscr != "")
                                valido = true;
                    }
                    else if (ehContribuinte == "N")
                        valido = true;

                    if (valido)
                        valido = Utils.ValidaCNPJ(c.cpf_cnpj);
                }

                if (valido)
                    novosCli.Add(c);
                else
                    reprovadosCli.Add(c);
            }
        }
        #endregion

        #region Exporta Fornecedores
        private void ExportaFornecedores(IDataReader dr, IObjectContainer db)
        {
            while (dr.Read())
            {
                Cliente c = new Cliente();

                c.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(Cliente), 0);
                c.nome = dr["FANTASIA"].ToString();
                c.apelido_razsoc = dr["FORNECEDOR"].ToString();
                c.cpf_cnpj = dr["CGC"].ToString();
                c.tipo = /*(p.cpf_cnpj == null) ? EPesTipo.Fisica : */EPesTipo.Juridica;

                c.dtNasc = "01/01/1900";
                c.dtNascTicks = Utils.DateParseBR(c.dtNasc).Ticks;

                c.__contatos = new List<ClienteContato>();
                object obj;

                obj = dr["TELEFONE1"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "TELEFONE1";
                    pc.tipo = EContatoTipo.fone_fixo;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["TELEFONE2"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "TELEFONE2";
                    pc.tipo = EContatoTipo.fone_fixo;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["FAX"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "FAX";
                    pc.tipo = EContatoTipo.fax;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["DDG"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "DDG";
                    pc.tipo = EContatoTipo.ddg;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["EMAIL"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "EMAIL";
                    pc.tipo = EContatoTipo.email;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }
                obj = dr["HOMEPAGE"];
                if (obj is String && obj.ToString().Length > 5)
                {
                    ClienteContato pc = new ClienteContato();
                    pc.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ClienteContato), 0);
                    pc.idCliente = c.id;
                    pc.campo = "HOMEPAGE";
                    pc.tipo = EContatoTipo.homepage;
                    pc.valor = obj.ToString();
                    c.__contatos.Add(pc);
                }

                StringBuilder sb = new StringBuilder();

                obj = dr["CADASTRO"];
                if (obj is DateTime)
                {
                    sb.AppendFormat("cadastrado no MULTISOFT LOJA em: {0}", Utils.getDateString((DateTime)obj));
                    sb.AppendLine();
                }
                if (dr["ATIVIDADE"] is String)
                    sb.AppendFormat("Atividade: {0}\r\n", dr["ATIVIDADE"]);

                if (dr["CONTATO1"] is String)
                    sb.AppendFormat("Contato1: {0}\r\n", dr["CONTATO1"]);
                if (dr["CONTATO2"] is String)
                    sb.AppendFormat("Contato2: {0}\r\n", dr["CONTATO2"]);

                #region Representante
                {
                    obj = dr["REPRESENTA"];
                    string repr = (obj is String) ? obj.ToString() : "";
                    obj = dr["FONEREP"];
                    string reprFone = (obj is String) ? obj.ToString() : "";
                    obj = dr["CELREP"];
                    string reprCel = (obj is String) ? obj.ToString() : "";
                    obj = dr["FAXREP"];
                    string reprFax = (obj is String) ? obj.ToString() : "";

                    if (string.Concat(repr, reprCel, reprFax, reprFone).Length > 6)//cep+estado nao conta
                        sb.AppendFormat("REPRESENTANTE: {0} Fon: {1} - Fax:{2} - Cel: {3}\r\n", repr, reprCel, reprFax, reprFone);
                }
                #endregion

                #region Endereço
                {
                    string rua = (dr["ENDERECO"] is String) ? dr["ENDERECO"].ToString() : "";
                    string bairro = (dr["BAIRRO"] is String) ? dr["BAIRRO"].ToString() : "";
                    string cidade = (dr["CIDADE"] is String) ? dr["CIDADE"].ToString() : "";
                    string uf = (dr["ESTADO"] is String) ? dr["ESTADO"].ToString() : "";
                    string cep = (dr["CEP"] is String) ? dr["CEP"].ToString() : "";
                    if (string.Concat(rua, bairro, cidade, uf, cep).Length > 11)//cep+estado nao conta
                        sb.AppendFormat("ENDEREÇO - {0} - {1} - {2} - {3} - {4}\r\n", rua, bairro, cidade, uf, cep);
                }
                #endregion

                if (dr["CX_POSTAL"] is String)
                    sb.AppendFormat("CAIXA POSTAL: {0}\r\n", dr["CX_POSTAL"]);
                if (dr["INSCRICAO"] is String)
                    sb.AppendFormat("INSCRICAO: {0}\r\n", dr["INSCRICAO"]);

                if (dr["CONTATO1"] is String)
                    sb.AppendFormat("CONTATO1: {0}\r\n", dr["CONTATO1"]);
                if (dr["CONTATO2"] is String)
                    sb.AppendFormat("CONTATO2: {0}\r\n", dr["CONTATO2"]);
                if (dr["CONTATO3"] is String)
                    sb.AppendFormat("CONTATO3: {0}\r\n", dr["CONTATO3"]);
                if (dr["FONEC1"] is String)
                    sb.AppendFormat("FONE1: {0}\r\n", dr["FONEC1"]);
                if (dr["FONEC2"] is String)
                    sb.AppendFormat("FONE2: {0}\r\n", dr["FONEC2"]);
                if (dr["FONEC3"] is String)
                    sb.AppendFormat("FONE3: {0}\r\n", dr["FONEC3"]);

                c.obs = sb.ToString();

                c.ehFornecedor = true;

                bool valido = false;

                if (c.tipo == EPesTipo.Fisica)
                    valido = Utils.ValidaCPF(c.cpf_cnpj);
                else if (c.tipo == EPesTipo.Juridica)
                    valido = Utils.ValidaCNPJ(c.cpf_cnpj);

                if (valido)
                    novosCli.Add(c);
                else
                    reprovadosCli.Add(c);
            }
        }
        #endregion

        #region Exporta Itens
        private void ExportaItens(IDataReader dr, IObjectContainer db)
        {
            Array lsUnidMed = Enum.GetValues(typeof(EItemUnidMed));
            ArrayList cstValidas = new ArrayList() { "000", "020", "040", "060" };

            while (dr.Read())
            {
                bool valido = dr["DESUSO"].Equals("N");

                object obj;

                Item it = new Item();
                it.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(Item), 0);
                it.idMarca = 1;
                it.idSecao = 1;
                it.__ie = new SDE.Entidade.ItemEmp();
                it.__ie.idEmp = 1;
                ItemEmpAliquotas iea = new ItemEmpAliquotas();
                iea.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ItemEmpAliquotas), 0);
                iea.idItem = it.id;
                iea.idEmp = 1;
                it.__ie.__aliquotas = iea;
                ItemEmpPreco iep = new ItemEmpPreco();
                iep.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ItemEmpPreco), 0);
                iep.idItem = it.id;
                iep.idEmp = 1;
                it.__ie.__preco = iep;

                foreach (FieldInfo f in typeof(ItemEmpAliquotas).GetFields())
                {
                    if (f.Name.StartsWith("icmsAliq"))
                        f.SetValue(iea, 0);
                    else if (f.Name.StartsWith("icmsCST"))
                        f.SetValue(iea, "000");
                }
                iea.pisCST = "08";    //operacao sem insidencia
                iea.cofinsCST = "08"; //operacao sem insidencia
                iea.ipiCST = "52";    //saida    sem insidencia

                #region Item Aliquotas

                obj = dr["IPI"];
                if (obj is double)
                {
                    double x = Convert.ToDouble(obj);
                    if (x > 0)
                    {
                        iea.ipiAliq = x;
                        iea.ipiAliqPadrao = x;
                    }
                }

                obj = dr["TRIBUTACAO"];
                if (obj is string && cstValidas.Contains(obj))
                {
                    iea.icmsCST_ED = obj.ToString();
                    iea.icmsCST_SD = obj.ToString();
                    iea.icmsCST_EF = obj.ToString();

                    obj = dr["ICM_SAI"];
                    if (obj is double)
                    {
                        double temp = Convert.ToInt32(obj);
                        if (temp > 0)
                        {
                            iea.icmsAliq_ED = temp;
                            iea.icmsAliqPadrao_ED = temp;
                            iea.icmsAliq_SD = temp;
                            iea.icmsAliqPadrao_SD = temp;
                            iea.icmsAliq_EF = temp;
                            iea.icmsAliqPadrao_EF = temp;

                            if (dr["REDU_DENTR"] is double)
                            {
                                double redutor = Convert.ToDouble(dr["REDU_DENTR"]);
                                if (redutor > 0)
                                {
                                    temp = temp * redutor / 100;
                                    double reduzida = Math.Round(temp, 1);
                                    iea.icmsAliq_ED = reduzida;
                                    iea.icmsAliq_EF = reduzida;
                                    iea.icmsAliq_SD = reduzida;

                                }
                            }
                        }
                    }

                }
                obj = dr["TRIBUTAC_F"];
                if (obj is string && cstValidas.Contains(obj))
                {
                    iea.icmsCST_SF = obj.ToString();
                    obj = dr["ICM_SAI_FO"];
                    if (obj is double)
                    {
                        double temp = Convert.ToInt32(obj);
                        if (temp > 0)
                        {
                            iea.icmsAliq_SF = temp;
                            iea.icmsAliqPadrao_SF = temp;

                            if (dr["REDU_FORA"] is double)
                            {
                                double redutor = Convert.ToDouble(dr["REDU_FORA"]);
                                if (redutor > 0)
                                {
                                    temp = temp * redutor / 100;
                                    double reduzida = Math.Round(temp, 1);
                                    iea.icmsAliq_SF = reduzida;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Item Preco
                //itemPreco

                obj = dr["PRECO_COMP"];
                if (obj is double)
                    iep.compra = Convert.ToDouble(obj);
                obj = dr["PRECO_CUST"];
                if (obj is double)
                    iep.custo = Convert.ToDouble(obj);
                obj = dr["PRECO_UNIT"];
                if (obj is double)
                    iep.venda = Convert.ToDouble(obj);
                #endregion

                it.nome = dr["DESCRICAO"].ToString().ToUpper();

                it.tipo = EItemTipo.produto;

                /*
                string strRfUnica = dr["FABRICANTE"].ToString();
                if (strRfUnica == "")
                    it.rfUnica = "#" + it.id.ToString();
                else
                    it.rfUnica = strRfUnica;
                 * */
                string strRfUnica = dr["BARRA"].ToString();
                it.rfUnica = strRfUnica.ToUpper();
                string strRfAuxiliar = dr["PRODUTO"].ToString();
                it.rfAuxiliar = strRfAuxiliar.ToUpper();
                string strUnidMed = dr["UNIDADE"].ToString();
                foreach (EItemUnidMed unidMed in lsUnidMed)
                {
                    if (strUnidMed == unidMed.ToString())
                        it.unidMed = unidMed;
                }
                //se não encontrar, cai em UN

                it.grupo = "GERAL";
                it.secao = "GERAL";
                it.subgrupo = "GERAL";
                it.marca = "GENERICA";
                it.modelo = "GERAL";

                it.tipo
                    = (dr["CLASSE"] == ("S" as object))
                    ? EItemTipo.servico
                    : EItemTipo.produto;

                obj = dr["PRATELEIRA"];
                if (obj is string)
                    it.locacao1 = obj.ToString();
                /*
                obj = dr["FABRICANTE"];
                if (obj is string)
                    it.rfAuxiliar = obj.ToString();
                */
                obj = dr["FISCAL"];
                if (obj is string)
                    it.classificacaoFiscal = obj.ToString();
                obj = dr["OBSERVACAO"];
                if (obj is string)
                    it.obs = obj.ToString();

                double qtd = 0;
                obj = dr["ESTOQUE"];
                if (obj.ToString() != null && obj.ToString() != "")
                    qtd = double.Parse(dr["ESTOQUE"].ToString());
                if (qtd < 0)
                    qtd = 0;
                ItemEmpEstoque iee = new ItemEmpEstoque();
                iee.id = AppFacade.get.reaproveitamento.getIncremento(db, typeof(ItemEmpEstoque), 0);
                iee.idItem = it.id;
                iee.idEmp = 1;
                iee.qtd = qtd;
                iee.identificador = "U:U";
                iee.codBarras = string.Concat("B", iee.id.ToString().PadLeft(6, '0'));
                it.__estoques = new List<ItemEmpEstoque>() { iee };

                if (valido)
                    novosItem.Add(it);
                else
                    reprovadosItem.Add(it);
            }
        }
        #endregion

        private void FormExportaLoja_Load(object sender, EventArgs e)
        {
            Program.raizExportacao = txtPastaLoja.Text;
        } 


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtPastaLoja.Text = folderBrowserDialog.SelectedPath;
                Program.raizExportacao = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
