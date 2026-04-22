using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using BE.Permisos;
namespace SERVICIOS
{
    public static class PermissionService
    {
        private static HashSet<string> _permisos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly UsuarioPermisoDAL _usuarioPermisoDal = new UsuarioPermisoDAL();
        private static readonly PermisoDAL _permisoDal = new PermisoDAL();
        public static void RefreshForCurrentUser()
        {
            _permisos.Clear();
            var usuario = Sesion.Instancia.UsuarioLogueado;
            if (usuario == null) return;

            var ids = _usuarioPermisoDal.ObtenerGruposDeUsuario(usuario.Id);
            var gruposAll = _permisoDal.ObtenerGruposDePermisos().OfType<GrupoPermiso>().ToDictionary(g => g.Id);
            var compuestos = new List<IPermiso>();
            
            bool esAdmin = false;

            foreach (var id in ids)
            {
                if (gruposAll.TryGetValue(id, out var g))
                {
                    if (g.Nombre.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                    {
                        esAdmin = true;
                    }
                    compuestos.Add(g);
                    _permisos.Add(g.Nombre);
                    foreach (var s in CollectSimplesSafe(g))
                        _permisos.Add(s.Nombre);
                }
            }

            if (esAdmin)
            {
                foreach (var g in gruposAll.Values)
                {
                    compuestos.Add(g);
                    _permisos.Add(g.Nombre);
                    foreach (var h in CollectSimplesSafe(g))
                        _permisos.Add(h.Nombre);
                }
            }
            Sesion.Instancia.PermisosCompuestos = compuestos;
            Sesion.Instancia.Permisos = new HashSet<string>(_permisos, StringComparer.OrdinalIgnoreCase);
        }
        private static IEnumerable<PermisoSimple> CollectSimplesSafe(GrupoPermiso grupo)
        {
            return CollectSimplesInternal(grupo, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }
        private static IEnumerable<PermisoSimple> CollectSimplesInternal(GrupoPermiso grupo, HashSet<string> visited)
        {
            if (!visited.Add(grupo.Nombre)) yield break;
            foreach (var h in grupo.Hijos)
            {
                if (h is PermisoSimple s) yield return s;
                else if (h is GrupoPermiso g)
                {
                    foreach (var sp in CollectSimplesInternal(g, visited))
                        yield return sp;
                }
            }
        }
        public static bool Has(string permisoNombre)
        {
            return _permisos.Contains(permisoNombre);
        }
        public static IEnumerable<string> All() => _permisos.ToArray();
        public static IEnumerable<IPermiso> Tree() => Sesion.Instancia.PermisosCompuestos?.ToArray() ?? Array.Empty<IPermiso>();
    }
}

