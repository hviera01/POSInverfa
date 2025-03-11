namespace CapaPresentacion.Modales
{
    partial class md_RegistrarVentaCredito
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
            this.txtidproveedor = new System.Windows.Forms.TextBox();
            this.btnbuscarproveedor = new FontAwesome.Sharp.IconButton();
            this.dtpfechavencimiento = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.BtnCancelar = new FontAwesome.Sharp.IconButton();
            this.btnregistrarcredito = new FontAwesome.Sharp.IconButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtnofactura = new System.Windows.Forms.TextBox();
            this.txtcliente = new System.Windows.Forms.TextBox();
            this.txtrtncliente = new System.Windows.Forms.TextBox();
            this.txtmontototal = new System.Windows.Forms.TextBox();
            this.txtsaldopendiente = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtidproveedor
            // 
            this.txtidproveedor.Location = new System.Drawing.Point(562, 23);
            this.txtidproveedor.Name = "txtidproveedor";
            this.txtidproveedor.Size = new System.Drawing.Size(21, 20);
            this.txtidproveedor.TabIndex = 160;
            this.txtidproveedor.Visible = false;
            // 
            // btnbuscarproveedor
            // 
            this.btnbuscarproveedor.BackColor = System.Drawing.Color.White;
            this.btnbuscarproveedor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnbuscarproveedor.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnbuscarproveedor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnbuscarproveedor.ForeColor = System.Drawing.Color.White;
            this.btnbuscarproveedor.IconChar = FontAwesome.Sharp.IconChar.Searchengin;
            this.btnbuscarproveedor.IconColor = System.Drawing.Color.Black;
            this.btnbuscarproveedor.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnbuscarproveedor.IconSize = 18;
            this.btnbuscarproveedor.Location = new System.Drawing.Point(290, 85);
            this.btnbuscarproveedor.Name = "btnbuscarproveedor";
            this.btnbuscarproveedor.Size = new System.Drawing.Size(36, 20);
            this.btnbuscarproveedor.TabIndex = 159;
            this.btnbuscarproveedor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnbuscarproveedor.UseVisualStyleBackColor = false;
            this.btnbuscarproveedor.Click += new System.EventHandler(this.btnbuscarproveedor_Click);
            // 
            // dtpfechavencimiento
            // 
            this.dtpfechavencimiento.CustomFormat = "dd/mm/yyyy";
            this.dtpfechavencimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpfechavencimiento.Location = new System.Drawing.Point(499, 149);
            this.dtpfechavencimiento.Name = "dtpfechavencimiento";
            this.dtpfechavencimiento.Size = new System.Drawing.Size(101, 20);
            this.dtpfechavencimiento.TabIndex = 158;
            this.dtpfechavencimiento.Value = new System.DateTime(2025, 3, 7, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(388, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 13);
            this.label7.TabIndex = 157;
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
            this.BtnCancelar.Location = new System.Drawing.Point(475, 223);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(143, 52);
            this.BtnCancelar.TabIndex = 156;
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
            this.btnregistrarcredito.Location = new System.Drawing.Point(290, 223);
            this.btnregistrarcredito.Name = "btnregistrarcredito";
            this.btnregistrarcredito.Size = new System.Drawing.Size(143, 52);
            this.btnregistrarcredito.TabIndex = 155;
            this.btnregistrarcredito.Text = "Registrar Credito";
            this.btnregistrarcredito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnregistrarcredito.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnregistrarcredito.UseVisualStyleBackColor = false;
            this.btnregistrarcredito.Click += new System.EventHandler(this.btnregistrarcredito_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 154;
            this.label6.Text = "Cliente:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 153;
            this.label5.Text = "No. de factura:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 152;
            this.label4.Text = "RTN:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(388, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 151;
            this.label3.Text = "Monto total:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(388, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 150;
            this.label2.Text = "Saldo pendiente:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 22);
            this.label1.TabIndex = 149;
            this.label1.Text = "INGRESAR CREDITO";
            // 
            // txtnofactura
            // 
            this.txtnofactura.Location = new System.Drawing.Point(94, 69);
            this.txtnofactura.Name = "txtnofactura";
            this.txtnofactura.Size = new System.Drawing.Size(125, 20);
            this.txtnofactura.TabIndex = 148;
            // 
            // txtcliente
            // 
            this.txtcliente.Location = new System.Drawing.Point(94, 111);
            this.txtcliente.Name = "txtcliente";
            this.txtcliente.Size = new System.Drawing.Size(267, 20);
            this.txtcliente.TabIndex = 147;
            // 
            // txtrtncliente
            // 
            this.txtrtncliente.Location = new System.Drawing.Point(94, 156);
            this.txtrtncliente.Name = "txtrtncliente";
            this.txtrtncliente.Size = new System.Drawing.Size(214, 20);
            this.txtrtncliente.TabIndex = 146;
            // 
            // txtmontototal
            // 
            this.txtmontototal.Location = new System.Drawing.Point(472, 66);
            this.txtmontototal.Name = "txtmontototal";
            this.txtmontototal.Size = new System.Drawing.Size(125, 20);
            this.txtmontototal.TabIndex = 145;
            this.txtmontototal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtmontototal_KeyPress);
            // 
            // txtsaldopendiente
            // 
            this.txtsaldopendiente.Location = new System.Drawing.Point(475, 106);
            this.txtsaldopendiente.Name = "txtsaldopendiente";
            this.txtsaldopendiente.Size = new System.Drawing.Size(125, 20);
            this.txtsaldopendiente.TabIndex = 144;
            this.txtsaldopendiente.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtsaldopendiente_KeyPress);
            // 
            // md_RegistrarVentaCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 309);
            this.Controls.Add(this.txtidproveedor);
            this.Controls.Add(this.btnbuscarproveedor);
            this.Controls.Add(this.dtpfechavencimiento);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.btnregistrarcredito);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtnofactura);
            this.Controls.Add(this.txtcliente);
            this.Controls.Add(this.txtrtncliente);
            this.Controls.Add(this.txtmontototal);
            this.Controls.Add(this.txtsaldopendiente);
            this.Name = "md_RegistrarVentaCredito";
            this.Text = "Registrar venta a credito";
            this.Load += new System.EventHandler(this.md_RegistrarVentaCredito_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtidproveedor;
        private FontAwesome.Sharp.IconButton btnbuscarproveedor;
        private System.Windows.Forms.DateTimePicker dtpfechavencimiento;
        private System.Windows.Forms.Label label7;
        private FontAwesome.Sharp.IconButton BtnCancelar;
        private FontAwesome.Sharp.IconButton btnregistrarcredito;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtnofactura;
        private System.Windows.Forms.TextBox txtcliente;
        private System.Windows.Forms.TextBox txtrtncliente;
        private System.Windows.Forms.TextBox txtmontototal;
        private System.Windows.Forms.TextBox txtsaldopendiente;
    }
}