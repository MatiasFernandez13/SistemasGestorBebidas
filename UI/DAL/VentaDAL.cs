using BE;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class VentaDAL
    {
        public int RegistrarVenta(Venta venta, ACCESO acceso)
        {
            try
            {
                var paramsVenta = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@Fecha",     venta.Fecha),
                    acceso.CrearParametro("@UsuarioId", venta.UsuarioId)
                };

                int ventaId = Convert.ToInt32(acceso.EscribirEscalar("sp_Ventas_Insertar", paramsVenta));

                foreach (VentaDetalle detalle in venta.Detalles)
                {
                    var paramsDetalle = new List<System.Data.SqlClient.SqlParameter>
                    {
                        acceso.CrearParametro("@VentaId",        ventaId),
                        acceso.CrearParametro("@ProductoId",     detalle.ProductoId),
                        acceso.CrearParametro("@Cantidad",       detalle.Cantidad),
                        acceso.CrearParametro("@PrecioUnitario", detalle.PrecioUnitario)
                    };
                    acceso.Escribir("sp_DetalleVentas_Upsert", paramsDetalle);

                    DescontarLotesFIFO(detalle.ProductoId, detalle.Cantidad, acceso);

                    int stockActual = CalcularStockTotalLotes(detalle.ProductoId, acceso);
                    var paramsStock = new List<System.Data.SqlClient.SqlParameter>
                    {
                        acceso.CrearParametro("@ProductoId", detalle.ProductoId),
                        acceso.CrearParametro("@Stock",      stockActual)
                    };
                    acceso.Escribir("sp_Lote_ActualizarStockProducto", paramsStock);
                }

                return ventaId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VentaDAL.RegistrarVenta", ex);
            }
        }

        private static void DescontarLotesFIFO(int productoId, int cantidadVendida, ACCESO acceso)
        {
            var paramsLotes = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@ProductoId", productoId)
            };

            DataTable lotes = acceso.Leer("sp_Lote_ListarPorProducto", paramsLotes);

            int restante = cantidadVendida;

            foreach (DataRow fila in lotes.Rows)
            {
                if (restante <= 0) break;

                int loteId       = Convert.ToInt32(fila["Id"]);
                int cantidadLote = Convert.ToInt32(fila["Cantidad"]);
                int aDescontar   = Math.Min(restante, cantidadLote);

                var paramsUpd = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@Cantidad", aDescontar),
                    acceso.CrearParametro("@Id",       loteId)
                };
                acceso.EscribirSQL(
                    "UPDATE Lote SET Cantidad = Cantidad - @Cantidad WHERE Id = @Id",
                    paramsUpd);

                restante -= aDescontar;
            }

            if (restante > 0)
                throw new Exception(
                    $"Stock insuficiente en lotes para el producto Id={productoId}. " +
                    $"Faltan {restante} unidades.");
        }

        private static int CalcularStockTotalLotes(int productoId, ACCESO acceso)
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
    }
}
