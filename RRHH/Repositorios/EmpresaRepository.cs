using RRHH.BaseDatos;
using RRHH.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace RRHH.Repositorios
{

    public interface IEmpresaRepository
    {
        List<EmpresasVM> GetAllEmpresas(bool soloHabilitadas);
        List<EmpresasVM> GetEmpresasByUsuario(int userId);
        List<EmpresasVM> GetEmpresasHabilitadasByUsuario(int userId);
        List<EmpresasVM> GetEmpresasByRut(string rut);
    }

    // Este servicio contiene todas aquellas funciones relacionadas con las empresas
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly IDatabaseManager _databaseManager;

        public EmpresaRepository(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        // Retorna la lista de todas las empresas habilitadas. Si el argumento es false, retorna todas las empresas de la database
        public List<EmpresasVM> GetAllEmpresas(bool soloHabilitadas)
        {
            List<EmpresasVM> empresas = new List<EmpresasVM>();

            string query;
            if (soloHabilitadas)
            {
                query = "SELECT * FROM empresas INNER JOIN paises on " +
                    "empresas.idpais = paises.id INNER JOIN regiones on empresas.idregion = regiones.id WHERE habilitado = 1";
            }
            else
            {
                query = "SELECT * FROM empresas INNER JOIN paises on " +
                    "empresas.idpais = paises.id INNER JOIN regiones on empresas.idregion = regiones.id";
            }

            DataTable dt = _databaseManager.ExecuteGlobalQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                empresas.Add(DataConverter.DataRowToEmpresaVM(row));
            }

            return empresas;
        }

        public List<EmpresasVM> GetEmpresasByUsuario(int userId)
        {
            List<EmpresasVM> empresas = new List<EmpresasVM>();
            string query = "SELECT * FROM empresas INNER JOIN usuarioempresa on usuarioempresa.idempresa = empresas.id WHERE" +
                " usuarioempresa.idusuario=@UserId";

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = _databaseManager.ExecuteGlobalQuery(query, sqlParameters);
            foreach (DataRow row in dt.Rows)
            {
                empresas.Add(DataConverter.DataRowToEmpresaVM(row));
            }

            return empresas;
        }
        public List<EmpresasVM> GetEmpresasByRut(string rut)
        {
            List<EmpresasVM> empresas = new List<EmpresasVM>();
            string query = "SELECT * FROM empresas  WHERE rut=@Rut";

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Rut", rut)
            };

            DataTable dt = _databaseManager.ExecuteGlobalQuery(query, sqlParameters);
            foreach (DataRow row in dt.Rows)
            {
                empresas.Add(DataConverter.DataRowToEmpresaVM(row));
            }
            return empresas;
        }

        public List<EmpresasVM> GetEmpresasHabilitadasByUsuario(int userId)
        {
            List<EmpresasVM> empresas = new List<EmpresasVM>();
            string query = "SELECT * FROM empresas INNER JOIN usuarioempresa on usuarioempresa.idempresa = empresas.id WHERE" +
                " usuarioempresa.idusuario=@UserId and empresas.habilitado=1";

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            DataTable dt = _databaseManager.ExecuteGlobalQuery(query, sqlParameters);
            foreach (DataRow row in dt.Rows)
            {
                empresas.Add(DataConverter.DataRowToEmpresaVM(row));
            }

            return empresas;
        }
    }
}
