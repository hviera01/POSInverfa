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
            this.TxtEstado = new System.Windows.Forms.TextBox();
            this.TxtFechaVencimiento = new System.Windows.Forms.TextBox();
            this.TxtSaldoPendiente = new System.Windows.Forms.TextBox();
            this.TxtMontoTotal = new System.Windows.Forms.TextBox();
            this.TxtNombreCliente = new System.Windows.Forms.TextBox();
            this.TxtNumeroDocumento = new System.Windows.Forms.TextBox();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.FechaAbono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MontoAbonado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaldoRestante = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InteresAplicado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MetodoPago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
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
            this.label2.Location = new System.Drawing.Point(517, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Estado:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(517, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha de vencimiento:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(272, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Saldo pendiente:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(272, 26);
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
            this.label10.Size = new System.Drawing.Size(916, 47);
            this.label10.TabIndex = 61;
            this.label10.Text = "ABONOS DE FACTURAS A CREDITO";
            // 
            // groupBox1
            // 
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
            this.groupBox1.Location = new System.Drawing.Point(44, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(787, 83);
            this.groupBox1.TabIndex = 62;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "INFORMACION DE LA FACTURA";
            // 
            // TxtEstado
            // 
            this.TxtEstado.Location = new System.Drawing.Point(557, 51);
            this.TxtEstado.Name = "TxtEstado";
            this.TxtEstado.ReadOnly = true;
            this.TxtEstado.Size = new System.Drawing.Size(103, 20);
            this.TxtEstado.TabIndex = 65;
            // 
            // TxtFechaVencimiento
            // 
            this.TxtFechaVencimiento.Location = new System.Drawing.Point(632, 23);
            this.TxtFechaVencimiento.Name = "TxtFechaVencimiento";
            this.TxtFechaVencimiento.ReadOnly = true;
            this.TxtFechaVencimiento.Size = new System.Drawing.Size(138, 20);
            this.TxtFechaVencimiento.TabIndex = 65;
            // 
            // TxtSaldoPendiente
            // 
            this.TxtSaldoPendiente.Location = new System.Drawing.Point(358, 51);
            this.TxtSaldoPendiente.Name = "TxtSaldoPendiente";
            this.TxtSaldoPendiente.ReadOnly = true;
            this.TxtSaldoPendiente.Size = new System.Drawing.Size(103, 20);
            this.TxtSaldoPendiente.TabIndex = 64;
            // 
            // TxtMontoTotal
            // 
            this.TxtMontoTotal.Location = new System.Drawing.Point(336, 19);
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
            this.dgvHistorial.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FechaAbono,
            this.MontoAbonado,
            this.SaldoRestante,
            this.InteresAplicado,
            this.MetodoPago});
            this.dgvHistorial.Location = new System.Drawing.Point(11, 312);
            this.dgvHistorial.MultiSelect = false;
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvHistorial.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvHistorial.RowTemplate.Height = 28;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(857, 389);
            this.dgvHistorial.TabIndex = 63;
            // 
            // FechaAbono
            // 
            this.FechaAbono.HeaderText = "Fecha Abono";
            this.FechaAbono.Name = "FechaAbono";
            this.FechaAbono.ReadOnly = true;
            this.FechaAbono.Width = 180;
            // 
            // MontoAbonado
            // 
            this.MontoAbonado.HeaderText = "Monto Abonado";
            this.MontoAbonado.Name = "MontoAbonado";
            this.MontoAbonado.ReadOnly = true;
            this.MontoAbonado.Width = 180;
            // 
            // SaldoRestante
            // 
            this.SaldoRestante.HeaderText = "Saldo Restante";
            this.SaldoRestante.Name = "SaldoRestante";
            this.SaldoRestante.ReadOnly = true;
            this.SaldoRestante.Width = 180;
            // 
            // InteresAplicado
            // 
            this.InteresAplicado.HeaderText = "Interes aplicado";
            this.InteresAplicado.Name = "InteresAplicado";
            this.InteresAplicado.ReadOnly = true;
            this.InteresAplicado.Width = 180;
            // 
            // MetodoPago
            // 
            this.MetodoPago.HeaderText = "Metodo de pago";
            this.MetodoPago.Name = "MetodoPago";
            this.MetodoPago.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPorcentajeInteres);
            this.groupBox2.Controls.Add(this.cmbInteresAplicadoA);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.cmbAplicarInteres);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label13);
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
            this.groupBox2.Location = new System.Drawing.Point(44, 145);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(787, 165);
            this.groupBox2.TabIndex = 71;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "INFORMACION DEL ABONO:";
            // 
            // txtPorcentajeInteres
            // 
            this.txtPorcentajeInteres.Location = new System.Drawing.Point(250, 67);
            this.txtPorcentajeInteres.Name = "txtPorcentajeInteres";
            this.txtPorcentajeInteres.Size = new System.Drawing.Size(52, 20);
            this.txtPorcentajeInteres.TabIndex = 134;
            // 
            // cmbInteresAplicadoA
            // 
            this.cmbInteresAplicadoA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInteresAplicadoA.FormattingEnabled = true;
            this.cmbInteresAplicadoA.Location = new System.Drawing.Point(409, 67);
            this.cmbInteresAplicadoA.Name = "cmbInteresAplicadoA";
            this.cmbInteresAplicadoA.Size = new System.Drawing.Size(116, 21);
            this.cmbInteresAplicadoA.TabIndex = 133;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(315, 72);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 132;
            this.label16.Text = "Interes aplicado a:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(137, 72);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(110, 13);
            this.label15.TabIndex = 130;
            this.label15.Text = "Porcentaje de interes:";
            // 
            // cmbAplicarInteres
            // 
            this.cmbAplicarInteres.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAplicarInteres.FormattingEnabled = true;
            this.cmbAplicarInteres.Location = new System.Drawing.Point(81, 67);
            this.cmbAplicarInteres.Name = "cmbAplicarInteres";
            this.cmbAplicarInteres.Size = new System.Drawing.Size(47, 21);
            this.cmbAplicarInteres.TabIndex = 129;
            this.cmbAplicarInteres.SelectedIndexChanged += new System.EventHandler(this.cmbAplicarInteres_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 70);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 128;
            this.label14.Text = "Aplicar Interes";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(543, 70);
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
            this.cmbMetodoPago.Location = new System.Drawing.Point(637, 63);
            this.cmbMetodoPago.Name = "cmbMetodoPago";
            this.cmbMetodoPago.Size = new System.Drawing.Size(141, 21);
            this.cmbMetodoPago.TabIndex = 126;
            // 
            // txtInteresAplicado
            // 
            this.txtInteresAplicado.Location = new System.Drawing.Point(95, 101);
            this.txtInteresAplicado.Name = "txtInteresAplicado";
            this.txtInteresAplicado.ReadOnly = true;
            this.txtInteresAplicado.Size = new System.Drawing.Size(125, 20);
            this.txtInteresAplicado.TabIndex = 125;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 124;
            this.label9.Text = "Interes generado:";
            // 
            // txtTotalAPagar
            // 
            this.txtTotalAPagar.Location = new System.Drawing.Point(364, 101);
            this.txtTotalAPagar.Name = "txtTotalAPagar";
            this.txtTotalAPagar.ReadOnly = true;
            this.txtTotalAPagar.Size = new System.Drawing.Size(125, 20);
            this.txtTotalAPagar.TabIndex = 123;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(235, 104);
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
            this.BtnCancelar.Location = new System.Drawing.Point(672, 130);
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
            this.btnregistrarabono.Location = new System.Drawing.Point(522, 132);
            this.btnregistrarabono.Name = "btnregistrarabono";
            this.btnregistrarabono.Size = new System.Drawing.Size(115, 23);
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
            this.txtfechafin.Location = new System.Drawing.Point(358, 19);
            this.txtfechafin.Name = "txtfechafin";
            this.txtfechafin.Size = new System.Drawing.Size(101, 20);
            this.txtfechafin.TabIndex = 94;
            this.txtfechafin.Value = new System.DateTime(2025, 2, 19, 18, 47, 16, 0);
            // 
            // txtNuevoSaldoPendiente
            // 
            this.txtNuevoSaldoPendiente.Location = new System.Drawing.Point(665, 23);
            this.txtNuevoSaldoPendiente.Name = "txtNuevoSaldoPendiente";
            this.txtNuevoSaldoPendiente.ReadOnly = true;
            this.txtNuevoSaldoPendiente.Size = new System.Drawing.Size(113, 20);
            this.txtNuevoSaldoPendiente.TabIndex = 65;
            // 
            // txtMontoAbono
            // 
            this.txtMontoAbono.Location = new System.Drawing.Point(92, 23);
            this.txtMontoAbono.Name = "txtMontoAbono";
            this.txtMontoAbono.Size = new System.Drawing.Size(125, 20);
            this.txtMontoAbono.TabIndex = 6;
            this.txtMontoAbono.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMontoAbono_KeyDown_1);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Monto de abono:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(272, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Fecha de abono:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(543, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Nuevo saldo pendiente:";
            // 
            // FrmCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 647);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.dgvHistorial);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label10);
            this.Name = "FrmCredito";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn FechaAbono;
        private System.Windows.Forms.DataGridViewTextBoxColumn MontoAbonado;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaldoRestante;
        private System.Windows.Forms.DataGridViewTextBoxColumn InteresAplicado;
        private System.Windows.Forms.DataGridViewTextBoxColumn MetodoPago;
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
    }
}