using BE;
using DAL;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class ProductoBLL
    {
        private readonly ProductoDAL        _productoDal   = new ProductoDAL();
        private readonly LoteDAL            _loteDal       = new LoteDAL();
        private readonly ProductoHistorialBLL _historialBll = new ProductoHistorialBLL();

        public List<Producto> Listar()
        {
            try
            {
                return _productoDal.ListarTodos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar productos", ex);
            }
        }

        public Producto ObtenerPorId(int id)
        {
            try
            {
                return _productoDal.ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el producto con Id={id}", ex);
            }
        }

        public void Agregar(Producto producto)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                producto.DVH = DigitoVerificador.CalcularDVH(producto);
                int idGenerado = _productoDal.Insertar(producto, acceso);

                producto.Id  = idGenerado;
                producto.DVH = DigitoVerificador.CalcularDVH(producto);
                _productoDal.Modificar(producto, acceso);

                acceso.RecalcularDVV("Productos");

                acceso.ConfirmarTransaccion();
                BitacoraHelper.Registrar("Producto", "Alta",
                    $"Se agregó el producto '{producto.Nombre}' (Id={producto.Id})");
            }
            catch (Exception ex)
            {
                acceso.CancelarTransaccion();
                BitacoraHelper.Registrar("Producto", "Error",
                    $"Alta fallida para '{producto.Nombre}': {MensajeCompleto(ex)}");
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Alta con lote inicial
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Inserta el producto y su primer lote en una única transacción:
        ///   1. Inserta Producto (DVH provisional con Id=0)
        ///   2. Actualiza DVH con el Id real asignado por la BD
        ///   3. Inserta el Lote asociado
        ///   4. Sincroniza Stock en Producto desde la suma de lotes
        ///   5. Recalcula DVH final (con stock actualizado) y persiste
        ///   6. Recalcula DVV de la tabla Productos
        /// </summary>
        public void AgregarConLote(Producto producto, Lote lote)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                // ── Paso 1: insertar producto ─────────────────────────────
                producto.DVH = DigitoVerificador.CalcularDVH(producto);
                int idGenerado = _productoDal.Insertar(producto, acceso);
                producto.Id  = idGenerado;

                // ── Paso 2: insertar lote ─────────────────────────────────
                lote.ProductoId = idGenerado;
                _loteDal.Agregar(lote, acceso);

                // ── Paso 3: sincronizar stock en Producto desde lotes ─────
                _loteDal.ActualizarStockEnProducto(idGenerado, acceso);

                // ── Paso 4: recalcular DVH con Id y Stock reales ──────────
                Producto productoPersistido = _productoDal.ObtenerPorId(idGenerado, acceso);
                productoPersistido.DVH = DigitoVerificador.CalcularDVH(productoPersistido);
                _productoDal.Modificar(productoPersistido, acceso);

                // ── Paso 5: actualizar DVV de la tabla ────────────────────
                acceso.RecalcularDVV("Productos");

                acceso.ConfirmarTransaccion();
                BitacoraHelper.Registrar("Producto", "Alta con Lote",
                    $"Se agregó '{producto.Nombre}' (Id={idGenerado}) con lote '{lote.NumeroLote}'");
            }
            catch (Exception ex)
            {
                acceso.CancelarTransaccion();
                BitacoraHelper.Registrar("Producto", "Error",
                    $"Alta con lote fallida para '{producto.Nombre}': {MensajeCompleto(ex)}");
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Modificación
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Modifica un producto, registrando su estado anterior como snapshot.
        /// Si se provee un lote, lo agrega o actualiza según corresponda.
        /// Todo en una única transacción.
        /// </summary>
        public void Modificar(Producto producto, Lote lote = null)
        {
            // El snapshot se registra antes de abrir la transacción principal
            // (es auditoría: debe quedar aunque la modificación falle)
            Producto estadoAnterior = _productoDal.ObtenerPorId(producto.Id);
            if (estadoAnterior != null)
                _historialBll.RegistrarSnapshot(estadoAnterior, "Modificar");

            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                // ── Modificar datos del producto ──────────────────────────
                producto.DVH = DigitoVerificador.CalcularDVH(producto);
                _productoDal.Modificar(producto, acceso);

                // ── Gestión de lote (opcional) ────────────────────────────
                if (lote != null)
                {
                    List<Lote> lotesExistentes = _loteDal.ListarPorProducto(producto.Id, acceso);
                    Lote loteExistente = lotesExistentes.FirstOrDefault(
                        l => l.NumeroLote == lote.NumeroLote);

                    if (loteExistente != null)
                        _loteDal.ActualizarCantidad(loteExistente.Id, lote.Cantidad, acceso);
                    else
                        _loteDal.Agregar(lote, acceso);

                    _loteDal.ActualizarStockEnProducto(producto.Id, acceso);

                    // Recalcular DVH con el stock actualizado
                    Producto productoActualizado = _productoDal.ObtenerPorId(producto.Id, acceso);
                    productoActualizado.DVH = DigitoVerificador.CalcularDVH(productoActualizado);
                    _productoDal.Modificar(productoActualizado, acceso);
                }

                acceso.RecalcularDVV("Productos");

                acceso.ConfirmarTransaccion();
                BitacoraHelper.Registrar("Producto", "Modificación",
                    $"Se modificó el producto '{producto.Nombre}' (Id={producto.Id})");
            }
            catch (Exception ex)
            {
                acceso.CancelarTransaccion();
                BitacoraHelper.Registrar("Producto", "Error",
                    $"Modificación fallida para Id={producto.Id}: {MensajeCompleto(ex)}");
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Baja lógica
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Realiza la baja lógica del producto, registrando un snapshot previo.
        /// </summary>
        public void Eliminar(Producto objeto)
        {
            Producto estadoAnterior = _productoDal.ObtenerPorId(objeto.Id);
            if (estadoAnterior != null)
                _historialBll.RegistrarSnapshot(estadoAnterior, "Eliminar");

            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                _productoDal.Eliminar(objeto, acceso);
                acceso.RecalcularDVV("Productos");

                acceso.ConfirmarTransaccion();
                BitacoraHelper.Registrar("Producto", "Baja lógica",
                    $"Se dio de baja el producto (Id={objeto.Id})");
            }
            catch (Exception ex)
            {
                acceso.CancelarTransaccion();
                BitacoraHelper.Registrar("Producto", "Error",
                    $"Baja fallida para Id={objeto.Id}: {MensajeCompleto(ex)}");
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Reversión a estado anterior (desde historial)
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Revierte un producto a un snapshot previo del historial.
        /// </summary>
        public void RevertirA(Producto snapshot)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            acceso.IniciarTransaccion();
            try
            {
                snapshot.DVH = DigitoVerificador.CalcularDVH(snapshot);
                _productoDal.Modificar(snapshot, acceso);
                acceso.RecalcularDVV("Productos");

                acceso.ConfirmarTransaccion();
                BitacoraHelper.Registrar("Producto", "Rollback",
                    $"Se revirtió el producto (Id={snapshot.Id}) a estado anterior");
            }
            catch (Exception ex)
            {
                acceso.CancelarTransaccion();
                BitacoraHelper.Registrar("Producto", "Error",
                    $"Rollback fallido para Id={snapshot.Id}: {MensajeCompleto(ex)}");
                throw;
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Helpers privados
        // ═══════════════════════════════════════════════════════════════════

        private static string MensajeCompleto(Exception ex)
        {
            return ex.InnerException != null
                ? $"{ex.Message} → {ex.InnerException.Message}"
                : ex.Message;
        }
    }
}
