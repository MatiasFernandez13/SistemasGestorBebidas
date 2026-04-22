using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ProductoHistorialDAL
    {
        private const string SQL_INSERT = @"
            INSERT INTO ProductoHistorial
                (IdProducto, Nombre, CategoriaId, Precio, LitrosPorUnidad, Stock, Activo, Accion)
            VALUES
                (@IdProducto, @Nombre, @CategoriaId, @Precio, @Litros, @Stock, @Activo, @Accion)";

        private const string SQL_LISTAR = @"
            SELECT IdHistorial, Fecha, Nombre, CategoriaId, Precio, LitrosPorUnidad, Stock, Activo
            FROM   ProductoHistorial
            WHERE  IdProducto = @Id
            ORDER  BY Fecha DESC";

        public void InsertarSnapshot(Producto p, string accion)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@IdProducto", SqlDbType.Int)           { Value = p.Id },
                    new SqlParameter("@Nombre",     SqlDbType.NVarChar, 200) { Value = (object)p.Nombre ?? DBNull.Value },
                    new SqlParameter("@CategoriaId",SqlDbType.Int)           { Value = p.Categoria },
                    new SqlParameter("@Precio",     SqlDbType.Decimal)       { Value = p.Precio },
                    new SqlParameter("@Litros",     SqlDbType.Float)         { Value = p.LitrosPorUnidad },
                    new SqlParameter("@Stock",      SqlDbType.Int)           { Value = p.Stock },
                    new SqlParameter("@Activo",     SqlDbType.Bit)           { Value = p.Activo },
                    new SqlParameter("@Accion",     SqlDbType.NVarChar, 20)  { Value = (object)accion ?? "Modificar" }
                };
                acceso.EscribirSQL(SQL_INSERT, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en ProductoHistorialDAL.InsertarSnapshot", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<(int IdHistorial, DateTime Fecha, Producto Snapshot)> ListarPorProducto(int idProducto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.Int) { Value = idProducto }
                };
                DataTable dt = acceso.LeerSQL(SQL_LISTAR, parametros);

                var lista = new List<(int, DateTime, Producto)>();
                foreach (DataRow rd in dt.Rows)
                {
                    var p = new Producto
                    {
                        Id             = idProducto,
                        Nombre         = rd["Nombre"].ToString(),
                        Categoria      = rd["CategoriaId"]      != DBNull.Value ? Convert.ToInt32(rd["CategoriaId"])   : 0,
                        Precio         = rd["Precio"]           != DBNull.Value ? Convert.ToDecimal(rd["Precio"])      : 0,
                        LitrosPorUnidad= rd["LitrosPorUnidad"]  != DBNull.Value ? Convert.ToDouble(rd["LitrosPorUnidad"]) : 0,
                        Stock          = rd["Stock"]            != DBNull.Value ? Convert.ToInt32(rd["Stock"])         : 0,
                        Activo         = rd["Activo"]           != DBNull.Value && Convert.ToBoolean(rd["Activo"])
                    };
                    lista.Add((Convert.ToInt32(rd["IdHistorial"]), Convert.ToDateTime(rd["Fecha"]), p));
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en ProductoHistorialDAL.ListarPorProducto", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
