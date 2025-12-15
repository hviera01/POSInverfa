namespace CapaPresentacion.Modales
{
    partial class md_CobrarFactura
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCambio = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPagoCon = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCobrarFactura = new FontAwesome.Sharp.IconButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCambio);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtPagoCon);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTotal);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(19, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(342, 196);
            this.groupBox1.TabIndex = 89;
            this.groupBox1.TabStop = false;
            // 
            // txtCambio
            // 
            this.txtCambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCambio.Location = new System.Drawing.Point(114, 140);
            this.txtCambio.Name = "txtCambio";
            this.txtCambio.ReadOnly = true;
            this.txtCambio.Size = new System.Drawing.Size(218, 31);
            this.txtCambio.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "CAMBIO";
            // 
            // txtPagoCon
            // 
            this.txtPagoCon.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPagoCon.Location = new System.Drawing.Point(114, 80);
            this.txtPagoCon.Name = "txtPagoCon";
            this.txtPagoCon.Size = new System.Drawing.Size(218, 31);
            this.txtPagoCon.TabIndex = 3;
            this.txtPagoCon.TextChanged += new System.EventHandler(this.txtPagoCon_TextChanged);
            this.txtPagoCon.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPagoCon_KeyDown);
            this.txtPagoCon.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPagoCon_KeyPress);
            this.txtPagoCon.Leave += new System.EventHandler(this.txtPagoCon_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "PAGO CON";
            // 
            // txtTotal
            // 
            this.txtTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(114, 24);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(218, 31);
            this.txtTotal.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "TOTAL";
            // 
            // btnCobrarFactura
            // 
            this.btnCobrarFactura.BackColor = System.Drawing.Color.ForestGreen;
            this.btnCobrarFactura.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCobrarFactura.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCobrarFactura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCobrarFactura.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCobrarFactura.ForeColor = System.Drawing.Color.White;
            this.btnCobrarFactura.IconChar = FontAwesome.Sharp.IconChar.HandHoldingHeart;
            this.btnCobrarFactura.IconColor = System.Drawing.Color.White;
            this.btnCobrarFactura.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCobrarFactura.IconSize = 20;
            this.btnCobrarFactura.Location = new System.Drawing.Point(38, 219);
            this.btnCobrarFactura.Name = "btnCobrarFactura";
            this.btnCobrarFactura.Size = new System.Drawing.Size(313, 51);
            this.btnCobrarFactura.TabIndex = 90;
            this.btnCobrarFactura.Text = "COBRAR FACTURA";
            this.btnCobrarFactura.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCobrarFactura.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCobrarFactura.UseVisualStyleBackColor = false;
            this.btnCobrarFactura.Click += new System.EventHandler(this.btnCobrarFactura_Click);
            // 
            // md_CobrarFactura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 287);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCobrarFactura);
            this.Name = "md_CobrarFactura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "COBRAR FACTURA";
            this.Load += new System.EventHandler(this.md_CobrarFactura_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCambio;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPagoCon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label1;
        private FontAwesome.Sharp.IconButton btnCobrarFactura;
    }
}