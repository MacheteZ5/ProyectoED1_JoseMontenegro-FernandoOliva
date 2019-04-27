using MiSQL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiSQL.Controllers
{
    public class MiSQLController : Controller
    {
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
        static List<object> valores = new List<object>();
        public ActionResult CrearTabla()
        {
            valores = new List<object>();
            string SQL = Request.Form["SQLs"].ToString();
            char[] delimitadores = { ' ', ',', '(', ')' };
            string[] separación = SQL.Split(delimitadores);
            string[] datos = new string[100];
            int i = 0;
            if (separación[0] == "")
            {
                for (int j = 0; j < separación.Count(); j++)
                {
                    if (j != (separación.Count() - 1))
                    {
                        if (separación[j] != "")
                        {
                            datos[i] = separación[j];
                            i++;
                        }
                    }
                    else
                    {
                        separación[j] = "";
                    }
                }
            }
            else
            {
                foreach (string linea in separación)
                {
                    if (linea != "")
                    {
                        datos[i] = linea;
                        i++;
                    }
                }
            }
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
                            if (linea == "VARCHAR")
                            {
                                if (valores.Contains(conteostring))
                                {
                                    valores.Remove("VARCHAR");
                                    valores.Remove(conteostring);
                                }
                                conteostring++;
                                valores.Add("VARCHAR");
                                valores.Add(conteostring);
                            }
                            else
                            {
                                if (linea == "INT")
                                {
                                    nombredelacolumna = datos[m - 1];
                                    if (valores.Contains(conteoint))
                                    {
                                        valores.Remove("INT");
                                        valores.Remove(conteoint);
                                    }
                                    conteoint++;
                                    valores.Add("INT");
                                    valores.Add(conteoint);
                                }
                                else
                                {
                                    if (linea == "DATETIME")
                                    {
                                        nombredelacolumna = datos[m - 1];
                                        if (valores.Contains(conteoDatetime))
                                        {
                                            valores.Remove("DATETIME");
                                            valores.Remove(conteoDatetime);
                                        }
                                        conteoDatetime++;
                                        valores.Add("DATETIME");
                                        valores.Add(conteoDatetime);
                                    }
                                }
                            }
                            if (linea == "int" || linea == "string" || linea == "DateTime")
                            {
                                return RedirectToAction("Conección");
                            }
                            m++;
                            if (linea == "GO")
                            {
                                cierto = true;
                            }
                        }
                        if (cierto == true)
                        {
                            valores = null;
                            return RedirectToAction("Conección");
                        }
                        else
                        {
                            return RedirectToAction("Conección2");
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