using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using BE;
namespace DAL
{
    public class CategoriaDAL
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MiConexion"].ConnectionString;
        public List<Categoria> Listar()
        {
            var lista = new List<Categoria>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    using (var cmd = new SqlCommand("sp_Categorias_Listar", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                lista.Add(new Categoria
                                {
                                    Id = Convert.ToInt32(rd["Id"]),
                                    Nombre = rd["Nombre"].ToString()
                                });
                            }
                        }
                    }
                }
                catch
                {
                    using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Categorias ORDER BY Nombre", conn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Categoria
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                Nombre = rd["Nombre"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }
        public void SeedDefault()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    var sql = @"
IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = N'Alcohólica')
    INSERT INTO Categorias (Nombre) VALUES (N'Alcohólica');
IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = N'No Alcohólica')
    INSERT INTO Categorias (Nombre) VALUES (N'No Alcohólica');";
                    using (var cmd = new SqlCommand(sql, conn))
                        cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CategoriaDAL.SeedDefault", ex);
                }
            }
        }
    }
}
