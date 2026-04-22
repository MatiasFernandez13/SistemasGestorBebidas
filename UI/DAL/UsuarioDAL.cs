using BE;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class UsuarioDAL
    {
        public int Insertar(Usuario usuario, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@NombreUsuario", usuario.NombreUsuario),
                acceso.CrearParametro("@Contraseña",    usuario.ContraseñaHasheada),
                acceso.CrearParametro("@Salt",          usuario.Salt ?? string.Empty),
                acceso.CrearParametro("@Idioma",        usuario.Idioma ?? "es"),
                acceso.CrearParametro("@Activo",        usuario.Activo),
                acceso.CrearParametro("@DVH",           usuario.DVH)
            };

            try
            {
                object resultado = acceso.EscribirEscalar("sp_Usuario_Insertar", parametros);
                return Convert.ToInt32(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioDAL.Insertar", ex);
            }
        }

        public void Modificar(Usuario usuario, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Id",            usuario.Id),
                acceso.CrearParametro("@NombreUsuario", usuario.NombreUsuario),
                acceso.CrearParametro("@Contraseña",    usuario.ContraseñaHasheada),
                acceso.CrearParametro("@Salt",          usuario.Salt ?? string.Empty),
                acceso.CrearParametro("@Idioma",        usuario.Idioma ?? "es"),
                acceso.CrearParametro("@Activo",        usuario.Activo),
                acceso.CrearParametro("@DVH",           usuario.DVH)
            };

            try
            {
                acceso.Escribir("sp_Usuario_Modificar", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioDAL.Modificar", ex);
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
                acceso.Escribir("sp_Usuario_BajaLogica", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioDAL.Eliminar", ex);
            }
        }

        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@NombreUsuario", nombreUsuario)
                };

                DataTable tabla = acceso.Leer("sp_Usuario_ObtenerPorNombre", parametros);
                return tabla.Rows.Count > 0 ? Mapear(tabla.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioDAL.ObtenerPorNombreUsuario", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public List<Usuario> ObtenerTodos()
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                DataTable tabla = acceso.Leer("sp_Usuario_ObtenerTodos");
                var lista = new List<Usuario>();
                foreach (DataRow fila in tabla.Rows)
                    lista.Add(Mapear(fila));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioDAL.ObtenerTodos", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void GuardarIdioma(int idUsuario, string idioma)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@IdUsuario", idUsuario),
                    acceso.CrearParametro("@Idioma",    idioma)
                };
                acceso.Escribir("sp_Usuario_GuardarIdioma", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioDAL.GuardarIdioma", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
        private static Usuario Mapear(DataRow fila)
        {
            var usuario = new Usuario
            {
                Id                 = Convert.ToInt32(fila["Id"]),
                NombreUsuario      = fila["NombreUsuario"].ToString(),
                ContraseñaHasheada = fila["Contraseña"].ToString(),
                Salt               = fila["Salt"]   != DBNull.Value ? fila["Salt"].ToString()              : string.Empty,
                Idioma             = fila["Idioma"]  != DBNull.Value ? fila["Idioma"].ToString()            : "es",
                Activo             = fila["Activo"]  != DBNull.Value && Convert.ToBoolean(fila["Activo"]),
                DVH                = fila["DVH"]     != DBNull.Value ? Convert.ToDecimal(fila["DVH"])       : 0
            };
            return usuario;
        }
    }
}
