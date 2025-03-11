namespace CapaPresentacion
{
    partial class frmVentas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtprecio = new System.Windows.Forms.TextBox();
            this.btncrearventa = new FontAwesome.Sharp.IconButton();
            this.dgvdata = new System.Windows.Forms.DataGridView();
            this.IdProducto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Producto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SubTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btneliminar = new System.Windows.Forms.DataGridViewButtonColumn();
            this.txtcantidad = new System.Windows.Forms.NumericUpDown();
            this.lblPagocon = new System.Windows.Forms.Label();
            this.txtpagocon = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboMetodoPago = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cbotipodocumento = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtfecha = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnbuscarproveedor = new FontAwesome.Sharp.IconButton();
            this.txtnombrecliente = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtdocumentocliente = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtstock = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtidproducto = new System.Windows.Forms.TextBox();
            this.btnbuscarproducto = new FontAwesome.Sharp.IconButton();
            this.txtproducto = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtcodproducto = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCambio = new System.Windows.Forms.Label();
            this.txtcambio = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txttotalpagar = new System.Windows.Forms.TextBox();
            this.btnCrearCotizacion = new FontAwesome.Sharp.IconButton();
            this.label12 = new System.Windows.Forms.Label();
            this.txtimpuesto = new System.Windows.Forms.TextBox();
            this.btnagregarproducto = new FontAwesome.Sharp.IconButton();
            this.cbocondicion = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtoc = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtregexo = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtregsag = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.dtpFechaVencimiento = new System.Windows.Forms.DateTimePicker();
            this.btnVentaSinFacturar = new FontAwesome.Sharp.IconButton();
            this.label15 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtIdCategoria = new System.Windows.Forms.TextBox();
            this.txtcolor = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvdata)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.White;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(-13, -13);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label10.Size = new System.Drawing.Size(1583, 626);
            this.label10.TabIndex = 22;
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(1124, 166);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Cantidad:";
            // 
            // txtprecio
            // 
            this.txtprecio.BackColor = System.Drawing.SystemColors.Control;
            this.txtprecio.Location = new System.Drawing.Point(937, 159);
            this.txtprecio.Name = "txtprecio";
            this.txtprecio.Size = new System.Drawing.Size(71, 20);
            this.txtprecio.TabIndex = 30;
            this.txtprecio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtprecio_KeyPress);
            // 
            // btncrearventa
            // 
            this.btncrearventa.BackColor = System.Drawing.SystemColors.Control;
            this.btncrearventa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btncrearventa.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btncrearventa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btncrearventa.ForeColor = System.Drawing.Color.Black;
            this.btncrearventa.IconChar = FontAwesome.Sharp.IconChar.LayerGroup;
            this.btncrearventa.IconColor = System.Drawing.Color.Black;
            this.btncrearventa.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btncrearventa.IconSize = 16;
            this.btncrearventa.Location = new System.Drawing.Point(1096, 482);
            this.btncrearventa.Name = "btncrearventa";
            this.btncrearventa.Size = new System.Drawing.Size(134, 62);
            this.btncrearventa.TabIndex = 43;
            this.btncrearventa.Text = "Crear venta";
            this.btncrearventa.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btncrearventa.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btncrearventa.UseVisualStyleBackColor = false;
            this.btncrearventa.Click += new System.EventHandler(this.btncrearventa_Click);
            // 
            // dgvdata
            // 
            this.dgvdata.AllowUserToAddRows = false;
            this.dgvdata.BackgroundColor = System.Drawing.Color.White;
            this.dgvdata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvdata.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdProducto,
            this.IdCategoria,
            this.Producto,
            this.Precio,
            this.Cantidad,
            this.SubTotal,
            this.btneliminar});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvdata.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvdata.Location = new System.Drawing.Point(7, 208);
            this.dgvdata.Name = "dgvdata";
            this.dgvdata.ReadOnly = true;
            this.dgvdata.Size = new System.Drawing.Size(1342, 267);
            this.dgvdata.TabIndex = 39;
            this.dgvdata.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvdata_CellContentClick);
            this.dgvdata.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvdata_CellPainting);
            // 
            // IdProducto
            // 
            this.IdProducto.HeaderText = "IdProducto";
            this.IdProducto.Name = "IdProducto";
            this.IdProducto.ReadOnly = true;
            this.IdProducto.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IdProducto.Visible = false;
            this.IdProducto.Width = 300;
            // 
            // IdCategoria
            // 
            this.IdCategoria.HeaderText = "IdCategoria";
            this.IdCategoria.Name = "IdCategoria";
            this.IdCategoria.ReadOnly = true;
            this.IdCategoria.Visible = false;
            // 
            // Producto
            // 
            this.Producto.HeaderText = "Descripcion";
            this.Producto.Name = "Producto";
            this.Producto.ReadOnly = true;
            this.Producto.Width = 300;
            // 
            // Precio
            // 
            this.Precio.HeaderText = "Precio unitario";
            this.Precio.Name = "Precio";
            this.Precio.ReadOnly = true;
            this.Precio.Width = 300;
            // 
            // Cantidad
            // 
            this.Cantidad.HeaderText = "Cantidad";
            this.Cantidad.Name = "Cantidad";
            this.Cantidad.ReadOnly = true;
            this.Cantidad.Width = 300;
            // 
            // SubTotal
            // 
            this.SubTotal.HeaderText = "Sub Total";
            this.SubTotal.Name = "SubTotal";
            this.SubTotal.ReadOnly = true;
            this.SubTotal.Width = 300;
            // 
            // btneliminar
            // 
            this.btneliminar.HeaderText = "";
            this.btneliminar.Name = "btneliminar";
            this.btneliminar.ReadOnly = true;
            this.btneliminar.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // txtcantidad
            // 
            this.txtcantidad.DecimalPlaces = 2;
            this.txtcantidad.Location = new System.Drawing.Point(1176, 160);
            this.txtcantidad.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.txtcantidad.Name = "txtcantidad";
            this.txtcantidad.Size = new System.Drawing.Size(67, 20);
            this.txtcantidad.TabIndex = 32;
            this.txtcantidad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblPagocon
            // 
            this.lblPagocon.AutoSize = true;
            this.lblPagocon.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblPagocon.Location = new System.Drawing.Point(293, 526);
            this.lblPagocon.Name = "lblPagocon";
            this.lblPagocon.Size = new System.Drawing.Size(56, 13);
            this.lblPagocon.TabIndex = 41;
            this.lblPagocon.Text = "Paga con:";
            // 
            // txtpagocon
            // 
            this.txtpagocon.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.txtpagocon.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpagocon.Location = new System.Drawing.Point(287, 487);
            this.txtpagocon.Multiline = true;
            this.txtpagocon.Name = "txtpagocon";
            this.txtpagocon.Size = new System.Drawing.Size(188, 57);
            this.txtpagocon.TabIndex = 42;
            this.txtpagocon.Text = "0";
            this.txtpagocon.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtpagocon_KeyDown);
            this.txtpagocon.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtpagocon_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(900, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Precio:";
            // 
            // comboMetodoPago
            // 
            this.comboMetodoPago.BackColor = System.Drawing.SystemColors.Control;
            this.comboMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMetodoPago.FormattingEnabled = true;
            this.comboMetodoPago.Location = new System.Drawing.Point(959, 85);
            this.comboMetodoPago.Name = "comboMetodoPago";
            this.comboMetodoPago.Size = new System.Drawing.Size(119, 21);
            this.comboMetodoPago.TabIndex = 85;
            this.comboMetodoPago.SelectedIndexChanged += new System.EventHandler(this.comboMetodoPago_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(871, 93);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(88, 13);
            this.label17.TabIndex = 84;
            this.label17.Text = "Metodo de pago:";
            // 
            // cbotipodocumento
            // 
            this.cbotipodocumento.BackColor = System.Drawing.SystemColors.Control;
            this.cbotipodocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbotipodocumento.FormattingEnabled = true;
            this.cbotipodocumento.Location = new System.Drawing.Point(714, 86);
            this.cbotipodocumento.Name = "cbotipodocumento";
            this.cbotipodocumento.Size = new System.Drawing.Size(125, 21);
            this.cbotipodocumento.TabIndex = 3;
            this.cbotipodocumento.SelectedIndexChanged += new System.EventHandler(this.cbotipodocumento_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(609, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tipo de documento:";
            // 
            // txtfecha
            // 
            this.txtfecha.BackColor = System.Drawing.SystemColors.Control;
            this.txtfecha.Location = new System.Drawing.Point(55, 56);
            this.txtfecha.Name = "txtfecha";
            this.txtfecha.Size = new System.Drawing.Size(152, 20);
            this.txtfecha.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(9, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Fecha:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(224, 25);
            this.label1.TabIndex = 35;
            this.label1.Text = "REGISTRAR VENTA";
            // 
            // btnbuscarproveedor
            // 
            this.btnbuscarproveedor.BackColor = System.Drawing.Color.Silver;
            this.btnbuscarproveedor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnbuscarproveedor.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnbuscarproveedor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnbuscarproveedor.ForeColor = System.Drawing.Color.Black;
            this.btnbuscarproveedor.IconChar = FontAwesome.Sharp.IconChar.Searchengin;
            this.btnbuscarproveedor.IconColor = System.Drawing.Color.Black;
            this.btnbuscarproveedor.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnbuscarproveedor.IconSize = 18;
            this.btnbuscarproveedor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnbuscarproveedor.Location = new System.Drawing.Point(454, 79);
            this.btnbuscarproveedor.Name = "btnbuscarproveedor";
            this.btnbuscarproveedor.Size = new System.Drawing.Size(121, 27);
            this.btnbuscarproveedor.TabIndex = 26;
            this.btnbuscarproveedor.Text = "Buscar cliente";
            this.btnbuscarproveedor.UseVisualStyleBackColor = false;
            this.btnbuscarproveedor.Click += new System.EventHandler(this.btnbuscarproveedor_Click);
            // 
            // txtnombrecliente
            // 
            this.txtnombrecliente.BackColor = System.Drawing.SystemColors.Control;
            this.txtnombrecliente.Location = new System.Drawing.Point(55, 86);
            this.txtnombrecliente.Name = "txtnombrecliente";
            this.txtnombrecliente.Size = new System.Drawing.Size(380, 20);
            this.txtnombrecliente.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(9, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Cliente:";
            // 
            // txtdocumentocliente
            // 
            this.txtdocumentocliente.BackColor = System.Drawing.SystemColors.Control;
            this.txtdocumentocliente.Location = new System.Drawing.Point(55, 117);
            this.txtdocumentocliente.Name = "txtdocumentocliente";
            this.txtdocumentocliente.Size = new System.Drawing.Size(1048, 20);
            this.txtdocumentocliente.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(9, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "RTN:";
            // 
            // txtstock
            // 
            this.txtstock.BackColor = System.Drawing.SystemColors.Control;
            this.txtstock.Location = new System.Drawing.Point(1048, 159);
            this.txtstock.Name = "txtstock";
            this.txtstock.ReadOnly = true;
            this.txtstock.Size = new System.Drawing.Size(71, 20);
            this.txtstock.TabIndex = 28;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(1012, 166);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Stock:";
            // 
            // txtidproducto
            // 
            this.txtidproducto.Location = new System.Drawing.Point(309, 9);
            this.txtidproducto.Name = "txtidproducto";
            this.txtidproducto.Size = new System.Drawing.Size(21, 20);
            this.txtidproducto.TabIndex = 25;
            this.txtidproducto.Visible = false;
            // 
            // btnbuscarproducto
            // 
            this.btnbuscarproducto.BackColor = System.Drawing.Color.Silver;
            this.btnbuscarproducto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnbuscarproducto.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnbuscarproducto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnbuscarproducto.ForeColor = System.Drawing.Color.Black;
            this.btnbuscarproducto.IconChar = FontAwesome.Sharp.IconChar.Searchengin;
            this.btnbuscarproducto.IconColor = System.Drawing.Color.Black;
            this.btnbuscarproducto.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnbuscarproducto.IconSize = 18;
            this.btnbuscarproducto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnbuscarproducto.Location = new System.Drawing.Point(537, 153);
            this.btnbuscarproducto.Name = "btnbuscarproducto";
            this.btnbuscarproducto.Size = new System.Drawing.Size(165, 27);
            this.btnbuscarproducto.TabIndex = 26;
            this.btnbuscarproducto.Text = "Buscar producto";
            this.btnbuscarproducto.UseVisualStyleBackColor = false;
            this.btnbuscarproducto.Click += new System.EventHandler(this.btnbuscarproducto_Click);
            // 
            // txtproducto
            // 
            this.txtproducto.BackColor = System.Drawing.SystemColors.Control;
            this.txtproducto.Location = new System.Drawing.Point(351, 159);
            this.txtproducto.Name = "txtproducto";
            this.txtproducto.Size = new System.Drawing.Size(162, 20);
            this.txtproducto.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(295, 166);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Producto:";
            // 
            // txtcodproducto
            // 
            this.txtcodproducto.BackColor = System.Drawing.SystemColors.Control;
            this.txtcodproducto.Location = new System.Drawing.Point(106, 159);
            this.txtcodproducto.Name = "txtcodproducto";
            this.txtcodproducto.Size = new System.Drawing.Size(172, 20);
            this.txtcodproducto.TabIndex = 1;
            this.txtcodproducto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtcodproducto_KeyDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(12, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Codigo producto:";
            // 
            // lblCambio
            // 
            this.lblCambio.AutoSize = true;
            this.lblCambio.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lblCambio.Location = new System.Drawing.Point(548, 526);
            this.lblCambio.Name = "lblCambio";
            this.lblCambio.Size = new System.Drawing.Size(45, 13);
            this.lblCambio.TabIndex = 44;
            this.lblCambio.Text = "Cambio:";
            // 
            // txtcambio
            // 
            this.txtcambio.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.txtcambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtcambio.Location = new System.Drawing.Point(542, 486);
            this.txtcambio.Multiline = true;
            this.txtcambio.Name = "txtcambio";
            this.txtcambio.Size = new System.Drawing.Size(188, 58);
            this.txtcambio.TabIndex = 45;
            this.txtcambio.Text = "0";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label16.Location = new System.Drawing.Point(813, 526);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(73, 13);
            this.label16.TabIndex = 46;
            this.label16.Text = "Total a pagar:";
            // 
            // txttotalpagar
            // 
            this.txttotalpagar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.txttotalpagar.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txttotalpagar.Location = new System.Drawing.Point(811, 485);
            this.txttotalpagar.Multiline = true;
            this.txttotalpagar.Name = "txttotalpagar";
            this.txttotalpagar.Size = new System.Drawing.Size(218, 58);
            this.txttotalpagar.TabIndex = 47;
            this.txttotalpagar.Text = "0";
            // 
            // btnCrearCotizacion
            // 
            this.btnCrearCotizacion.BackColor = System.Drawing.SystemColors.Control;
            this.btnCrearCotizacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCrearCotizacion.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCrearCotizacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrearCotizacion.ForeColor = System.Drawing.Color.Black;
            this.btnCrearCotizacion.IconChar = FontAwesome.Sharp.IconChar.LayerGroup;
            this.btnCrearCotizacion.IconColor = System.Drawing.Color.Black;
            this.btnCrearCotizacion.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCrearCotizacion.IconSize = 16;
            this.btnCrearCotizacion.Location = new System.Drawing.Point(1096, 483);
            this.btnCrearCotizacion.Name = "btnCrearCotizacion";
            this.btnCrearCotizacion.Size = new System.Drawing.Size(134, 57);
            this.btnCrearCotizacion.TabIndex = 48;
            this.btnCrearCotizacion.Text = "Crear cotizacion";
            this.btnCrearCotizacion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCrearCotizacion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCrearCotizacion.UseVisualStyleBackColor = false;
            this.btnCrearCotizacion.Click += new System.EventHandler(this.btnCrearCotizacion_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label12.Location = new System.Drawing.Point(57, 527);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 13);
            this.label12.TabIndex = 49;
            this.label12.Text = "Impuesto 15%:";
            // 
            // txtimpuesto
            // 
            this.txtimpuesto.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.txtimpuesto.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtimpuesto.Location = new System.Drawing.Point(51, 487);
            this.txtimpuesto.Multiline = true;
            this.txtimpuesto.Name = "txtimpuesto";
            this.txtimpuesto.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtimpuesto.Size = new System.Drawing.Size(188, 57);
            this.txtimpuesto.TabIndex = 50;
            this.txtimpuesto.Text = "0";
            // 
            // btnagregarproducto
            // 
            this.btnagregarproducto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnagregarproducto.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnagregarproducto.IconColor = System.Drawing.Color.ForestGreen;
            this.btnagregarproducto.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnagregarproducto.Location = new System.Drawing.Point(1260, 143);
            this.btnagregarproducto.Name = "btnagregarproducto";
            this.btnagregarproducto.Size = new System.Drawing.Size(81, 59);
            this.btnagregarproducto.TabIndex = 40;
            this.btnagregarproducto.Text = "Agregar";
            this.btnagregarproducto.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnagregarproducto.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnagregarproducto.UseVisualStyleBackColor = true;
            this.btnagregarproducto.Click += new System.EventHandler(this.btnagregarproducto_Click);
            // 
            // cbocondicion
            // 
            this.cbocondicion.BackColor = System.Drawing.SystemColors.Control;
            this.cbocondicion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbocondicion.FormattingEnabled = true;
            this.cbocondicion.Location = new System.Drawing.Point(1169, 85);
            this.cbocondicion.Name = "cbocondicion";
            this.cbocondicion.Size = new System.Drawing.Size(119, 21);
            this.cbocondicion.TabIndex = 87;
            this.cbocondicion.SelectedIndexChanged += new System.EventHandler(this.cbocondicion_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(1112, 91);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(57, 13);
            this.label13.TabIndex = 86;
            this.label13.Text = "Condicion:";
            // 
            // txtoc
            // 
            this.txtoc.BackColor = System.Drawing.SystemColors.Control;
            this.txtoc.Location = new System.Drawing.Point(325, 53);
            this.txtoc.Name = "txtoc";
            this.txtoc.Size = new System.Drawing.Size(152, 20);
            this.txtoc.TabIndex = 93;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(237, 59);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 13);
            this.label18.TabIndex = 92;
            this.label18.Text = "No. O/C exenta";
            // 
            // txtregexo
            // 
            this.txtregexo.BackColor = System.Drawing.SystemColors.Control;
            this.txtregexo.Location = new System.Drawing.Point(612, 52);
            this.txtregexo.Name = "txtregexo";
            this.txtregexo.Size = new System.Drawing.Size(152, 20);
            this.txtregexo.TabIndex = 95;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.White;
            this.label19.Location = new System.Drawing.Point(494, 58);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(118, 13);
            this.label19.TabIndex = 94;
            this.label19.Text = "No. Reg de exonerado:";
            // 
            // txtregsag
            // 
            this.txtregsag.BackColor = System.Drawing.SystemColors.Control;
            this.txtregsag.Location = new System.Drawing.Point(896, 51);
            this.txtregsag.Name = "txtregsag";
            this.txtregsag.Size = new System.Drawing.Size(152, 20);
            this.txtregsag.TabIndex = 97;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.Color.White;
            this.label20.Location = new System.Drawing.Point(789, 57);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(101, 13);
            this.label20.TabIndex = 96;
            this.label20.Text = "No. Reg de la SAG:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.White;
            this.label14.Location = new System.Drawing.Point(1112, 117);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(115, 13);
            this.label14.TabIndex = 98;
            this.label14.Text = "Fecha de vencimiento:";
            // 
            // dtpFechaVencimiento
            // 
            this.dtpFechaVencimiento.CustomFormat = "dd/MM/yyyy";
            this.dtpFechaVencimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaVencimiento.Location = new System.Drawing.Point(1224, 112);
            this.dtpFechaVencimiento.Name = "dtpFechaVencimiento";
            this.dtpFechaVencimiento.Size = new System.Drawing.Size(101, 20);
            this.dtpFechaVencimiento.TabIndex = 99;
            // 
            // btnVentaSinFacturar
            // 
            this.btnVentaSinFacturar.BackColor = System.Drawing.SystemColors.Control;
            this.btnVentaSinFacturar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVentaSinFacturar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnVentaSinFacturar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVentaSinFacturar.ForeColor = System.Drawing.Color.Black;
            this.btnVentaSinFacturar.IconChar = FontAwesome.Sharp.IconChar.LayerGroup;
            this.btnVentaSinFacturar.IconColor = System.Drawing.Color.Black;
            this.btnVentaSinFacturar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnVentaSinFacturar.IconSize = 16;
            this.btnVentaSinFacturar.Location = new System.Drawing.Point(1096, 482);
            this.btnVentaSinFacturar.Name = "btnVentaSinFacturar";
            this.btnVentaSinFacturar.Size = new System.Drawing.Size(134, 62);
            this.btnVentaSinFacturar.TabIndex = 100;
            this.btnVentaSinFacturar.Text = "Crear venta sin fact.";
            this.btnVentaSinFacturar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVentaSinFacturar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnVentaSinFacturar.UseVisualStyleBackColor = false;
            this.btnVentaSinFacturar.Click += new System.EventHandler(this.btnVentaSinFacturar_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.White;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(776, 488);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 29);
            this.label15.TabIndex = 114;
            this.label15.Text = "L.";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.BackColor = System.Drawing.Color.White;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(507, 491);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 29);
            this.label21.TabIndex = 115;
            this.label21.Text = "L.";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.BackColor = System.Drawing.Color.White;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(252, 491);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(32, 29);
            this.label22.TabIndex = 116;
            this.label22.Text = "L.";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.Color.White;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(19, 495);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(32, 29);
            this.label23.TabIndex = 117;
            this.label23.Text = "L.";
            // 
            // txtIdCategoria
            // 
            this.txtIdCategoria.Location = new System.Drawing.Point(456, 12);
            this.txtIdCategoria.Name = "txtIdCategoria";
            this.txtIdCategoria.Size = new System.Drawing.Size(21, 20);
            this.txtIdCategoria.TabIndex = 118;
            this.txtIdCategoria.Visible = false;
            // 
            // txtcolor
            // 
            this.txtcolor.BackColor = System.Drawing.SystemColors.Control;
            this.txtcolor.Location = new System.Drawing.Point(781, 160);
            this.txtcolor.Name = "txtcolor";
            this.txtcolor.Size = new System.Drawing.Size(113, 20);
            this.txtcolor.TabIndex = 120;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.BackColor = System.Drawing.Color.White;
            this.label24.Location = new System.Drawing.Point(710, 167);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 13);
            this.label24.TabIndex = 119;
            this.label24.Text = "Color/Codigo:";
            // 
            // frmVentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1361, 584);
            this.Controls.Add(this.txtcolor);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.txtIdCategoria);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btnVentaSinFacturar);
            this.Controls.Add(this.dtpFechaVencimiento);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtregsag);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.txtregexo);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txtoc);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.cbocondicion);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCambio);
            this.Controls.Add(this.lblPagocon);
            this.Controls.Add(this.btncrearventa);
            this.Controls.Add(this.txtimpuesto);
            this.Controls.Add(this.btnCrearCotizacion);
            this.Controls.Add(this.txttotalpagar);
            this.Controls.Add(this.txtcambio);
            this.Controls.Add(this.txtpagocon);
            this.Controls.Add(this.comboMetodoPago);
            this.Controls.Add(this.btnagregarproducto);
            this.Controls.Add(this.txtcantidad);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtdocumentocliente);
            this.Controls.Add(this.txtstock);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtprecio);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbotipodocumento);
            this.Controls.Add(this.btnbuscarproveedor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtidproducto);
            this.Controls.Add(this.btnbuscarproducto);
            this.Controls.Add(this.txtnombrecliente);
            this.Controls.Add(this.txtproducto);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtcodproducto);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtfecha);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvdata);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label10);
            this.Name = "frmVentas";
            this.Text = "frmVentas";
            this.Load += new System.EventHandler(this.frmVentas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvdata)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcantidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtprecio;
        private FontAwesome.Sharp.IconButton btncrearventa;
        private System.Windows.Forms.DataGridView dgvdata;
        private System.Windows.Forms.NumericUpDown txtcantidad;
        private System.Windows.Forms.Label lblPagocon;
        private System.Windows.Forms.TextBox txtpagocon;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbotipodocumento;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtfecha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private FontAwesome.Sharp.IconButton btnbuscarproveedor;
        private System.Windows.Forms.TextBox txtnombrecliente;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtdocumentocliente;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtstock;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtidproducto;
        private FontAwesome.Sharp.IconButton btnbuscarproducto;
        private System.Windows.Forms.TextBox txtproducto;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtcodproducto;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCambio;
        private System.Windows.Forms.TextBox txtcambio;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txttotalpagar;
        private System.Windows.Forms.ComboBox comboMetodoPago;
        private System.Windows.Forms.Label label17;
        private FontAwesome.Sharp.IconButton btnCrearCotizacion;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtimpuesto;
        private FontAwesome.Sharp.IconButton btnagregarproducto;
        private System.Windows.Forms.ComboBox cbocondicion;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtoc;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtregexo;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtregsag;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DateTimePicker dtpFechaVencimiento;
        private FontAwesome.Sharp.IconButton btnVentaSinFacturar;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtIdCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdProducto;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn Producto;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn SubTotal;
        private System.Windows.Forms.DataGridViewButtonColumn btneliminar;
        private System.Windows.Forms.TextBox txtcolor;
        private System.Windows.Forms.Label label24;
    }
}