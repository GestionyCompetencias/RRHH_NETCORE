using Microsoft.AspNetCore.Mvc;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RRHH.Servicios.ModificaMarcas
{
    /// <summary>
    /// Servicio para gestionar las marcas del trabajador.
    /// </summary>
    public interface IModificaMarcasService
    {
        /// <summary>
        /// Genera listado de marcas del trabajador.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="desde">desde fecha </param>
        /// <param name="hasta">hasta fecha</param>
        /// <returns>Lista marcas del periodo</returns>
        public List<DetMarcasTrabajador> ListarMarcasTrabajadorService(int idEmpresa,  string desde, string hasta,string rut);

        /// <summary>
        /// Consulta una marca especificada por su id.
        /// </summary>
        /// <param name="id">ID de haber informado</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Muestra informacion de marca</returns>
        public DetMarcasTrabajador ConsultaMarcaTrabajadorIdService(string id, int idEmpresa);

        /// <summary>
        /// Modifica marca del trabajador.
        /// </summary>
        /// <param name="opciones">Registro de marca diaria</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <param name="usuario">Usuario logeado.</param>
        /// <returns>Inhabilita asistencia informada</returns>
        public Resultado ModificaMarcaTrabajadorService(DetMarcasTrabajador opciones, int idEmpresa, string usuario);

        /// <summary>
        /// Inhabilitar asistencia.
        /// </summary>
        /// <param name="id">ID de asistencia informada</param>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Inhabilita asistencia informada</returns>
        public Resultado InhabilitaMarcaTrabajadorService(MarcacionDeleteVM opciones, int idEmpresa);


        /// <summary>
        /// Carga lista de trabajadores.
        /// </summary>
        /// <param name="idEmpresa">ID de una empresa.</param>
        /// <returns>Lista de trabajadores</returns>
        public Resultado ComboTrabajadoresService(int idEmpresa);
 

    }

    public class ModificaMarcasService : IModificaMarcasService
    {
        private readonly IDatabaseManager _databaseManager;
		private static Funciones f = new Funciones();
		private Correos correos = new Correos();
		private Generales Generales = new Generales();
		private Seguridad seguridad = new Seguridad();

		public ModificaMarcasService(IDatabaseManager databaseManager)
        {

            _databaseManager = databaseManager;
        }

        public List<DetMarcasTrabajador> ListarMarcasTrabajadorService(int empresa,string desde,string hasta, string rut)
        {
            UsuarioVM perfiles = new UsuarioVM();
            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;
                DateTime fecini = Convert.ToDateTime(desde);
                DateTime fecfin = Convert.ToDateTime(hasta);
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");


                f.EjecutarConsultaSQLCli("SELECT marcacion.RUT,marcacion.CHECKTIME,marcacion.CHECKTYPE,marcacion.SENSORID, marcacion.DIA_SEMANA " +
                                            "FROM MARCACION " +
                                            "WHERE marcacion.CHECKTIME <= '"+fecfinstr+"' and marcacion.CHECKTIME >= '"+fecinistr+
                                            "' and marcacion.Rut = '"+rut+"' ", BD_Cli);


                List<MarcacionBaseVM> Lista = new List<MarcacionBaseVM>();
                    Lista = (from DataRow dr in f.Tabla.Rows
                                    select new MarcacionBaseVM()
                                    {
                                        rutTrabajador = dr["RUT"].ToString(),
                                        marca = DateTime.Parse(dr["CHECKTIME"].ToString()),
                                        tipoMarca = dr["CHECKTYPE"].ToString(),
                                        diaSemana = int.Parse(dr["DIA_SEMANA"].ToString())
                                    }).ToList();
                    List<DetMarcasTrabajador> detalle = new List<DetMarcasTrabajador>();
                    DateTime marca;
                    DateTime fecproc = fecini.Date;
                    DateTime fecfinal = fecfin.Date;
                    while( fecproc <= fecfinal)
                    {
                        List<MarcacionBaseVM> dia = new List<MarcacionBaseVM>();
                        dia = Lista.Where(x => x.marca >= fecproc && x.marca < fecproc.AddDays(1)).ToList();
                        DetMarcasTrabajador det = new DetMarcasTrabajador();
                        
                        det.ruttrabajador = rut;
                        det.fecha = fecproc.ToString("dd'-'MM'-'yyyy");
                        det.diasemana = f.SemanaLetras(fecproc.DayOfWeek.ToString());

                        if (dia.Count> 0)
                        {
                            foreach (var r in dia)
                            {
                                marca = r.marca;
                                if (r.tipoMarca == "I")
                                {
                                    det.entrada = marca.ToString("HH':'mm");
                                    det.checkent = marca.ToString("HH':'mm");
                                }
                                else
                                {
                                    det.salida = marca.ToString("HH':'mm");
                                    det.checksal = marca.ToString("HH':'mm");
                                }

                            }
                        }
                        else
                        {
                            det.salida = "00:00";
                            det.checksal = "00:00";
                            det.entrada = "00:00";
                            det.checkent = "00:00";
                        }
                        if(det.entrada == null)
                        {
                            det.entrada = "00:00";
                            det.checkent = "00:00";

                        }
                        if(det.salida == null)
                        {
                            det.salida = "00:00";
                            det.checksal = "00:00";
                        }
                        detalle.Add(det);
                        fecproc = fecproc.AddDays(1);
                    }
                    return detalle;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar marcas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar consultar marcas", correos.destinatarioErrores);
                return null;
            }

        }
        public DetMarcasTrabajador ConsultaMarcaTrabajadorIdService(string id, int empresa)
        {

            try
            {

                string RutEmpresa = f.obtenerRUT(empresa);
                var BD_Cli = "remuneracion_" + RutEmpresa;

                string[] idsSeparados = id.Split("*");
                string rut = idsSeparados[0];
                string fec = idsSeparados[1];
                DateTime fecini = Convert.ToDateTime(fec);
                DateTime fecfin = fecini.AddDays(1) ;
                string fecinistr = fecini.ToString("yyyy'-'MM'-'dd");
                string fecfinstr = fecfin.ToString("yyyy'-'MM'-'dd");



                f.EjecutarConsultaSQLCli("SELECT marcacion.RUT,marcacion.CHECKTIME,marcacion.CHECKTYPE,marcacion.SENSORID, marcacion.DIA_SEMANA " +
                                            "FROM MARCACION " +
                                            "WHERE marcacion.CHECKTIME <= '" + fecfinstr + "' and marcacion.CHECKTIME >= '" + fecinistr +
                                            "' and marcacion.Rut = '" + rut + "' ", BD_Cli);


                List<MarcacionBaseVM> Lista = new List<MarcacionBaseVM>();

                    Lista = (from DataRow dr in f.Tabla.Rows
                             select new MarcacionBaseVM()
                             {
                                 rutTrabajador = dr["RUT"].ToString(),
                                 marca = DateTime.Parse(dr["CHECKTIME"].ToString()),
                                 tipoMarca = dr["CHECKTYPE"].ToString(),
                                 diaSemana = int.Parse(dr["DIA_SEMANA"].ToString())
                             }).ToList();
                    DateTime marca;
                    DetMarcasTrabajador det = new DetMarcasTrabajador();
                    det.ruttrabajador = rut;
                    det.fecha = fecini.ToString("yyyy'-'MM'-'dd");
                    det.diasemana = fecini.DayOfWeek.ToString();
                    det.entrada = null;
                    det.checkent = null;
                    det.salida = null;
                    det.checksal = null;
                    if (Lista.Count > 0)
                    {
                        foreach (var r in Lista)
                        {
                            marca = r.marca;
                            if (r.tipoMarca == "I")
                            {
                                det.entrada = marca.ToString("HH':'mm");
                                det.checkent = marca.ToString("HH':'mm");
                            }
                            else
                            {
                                det.salida = marca.ToString("HH':'mm");
                                det.checksal = marca.ToString("HH':'mm");
                            }
                        }
                    }
                    return det;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al Consultar marcas";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();
                correos.SendEmail(Mensaje, Asunto, "Se generó un error al consultar marcas", correos.destinatarioErrores);
                return null;
            }
        }



        public Resultado InhabilitaMarcaTrabajadorService(MarcacionDeleteVM opciones, int empresa)
        {

            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            try
            {
                if (opciones.Id == null)
                {
                    resultado.result = 0;
                    resultado.mensaje = "Debe indicar la informacion de la asistencia que desea eliminar";
                    return resultado;
                }

                string query = "update marcacion set habilitado=0 where id=" + opciones.Id + "  ! ";

                if (f.EjecutarQuerySQLCli(query, BD_Cli))
                {
                    resultado.result = 1;
                    resultado.mensaje = "Asistencia eliminada de manera exitosa.";
                }
                else
                {
                    resultado.result = 0;
                    resultado.mensaje = "No se eliminó la información de asistencia.";
                }

                return resultado;
            }
            catch (System.Exception ex)
            {
                var Asunto = "Error al eliminar asistencia";
                var Mensaje = ex.Message.ToString() + "<br><hr><br>" + ex.StackTrace.ToString();

                correos.SendEmail(Mensaje, Asunto, "Se generó un error al intentar eliminarasistencia en el sistema", correos.destinatarioErrores);

                resultado.result = -1;
                resultado.mensaje = "Fallo al intentar eliminar una asistencia" + ex.Message.ToString();
                return resultado;
            }
        }
        public Resultado ComboTrabajadoresService(int empresa)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;

            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;

            f.EjecutarConsultaSQLCli("SELECT personas.rut,personas.nombres,personas.apellidos " +
                                        "FROM personas " , BD_Cli);

            List<PersonasBaseVM> opcionesList = new List<PersonasBaseVM>();
            if (f.Tabla.Rows.Count > 0)
            {

                opcionesList = (from DataRow dr in f.Tabla.Rows
                                select new PersonasBaseVM()
                                {
                                    Rut = dr["Rut"].ToString(),
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString()
                                }).ToList();
            }
            var data = new List<Conceptos>();
            foreach (var h in opcionesList)
            {
                data.Add(new Conceptos() { Codigo = h.Rut, Descripcion = h.Nombres+" "+h.Apellidos });
            }
            resultado.result = 1;
            resultado.data = data;
            return resultado;
        }
        public Resultado ModificaMarcaTrabajadorService(DetMarcasTrabajador regis,int empresa, string idusuario)
        {
            string RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            int id = Convert.ToInt32(idusuario);
            UsuarioVM usuario = Generales.BuscaUsuario(id);

                Resultado resultado = new Resultado();
                resultado.result = 0;
                try
                {
                    if (regis.ruttrabajador == null)
                    {
                        resultado.result = 0;
                        resultado.mensaje = "Debe indicar trabajador";
                        return resultado ;
                    }
                    DateTime marcaent = Convert.ToDateTime(regis.fecha + " " + regis.entrada);
                    DateTime marcasal = Convert.ToDateTime(regis.fecha + " " + regis.salida);
                    DateTime origent = Convert.ToDateTime(regis.checkent);
                    DateTime origsal = Convert.ToDateTime(regis.checksal);

                    //int dias = DateTime.Now.Date.Subtract(marcaent).Days;
                    //if (dias > 60)
                    //{
                    //resultado.mensaje = "Marcas muy antiguas";
                    //return resultado;
                    //}
                    //else
                    //{
                    //    if (dias < 0)
                    //    {
                    //    resultado.mensaje = "No existen marcas futuras";
                    //    return resultado;
                    //    }
                    //}

                    if (regis.checkent == null && regis.checksal == null)
                    {
                        resultado =MarcaManual(regis,usuario.usuraio, BD_Cli);
                        return resultado ;
                    }
                    else
                    {
                        //if (regis.checkent == null)
                        //{
                        //    EventoPareado(regis.ruttrabajador, marcaent, origsal);
                        //}
                        //else
                        //{
                        //    EventoPareado(regis.ruttrabajador, marcasal, origent);
                        //}
                    }

                    resultado.result = 1;
                    resultado.mensaje = "Asistencia actualizada de manera exitosa.";
                }
                catch (Exception eG)
                {
                    var Mensaje = eG.Message.ToString() + "<br><hr><br>" + eG.StackTrace.ToString();
                    resultado.result = -1;
                    resultado.mensaje = "Fallo al intentar guardar asistencia" + eG.Message.ToString();
                }
                return resultado;
            }
        public Resultado MarcaManual(DetMarcasTrabajador marca, string usuario,string BD_Cli)
        {
            Resultado resultado = new Resultado();
            resultado.result = 0;
            resultado.mensaje = "Ocurrio un error";
            try
            {
                DateTime sig = DateTime.Now.Date;
                sig = sig.AddDays(1);
                DateTime fec = Convert.ToDateTime(marca.fecha);
                int fec1 = fec.Year * 10000 + fec.Month * 100 + fec.Day;
                int fec2 = sig.Year * 10000 + sig.Month * 100 + sig.Day;


                DateTime MarcaEntrada = Convert.ToDateTime(marca.fecha + " " + marca.entrada);
                DateTime MarcaCheckent = Convert.ToDateTime(marca.fecha + " 00:00:00");
                DateTime MarcaSalida  = Convert.ToDateTime(marca.fecha + " " + marca.salida);
                DateTime MarcaChecksal = Convert.ToDateTime(marca.fecha + " 00:00:00");
                DateTime hoy = DateTime.Now;
                string entrada = MarcaEntrada.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                string checkent = MarcaCheckent.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                string salida = MarcaSalida.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                string checksal = MarcaChecksal.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                string hoystr = hoy.ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                int VRenid = 0;
                int VRenSal = 0;

                // Correo
                PersonasBaseVM  per = Generales.BuscaPersona(marca.ruttrabajador, BD_Cli);
                string VMailTrabajador = per.Email;
                string VTipo = "I";
                string Vtoken = Guid.NewGuid().ToString();
                string vfullname = per.Apellidos + " " +per.Nombres;


                // Marca de entrada
                string query2 = "insert into logMarcaciones (rutTrabajador,marcaOriginal,tipoMarcaOriginal,MarcaNueva,tipoMarcaNueva,usuario,fechaModificacion,accion) " +
               "values " +
               "('" + marca.ruttrabajador+ "','" + checkent + "','I','" + entrada + "','I', '" + usuario +
               "', '" + hoystr + "', 'ESPERANUE') ! ";
                if (f.EjecutarQuerySQLCli(query2, BD_Cli))
                {
                    int idlog = 0;
                    f.EjecutarConsultaSQLCli("SELECT * from logMarcaciones " +
                                    "where ruttrabajador = '" + marca.ruttrabajador + "' and fechaModificacion = '" + hoystr + 
                                    "' and tipoMarcaOriginal = 'I' ", BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var tablog = (from DataRow dr in f.Tabla.Rows
                                      select new
                                      {
                                          Id = int.Parse(dr["id"].ToString()),
                                      }).ToList();
                        var buslog = tablog.LastOrDefault();
                        idlog = buslog.Id;
                    }
                    string query3 = "insert into logAceptaTiempo (id,estado,accion,fechaCambio,correo,token) " +
                                    "values " +
                                    "(" + idlog + ",'ESPERA','I','" + VTipo + "', '" + hoystr + "', '" + VMailTrabajador + "', '" + Vtoken + "') ! ";
                    if (f.EjecutarQuerySQLCli(query3, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Log de marcación ingresado de manera exitosa";
                    }
                }


                //               Marca de salida
                string query4 = "insert into logMarcaciones (rutTrabajador,marcaOriginal,tipoMarcaOriginal,MarcaNueva,tipoMarcaNueva,usuario,fechaModificacion,accion) " +
                                "values " +
                                "('" + marca.ruttrabajador + "','" + checksal + "','O','" + salida + "','O', '" + usuario +
                                "', '" + hoystr + "', 'ESPERANUE') ! ";
                if (f.EjecutarQuerySQLCli(query4, BD_Cli))
                {
                    int idlog = 0;
                    f.EjecutarConsultaSQLCli("SELECT * from logMarcaciones " +
                                    "where ruttrabajador = '" + marca.ruttrabajador + "' and fechaModificacion = '" + hoystr +
                                    "' and tipoMarcacionOriginal = 'O' ", BD_Cli);
                    if (f.Tabla.Rows.Count > 0)
                    {
                        var tablog = (from DataRow dr in f.Tabla.Rows
                                      select new
                                      {
                                          Id = int.Parse(dr["id"].ToString()),
                                      }).ToList();
                        var buslog = tablog.LastOrDefault();
                        idlog = buslog.Id;
                    }
                    string query5 = "insert into logAceptaTiempo (id,estado,accion,fechaCambio,correo,token) " +
                            "values " +
                            "(" + idlog + ",'ESPERA','O','" + VTipo + "', '" + DateTime.Now + "', '" + VMailTrabajador + "', '" + Vtoken + "') ! ";
                    if (f.EjecutarQuerySQLCli(query5, BD_Cli))
                    {
                        resultado.result = 1;
                        resultado.mensaje = "Log de marcación ingresado de manera exitosa";
                    }
                }


                Notifica_modificacion notifica_Modificacion = new Notifica_modificacion();
                notifica_Modificacion.Notifica_trabajador(" ", " ", marca.ruttrabajador, MarcaEntrada, MarcaSalida, VMailTrabajador, VTipo, Vtoken, VRenid, VRenSal, vfullname);
                resultado.result = 1;
                resultado.mensaje = "Marcación registrada exitosamente";

                return resultado;
            }
            catch (Exception e2)
            {
                resultado.result = -1;
                resultado.mensaje = "No se pudo finalizar el proceso";
                return resultado;
            }

        }
        //public virtual JsonResult EventoPareado(string RUT, DateTime PAREADO, DateTime MARCAORIGINAL)
        //{
        //        string Tipo = string.Empty;
        //        string empresa = System.Web.HttpContext.Current.Session["sessionEmpresa"] as String;
        //        string usuario = System.Web.HttpContext.Current.Session["sessionUsuario"] as String;

        //        var LeeCorreo = db.PERSONA.Where(x => x.RUT == RUT).Select(x => new { x.CORREO, x.APATERNO, x.AMATERNO, x.NOMBRE }).First();

        //        string VMailTrabajador = LeeCorreo.CORREO;
        //        string VTipo = "I";
        //        string Vtoken = Guid.NewGuid().ToString();
        //        string vfullname = LeeCorreo.APATERNO + " " + LeeCorreo.AMATERNO + "," + LeeCorreo.NOMBRE;
        //        int VRenid = 0;
        //        int VRenSal = 0;
        //        String VPareado = " ";

        //        // DateTime MarcaEntrada = Convert.ToDateTime(Fecha + " " + HEntrada);
        //        // DateTime MarcaSalida = Convert.ToDateTime(Fecha + " " + HSalida);

        //        DateTime MarcaSalida = PAREADO;
        //        DateTime MarcaEntrada = PAREADO;
        //        DateTime MarcaOri = MARCAORIGINAL;



        //        if (MARCAORIGINAL < PAREADO)
        //        {
        //            Tipo = "O";
        //            VTipo = "S";
        //            LOG_MARCACIONES MarcaOriginalSalida = new LOG_MARCACIONES
        //            {
        //                EMPRESA = empresa,
        //                USUARIO = usuario,
        //                TRABAJADOR = RUT,
        //                FECHA_MOD = DateTime.Now,
        //                MARCA_ORIGINAL = MarcaSalida,
        //                TIPO_MARCA_ORIGINAL = "O",
        //                MARCA_NUEVA = MarcaSalida,
        //                TIPO_MARCA_NUEVA = "O",
        //                ACCION = "ESPERANUE"
        //            };
        //            db.LOG_MARCACIONES.Add(MarcaOriginalSalida);
        //            db.SaveChanges();

        //            VRenSal = MarcaOriginalSalida.ID;

        //            LOG_ACEPTATIEMPO MarcaNuevaSal = new LOG_ACEPTATIEMPO
        //            {
        //                LID = VRenSal,
        //                LESTADO = "ESPERA",
        //                LACCION = Tipo,
        //                LFECHACAMBIO = DateTime.Now,
        //                LCORREO = VMailTrabajador,
        //                LTOKEN = Vtoken,
        //            };

        //            db.LOG_ACEPTATIEMPO.Add(MarcaNuevaSal);
        //            db.SaveChanges();

        //        }
        //        else
        //        {

        //            /*
        //             * Re Escribir Evento Pareado existente
        //             */
        //            VPareado = "P";
        //            LOG_MARCACIONES MarcaOriginalori = new LOG_MARCACIONES
        //            {
        //                EMPRESA = empresa,
        //                USUARIO = usuario,
        //                TRABAJADOR = RUT,
        //                FECHA_MOD = DateTime.Now,
        //                MARCA_ORIGINAL = MarcaOri,
        //                TIPO_MARCA_ORIGINAL = "I",
        //                MARCA_NUEVA = MarcaOri,
        //                TIPO_MARCA_NUEVA = "O",
        //                ACCION = "ESPERAMOD"

        //            };

        //            db.LOG_MARCACIONES.Add(MarcaOriginalori);
        //            db.SaveChanges();
        //            VRenSal = MarcaOriginalori.ID;

        //            LOG_ACEPTATIEMPO MarcaTiempoori = new LOG_ACEPTATIEMPO
        //            {
        //                LID = VRenSal,
        //                LESTADO = "ESPERA",
        //                LACCION = "M",
        //                LFECHACAMBIO = DateTime.Now,
        //                LCORREO = VMailTrabajador,
        //                LTOKEN = Vtoken,
        //            };

        //            db.LOG_ACEPTATIEMPO.Add(MarcaTiempoori);
        //            db.SaveChanges();

        //            /* Nuevo Cambio Realizado
        //             */
        //            LOG_MARCACIONES MarcaOriginalEntrada = new LOG_MARCACIONES
        //            {
        //                EMPRESA = empresa,
        //                USUARIO = usuario,
        //                TRABAJADOR = RUT,
        //                FECHA_MOD = DateTime.Now,
        //                MARCA_ORIGINAL = MarcaEntrada,
        //                TIPO_MARCA_ORIGINAL = "I",
        //                MARCA_NUEVA = MarcaEntrada,
        //                TIPO_MARCA_NUEVA = "I",
        //                ACCION = "ESPERANUE"
        //            };

        //            db.LOG_MARCACIONES.Add(MarcaOriginalEntrada);
        //            db.SaveChanges();
        //            VRenid = MarcaOriginalEntrada.ID;

        //            LOG_ACEPTATIEMPO MarcaNuevaEnt = new LOG_ACEPTATIEMPO
        //            {
        //                LID = VRenid,
        //                LESTADO = "ESPERA",
        //                LACCION = "I",
        //                LFECHACAMBIO = DateTime.Now,
        //                LCORREO = VMailTrabajador,
        //                LTOKEN = Vtoken,
        //            };

        //            db.LOG_ACEPTATIEMPO.Add(MarcaNuevaEnt);
        //            db.SaveChanges();


        //        Notifica_modificacion notifica_Modificacion = new Notifica_modificacion();
        //        notifica_Modificacion.Notifica_trabajador(VPareado, " ", RUT, MARCAORIGINAL, PAREADO, VMailTrabajador, VTipo, Vtoken, VRenid, VRenSal, vfullname);

        //        return Json(new
        //        {
        //            Mensaje = "Marcación pareada"
        //        });
        //    }
        //}
        public static void InsertaMarca(string rut,DateTime marca, string tipoMarca, string coordenada,string modificada)
        {

            f.EjecutarConsultaSQLCli("SELECT USERID from USERINFO " +
                "where REPLACE(SSN,'-','') = '" + rut, "gycsolcl_Relojcontrol");
            if (f.Tabla.Rows.Count > 0)
            {
                var tablog = (from DataRow dr in f.Tabla.Rows
                              select new
                              {
                                  userid = int.Parse(dr["USERID"].ToString()),
                              }).ToList();
                var buslog = tablog.LastOrDefault();
                int userid = buslog.userid;
                string query4 = "insert into CHECKINOUT (USERID,CHECKTIME, CHECKTYPE, COORDENADA, MODIFICADA) " +
                    "values " +
                    "('" + userid + "','" + marca + "','" + tipoMarca + "','" + coordenada + "','" + modificada + "') ! ";
                if (f.EjecutarQuerySQLCli(query4, "gycsolcl_Relojcontrol"))
                {
                    return;
                }
            }
            return;
        }
        public class Notifica_modificacion
        {
            /// <summary>
            ///  Se envia RUT para notifica al trabajdor
            ///  se recibe True
            /// </summary>
            /// <param name="Prut"></param>
            /// <returns></returns>
            /// 

            Funciones f = new Funciones();

            public bool Notifica_trabajador(string PTIPO1, string PTIPO_ORIGINAL, string Prut, DateTime PMARCACION, DateTime PMARCACION_NUEVA, String PCORREO, String PTipo, string PToken, int PID1, int PID2, string PFullname)
            {
                // Para no envíar correos a servimar
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

                //string correo_usr = "Ricardo.rocco@gmail.com";
                string correo_usr = PCORREO;
                string nombre_full = PFullname;
                //DateTime hoy = DateTime.Now.AddHours(f.AjusteHora());
                DateTime hoy = DateTime.Now;

                string FMARCACION = PMARCACION.ToString("dd'-'MM'-'yyyy HH:mm:ss");
                string FNUEVA = PMARCACION_NUEVA.ToString("dd'-'MM'-'yyyy HH:mm:ss");
                string Fhoy = hoy.ToString("dd'-'MM'-'yyyy HH:mm:ss");
                int enroquepareado = 0;

                string rut_usr = Prut;
                string fvtiponuevo = " ";
                string fvtipooriginal = " ";
                if (PTIPO1 == "I") { fvtiponuevo = "Entrada"; } else { fvtiponuevo = "Salida"; }
                ;
                if (PTIPO_ORIGINAL == "I") { fvtipooriginal = "Entrada"; } else { fvtipooriginal = "Salida"; }
                ;

                //var dettiemp = db.remepage.Where(x => "5         " == x.cod_param && x.nom_tabla == "TIEMPO" && x.rut_empr == empresa).SingleOrDefault();
                string envmail = "S";
                // Para no envíar correos a servimar
                if (envmail == "N")
                {
                    if (PTipo == "I")
                    {
                        InsertaMarca(Prut, PMARCACION, "I", string.Empty, string.Empty);
                        InsertaMarca(Prut, PMARCACION_NUEVA, "O", string.Empty, string.Empty);

                    }
                    //if (PTipo == "M")
                    //{
                    //    if (PTIPO1 == "I")
                    //    {
                    //        db2.ELIMINA_MARCACION(Prut, PMARCACION, "I");
                    //        db.JVC_INSERTMARCAMANUAL(Prut, PMARCACION_NUEVA, "I", string.Empty, string.Empty);
                    //    }
                    //    else
                    //    {
                    //        db2.ELIMINA_MARCACION(Prut, PMARCACION, "O");
                    //        db.JVC_INSERTMARCAMANUAL(Prut, PMARCACION_NUEVA, "O", string.Empty, string.Empty);
                    //    }

                    //}
                    //if (PTipo == "E")
                    //{
                    //    db.JVC_INSERTMARCAMANUAL(Prut, PMARCACION_NUEVA, "I", string.Empty, string.Empty);

                    //}
                    //if (PTipo == "S")
                    //{
                    //    db.JVC_INSERTMARCAMANUAL(Prut, PMARCACION_NUEVA, "O", string.Empty, string.Empty);

                    //}
                    //if (PTIPO1 == "D")
                    //{
                    //    db.JVC_INSERTMARCAMANUAL(Prut, PMARCACION_NUEVA, "O", string.Empty, string.Empty);

                    //}


                    return true;
                }

                string linkhost = " ";

                if (PTIPO1 == "P")
                {
                    linkhost = "https://api.gycsol.cl/api/RespuestaPareada/GetPERSONA?Token=";
                    enroquepareado = PID2;
                    PID2 = PID1;
                    PID1 = enroquepareado;
                }
                else
                {
                    linkhost = "https://api.gycsol.cl/api/RespuestaTrabajador/GetPERSONA?Token=";
                }

                string LinkSi = linkhost + PToken + "&Answer=SI&Id1=" + PID1 + "&Id2=" + PID2;

                string LinkNo = linkhost + PToken + "&Answer=NO&Id1=" + PID1 + "&Id2=" + PID2;


                msg.To.Add(correo_usr);
                /// msg.Subject = "Envío Modificacion Control  Reloj " + DateTime.Now;
                /// 
                /*
                 * Tipos en Variables Ptipo
                 * 
                 * M =  Modificacion de Tiempo
                 * I =  Nuevo Ingreso de Tiempo
                 * E =  Nueva Entrada en Pareado (PMARCACION = Fecha Marcada x trabajador ; PMARCACION_NUEVA = Fecha Nueva Pareada )
                 * S =  Nueva Salida en Pareado
                 * 
                 * Tipo en Variable PTIPO1
                 * P =  Corresponde a Evento Pareado con tratamiento especial en entrada y salida
                 * D =  Elemento Eliminado
                 */

                if (PTIPO1 == "D")
                {
                    PTipo = "N"; // Se asigna N de Nulo para evitar otro tratamiento //

                    msg.Subject = "Notifica Eliminación Registro Control Tiempo -  GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                                           "Notificamos que se ha efectuado  una eliminación  en el registro control tiempo por parte de su empresa." +
                                           "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Eliminada : " + FMARCACION + " " + fvtipooriginal
                                           + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" +
                                           " <br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";


                }

                if (PTIPO1 == "P")
                {
                    PTipo = "N"; // Se asigna N de Nulo para evitar otro tratamiento //

                    msg.Subject = "Autorización de Nuevo Ingreso Pareado - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                                           "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                                           "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Original : " + FMARCACION + " Entrada " + "<br/> Marcación Agregada: " + FNUEVA +
                                           " Entrada " + "<br/><br/> Marcación final a autorizar : " + FNUEVA + " Entrada / " + FMARCACION + " Salida <br/><br/>"
                                           + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                                           " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                                           + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                                       + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                                              + LinkNo +
                                           " <br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";
                }


                if (PTipo == "M")
                {

                    msg.Subject = "Autorización de Modificación - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un cambio en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/> Marcación Registrada : " + FMARCACION + " " + fvtipooriginal + "<br/> Nueva Marcación : " + FNUEVA + " " + fvtiponuevo +
                        " <br/>Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        " <br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";

                }
                if (PTipo == "I")
                {
                    msg.Subject = "Autorización de Nuevo Ingreso - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/>  Nuevo Registro : " + FMARCACION + " Entrada " + "<br/> Nuevo Registro: " + FNUEVA +
                        " Salida " + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        "<br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";


                }
                if (PTipo == "E")
                {
                    msg.Subject = "Autorización de Nuevo Ingreso Pareado - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Registrada : " + FMARCACION + " Salida " + "<br/> Evento Pareado: " + FNUEVA +
                        " Entrada " + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        "<br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";

                }
                if (PTipo == "S")
                {
                    msg.Subject = "Autorización de Nuevo Ingreso Pareado - Marcación asistencia GYCSol.";

                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    msg.Body = "Estimado Sr(a).   " + nombre_full + " " + rut_usr + " : <br/><br/>" +
                        "Notificamos que el sistema ha detectado un nuevo ingreso en el registro control tiempo por parte de su empresa." +
                        "<br/><br/> Fecha : " + Fhoy + "<br/>  Marcación Registrada : " + FMARCACION + " Entrada " + "<br/> Evento Pareado: " + FNUEVA +
                        " Salida " + "<br/> Con el objeto de cumplir con lo instruído en ORD Nº 5849/133 de la Dirección del Trabajo, <br/>" + " Se somete a su " + "<b> ACEPTACION </b> " +
                        " o su <b> RECHAZO </b> ," + " la marcación sugerida . En Caso de no responder la propuesta en un plazo máximo de 48 Horas, "
                        + " se entederá que rechaza la fórmula y la marca o ausencia permanecerán en su estado original. <br/> <br/> <br/>" + "Adjunto link para <b> ACEPTACION </b> "
                    + LinkSi + " <br/><br/><br/>" + "Adjunto link para <b> RECHAZO  </b> "
                           + LinkNo +
                        "<br/><br/>Usuario: " + rut_usr.Replace("-", "") +
                        "<br/><br/>Atentamente - <br/>Gestión y Competencia SPA.";

                }

                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.From = new System.Net.Mail.MailAddress("plataforma@gycsol.cl");
                //msg.Bcc.Add("gycsol.almacen@gmail.com");
                msg.Bcc.Add("gycsol.almacen@gmail.com,jvalladaresc2@gmail.com");
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient()
                {
                    //Credentials = new System.Net.NetworkCredential("plataforma@gycsol.cl", "Lq!6q9g2"),
                    Credentials = new System.Net.NetworkCredential("plataforma@gycsol.cl", "4Ww8y7^j"),
                    EnableSsl = false,
                    Host = "gycsol.cl",
                    Port = 25,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                };

                try
                {
                    cliente.Send(msg);
                    return true;
                }
                catch (Exception e2)
                {
                    //f.LogError(e2, "web.gycsol");
                    return true;
                }
            }
        }
    }
}
namespace RRHH.Models.ViewModels
{
    public class DetMarcasTrabajador
    {
        public int id { get; set; }
        public string ruttrabajador { get; set; }
        public string fecha { get; set; }
        public string diasemana { get; set; }
        public string checkent { get; set; }
        public string checksal { get; set; }
        public string entrada { get; set; }
        public string salida  { get; set; }
    }
}

