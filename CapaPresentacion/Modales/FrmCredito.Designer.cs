namespace CapaPresentacion.Modales
{
    partial class FrmCredito
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPrima = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtMontoCuota = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtCuotaCurso = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtNoCuotas = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnResumen = new FontAwesome.Sharp.IconButton();
            this.TxtEstado = new System.Windows.Forms.TextBox();
            this.TxtFechaVencimiento = new System.Windows.Forms.TextBox();
            this.TxtSaldoPendiente = new System.Windows.Forms.TextBox();
            this.TxtMontoTotal = new System.Windows.Forms.TextBox();
            this.TxtNombreCliente = new System.Windows.Forms.TextBox();
            this.TxtNumeroDocumento = new System.Windows.Forms.TextBox();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnEliminarAbono = new FontAwesome.Sharp.IconButton();
            this.txtrecibo = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtPorcentajeInteres = new System.Windows.Forms.TextBox();
            this.cmbInteresAplicadoA = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cmbAplicarInteres = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbMetodoPago = new System.Windows.Forms.ComboBox();
            this.txtInteresAplicado = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTotalAPagar = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.BtnCancelar = new FontAwesome.Sharp.IconButton();
            this.btnregistrarabono = new FontAwesome.Sharp.IconButton();
            this.txtfechafin = new System.Windows.Forms.DateTimePicker();
            this.txtNuevoSaldoPendiente = new System.Windows.Forms.TextBox();
            this.txtMontoAbono = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "No. de factura:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(491, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Estado:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(491, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha de vencimiento:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(252, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Saldo pendiente:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(252, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Monto total:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Cliente:";
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.White;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(-3, -1);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label10.Size = new System.Drawing.Size(1361, 47);
            this.label10.TabIndex = 61;
            this.label10.Text = "ABONOS DE FACTURAS A CREDITO";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPrima);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.txtMontoCuota);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.txtCuotaCurso);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.txtDescripcion);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.txtNoCuotas);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.TxtEstado);
            this.groupBox1.Controls.Add(this.TxtFechaVencimiento);
            this.groupBox1.Controls.Add(this.TxtSaldoPendiente);
            this.groupBox1.Controls.Add(this.TxtMontoTotal);
            this.groupBox1.Controls.Add(this.TxtNombreCliente);
            this.groupBox1.Controls.Add(this.TxtNumeroDocumento);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(8, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1297, 83);
            this.groupBox1.TabIndex = 62;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "INFORMACION DE LA FACTURA";
            // 
            // txtPrima
            // 
            this.txtPrima.Location = new System.Drawing.Point(1098, 53);
            this.txtPrima.Name = "txtPrima";
            this.txtPrima.ReadOnly = true;
            this.txtPrima.Size = new System.Drawing.Size(91, 20);
            this.txtPrima.TabIndex = 150;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(1029, 57);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(68, 13);
            this.label22.TabIndex = 149;
            this.label22.Text = "Monto prima:";
            // 
            // txtMontoCuota
            // 
            this.txtMontoCuota.Location = new System.Drawing.Point(1155, 20);
            this.txtMontoCuota.Name = "txtMontoCuota";
            this.txtMontoCuota.ReadOnly = true;
            this.txtMontoCuota.Size = new System.Drawing.Size(91, 20);
            this.txtMontoCuota.TabIndex = 148;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(1082, 24);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(71, 13);
            this.label21.TabIndex = 147;
            this.label21.Text = "Monto Cuota:";
            // 
            // txtCuotaCurso
            // 
            this.txtCuotaCurso.Location = new System.Drawing.Point(1012, 19);
            this.txtCuotaCurso.Name = "txtCuotaCurso";
            this.txtCuotaCurso.ReadOnly = true;
            this.txtCuotaCurso.Size = new System.Drawing.Size(57, 20);
            this.txtCuotaCurso.TabIndex = 146;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(929, 25);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(82, 13);
            this.label20.TabIndex = 145;
            this.label20.Text = "Cuota en curso:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(834, 54);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.ReadOnly = true;
            this.txtDescripcion.Size = new System.Drawing.Size(129, 20);
            this.txtDescripcion.TabIndex = 144;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(769, 60);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(66, 13);
            this.label19.TabIndex = 143;
            this.label19.Text = "Descripcion:";
            // 
            // txtNoCuotas
            // 
            this.txtNoCuotas.Location = new System.Drawing.Point(837, 22);
            this.txtNoCuotas.Name = "txtNoCuotas";
            this.txtNoCuotas.ReadOnly = true;
            this.txtNoCuotas.Size = new System.Drawing.Size(57, 20);
            this.txtNoCuotas.TabIndex = 142;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(773, 29);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 13);
            this.label18.TabIndex = 141;
            this.label18.Text = "No. Cuotas:";
            // 
            // btnResumen
            // 
            this.btnResumen.BackColor = System.Drawing.Color.White;
            this.btnResumen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResumen.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnResumen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResumen.ForeColor = System.Drawing.Color.Black;
            this.btnResumen.IconChar = FontAwesome.Sharp.IconChar.BarsStaggered;
            this.btnResumen.IconColor = System.Drawing.Color.Black;
            this.btnResumen.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnResumen.IconSize = 21;
            this.btnResumen.Location = new System.Drawing.Point(1121, 117);
            this.btnResumen.Name = "btnResumen";
            this.btnResumen.Size = new System.Drawing.Size(156, 41);
            this.btnResumen.TabIndex = 140;
            this.btnResumen.Text = "TABLA DEL CREDITO";
            this.btnResumen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnResumen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnResumen.UseVisualStyleBackColor = false;
            this.btnResumen.Click += new System.EventHandler(this.btnResumen_Click);
            // 
            // TxtEstado
            // 
            this.TxtEstado.Location = new System.Drawing.Point(531, 51);
            this.TxtEstado.Name = "TxtEstado";
            this.TxtEstado.ReadOnly = true;
            this.TxtEstado.Size = new System.Drawing.Size(103, 20);
            this.TxtEstado.TabIndex = 65;
            // 
            // TxtFechaVencimiento
            // 
            this.TxtFechaVencimiento.Location = new System.Drawing.Point(606, 23);
            this.TxtFechaVencimiento.Name = "TxtFechaVencimiento";
            this.TxtFechaVencimiento.ReadOnly = true;
            this.TxtFechaVencimiento.Size = new System.Drawing.Size(138, 20);
            this.TxtFechaVencimiento.TabIndex = 65;
            // 
            // TxtSaldoPendiente
            // 
            this.TxtSaldoPendiente.Location = new System.Drawing.Point(338, 51);
            this.TxtSaldoPendiente.Name = "TxtSaldoPendiente";
            this.TxtSaldoPendiente.ReadOnly = true;
            this.TxtSaldoPendiente.Size = new System.Drawing.Size(103, 20);
            this.TxtSaldoPendiente.TabIndex = 64;
            // 
            // TxtMontoTotal
            // 
            this.TxtMontoTotal.Location = new System.Drawing.Point(316, 19);
            this.TxtMontoTotal.Name = "TxtMontoTotal";
            this.TxtMontoTotal.ReadOnly = true;
            this.TxtMontoTotal.Size = new System.Drawing.Size(125, 20);
            this.TxtMontoTotal.TabIndex = 63;
            // 
            // TxtNombreCliente
            // 
            this.TxtNombreCliente.Location = new System.Drawing.Point(49, 50);
            this.TxtNombreCliente.Name = "TxtNombreCliente";
            this.TxtNombreCliente.ReadOnly = true;
            this.TxtNombreCliente.Size = new System.Drawing.Size(157, 20);
            this.TxtNombreCliente.TabIndex = 7;
            // 
            // TxtNumeroDocumento
            // 
            this.TxtNumeroDocumento.Location = new System.Drawing.Point(81, 23);
            this.TxtNumeroDocumento.Name = "TxtNumeroDocumento";
            this.TxtNumeroDocumento.ReadOnly = true;
            this.TxtNumeroDocumento.Size = new System.Drawing.Size(125, 20);
            this.TxtNumeroDocumento.TabIndex = 6;
            // 
            // dgvHistorial
            // 
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHistorial.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Location = new System.Drawing.Point(12, 316);
            this.dgvHistorial.MultiSelect = false;
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvHistorial.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvHistorial.RowTemplate.Height = 28;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(1293, 322);
            this.dgvHistorial.TabIndex = 63;
            this.dgvHistorial.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistorial_CellDoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnEliminarAbono);
            this.groupBox2.Controls.Add(this.txtrecibo);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.txtPorcentajeInteres);
            this.groupBox2.Controls.Add(this.cmbInteresAplicadoA);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.cmbAplicarInteres);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.btnResumen);
            this.groupBox2.Controls.Add(this.cmbMetodoPago);
            this.groupBox2.Controls.Add(this.txtInteresAplicado);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtTotalAPagar);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.BtnCancelar);
            this.groupBox2.Controls.Add(this.btnregistrarabono);
            this.groupBox2.Controls.Add(this.txtfechafin);
            this.groupBox2.Controls.Add(this.txtNuevoSaldoPendiente);
            this.groupBox2.Controls.Add(this.txtMontoAbono);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Location = new System.Drawing.Point(12, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1293, 165);
            this.groupBox2.TabIndex = 71;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "INFORMACION DEL ABONO:";
            // 
            // btnEliminarAbono
            // 
            this.btnEliminarAbono.BackColor = System.Drawing.Color.White;
            this.btnEliminarAbono.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarAbono.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnEliminarAbono.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarAbono.ForeColor = System.Drawing.Color.Black;
            this.btnEliminarAbono.IconChar = FontAwesome.Sharp.IconChar.CircleMinus;
            this.btnEliminarAbono.IconColor = System.Drawing.Color.Black;
            this.btnEliminarAbono.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEliminarAbono.IconSize = 21;
            this.btnEliminarAbono.Location = new System.Drawing.Point(726, 136);
            this.btnEliminarAbono.Name = "btnEliminarAbono";
            this.btnEliminarAbono.Size = new System.Drawing.Size(112, 25);
            this.btnEliminarAbono.TabIndex = 139;
            this.btnEliminarAbono.Text = "Eliminar abono";
            this.btnEliminarAbono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEliminarAbono.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEliminarAbono.UseVisualStyleBackColor = false;
            this.btnEliminarAbono.Visible = false;
            this.btnEliminarAbono.Click += new System.EventHandler(this.btnEliminarAbono_Click);
            // 
            // txtrecibo
            // 
            this.txtrecibo.Location = new System.Drawing.Point(793, 24);
            this.txtrecibo.Name = "txtrecibo";
            this.txtrecibo.Size = new System.Drawing.Size(118, 20);
            this.txtrecibo.TabIndex = 138;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(722, 32);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 13);
            this.label17.TabIndex = 137;
            this.label17.Text = "No de recibo:";
            // 
            // txtPorcentajeInteres
            // 
            this.txtPorcentajeInteres.Location = new System.Drawing.Point(938, 78);
            this.txtPorcentajeInteres.Name = "txtPorcentajeInteres";
            this.txtPorcentajeInteres.Size = new System.Drawing.Size(52, 20);
            this.txtPorcentajeInteres.TabIndex = 134;
            // 
            // cmbInteresAplicadoA
            // 
            this.cmbInteresAplicadoA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInteresAplicadoA.FormattingEnabled = true;
            this.cmbInteresAplicadoA.Location = new System.Drawing.Point(1161, 72);
            this.cmbInteresAplicadoA.Name = "cmbInteresAplicadoA";
            this.cmbInteresAplicadoA.Size = new System.Drawing.Size(116, 21);
            this.cmbInteresAplicadoA.TabIndex = 133;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(1067, 77);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 132;
            this.label16.Text = "Interes aplicado a:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(825, 83);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(110, 13);
            this.label15.TabIndex = 130;
            this.label15.Text = "Porcentaje de interes:";
            // 
            // cmbAplicarInteres
            // 
            this.cmbAplicarInteres.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAplicarInteres.FormattingEnabled = true;
            this.cmbAplicarInteres.Location = new System.Drawing.Point(1081, 28);
            this.cmbAplicarInteres.Name = "cmbAplicarInteres";
            this.cmbAplicarInteres.Size = new System.Drawing.Size(47, 21);
            this.cmbAplicarInteres.TabIndex = 129;
            this.cmbAplicarInteres.SelectedIndexChanged += new System.EventHandler(this.cmbAplicarInteres_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(1006, 31);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 128;
            this.label14.Text = "Aplicar Interes";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(450, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(88, 13);
            this.label13.TabIndex = 127;
            this.label13.Text = "Metodo de pago:";
            // 
            // cmbMetodoPago
            // 
            this.cmbMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetodoPago.FormattingEnabled = true;
            this.cmbMetodoPago.Items.AddRange(new object[] {
            "Efectivo",
            "Tarjeta",
            "Transferencia"});
            this.cmbMetodoPago.Location = new System.Drawing.Point(539, 24);
            this.cmbMetodoPago.Name = "cmbMetodoPago";
            this.cmbMetodoPago.Size = new System.Drawing.Size(141, 21);
            this.cmbMetodoPago.TabIndex = 126;
            // 
            // txtInteresAplicado
            // 
            this.txtInteresAplicado.Location = new System.Drawing.Point(100, 78);
            this.txtInteresAplicado.Name = "txtInteresAplicado";
            this.txtInteresAplicado.ReadOnly = true;
            this.txtInteresAplicado.Size = new System.Drawing.Size(125, 20);
            this.txtInteresAplicado.TabIndex = 125;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 124;
            this.label9.Text = "Interes generado:";
            // 
            // txtTotalAPagar
            // 
            this.txtTotalAPagar.Location = new System.Drawing.Point(369, 78);
            this.txtTotalAPagar.Name = "txtTotalAPagar";
            this.txtTotalAPagar.ReadOnly = true;
            this.txtTotalAPagar.Size = new System.Drawing.Size(125, 20);
            this.txtTotalAPagar.TabIndex = 123;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(240, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(128, 13);
            this.label7.TabIndex = 122;
            this.label7.Text = "Total a pagar con interes:";
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.BackColor = System.Drawing.Color.White;
            this.BtnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCancelar.ForeColor = System.Drawing.Color.Black;
            this.BtnCancelar.IconChar = FontAwesome.Sharp.IconChar.Ban;
            this.BtnCancelar.IconColor = System.Drawing.Color.Black;
            this.BtnCancelar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.BtnCancelar.IconSize = 21;
            this.BtnCancelar.Location = new System.Drawing.Point(1013, 135);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(80, 25);
            this.BtnCancelar.TabIndex = 121;
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnCancelar.UseVisualStyleBackColor = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click_1);
            // 
            // btnregistrarabono
            // 
            this.btnregistrarabono.BackColor = System.Drawing.Color.White;
            this.btnregistrarabono.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnregistrarabono.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnregistrarabono.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnregistrarabono.ForeColor = System.Drawing.Color.Black;
            this.btnregistrarabono.IconChar = FontAwesome.Sharp.IconChar.Folder;
            this.btnregistrarabono.IconColor = System.Drawing.Color.Black;
            this.btnregistrarabono.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnregistrarabono.IconSize = 16;
            this.btnregistrarabono.Location = new System.Drawing.Point(866, 136);
            this.btnregistrarabono.Name = "btnregistrarabono";
            this.btnregistrarabono.Size = new System.Drawing.Size(115, 25);
            this.btnregistrarabono.TabIndex = 120;
            this.btnregistrarabono.Text = "Registrar abono";
            this.btnregistrarabono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnregistrarabono.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnregistrarabono.UseVisualStyleBackColor = false;
            this.btnregistrarabono.Click += new System.EventHandler(this.btnregistrarabono_Click_1);
            // 
            // txtfechafin
            // 
            this.txtfechafin.CustomFormat = "dd/mm/yyyy";
            this.txtfechafin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtfechafin.Location = new System.Drawing.Point(95, 26);
            this.txtfechafin.Name = "txtfechafin";
            this.txtfechafin.Size = new System.Drawing.Size(101, 20);
            this.txtfechafin.TabIndex = 94;
            this.txtfechafin.Value = new System.DateTime(2025, 2, 19, 18, 47, 16, 0);
            // 
            // txtNuevoSaldoPendiente
            // 
            this.txtNuevoSaldoPendiente.Location = new System.Drawing.Point(639, 77);
            this.txtNuevoSaldoPendiente.Name = "txtNuevoSaldoPendiente";
            this.txtNuevoSaldoPendiente.ReadOnly = true;
            this.txtNuevoSaldoPendiente.Size = new System.Drawing.Size(113, 20);
            this.txtNuevoSaldoPendiente.TabIndex = 65;
            // 
            // txtMontoAbono
            // 
            this.txtMontoAbono.Location = new System.Drawing.Point(304, 25);
            this.txtMontoAbono.Name = "txtMontoAbono";
            this.txtMontoAbono.Size = new System.Drawing.Size(125, 20);
            this.txtMontoAbono.TabIndex = 6;
            this.txtMontoAbono.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMontoAbono_KeyDown_1);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(218, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Monto de abono:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Fecha de abono:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(517, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Nuevo saldo pendiente:";
            // 
            // FrmCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1321, 647);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.dgvHistorial);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label10);
            this.Name = "FrmCredito";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABONOS";
            this.Load += new System.EventHandler(this.FrmCredito_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox TxtSaldoPendiente;
        private System.Windows.Forms.TextBox TxtMontoTotal;
        private System.Windows.Forms.TextBox TxtNombreCliente;
        private System.Windows.Forms.TextBox TxtNumeroDocumento;
        private System.Windows.Forms.TextBox TxtEstado;
        private System.Windows.Forms.TextBox TxtFechaVencimiento;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtPorcentajeInteres;
        private System.Windows.Forms.ComboBox cmbInteresAplicadoA;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbAplicarInteres;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbMetodoPago;
        private System.Windows.Forms.TextBox txtInteresAplicado;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtTotalAPagar;
        private System.Windows.Forms.Label label7;
        private FontAwesome.Sharp.IconButton BtnCancelar;
        private FontAwesome.Sharp.IconButton btnregistrarabono;
        private System.Windows.Forms.DateTimePicker txtfechafin;
        private System.Windows.Forms.TextBox txtNuevoSaldoPendiente;
        private System.Windows.Forms.TextBox txtMontoAbono;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtrecibo;
        private System.Windows.Forms.Label label17;
        private FontAwesome.Sharp.IconButton btnEliminarAbono;
        private FontAwesome.Sharp.IconButton btnResumen;
        private System.Windows.Forms.TextBox txtNoCuotas;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtPrima;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtMontoCuota;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtCuotaCurso;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label label19;
    }
}