using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Modales
{
    public partial class Md_RegistrarCredito : Form
    {
        public Md_RegistrarCredito()
        {
            InitializeComponent();
        }

        private void Md_RegistrarCredito_Load(object sender, EventArgs e)
        {
            txtnofactura.Select();

            foreach (System.Windows.Forms.Control ctrl in this.Controls)

            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProveedor())
            {
                var result = modal.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtproveedor.Text = modal._Proveedor.RazonSocial;
                    txtrtnproveedor.Text = modal._Proveedor.Documento;
                    txtidproveedor.Text = modal._Proveedor.IdProveedor.ToString(); 
                }
            }
        }

        private void btnregistrarcredito_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtmontototal.Text) ||
        string.IsNullOrWhiteSpace(txtsaldopendiente.Text) ||
        string.IsNullOrWhiteSpace(dtpfechavencimiento.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.", "Campos Obligatorios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

          
            string fechaVencimiento = dtpfechavencimiento.Value.ToString("dd/MM/yyyy"); 
            string fechaRegistro = DateTime.Now.ToString("dd/MM/yyyy");

           
            Compra nuevaCompra = new Compra
            {
                NoFactura = string.IsNullOrWhiteSpace(txtnofactura.Text) ? "N/A" : txtnofactura.Text,
                oProveedor = new Proveedor
                {
                    IdProveedor = string.IsNullOrWhiteSpace(txtidproveedor.Text) ? 0 : Convert.ToInt32(txtidproveedor.Text),
                    RazonSocial = string.IsNullOrWhiteSpace(txtproveedor.Text) ? "N/A" : txtproveedor.Text,
                    Documento = string.IsNullOrWhiteSpace(txtrtnproveedor.Text) ? "N/A" : txtrtnproveedor.Text
                },
                MontoTotal = Convert.ToDecimal(txtmontototal.Text),
                SaldoPendiente = Convert.ToDecimal(txtsaldopendiente.Text),
                FechaVencimiento = fechaVencimiento, 
                MetodoPago = "N/A",
                Condicion = "Credito",
                FechaRegistro = fechaRegistro, 
                TipoDocumento = "Factura",
                NumeroDocumento = "N/A"
            };

           
            string mensaje;
            bool resultado = new CN_Compra().RegistrarCredito(nuevaCompra, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Compra registrada correctamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtmontototal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true; 
            }

            
            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true; 
            }
        }

        private void txtsaldopendiente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true; 
            }

           
            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtrtnproveedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
