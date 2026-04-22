using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class UsuarioPermisoDAL
    {

        public void AsignarGruposAUsuario(int idUsuario, List<int> idGrupos, ACCESO acceso)
        {
            acceso.EscribirSQL(
                "DELETE FROM Usuario_Permiso WHERE IdUsuario=@U",
                new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@U", idUsuario)
                });

            foreach (var idGrupo in idGrupos)
            {
                acceso.EscribirSQL(
                    "INSERT INTO Usuario_Permiso (IdUsuario, IdPermiso) VALUES (@U, @P)",
                    new List<System.Data.SqlClient.SqlParameter>
                    {
                        acceso.CrearParametro("@U", idUsuario),
                        acceso.CrearParametro("@P", idGrupo)
                    });
            }
        }

        public List<int> ObtenerGruposDeUsuario(int idUsuario)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@U", idUsuario)
                };
                DataTable dt = acceso.LeerSQL(
                    "SELECT IdPermiso FROM Usuario_Permiso WHERE IdUsuario=@U", parametros);

                var lista = new List<int>();
                foreach (DataRow fila in dt.Rows)
                    lista.Add(Convert.ToInt32(fila["IdPermiso"]));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en UsuarioPermisoDAL.ObtenerGruposDeUsuario", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
