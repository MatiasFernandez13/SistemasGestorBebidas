using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE;
using BLL;
using System.Data.SqlClient;

namespace UI
{
    public partial class FrmProductos : FrmBase
    {
        private ProductoBLL _productoBLL = new ProductoBLL();
        private LoteBLL _loteBLL = new LoteBLL();
        private Producto _productoSeleccionado = null;
        private bool _mostrarEliminados = false;

        public FrmProductos()
        {
            InitializeComponent();
        }

        private void FrmProductos_Load1(object sender, EventArgs e)
        {
            CargarGrilla();
            CargarComboCategorias();
            txtNombre.MaxLength = 100;
            if (cbCategoria.Items.Count == 0)
            {
                MessageBox.Show("No se encontraron categorías para cargar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CambiarEstadoCampos(true);
            btnAgregar.Enabled = SERVICIOS.PermissionService.Has("Productos.Agregar");
            btnModificar.Enabled = SERVICIOS.PermissionService.Has("Productos.Modificar");
            btnEliminar.Enabled = SERVICIOS.PermissionService.Has("Productos.Eliminar");
            btnToggleEliminados.Enabled = SERVICIOS.PermissionService.Has("Productos.Ver");
        }

        private void CargarGrilla()
        {
            dgvProductos.DataSource = null;
            var productos = _productoBLL.Listar();
            dgvProductos.DataSource = _mostrarEliminados ? productos : productos.Where(p => p.Activo).ToList();
            dgvProductos.ClearSelection();
            if (dgvProductos.Columns.Contains("DVH"))
                dgvProductos.Columns["DVH"].Visible = false;
            if (dgvProductos.Columns.Contains("Id"))
                dgvProductos.Columns["Id"].HeaderText = "ID";
            if (dgvProductos.Columns.Contains("Categoria"))
                dgvProductos.Columns["Categoria"].Visible = false;
            if (dgvProductos.Columns.Contains("CategoriaNombre"))
            {
                dgvProductos.Columns["CategoriaNombre"].HeaderText = "Categoría";
                dgvProductos.Columns["CategoriaNombre"].DisplayIndex = 2;
            }
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AjustarColumnasGrid(dgvProductos, "Nombre");
            AplicarEstilo(dgvProductos);
            
            ConfigurarAutocomplete(productos);
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

        private void ConfigurarAutocomplete(List<Producto> productos)
        {
            try
            {
                var nombres = productos.Select(p => p.Nombre).Where(n => !string.IsNullOrEmpty(n)).ToArray();
                txtNombre.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtNombre.AutoCompleteSource = AutoCompleteSource.CustomSource;
                var source = new AutoCompleteStringCollection();
                source.AddRange(nombres);
                txtNombre.AutoCompleteCustomSource = source;
            }
            catch { }
        }

       
        private void CargarComboCategorias()
        {
            var categoriaBLL = new BLL.CategoriaBLL();
            var categorias = categoriaBLL.ObtenerCategorias();
            cbCategoria.DataSource = categorias;
            cbCategoria.DisplayMember = "Nombre";
            cbCategoria.ValueMember = "Id";
            cbCategoria.SelectedIndex = -1; 
        }

        private void CambiarEstadoCampos(bool habilitar)
        {
            txtNombre.ReadOnly = !habilitar;
            cbCategoria.Enabled = habilitar;
            txtLitrosPorUnidad.ReadOnly = !habilitar;
            txtStock.ReadOnly = !habilitar; 
            txtPrecio.ReadOnly = !habilitar;
            
            txtLote.ReadOnly = !habilitar;
            dtpFechaIngreso.Enabled = habilitar;
            dtpFechaVencimiento.Enabled = habilitar;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            cbCategoria.SelectedIndex = -1;
            txtLitrosPorUnidad.Clear();
            txtStock.Clear();
            txtPrecio.Clear();
            
            txtLote.Clear();
            dtpFechaIngreso.Value = DateTime.Now;
            dtpFechaVencimiento.Value = DateTime.Now.AddMonths(1);

            _productoSeleccionado = null;
            CambiarEstadoCampos(true);
            
            btnAgregar.Visible = true;
            btnModificar.Enabled = false;
            btnGrabar.Visible = false;
            btnCancelar.Visible = false;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;
            
            if (string.IsNullOrWhiteSpace(txtLote.Text))
            {
                MessageBox.Show("Debe ingresar un número de lote para el stock inicial.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtStock.Text) || !int.TryParse(txtStock.Text, out int cantidad) || cantidad <= 0)
            {
                 MessageBox.Show("Debe ingresar una cantidad válida (Stock) mayor a 0.");
                 return;
            }

            var productosExistentes = _productoBLL.Listar();
            
            int categoriaId = cbCategoria.SelectedValue != null ? Convert.ToInt32(cbCategoria.SelectedValue) : 0;
            double litros = double.Parse(txtLitrosPorUnidad.Text.Replace('.', ','));
            
            var productoExistente = productosExistentes.FirstOrDefault(p => 
                p.Nombre.Equals(txtNombre.Text.Trim(), StringComparison.OrdinalIgnoreCase) && 
                p.Categoria == categoriaId &&
                Math.Abs(p.LitrosPorUnidad - litros) < 0.001 && 
                p.Activo);

            if (productoExistente != null)
            {
                var result = MessageBox.Show(
                    $"Ya existe un producto con el mismo Nombre, Categoría y Capacidad ('{productoExistente.Nombre}').\n¿Desea agregar este lote al stock del producto existente?",
                    "Producto Existente",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        productoExistente.Precio = decimal.Parse(txtPrecio.Text.Replace('.', ','));

                        var nuevoLote = new Lote
                        {
                            NumeroLote = txtLote.Text.Trim(),
                            FechaIngreso = dtpFechaIngreso.Value,
                            FechaVencimiento = dtpFechaVencimiento.Value,
                            Cantidad = cantidad,
                            ProductoId = productoExistente.Id,
                            Activo = true
                        };

                        _productoBLL.Modificar(productoExistente, nuevoLote);
                        MessageBox.Show("Stock y lote agregados al producto existente con éxito.");
                        CargarGrilla();
                        LimpiarCampos();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al actualizar producto existente: " + ex.Message);
                        return;
                    }
                }
                else
                {
                     return; 
                }
            }

            var nuevoProducto = new Producto
            {
                Nombre = txtNombre.Text.Trim(),
                Categoria = cbCategoria.SelectedValue != null ? Convert.ToInt32(cbCategoria.SelectedValue) : 0,
                LitrosPorUnidad = double.Parse(txtLitrosPorUnidad.Text.Replace('.', ',')),
                Stock = 0, 
                Precio = decimal.Parse(txtPrecio.Text.Replace('.', ','))
            };

            try
            {
                var nuevoLote = new Lote
                {
                    NumeroLote = txtLote.Text.Trim(),
                    FechaIngreso = dtpFechaIngreso.Value,
                    FechaVencimiento = dtpFechaVencimiento.Value,
                    Cantidad = cantidad,
                    Activo = true
                };

                _productoBLL.AgregarConLote(nuevoProducto, nuevoLote);
                MessageBox.Show("Producto y stock inicial agregados con éxito.");
                CargarGrilla();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.Message + " | " + ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error al agregar producto: {msg}");
            }
        }
        
        private void btnAgregarUnidades_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null) return;

            var frm = new FrmAgregarLote(_productoSeleccionado.Id, _productoSeleccionado.Nombre);
            frm.ShowDialog();
            
            if (frm.LoteAgregado)
            {
                CargarGrilla();
            }
        }

        private void btnVerLotes_Click(object sender, EventArgs e)
        {
             if (_productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto para ver sus lotes.");
                return;
            }
            
            var frm = new FrmLotes(_productoSeleccionado.Id);
            frm.ShowDialog();
            CargarGrilla();
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null || !ValidarCampos()) return;


            try
            {
                _productoSeleccionado.Nombre = txtNombre.Text.Trim();
                _productoSeleccionado.Categoria = (int)cbCategoria.SelectedValue;
                _productoSeleccionado.LitrosPorUnidad = double.Parse(txtLitrosPorUnidad.Text.Replace('.', ','));
                _productoSeleccionado.Precio = decimal.Parse(txtPrecio.Text.Replace('.', ','));

                Lote loteActualizar = null;
                if (!string.IsNullOrWhiteSpace(txtLote.Text))
                {
                     if (string.IsNullOrWhiteSpace(txtStock.Text) || !int.TryParse(txtStock.Text, out int cantidad) || cantidad <= 0)
                    {
                        MessageBox.Show("Debe ingresar una cantidad válida para el lote.");
                        return;
                    }
                    
                    loteActualizar = new Lote
                    {
                        NumeroLote = txtLote.Text.Trim(),
                        FechaIngreso = dtpFechaIngreso.Value,
                        FechaVencimiento = dtpFechaVencimiento.Value,
                        Cantidad = cantidad,
                        ProductoId = _productoSeleccionado.Id,
                        Activo = true
                    };
                }

                _productoBLL.Modificar(_productoSeleccionado, loteActualizar);
                MessageBox.Show("Producto actualizado con éxito.");
                CargarGrilla();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar producto: {ex.Message}");
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _productoSeleccionado = (Producto)dgvProductos.CurrentRow.DataBoundItem;
                txtNombre.Text = _productoSeleccionado.Nombre;
                cbCategoria.SelectedValue = _productoSeleccionado.Categoria;
                txtLitrosPorUnidad.Text = _productoSeleccionado.LitrosPorUnidad.ToString();
                txtStock.Text = "";
                txtStock.PlaceholderText = "Cant. Lote";
                txtPrecio.Text = _productoSeleccionado.Precio.ToString();

                CambiarEstadoCampos(false);
                
                txtLote.Clear();
                dtpFechaIngreso.Value = DateTime.Now;
                dtpFechaVencimiento.Value = DateTime.Now.AddMonths(1);

                btnAgregar.Visible = false;
                btnModificar.Enabled = true;
                btnAgregarStock.Visible = true; 
                btnGrabar.Visible = false;
                btnCancelar.Visible = true;
                
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null) return;
            CambiarEstadoCampos(true);
            btnGrabar.Visible = true;
            btnModificar.Enabled = false;
            btnAgregarStock.Visible = false; 
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto para eliminar.");
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _productoBLL.Eliminar(_productoBLL.ObtenerPorId(_productoSeleccionado.Id));
                    CargarGrilla();
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    var msg = ex.InnerException != null ? ex.Message + " | " + ex.InnerException.Message : ex.Message;
                    MessageBox.Show("Error al eliminar (baja lógica): " + msg);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            CargarGrilla();
        }
        private void btnToggleEliminados_Click(object sender, EventArgs e)
        {
            _mostrarEliminados = !_mostrarEliminados;
            btnToggleEliminados.Text = _mostrarEliminados ? "Ocultar eliminados" : "Mostrar eliminados";
            btnToggleEliminados.Tag = _mostrarEliminados ? "OcultarEliminados" : "MostrarEliminados";
            CargarGrilla();
        }

        private void FrmProductos_Load_1(object sender, EventArgs e)
        {
            CargarGrilla();
            CargarComboCategorias();
            txtNombre.MaxLength = 100;
            if (cbCategoria.Items.Count == 0)
            {
                MessageBox.Show("No se encontraron categorías para cargar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CambiarEstadoCampos(true);
            btnAgregar.Enabled = SERVICIOS.PermissionService.Has("Productos.Agregar");
            btnModificar.Enabled = SERVICIOS.PermissionService.Has("Productos.Modificar");
            btnEliminar.Enabled = SERVICIOS.PermissionService.Has("Productos.Eliminar");
            btnToggleEliminados.Enabled = SERVICIOS.PermissionService.Has("Productos.Ver");
            btnAgregarStock.Enabled = SERVICIOS.PermissionService.Has("Productos.Modificar"); 
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                cbCategoria.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtLitrosPorUnidad.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return false;
            }

            if (!double.TryParse(txtLitrosPorUnidad.Text.Replace('.', ','), out double capacidad))
            {
                MessageBox.Show("Capacidad debe ser un número válido.");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Replace('.', ','), out decimal precio))
            {
                MessageBox.Show("Precio debe ser un número decimal válido.");
                return false;
            }

            return true;
        }
    }
}
