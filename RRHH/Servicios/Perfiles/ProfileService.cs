using RRHH.BaseDatos;
using RRHH.BaseDatos.NoSQL;
using RRHH.BaseDatos.NoSQL.Permisos;
using RRHH.Data;
using RRHH.Models.Estructuras.Permisos;
using RRHH.Models.Utilities.Configuration;
using RRHH.Models.Utilities.Sessions;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using Rollbar;
using System.Data;

namespace RRHH.Servicios.Perfiles
{
    /// <summary>
    /// Interfaz para gestionar los permisos, accesos y perfiles de recursos humanos.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Obtiene el perfil de un usuario basado en su ID y la empresa asignada.
        /// Esta función utiliza el PerfilBuilder para su operación.
        /// </summary>
        /// <param name="usuarioId">ID del usuario.</param>
        /// <param name="empresaId">ID de la empresa.</param>
        /// <returns>Objeto de tipo Perfil asignado a tal usuario.</returns>
        public Perfil GetUsuarioPerfil(int usuarioId, int empresaId);
        /// <summary>
        /// Establece los permisos de la sesión de acuerdo al perfil del usuario para una empresa determinada.
        /// </summary>
        /// <param name="empresaId"></param>
        public void SetSessionAccess(int empresaId);
        /// <summary>
        /// Elimina el perfil de la sesión asociada.
        /// </summary>
        public void ClearSessionAccess();
    }

    public class ProfileService : IProfileService
    {
        private readonly IMongoCollectionManager _mongoCollectionManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _environment;
        private readonly IJsonDataReader _jsonDataReader;

        public ProfileService(IMongoCollectionManager mongoCollectionManager, 
            IDatabaseManager databaseManager, 
            IHttpContextAccessor contextAccessor, 
            IWebHostEnvironment webHostEnvironment, 
            IJsonDataReader jsonDataReader)
        {
            _mongoCollectionManager = mongoCollectionManager;
            _databaseManager = databaseManager;
            _contextAccessor = contextAccessor;
            _environment = webHostEnvironment;
            _jsonDataReader = jsonDataReader;
        }

        public Perfil GetUsuarioPerfil(int usuarioId, int empresaId)
        {
            string query = "SELECT * FROM usuarioperfil WHERE idusuario = @idUsuario AND idempresa = @idEmpresa";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@idUsuario", usuarioId),
                new SqlParameter("@idEmpresa", empresaId)
            };

            // Obtenemos el ID del perfil almacenado en MongoDB
            string profileId = String.Empty;
            try
            {
                DataTable dataTablePerfil = _databaseManager.ExecuteGlobalQuery(query, parameters);

                // Perfil no encontrado, se devuelve nulo
                if (dataTablePerfil.Rows.Count == 0) return null;

                profileId = dataTablePerfil.Rows[0]["idProfile"].ToString();

                // Perfil en blanco, se devuelve nulo
                if (profileId == String.Empty) return null;
            }
            catch (Exception ex)
            {
                if (DeployEnvironment.isProduction())
                {
                    RollbarLocator.RollbarInstance.Error($"La query para encontrar perfil de usuario ha fallado", new Dictionary<string, object?>()
                    {
                        {"ExtraInfo", new { usuarioId = usuarioId, empresaId = empresaId, ex = ex}}
                    });
                }
            
                return null;
            }

            var profileCollection = _mongoCollectionManager.GetCollection<Profile>();
            Profile profile = profileCollection.Find(p => p.Id == profileId).FirstOrDefault();

            // El perfil asignado en MongoDB no existe, no debería ocurrir se debe registrar en Rollbar
            if (profile == null)
            {
                if (DeployEnvironment.isProduction())
                {
                    RollbarLocator.RollbarInstance.Error($"Un usuario tiene un perfil MongoDB inexistente", new Dictionary<string, object?>()
                    {
                        {"ExtraInfo", new { usuarioId = usuarioId, empresaId = empresaId, profileId = profileId}}
                    });
                }

                return null;
            }

            // Convertimos el modelo MongoDB Profile a Perfil legible por el código
            Perfil perfil = PerfilBuilder.BuildFromNoSQL(profile);
            return perfil;
        }

        public void SetSessionAccess(int empresaId)
        {
            // Obtención del user id desde el contexto
            string contextUserId = _contextAccessor?.HttpContext?.Session.GetString("UserIdLog");

            int userId = 0;
            try
            {
                userId = int.Parse(contextUserId);
            }
            catch (Exception ex)
            {
                // No se pudo generar un ID de usuario a partir de la variable de contexto
                return;
            }

            Perfil perfil = GetUsuarioPerfil(userId, empresaId);

            // Si el perfil no se encuentra, se asigna un perfil que no tiene accesos
            if (perfil == null)
            {
                Profile noAccessProfile = PerfilBuilder.GenerateDefaultProfile(DefaultProfile.NoAccess, _environment, _jsonDataReader);
                perfil = PerfilBuilder.BuildFromNoSQL(noAccessProfile);
            }

            // Establece el perfil de accesos para una sesión
            _contextAccessor?.HttpContext?.Session.SetObjectAsJson("PerfilEmpresa", perfil);
        }

        public void ClearSessionAccess()
        {
            _contextAccessor?.HttpContext?.Session.Remove("PerfilEmpresa");
        }
    }
}
