using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using BE;
using BE.Permisos;
namespace UI
{
    public partial class FrmPermisos : FrmBase
    {
        private readonly PermisoBLL _permisoBLL = new PermisoBLL();
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private readonly UsuarioPermisoBLL _usuarioPermisoBLL = new UsuarioPermisoBLL();
        private List<PermisoSimple> _simples = new List<PermisoSimple>();
        private List<GrupoPermiso> _grupos = new List<GrupoPermiso>();
        private List<Usuario> _usuarios = new List<Usuario>();
        private bool _updatingChecks = false;
        public FrmPermisos()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lstCompuestos.Format += (s, ev) =>
            {
                if (ev.ListItem is GrupoPermiso gp)
                    ev.Value = gp.Nombre.Equals("Administracion", StringComparison.OrdinalIgnoreCase) ? "Gestion Administracion" : gp.Nombre;
            };
            clbGruposAsignar.Format += (s, ev) =>
            {
                if (ev.ListItem is GrupoPermiso gp)
                    ev.Value = gp.Nombre.Equals("Administracion", StringComparison.OrdinalIgnoreCase) ? "Gestion Administracion" : gp.Nombre;
            };
            CargarDatos();
            var puedeAsignar = SERVICIOS.PermissionService.Has("Permisos.Asignar") || SERVICIOS.PermissionService.Has("AccesoPermisos");
            var puedeGestionar = SERVICIOS.PermissionService.Has("Permisos.Gestionar");
            foreach (TabPage tp in tabControl.TabPages)
            {
                if (tp.Tag?.ToString() == "Asignar") tp.Enabled = puedeAsignar;
                if (tp.Tag?.ToString() == "Gestionar") tp.Enabled = puedeGestionar;
            }
        }
        private void CargarDatos()
        {
            var borrar = new[]{
                "FullUser","GrupeteTest","ModElim","Pruebita","ReportesLectura","test1","Vender"
            };
            foreach (var n in borrar) _permisoBLL.EliminarGrupoPorNombre(n);
            _grupos = _permisoBLL.ObtenerGruposDePermisos().OfType<GrupoPermiso>().GroupBy(g=>g.Nombre).Select(g=>g.First()).ToList();
            var orden = new List<string>{
                "Gestion Usuarios","Gestion Productos","Gestion Venta","Gestion Reportes",
                "Gestion Idioma","Gestion Permisos","Administracion","Base","Vendedor","Supervisor","Administrador"
            };
            _grupos = _grupos
                .OrderBy(g => {
                    var idx = orden.FindIndex(o => o.Equals(g.Nombre, StringComparison.OrdinalIgnoreCase));
                    return idx >= 0 ? idx : int.MaxValue;
                })
                .ThenBy(g => g.Nombre)
                .ToList();
            _usuarios = _usuarioBLL.ObtenerTodos().Where(u=>u.Activo).ToList();
            lstCompuestos.DataSource = null;
            lstCompuestos.DataSource = _grupos;
            lstCompuestos.DisplayMember = "Nombre";
            ConstruirArbolPermisos();
            lstUsuarios.DataSource = null;
            lstUsuarios.DataSource = _usuarios;
            lstUsuarios.DisplayMember = "NombreUsuario";
            lstUsuarios.ValueMember = "Id";
            clbGruposAsignar.Items.Clear();
            foreach (var g in _grupos) clbGruposAsignar.Items.Add(g);
            clbGruposAsignar.DisplayMember = "Nombre";
            clbSimplesParaGrupo.Items.Clear();
            foreach (var g in _grupos) clbSimplesParaGrupo.Items.Add(g);
            clbSimplesParaGrupo.DisplayMember = "Nombre";
        }
        private void ConstruirArbolPermisos()
        {
            tvPermisos.BeginUpdate();
            tvPermisos.Nodes.Clear();
            var raiz = new TreeNode("Catálogo");
            var catalogo = new Dictionary<string, List<string>>
            {
                { "Usuarios", new List<string>{ "AccesoUsuarios","Usuarios.Alta","Usuarios.Modificar","Usuarios.Baja" } },
                { "Productos", new List<string>{ "AccesoProductos","Productos.Ver","Productos.Agregar","Productos.Modificar","Productos.Eliminar" } },
                { "Ventas", new List<string>{ "AccesoVentas","Ventas.Realizar" } },
                { "Reportes", new List<string>{ "AccesoReportes","Reportes.Ver","Reportes.Modificar","Reportes.Eliminar" } },
                { "Idiomas", new List<string>{ "AccesoIdiomas" } },
                { "Permisos", new List<string>{ "AccesoPermisos","Permisos.Asignar","Permisos.Gestionar" } },
                { "Administracion", new List<string>{ "AccesoAdministracion","AccesoBitacora","AccesoVerificarIntegridad","AccesoRecalcularIntegridad","AccesoControlCambios" } }
            };
            foreach (var kv in catalogo)
            {
                var nodoCat = new TreeNode(kv.Key);
                foreach (var nombre in kv.Value)
                {
                    nodoCat.Nodes.Add(new TreeNode(nombre){ Tag = new PermisoSimple{ Nombre = nombre } });
                }
                raiz.Nodes.Add(nodoCat);
            }
            tvPermisos.Nodes.Add(raiz);
            tvPermisos.ExpandAll();
            tvPermisos.EndUpdate();
        }
        private void tvPermisos_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_updatingChecks) return;
            foreach (TreeNode child in e.Node.Nodes)
                child.Checked = e.Node.Checked;
        }
        private void lstCompuestos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCompuestos.SelectedItem is GrupoPermiso grupoSel)
            {
                tvJerarquia.BeginUpdate();
                tvJerarquia.Nodes.Clear();
                var grpRoot = new TreeNode(grupoSel.Nombre);
                BuildHierarchyTree(grpRoot, grupoSel);
                tvJerarquia.Nodes.Add(grpRoot);
                tvJerarquia.ExpandAll();
                tvJerarquia.EndUpdate();
                var simplesGrupo = grupoSel.Hijos.OfType<PermisoSimple>().Select(h => h.Nombre).ToHashSet(StringComparer.OrdinalIgnoreCase);
                var subgrupos = grupoSel.Hijos.OfType<GrupoPermiso>().Select(g => g.Nombre).ToHashSet(StringComparer.OrdinalIgnoreCase);
                _updatingChecks = true;
                for (int i = 0; i < clbSimplesParaGrupo.Items.Count; i++)
                {
                    if (clbSimplesParaGrupo.Items[i] is GrupoPermiso g)
                        clbSimplesParaGrupo.SetItemChecked(i, subgrupos.Contains(g.Nombre));
                }
                _updatingChecks = false;
                var simplesHeredados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var sg in grupoSel.Hijos.OfType<GrupoPermiso>())
                    foreach (var ps in sg.Hijos.OfType<PermisoSimple>())
                        simplesHeredados.Add(ps.Nombre);
                _updatingChecks = true;
                tvPermisos.BeginUpdate();
                foreach (TreeNode catalogRoot in tvPermisos.Nodes)
                {
                    foreach (TreeNode n in catalogRoot.Nodes)
                    {
                        foreach (TreeNode c in n.Nodes)
                            c.Checked = simplesGrupo.Contains(c.Text) || simplesHeredados.Contains(c.Text);
                    }
                }
                tvPermisos.EndUpdate();
                _updatingChecks = false;
            }
        }
        private void BuildHierarchyTree(TreeNode parentNode, GrupoPermiso grupo)
        {
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            BuildHierarchyTreeInternal(parentNode, grupo, visited);
        }
        private void BuildHierarchyTreeInternal(TreeNode parentNode, GrupoPermiso grupo, HashSet<string> visited)
        {
            if (!visited.Add(grupo.Nombre)) return;
            foreach (var hijo in grupo.Hijos)
            {
                if (hijo is GrupoPermiso g)
                {
                    var n = new TreeNode(g.Nombre);
                    parentNode.Nodes.Add(n);
                    BuildHierarchyTreeInternal(n, g, visited);
                }
                else if (hijo is PermisoSimple s)
                {
                    parentNode.Nodes.Add(new TreeNode(s.Nombre));
                }
            }
        }
        private void btnCrearGrupo_Click(object sender, EventArgs e)
        {
            var nombre = txtGrupoNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese nombre del grupo.");
                return;
            }
            var seleccionados = new List<string>();
            foreach (TreeNode root in tvPermisos.Nodes)
            {
                foreach (TreeNode n in root.Nodes)
                {
                    foreach (TreeNode c in n.Nodes)
                    {
                        if (c.Checked && c.Tag is PermisoSimple ps)
                            seleccionados.Add(ps.Nombre);
                    }
                }
            }
            var subgruposSeleccionados = new List<GrupoPermiso>();
            foreach (var item in clbSimplesParaGrupo.CheckedItems)
            {
                if (item is GrupoPermiso gSel) subgruposSeleccionados.Add(gSel);
            }
            if (seleccionados.Count == 0)
            {
            }
            try
            {
                var grupo = new GrupoPermiso { Nombre = nombre };
                foreach (var nom in seleccionados) grupo.Agregar(new PermisoSimple { Nombre = nom });
                foreach (var sg in subgruposSeleccionados) grupo.Agregar(new GrupoPermiso { Nombre = sg.Nombre });
                _permisoBLL.CrearGrupoPermiso(grupo);
                CargarDatos();
                txtGrupoNombre.Clear();
                foreach (TreeNode root in tvPermisos.Nodes)
                {
                    foreach (TreeNode n in root.Nodes)
                        n.Checked = false;
                }
                for (int i = 0; i < clbSimplesParaGrupo.Items.Count; i++) clbSimplesParaGrupo.SetItemChecked(i, false);
                MessageBox.Show("Grupo creado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear grupo: " + ex.Message);
            }
        }
        private void btnGuardarGrupo_Click(object sender, EventArgs e)
        {
            if (lstCompuestos.SelectedItem is not GrupoPermiso grupoSel)
            {
                MessageBox.Show("Seleccione un grupo para actualizar.");
                return;
            }
            var seleccionados = new List<string>();
            foreach (TreeNode root in tvPermisos.Nodes)
            {
                foreach (TreeNode n in root.Nodes)
                {
                    foreach (TreeNode c in n.Nodes)
                    {
                        if (c.Checked && c.Tag is PermisoSimple ps)
                            seleccionados.Add(ps.Nombre);
                    }
                }
            }
            var subgruposSeleccionados = new List<GrupoPermiso>();
            foreach (var item in clbSimplesParaGrupo.CheckedItems)
            {
                if (item is GrupoPermiso g) subgruposSeleccionados.Add(g);
            }
            if (subgruposSeleccionados.Any(g => g.Nombre.Equals(grupoSel.Nombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("No se puede asignar el grupo como subgrupo de sí mismo. Se omitirá esa selección.");
                subgruposSeleccionados = subgruposSeleccionados
                    .Where(g => !g.Nombre.Equals(grupoSel.Nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            try
            {
                _permisoBLL.ActualizarGrupoSimples(grupoSel.Nombre, seleccionados);
                _permisoBLL.ActualizarGrupoSubgrupos(grupoSel.Nombre, subgruposSeleccionados.Select(g => g.Nombre).ToList());
                CargarDatos();
                MessageBox.Show("Grupo actualizado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar grupo: " + ex.Message);
            }
        }
        private void clbSimplesParaGrupo_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_updatingChecks) return;
            if (lstCompuestos.SelectedItem is not GrupoPermiso grupoSel) return;
            var toggledGrupo = clbSimplesParaGrupo.Items[e.Index] as GrupoPermiso;
            if (toggledGrupo != null && toggledGrupo.Nombre.Equals(grupoSel.Nombre, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("No se puede asignar un grupo como subgrupo de sí mismo.");
                e.NewValue = CheckState.Unchecked;
                return;
            }
            var selectedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < clbSimplesParaGrupo.Items.Count; i++)
            {
                if (i == e.Index)
                {
                    if (clbSimplesParaGrupo.Items[i] is GrupoPermiso g)
                    {
                        if (e.NewValue == CheckState.Checked) selectedNames.Add(g.Nombre);
                    }
                    continue;
                }
                if (clbSimplesParaGrupo.Items[i] is GrupoPermiso gi)
                {
                    if (clbSimplesParaGrupo.GetItemChecked(i)) selectedNames.Add(gi.Nombre);
                }
            }
            var simplesDirectos = grupoSel.Hijos.OfType<PermisoSimple>().Select(h => h.Nombre).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var simplesHeredados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var nombre in selectedNames)
            {
                var sg = _grupos.FirstOrDefault(g => g.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
                if (sg != null)
                {
                    foreach (var ps in sg.Hijos.OfType<PermisoSimple>())
                        simplesHeredados.Add(ps.Nombre);
                }
            }
            try
            {
                _permisoBLL.ActualizarGrupoSubgrupos(grupoSel.Nombre, selectedNames.ToList());
                if (toggledGrupo != null && e.NewValue == CheckState.Unchecked)
                {
                    var confirm = MessageBox.Show(
                        $"¿Quitar los permisos del subgrupo \"{toggledGrupo.Nombre}\" del grupo \"{grupoSel.Nombre}\"?",
                        "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes)
                    {
                        e.NewValue = CheckState.Checked;
                        return;
                    }
                    var removedSimples = _grupos
                        .FirstOrDefault(g => g.Nombre.Equals(toggledGrupo.Nombre, StringComparison.OrdinalIgnoreCase))?
                        .Hijos.OfType<PermisoSimple>().Select(ps => ps.Nombre).ToHashSet(StringComparer.OrdinalIgnoreCase)
                        ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    var otrosHeredados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var nombre in selectedNames)
                    {
                        var sg = _grupos.FirstOrDefault(g => g.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
                        if (sg != null)
                        {
                            foreach (var ps in sg.Hijos.OfType<PermisoSimple>())
                                otrosHeredados.Add(ps.Nombre);
                        }
                    }
                    var directos = grupoSel.Hijos.OfType<PermisoSimple>().Select(ps => ps.Nombre).ToList();
                    var nuevosDirectos = directos.Where(n => !removedSimples.Contains(n) || otrosHeredados.Contains(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    _permisoBLL.ActualizarGrupoSimples(grupoSel.Nombre, nuevosDirectos);
                    simplesDirectos = nuevosDirectos.ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar subgrupos: " + ex.Message);
            }
            _updatingChecks = true;
            tvPermisos.BeginUpdate();
            foreach (TreeNode catalogRoot in tvPermisos.Nodes)
            {
                foreach (TreeNode n in catalogRoot.Nodes)
                {
                    foreach (TreeNode c in n.Nodes)
                    {
                        var on = simplesDirectos.Contains(c.Text) || simplesHeredados.Contains(c.Text);
                        c.Checked = on;
                    }
                }
            }
            tvPermisos.EndUpdate();
            _updatingChecks = false;
        }
        private void btnEliminarGrupo_Click(object sender, EventArgs e)
        {
            if (lstCompuestos.SelectedItem is not GrupoPermiso grupoSel)
            {
                MessageBox.Show("Seleccione un grupo para eliminar.");
                return;
            }
            var r = MessageBox.Show($"¿Eliminar el grupo \"{grupoSel.Nombre}\"?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;
            try
            {
                _permisoBLL.EliminarGrupoPorNombre(grupoSel.Nombre);
                CargarDatos();
                MessageBox.Show("Grupo eliminado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar grupo: " + ex.Message);
            }
        }
        private void lstUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedItem is Usuario u)
            {
                var idsAsignados = new UsuarioPermisoBLL().ObtenerGrupos(u.Id);
                for (int i = 0; i < clbGruposAsignar.Items.Count; i++)
                {
                    if (clbGruposAsignar.Items[i] is GrupoPermiso g)
                        clbGruposAsignar.SetItemChecked(i, idsAsignados.Contains(g.Id));
                }
                RenderUserHierarchy(idsAsignados);
            }
        }
        private void btnGuardarAsignacion_Click(object sender, EventArgs e)
        {
            if (lstUsuarios.SelectedItem is not Usuario usuario)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }
            var seleccionados = new List<int>();
            foreach (var item in clbGruposAsignar.CheckedItems)
            {
                if (item is GrupoPermiso g) seleccionados.Add(g.Id);
            }
            try
            {
                _usuarioPermisoBLL.AsignarGrupos(usuario.Id, seleccionados);
                MessageBox.Show("Permisos asignados.");
                RenderUserHierarchy(seleccionados);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permisos: " + ex.Message);
            }
        }
        private void RenderUserHierarchy(List<int> idsAsignados)
        {
            tvJerarquiaUsuario.BeginUpdate();
            tvJerarquiaUsuario.Nodes.Clear();
            var dict = _grupos.ToDictionary(g => g.Id);
            foreach (var id in idsAsignados)
            {
                if (dict.TryGetValue(id, out var g))
                {
                    var root = new TreeNode(g.Nombre);
                    BuildHierarchyTree(root, g);
                    tvJerarquiaUsuario.Nodes.Add(root);
                }
            }
            tvJerarquiaUsuario.ExpandAll();
            tvJerarquiaUsuario.EndUpdate();
        }
    }
}
