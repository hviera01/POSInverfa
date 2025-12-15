namespace CapaPresentacion
{
    partial class frmFinanzas
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBuscar = new FontAwesome.Sharp.IconButton();
            this.cboUsuario = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboMetodoPago = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnUtilidadBruta = new System.Windows.Forms.Button();
            this.btnUtilidadNeta = new System.Windows.Forms.Button();
            this.btnExcel = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnGananciaProducto = new System.Windows.Forms.Button();
            this.labelTitulo = new System.Windows.Forms.Label();
            this.dgvReporte = new System.Windows.Forms.DataGridView();
            this.labelTotal1 = new System.Windows.Forms.Label();
            this.labelValor2 = new System.Windows.Forms.Label();
            this.labelValor1 = new System.Windows.Forms.Label();
            this.labelTotal3 = new System.Windows.Forms.Label();
            this.labelTotal2 = new System.Windows.Forms.Label();
            this.labelValor3 = new System.Windows.Forms.Label();
            this.btnCierreCaja = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnReporteGeneral = new System.Windows.Forms.Button();
            this.labelValor4 = new System.Windows.Forms.Label();
            this.labelTotal4 = new System.Windows.Forms.Label();
            this.cboEstadoFactura = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboCondicion = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 600);
            this.label1.TabIndex = 29;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cboEstadoFactura);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboCondicion);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.cboUsuario);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboMetodoPago);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.dtpFin);
            this.groupBox1.Controls.Add(this.dtpInicio);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 412);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filtros";
            // 
            // btnBuscar
            // 
            this.btnBuscar.BackColor = System.Drawing.Color.White;
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscar.ForeColor = System.Drawing.Color.Black;
            this.btnBuscar.IconChar = FontAwesome.Sharp.IconChar.Searchengin;
            this.btnBuscar.IconColor = System.Drawing.Color.Black;
            this.btnBuscar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscar.IconSize = 18;
            this.btnBuscar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscar.Location = new System.Drawing.Point(40, 364);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(77, 27);
            this.btnBuscar.TabIndex = 104;
            this.btnBuscar.Text = "BUSCAR";
            this.btnBuscar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // cboUsuario
            // 
            this.cboUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUsuario.FormattingEnabled = true;
            this.cboUsuario.Location = new System.Drawing.Point(17, 191);
            this.cboUsuario.Name = "cboUsuario";
            this.cboUsuario.Size = new System.Drawing.Size(118, 21);
            this.cboUsuario.TabIndex = 103;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(14, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 102;
            this.label4.Text = "Usuario";
            // 
            // cboMetodoPago
            // 
            this.cboMetodoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetodoPago.FormattingEnabled = true;
            this.cboMetodoPago.Location = new System.Drawing.Point(16, 136);
            this.cboMetodoPago.Name = "cboMetodoPago";
            this.cboMetodoPago.Size = new System.Drawing.Size(118, 21);
            this.cboMetodoPago.TabIndex = 101;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(13, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 13);
            this.label11.TabIndex = 100;
            this.label11.Text = "Metodo de pago";
            // 
            // dtpFin
            // 
            this.dtpFin.CustomFormat = "dd/MM/yyyy";
            this.dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFin.Location = new System.Drawing.Point(16, 83);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(101, 20);
            this.dtpFin.TabIndex = 97;
            // 
            // dtpInicio
            // 
            this.dtpInicio.CustomFormat = "dd/MM/yyyy";
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(16, 38);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(101, 20);
            this.dtpInicio.TabIndex = 95;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(16, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 98;
            this.label2.Text = "Fecha fin:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(29, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 96;
            this.label3.Text = "Fecha inicio:";
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.White;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(165, 0);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label10.Size = new System.Drawing.Size(1268, 89);
            this.label10.TabIndex = 97;
            this.label10.Text = "REPORTES FINANCIEROS";
            // 
            // btnUtilidadBruta
            // 
            this.btnUtilidadBruta.Location = new System.Drawing.Point(187, 49);
            this.btnUtilidadBruta.Name = "btnUtilidadBruta";
            this.btnUtilidadBruta.Size = new System.Drawing.Size(134, 31);
            this.btnUtilidadBruta.TabIndex = 98;
            this.btnUtilidadBruta.Text = "UTILIDAD BRUTA";
            this.btnUtilidadBruta.UseVisualStyleBackColor = true;
            this.btnUtilidadBruta.Click += new System.EventHandler(this.btnUtilidadBruta_Click);
            // 
            // btnUtilidadNeta
            // 
            this.btnUtilidadNeta.Location = new System.Drawing.Point(372, 49);
            this.btnUtilidadNeta.Name = "btnUtilidadNeta";
            this.btnUtilidadNeta.Size = new System.Drawing.Size(134, 31);
            this.btnUtilidadNeta.TabIndex = 99;
            this.btnUtilidadNeta.Text = "UTILIDAD NETA";
            this.btnUtilidadNeta.UseVisualStyleBackColor = true;
            this.btnUtilidadNeta.Click += new System.EventHandler(this.btnUtilidadNeta_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(1185, 49);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(134, 31);
            this.btnExcel.TabIndex = 100;
            this.btnExcel.Text = "EXPORTAR EXCEL";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(997, 49);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 31);
            this.button4.TabIndex = 101;
            this.button4.Text = "EXPORTAR PDF";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnGananciaProducto
            // 
            this.btnGananciaProducto.Location = new System.Drawing.Point(560, 49);
            this.btnGananciaProducto.Name = "btnGananciaProducto";
            this.btnGananciaProducto.Size = new System.Drawing.Size(166, 31);
            this.btnGananciaProducto.TabIndex = 102;
            this.btnGananciaProducto.Text = "GANANCIA POR PRODUCTO";
            this.btnGananciaProducto.UseVisualStyleBackColor = true;
            this.btnGananciaProducto.Click += new System.EventHandler(this.btnGananciaProducto_Click);
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitulo.Location = new System.Drawing.Point(219, 92);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(109, 39);
            this.labelTitulo.TabIndex = 103;
            this.labelTitulo.Text = "label5";
            // 
            // dgvReporte
            // 
            this.dgvReporte.AllowUserToAddRows = false;
            this.dgvReporte.BackgroundColor = System.Drawing.Color.White;
            this.dgvReporte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReporte.Location = new System.Drawing.Point(176, 134);
            this.dgvReporte.MultiSelect = false;
            this.dgvReporte.Name = "dgvReporte";
            this.dgvReporte.ReadOnly = true;
            this.dgvReporte.Size = new System.Drawing.Size(1182, 336);
            this.dgvReporte.TabIndex = 104;
            // 
            // labelTotal1
            // 
            this.labelTotal1.AutoSize = true;
            this.labelTotal1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal1.Location = new System.Drawing.Point(866, 473);
            this.labelTotal1.Name = "labelTotal1";
            this.labelTotal1.Size = new System.Drawing.Size(79, 29);
            this.labelTotal1.TabIndex = 105;
            this.labelTotal1.Text = "label6";
            // 
            // labelValor2
            // 
            this.labelValor2.AutoSize = true;
            this.labelValor2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValor2.Location = new System.Drawing.Point(1196, 498);
            this.labelValor2.Name = "labelValor2";
            this.labelValor2.Size = new System.Drawing.Size(85, 29);
            this.labelValor2.TabIndex = 106;
            this.labelValor2.Text = "label7";
            // 
            // labelValor1
            // 
            this.labelValor1.AutoSize = true;
            this.labelValor1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValor1.Location = new System.Drawing.Point(1196, 473);
            this.labelValor1.Name = "labelValor1";
            this.labelValor1.Size = new System.Drawing.Size(85, 29);
            this.labelValor1.TabIndex = 107;
            this.labelValor1.Text = "label8";
            // 
            // labelTotal3
            // 
            this.labelTotal3.AutoSize = true;
            this.labelTotal3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal3.Location = new System.Drawing.Point(866, 522);
            this.labelTotal3.Name = "labelTotal3";
            this.labelTotal3.Size = new System.Drawing.Size(79, 29);
            this.labelTotal3.TabIndex = 108;
            this.labelTotal3.Text = "label9";
            // 
            // labelTotal2
            // 
            this.labelTotal2.AutoSize = true;
            this.labelTotal2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal2.Location = new System.Drawing.Point(866, 498);
            this.labelTotal2.Name = "labelTotal2";
            this.labelTotal2.Size = new System.Drawing.Size(92, 29);
            this.labelTotal2.TabIndex = 109;
            this.labelTotal2.Text = "label12";
            // 
            // labelValor3
            // 
            this.labelValor3.AutoSize = true;
            this.labelValor3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValor3.Location = new System.Drawing.Point(1196, 522);
            this.labelValor3.Name = "labelValor3";
            this.labelValor3.Size = new System.Drawing.Size(99, 29);
            this.labelValor3.TabIndex = 110;
            this.labelValor3.Text = "label13";
            // 
            // btnCierreCaja
            // 
            this.btnCierreCaja.Location = new System.Drawing.Point(779, 49);
            this.btnCierreCaja.Name = "btnCierreCaja";
            this.btnCierreCaja.Size = new System.Drawing.Size(166, 31);
            this.btnCierreCaja.TabIndex = 111;
            this.btnCierreCaja.Text = "CIERRES DE CAJA";
            this.btnCierreCaja.UseVisualStyleBackColor = true;
            this.btnCierreCaja.Click += new System.EventHandler(this.btnCierreCaja_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 447);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 39);
            this.button1.TabIndex = 112;
            this.button1.Text = "PDF DETALLE CIERRE DE CAJA";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnReporteGeneral
            // 
            this.btnReporteGeneral.Location = new System.Drawing.Point(13, 501);
            this.btnReporteGeneral.Name = "btnReporteGeneral";
            this.btnReporteGeneral.Size = new System.Drawing.Size(134, 31);
            this.btnReporteGeneral.TabIndex = 113;
            this.btnReporteGeneral.Text = "REPORTE GENERAL";
            this.btnReporteGeneral.UseVisualStyleBackColor = true;
            this.btnReporteGeneral.Click += new System.EventHandler(this.btnReporteGeneral_Click);
            // 
            // labelValor4
            // 
            this.labelValor4.AutoSize = true;
            this.labelValor4.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValor4.Location = new System.Drawing.Point(1196, 546);
            this.labelValor4.Name = "labelValor4";
            this.labelValor4.Size = new System.Drawing.Size(99, 29);
            this.labelValor4.TabIndex = 114;
            this.labelValor4.Text = "label13";
            // 
            // labelTotal4
            // 
            this.labelTotal4.AutoSize = true;
            this.labelTotal4.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal4.Location = new System.Drawing.Point(866, 546);
            this.labelTotal4.Name = "labelTotal4";
            this.labelTotal4.Size = new System.Drawing.Size(79, 29);
            this.labelTotal4.TabIndex = 115;
            this.labelTotal4.Text = "label9";
            // 
            // cboEstadoFactura
            // 
            this.cboEstadoFactura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstadoFactura.FormattingEnabled = true;
            this.cboEstadoFactura.Location = new System.Drawing.Point(16, 310);
            this.cboEstadoFactura.Name = "cboEstadoFactura";
            this.cboEstadoFactura.Size = new System.Drawing.Size(118, 21);
            this.cboEstadoFactura.TabIndex = 112;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(13, 294);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 111;
            this.label5.Text = "Estado de la factura";
            // 
            // cboCondicion
            // 
            this.cboCondicion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCondicion.FormattingEnabled = true;
            this.cboCondicion.Location = new System.Drawing.Point(16, 249);
            this.cboCondicion.Name = "cboCondicion";
            this.cboCondicion.Size = new System.Drawing.Size(118, 21);
            this.cboCondicion.TabIndex = 110;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(13, 233);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 109;
            this.label6.Text = "Condicion";
            // 
            // frmFinanzas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 600);
            this.Controls.Add(this.labelTotal4);
            this.Controls.Add(this.labelValor4);
            this.Controls.Add(this.btnReporteGeneral);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnCierreCaja);
            this.Controls.Add(this.labelValor3);
            this.Controls.Add(this.labelTotal2);
            this.Controls.Add(this.labelTotal3);
            this.Controls.Add(this.labelValor1);
            this.Controls.Add(this.labelValor2);
            this.Controls.Add(this.labelTotal1);
            this.Controls.Add(this.dgvReporte);
            this.Controls.Add(this.labelTitulo);
            this.Controls.Add(this.btnGananciaProducto);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnExcel);
            this.Controls.Add(this.btnUtilidadNeta);
            this.Controls.Add(this.btnUtilidadBruta);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "frmFinanzas";
            this.Text = "frmFinanzas";
            this.Load += new System.EventHandler(this.frmFinanzas_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboMetodoPago;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cboUsuario;
        private System.Windows.Forms.Label label4;
        private FontAwesome.Sharp.IconButton btnBuscar;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnUtilidadBruta;
        private System.Windows.Forms.Button btnUtilidadNeta;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnGananciaProducto;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.DataGridView dgvReporte;
        private System.Windows.Forms.Label labelTotal1;
        private System.Windows.Forms.Label labelValor2;
        private System.Windows.Forms.Label labelValor1;
        private System.Windows.Forms.Label labelTotal3;
        private System.Windows.Forms.Label labelTotal2;
        private System.Windows.Forms.Label labelValor3;
        private System.Windows.Forms.Button btnCierreCaja;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnReporteGeneral;
        private System.Windows.Forms.Label labelValor4;
        private System.Windows.Forms.Label labelTotal4;
        private System.Windows.Forms.ComboBox cboEstadoFactura;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboCondicion;
        private System.Windows.Forms.Label label6;
    }
}