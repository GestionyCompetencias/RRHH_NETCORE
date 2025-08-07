using RRHH.BaseDatos;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace RRHH.Servicios
{
    public class Correos 
    {
        private Funciones f = new Funciones();
        string RutEmpresa = "";


        public string destinatarioErrores = "jvalladaresc2@gmail.com";
        public bool SendEmail(string Mensaje, string Asunto, string CabeceraMensaje, string destinatario, string? origen = null, int? empresa = null)
        {

            var correo = new System.Net.Mail.MailMessage();
            correo.From = new System.Net.Mail.MailAddress("notificacion@gycsol.cl");
            //correo.To.Add(CorreoPersona);
            correo.To.Add(destinatario);


            correo.Subject = Asunto;
            correo.SubjectEncoding = System.Text.Encoding.UTF8;
            correo.Body = CabeceraMensaje + "<br/>";
            if (Asunto == "Solicitud de boleta de honorarios" || Asunto == "Detalle de reembolso de gastos") correo.CC.Add("snanco@gestionycompetencias.cl,jvalladares@gestionycompetencias.cl");
            if (Asunto == "Notificacion de cobro") correo.CC.Add("ronaldjairo@gmail.com");

            correo.Body += Mensaje;
            correo.BodyEncoding = System.Text.Encoding.UTF8;

            correo.IsBodyHtml = true;

            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient()
            {
                Credentials = new System.Net.NetworkCredential("notificacion@gycsol.cl", "Febrero.2022*"),
                EnableSsl = false,
                Host = "contable.gycsol.cl",
                Port = 25,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
            };

            try
            {
                cliente.Send(correo);
                return true;
            }
            catch (Exception e2)
            {
                var errorMail = " Correo no enviado " + e2.Message.ToString() + " " + e2.StackTrace.ToString();
                return false;
            }

        }
        public bool SendEmailCC(string Mensaje, string Asunto, string CabeceraMensaje, string destinatario, string copia)
        {

            var correo = new System.Net.Mail.MailMessage();
            correo.From = new System.Net.Mail.MailAddress("notificacion@gycsol.cl");
            //correo.To.Add(CorreoPersona);
            correo.To.Add(destinatario);


            correo.Subject = Asunto;
            correo.SubjectEncoding = System.Text.Encoding.UTF8;
            correo.Body = CabeceraMensaje + "<br/>";
            if(copia != null)correo.CC.Add(copia);

            correo.Body += Mensaje;
            correo.BodyEncoding = System.Text.Encoding.UTF8;

            correo.IsBodyHtml = true;

            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient()
            {
                Credentials = new System.Net.NetworkCredential("notificacion@gycsol.cl", "Febrero.2022*"),
                EnableSsl = false,
                Host = "contable.gycsol.cl",
                Port = 25,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
            };

            try
            {
                cliente.Send(correo);
                return true;
            }
            catch (Exception e2)
            {
                var errorMail = " Correo no enviado " + e2.Message.ToString() + " " + e2.StackTrace.ToString();
                return false;
            }

        }


        public bool SendEmailRendiciones(string Mensaje, string Asunto, string CabeceraMensaje, string destinatario)
        {

            var correo = new System.Net.Mail.MailMessage();
            correo.From = new System.Net.Mail.MailAddress("notificacion@gycsol.cl");
            //correo.To.Add(CorreoPersona);
            correo.To.Add(destinatario);

            correo.CC.Add("ronaldjairo@gmail.com"); //,snanco@gestionycompetencias.cl

            correo.Subject = Asunto;
            correo.SubjectEncoding = System.Text.Encoding.UTF8;
            correo.Body = CabeceraMensaje + "<br/>";

            correo.Body += Mensaje;
            correo.BodyEncoding = System.Text.Encoding.UTF8;

            correo.IsBodyHtml = true;

            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient()
            {
                Credentials = new System.Net.NetworkCredential("notificacion@gycsol.cl", "Febrero.2022*"),
                EnableSsl = false,
                Host = "contable.gycsol.cl",
                Port = 25,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
            };

            try
            {
                cliente.Send(correo);
                return true;
            }
            catch (Exception e2)
            {
                var errorMail = " Correo no enviado " + e2.Message.ToString() + " " + e2.StackTrace.ToString();
                return false;
            }

        }


        public void RegistroLogMails(string Mensaje, string Asunto, string CabeceraMensaje, string destinatario, int empresa)
        {
            RutEmpresa = f.obtenerRUT(empresa);
            var BD_Cli = "remuneracion_" + RutEmpresa;
            DateTime _fregist = DateTime.Now;
            DateTime _hregist = DateTime.Now;
            string connectString1 = f.CadenaConexionCliente(BD_Cli);

            using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {                
                using (SqlConnection conexion = new SqlConnection(connectString1))
                {
                    conexion.Open();

                    var query1 = "insert into mailsEnviados (fecha,hora,destinatarios,asunto,mensaje) " +
                        " values " +
                        "(@param_fecha,@param_hora,@param_destinatarios,@param_asunto,@param_mensaje) ";

                    SqlCommand comando = new SqlCommand(query1, conexion);

                    comando.Parameters.AddWithValue("@param_fecha", _fregist);
                    comando.Parameters.AddWithValue("@param_hora", _hregist);
                    comando.Parameters.AddWithValue("@param_destinatarios", destinatario);
                    comando.Parameters.AddWithValue("@param_asunto", Asunto);
                    comando.Parameters.AddWithValue("@param_mensaje", Mensaje);

                    comando.ExecuteNonQuery();
                }
                transaction.Complete();
                transaction.Dispose();
            }
        }



    }
}
