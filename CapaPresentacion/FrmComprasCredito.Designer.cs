namespace CapaPresentacion
{
    partial class FrmComprasCredito
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
            this.btnregistrarabono = new FontAwesome.Sharp.IconButton();
            this.dgvComprasCredito = new System.Windows.Forms.DataGridView();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnlimpiarbuscador = new FontAwesome.Sharp.IconButton();
            this.txtbusqueda = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnExportar = new FontAwesome.Sharp.IconButton();
            this.btnregistrarcompracredito = new FontAwesome.Sharp.IconButton();
            this.txtTotalSaldoPendiente = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComprasCredito)).BeginInit();
            this.SuspendLayout();
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
            this.btnregistrarabono.Location = new System.Drawing.Point(165, 68);
            this.btnregistrarabono.Name = "btnregistrarabono";
            this.btnregistrarabono.Size = new System.Drawing.Size(115, 37);
            this.btnregistrarabono.TabIndex = 127;
            this.btnregistrarabono.Text = "Registrar abono";
            this.btnregistrarabono.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnregistrarabono.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnregistrarabono.UseVisualStyleBackColor = false;
            this.btnregistrarabono.Click += new System.EventHandler(this.btnregistrarabono_Click);
            // 
            // dgvComprasCredito
            // 
            this.dgvComprasCredito.AllowUserToAddRows = false;
            this.dgvComprasCredito.BackgroundColor = System.Drawing.Color.White;
            this.dgvComprasCredito.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComprasCredito.Location = new System.Drawing.Point(27, 111);
            this.dgvComprasCredito.MultiSelect = false;
            this.dgvComprasCredito.Name = "dgvComprasCredito";
            this.dgvComprasCredito.ReadOnly = true;
            this.dgvComprasCredito.Size = new System.Drawing.Size(1319, 406);
            this.dgvComprasCredito.TabIndex = 120;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.White;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(1, 2);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label10.Size = new System.Drawing.Size(1368, 39);
            this.label10.TabIndex = 121;
            this.label10.Text = "COMPRAS A CREDITO";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1, 44);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label4.Size = new System.Drawing.Size(1368, 488);
            this.label4.TabIndex = 122;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.HeaderText = "Fecha registro";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.HeaderText = "Tipo documento";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.HeaderText = "Numero documento";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "Monto total";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "Usuario registro";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "Razon social";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "CodigoProducto";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "SubTotal";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Nombre producto";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Categoria";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Precio compra";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Precio venta";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Cantidad";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
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
            this.btnlimpiarbuscador.Location = new System.Drawing.Point(634, 84);
            this.btnlimpiarbuscador.Name = "btnlimpiarbuscador";
            this.btnlimpiarbuscador.Size = new System.Drawing.Size(36, 21);
            this.btnlimpiarbuscador.TabIndex = 125;
            this.btnlimpiarbuscador.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnlimpiarbuscador.UseVisualStyleBackColor = false;
            this.btnlimpiarbuscador.Click += new System.EventHandler(this.btnlimpiarbuscador_Click);
            // 
            // txtbusqueda
            // 
            this.txtbusqueda.Location = new System.Drawing.Point(417, 85);
            this.txtbusqueda.Name = "txtbusqueda";
            this.txtbusqueda.Size = new System.Drawing.Size(202, 20);
            this.txtbusqueda.TabIndex = 124;
            this.txtbusqueda.TextChanged += new System.EventHandler(this.txtbusqueda_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(368, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 123;
            this.label11.Text = "Buscar:";
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.HeaderText = "Documento proveedor";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
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
            this.BtnExportar.Location = new System.Drawing.Point(27, 68);
            this.BtnExportar.Name = "BtnExportar";
            this.BtnExportar.Size = new System.Drawing.Size(115, 37);
            this.BtnExportar.TabIndex = 126;
            this.BtnExportar.Text = "Descargar Excel";
            this.BtnExportar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.BtnExportar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.BtnExportar.UseVisualStyleBackColor = false;
            this.BtnExportar.Click += new System.EventHandler(this.BtnExportar_Click);
            // 
            // btnregistrarcompracredito
            // 
            this.btnregistrarcompracredito.BackColor = System.Drawing.Color.White;
            this.btnregistrarcompracredito.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnregistrarcompracredito.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnregistrarcompracredito.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnregistrarcompracredito.ForeColor = System.Drawing.Color.Black;
            this.btnregistrarcompracredito.IconChar = FontAwesome.Sharp.IconChar.Folder;
            this.btnregistrarcompracredito.IconColor = System.Drawing.Color.Black;
            this.btnregistrarcompracredito.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnregistrarcompracredito.IconSize = 16;
            this.btnregistrarcompracredito.Location = new System.Drawing.Point(705, 68);
            this.btnregistrarcompracredito.Name = "btnregistrarcompracredito";
            this.btnregistrarcompracredito.Size = new System.Drawing.Size(115, 37);
            this.btnregistrarcompracredito.TabIndex = 128;
            this.btnregistrarcompracredito.Text = "Registrar credito";
            this.btnregistrarcompracredito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnregistrarcompracredito.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnregistrarcompracredito.UseVisualStyleBackColor = false;
            this.btnregistrarcompracredito.Click += new System.EventHandler(this.btnregistrarcompracredito_Click);
            // 
            // txtTotalSaldoPendiente
            // 
            this.txtTotalSaldoPendiente.Font = new System.Drawing.Font("Microsoft Sans Serif", 34F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotalSaldoPendiente.Location = new System.Drawing.Point(999, 51);
            this.txtTotalSaldoPendiente.Multiline = true;
            this.txtTotalSaldoPendiente.Name = "txtTotalSaldoPendiente";
            this.txtTotalSaldoPendiente.Size = new System.Drawing.Size(359, 54);
            this.txtTotalSaldoPendiente.TabIndex = 129;
            this.txtTotalSaldoPendiente.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(837, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 12);
            this.label3.TabIndex = 130;
            this.label3.Text = "TOTAL CREDITO PENDIENTE";
            // 
            // FrmComprasCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 531);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTotalSaldoPendiente);
            this.Controls.Add(this.btnregistrarcompracredito);
            this.Controls.Add(this.btnregistrarabono);
            this.Controls.Add(this.dgvComprasCredito);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnlimpiarbuscador);
            this.Controls.Add(this.txtbusqueda);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.BtnExportar);
            this.Controls.Add(this.label4);
            this.Name = "FrmComprasCredito";
            this.Text = "FrmComprasCredito";
            this.Load += new System.EventHandler(this.FrmComprasCredito_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvComprasCredito)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FontAwesome.Sharp.IconButton btnregistrarabono;
        private System.Windows.Forms.DataGridView dgvComprasCredito;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private FontAwesome.Sharp.IconButton btnlimpiarbuscador;
        private System.Windows.Forms.TextBox txtbusqueda;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private FontAwesome.Sharp.IconButton BtnExportar;
        private FontAwesome.Sharp.IconButton btnregistrarcompracredito;
        private System.Windows.Forms.TextBox txtTotalSaldoPendiente;
        private System.Windows.Forms.Label label3;
    }
}