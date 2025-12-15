namespace CapaPresentacion
{
    partial class frmProducto
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnbuscar = new FontAwesome.Sharp.IconButton();
            this.btnlimpiarbuscador = new FontAwesome.Sharp.IconButton();
            this.txtbusqueda = new System.Windows.Forms.TextBox();
            this.cbobusqueda = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.BtnExportar = new FontAwesome.Sharp.IconButton();
            this.BtnHistorialStock = new FontAwesome.Sharp.IconButton();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.btnHistorialVentas = new FontAwesome.Sharp.IconButton();
            this.CboFiltro = new System.Windows.Forms.ComboBox();
            this.timerBuscar = new System.Windows.Forms.Timer(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnBackUp = new FontAwesome.Sharp.IconButton();
            this.btnImprimirCodigo = new FontAwesome.Sharp.IconButton();
            this.iconButton3 = new FontAwesome.Sharp.IconButton();
            this.txtTotalProductosRegistrados = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtValorInventarioCompra = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.dgvdata = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Codigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Nombre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdCategoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Categoria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Stock = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrecioVenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Prima = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Precio3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrecioPuntos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrecioCompra = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstadoValor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Estado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnHistorialCompras = new FontAwesome.Sharp.IconButton();
            this.btnExportarInventarioPDF = new FontAwesome.Sharp.IconButton();
            this.btnImprimirInventario = new FontAwesome.Sharp.IconButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.iconButton4 = new FontAwesome.Sharp.IconButton();
            this.iconButton5 = new FontAwesome.Sharp.IconButton();
            this.iconButton6 = new FontAwesome.Sharp.IconButton();
            this.txtid = new System.Windows.Forms.TextBox();
            this.txtindice = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvdata)).BeginInit();
            this.SuspendLayout();
            // 
            // btnbuscar
            // 
            this.btnbuscar.BackColor = System.Drawing.Color.White;
            this.btnbuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnbuscar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnbuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnbuscar.ForeColor = System.Drawing.Color.White;
            this.btnbuscar.IconChar = FontAwesome.Sharp.IconChar.Searchengin;
            this.btnbuscar.IconColor = System.Drawing.Color.Black;
            this.btnbuscar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnbuscar.IconSize = 18;
            this.btnbuscar.Location = new System.Drawing.Point(1081, -172);
            this.btnbuscar.Name = "btnbuscar";
            this.btnbuscar.Size = new System.Drawing.Size(36, 21);
            this.btnbuscar.TabIndex = 53;
            this.btnbuscar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnbuscar.UseVisualStyleBackColor = false;
            // 
            // btnlimpiarbuscador
            // 
            this.btnlimpiarbuscador.BackColor = System.Drawing.Color.White;
            this.btnlimpiarbuscador.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnlimpiarbuscador.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnlimpiarbuscador.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnlimpiarbuscador.ForeColor = System.Drawing.Color.White;
            this.btnlimpiarbuscador.IconChar = FontAwesome.Sharp.IconChar.Broom;
            this.btnlimpiarbuscador.IconColor = System.Drawing.Color.Black;
            this.btnlimpiarbuscador.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnlimpiarbuscador.IconSize = 18;
            this.btnlimpiarbuscador.Location = new System.Drawing.Point(1104, 111);
            this.btnlimpiarbuscador.Name = "btnlimpiarbuscador";
            this.btnlimpiarbuscador.Size = new System.Drawing.Size(36, 21);
            this.btnlimpiarbuscador.TabIndex = 54;
            this.btnlimpiarbuscador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnlimpiarbuscador.UseVisualStyleBackColor = false;
            this.btnlimpiarbuscador.Click += new System.EventHandler(this.btnlimpiarbuscador_Click);
            // 
            // txtbusqueda
            // 
            this.txtbusqueda.Location = new System.Drawing.Point(894, 111);
            this.txtbusqueda.Name = "txtbusqueda";
            this.txtbusqueda.Size = new System.Drawing.Size(202, 20);
            this.txtbusqueda.TabIndex = 52;
            this.txtbusqueda.TextChanged += new System.EventHandler(this.txtbusqueda_TextChanged);
            this.txtbusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbusqueda_KeyDown);
            // 
            // cbobusqueda
            // 
            this.cbobusqueda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbobusqueda.FormattingEnabled = true;
            this.cbobusqueda.Location = new System.Drawing.Point(745, 111);
            this.cbobusqueda.Name = "cbobusqueda";
            this.cbobusqueda.Size = new System.Drawing.Size(145, 21);
            this.cbobusqueda.TabIndex = 51;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(681, 116);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 50;
            this.label11.Text = "Buscar por";
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.White;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(-3, 9);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label10.Size = new System.Drawing.Size(1547, 135);
            this.label10.TabIndex = 48;
            this.label10.Text = "LISTA DE PRODUCTOS";
            // 
            // BtnExportar
            // 
            this.BtnExportar.BackColor = System.Drawing.Color.White;
            this.BtnExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnExportar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnExportar.ForeColor = System.Drawing.Color.Black;
            this.BtnExportar.IconChar = FontAwesome.Sharp.IconChar.FloppyDisk;
            this.BtnExportar.IconColor = System.Drawing.Color.DarkGoldenrod;
            this.BtnExportar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.BtnExportar.IconSize = 16;
            this.BtnExportar.Location = new System.Drawing.Point(235, 43);
            this.BtnExportar.Name = "BtnExportar";
            this.BtnExportar.Size = new System.Drawing.Size(123, 35);
            this.BtnExportar.TabIndex = 57;
            this.BtnExportar.Text = "Descargar Excel";
            this.BtnExportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnExportar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnExportar.UseVisualStyleBackColor = false;
            this.BtnExportar.Click += new System.EventHandler(this.BtnExportar_Click);
            // 
            // BtnHistorialStock
            // 
            this.BtnHistorialStock.BackColor = System.Drawing.SystemColors.Control;
            this.BtnHistorialStock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnHistorialStock.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnHistorialStock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnHistorialStock.ForeColor = System.Drawing.Color.Black;
            this.BtnHistorialStock.IconChar = FontAwesome.Sharp.IconChar.Upload;
            this.BtnHistorialStock.IconColor = System.Drawing.Color.Black;
            this.BtnHistorialStock.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.BtnHistorialStock.IconSize = 16;
            this.BtnHistorialStock.Location = new System.Drawing.Point(701, 17);
            this.BtnHistorialStock.Name = "BtnHistorialStock";
            this.BtnHistorialStock.Size = new System.Drawing.Size(136, 26);
            this.BtnHistorialStock.TabIndex = 82;
            this.BtnHistorialStock.Text = "HISTORIAL STOCK";
            this.BtnHistorialStock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnHistorialStock.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnHistorialStock.UseVisualStyleBackColor = false;
            this.BtnHistorialStock.Click += new System.EventHandler(this.BtnHistorialStock_Click);
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.SystemColors.Control;
            this.iconButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.ForeColor = System.Drawing.Color.Black;
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.Sync;
            this.iconButton2.IconColor = System.Drawing.Color.Black;
            this.iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton2.IconSize = 16;
            this.iconButton2.Location = new System.Drawing.Point(1251, 17);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Size = new System.Drawing.Size(109, 26);
            this.iconButton2.TabIndex = 133;
            this.iconButton2.Text = "REFRESCAR";
            this.iconButton2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.iconButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.iconButton2_Click);
            // 
            // iconButton1
            // 
            this.iconButton1.BackColor = System.Drawing.Color.White;
            this.iconButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.ForeColor = System.Drawing.Color.White;
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.Searchengin;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 18;
            this.iconButton1.Location = new System.Drawing.Point(12, 0);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(36, 21);
            this.iconButton1.TabIndex = 56;
            this.iconButton1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.Visible = false;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // btnHistorialVentas
            // 
            this.btnHistorialVentas.BackColor = System.Drawing.SystemColors.Control;
            this.btnHistorialVentas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHistorialVentas.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnHistorialVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistorialVentas.ForeColor = System.Drawing.Color.Black;
            this.btnHistorialVentas.IconChar = FontAwesome.Sharp.IconChar.History;
            this.btnHistorialVentas.IconColor = System.Drawing.Color.Black;
            this.btnHistorialVentas.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnHistorialVentas.IconSize = 16;
            this.btnHistorialVentas.Location = new System.Drawing.Point(1016, 16);
            this.btnHistorialVentas.Name = "btnHistorialVentas";
            this.btnHistorialVentas.Size = new System.Drawing.Size(160, 26);
            this.btnHistorialVentas.TabIndex = 134;
            this.btnHistorialVentas.Text = "HISTORIAL DE VENTAS";
            this.btnHistorialVentas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHistorialVentas.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHistorialVentas.UseVisualStyleBackColor = false;
            this.btnHistorialVentas.Click += new System.EventHandler(this.btnHistorialVentas_Click);
            // 
            // CboFiltro
            // 
            this.CboFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CboFiltro.FormattingEnabled = true;
            this.CboFiltro.Location = new System.Drawing.Point(1182, 110);
            this.CboFiltro.Name = "CboFiltro";
            this.CboFiltro.Size = new System.Drawing.Size(171, 21);
            this.CboFiltro.TabIndex = 135;
            this.CboFiltro.SelectedIndexChanged += new System.EventHandler(this.CboFiltro_SelectedIndexChanged);
            // 
            // timerBuscar
            // 
            this.timerBuscar.Interval = 1000;
            this.timerBuscar.Tick += new System.EventHandler(this.timerBuscar_Tick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(742, -23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 136;
            this.label12.Text = "Precio 3: L.";
            this.label12.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(665, -23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 138;
            this.label13.Text = "Precio 2: L.";
            this.label13.Visible = false;
            // 
            // btnBackUp
            // 
            this.btnBackUp.BackColor = System.Drawing.Color.White;
            this.btnBackUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackUp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnBackUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackUp.ForeColor = System.Drawing.Color.Black;
            this.btnBackUp.IconChar = FontAwesome.Sharp.IconChar.CloudUploadAlt;
            this.btnBackUp.IconColor = System.Drawing.Color.Black;
            this.btnBackUp.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBackUp.IconSize = 19;
            this.btnBackUp.Location = new System.Drawing.Point(12, 46);
            this.btnBackUp.Name = "btnBackUp";
            this.btnBackUp.Size = new System.Drawing.Size(202, 35);
            this.btnBackUp.TabIndex = 145;
            this.btnBackUp.Text = "COPIA DE SEGURIDAD";
            this.btnBackUp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBackUp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBackUp.UseVisualStyleBackColor = false;
            this.btnBackUp.Click += new System.EventHandler(this.btnBackUp_Click);
            // 
            // btnImprimirCodigo
            // 
            this.btnImprimirCodigo.BackColor = System.Drawing.SystemColors.Control;
            this.btnImprimirCodigo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimirCodigo.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnImprimirCodigo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimirCodigo.ForeColor = System.Drawing.Color.Black;
            this.btnImprimirCodigo.IconChar = FontAwesome.Sharp.IconChar.FileWaveform;
            this.btnImprimirCodigo.IconColor = System.Drawing.Color.Black;
            this.btnImprimirCodigo.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnImprimirCodigo.IconSize = 16;
            this.btnImprimirCodigo.Location = new System.Drawing.Point(553, -4);
            this.btnImprimirCodigo.Name = "btnImprimirCodigo";
            this.btnImprimirCodigo.Size = new System.Drawing.Size(204, 29);
            this.btnImprimirCodigo.TabIndex = 146;
            this.btnImprimirCodigo.Text = "IMPRIMIR CODIGO DE BARRAS";
            this.btnImprimirCodigo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImprimirCodigo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImprimirCodigo.UseVisualStyleBackColor = false;
            this.btnImprimirCodigo.Visible = false;
            this.btnImprimirCodigo.Click += new System.EventHandler(this.btnImprimirCodigo_Click);
            // 
            // iconButton3
            // 
            this.iconButton3.BackColor = System.Drawing.Color.White;
            this.iconButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton3.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.iconButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton3.ForeColor = System.Drawing.Color.Black;
            this.iconButton3.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.iconButton3.IconColor = System.Drawing.Color.Black;
            this.iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton3.IconSize = 16;
            this.iconButton3.Location = new System.Drawing.Point(456, -9);
            this.iconButton3.Name = "iconButton3";
            this.iconButton3.Size = new System.Drawing.Size(204, 34);
            this.iconButton3.TabIndex = 147;
            this.iconButton3.Text = "Vista previa Codigo de barras";
            this.iconButton3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.iconButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton3.UseVisualStyleBackColor = false;
            this.iconButton3.Visible = false;
            this.iconButton3.Click += new System.EventHandler(this.iconButton3_Click);
            // 
            // txtTotalProductosRegistrados
            // 
            this.txtTotalProductosRegistrados.Location = new System.Drawing.Point(1141, 60);
            this.txtTotalProductosRegistrados.Name = "txtTotalProductosRegistrados";
            this.txtTotalProductosRegistrados.Size = new System.Drawing.Size(112, 20);
            this.txtTotalProductosRegistrados.TabIndex = 149;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(1025, 65);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(113, 13);
            this.label16.TabIndex = 148;
            this.label16.Text = "TOTAL PRODUCTOS";
            // 
            // txtValorInventarioCompra
            // 
            this.txtValorInventarioCompra.Location = new System.Drawing.Point(853, 61);
            this.txtValorInventarioCompra.Name = "txtValorInventarioCompra";
            this.txtValorInventarioCompra.Size = new System.Drawing.Size(111, 20);
            this.txtValorInventarioCompra.TabIndex = 150;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(717, 67);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(114, 13);
            this.label17.TabIndex = 151;
            this.label17.Text = "TOTAL INVENTARIO:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.White;
            this.label18.Location = new System.Drawing.Point(833, 65);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(16, 13);
            this.label18.TabIndex = 152;
            this.label18.Text = "L.";
            // 
            // dgvdata
            // 
            this.dgvdata.AllowUserToAddRows = false;
            this.dgvdata.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvdata.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvdata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvdata.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Codigo,
            this.Nombre,
            this.Descripcion,
            this.IdCategoria,
            this.Categoria,
            this.Stock,
            this.PrecioVenta,
            this.Prima,
            this.Precio2,
            this.Precio3,
            this.PrecioPuntos,
            this.PrecioCompra,
            this.EstadoValor,
            this.Estado});
            this.dgvdata.Location = new System.Drawing.Point(12, 147);
            this.dgvdata.MultiSelect = false;
            this.dgvdata.Name = "dgvdata";
            this.dgvdata.ReadOnly = true;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvdata.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvdata.RowTemplate.Height = 28;
            this.dgvdata.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvdata.Size = new System.Drawing.Size(1348, 393);
            this.dgvdata.TabIndex = 153;
            this.dgvdata.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvdata_CellClick);
            this.dgvdata.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvdata_CellDoubleClick);
            this.dgvdata.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvdata_CellFormatting_1);
            this.dgvdata.SelectionChanged += new System.EventHandler(this.dgvdata_SelectionChanged);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // Codigo
            // 
            this.Codigo.HeaderText = "Codigo";
            this.Codigo.Name = "Codigo";
            this.Codigo.ReadOnly = true;
            // 
            // Nombre
            // 
            this.Nombre.HeaderText = "Nombre ";
            this.Nombre.Name = "Nombre";
            this.Nombre.ReadOnly = true;
            this.Nombre.Width = 300;
            // 
            // Descripcion
            // 
            this.Descripcion.HeaderText = "Descripcion";
            this.Descripcion.Name = "Descripcion";
            this.Descripcion.ReadOnly = true;
            this.Descripcion.Width = 200;
            // 
            // IdCategoria
            // 
            this.IdCategoria.HeaderText = "IdCategoria";
            this.IdCategoria.Name = "IdCategoria";
            this.IdCategoria.ReadOnly = true;
            this.IdCategoria.Visible = false;
            // 
            // Categoria
            // 
            this.Categoria.HeaderText = "Categoria";
            this.Categoria.Name = "Categoria";
            this.Categoria.ReadOnly = true;
            this.Categoria.Width = 180;
            // 
            // Stock
            // 
            this.Stock.HeaderText = "Stock";
            this.Stock.Name = "Stock";
            this.Stock.ReadOnly = true;
            // 
            // PrecioVenta
            // 
            this.PrecioVenta.HeaderText = "Precio venta";
            this.PrecioVenta.Name = "PrecioVenta";
            this.PrecioVenta.ReadOnly = true;
            this.PrecioVenta.Width = 120;
            // 
            // Prima
            // 
            this.Prima.HeaderText = "Prima";
            this.Prima.Name = "Prima";
            this.Prima.ReadOnly = true;
            // 
            // Precio2
            // 
            this.Precio2.HeaderText = "Precio2";
            this.Precio2.Name = "Precio2";
            this.Precio2.ReadOnly = true;
            this.Precio2.Visible = false;
            // 
            // Precio3
            // 
            this.Precio3.HeaderText = "Precio3";
            this.Precio3.Name = "Precio3";
            this.Precio3.ReadOnly = true;
            this.Precio3.Visible = false;
            // 
            // PrecioPuntos
            // 
            this.PrecioPuntos.HeaderText = "Precio en puntos";
            this.PrecioPuntos.Name = "PrecioPuntos";
            this.PrecioPuntos.ReadOnly = true;
            this.PrecioPuntos.Visible = false;
            // 
            // PrecioCompra
            // 
            this.PrecioCompra.HeaderText = "Precio Compra";
            this.PrecioCompra.Name = "PrecioCompra";
            this.PrecioCompra.ReadOnly = true;
            // 
            // EstadoValor
            // 
            this.EstadoValor.HeaderText = "EstadoValor";
            this.EstadoValor.Name = "EstadoValor";
            this.EstadoValor.ReadOnly = true;
            this.EstadoValor.Visible = false;
            // 
            // Estado
            // 
            this.Estado.HeaderText = "Estado";
            this.Estado.Name = "Estado";
            this.Estado.ReadOnly = true;
            // 
            // btnHistorialCompras
            // 
            this.btnHistorialCompras.BackColor = System.Drawing.SystemColors.Control;
            this.btnHistorialCompras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHistorialCompras.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnHistorialCompras.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistorialCompras.ForeColor = System.Drawing.Color.Black;
            this.btnHistorialCompras.IconChar = FontAwesome.Sharp.IconChar.History;
            this.btnHistorialCompras.IconColor = System.Drawing.Color.Black;
            this.btnHistorialCompras.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnHistorialCompras.IconSize = 16;
            this.btnHistorialCompras.Location = new System.Drawing.Point(843, 16);
            this.btnHistorialCompras.Name = "btnHistorialCompras";
            this.btnHistorialCompras.Size = new System.Drawing.Size(167, 26);
            this.btnHistorialCompras.TabIndex = 154;
            this.btnHistorialCompras.Text = "HISTORIAL DE COMPRAS";
            this.btnHistorialCompras.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHistorialCompras.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHistorialCompras.UseVisualStyleBackColor = false;
            this.btnHistorialCompras.Click += new System.EventHandler(this.btnHistorialCompras_Click);
            // 
            // btnExportarInventarioPDF
            // 
            this.btnExportarInventarioPDF.BackColor = System.Drawing.Color.White;
            this.btnExportarInventarioPDF.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportarInventarioPDF.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnExportarInventarioPDF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarInventarioPDF.ForeColor = System.Drawing.Color.Black;
            this.btnExportarInventarioPDF.IconChar = FontAwesome.Sharp.IconChar.SheetPlastic;
            this.btnExportarInventarioPDF.IconColor = System.Drawing.Color.DarkGoldenrod;
            this.btnExportarInventarioPDF.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnExportarInventarioPDF.IconSize = 16;
            this.btnExportarInventarioPDF.Location = new System.Drawing.Point(382, 43);
            this.btnExportarInventarioPDF.Name = "btnExportarInventarioPDF";
            this.btnExportarInventarioPDF.Size = new System.Drawing.Size(117, 35);
            this.btnExportarInventarioPDF.TabIndex = 155;
            this.btnExportarInventarioPDF.Text = "Descargar PDF";
            this.btnExportarInventarioPDF.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportarInventarioPDF.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExportarInventarioPDF.UseVisualStyleBackColor = false;
            this.btnExportarInventarioPDF.Click += new System.EventHandler(this.btnExportarInventarioPDF_Click);
            // 
            // btnImprimirInventario
            // 
            this.btnImprimirInventario.BackColor = System.Drawing.Color.White;
            this.btnImprimirInventario.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimirInventario.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnImprimirInventario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimirInventario.ForeColor = System.Drawing.Color.Black;
            this.btnImprimirInventario.IconChar = FontAwesome.Sharp.IconChar.SquareFontAwesome;
            this.btnImprimirInventario.IconColor = System.Drawing.Color.DarkGoldenrod;
            this.btnImprimirInventario.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnImprimirInventario.IconSize = 16;
            this.btnImprimirInventario.Location = new System.Drawing.Point(528, 43);
            this.btnImprimirInventario.Name = "btnImprimirInventario";
            this.btnImprimirInventario.Size = new System.Drawing.Size(117, 35);
            this.btnImprimirInventario.TabIndex = 156;
            this.btnImprimirInventario.Text = "Imprimir ticket";
            this.btnImprimirInventario.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImprimirInventario.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnImprimirInventario.UseVisualStyleBackColor = false;
            this.btnImprimirInventario.Click += new System.EventHandler(this.btnImprimirInventario_Click);
            // 
            // iconButton4
            // 
            this.iconButton4.BackColor = System.Drawing.Color.ForestGreen;
            this.iconButton4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton4.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.iconButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton4.ForeColor = System.Drawing.Color.White;
            this.iconButton4.IconChar = FontAwesome.Sharp.IconChar.FloppyDisk;
            this.iconButton4.IconColor = System.Drawing.Color.White;
            this.iconButton4.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton4.IconSize = 16;
            this.iconButton4.Location = new System.Drawing.Point(27, 97);
            this.iconButton4.Name = "iconButton4";
            this.iconButton4.Size = new System.Drawing.Size(202, 37);
            this.iconButton4.TabIndex = 159;
            this.iconButton4.Text = "AGREGAR NUEVO";
            this.iconButton4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.iconButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton4.UseVisualStyleBackColor = false;
            this.iconButton4.Click += new System.EventHandler(this.iconButton4_Click);
            // 
            // iconButton5
            // 
            this.iconButton5.BackColor = System.Drawing.Color.RoyalBlue;
            this.iconButton5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton5.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.iconButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton5.ForeColor = System.Drawing.Color.White;
            this.iconButton5.IconChar = FontAwesome.Sharp.IconChar.Broom;
            this.iconButton5.IconColor = System.Drawing.Color.White;
            this.iconButton5.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton5.IconSize = 18;
            this.iconButton5.Location = new System.Drawing.Point(235, 97);
            this.iconButton5.Name = "iconButton5";
            this.iconButton5.Size = new System.Drawing.Size(202, 37);
            this.iconButton5.TabIndex = 160;
            this.iconButton5.Text = "EDITAR";
            this.iconButton5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.iconButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton5.UseVisualStyleBackColor = false;
            this.iconButton5.Click += new System.EventHandler(this.iconButton5_Click);
            // 
            // iconButton6
            // 
            this.iconButton6.BackColor = System.Drawing.Color.Firebrick;
            this.iconButton6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton6.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.iconButton6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton6.ForeColor = System.Drawing.Color.White;
            this.iconButton6.IconChar = FontAwesome.Sharp.IconChar.TrashAlt;
            this.iconButton6.IconColor = System.Drawing.Color.White;
            this.iconButton6.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton6.IconSize = 16;
            this.iconButton6.Location = new System.Drawing.Point(443, 97);
            this.iconButton6.Name = "iconButton6";
            this.iconButton6.Size = new System.Drawing.Size(202, 37);
            this.iconButton6.TabIndex = 161;
            this.iconButton6.Text = "ELIMINAR";
            this.iconButton6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.iconButton6.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton6.UseVisualStyleBackColor = false;
            this.iconButton6.Click += new System.EventHandler(this.iconButton6_Click);
            // 
            // txtid
            // 
            this.txtid.Location = new System.Drawing.Point(675, 76);
            this.txtid.Name = "txtid";
            this.txtid.Size = new System.Drawing.Size(21, 20);
            this.txtid.TabIndex = 183;
            this.txtid.Text = "0";
            this.txtid.Visible = false;
            // 
            // txtindice
            // 
            this.txtindice.Location = new System.Drawing.Point(648, 76);
            this.txtindice.Name = "txtindice";
            this.txtindice.Size = new System.Drawing.Size(21, 20);
            this.txtindice.TabIndex = 184;
            this.txtindice.Text = "-1";
            this.txtindice.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1179, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 185;
            this.label1.Text = "VISTA";
            // 
            // frmProducto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 608);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtid);
            this.Controls.Add(this.txtindice);
            this.Controls.Add(this.iconButton6);
            this.Controls.Add(this.iconButton5);
            this.Controls.Add(this.iconButton4);
            this.Controls.Add(this.btnImprimirInventario);
            this.Controls.Add(this.btnExportarInventarioPDF);
            this.Controls.Add(this.btnHistorialCompras);
            this.Controls.Add(this.dgvdata);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtValorInventarioCompra);
            this.Controls.Add(this.btnImprimirCodigo);
            this.Controls.Add(this.txtTotalProductosRegistrados);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.iconButton3);
            this.Controls.Add(this.btnBackUp);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.CboFiltro);
            this.Controls.Add(this.btnHistorialVentas);
            this.Controls.Add(this.iconButton2);
            this.Controls.Add(this.BtnHistorialStock);
            this.Controls.Add(this.BtnExportar);
            this.Controls.Add(this.iconButton1);
            this.Controls.Add(this.btnbuscar);
            this.Controls.Add(this.btnlimpiarbuscador);
            this.Controls.Add(this.txtbusqueda);
            this.Controls.Add(this.cbobusqueda);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Name = "frmProducto";
            this.Text = "frmProducto";
            this.Load += new System.EventHandler(this.frmProducto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvdata)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FontAwesome.Sharp.IconButton btnbuscar;
        private FontAwesome.Sharp.IconButton btnlimpiarbuscador;
        private System.Windows.Forms.TextBox txtbusqueda;
        private System.Windows.Forms.ComboBox cbobusqueda;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private FontAwesome.Sharp.IconButton BtnExportar;
        private FontAwesome.Sharp.IconButton BtnHistorialStock;
        private FontAwesome.Sharp.IconButton iconButton2;
        private FontAwesome.Sharp.IconButton iconButton1;
        private FontAwesome.Sharp.IconButton btnHistorialVentas;
        private System.Windows.Forms.ComboBox CboFiltro;
        private System.Windows.Forms.Timer timerBuscar;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private FontAwesome.Sharp.IconButton btnBackUp;
        private FontAwesome.Sharp.IconButton btnImprimirCodigo;
        private FontAwesome.Sharp.IconButton iconButton3;
        private System.Windows.Forms.TextBox txtTotalProductosRegistrados;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtValorInventarioCompra;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataGridView dgvdata;
        private FontAwesome.Sharp.IconButton btnHistorialCompras;
        private FontAwesome.Sharp.IconButton btnExportarInventarioPDF;
        private FontAwesome.Sharp.IconButton btnImprimirInventario;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Codigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nombre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdCategoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn Categoria;
        private System.Windows.Forms.DataGridViewTextBoxColumn Stock;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrecioVenta;
        private System.Windows.Forms.DataGridViewTextBoxColumn Prima;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Precio3;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrecioPuntos;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrecioCompra;
        private System.Windows.Forms.DataGridViewTextBoxColumn EstadoValor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Estado;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private FontAwesome.Sharp.IconButton iconButton4;
        private FontAwesome.Sharp.IconButton iconButton5;
        private FontAwesome.Sharp.IconButton iconButton6;
        private System.Windows.Forms.TextBox txtid;
        private System.Windows.Forms.TextBox txtindice;
        private System.Windows.Forms.Label label1;
    }
}