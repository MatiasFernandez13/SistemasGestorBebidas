namespace UI
{
    partial class FrmProductos
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.ComboBox cbCategoria;
        private System.Windows.Forms.TextBox txtLitrosPorUnidad;
        private System.Windows.Forms.TextBox txtStock;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnGrabar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnToggleEliminados;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvProductos = new DataGridView();
            txtNombre = new TextBox();
            cbCategoria = new ComboBox();
            txtLitrosPorUnidad = new TextBox();
            txtStock = new TextBox();
            txtPrecio = new TextBox();
            txtLote = new TextBox();
            dtpFechaIngreso = new DateTimePicker();
            dtpFechaVencimiento = new DateTimePicker();
            lblLote = new Label();
            lblFechaIngreso = new Label();
            lblFechaVencimiento = new Label();
            btnVerLotes = new Button();
            btnAgregarStock = new Button();
            btnAgregar = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
            btnGrabar = new Button();
            btnCancelar = new Button();
            btnToggleEliminados = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvProductos).BeginInit();
            SuspendLayout();
            // 
            // dgvProductos
            // 
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Location = new Point(14, 207);
            dgvProductos.MultiSelect = false;
            dgvProductos.Name = "dgvProductos";
            dgvProductos.ReadOnly = true;
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.Size = new Size(1123, 531);
            dgvProductos.TabIndex = 0;
            dgvProductos.CellClick += dgvProductos_CellClick;
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(14, 29);
            txtNombre.Name = "txtNombre";
            txtNombre.PlaceholderText = "Nombre del producto";
            txtNombre.Size = new Size(224, 30);
            txtNombre.TabIndex = 1;
            // 
            // cbCategoria
            // 
            cbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cbCategoria.Location = new Point(259, 29);
            cbCategoria.Name = "cbCategoria";
            cbCategoria.Size = new Size(168, 31);
            cbCategoria.TabIndex = 2;
            // 
            // txtLitrosPorUnidad
            // 
            txtLitrosPorUnidad.Location = new Point(450, 29);
            txtLitrosPorUnidad.Name = "txtLitrosPorUnidad";
            txtLitrosPorUnidad.PlaceholderText = "Capacidad (litros)";
            txtLitrosPorUnidad.Size = new Size(139, 30);
            txtLitrosPorUnidad.TabIndex = 3;
            // 
            // txtStock
            // 
            txtStock.Location = new Point(14, 75);
            txtStock.Name = "txtStock";
            txtStock.PlaceholderText = "Stock";
            txtStock.ReadOnly = true;
            txtStock.Size = new Size(112, 30);
            txtStock.TabIndex = 4;
            // 
            // txtPrecio
            // 
            txtPrecio.Location = new Point(146, 75);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.PlaceholderText = "Precio";
            txtPrecio.Size = new Size(112, 30);
            txtPrecio.TabIndex = 5;
            // 
            // txtLote
            // 
            txtLote.Location = new Point(350, 75);
            txtLote.Name = "txtLote";
            txtLote.Size = new Size(100, 30);
            txtLote.TabIndex = 6;
            // 
            // dtpFechaIngreso
            // 
            dtpFechaIngreso.Format = DateTimePickerFormat.Short;
            dtpFechaIngreso.Location = new Point(560, 75);
            dtpFechaIngreso.Name = "dtpFechaIngreso";
            dtpFechaIngreso.Size = new Size(120, 30);
            dtpFechaIngreso.TabIndex = 7;
            // 
            // dtpFechaVencimiento
            // 
            dtpFechaVencimiento.Format = DateTimePickerFormat.Short;
            dtpFechaVencimiento.Location = new Point(795, 75);
            dtpFechaVencimiento.Name = "dtpFechaVencimiento";
            dtpFechaVencimiento.Size = new Size(120, 30);
            dtpFechaVencimiento.TabIndex = 8;
            // 
            // lblLote
            // 
            lblLote.AutoSize = true;
            lblLote.Location = new Point(300, 78);
            lblLote.Name = "lblLote";
            lblLote.Size = new Size(47, 23);
            lblLote.TabIndex = 15;
            lblLote.Text = "Lote:";
            // 
            // lblFechaIngreso
            // 
            lblFechaIngreso.AutoSize = true;
            lblFechaIngreso.Location = new Point(470, 78);
            lblFechaIngreso.Name = "lblFechaIngreso";
            lblFechaIngreso.Size = new Size(88, 23);
            lblFechaIngreso.TabIndex = 14;
            lblFechaIngreso.Text = "F. Ingreso:";
            // 
            // lblFechaVencimiento
            // 
            lblFechaVencimiento.AutoSize = true;
            lblFechaVencimiento.Location = new Point(700, 78);
            lblFechaVencimiento.Name = "lblFechaVencimiento";
            lblFechaVencimiento.Size = new Size(91, 23);
            lblFechaVencimiento.TabIndex = 13;
            lblFechaVencimiento.Text = "F. Vencim.:";
            // 
            // btnVerLotes
            // 
            btnVerLotes.Location = new Point(750, 126);
            btnVerLotes.Name = "btnVerLotes";
            btnVerLotes.Size = new Size(120, 34);
            btnVerLotes.TabIndex = 12;
            btnVerLotes.Text = "Ver Lotes";
            btnVerLotes.Click += btnVerLotes_Click;
            // 
            // btnAgregarStock
            // 
            btnAgregarStock.Location = new Point(600, 126);
            btnAgregarStock.Name = "btnAgregarStock";
            btnAgregarStock.Size = new Size(140, 34);
            btnAgregarStock.TabIndex = 13;
            btnAgregarStock.Text = "Agregar Unidades";
            btnAgregarStock.UseVisualStyleBackColor = true;
            btnAgregarStock.Visible = false;
            btnAgregarStock.Click += btnAgregarUnidades_Click;
            // 
            // btnAgregar
            // 
            btnAgregar.Location = new Point(14, 126);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(84, 34);
            btnAgregar.TabIndex = 6;
            btnAgregar.Tag = "Agregar";
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = true;
            btnAgregar.Click += btnAgregar_Click;
            // 
            // btnModificar
            // 
            btnModificar.Enabled = false;
            btnModificar.Location = new Point(112, 126);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(96, 34);
            btnModificar.TabIndex = 7;
            btnModificar.Tag = "Modificar";
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = true;
            btnModificar.Click += btnModificar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.Location = new Point(214, 126);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(84, 34);
            btnEliminar.TabIndex = 8;
            btnEliminar.Tag = "Eliminar";
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = true;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnGrabar
            // 
            btnGrabar.Location = new Point(315, 126);
            btnGrabar.Name = "btnGrabar";
            btnGrabar.Size = new Size(84, 34);
            btnGrabar.TabIndex = 9;
            btnGrabar.Tag = "Grabar";
            btnGrabar.Text = "Grabar";
            btnGrabar.UseVisualStyleBackColor = true;
            btnGrabar.Visible = false;
            btnGrabar.Click += btnGrabar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(416, 126);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(84, 34);
            btnCancelar.TabIndex = 10;
            btnCancelar.Tag = "Cancelar";
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Visible = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // btnToggleEliminados
            // 
            btnToggleEliminados.Location = new Point(902, 126);
            btnToggleEliminados.Name = "btnToggleEliminados";
            btnToggleEliminados.Size = new Size(180, 34);
            btnToggleEliminados.TabIndex = 11;
            btnToggleEliminados.Tag = "MostrarEliminados";
            btnToggleEliminados.Text = "Mostrar eliminados";
            btnToggleEliminados.UseVisualStyleBackColor = true;
            btnToggleEliminados.Click += btnToggleEliminados_Click;
            // 
            // FrmProductos
            // 
            AutoScaleDimensions = new SizeF(9F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1150, 750);
            Controls.Add(btnAgregarStock);
            Controls.Add(btnVerLotes);
            Controls.Add(dtpFechaVencimiento);
            Controls.Add(lblFechaVencimiento);
            Controls.Add(dtpFechaIngreso);
            Controls.Add(lblFechaIngreso);
            Controls.Add(txtLote);
            Controls.Add(lblLote);
            Controls.Add(dgvProductos);
            Controls.Add(txtNombre);
            Controls.Add(cbCategoria);
            Controls.Add(txtLitrosPorUnidad);
            Controls.Add(txtStock);
            Controls.Add(txtPrecio);
            Controls.Add(btnAgregar);
            Controls.Add(btnModificar);
            Controls.Add(btnEliminar);
            Controls.Add(btnGrabar);
            Controls.Add(btnCancelar);
            Controls.Add(btnToggleEliminados);
            Font = new Font("Segoe UI", 10F);
            Name = "FrmProductos";
            Text = "Gestión de Productos";
            Load += FrmProductos_Load_1;
            ((System.ComponentModel.ISupportInitialize)dgvProductos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox txtLote;
        private DateTimePicker dtpFechaIngreso;
        private DateTimePicker dtpFechaVencimiento;
        private Label lblLote;
        private Label lblFechaIngreso;
        private Label lblFechaVencimiento;
        private Button btnVerLotes;
        private Button btnAgregarStock;
    }
}
