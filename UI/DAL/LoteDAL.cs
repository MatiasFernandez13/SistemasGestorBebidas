using BE;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class LoteDAL
    {
        public void Agregar(Lote lote, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@NumeroLote",lote.NumeroLote),
                acceso.CrearParametro("@FechaIngreso",lote.FechaIngreso),
                acceso.CrearParametro("@FechaVencimiento",lote.FechaVencimiento),
                acceso.CrearParametro("@Cantidad",lote.Cantidad),
                acceso.CrearParametro("@ProductoId",lote.ProductoId)
            };

            try
            {
                acceso.Escribir("sp_Lote_Agregar", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LoteDAL.Agregar", ex);
            }
        }

        public void ActualizarStockEnProducto(int productoId, ACCESO acceso)
        {
            try
            {
                int stockTotal = CalcularStockTotal(productoId, acceso);

                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@ProductoId", productoId),
                    acceso.CrearParametro("@Stock", stockTotal)
                };
                acceso.Escribir("sp_Lote_ActualizarStockProducto", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LoteDAL.ActualizarStockEnProducto", ex);
            }
        }

        public void ActualizarCantidad(int loteId, int nuevaCantidad, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Cantidad", nuevaCantidad),
                acceso.CrearParametro("@Id",loteId)
            };

            try
            {
                acceso.EscribirSQL(
                    "UPDATE Lote SET Cantidad = @Cantidad WHERE Id = @Id",
                    parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LoteDAL.ActualizarCantidad", ex);
            }
        }

        public void Eliminar(int id, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Id", id)
            };

            try
            {
                acceso.Escribir("sp_Lote_Eliminar", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LoteDAL.Eliminar", ex);
            }
        }

        public List<Lote> ListarPorProducto(int productoId, ACCESO acceso = null)
        {
            bool propioAcceso = acceso == null;
            if (propioAcceso)
            {
                acceso = new ACCESO();
                acceso.Abrir();
            }

            try
            {
                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@ProductoId", productoId)
                };

                DataTable tabla = acceso.Leer("sp_Lote_ListarPorProducto", parametros);
                var lista = new List<Lote>();
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(MapearLote(fila));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LoteDAL.ListarPorProducto", ex);
            }
            finally
            {
                if (propioAcceso) acceso.Cerrar();
            }
        }

        public int CalcularStockTotal(int productoId, ACCESO acceso = null)
        {
            bool propioAcceso = acceso == null;
            if (propioAcceso)
            {
                acceso = new ACCESO();
                acceso.Abrir();
            }

            try
            {
                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@ProductoId", productoId)
                };

                object resultado = acceso.EscribirEscalar("sp_Lote_CalcularStockTotal", parametros);
                return resultado != null && resultado != DBNull.Value
                    ? Convert.ToInt32(resultado)
                    : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en LoteDAL.CalcularStockTotal", ex);
            }
            finally
            {
                if (propioAcceso) acceso.Cerrar();
            }
        }

        private static Lote MapearLote(DataRow fila)
        {
            return new Lote
            {
                Id= Convert.ToInt32(fila["Id"]),
                NumeroLote= fila["NumeroLote"].ToString(),
                FechaIngreso= Convert.ToDateTime(fila["FechaIngreso"]),
                FechaVencimiento = Convert.ToDateTime(fila["FechaVencimiento"]),
                Cantidad= Convert.ToInt32(fila["Cantidad"]),
                ProductoId= Convert.ToInt32(fila["ProductoId"])
            };
        }
    }
}
