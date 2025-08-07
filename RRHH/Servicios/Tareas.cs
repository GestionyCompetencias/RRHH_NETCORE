using RRHH.BaseDatos;

namespace RRHH.Servicios
{
    public class Tareas
    {
        Funciones f = new Funciones();

        public bool llenarTablasNuevaBD(string catalogo, string queryInsertEmpresa)
        {

            try
            {
                int i = 0;

                //  LLENAMOS LA TABLA PAISES
                string queryInsertPaises = "";
                i = 0;
                f.EjecutarConsultaSQL("select * from paises");
                foreach (var d in f.Tabla.Rows)
                {
                    var nombrePais = f.Tabla.Rows[i]["nombre"].ToString();
                    var monedaPais = f.Tabla.Rows[i]["moneda"].ToString();
                    var simbolPais = f.Tabla.Rows[i]["simbolo"].ToString();
                    queryInsertPaises += "insert into paises (nombre,moneda,simbolo) values ('" + nombrePais + "','" + monedaPais + "','" + simbolPais + "'); ";
                    i++;
                }

                f.EjecutarConsultaSQLCli(queryInsertPaises, catalogo);


                //LLENAMOS LA TABLA REGIONES
                string queryInsertRegiones = "";
                i = 0;
                f.EjecutarConsultaSQL("select * from regiones");
                foreach (var d in f.Tabla.Rows)
                {
                    var idPais = f.Tabla.Rows[i]["idpais"].ToString();
                    var nombre = f.Tabla.Rows[i]["nombre"].ToString();
                    queryInsertRegiones += "insert into regiones (idpais,nombre) values ('" + idPais + "','" + nombre + "'); ";
                    i++;
                }

                f.EjecutarConsultaSQLCli(queryInsertRegiones, catalogo);


                //LLENAMOS LA TABLA COMUNAS
                string queryInsertComunas = "";
                i = 0;
                f.EjecutarConsultaSQL("select * from comunas");
                foreach (var d in f.Tabla.Rows)
                {
                    var idregion = f.Tabla.Rows[i]["idregion"].ToString();
                    var nombre = f.Tabla.Rows[i]["nombre"].ToString();
                    queryInsertComunas += "insert into comunas (idregion,nombre) values ('" + idregion + "','" + nombre + "'); ";
                    i++;
                }

                f.EjecutarConsultaSQLCli(queryInsertComunas, catalogo);


                //LLENAMOS LA TABLA EMPRESA                
                f.EjecutarConsultaSQLCli(queryInsertEmpresa.Replace("!", ""), catalogo);


                //LLENAMOS LA TABLA MENUS          
                string queryInsertMenus = "";
                i = 0;
                f.EjecutarConsultaSQL("select * from opcionesMenu");
                foreach (var d in f.Tabla.Rows)
                {
                    var padre = f.Tabla.Rows[i]["padre"].ToString();
                    var obs = f.Tabla.Rows[i]["obs"].ToString();
                    var texto = f.Tabla.Rows[i]["texto"].ToString();
                    var url = f.Tabla.Rows[i]["url"].ToString();
                    var orden = f.Tabla.Rows[i]["orden"].ToString();
                    var habilitado = f.Tabla.Rows[i]["habilitado"].ToString();
                    queryInsertMenus += "insert into opcionesMenu (padre,obs,texto,url,orden,habilitado) values ('" + padre + "','" + obs + "','" + texto + "','" + url + "','" + orden + "','" + habilitado + "'); ";
                    i++;
                }

                f.EjecutarConsultaSQLCli(queryInsertMenus, catalogo);



                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public string obtenerRUTempresa(int idEmpresa)
        {
            string Ruc = "";

            f.EjecutarConsultaSQL("select rut from empresas where id=" + idEmpresa + "");
            if (f.Tabla.Rows.Count > 0)
            {
                Ruc = f.Tabla.Rows[0]["rut"].ToString();
            }
            return Ruc;
        }

    }
}
