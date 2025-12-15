namespace CapaPresentacion
{
    partial class md_ProductoCredito
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
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.txtCuota = new System.Windows.Forms.TextBox();
            this.cboFrecuencia = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtNoCuotas = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtPrima = new System.Windows.Forms.TextBox();
            this.txtidproveedor = new System.Windows.Forms.TextBox();
            this.dtpfechavencimiento = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.BtnCancelar = new FontAwesome.Sharp.IconButton();
            this.btnregistrarcredito = new FontAwesome.Sharp.IconButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtmontototal = new System.Windows.Forms.TextBox();
            this.txtsaldopendiente = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.White;
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(-4, 10);
            this.label14.Name = "label14";
            this.label14.Padding = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label14.Size = new System.Drawing.Size(709, 47);
            this.label14.TabIndex = 202;
            this.label14.Text = "AGREGAR PRODUCTO CREDITO";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 221);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 13);
            this.label13.TabIndex = 201;
            this.label13.Text = "Fecha inicio credito:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.CustomFormat = "dd/mm/yyyy";
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(142, 215);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(101, 20);
            this.dtpFechaInicio.TabIndex = 200;
            this.dtpFechaInicio.Value = new System.DateTime(2025, 3, 7, 0, 0, 0, 0);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(396, 217);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(71, 13);
            this.label12.TabIndex = 199;
            this.label12.Text = "Monto Cuota:";
            // 
            // txtCuota
            // 
            this.txtCuota.Location = new System.Drawing.Point(514, 210);
            this.txtCuota.Name = "txtCuota";
            this.txtCuota.ReadOnly = true;
            this.txtCuota.Size = new System.Drawing.Size(125, 20);
            this.txtCuota.TabIndex = 198;
            // 
            // cboFrecuencia
            // 
            this.cboFrecuencia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrecuencia.FormattingEnabled = true;
            this.cboFrecuencia.Location = new System.Drawing.Point(514, 164);
            this.cboFrecuencia.Name = "cboFrecuencia";
            this.cboFrecuencia.Size = new System.Drawing.Size(141, 21);
            this.cboFrecuencia.TabIndex = 197;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(395, 172);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 196;
            this.label11.Text = "Frecuencia:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(396, 129);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 195;
            this.label10.Text = "No. Cuotas:";
            // 
            // txtNoCuotas
            // 
            this.txtNoCuotas.Location = new System.Drawing.Point(514, 122);
            this.txtNoCuotas.Name = "txtNoCuotas";
            this.txtNoCuotas.Size = new System.Drawing.Size(125, 20);
            this.txtNoCuotas.TabIndex = 194;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 193;
            this.label9.Text = "Prima:";
            // 
            // txtPrima
            // 
            this.txtPrima.Location = new System.Drawing.Point(142, 126);
            this.txtPrima.Name = "txtPrima";
            this.txtPrima.Size = new System.Drawing.Size(125, 20);
            this.txtPrima.TabIndex = 192;
            // 
            // txtidproveedor
            // 
            this.txtidproveedor.Location = new System.Drawing.Point(562, 37);
            this.txtidproveedor.Name = "txtidproveedor";
            this.txtidproveedor.Size = new System.Drawing.Size(21, 20);
            this.txtidproveedor.TabIndex = 189;
            this.txtidproveedor.Visible = false;
            // 
            // dtpfechavencimiento
            // 
            this.dtpfechavencimiento.CustomFormat = "dd/mm/yyyy";
            this.dtpfechavencimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpfechavencimiento.Location = new System.Drawing.Point(514, 85);
            this.dtpfechavencimiento.Name = "dtpfechavencimiento";
            this.dtpfechavencimiento.Size = new System.Drawing.Size(101, 20);
            this.dtpfechavencimiento.TabIndex = 187;
            this.dtpfechavencimiento.Value = new System.DateTime(2025, 3, 7, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(393, 91);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 13);
            this.label7.TabIndex = 186;
            this.label7.Text = "Fecha de vencimiento:";
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
            this.BtnCancelar.Location = new System.Drawing.Point(278, 282);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(143, 52);
            this.BtnCancelar.TabIndex = 185;
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnCancelar.UseVisualStyleBackColor = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // btnregistrarcredito
            // 
            this.btnregistrarcredito.BackColor = System.Drawing.Color.White;
            this.btnregistrarcredito.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnregistrarcredito.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnregistrarcredito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnregistrarcredito.ForeColor = System.Drawing.Color.Black;
            this.btnregistrarcredito.IconChar = FontAwesome.Sharp.IconChar.Folder;
            this.btnregistrarcredito.IconColor = System.Drawing.Color.Black;
            this.btnregistrarcredito.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnregistrarcredito.IconSize = 16;
            this.btnregistrarcredito.Location = new System.Drawing.Point(514, 282);
            this.btnregistrarcredito.Name = "btnregistrarcredito";
            this.btnregistrarcredito.Size = new System.Drawing.Size(143, 52);
            this.btnregistrarcredito.TabIndex = 184;
            this.btnregistrarcredito.Text = "Guardar a Venta";
            this.btnregistrarcredito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnregistrarcredito.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnregistrarcredito.UseVisualStyleBackColor = false;
            this.btnregistrarcredito.Click += new System.EventHandler(this.btnregistrarcredito_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 180;
            this.label3.Text = "Precio:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 179;
            this.label2.Text = "Saldo pendiente:";
            // 
            // txtmontototal
            // 
            this.txtmontototal.Location = new System.Drawing.Point(142, 82);
            this.txtmontototal.Name = "txtmontototal";
            this.txtmontototal.Size = new System.Drawing.Size(125, 20);
            this.txtmontototal.TabIndex = 175;
            // 
            // txtsaldopendiente
            // 
            this.txtsaldopendiente.Location = new System.Drawing.Point(142, 171);
            this.txtsaldopendiente.Name = "txtsaldopendiente";
            this.txtsaldopendiente.Size = new System.Drawing.Size(125, 20);
            this.txtsaldopendiente.TabIndex = 174;
            // 
            // md_ProductoCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 348);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.dtpFechaInicio);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtCuota);
            this.Controls.Add(this.cboFrecuencia);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtNoCuotas);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtPrima);
            this.Controls.Add(this.txtidproveedor);
            this.Controls.Add(this.dtpfechavencimiento);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.btnregistrarcredito);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtmontototal);
            this.Controls.Add(this.txtsaldopendiente);
            this.Name = "md_ProductoCredito";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Producto Credito";
            this.Load += new System.EventHandler(this.md_ProductoCredito_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtCuota;
        private System.Windows.Forms.ComboBox cboFrecuencia;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtNoCuotas;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPrima;
        private System.Windows.Forms.TextBox txtidproveedor;
        private System.Windows.Forms.DateTimePicker dtpfechavencimiento;
        private System.Windows.Forms.Label label7;
        private FontAwesome.Sharp.IconButton BtnCancelar;
        private FontAwesome.Sharp.IconButton btnregistrarcredito;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtmontototal;
        private System.Windows.Forms.TextBox txtsaldopendiente;
    }
}