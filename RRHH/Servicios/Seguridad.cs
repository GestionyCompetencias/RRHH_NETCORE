namespace RRHH.Servicios
{
    public class Seguridad
    {
        public bool ValidarUsuario(string logeado)
        {
            bool validado = false;

            if (string.IsNullOrEmpty(logeado))
            {
                validado = false;
            }
            else
            {
                validado = true;
            }

            return validado;
        }
    }
}
