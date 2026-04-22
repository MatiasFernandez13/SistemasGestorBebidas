using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BLL;
using BE;
namespace UI
{
    public partial class FrmIdiomas : FrmBase
    {
        private readonly IdiomaBLL _idiomaBLL = new IdiomaBLL();
        private readonly IdiomaAdminBLL _adminBLL = new IdiomaAdminBLL();
        private List<IdiomaDTO> _idiomas = new List<IdiomaDTO>();
        private DataTable _tags;
        private bool _modoNuevo = false;
        private bool _dirty = false;
        public FrmIdiomas()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarIdiomas();
            CargarTagsParaIdiomaActual();
            btnGuardar.Enabled = false;
        }
        private void CargarIdiomas()
        {
            _idiomas = _idiomaBLL.ObtenerIdiomas();
            cbIdiomas.DataSource = _idiomas;
            cbIdiomas.DisplayMember = "Nombre";
            cbIdiomas.ValueMember = "Codigo";
        }
        private void CargarTagsParaIdiomaActual()
        {
            _tags = _adminBLL.ListarTags();
            dgvTags.DataSource = null;
            var codigo = cbIdiomas.SelectedValue?.ToString();
            var traducciones = string.IsNullOrEmpty(codigo) ? new Dictionary<string,string>() : _idiomaBLL.ObtenerTraducciones(codigo);
            var dt = new DataTable();
            dt.Columns.Add("IdTag", typeof(int));
            dt.Columns.Add("Tag", typeof(string));
            dt.Columns.Add("Traduccion", typeof(string));
            foreach (DataRow r in _tags.Rows)
            {
                var id = r.Field<int>("Id");
                var tag = r.Field<string>("Nombre");
                var tr = traducciones.TryGetValue(tag, out var t) ? t : "";
                dt.Rows.Add(id, tag, tr);
            }
            dgvTags.AutoGenerateColumns = true;
            dgvTags.DataSource = dt;
            try
            {
                foreach (DataGridViewColumn col in dgvTags.Columns)
                    col.ReadOnly = !string.Equals(col.DataPropertyName, "Traduccion", StringComparison.OrdinalIgnoreCase);
                if (dgvTags.Columns.Contains("IdTag")) dgvTags.Columns["IdTag"].Visible = false;
            }
            catch { }
            dgvTags.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            AjustarColumnasGrid(dgvTags, "Traduccion");
            AplicarEstilo(dgvTags);
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
            grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.RowTemplate.Height = 30;
            grid.MultiSelect = false;
        }
        private void cbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTagsParaIdiomaActual();
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            _modoNuevo = true;
            cbIdiomas.Enabled = false;
            btnGuardar.Enabled = true;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = true;
            _dirty = false;
            txtCodigo.Clear();
            txtNombre.Clear();
            dgvTags.DataSource = null;
            _tags = _adminBLL.ListarTags();
            var dt = new DataTable();
            dt.Columns.Add("IdTag", typeof(int));
            dt.Columns.Add("Tag", typeof(string));
            dt.Columns.Add("Traduccion", typeof(string));
            foreach (DataRow r in _tags.Rows)
                dt.Rows.Add(r.Field<int>("Id"), r.Field<string>("Nombre"), "");
            dgvTags.AutoGenerateColumns = true;
            dgvTags.DataSource = dt;
            try
            {
                foreach (DataGridViewColumn col in dgvTags.Columns)
                    col.ReadOnly = !string.Equals(col.DataPropertyName, "Traduccion", StringComparison.OrdinalIgnoreCase);
                if (dgvTags.Columns.Contains("IdTag")) dgvTags.Columns["IdTag"].Visible = false;
            }
            catch { }
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_modoNuevo)
            {
                var codigo = txtCodigo.Text.Trim();
                var nombre = txtNombre.Text.Trim();
                if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("Código y Nombre son obligatorios.");
                    return;
                }
                var dict = new Dictionary<int,string>();
                foreach (DataGridViewRow row in dgvTags.Rows)
                {
                    var idTag = (int)row.Cells["IdTag"].Value;
                    var traduccion = (row.Cells["Traduccion"].Value ?? "").ToString();
                    if (!string.IsNullOrWhiteSpace(traduccion))
                        dict[idTag] = traduccion;
                }
                try
                {
                    var idIdioma = _adminBLL.CrearIdiomaConTraducciones(codigo, nombre, dict);
                    MessageBox.Show("Idioma creado. Las traducciones ingresadas fueron guardadas. Puede completar el resto luego.");
                    _modoNuevo = false;
                    cbIdiomas.Enabled = true;
                    btnGuardar.Enabled = false;
                    btnEliminar.Enabled = true;
                    btnCancelar.Enabled = false;
                    CargarIdiomas();
                    cbIdiomas.SelectedValue = codigo;
                    if (this.MdiParent is FrmPrincipal principal)
                        principal.RecargarIdiomasMenu(codigo);
                    txtCodigo.Clear();
                    txtNombre.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear idioma: " + ex.Message);
                }
            }
            else
            {
                var codigo = cbIdiomas.SelectedValue?.ToString();
                if (string.IsNullOrEmpty(codigo)) return;
                var idIdioma = _adminBLL.ObtenerIdIdiomaPorCodigo(codigo);
                if (idIdioma <= 0) return;
                var dict = new Dictionary<int, string>();
                foreach (DataGridViewRow row in dgvTags.Rows)
                {
                    var idTag = (int)row.Cells["IdTag"].Value;
                    var traduccion = (row.Cells["Traduccion"].Value ?? "").ToString();
                    dict[idTag] = traduccion;
                }
                try
                {
                    _adminBLL.GuardarTodasLasTraducciones(idIdioma, dict);
                    _dirty = false;
                    btnGuardar.Enabled = false;
                    MessageBox.Show("Traducciones actualizadas.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar traducciones: " + ex.Message);
                }
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var codigo = cbIdiomas.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Seleccione un idioma.");
                return;
            }
            if (MessageBox.Show("¿Eliminar idioma y sus traducciones?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _adminBLL.EliminarIdioma(codigo);
                    MessageBox.Show("Idioma eliminado.");
                    CargarIdiomas();
                    CargarTagsParaIdiomaActual();
                    if (this.MdiParent is FrmPrincipal principal)
                        principal.RecargarIdiomasMenu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar idioma: " + ex.Message);
                }
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            _modoNuevo = false;
            cbIdiomas.Enabled = true;
            btnGuardar.Enabled = false;
            btnEliminar.Enabled = true;
            btnCancelar.Enabled = false;
            _dirty = false;
            txtCodigo.Clear();
            txtNombre.Clear();
            CargarTagsParaIdiomaActual();
        }
        private void dgvTags_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_modoNuevo) return;
            var codigo = cbIdiomas.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(codigo)) return;
            var idIdioma = _adminBLL.ObtenerIdIdiomaPorCodigo(codigo);
            if (idIdioma <= 0) return;
            var row = dgvTags.Rows[e.RowIndex];
            var idTag = (int)row.Cells["IdTag"].Value;
            var traduccion = (row.Cells["Traduccion"].Value ?? "").ToString();
            try
            {
                _dirty = true;
                btnGuardar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar traducción: " + ex.Message);
            }
        }
    }
}
