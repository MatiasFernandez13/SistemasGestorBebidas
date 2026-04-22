using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class BitacoraDAL
    {
        public void Insertar(Bitacora b)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@UsuarioId",     SqlDbType.Int)           { Value = b.UsuarioId > 0 ? (object)b.UsuarioId : DBNull.Value },
                    new SqlParameter("@FechaRegistro", SqlDbType.DateTime)      { Value = b.FechaRegistro },
                    new SqlParameter("@Entidad",       SqlDbType.NVarChar, 200) { Value = (object)b.Entidad  ?? DBNull.Value },
                    new SqlParameter("@Accion",        SqlDbType.NVarChar, 100) { Value = (object)b.Accion   ?? DBNull.Value },
                    new SqlParameter("@Detalle",       SqlDbType.NVarChar, 500) { Value = (object)b.Detalle  ?? DBNull.Value }
                };
                acceso.Escribir("sp_Bitacora_Insertar", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BitacoraDAL.Insertar", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string usuario, string accion)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@Desde",   SqlDbType.DateTime)      { Value = (object)desde   ?? DBNull.Value },
                    new SqlParameter("@Hasta",   SqlDbType.DateTime)      { Value = (object)hasta   ?? DBNull.Value },
                    new SqlParameter("@Usuario", SqlDbType.NVarChar, 200) { Value = (object)usuario ?? DBNull.Value },
                    new SqlParameter("@Accion",  SqlDbType.NVarChar, 100) { Value = (object)accion  ?? DBNull.Value }
                };

                DataTable dt = acceso.Leer("sp_Bitacora_Buscar", parametros);
                var lista = new List<Bitacora>();
                foreach (DataRow fila in dt.Rows)
                {
                    lista.Add(new Bitacora
                    {
                        Id            = Convert.ToInt32(fila["Id"]),
                        UsuarioId     = fila["UsuarioId"]     != DBNull.Value ? Convert.ToInt32(fila["UsuarioId"])        : 0,
                        UsuarioNombre = fila["NombreUsuario"] != DBNull.Value ? fila["NombreUsuario"].ToString() : string.Empty,
                        FechaRegistro = Convert.ToDateTime(fila["FechaRegistro"]),
                        Entidad       = fila["Entidad"].ToString(),
                        Accion        = fila["Accion"].ToString(),
                        Detalle       = fila["Detalle"].ToString()
                    });
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BitacoraDAL.Buscar", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
