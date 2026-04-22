namespace UI
{
    partial class FrmLotes
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cbProducto = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvLotes = new System.Windows.Forms.DataGridView();
            this.txtNumeroLote = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpFechaIngreso = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpFechaVencimiento = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // cbProducto
            // 
            this.cbProducto.Visible = false;
            this.label1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Producto";
            // 
            // dgvLotes
            // 
            this.dgvLotes.AllowUserToAddRows = false;
            this.dgvLotes.AllowUserToDeleteRows = false;
            this.dgvLotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLotes.Location = new System.Drawing.Point(23, 23); // Movido arriba
            this.dgvLotes.Name = "dgvLotes";
            this.dgvLotes.ReadOnly = true;
            this.dgvLotes.RowHeadersWidth = 51;
            this.dgvLotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLotes.Size = new System.Drawing.Size(750, 400); // Mas grande
            this.dgvLotes.TabIndex = 2;
            // 
            // txtNumeroLote
            // 
            this.txtNumeroLote.Location = new System.Drawing.Point(120, 60);
            this.txtNumeroLote.Name = "txtNumeroLote";
            this.txtNumeroLote.Size = new System.Drawing.Size(150, 22);
            this.txtNumeroLote.TabIndex = 3;
            this.txtNumeroLote.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Nº Lote";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Fecha Ingreso";
            this.label3.Visible = false;
            // 
            // dtpFechaIngreso
            // 
            this.dtpFechaIngreso.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaIngreso.Location = new System.Drawing.Point(120, 98);
            this.dtpFechaIngreso.Name = "dtpFechaIngreso";
            this.dtpFechaIngreso.Size = new System.Drawing.Size(150, 22);
            this.dtpFechaIngreso.TabIndex = 6;
            this.dtpFechaIngreso.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(300, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Fecha Vencimiento";
            this.label4.Visible = false;
            // 
            // dtpFechaVencimiento
            // 
            this.dtpFechaVencimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaVencimiento.Location = new System.Drawing.Point(434, 98);
            this.dtpFechaVencimiento.Name = "dtpFechaVencimiento";
            this.dtpFechaVencimiento.Size = new System.Drawing.Size(150, 22);
            this.dtpFechaVencimiento.TabIndex = 8;
            this.dtpFechaVencimiento.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(300, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Cantidad";
            this.label5.Visible = false;
            // 
            // nudCantidad
            // 
            this.nudCantidad.Location = new System.Drawing.Point(434, 61);
            this.nudCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(100, 22);
            this.nudCantidad.TabIndex = 10;
            this.nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCantidad.Visible = false;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(120, 140);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(100, 30);
            this.btnAgregar.TabIndex = 11;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            this.btnAgregar.Visible = false;
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(23, 440);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(100, 30);
            this.btnEliminar.TabIndex = 12;
            this.btnEliminar.Text = "Eliminar Lote";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // FrmLotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtpFechaVencimiento);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtpFechaIngreso);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNumeroLote);
            this.Controls.Add(this.dgvLotes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbProducto);
            this.Name = "FrmLotes";
            this.Text = "Gestión de Lotes";
            this.Load += new System.EventHandler(this.FrmLotes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ComboBox cbProducto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvLotes;
        private System.Windows.Forms.TextBox txtNumeroLote;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpFechaIngreso;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpFechaVencimiento;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnEliminar;
    }
}
