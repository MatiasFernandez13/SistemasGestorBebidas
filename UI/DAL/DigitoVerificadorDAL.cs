using System;
using System.Collections.Generic;

namespace DAL
{
    /// <summary>
    /// Encapsula las operaciones de base de datos del Dígito Verificador.
    /// Separa la responsabilidad de persistencia de la lógica de cálculo
    /// que permanece en SERVICIOS.DigitoVerificador.
    /// </summary>
    public static class DigitoVerificadorDAL
    {
        /// <summary>
        /// Recalcula y persiste el DVV de la tabla indicada dentro de la
        /// transacción activa del ACCESO provisto por la BLL.
        /// Llama a sp_RecalcularDVV que realiza internamente:
        ///   1. SUM(DVH) de la tabla
        ///   2. SUM % primo → DVV
        ///   3. UPSERT en DigitoVerificador
        /// </summary>
        public static void RecalcularDVV(string tabla, ACCESO acceso)
        {
            if (string.IsNullOrWhiteSpace(tabla))
                throw new ArgumentException("El nombre de tabla es obligatorio.", nameof(tabla));

            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Tabla", tabla)
            };

            try
            {
                acceso.Escribir("sp_RecalcularDVV", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recalcular DVV de la tabla '{tabla}'", ex);
            }
        }
    }
}
