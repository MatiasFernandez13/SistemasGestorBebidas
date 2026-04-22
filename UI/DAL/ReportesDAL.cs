using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace DAL
{
    public class ReportesDAL
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
        public DataTable ListarZonas()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_Clientes_ListarZonas", conn) { CommandType = CommandType.StoredProcedure })
            {
                conn.Open();
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }
        public DataTable ListarProductosBasicos()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_Productos_Listar_IdNombre", conn) { CommandType = CommandType.StoredProcedure })
            {
                conn.Open();
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }
        public DataTable GenerarReporte(string zona, int? productoId, DateTime? fecha)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_Reporte_Ventas", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Zona", (object)zona ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductoId", (object)productoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Fecha", (object)fecha ?? DBNull.Value);
                conn.Open();
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }
    }
}
