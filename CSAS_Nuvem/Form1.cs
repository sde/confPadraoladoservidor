using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using SDE.Atributos;

namespace CSAS_Nuvem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //txtAssembly.Text = @"F:\sistemadaempresa\fontes\servidor1\SDE\bin\SDE.dll";

            txtDestino.Text = @"C:\dev\sistemadaempresa\ambiente_desenvolvimento\principal\lado_cliente\src\";

        }

        private void btnGera_Click(object sender, EventArgs e)
        {
            //Assembly a = Assembly.LoadFile( txtAssembly.Text );

            Assembly a = Assembly.Load("SDE");

            Type[] tipos = a.GetTypes();//referenciar as dlls neste projeto

            string nsEntidade = "SDE.Entidade";
            List<Type> tiposEntidadesTodas = filtraTipos(tipos, nsEntidade);
            List<Type> tiposEntidades = new List<Type>();
            foreach (Type t in tiposEntidadesTodas)
            {
                AtEnt at = AtributoUtils.getAtributos(t);
                if (at.baixaLocal)
                    tiposEntidades.Add(t);
            }

            List<Type> tiposEnumeradores = filtraTipos(tipos, "SDE.Enumerador");



            //List<Type> tiposEnum = filtraTipos(tipos, "SDE.Enumerador");
            List<Type> tiposNuvem = filtraTipos(tipos, "SDE.CamadaNuvem");

            //SDE.CamadaNuvem.NuvemListagem

            string diretorio = txtDestino.Text +"SDE\\CamadaNuvem\\";
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            string fileName = "";
            string codigoAS = "";
            //foreach (Type t in tiposNuvem)
            //{
            Type tNuvemLst = a.GetType("SDE.CamadaNuvem.NuvemListagem");
            fileName = diretorio + "NuvemListagem.as";
            codigoAS = getAlgoritimoNuvemListagem(tNuvemLst, tiposEntidades);
            File.WriteAllText(fileName, codigoAS);
            fileName = diretorio + "NuvemCache.as";
            codigoAS = getAlgoritimoNuvemCache(tNuvemLst, tiposEntidades);
            File.WriteAllText(fileName, codigoAS);
            //}
            Type tNuvemMod = a.GetType("SDE.CamadaNuvem.NuvemModificacoes");
            fileName = diretorio + "NuvemModificacoes.as";
            codigoAS = getAlgoritimoNuvemModificacoes(tNuvemMod, nsEntidade);
            File.WriteAllText(fileName, codigoAS);

            Type tNuvemNot = a.GetType("SDE.CamadaNuvem.NuvemNotificacoes");
            fileName = diretorio + "NuvemNotificacoes.as";
            codigoAS = getAlgoritimoNuvemNotificacoes(tNuvemNot, nsEntidade);
            File.WriteAllText(fileName, codigoAS);

            diretorio = txtDestino.Text + "SDE\\";
            fileName = diretorio + "Nuvens.as";
            codigoAS = getAlgoritimoASNuvemCentral();
            File.WriteAllText(fileName, codigoAS);

            diretorio = txtDestino.Text + "SDE\\Entidade\\";
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            Dictionary<string, string> dictEntidades = getAlgoritimoASEntidades(tiposEntidades);
            foreach (KeyValuePair<string, string> parEntidade in dictEntidades)
            {
                fileName = diretorio + parEntidade.Key;
                File.WriteAllText(fileName, parEntidade.Value);
            }

            diretorio = txtDestino.Text + "SDE\\Componentes\\Combo\\";
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);
            dictEntidades = getAlgoritimo_Componentes_Combo(tiposEntidades);
            foreach (KeyValuePair<string, string> parEntidade in dictEntidades)
            {
                fileName = diretorio + parEntidade.Key;
                File.WriteAllText(fileName, parEntidade.Value);
            }


            diretorio = txtDestino.Text + "SDE\\Enumerador\\";
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            Dictionary<string, string> dictEnumeradores = getAlgoritimoASEnumeradores(tiposEnumeradores);
            foreach (KeyValuePair<string, string> parEnumerador in dictEnumeradores)
            {
                fileName = diretorio + parEnumerador.Key;
                File.WriteAllText(fileName, parEnumerador.Value);
            }






            diretorio = txtDestino.Text + "SDE\\Constantes\\";
            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);
            Type tConstVariaveisSdeConfig = a.GetType("SDE.Constantes.Variaveis_SdeConfig");
            fileName = diretorio + "Variaveis_SdeConfig.as";
            codigoAS = getAlgoritimoConstantes(tConstVariaveisSdeConfig);
            File.WriteAllText(fileName, codigoAS);






            //MessageBox.Show("ok");
            this.Close();
        }


        private string getAlgoritimoConstantes(Type tConstVariaveisSdeConfig)
        {
            StringBuilder sb = new StringBuilder();
            string ESPACO = "    ";

            sb.AppendLine("package SDE.Constantes");
            sb.AppendLine("{");
            
            sb.AppendFormat("{0}\n", ESPACO);
            sb.AppendFormat("{0}public final class Variaveis_SdeConfig\n", ESPACO);
            sb.AppendFormat("{0}{{\n", ESPACO);

            FieldInfo[] campos = tConstVariaveisSdeConfig.GetFields();

            sb.AppendFormat("{0}{0}public static function getCampos():Array{{return[", ESPACO);
            int iMax = campos.Length - 1;
            int i = 0;
            while (i <= iMax)
            {
                FieldInfo f = campos[i];
                i++;
                string[] aaa = (string[])f.GetValue(null);

                sb.AppendFormat("'{0}'", aaa[0]);
                if (i <= iMax)
                    sb.Append(",");
            }
            sb.AppendLine("]};");

            foreach (FieldInfo f in campos)
            {
                string[] aaa = (string[])f.GetValue(null);
                sb.AppendFormat("{0}{0}public static var {1}:String = '{2}';\n", ESPACO, f.Name, aaa[0]);
            }

            sb.AppendFormat("{0}}}\n", ESPACO);
            sb.AppendLine("}");
            return sb.ToString();
        }



        private List<Type> filtraTipos(Type[] tipos, string ns)
        {
            List<Type> ls = new List<Type>();
            foreach (Type t in tipos)
                if (t.Namespace == ns && !t.Name.StartsWith("Super"))
                    ls.Add(t);
            return ls;
        }



        private string transformaTipo(Type t)
        {
            string tipo = t.Name;

            if (tipo.Contains("Int") || tipo == "Double")
                tipo = "Number";
            else if (tipo == "DateTime")
                tipo = "Date";

            if (tipo.Contains("[]") || t.Namespace.Contains("Collection"))
                tipo = "Array";
            if (t.Name.Contains("Dictionary"))
                tipo = "Object";

            return tipo;
        }








        
        private Dictionary<string, string> getAlgoritimoASEnumeradores(List<Type> tiposEnumeradores)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string ESPACO = "    ";

            foreach (Type t in tiposEnumeradores)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("package {0}\n", t.Namespace);
                sb.AppendLine("{");
                sb.AppendFormat("{0}public final class {1}\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{{\n", ESPACO);

                FieldInfo[] campos = t.GetFields();
                #region static function getCampos():Array
                sb.AppendFormat("{0}{0}public static function getCampos():Array{{return[", ESPACO);
                int iMax = campos.Length - 1;
                int i = 0;
                while (i <= iMax)
                {
                    FieldInfo f = campos[i];
                    i++;
                    if (t.IsEnum && f.Name == "value__")
                        continue;

                    sb.AppendFormat("'{0}'", f.Name);
                    if (i <= iMax)
                        sb.Append(",");

                }
                sb.AppendLine("]};");
                sb.AppendFormat("{0}{0}\n", ESPACO);
                #endregion

                #region public var campo:Tipo;
                i = 0;
                while (i <= iMax)
                {
                    FieldInfo f = campos[i];
                    i++;

                    if (t.IsEnum && f.Name == "value__")
                        continue;

                    sb.AppendFormat("{0}{0}public static const {1}:String = '{1}';\n", ESPACO, f.Name);
                }
                #endregion

                #region public static function valida()
                sb.AppendFormat("{0}{0}\n", ESPACO);
                sb.AppendFormat("{0}{0}public static function valida(value:String):Boolean\n", ESPACO);
                sb.AppendFormat("{0}{0}{{\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}return (getCampos().indexOf(value)>-1);\n", ESPACO);
                sb.AppendFormat("{0}{0}}}\n", ESPACO);
                #endregion

                sb.AppendFormat("{0}}}\n", ESPACO);//end class
                sb.AppendLine("}");//end package

                ret.Add(t.Name + ".as", sb.ToString());
            }
            return ret;
        }



        private Dictionary<string, string> getAlgoritimoASEntidades(List<Type> tiposEntidades)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string ESPACO = "    ";

            foreach (Type t in tiposEntidades)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("package {0}\n", t.Namespace);
                sb.AppendLine("{");
                sb.AppendFormat("{0}[Bindable]\n", ESPACO);
                sb.AppendFormat("{0}[RemoteClass(alias='{1}.{2}')]\n", ESPACO, t.Namespace, t.Name);
                sb.AppendFormat("{0}public final class {1}\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{{\n", ESPACO);

                sb.AppendFormat("{0}{0}public static function get CLASSE():String{{return '{1}';}}\n", ESPACO, t.Name);
                
                
                FieldInfo[] campos = t.GetFields();
                #region static function getCampos():Array
                sb.AppendFormat("{0}{0}public static function getCampos():Array{{return[", ESPACO);
                int iMax = campos.Length - 1;
                int i = 0;
                while (i <= iMax)
                {
                    FieldInfo f = campos[i];

                    sb.AppendFormat("'{0}'", f.Name);
                    if (i < iMax)
                        sb.Append(",");
                    i++;

                }
                sb.AppendLine("]};");
                sb.AppendFormat("{0}{0}\n", ESPACO);
                i = 0;
                while (i <= iMax)
                {
                    FieldInfo f = campos[i];
                    sb.AppendFormat("{0}{0}public static var campo_{1}:String = '{1}';\n", ESPACO, f.Name);
                    i++;
                }
                #endregion



                #region public function

                sb.AppendFormat("{0}{0}public function {1}(obj:Object=null){{if (obj==null)return;for each(var campo:String in getCampos())this[campo]=obj[campo];}}", ESPACO, t.Name);
                
                sb.AppendFormat("{0}{0}public function injeta(o:*):void{{for each (var campo:String in {1}.getCampos()){{this[campo]=o[campo];}}}}\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{0}public function clone():{1}{{return new {1}(this);}}\n", ESPACO, t.Name);



                sb.AppendFormat("{0}{0}public function toString():String\n", ESPACO);
                sb.AppendFormat("{0}{0}{{\n", ESPACO);

                AtEnt at = AtributoUtils.getAtributos(t);
                if (at.toString == null)
                {
                    //sb.AppendFormat("{0}{0}{0}var s:String='';for each(var campo:String in getCampos()){{s+=campo+':\"'+this[campo]+'\",';}}return '{{'+s.substr(0,s.length-1)+'}}';\n", ESPACO);
                    sb.AppendFormat("{0}{0}{0}return '[{1} '+id+']';\n", ESPACO, t.Name);
                }
                else
                {
                    sb.AppendFormat("{0}{0}{0}return {1};\n", ESPACO, at.toString);
                }

                sb.AppendFormat("{0}{0}}}\n", ESPACO);


                #endregion


                #region public var campo:Tipo;
                foreach (FieldInfo f in t.GetFields())
                {
                    string tipo = transformaTipo(f.FieldType);

                    if (f.FieldType.IsEnum)
                        tipo = string.Format("String = '{0}'", f.FieldType.GetFields()[1].Name);//__value, 0, 1,2...
                    else
                        if (tipo == "Number")
                            tipo += " = 0";
                        else
                            if (tipo == "String")
                                tipo += " = ''";
                            else
                                if (tipo == "Boolean")
                                    tipo += " = false";
                                else
                                    tipo += " = null";//Arrays e Complexos
                    sb.AppendFormat("{0}{0}public var {1}:{2};\n", ESPACO, f.Name, tipo);
                }
                #endregion

                sb.AppendFormat("{0}}}\n", ESPACO);//end class
                sb.AppendLine("}");//end package

                ret.Add(t.Name + ".as", sb.ToString());
            }
            return ret;
        }






        private string getAlgoritimoASNuvemCentral()
        {
            StringBuilder sb = new StringBuilder();
            string ESPACO = "    ";

            sb.AppendLine("package SDE");
            sb.AppendLine("{");
            sb.AppendFormat("{0}import SDE.CamadaNuvem.*;\n", ESPACO);
            /*
            foreach (Type t in tiposNuvem)
            {
                sb.AppendFormat("{0}import {1}.{2};\n", ESPACO, t.Namespace, t.Name);
            }
             * */
            sb.AppendFormat("{0}\n", ESPACO);
            sb.AppendFormat("{0}public final class Nuvens\n", ESPACO);
            sb.AppendFormat("{0}{{\n", ESPACO);

            //sb.AppendFormat("{0}{0}private static var _:{1};\n", ESPACO, t.Name);

            sb.AppendFormat("{0}{0}private static var _:Nuvens = new Nuvens();\n", ESPACO);
            sb.AppendFormat("{0}{0}public static function get instancia():Nuvens{{return _;}}\n", ESPACO);
            sb.AppendFormat("{0}{0}\n", ESPACO);

            /*
        public var cache:NuvemCache = new NuvemCache();
        public var listagem:NuvemListagem = new NuvemListagem(cache);
        public var modificacoes:NuvemModificacoes = new NuvemModificacoes();
            */

            sb.AppendFormat("{0}{0}public var cache:NuvemCache;\n", ESPACO);
            sb.AppendFormat("{0}{0}public var listagem:NuvemListagem;\n", ESPACO);
            sb.AppendFormat("{0}{0}public var modificacoes:NuvemModificacoes;\n", ESPACO);
            sb.AppendFormat("{0}{0}public var notificacoes:NuvemNotificacoes;\n", ESPACO);


            
            sb.AppendFormat("{0}{0}\n", ESPACO);

            sb.AppendFormat("{0}{0}public function Inicializa():void\n", ESPACO);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}cache = new NuvemCache();\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}listagem = new NuvemListagem();\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}listagem.Inicializa();\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}modificacoes = new NuvemModificacoes();\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}notificacoes = new NuvemNotificacoes();\n", ESPACO);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);

            sb.AppendFormat("{0}{0}public function Fecha():void\n", ESPACO);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}notificacoes.Fecha();\n", ESPACO);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);
            /*
            sb.AppendFormat("{0}{0}public var cache:NuvemCache = new NuvemCache();\n", ESPACO);
            sb.AppendFormat("{0}{0}public var listagem:NuvemListagem = new NuvemListagem();\n", ESPACO);
            sb.AppendFormat("{0}{0}public var modificacoes:NuvemModificacoes = new NuvemModificacoes();\n", ESPACO);
            /*
            foreach (Type t in tiposNuvem)
            {
                string nomeVar = t.Name.Replace("Nuvem", "").ToLower();
                sb.AppendFormat("{0}{0}public var {2}:{1} = new {1}();\n", ESPACO, t.Name, nomeVar);
            }
        */





            sb.AppendFormat("{0}}}\n", ESPACO);
            sb.AppendLine("}");
            return sb.ToString();

        }

        private string getAlgoritimoNuvemNotificacoes(Type t, string nsEntidade)
        {
            StringBuilder sb = new StringBuilder();
            string ESPACO = "    ";

            sb.AppendFormat("package {0}\n", t.Namespace);
            sb.AppendLine("{");

            sb.AppendFormat("{0}import mx.rpc.events.ResultEvent;\n", ESPACO);
            sb.AppendFormat("{0}import Core.App;\n", ESPACO);
            sb.AppendFormat("{0}import Core.Sessao;\n", ESPACO);
            sb.AppendFormat("{0}import Core.Alerta.AlertaSistema;\n", ESPACO);
            sb.AppendFormat("{0}import Core.ConexaoServidor.MyRemoteObject;\n", ESPACO);
            sb.AppendFormat("{0}import SDE.Entidade.*;\n", ESPACO);
            sb.AppendFormat("{0}import SDE.Outros.*;\n", ESPACO);
            sb.AppendFormat("{0}import Core.Utils.MeuFiltroWhere;\n", ESPACO);
            sb.AppendFormat("{0}import flash.utils.setInterval;\n", ESPACO);
            sb.AppendFormat("{0}import flash.utils.clearInterval;\n", ESPACO);

            sb.AppendFormat("{0}import mx.controls.Alert;\n", ESPACO);

            sb.AppendFormat("{0}\n", ESPACO);

            sb.AppendFormat("{0}public final class NuvemNotificacoes\n", ESPACO);
            sb.AppendFormat("{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}private var ro:MyRemoteObject = new MyRemoteObject('{1}.{2}');\n", ESPACO, t.Namespace, t.Name);
            sb.AppendFormat("{0}{0}private var ultima:Number = 0;\n", ESPACO);
            sb.AppendFormat("{0}{0}private var TEMPO_INTERVALO_ATUALIZA:Number = {1};\n", ESPACO, 1000 * 5);
            sb.AppendFormat("{0}{0}private var idInterval:uint = 0;\n", ESPACO);

            sb.AppendFormat("{0}{0}\n", ESPACO);

            sb.AppendFormat("{0}{0}public function NuvemNotificacoes()\n", ESPACO);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}idInterval = setInterval(Lista_Notificacoes, TEMPO_INTERVALO_ATUALIZA);\n", ESPACO);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}public function Fecha():void\n", ESPACO);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}clearInterval(idInterval);\n", ESPACO);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}public function Lista_Notificacoes(fRetorno:Function=null):void\n", ESPACO);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}if (!Sessao.unica.isLogado)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}return;\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}ro.Invoca('Lista_Notificacoes', [Sessao.unica.idCorp, ultima],\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}function(retorno:Array):void\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}for each(var at:Atualizacao in retorno)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}var todos:Array = App.single.cache['array'+at.classe];\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}ultima = at.idAtualizacao;\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}var filtro:MeuFiltroWhere = new MeuFiltroWhere(todos).andEquals(at.idObj);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}var filtrados:Array = filtro.getResultadoArraySimples();\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}//queremos que o resultado seja 1\n", ESPACO);


            sb.AppendFormat("{0}{0}{0}{0}{0}{0}if (filtrados.length==0)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}if (at.ehInsercao)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}todos.push(at.obj);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}ultima = at.idAtualizacao;\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}AlertaSistema.mensagem('INS ultima agora é '+ultima+' de um filtro de '+filtrados.length +' idObj: '+at.idObj, true);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}else\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}AlertaSistema.mensagem('DEL ZERO ultima agora é '+ultima+' de um filtro de '+filtrados.length +' idObj: '+at.idObj, true);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}}}\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}else if (filtrados.length==1)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}var objAtualizar:Object = filtrados[0];\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}for(var i:int=0; i<todos.length; i++)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}if (todos[i].id == at.idObj)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}if (at.ehInsercao)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}todos[i] = at.obj;\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}else\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}todos.splice(i,1);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}{0}AlertaSistema.mensagem('DEL UM ultima agora é '+ultima+' de um filtro de '+filtrados.length +' idObj: '+at.idObj, true);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}{0}break;\n", ESPACO);
            
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}ultima = at.idAtualizacao;\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}AlertaSistema.mensagem('UPD ultima agora é '+ultima+' de um filtro de '+filtrados.length, true);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}}}\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}{0}{0}else\n", ESPACO);
            //sb.AppendFormat("{0}{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}Alert.show( ultima+' | foram encontradas '+filtrados.length+' instancias no caché local\\n'+at.classe+' id: '+at.idObj, 'erro!');\n", ESPACO);
            //sb.AppendFormat("{0}{0}{0}{0}{0}{0}}}\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}if (fRetorno != null)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}fRetorno();\n", ESPACO);

            //sb.AppendFormat("{0}{0}{0}{0}{0}\n", ESPACO);
            //sb.AppendFormat("{0}{0}{0}{0}{0}\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0});\n", ESPACO);
            //sb.AppendFormat("{0}{0}{0}\n", ESPACO);
            //sb.AppendFormat("{0}{0}{0}\n", ESPACO);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}\n", ESPACO);
            /*
                                    //queremos que o resultado seja 1
                                    else
                                    {
                                        Alert.show( ultima+" | foram encontradas "+filtrados.length+" instancias no cachê local\n"+at.classe+" id: "+at.idObj, "erro!");
                                    }
                                }
                            }
                        );
                    }
            */

            sb.AppendFormat("{0}}}\n", ESPACO);

            sb.AppendLine("}");
            return sb.ToString();
        }
        private string getAlgoritimoNuvemModificacoes(Type t, string nsEntidade)
        {
            StringBuilder sb = new StringBuilder();
            string ESPACO = "    ";

            sb.AppendFormat("package {0}\n", t.Namespace);
            sb.AppendLine("{");

            sb.AppendFormat("{0}import mx.rpc.events.ResultEvent;\n", ESPACO);
            sb.AppendFormat("{0}import Core.Sessao;\n", ESPACO);
            sb.AppendFormat("{0}import Core.ConexaoServidor.MyRemoteObject;\n", ESPACO);
            sb.AppendFormat("{0}import SDE.Entidade.*;\n", ESPACO);
            sb.AppendFormat("{0}\n", ESPACO);

            sb.AppendFormat("{0}public final class {1}\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}private var ro:MyRemoteObject = new MyRemoteObject('{1}.{2}');\n", ESPACO, t.Namespace, t.Name);

            //cada método
            foreach (MethodInfo m in t.GetMethods())
            {
                if (m.DeclaringType != t)
                    continue;
                
                #region Comentários
                string tipoRetornoCS = m.ReturnType.Name;
                string tipoRetornoAS = transformaTipo(m.ReturnType);
                if (m.ReturnType.IsGenericType)
                {
                    Type[] tArgs = m.ReturnType.GetGenericArguments();
                    tipoRetornoCS = tArgs[0].Name + "[]";
                }
                sb.AppendFormat("{0}{0}//ev.result{0}//CS={1}{0}//AS={2}\n", ESPACO, tipoRetornoCS, tipoRetornoAS);
                #endregion

                #region public function metodo(args:*):void
                sb.AppendFormat("{0}{0}public function {1}(", ESPACO, m.Name);

                int larguraParametros = m.GetParameters().Length;
                for (int i = 0; i < larguraParametros; i++)
                {
                    ParameterInfo p = m.GetParameters()[i];

                    //
                    if (p.Name == "idCorp" || p.Name == "idEmp" || p.Name == "idClienteFuncionarioLogado")// || p.Name == "VERSAO_CLIENTE"
                        continue;

                    string tipo = transformaTipo(p.ParameterType);
                    sb.AppendFormat("{0}:{1}, ", p.Name, tipo);
                }
                sb.AppendLine("fRetorno:Function=null):void");
                #endregion
                #region { ... }
                sb.AppendFormat("{0}{0}{{\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}ro.Invoca('{1}', [", ESPACO, m.Name);
                for (int i = 0; i < larguraParametros; i++)
                {
                    ParameterInfo p = m.GetParameters()[i];
                    string nomeParam = p.Name;

                    if (nomeParam == "idCorp")
                        nomeParam = "Sessao.unica.idCorp";
                    else if (nomeParam == "idEmp")
                        nomeParam = "Sessao.unica.idEmp";
                    else if (nomeParam == "idClienteFuncionarioLogado")
                        nomeParam = "Sessao.unica.idClienteFuncionarioLogado";

                    sb.AppendFormat("{0}", nomeParam);
                    if (i < larguraParametros - 1)
                        sb.Append(", ");
                }
                if (tipoRetornoAS=="Void")
                    sb.AppendFormat("], function():void{{Sessao.unica.nuvens.notificacoes.Lista_Notificacoes(function():void{{fRetorno();}});}});\n");
                else
                    sb.AppendFormat("], function(r:{0}):void{{Sessao.unica.nuvens.notificacoes.Lista_Notificacoes(function():void{{if(fRetorno!=null)fRetorno(r);}});}});\n", tipoRetornoAS);
                sb.AppendFormat("{0}{0}}}\n", ESPACO);
                #endregion
            }

            sb.AppendFormat("{0}}}\n", ESPACO);

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string getAlgoritimoNuvemCache(Type t, List<Type> tiposEntidades)
        {
            StringBuilder sb = new StringBuilder();
            string ESPACO = "    ";

            sb.AppendFormat("package {0}\n", t.Namespace);
            sb.AppendLine("{");

            //foreach (Type tEnt in tiposEntidades)
            //    sb.AppendFormat("{0}import SDE.Entidade.{1};\n", ESPACO, tEnt.Name);

            sb.AppendFormat("{0}import SDE.Entidade.*;\n", ESPACO);
            sb.AppendFormat("{0}import mx.collections.ArrayCollection;\n", ESPACO);

            //System.Windows.Forms.MessageBox.Show(lsParamImports.Count.ToString());
            sb.AppendFormat("{0}\n", ESPACO);

            sb.AppendFormat("{0}public final class NuvemCache\n", ESPACO);
            sb.AppendFormat("{0}{{\n", ESPACO);

            //string tipoRetornoAS = transformaTipo(t);

            foreach (Type tEnt in tiposEntidades)
                sb.AppendFormat("{0}{0}[Bindable]public var array{1}:Array = null;\n", ESPACO, tEnt.Name);
            foreach (Type tEnt in tiposEntidades)//SDE.Entidade.
                sb.AppendFormat("{0}{0}public function get{1}(id:Number):{1}{{for each(var xxx:{1} in this.array{1}){{if (xxx.id==id)return xxx;}}return null;}}\n", ESPACO, tEnt.Name);
            foreach (Type tEnt in tiposEntidades)
                sb.AppendFormat("{0}{0}public function get clone{1}():Array{{var r:Array=[];for each(var xxx:{1} in this.array{1}){{r.push(xxx.clone());}}return r;}}\n", ESPACO, tEnt.Name);
            foreach (Type tEnt in tiposEntidades)
                sb.AppendFormat("{0}{0}public function get arrayc{1}():ArrayCollection{{var r:ArrayCollection=new ArrayCollection();for each(var xxx:* in this.array{1}){{r.addItem(xxx.clone());}}return r;}}\n", ESPACO, tEnt.Name);

            sb.AppendFormat("{0}}}\n", ESPACO);
            sb.AppendLine("}");

            return sb.ToString();
        }

        private Dictionary<string, string> getAlgoritimo_Componentes_Combo(List<Type> tiposEntidades)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string ESPACO = "    ";

            foreach (Type t in tiposEntidades)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("package SDE.Componentes.Combo\n");
                sb.AppendLine("{");

                sb.AppendFormat("{0}import Core.App;\n", ESPACO);
                sb.AppendFormat("{0}import SDE.Entidade.{1};\n", ESPACO, t.Name);
                sb.AppendFormat("{0}import mx.controls.ComboBox;\n", ESPACO);
                //sb.AppendFormat("{0}[Bindable]\n", ESPACO);
                //sb.AppendFormat("{0}[RemoteClass(alias='{1}.{2}')]\n", ESPACO, t.Namespace, t.Name);
                sb.AppendFormat("{0}public final class Cmb{1} extends ComboBox\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{{\n", ESPACO);

                //AtEnt at = AtributoUtils.getAtributos(t);

                sb.AppendFormat("{0}{0}public function Cmb{1}()\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{0}{{\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}super();\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}dataProvider = App.single.cache.array{1};\n", ESPACO, t.Name);
                //sb.AppendFormat("{0}{0}{0}labelField = {1}.campo_{2};\n", ESPACO, t.Name, at.labelField);
                sb.AppendFormat("{0}{0}}}\n", ESPACO);

                sb.AppendFormat("{0}{0}public function setIdentificador(identificador:Number):void\n", ESPACO);
                sb.AppendFormat("{0}{0}{{\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}if (identificador==0)\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}{0}selectedIndex = 0;\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}else\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}{0}selectedItem = App.single.cache.get{1}(identificador);\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{0}}}\n", ESPACO);

                sb.AppendFormat("{0}{0}public function getAs():{1}\n", ESPACO, t.Name);//{1}
                sb.AppendFormat("{0}{0}{{\n", ESPACO);
                sb.AppendFormat("{0}{0}{0}return selectedItem as {1};\n", ESPACO, t.Name);
                sb.AppendFormat("{0}{0}}}\n", ESPACO);

                sb.AppendFormat("{0}}}\n", ESPACO);//end class
                sb.AppendLine("}");//end package

                ret.Add("Cmb"+t.Name + ".as", sb.ToString());
            }
            return ret;
        }


        
        private string getAlgoritimoNuvemListagem(Type t, List<Type> tiposEntidades)
        {
            StringBuilder sb = new StringBuilder();
            string ESPACO = "    ";

            sb.AppendLine("package SDE.CamadaNuvem");
            sb.AppendLine("{");

            sb.AppendFormat("{0}import mx.rpc.events.ResultEvent;\n", ESPACO);
            sb.AppendFormat("{0}import mx.core.Application;\n", ESPACO);
            sb.AppendFormat("{0}import Core.App;\n", ESPACO);
            sb.AppendFormat("{0}import Core.ConexaoServidor.MyRemoteObject;\n", ESPACO);
            sb.AppendFormat("{0}import SDE.Entidade.*;\n", ESPACO);

            
            //System.Windows.Forms.MessageBox.Show(lsParamImports.Count.ToString());
            sb.AppendFormat("{0}\n", ESPACO);

            sb.AppendFormat("{0}public final class NuvemListagem\n", ESPACO);
            sb.AppendFormat("{0}{{\n", ESPACO);
            /*
            sb.AppendFormat("{0}{0}private static var _:{1} = new {1}();\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}public static function get instancia():{1}{{return _;}}\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}\n", ESPACO);
            */

            //sb.AppendFormat("{0}{0}private var nuvemCache:NuvemCache = null;\n", ESPACO, t.Name);
            //sb.AppendFormat("{0}{0}public function NuvemListagem(nuvemCache:NuvemCache){{ this.nuvemCache = nuvemCache; }}\n", ESPACO, t.Name);
            //sb.AppendFormat("{0}{0}public function NuvemListagem(){{ this.nuvemCache = nuvemCache; }}\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}\n", ESPACO);

            sb.AppendFormat("{0}{0}private var ro:MyRemoteObject = new MyRemoteObject('{1}.{2}');\n", ESPACO, t.Namespace, t.Name);
            sb.AppendFormat("{0}{0}private var TOTAL_INVOCACOES:Number = {1};\n", ESPACO, tiposEntidades.Count);
            sb.AppendFormat("{0}{0}private var a:App = null;\n", ESPACO);
            sb.AppendFormat("{0}{0}\n", ESPACO);

            /*
            //constantes
            foreach (Type tEnt in tiposEntidades)
            {
                sb.AppendFormat("{0}{0}public static const String_{1}:String = '{1}';\n", ESPACO, tEnt.Name);
            }
             * */
            //ctor
            sb.AppendFormat("{0}{0}public function Inicializa():void\n", ESPACO);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}this.a = App.single;\n", ESPACO);
            foreach (Type tEnt in tiposEntidades)
            {
                sb.AppendFormat("{0}{0}{0}this.ListaDB({1}.CLASSE);\n", ESPACO, tEnt.Name);
            }
            sb.AppendFormat("{0}{0}}}\n", ESPACO);

            sb.AppendFormat("{0}{0}public function ListaDB(classe:String, fRetorno:Function=null):void\n", ESPACO);
            
            #region { ... }
            sb.AppendFormat("{0}{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}if (a.cache['array'+classe]!=null && fRetorno!=null)\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}fRetorno(a.cache['array'+classe]);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}else\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{{\n", ESPACO);
            //invoca begin

            sb.AppendFormat("{0}{0}{0}{0}ro.Invoca('ListaDB', [a.idCorp, classe],\n", ESPACO);
            
            sb.AppendFormat("{0}{0}{0}{0}{0}function(retorno:Array):void\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}a.gerJan.CompletouDownloadUmaTabela(TOTAL_INVOCACOES);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}a.cache['array'+classe] = retorno;\n", ESPACO);

            sb.AppendFormat("{0}{0}{0}{0}{0}{0}if (fRetorno!=null)\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}{0}{0}fRetorno(a.cache['array'+classe]);\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}{0});\n", ESPACO);

            //invoca end
            sb.AppendFormat("{0}{0}{0}}}\n", ESPACO);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);
            #endregion

            sb.AppendFormat("{0}}}\n", ESPACO);
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
