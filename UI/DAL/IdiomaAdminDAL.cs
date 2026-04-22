using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class IdiomaAdminDAL
    {

        public int InsertarIdioma(string codigo, string nombre, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Codigo", codigo),
                acceso.CrearParametro("@Nombre", nombre)
            };
            object resultado = acceso.EscribirEscalar("sp_Idiomas_Insertar", parametros);
            return resultado != null && resultado != DBNull.Value ? Convert.ToInt32(resultado) : 0;
        }

        public void EliminarIdioma(string codigo, ACCESO acceso)
        {
            acceso.EscribirSQL(
                "IF NOT EXISTS (SELECT 1 FROM Idiomas WHERE Codigo='es') " +
                "INSERT INTO Idiomas (Codigo, Nombre) VALUES ('es', 'Español');",
                null);

            var paramsUpd = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Codigo", codigo)
            };
            acceso.EscribirSQL(
                "UPDATE Usuarios SET Idioma='es' WHERE Idioma=@Codigo AND @Codigo <> 'es';",
                paramsUpd);


            var paramsDel = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@Codigo", codigo)
            };
            acceso.Escribir("sp_Idiomas_Eliminar", paramsDel);
        }

        public void GuardarTraduccion(int idIdioma, int idTag, string traduccion, ACCESO acceso)
        {
            var parametros = new List<System.Data.SqlClient.SqlParameter>
            {
                acceso.CrearParametro("@IdIdioma",   idIdioma),
                acceso.CrearParametro("@IdTag",      idTag),
                acceso.CrearParametro("@Traduccion", traduccion)
            };
            acceso.Escribir("sp_Traduccion_Upsert", parametros);
        }

        public DataTable ListarTags()
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                return acceso.Leer("sp_Tag_Listar");
            }
            catch (Exception ex)
            {
                throw new Exception("Error en IdiomaAdminDAL.ListarTags", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public int ObtenerIdIdiomaPorCodigo(string codigo)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                var parametros = new List<System.Data.SqlClient.SqlParameter>
                {
                    acceso.CrearParametro("@Codigo", codigo)
                };
                DataTable dt = acceso.LeerSQL(
                    "SELECT Id FROM Idiomas WHERE Codigo=@Codigo", parametros);
                return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["Id"]) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en IdiomaAdminDAL.ObtenerIdIdiomaPorCodigo", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        public void AsegurarTags(IEnumerable<string> tags)
        {
            ACCESO acceso = new ACCESO();
            acceso.Abrir();
            try
            {
                foreach (var t in tags)
                {
                    var parametros = new List<System.Data.SqlClient.SqlParameter>
                    {
                        acceso.CrearParametro("@Nombre", t)
                    };
                    acceso.EscribirSQL(
                        "IF NOT EXISTS (SELECT 1 FROM Tag WHERE Nombre=@Nombre) " +
                        "INSERT INTO Tag (Nombre) VALUES (@Nombre);",
                        parametros);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en IdiomaAdminDAL.AsegurarTags", ex);
            }
            finally
            {
                acceso.Cerrar();
            }
        }
    }
}
