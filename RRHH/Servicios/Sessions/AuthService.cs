using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Rollbar;
using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RRHH.Servicios.Sessions
{
    public interface IAuthService
    {
        public resultLoginVM Login(string username, string password);
        public bool Logout();
    }

    // Servicio para autenticación, registro y manejo de sesión
    public class AuthService : IAuthService
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;

        public AuthService(IDatabaseManager databaseManager, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _databaseManager = databaseManager;
            _contextAccessor = httpContextAccessor;
            _config = config;
        }

        // Inicio de sesión para un usuario
        public resultLoginVM Login(string username, string password)
        {
            string dbQuery = "SELECT * FROM usuarios INNER JOIN usuarioempresa on usuarios.id = usuarioempresa.idusuario " +
                "WHERE usuarios.nombreUsu=@Username AND usuarios.contra=@Password";

            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)  
            };

            DataTable dt = _databaseManager.ExecuteGlobalQuery(dbQuery, sqlParameters);

            // Si el usuario no fue encontrado, la ejecución retorna nulo
            if (dt.Rows.Count == 0) 
                return null;

            // Si el usuario no se ha podido gestionar en DataConverter, terminar ejecución
            UsuarioVM usuario = DataConverter.DataRowToUsuarioVM(dt.Rows[0]);
            if (usuario == null)
                return null;

            // Ajustar contexto de sesión
            _contextAccessor?.HttpContext?.Session.SetString("UserIdLog", usuario.id.ToString());
            _contextAccessor?.HttpContext?.Session.SetString("UserNameLog", usuario.nombre);
            _contextAccessor?.HttpContext?.Session.SetString("UserIdPerfil", dt.Rows[0]["idPerfil"].ToString());

            // Generar token de inicio de sesión
            string authToken = GenerateAuthToken(usuario);

            return new resultLoginVM(usuario.id, authToken);    
        }

        // Cierre de sesión
        public bool Logout()
        {
            // Llaves de las strings que deben ser limpiadas
            string[] sessionStrings = new string[] { "UserIdLog", "UserNameLog", "UserIdPerfil", 
                "EmpresaLog", "NombreEmpresaLog", "stringBDcli" };

            try
            {
                // Limpieza de la información
                foreach (string str in sessionStrings)
                {
                    _contextAccessor?.HttpContext?.Session.SetString(str, "");

                }           
            }
            catch (Exception ex)
            {
                // Log en rollbar solo si es ambiente de producción
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                    RollbarLocator.RollbarInstance.Error(ex);
                return false;
            }

            return true;
        }

        // Generación de la token de autenticación JWT
        private string GenerateAuthToken(UsuarioVM usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Crear los claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.usuraio),
                new Claim(ClaimTypes.Email, usuario.email),
                new Claim(ClaimTypes.GivenName, usuario.nombre),
                new Claim(ClaimTypes.Surname, usuario.apellido),
                new Claim(ClaimTypes.Role, usuario.perfil),
            };

            //Crear el token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(600),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
