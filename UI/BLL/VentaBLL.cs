using BE;
using DAL;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class VentaBLL
    {
        private readonly VentaDAL    _ventaDAL    = new VentaDAL();
        private readonly ProductoDAL _productoDal = new ProductoDAL();

        public Venta ConfirmarVenta(List<VentaDetalle> carrito, int usuarioId)
        {
            if (carrito == null || !carrito.Any())
                throw new ArgumentException("El carrito está vacío.");

            var consolidados = carrito
                .GroupBy(d => d.ProductoId)
                .Select(g => new VentaDetalle
                {
                    ProductoId      = g.Key,
                    ProductoNombre  = g.Last().ProductoNombre,
                    Cantidad        = g.Sum(x => x.Cantidad),
                    PrecioUnitario  = g.Last().PrecioUnitario
                })
                .ToList();

            var venta = new Venta
            {
                Fecha      = DateTime.Now,
                UsuarioId  = usuarioId,
                Detalles   = consolidados
            };

            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                int ventaId = _ventaDAL.RegistrarVenta(venta, acceso);
                venta.Id = ventaId;

                foreach (var detalle in consolidados)
                {
                    Producto p = _productoDal.ObtenerPorId(detalle.ProductoId, acceso);
                    if (p != null)
                    {
                        p.DVH = DigitoVerificador.CalcularDVH(p);
                        _productoDal.Modificar(p, acceso);
                    }
                }

                acceso.RecalcularDVV("Productos");

                acceso.ConfirmarTransaccion();
                BitacoraHelper.Registrar("Venta", "Alta",
                    $"Venta #{ventaId} registrada por UsuarioId={usuarioId} " +
                    $"({consolidados.Count} producto(s))");

                return venta;
            }
            catch (Exception ex)
            {
                acceso.CancelarTransaccion();
                BitacoraHelper.Registrar("Venta", "Error",
                    $"Confirmar venta fallida: {MensajeCompleto(ex)}");
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        private static string MensajeCompleto(Exception ex) =>
            ex.InnerException != null
                ? $"{ex.Message} → {ex.InnerException.Message}"
                : ex.Message;
    }
}
