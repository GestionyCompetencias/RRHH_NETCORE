using RRHH.BaseDatos.NoSQL.Permisos;
using RRHH.Data;
using RRHH.Models.Utilities.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Rollbar;

namespace RRHH.Models.Estructuras.Permisos
{
    public enum DefaultProfile
    {
        /// <summary>
        /// Contiene todos los permisos posibles para administración.
        /// </summary>
        Admin,
        /// <summary>
        /// No tiene accesos para operar en la plataforma.
        /// </summary>
        NoAccess
    }

    public static class PerfilBuilder 
    {
        /// <summary>
        /// Construye un perfil a partir de los datos almacenados en NoSQL.
        /// </summary>
        /// <param name="mongoProfile">Profile MongoDB</param>
        /// <returns>Objeto de tipo Perfil.</returns>
        public static Perfil BuildFromNoSQL(Profile mongoProfile)
        {
            // Se inicializa el perfil a construir
            Perfil perfilObject = new Perfil();

            // Definir datos básicos del permiso. Estos datos son esenciales para la construcción.
            perfilObject.Id = mongoProfile.Id;
            perfilObject.Title = mongoProfile.Title;
            int empresaId;
            bool validEmpresaId = int.TryParse(mongoProfile.EmpresaId, out empresaId);
            perfilObject.EmpresaId = validEmpresaId ? empresaId : 0;

            // Iterar sobre todas las features y agregar los accesos
            mongoProfile.Modules.ForEach(module =>
            {
                module.SubModules.ForEach(subModule => {
                    subModule.Features.ForEach(feature =>
                    {
                        foreach(var access in (Dictionary<string,object>)feature.AccessList.First().Value)
                        {
                            perfilObject.AddAccessValue(module.Title, subModule.Title, feature.Title, access.Key, access.Value.ToString());
                        }
                    });
                });
            });

            return perfilObject;
        }

        /// <summary>
        /// Convierte un perfil a formato MongoDB.
        /// </summary>
        /// <param name="perfil">Objeto de tipo perfil.</param>
        /// <returns>Objeto de tipo Profile para MongoDB.</returns>
        public static Profile ConvertToNoSQL(Perfil perfil)
        {
            Profile profile = new Profile();
            profile.Id = perfil.Id;
            profile.Title = perfil.Title;
            profile.EmpresaId = perfil.EmpresaId.ToString();

            perfil.Accesos.ForEach(acceso =>
            {
                if (profile.Modules == null) profile.Modules = new List<Module> { };

                Module modulo = profile.Modules.Find(m => m.Title == acceso.Modulo);
                if (modulo == null) { 
                    modulo = new Module();
                    modulo.Title = acceso.Modulo;
                    modulo.SubModules = new List<SubModule>();
                    profile.Modules.Add(modulo);  
                }

                SubModule subModulo = modulo.SubModules.Find(s => s.Title == acceso.SubModulo);
                if (subModulo == null) { 
                    subModulo = new SubModule();
                    subModulo.Title = acceso.SubModulo;
                    subModulo.Features = new List<Feature>();
                    modulo.SubModules.Add(subModulo); 
                }

                Feature feat = subModulo.Features.Find(f => f.Title == acceso.Funcion);
                if (feat == null) { 
                    feat = new Feature();
                    feat.Title = acceso.Funcion;
                    feat.AccessList = new Dictionary<string, object> { };
                    subModulo.Features.Add(feat); 
                }

                feat.AccessList.Add(acceso.NombreAcceso, acceso.Value);
            });

            return profile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileTemplate"></param>
        /// <param name="_environment"></param>
        /// <returns></returns>
        public static Profile GenerateDefaultProfile(DefaultProfile profileTemplate, IWebHostEnvironment environment, IJsonDataReader jsonDataReader)
        {
            // Seleccion del archivo JSON para tal perfil
            string filename = String.Empty;
            switch(profileTemplate)
            {
                case DefaultProfile.Admin:
                    filename = "admin_profile.json";
                    break;
                case DefaultProfile.NoAccess:
                    filename = "no_access.json";
                    break;
                default:
                    break;
            }

            // Si el perfil no se ha encontrado, abortar operacion y generar reporte en Rollbar
            if (filename == String.Empty)
            {
                if (DeployEnvironment.isProduction())
                {
                    RollbarLocator.RollbarInstance.Error($"Error al generar un perfil por defecto", new Dictionary<string, object?>()
                    {
                        {"ExtraInfo", new { tipoPerfil = profileTemplate} }
                    });
                }
                return null;
            }

            
            string jsonPath = $"{environment.ContentRootPath}/Data/Perfiles/{filename}";
            Profile defaultProfile = jsonDataReader.DeserializeData<Profile>(jsonPath);

            return defaultProfile;
        }
    }
}
