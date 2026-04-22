namespace UI
{
    partial class FrmVentas
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.DataGridView dgvCarrito;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button btnAgregarProducto;
        private System.Windows.Forms.Button btnVerLotes;
        private System.Windows.Forms.Button btnRegistrarVenta;
        private System.Windows.Forms.Button btnQuitarItem;
        private System.Windows.Forms.Button btnCancelarCarrito;
        private System.Windows.Forms.Label lblTotal;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvProductos = new DataGridView();
            dgvCarrito = new DataGridView();
            nudCantidad = new NumericUpDown();
            btnAgregarProducto = new Button();
            btnRegistrarVenta = new Button();
            btnQuitarItem = new Button();
            btnCancelarCarrito = new Button();
            btnVerLotes = new Button();
            lblTotal = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).BeginInit();
            SuspendLayout();
            // 
            // dgvProductos
            // 
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Location = new Point(20, 20);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.Size = new Size(764, 811);
            dgvProductos.TabIndex = 0;
            // 
            // dgvCarrito
            // 
            dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCarrito.Location = new Point(988, 20);
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.Size = new Size(700, 811);
            dgvCarrito.TabIndex = 3;
            // 
            // nudCantidad
            // 
            nudCantidad.Location = new Point(834, 196);
            nudCantidad.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCantidad.Name = "nudCantidad";
            nudCantidad.Size = new Size(80, 30);
            nudCantidad.TabIndex = 1;
            nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnAgregarProducto
            // 
            btnAgregarProducto.Location = new Point(822, 261);
            btnAgregarProducto.Name = "btnAgregarProducto";
            btnAgregarProducto.Size = new Size(120, 40);
            btnAgregarProducto.TabIndex = 2;
            btnAgregarProducto.Tag = "Agregar";
            btnAgregarProducto.Text = "Agregar";
            btnAgregarProducto.Click += btnAgregarProducto_Click;
            // 
            // btnRegistrarVenta
            // 
            btnRegistrarVenta.Location = new Point(1341, 862);
            btnRegistrarVenta.Name = "btnRegistrarVenta";
            btnRegistrarVenta.Size = new Size(120, 40);
            btnRegistrarVenta.TabIndex = 4;
            btnRegistrarVenta.Tag = "Registrar";
            btnRegistrarVenta.Text = "Registrar";
            btnRegistrarVenta.Click += btnRegistrarVenta_Click;
            // 
            // btnQuitarItem
            // 
            btnQuitarItem.Location = new Point(1211, 862);
            btnQuitarItem.Name = "btnQuitarItem";
            btnQuitarItem.Size = new Size(120, 40);
            btnQuitarItem.TabIndex = 8;
            btnQuitarItem.Tag = "Quitar";
            btnQuitarItem.Text = "Quitar";
            btnQuitarItem.Enabled = false;
            btnQuitarItem.Click += btnQuitarItem_Click;
            // 
            // btnCancelarCarrito
            // 
            btnCancelarCarrito.Location = new Point(1471, 862);
            btnCancelarCarrito.Name = "btnCancelarCarrito";
            btnCancelarCarrito.Size = new Size(120, 40);
            btnCancelarCarrito.TabIndex = 7;
            btnCancelarCarrito.Tag = "Cancelar";
            btnCancelarCarrito.Text = "Cancelar";
            btnCancelarCarrito.Click += btnCancelarCarrito_Click;
            // 
            // btnVerLotes
            // 
            btnVerLotes.Location = new Point(822, 320);
            btnVerLotes.Name = "btnVerLotes";
            btnVerLotes.Size = new Size(120, 40);
            btnVerLotes.TabIndex = 6;
            btnVerLotes.Text = "Ver Lotes";
            btnVerLotes.Click += btnVerLotes_Click;
            // 
            // lblTotal
            // 
            lblTotal.Location = new Point(1053, 862);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(160, 40);
            lblTotal.TabIndex = 5;
            lblTotal.Tag = "Total";
            lblTotal.Text = "Total: $0";
            lblTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmVentas
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1714, 1055);
            Controls.Add(dgvProductos);
            Controls.Add(nudCantidad);
            Controls.Add(btnAgregarProducto);
            Controls.Add(dgvCarrito);
            Controls.Add(btnRegistrarVenta);
            Controls.Add(btnQuitarItem);
            Controls.Add(btnCancelarCarrito);
            Controls.Add(lblTotal);
            Font = new Font("Segoe UI", 10F);
            Name = "FrmVentas";
            Text = "Gestión de Ventas";
            Load += FrmVentas_Load;
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).EndInit();
            ResumeLayout(false);
        }
    }
}
