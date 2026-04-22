using BE;
using BE.Permisos;
using BLL;
using SERVICIOS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace UI
{
    public partial class FrmUsuarios : FrmBase, IObservadorIdioma
    {
        private readonly PermisoBLL _permisoBLL = new PermisoBLL();
        private readonly UsuarioPermisoBLL _usuarioPermisoBLL = new UsuarioPermisoBLL();
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private Usuario _usuarioSeleccionado;
        private bool _modoAgregar = false;
        private bool _mostrarEliminados = false;

        public FrmUsuarios()
        {
            InitializeComponent();
            IdiomaService.Suscribir(this);
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
            ConfigurarEstadoInicial();
            btnAgregar.Enabled = PermissionService.Has("Usuarios.Alta");
            btnModificar.Enabled = PermissionService.Has("Usuarios.Modificar");
            btnEliminar.Enabled = PermissionService.Has("Usuarios.Baja");
        }

        private void FrmUsuarios_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaService.Desuscribir(this);
        }

        public void ActualizarIdioma(Dictionary<string, string> traducciones)
        {
            foreach (Control control in this.Controls)
            {
                if (control.Tag != null)
                {
                    string tag = control.Tag.ToString();
                    if (traducciones.ContainsKey(tag))
                        control.Text = traducciones[tag];
                }
            }

            if (traducciones.ContainsKey("FrmUsuarios"))
                this.Text = traducciones["FrmUsuarios"];
        }
        private void CargarUsuarios()
        {
            dgvUsuarios.DataSource = null;
            var usuarios = _usuarioBLL.ObtenerTodos();
            var fuente = _mostrarEliminados ? usuarios : usuarios.Where(u => u.Activo).ToList();

            var todosGrupos = _permisoBLL.ObtenerGruposDePermisos().OfType<GrupoPermiso>().ToList();

            dgvUsuarios.DataSource = fuente.Select(u => {
                var gruposIds = _usuarioPermisoBLL.ObtenerGrupos(u.Id);
                var nombresGrupos = todosGrupos
                    .Where(g => gruposIds.Contains(g.Id))
                    .Select(g => g.Nombre)
                    .ToList();
                
                u.Permisos = nombresGrupos.Any() ? string.Join(", ", nombresGrupos) : "Sin permisos";

                return new
                {
                    u.Id,
                u.NombreUsuario,
                Rol = u.Permisos,
                u.Activo
                };
            }).ToList();

            dgvUsuarios.ClearSelection();
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (dgvUsuarios.Columns.Contains("Id")) dgvUsuarios.Columns["Id"].HeaderText = "ID";
            if (dgvUsuarios.Columns.Contains("Activo")) dgvUsuarios.Columns["Activo"].HeaderText = "Activo";
            AjustarColumnasGrid(dgvUsuarios, "NombreUsuario");
            AplicarEstilo(dgvUsuarios);
        }
        private void AjustarColumnasGrid(DataGridView grid, string fillColumnName = null)
        {
            if (grid?.Columns == null) return;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (!string.IsNullOrEmpty(fillColumnName) && string.Equals(col.Name, fillColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    col.FillWeight = 60;
                }
                else
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }
        private void AplicarEstilo(DataGridView grid)
        {
            if (grid == null) return;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.RowTemplate.Height = 30;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
        }

        private void ConfigurarEstadoInicial()
        {
            txtNombreUsuario.Enabled = false;
            txtContraseña.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGrabar.Visible = false;
            btnCancelar.Visible = false;
            dgvUsuarios.ClearSelection();
        }

        private void HabilitarCamposEdicion(bool habilitar)
        {
            txtNombreUsuario.Enabled = habilitar;
            txtContraseña.Enabled = habilitar;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _modoAgregar = true;
            HabilitarCamposEdicion(true);
            btnGrabar.Visible = true;
            btnCancelar.Visible = true;
            btnAgregar.Enabled = false;
            LimpiarCampos();
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            dgvUsuarios.Enabled = false;
            btnGrabar.Enabled = true;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombreUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            var nuevoUsuario = new Usuario
            {
                NombreUsuario = txtNombreUsuario.Text,
                Permisos = string.Empty 
            };

            try
            {
                _usuarioBLL.AgregarUsuario(nuevoUsuario, txtContraseña.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar usuario: " + ex.Message);
                return;
            }

            MessageBox.Show("Usuario agregado exitosamente.");
            MessageBox.Show("Recuerde asignar los permisos del usuario desde el menú de permisos.");
            LimpiarCampos();
            CargarUsuarios();
            ConfigurarEstadoInicial();
            btnAgregar.Enabled = true;
            HabilitarCamposEdicion(false);
            dgvUsuarios.Enabled = true;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario para modificar.");
                return;
            }

            btnGrabar.Enabled = false;
            btnCancelar.Enabled = true;
            btnModificar.Enabled = false;
            _usuarioSeleccionado.NombreUsuario = txtNombreUsuario.Text;

            string nuevaPass = string.IsNullOrEmpty(txtContraseña.Text) ? null : txtContraseña.Text;
            _usuarioBLL.ModificarUsuario(_usuarioSeleccionado, nuevaPass);

            MessageBox.Show("Usuario modificado correctamente.");
            LimpiarCampos();
            CargarUsuarios();
            ConfigurarEstadoInicial();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.");
                return;
            }

            var r = MessageBox.Show(
                $"¿Está seguro que desea eliminar el usuario '{_usuarioSeleccionado.NombreUsuario}'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;
            _usuarioBLL.EliminarUsuario(_usuarioSeleccionado.Id);

            MessageBox.Show("Usuario eliminado correctamente.");
            LimpiarCampos();
            CargarUsuarios();
            ConfigurarEstadoInicial();
            dgvUsuarios.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            ConfigurarEstadoInicial();
            btnAgregar.Enabled = true;
            dgvUsuarios.Enabled = true;
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;
            btnGrabar.Enabled = false;
            btnCancelar.Enabled = false;
            CargarUsuarios();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                var fila = dgvUsuarios.SelectedRows[0];
                int id = (int)fila.Cells["Id"].Value;
                _usuarioSeleccionado = _usuarioBLL.ObtenerTodos().First(u => u.Id == id);
                txtNombreUsuario.Text = _usuarioSeleccionado.NombreUsuario;

                btnModificar.Enabled = true;
                btnEliminar.Enabled = true;
                btnCancelar.Visible = true;
                btnCancelar.Enabled = true;
                btnGrabar.Visible=true;
                txtNombreUsuario.Enabled = true;
                txtContraseña.Enabled = true;
                txtContraseña.Clear();
                btnReactivar.Enabled = _mostrarEliminados && _usuarioSeleccionado != null && !_usuarioSeleccionado.Activo;
            }
            else
            {

            }
        }

        private void LimpiarCampos()
        {
            txtNombreUsuario.Clear();
            txtContraseña.Clear();
            _usuarioSeleccionado = null;
        }

        private void cbRol_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            
        }
        private void btnToggleEliminados_Click(object sender, EventArgs e)
        {
            _mostrarEliminados = !_mostrarEliminados;
            btnToggleEliminados.Text = _mostrarEliminados ? "Ocultar eliminados" : "Mostrar eliminados";
            btnToggleEliminados.Tag = _mostrarEliminados ? "OcultarEliminados" : "MostrarEliminados";
            btnReactivar.Visible = _mostrarEliminados;
            btnReactivar.Enabled = false;
            CargarUsuarios();
        }
        private void btnReactivar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un usuario eliminado.");
                return;
            }
            if (_usuarioSeleccionado.Activo)
            {
                MessageBox.Show("El usuario ya está activo.");
                return;
            }
            var r = MessageBox.Show(
                $"¿Reactivar el usuario '{_usuarioSeleccionado.NombreUsuario}'?",
                "Confirmar reactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;
            _usuarioSeleccionado.Activo = true;
            _usuarioBLL.ModificarUsuario(_usuarioSeleccionado, null);
            MessageBox.Show("Usuario reactivado.");
            CargarUsuarios();
            btnReactivar.Enabled = false;
        }
    }


}
