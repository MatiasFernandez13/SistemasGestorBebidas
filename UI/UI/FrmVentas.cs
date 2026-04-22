using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SERVICIOS;

namespace UI
{
    public partial class FrmVentas : FrmBase
    {
        private ProductoBLL _productoBLL = new ProductoBLL();
        private VentaBLL _ventaBLL;
        private LoteBLL _loteBLL = new LoteBLL();
        private List<VentaDetalle> _carrito = new List<VentaDetalle>();

        public FrmVentas()
        {
            InitializeComponent();
            _ventaBLL = new VentaBLL();
        }

        private void FrmVentas_Load(object sender, EventArgs e)
        {
            CargarProductos();
            ActualizarCarrito();
        }

        private void CargarProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = _productoBLL.Listar().Where(p => p.Activo).ToList();
            dgvProductos.ClearSelection();
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.RowHeadersVisible = false;
            if (dgvProductos.Columns.Contains("DVH"))
                dgvProductos.Columns["DVH"].Visible = false;
            if (dgvProductos.Columns.Contains("Id"))
                dgvProductos.Columns["Id"].HeaderText = "ID";
            if (dgvProductos.Columns.Contains("Nombre"))
                dgvProductos.Columns["Nombre"].HeaderText = "Producto";
            if (dgvProductos.Columns.Contains("Categoria"))
                dgvProductos.Columns["Categoria"].Visible = false;
            if (dgvProductos.Columns.Contains("CategoriaNombre"))
            {
                dgvProductos.Columns["CategoriaNombre"].HeaderText = "Categoría";
                dgvProductos.Columns["CategoriaNombre"].DisplayIndex = 2;
            }
            if (dgvProductos.Columns.Contains("LitrosPorUnidad"))
            {
                dgvProductos.Columns["LitrosPorUnidad"].HeaderText = "Litros/Unidad";
                dgvProductos.Columns["LitrosPorUnidad"].DefaultCellStyle.Format = "N2";
            }
            if (dgvProductos.Columns.Contains("Precio"))
                dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "N2";
            foreach (DataGridViewColumn col in dgvProductos.Columns)
                col.ReadOnly = true;
            AjustarColumnasGrid(dgvProductos, "Nombre");
            AplicarEstilo(dgvProductos);
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductos.SelectedRows.Count > 0)
                {
                    var producto = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
                    int cantidad = (int)nudCantidad.Value;
                    var lotesActivos = _loteBLL.ListarPorProducto(producto.Id);
                    if (lotesActivos == null || lotesActivos.Count == 0)
                    {
                        MessageBox.Show("Este producto no tiene lotes activos. Cree al menos un lote antes de vender.");
                        return;
                    }

                    var existente = _carrito.FirstOrDefault(x => x.ProductoId == producto.Id);
                    int cantidadActual = existente != null ? existente.Cantidad : 0;
                    if (cantidad + cantidadActual > producto.Stock)
                    {
                        MessageBox.Show("No hay suficiente stock disponible para acumular esa cantidad.");
                        return;
                    }

                    if (existente != null)
                    {
                        existente.Cantidad += cantidad;
                        existente.PrecioUnitario = producto.Precio;
                    }
                    else
                    {
                        _carrito.Add(new VentaDetalle
                        {
                            ProductoId = producto.Id,
                            ProductoNombre = producto.Nombre,
                            Cantidad = cantidad,
                            PrecioUnitario = producto.Precio
                        });
                    }

                    ActualizarCarrito();
                    CalcularTotal();
                    nudCantidad.Value = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar producto: {ex.Message}");
            }
        }
        private void btnCancelarCarrito_Click(object sender, EventArgs e)
        {
            _carrito.Clear();
            ActualizarCarrito();
            CalcularTotal();
            dgvProductos.ClearSelection();
            dgvCarrito.ClearSelection();
            nudCantidad.Value = 1;
        }

        private void ActualizarCarrito()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = _carrito;
            dgvCarrito.ClearSelection();
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            dgvCarrito.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvCarrito.AllowUserToAddRows = false;
            dgvCarrito.AllowUserToDeleteRows = false;
            dgvCarrito.ReadOnly = true;
            dgvCarrito.RowHeadersVisible = false;
            dgvCarrito.SelectionChanged -= dgvCarrito_SelectionChanged;
            dgvCarrito.SelectionChanged += dgvCarrito_SelectionChanged;
            if (dgvCarrito.Columns.Contains("ProductoId"))
                dgvCarrito.Columns["ProductoId"].Visible = false;
            if (dgvCarrito.Columns.Contains("ProductoNombre"))
                dgvCarrito.Columns["ProductoNombre"].HeaderText = "Producto";
            if (dgvCarrito.Columns.Contains("Cantidad"))
                dgvCarrito.Columns["Cantidad"].HeaderText = "Cantidad";
            if (dgvCarrito.Columns.Contains("PrecioUnitario"))
                dgvCarrito.Columns["PrecioUnitario"].DefaultCellStyle.Format = "N2";
            if (dgvCarrito.Columns.Contains("Subtotal"))
                dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
            if (dgvCarrito.Columns.Contains("ProductoNombre")) dgvCarrito.Columns["ProductoNombre"].DisplayIndex = 0;
            if (dgvCarrito.Columns.Contains("Cantidad")) dgvCarrito.Columns["Cantidad"].DisplayIndex = 1;
            if (dgvCarrito.Columns.Contains("PrecioUnitario")) dgvCarrito.Columns["PrecioUnitario"].DisplayIndex = 2;
            if (dgvCarrito.Columns.Contains("Subtotal")) dgvCarrito.Columns["Subtotal"].DisplayIndex = 3;
            foreach (DataGridViewColumn col in dgvCarrito.Columns)
                col.ReadOnly = true;
            AjustarColumnasGrid(dgvCarrito);
            AplicarEstilo(dgvCarrito);
            UpdateRemoveButtonState();
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
        private void dgvCarrito_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRemoveButtonState();
        }
        private void UpdateRemoveButtonState()
        {
            btnQuitarItem.Enabled = dgvCarrito.SelectedRows.Count > 0;
        }
        private void btnQuitarItem_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.SelectedRows.Count == 0) return;
            var item = dgvCarrito.SelectedRows[0].DataBoundItem as VentaDetalle;
            if (item == null) return;
            _carrito.RemoveAll(x => x.ProductoId == item.ProductoId);
            ActualizarCarrito();
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            decimal total = 0;
            foreach (var item in _carrito)
            {
                total += item.PrecioUnitario * item.Cantidad;
            }
            lblTotal.Text = $"Total: ${total}";
        }

        private void btnVerLotes_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto para ver sus lotes.");
                return;
            }

            var producto = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
            
            var frm = new FrmLotes(producto.Id);
            frm.ShowDialog();
        }

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                int usuarioId = Sesion.Instancia.UsuarioLogueado.Id;
                var venta = _ventaBLL.ConfirmarVenta(_carrito, usuarioId);

                var detalle = new StringBuilder();
                detalle.AppendLine($"Venta #{venta.Id} - Fecha: {venta.Fecha}");
                foreach (var d in venta.Detalles)
                    detalle.AppendLine($"- {d.ProductoNombre} x {d.Cantidad} @ ${d.PrecioUnitario} = ${d.Subtotal}");
                var total = venta.Detalles.Sum(x => x.Subtotal);
                detalle.AppendLine($"Total: ${total}");
                MessageBox.Show(detalle.ToString(), "Venta registrada con éxito");
                try
                {
                    var html = new StringBuilder();
                    html.Append("<html><head><meta charset='utf-8'><title>Comprobante</title></head><body>");
                    html.Append($"<h2>Venta #{venta.Id}</h2><p>Fecha: {venta.Fecha}</p><ul>");
                    foreach (var d in venta.Detalles)
                        html.Append($"<li>{d.ProductoNombre} x {d.Cantidad} @ ${d.PrecioUnitario} = ${d.Subtotal}</li>");
                    html.Append($"</ul><h3>Total: ${total}</h3></body></html>");
                    var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Comprobante_Venta_{venta.Id}.html");
                    System.IO.File.WriteAllText(path, html.ToString(), Encoding.UTF8);
                }
                catch { }
                _carrito.Clear();
                dgvCarrito.DataSource = null;
                lblTotal.Text = "Total: $0";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar venta: {ex.Message}");
            }
            CargarProductos();
            ActualizarCarrito();
        }

        public override void ActualizarIdioma(Dictionary<string, string> traducciones)
        {
            base.ActualizarIdioma(traducciones);
            if (traducciones.TryGetValue("Total", out var txTotal))
            {
                var texto = lblTotal.Text;
                var valor = "0";
                var idx = texto.LastIndexOf('$');
                if (idx >= 0 && idx + 1 < texto.Length)
                    valor = texto.Substring(idx + 1);
                lblTotal.Text = $"{txTotal}: ${valor}";
            }
        }
    }
}
