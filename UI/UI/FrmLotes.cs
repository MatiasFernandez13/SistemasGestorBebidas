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

namespace UI
{
    public partial class FrmLotes : FrmBase
    {
        private LoteBLL _loteBLL = new LoteBLL();
        private ProductoBLL _productoBLL = new ProductoBLL();
        private int _productoId;

        public FrmLotes(int productoId)
        {
            InitializeComponent();
            _productoId = productoId;
        }

        private void FrmLotes_Load(object sender, EventArgs e)
        {
            var producto = _productoBLL.ObtenerPorId(_productoId);
            this.Text = $"Lotes de: {producto.Nombre}";
            CargarLotes(_productoId);
        }

        private void CargarLotes(int productoId)
        {
            dgvLotes.DataSource = null;
            dgvLotes.DataSource = _loteBLL.ListarPorProducto(productoId);
            dgvLotes.ClearSelection();
            dgvLotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            foreach (DataGridViewColumn col in dgvLotes.Columns)
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            
            if (dgvLotes.Columns.Contains("ProductoId")) dgvLotes.Columns["ProductoId"].Visible = false;
            if (dgvLotes.Columns.Contains("Id")) dgvLotes.Columns["Id"].Visible = false;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLotes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un lote para eliminar.");
                return;
            }

            var lote = (Lote)dgvLotes.SelectedRows[0].DataBoundItem;
            if (MessageBox.Show("¿Está seguro de eliminar este lote? Esto descontará el stock del producto.", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _loteBLL.Eliminar(lote.Id, lote.ProductoId);
                    CargarLotes(lote.ProductoId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar lote: {ex.Message}");
                }
            }
        }

        private void cbProducto_SelectedIndexChanged(object sender, EventArgs e) { }
        private void btnAgregar_Click(object sender, EventArgs e) { }
    }
}
