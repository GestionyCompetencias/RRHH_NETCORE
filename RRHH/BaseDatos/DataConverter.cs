using Microsoft.IdentityModel.Tokens;
using Rollbar;
using RRHH.Models.Utilities.Configuration;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.BaseDatos
{
    public static class DataConverter
    {

        public static EmpresasVM DataRowToEmpresaVM(DataRow row)
        {
            // TODO: SE NECESITA HACER EL CAMPO DE IDREPRESENTANTE E IDENCARGADO OBLIGATORIO
            EmpresasVM empresaVM = new EmpresasVM();
            try
            {
                empresaVM.id = int.Parse(row["id"].ToString());
                empresaVM.rut = row["rut"].ToString();
                empresaVM.razonsocial = row["razonsocial"].ToString();
                empresaVM.fantasia = row["fantasia"].ToString();
                empresaVM.giro = row["giro"].ToString();
                empresaVM.idPais = int.Parse(row["idpais"].ToString());
                empresaVM.idregion = int.Parse(row["idregion"].ToString());
                empresaVM.idcomuna = int.Parse(row["idcomuna"].ToString());
                empresaVM.email = row["email"].ToString();
                empresaVM.obs = row["obs"].ToString();

                // Campos Opcionales
                if (!row["idencargado"].ToString().IsNullOrEmpty())
                    empresaVM.idencargado = int.Parse(row["idencargado"].ToString());

                if (!row["idrepresentante"].ToString().IsNullOrEmpty())
                    empresaVM.idrepresentante = int.Parse(row["idrepresentante"].ToString());
            }
            catch (Exception ex)
            {
                if (DeployEnvironment.isProduction())
                    RollbarLocator.RollbarInstance.Error(ex);

                return null;
            }

            return empresaVM;
        }

        public static BancosVM DataRowToBancoVM(DataRow row)
        {
            BancosVM bancoVM = new BancosVM();
            bancoVM.Id = int.Parse(row["id"].ToString());
            bancoVM.Nombre = row["nombre"].ToString();
            return bancoVM;
        }

        public static UsuarioVM DataRowToUsuarioVM(DataRow row)
        {
            UsuarioVM usuarioVM = new UsuarioVM();
            try
            {
                usuarioVM.id = int.Parse(row["id"].ToString());
                usuarioVM.usuraio = row["nombreUsu"].ToString();
                usuarioVM.nombre = row["nombres"].ToString();
                usuarioVM.apellido = row["apellidos"].ToString();
                usuarioVM.perfil = row["idperfil"].ToString();
                usuarioVM.email = row["email"].ToString();
                usuarioVM.telef = row["whatsapp"].ToString();
            }
            catch (Exception ex)
            {
                if (DeployEnvironment.isProduction())
                    RollbarLocator.RollbarInstance.Error(ex);

                return null;
            }
            
            return usuarioVM;
        }
    }
}
