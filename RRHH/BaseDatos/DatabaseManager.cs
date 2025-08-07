using Microsoft.Data.SqlClient;
using RRHH.Models.ViewModels;
using System.Data;

namespace RRHH.BaseDatos
{

    public interface IDatabaseManager
    {
        public bool ExecuteTransactionQuery(string query, List<SqlParameter>? sqlParameters = null);
        public DataTable ExecuteGlobalQuery(string query, List<SqlParameter>? sqlParameters = null);
        public bool ExecuteEmpresaTransaction(string query, int idEmpresa, List<SqlParameter>? sqlParameters = null);
        public DataTable ExecuteEmpresaQuery(string query, int idEmpresa, List<SqlParameter>? sqlParameters = null);
        public string EmpresaDatabaseName(int empresaId);

    }

    public class DatabaseManager : IDatabaseManager
    {
        // Conexión a la base de datos principal
        private String _globalConnectionString;

        private readonly IConfiguration _configuration;

        public DatabaseManager(IConfiguration config) 
        {
            _configuration = config;

            // Si por alguna razón el environment no se encuentra, asumimos que es dev
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            env = env != null ? "Environments:" + env : "Environments:Development";

            string databaseIP = _configuration[env + ":GlobalDatabase:Host"];
            string userId = _configuration[env + ":GlobalDatabase:UserId"];
            string password = _configuration[env + ":GlobalDatabase:Password"];
            string baseDatos = _configuration[env + ":GlobalDatabase:Catalog"];
            string port = _configuration[env + ":GlobalDatabase:Port"];

            
            // Asignacion de la string de conexión basada en el ambiente 
            _globalConnectionString = String.Format("Data Source={0},{1};Initial Catalog={4};User ID={2};" +
                "Password={3};Trust Server Certificate=true", databaseIP, port, userId, password, baseDatos);
        }    

        // Función estática para hacer transacciones a la base de datos global
        // Si el número afectado de filas es mayor a 0, el query es exitoso.
        // Ejemplos de uso: insertar nuevas filas, alterar filas preexistentes
        public bool ExecuteTransactionQuery(string query, List<SqlParameter>? sqlParameters = null)
        {
            try
            {
                int rowsAffected = 0;
                using (SqlConnection connection = new SqlConnection(_globalConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Agregar parametros al query (evitar inyecciones SQL a toda costa)
                        if (sqlParameters != null)
                            sqlParameters.ForEach(sqlParameter => { command.Parameters.Add(sqlParameter); });

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }

                // True si se modifico/creo al menos un registro
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public DataTable ExecuteGlobalQuery(string query, List<SqlParameter>? sqlParameters = null)
        {
            DataTable dt = new DataTable(); 
            SqlConnection connection = new SqlConnection(_globalConnectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);

            // Agregar parametros al query (evitar inyecciones SQL a toda costa)
            if (sqlParameters != null) 
                sqlParameters.ForEach(sqlParameter => { command.Parameters.Add(sqlParameter); });   


            using (SqlDataReader reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }

            connection.Close();
            return dt;
        }


        // Función estática para hacer transacciones a la base de datos particular de una empresa
        // Si el número afectado de filas es mayor a 0, el query es exitoso.
        // Ejemplos de uso: insertar nuevas filas, alterar filas preexistentes
        public bool ExecuteEmpresaTransaction(string query, int idEmpresa, List<SqlParameter>? sqlParameters = null)
        {
            try
            {
                string localConnectionString = EmpresaConnectionString(idEmpresa);
                int rowsAffected = 0;
                using (SqlConnection connection = new SqlConnection(localConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Agregar parametros al query (evitar inyecciones SQL a toda costa)
                        if (sqlParameters != null)
                            sqlParameters.ForEach(sqlParameter => { command.Parameters.Add(sqlParameter); });

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                    }
                    connection.Close();
                }

                // True si se modifico/creo al menos un registro
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable ExecuteEmpresaQuery(string query, int idEmpresa, List<SqlParameter>? sqlParameters = null)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(EmpresaConnectionString(idEmpresa));
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);

            // Agregar parametros al query (evitar inyecciones SQL a toda costa)
            if (sqlParameters != null)
                sqlParameters.ForEach(sqlParameter => { command.Parameters.Add(sqlParameter); });

            using (SqlDataReader reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }

            connection.Close();
            return dt;
        }

        // ------- HELPERS ------------
        // Retorna el connection string para una empresa en particular
        private string EmpresaConnectionString(int idEmpresa)
        {
            string databaseName = EmpresaDatabaseName(idEmpresa);

			// Si por alguna razón el environment no se encuentra, asumimos que es dev
			string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			env = env != null ? "Environments:" + env : "Environments:Development";

			string databaseIP = _configuration[env + ":GlobalDatabase:Host"];
			string userId = _configuration[env + ":GlobalDatabase:UserId"];
			string password = _configuration[env + ":GlobalDatabase:Password"];
			string port = _configuration[env + ":GlobalDatabase:Port"];

			// Asignacion de la string de conexión basada en el ambiente 
			return String.Format("Data Source={0},{1};Initial Catalog={2};User ID={3};" +
				"Password={4};Trust Server Certificate=true", databaseIP, port, databaseName ,userId, password);
        }

        // Retorna el nombre de la base de datos de una empresa en particular
        public string EmpresaDatabaseName(int empresaId)
        {
            string dbQuery = "SELECT * FROM empresas WHERE id = @empresaId";
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@empresaId", empresaId)
            };
            
            DataTable empresasList = ExecuteGlobalQuery(dbQuery, sqlParameters);

            // Si no existe una empresa con tal ID
            if (empresasList.Rows.Count == 0)
                throw new Exception(
                    String.Format("DatabaseManager: No existe una empresa registrada con el ID: {0}", empresaId.ToString()));

            EmpresasVM empresa = DataConverter.DataRowToEmpresaVM(empresasList.Rows[0]);        
            return "remuneracion_" + empresa.rut;
        }
    }
}
