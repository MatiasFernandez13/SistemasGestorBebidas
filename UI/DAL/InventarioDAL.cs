using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class InventarioDAL
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;


        public List<Inventario> ObtenerStockPorProducto()
        {
            var lista = new List<Inventario>();


            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var query = @"
                        SELECT p.Nombre AS NombreProducto, p.CategoriaId,
                        CASE p.CategoriaId WHEN 1 THEN 'Alcohólica' ELSE 'No Alcohólica' END AS CategoriaNombre,
                        SUM(l.Cantidad) AS StockTotal
                        FROM Productos p
                        LEFT JOIN Lotes l ON p.Id = l.ProductoId
                        GROUP BY p.Nombre, p.CategoriaId";


                using (var cmd = new SqlCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Inventario
                        {
                            NombreProducto = reader["NombreProducto"].ToString(),
                            CategoriaId = (int)reader["CategoriaId"],
                            CategoriaNombre = reader["CategoriaNombre"].ToString(),
                            StockTotal = reader["StockTotal"] != DBNull.Value ? (int)reader["StockTotal"] : 0
                        });
                    }
                }
            }
            return lista;
        }
    }
}
