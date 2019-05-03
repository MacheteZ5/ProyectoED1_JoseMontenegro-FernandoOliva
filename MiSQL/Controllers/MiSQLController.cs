using MiSQL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArbolBMas;
namespace MiSQL.Controllers
{
    public class MiSQLController : Controller
    {
        //arbol
        //int grado=3;
        //ArbolBMas.ArbolB arbol = new ArbolBMas.ArbolB();
        //ArbolBMas.Información info = new ArbolBMas.Información();
        //ArbolBMas.NodoB nodo;
        // GET: MiSQL
        public ActionResult Index()
        {
            return View(new List<Diccionario>());
        }
        private static T GetAnyValue<T>(string strKey)
        {
            object obj;
            T retType;

            dic.TryGetValue(strKey, out obj);
            try
            {
                retType = (T)obj;
            }
            catch
            {
                retType = default(T);
            }
            return retType;
        }
        static int conteo = 0;
        static Dictionary<string, object> dic = new Dictionary<string, object>();
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            if (dic.Count == 0)
            {
                int contador = 0;
                string filePath = string.Empty;
                if (postedFile != null)
                {
                    //dirección del archivo
                    string path = Server.MapPath("~/archivo/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    string csvData = System.IO.File.ReadAllText(filePath);

                    foreach (string row in csvData.Split('\r'))
                    {
                        if ((!string.IsNullOrEmpty(row)) && (contador != 0))
                        {
                            if (row != "\n")
                            {
                                Diccionario dicc = new Diccionario();
                                string extra = row.Split('\n')[0];
                                string clave = row.Split(';')[1];
                                string valor = row.Split(';')[1];
                                object valores = valor;
                                dic.Add(clave, valores);
                                dicc.clave = clave;
                                dicc.Valor = valor;
                            }
                        }
                        else
                        {
                            contador++;
                        }
                    }
                }
            }
            return View();
        }
        public ActionResult Menú()
        {
            if (conteo == 0)
            {
                return RedirectToAction("PrimeraConección");
            }
            else
            {
                return View();
            }
        }
        public ActionResult PrimeraConección()
        {
            return View();
        }
        public ActionResult CrearBaseDeDatos()
        {
            string sql = Request.Form["SQL"].ToString();
            char[] delimitador = { ' ' };
            string[] trozos = sql.Split(delimitador);
            if (trozos[0] == "")
            {
                for (int j = 0; j < trozos.Count(); j++)
                {
                    if (j != (trozos.Count() - 1))
                    {
                        trozos[j] = trozos[j + 1];
                    }
                    else
                    {
                        trozos[j] = "";
                    }
                }
            }
            int i = 0;
            if ((dic.ContainsValue(trozos[i])))
            {
                if (GetAnyValue<string>("CREATE") == trozos[i])
                {
                    if (trozos[i + 1] == "DATABASE")
                    {
                        conteo++;
                        return RedirectToAction("Conección");
                    }
                    else
                    {
                        return RedirectToAction("PrimeraConección");
                    }
                }
                else
                {
                    return RedirectToAction("PrimeraConección");
                }
            }
            else
            {
                return RedirectToAction("PrimeraConección");
            }

        }
        //creación de tabla
        public ActionResult Conección()
        {
            return View();
        }
        //static arbol[] arboles = new arbol[];
        static List<string> nombrestabla = new List<string>();
        static List<string> nombrecolumnas = new List<string>();
        static List<List<string>> listadelistadenombrescolumnas = new List<List<string>>();
        //
        static List<string> tiposdevalores = new List<string>();
        static List<List<Models.Información>> listadelistas = new List<List<Models.Información>>();
        public ActionResult CrearTabla()
        {
            //valorescantidad = new List<object>();
            nombrecolumnas = new List<string>();
            nombrestabla = new List<string>();
            string SQL = Request.Form["SQLs"].ToString();
            char[] delimitadores = { ' ', ',', '(', ')','\r','"','\n' };
            string[] separación = SQL.Split(delimitadores);
            string[] datos = new string[100];
            int i = 0;
            foreach (string linea in separación)
            {
                if (linea != "")
                {
                    datos[i] = linea;
                    i++;
                }
            }
            nombrestabla.Add(datos[2]);
            if (dic.ContainsValue(datos[0]))
            {
                if (GetAnyValue<string>("CREATE") == datos[0])
                {
                    if (datos[1] == "TABLE")
                    {
                        int m = 0;
                        string nombredelacolumna;
                        int conteostring = 0;
                        int conteoint = 0;
                        int conteoDatetime = 0;
                        bool cierto = false;
                        foreach (string linea in datos)
                        {
                            if (linea == "int" || linea == "string" || linea == "DateTime")
                            {
                                return RedirectToAction("Conección");
                            }
                            else
                            {
                                if (linea != null)
                                {
                                    if (linea == "VARCHAR")
                                    {
                                        nombrecolumnas.Add(datos[m - 1]);
                                        tiposdevalores.Add("VARCHAR");
                                        conteostring++;
                                    }
                                    else
                                    {
                                        if (linea == "INT")
                                        {
                                            nombrecolumnas.Add(datos[m - 1]);
                                            nombredelacolumna = datos[m - 1];
                                            conteoint++;
                                            tiposdevalores.Add("INT");
                                        }
                                        else
                                        {
                                            if (linea == "DATETIME")
                                            {
                                                nombrecolumnas.Add(datos[m - 1]);
                                                nombredelacolumna = datos[m - 1];
                                                conteoDatetime++;
                                                tiposdevalores.Add("DATETIME");
                                            }
                                        }
                                    }
                                    m++;
                                    if (linea == "GO")
                                    {
                                        cierto = true;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (conteoint == 0)
                        {
                            return RedirectToAction("Conección");
                        }
                        else
                        {
                            if (cierto == true)
                            {
                                return RedirectToAction("Conección");
                            }
                            else
                            {
                                listadelistadenombrescolumnas.Add(nombrecolumnas);
                                return RedirectToAction("Conección2");
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Conección");
                    }
                }
                else
                {
                    return RedirectToAction("Conección");
                }
            }
            else
            {
                return RedirectToAction("Conección");
            }
        }
        //Introducción de valores
        public ActionResult Conección2()
        {
            return View();
        }
        public ActionResult InserciónDeDatos()
        {
            string insertar = Request.Form["valores"].ToString();
            char[] delimitadores = { ' ', ',', '(', ')', '\r', '"', '\n'};
            string[] separación = insertar.Split(delimitadores);
            List<string> datos = new List<string>();
            //string[] datos = new string[100];
            int i = 0;
            foreach(string k in separación)
            {
                if (k != "")
                {
                    datos.Add(k);
                    i++;
                }
            }
            bool verdad = false;
            int j =0;
            foreach(string nombre in nombrestabla)
            {
                if(nombre == datos[2])
                {
                    verdad = true;
                    break;
                }
                j++;
            }
            if(verdad == true)
            {
                int s = 0;
                int contador = 0;
                if (listadelistas.Count == 0)
                {
                    foreach(string d in datos)
                    {
                        if (s != nombrecolumnas.Count())
                        {
                            if (nombrecolumnas[s] == d)
                            {
                                s++;
                                contador++;
                            }
                        }
                    }
                    if (contador == nombrecolumnas.Count())
                    {
                        Models.Información info = new Models.Información();
                        int cantidad=0;
                        int cantidad2=0;
                        int cantidad3=0;
                        foreach(string lineas in tiposdevalores)
                        {
                            if(lineas == "INT")
                            {
                                cantidad++;
                            }
                            else
                            {
                                if (lineas == "DATETIME")
                                {
                                    cantidad2++;
                                }
                                else
                                {
                                    if (lineas == "VARCHAR")
                                    {
                                        cantidad3++;
                                    }
                                }
                            }
                        }
                        
                        string[] valor = new string[10];
                        int q = 0;
                        while (datos[q] != "VALUES")
                        {
                            q++;
                        }
                        int y = q+1;
                        for(int ñ = 0; ñ < datos.Count(); ñ++)
                        {
                            if (y != datos.Count())
                            {
                                if (datos[y] != null)
                                {
                                    valor[ñ] = datos[y];
                                    y++;
                                }
                            }
                        }
                        int r = 0;
                        int rastreoint = 0;
                        int rastreovarchar = 0;
                        int rastreodatetime = 0;
                        foreach(string l in tiposdevalores)
                        {
                            if(l == "INT")
                            {
                                if (cantidad == 1)
                                {
                                    info.num =Convert.ToInt32(valor[r]);
                                    r++;
                                }
                                else
                                {
                                    if (cantidad == 2)
                                    {
                                        if (rastreoint == 0)
                                        {
                                           info.num = Convert.ToInt32(valor[r]);
                                            r++;
                                            rastreoint++;
                                        }
                                        else
                                        {
                                            if (rastreoint == 1)
                                            {
                                                info.num2=Convert.ToInt32(valor[r]);
                                                r++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (cantidad == 3)
                                        {
                                            if (rastreoint == 0)
                                            {
                                                info.num = Convert.ToInt32(valor[r]);
                                                rastreoint++;
                                                r++;
                                            }
                                            else
                                            {
                                                if (rastreoint == 1)
                                                {
                                                    info.num2 = Convert.ToInt32(valor[r]);
                                                    rastreoint++;
                                                    r++;
                                                }
                                                else
                                                {
                                                    if (rastreoint == 2)
                                                    {
                                                        info.num3=Convert.ToInt32(valor[r]);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (l == "DATETIME")
                                {
                                    if (cantidad2 == 1)
                                    {
                                        info.tiempo = Convert.ToDateTime(valor[r]);
                                        r++;
                                    }
                                    else
                                    {
                                        if (cantidad2 == 2)
                                        {
                                            if (rastreodatetime == 0)
                                            {
                                                info.tiempo = Convert.ToDateTime(valor[r]);
                                                rastreodatetime++;
                                                r++;
                                            }
                                            else
                                            {
                                                if (rastreodatetime == 1)
                                                {
                                                    info.tiempo2 = Convert.ToDateTime(valor[r]);
                                                    r++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (cantidad2 == 3)
                                            {
                                                if (rastreodatetime == 0)
                                                {
                                                    info.tiempo = Convert.ToDateTime(valor[r]);
                                                    rastreodatetime++;
                                                    r++;
                                                }
                                                else
                                                {
                                                    if (rastreodatetime == 1)
                                                    {
                                                        info.tiempo2 = Convert.ToDateTime(valor[r]);
                                                        rastreodatetime++;
                                                        r++;
                                                    }
                                                    else
                                                    {
                                                        if (rastreodatetime == 2)
                                                        {
                                                            info.tiempo3 = Convert.ToDateTime(valor[r]);
                                                            r++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if(l=="VARCHAR")
                                    {
                                        if (cantidad3 == 1)
                                        {
                                            info.varchar = Convert.ToString(valor[r]);
                                            r++;
                                        }
                                        else
                                        {
                                            if (cantidad3 == 2)
                                            {
                                                if (rastreovarchar == 0)
                                                {
                                                    info.varchar = Convert.ToString(valor[r]);
                                                    rastreovarchar++;
                                                    r++;
                                                }
                                                else
                                                {
                                                    if (rastreovarchar == 1)
                                                    {
                                                        info.varchar2 = Convert.ToString(valor[r]);
                                                        r++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (cantidad3 == 3)
                                                {
                                                    if (rastreovarchar == 0)
                                                    {
                                                        info.varchar = Convert.ToString(valor[r]);
                                                        rastreovarchar++;
                                                        r++;
                                                    }
                                                    else
                                                    {
                                                        if (rastreovarchar == 1)
                                                        {
                                                            info.varchar2 = Convert.ToString(valor[r]);
                                                            rastreovarchar++;
                                                            r++;
                                                        }
                                                        else
                                                        {
                                                            if (rastreovarchar == 2)
                                                            {
                                                                info.varchar3 = Convert.ToString(valor[r]);
                                                                r++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }  
                            }
                        }
                        List<Models.Información> aux = new List<Models.Información>();
                        aux.Add(info);
                        listadelistas.Add(aux);
                        return View(aux);
                    }
                }
                else
                {
                    int recorrido = 0;
                    foreach(string nombre in nombrestabla)
                    {
                        if (datos[2] == nombre)
                        {
                            break;
                        }
                        recorrido++;
                    }
                    int cont = 0;
                    List<Models.Información> lista = new List<Models.Información>();
                    foreach(List<Models.Información> extra in listadelistas)
                    {
                        if(cont == recorrido)
                        {
                            lista = extra;
                            break;
                        }
                        cont++;
                    }
                    int numeros = 0;
                    int busqueda = lista.Count();
                    int n = lista[0].num;
                    int n2 = lista[0].num2;
                    int n3 = lista[0].num3;
                    DateTime d = lista[0].tiempo;
                    DateTime d2 = lista[0].tiempo2;
                    DateTime d3 = lista[0].tiempo3;
                    string st = lista[0].varchar;
                    string st2 = lista[0].varchar2;
                    string st3 = lista[0].varchar3;
                    DateTime tiemp = new DateTime();
                    if(d != tiemp)
                    {
                        numeros++;
                    }
                    if (d2 != tiemp)
                    {
                        numeros++;
                    }
                    if (d3 != tiemp)
                    {
                        numeros++;
                    }
                    if (n != 0)
                    {
                        numeros++;
                    }
                    if (n2 != 0)
                    {
                        numeros++;
                    }
                    if (n3 != 0)
                    {
                        numeros++;
                    }
                    if (st != null)
                    {
                        numeros++;
                    }
                    if (st2 != null)
                    {
                        numeros++;
                    }
                    if (st3 != null)
                    {
                        numeros++;
                    }
                    int x=0;
                }
            }
            else
            {
                return RedirectToAction("Conección");
            }

            return View();
        }
        //cambio de valor
        public ActionResult cambiodevalor()
        {
            return View();
        }
        private string ruta = AppDomain.CurrentDomain.BaseDirectory + "Archivos A Utilizar\\csv\\Actual.csv";
        public ActionResult valor()
        {
            string clave = Request.Form["clave"].ToString();
            string valor = Request.Form["valor"].ToString();
            char[] delimitadores = { ' ', '"' };
            string[] trozo = valor.Split(delimitadores);
            if (trozo[0] == "")
            {
                for (int j = 0; j < trozo.Count(); j++)
                {
                    if (j != (trozo.Count() - 1))
                    {
                        trozo[j] = trozo[j + 1];
                    }
                    else
                    {
                        trozo[j] = "";
                    }
                }
            }
            if (dic.ContainsKey(clave))
            {
                dic.Remove(clave);
                dic[clave] = trozo[0];

                StreamWriter writer = new StreamWriter(ruta);
                string contenido = null;

                foreach (KeyValuePair<string, object> k in dic)
                {
                    if (k.Key != null)
                    {
                        contenido = string.Format("{0},{1}", k.Key, k.Value);
                        writer.WriteLine(contenido);
                    }
                }
                writer.Close();
                return RedirectToAction("Menú");
            }
            else
            {
                char[] delimitador = { ' ', '"' };
                string[] trozos = clave.Split(delimitador);
                if (trozos.Count() == 2)
                {
                    dic.Remove(trozos[1]);
                    dic[clave] = trozo[0];
                    StreamWriter writer = new StreamWriter(ruta);
                    string contenido = null;
                    foreach (KeyValuePair<string, object> k in dic)
                    {
                        if (k.Key != null)
                        {
                            contenido = string.Format("{0},{1}", k.Key, k.Value);
                            writer.WriteLine(contenido);
                        }
                    }
                    writer.Close();
                    return RedirectToAction("Menú");
                }
                else
                {
                    if ((trozos.Count() == 3))
                    {
                        dic.Remove(trozos[1] + " " + trozos[2]);
                        dic[trozos[1] + " " + trozos[2]] = trozo[0];
                        StreamWriter writer = new StreamWriter(ruta);
                        string contenido = null;
                        foreach (KeyValuePair<string, object> k in dic)
                        {
                            if (k.Key != null)
                            {
                                contenido = string.Format("{0},{1}", k.Key, k.Value);
                                writer.WriteLine(contenido);
                            }
                        }
                        writer.Close();
                        return RedirectToAction("Menú");
                    }
                    else
                    {
                        return RedirectToAction("cambiodevalor");
                    }
                }
            }
        }
    }
}