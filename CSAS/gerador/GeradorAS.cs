using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

public class GeradorAS
{
    string ESPACO = "    ";

    //List<Type> entidades = new List<Type>();
    List<Type> parametros = new List<Type>();
    List<Type> outros = new List<Type>();
    List<Type> enumeradores = new List<Type>();
    List<Type> servicos = new List<Type>();
    /*
    public void addEntidade(List<Type> tipos)
    {
        foreach (Type t in tipos)
            entidades.Add(t);
    }*/
    public void addParametro(List<Type> tipos)
    {
        foreach (Type t in tipos)
            parametros.Add(t);
    }
    public void addOutro(List<Type> tipos)
    {
        foreach (Type t in tipos)
            outros.Add(t);
    }
    public void addEnum(List<Type> tipos)
    {
        foreach (Type t in tipos)
            enumeradores.Add(t);
    }
    public void addServico(List<Type> tipos)
    {
        foreach (Type t in tipos)
            servicos.Add(t);
    }








    public void gerar(string dir)
    {
        string nsEnt = "SDE.Entidade";
        string nsPar = parametros[0].Namespace;

        //gerarEntidadesParametros(dir, entidades);
        gerarEntidadesParametros(dir, parametros);
        gerarEntidadesParametros(dir, outros);
        gerarEnums(dir, enumeradores);
        gerarServicos(dir, servicos, nsEnt, nsPar);
        gerarServicosFachadas(dir, servicos, nsEnt, nsPar);
    }

    private void SalvaArquivo(string dirSaida, string fileName, StringBuilder sb)
    {
        string sFile = string.Format("{0}{1}.as", dirSaida, fileName);
        string sConteudo = sb.ToString();
        File.WriteAllText(sFile, sConteudo);
    }














    private void gerarEntidadesParametros(string dirRaiz, List<Type> tipos)
    {
        string ns = tipos[0].Namespace;
        string dir = string.Concat(dirRaiz, ns, '\\');
        dir = dir.Replace(".", "\\");
        if (Directory.Exists(dir))
            foreach (string arq in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                if (!arq.Contains(".svn"))
                    File.Delete(arq);
        else
            Directory.CreateDirectory(dir);

        foreach (Type t in tipos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("package {0}\n", ns);
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

            #region public function clone():Object
            /*
            sb.AppendFormat("{0}{0}public function clone():{1}", ESPACO, t.Name);
            sb.Append("{");
            sb.AppendFormat("var _clone:{0}=new {0}();", t.Name);
            sb.Append("for each(var campo:String in getCampos())");
            sb.Append("_clone[campo]=this[campo];");
            sb.Append("return _clone;");
            sb.Append("}\n\n");
            */

            sb.AppendFormat("{0}{0}public function clone():{1} {{ return new {1}(this); }}\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}public function {1}(obj:Object=null)\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}if (obj==null)return;\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}for each(var campo:String in getCampos())this[campo]=obj[campo];\n", ESPACO);
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

            SalvaArquivo(dir, t.Name, sb);
        }

    }

    private void gerarEnums(string dirRaiz, List<Type> tipos)
    {
        string ns = "SDE.Enumerador";
        string dir = string.Concat(dirRaiz, ns, '\\');
        dir = dir.Replace(".", "\\");
        if (Directory.Exists(dir))
            foreach (string arq in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                if (!arq.Contains(".svn"))
                    File.Delete(arq);
        else
            Directory.CreateDirectory(dir);

        foreach (Type t in tipos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("package {0}\n", ns);
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

            SalvaArquivo(dir, t.Name, sb);
        }
    }

    private void gerarServicos(string dirRaiz, List<Type> tipos, string nsEntidades, string nsParametros)
    {
        string ns = tipos[0].Namespace;
        string dir = string.Concat(dirRaiz, ns, '\\');
        dir = dir.Replace(".", "\\");
        if (Directory.Exists(dir))
            foreach (string arq in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                if (!arq.Contains(".svn"))
                    File.Delete(arq);
        else
            Directory.CreateDirectory(dir);

        foreach (Type t in tipos)
        {
            if (t.Name.StartsWith("Super"))
                continue;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("package {0}\n", ns);
            sb.AppendLine("{");

            sb.AppendFormat("{0}import mx.rpc.events.ResultEvent;\n", ESPACO);
            sb.AppendFormat("{0}import Core.Sessao;\n", ESPACO);
            sb.AppendFormat("{0}import Core.ConexaoServidor.MyRemoteObject;\n", ESPACO);

            List<String> lsParamImports = new List<String>();
            #region Registrando os Imports
            foreach (MethodInfo m in t.GetMethods())
            {
                foreach (ParameterInfo p in m.GetParameters())
                {
                    string fullN = string.Concat(p.ParameterType.Namespace,'.', p.ParameterType.Name);
                    if (
                        (
                            fullN.Contains(nsEntidades)
                            || fullN.Contains(nsParametros)
                        )
                        &&
                        (
                            !fullN.Contains("System.Collection")
                            && !fullN.Contains("[]")
                            && !lsParamImports.Contains(fullN)
                        )
                       )
                    lsParamImports.Add(fullN);
                    //pega tipos de parametros usados
                }
            }
            foreach (string sParam in lsParamImports)
                sb.AppendFormat("{0}import {1};\n", ESPACO, sParam);
            #endregion
            //System.Windows.Forms.MessageBox.Show(lsParamImports.Count.ToString());
            sb.AppendFormat("{0}\n", ESPACO);

            sb.AppendFormat("{0}public final class {1}\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{{\n", ESPACO);


            sb.AppendFormat("{0}{0}private static var _:{1};\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}public static function get unica():{1}\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}if (_==null) _=new {1}();\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}{0}{0}return _;\n", ESPACO, t.Name);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);

            sb.AppendFormat("{0}{0}private var ro:MyRemoteObject = new MyRemoteObject('{1}.{2}');\n", ESPACO, t.Namespace, t.Name);



            //cada método
            foreach (MethodInfo m in t.GetMethods())
            {
                if (m.DeclaringType != t)
                    continue;
                #region Comentários
                string tipoRetorno = m.ReturnType.Name;
                if (m.ReturnType.IsGenericType)
                {
                    Type[] tArgs = m.ReturnType.GetGenericArguments();
                    tipoRetorno = "IList<" + tArgs[0].Name + ">";
                }
                sb.AppendFormat("{0}{0}//ev.result{0}//CS={1}{0}//AS={2}\n", ESPACO, tipoRetorno, transformaTipo(m.ReturnType));
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
                    if (p.ParameterType.IsEnum)
                        tipo = "String";

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
                sb.AppendFormat("], fRetorno);\n", 1, 2);

                
                sb.AppendFormat("{0}{0}}}\n", ESPACO);
                #endregion
            }
            
            sb.AppendFormat("{0}}}\n", ESPACO);//end class
            sb.AppendLine("}");//end package

            SalvaArquivo(dir, t.Name, sb);
        }

    }

    private void gerarServicosFachadas(string dirRaiz, List<Type> tipos, string nsEntidades, string nsParametros)
    {
        string nsServico = tipos[0].Namespace;
        string ns = nsServico.Replace("Camada", "Fachada");
        //string ns = nsServico.Replace("Camada", "__ATUALIZADAS__Fachada");

        string dir = string.Concat(dirRaiz, ns, '\\');
        dir = dir.Replace(".", "\\");
        if (Directory.Exists(dir))
            foreach (string arq in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                if (!arq.Contains(".svn"))
                    File.Delete(arq);
        else
            Directory.CreateDirectory(dir);


        foreach (Type t in tipos)
        {
            if (t.Name.StartsWith("Super"))
                continue;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("package {0}\n", ns);
            sb.AppendLine("{");

            sb.AppendFormat("{0}import mx.rpc.events.ResultEvent;\n", ESPACO);

            List<String> lsParamImports = new List<String>();
            #region Registrando os Imports
            foreach (MethodInfo m in t.GetMethods())
            {
                foreach (ParameterInfo p in m.GetParameters())
                {
                    string fullN = string.Concat(p.ParameterType.Namespace, '.', p.ParameterType.Name);
                    if (
                        (
                            fullN.Contains(nsEntidades)
                            || fullN.Contains(nsParametros)
                        )
                        &&
                        (
                            !fullN.Contains("System.Collection")
                            && !fullN.Contains("[]")
                            && !lsParamImports.Contains(fullN)
                        )
                       )
                        lsParamImports.Add(fullN);
                    //pega tipos de parametros usados
                }
            }
            foreach (string sParam in lsParamImports)
                sb.AppendFormat("{0}import {1};\n", ESPACO, sParam);

            sb.AppendFormat("{0}import {1}.{2};\n", ESPACO, nsServico, t.Name);
            #endregion
            //System.Windows.Forms.MessageBox.Show(lsParamImports.Count.ToString());
            sb.AppendFormat("{0}\n", ESPACO);

            string nomeClasse = t.Name;
            nomeClasse = "Fcd"+ nomeClasse.Substring(1, nomeClasse.Length - 1);
            //System.Windows.Forms.MessageBox.Show(nomeClasse);

            sb.AppendFormat("{0}public final class {1}\n", ESPACO, nomeClasse);
            sb.AppendFormat("{0}{{\n", ESPACO);

            sb.AppendFormat("{0}{0}private static var _:{1};\n", ESPACO, nomeClasse);
            sb.AppendFormat("{0}{0}public static function get unica():{1}\n", ESPACO, nomeClasse);
            sb.AppendFormat("{0}{0}{{\n", ESPACO);
            sb.AppendFormat("{0}{0}{0}if (_==null) _=new {1}();\n", ESPACO, nomeClasse);
            sb.AppendFormat("{0}{0}{0}{0}return _;\n", ESPACO, nomeClasse);
            sb.AppendFormat("{0}{0}}}\n", ESPACO);



            //cada método
            foreach (MethodInfo m in t.GetMethods())
            {
                if (m.DeclaringType != t)
                    continue;
                #region Comentários
                string tipoRetorno = m.ReturnType.Name;
                if (m.ReturnType.IsGenericType)
                {
                    Type[] tArgs = m.ReturnType.GetGenericArguments();
                    tipoRetorno = "IList<" + tArgs[0].Name + ">";
                }
                sb.AppendFormat("{0}{0}//ev.result{0}//CS={1}{0}//AS={2}\n", ESPACO, tipoRetorno, transformaTipo(m.ReturnType));
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
                    if (p.ParameterType.IsEnum)
                        tipo = "String";
                    sb.AppendFormat("{0}:{1}, ", p.Name, tipo);

                }
                sb.AppendLine("fRetorno:Function=null):void");
                #endregion
                #region { ... }
                sb.AppendFormat("{0}{0}{{\n", ESPACO);
                //sb.AppendFormat("{0}{0}{0}var srv:{1} = new {1}();\n", ESPACO, t.Name);
                //sb.AppendFormat("{0}{0}{0}srv.{1}(", ESPACO, m.Name);
                sb.AppendFormat("{0}{0}{0}{1}.unica.{2}(", ESPACO, t.Name, m.Name);

                for (int i = 0; i < larguraParametros; i++)
                {
                    ParameterInfo p = m.GetParameters()[i];
                    string nomeParam = p.Name;
                    if (nomeParam == "idCorp" || nomeParam == "idEmp" || nomeParam == "idClienteFuncionarioLogado")
                        continue;
                    sb.AppendFormat("{0}", nomeParam);
                    if (i < larguraParametros/* - 1*/)
                        sb.Append(", ");
                }
                sb.AppendLine("fRetorno);");

                sb.AppendFormat("{0}{0}}}\n", ESPACO);
                #endregion
            }

            sb.AppendFormat("{0}}}\n", ESPACO);//end class
            sb.AppendLine("}");//end package

            SalvaArquivo(dir, nomeClasse, sb);
        }
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

}