using BE;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class LoteBLL
    {
        private readonly LoteDAL _loteDAL = new LoteDAL();

        public void Agregar(Lote lote)
        {
            if (lote == null)
                throw new ArgumentNullException(nameof(lote));
            if (string.IsNullOrWhiteSpace(lote.NumeroLote))
                throw new ArgumentException("El número de lote no puede estar vacío.");
            if (lote.Cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                _loteDAL.Agregar(lote, acceso);
                _loteDAL.ActualizarStockEnProducto(lote.ProductoId, acceso);
                acceso.ConfirmarTransaccion();
            }
            catch
            {
                acceso.CancelarTransaccion();
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Lote> ListarPorProducto(int productoId)
        {
            return _loteDAL.ListarPorProducto(productoId);
        }

        public void Eliminar(int loteId, int productoId)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                _loteDAL.Eliminar(loteId, acceso);
                _loteDAL.ActualizarStockEnProducto(productoId, acceso);
                acceso.ConfirmarTransaccion();
            }
            catch
            {
                acceso.CancelarTransaccion();
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public int CalcularStockTotal(int productoId)
        {
            return _loteDAL.CalcularStockTotal(productoId);
        }

        public void ActualizarCantidad(int loteId, int nuevaCantidad)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                _loteDAL.ActualizarCantidad(loteId, nuevaCantidad, acceso);
                acceso.ConfirmarTransaccion();
            }
            catch
            {
                acceso.CancelarTransaccion();
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
