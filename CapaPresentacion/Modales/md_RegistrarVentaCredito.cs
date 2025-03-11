using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Modales
{
    public partial class md_RegistrarVentaCredito : Form
    {
        public md_RegistrarVentaCredito()
        {
            InitializeComponent();
        }

        private void md_RegistrarVentaCredito_Load(object sender, EventArgs e)
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
            using (var modal = new mdCliente())
            {
                var result = modal.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Rellenar los campos con la información del cliente seleccionado
                    txtcliente.Text = modal._Cliente.NombreCompleto;
                    txtrtncliente.Text = modal._Cliente.Documento;
                }
            }
        }

        private void btnregistrarcredito_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtnofactura.Text))
            {
                txtnofactura.Text = "N/A"; // Asignar "N/A" si el campo está vacío
            }

            if (string.IsNullOrWhiteSpace(txtcliente.Text))
            {
                MessageBox.Show("Por favor, seleccione un cliente o ingrese el nombre del cliente.", "Cliente Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si no hay cliente
            }

            // Crear el objeto de venta
            Venta nuevaVenta = new Venta
            {
                NumeroDocumento = txtnofactura.Text,
                NombreCliente = txtcliente.Text,
                DocumentoCliente = string.IsNullOrWhiteSpace(txtrtncliente.Text) ? "N/A" : txtrtncliente.Text, // Asignar "N/A" si el RTN está vacío
                MontoTotal = Convert.ToDecimal(txtmontototal.Text),
                SaldoPendiente = Convert.ToDecimal(txtsaldopendiente.Text),
                FechaVencimiento = dtpfechavencimiento.Value,
                MetodoPago = "N/A", // Método de pago
                Condicion = "Credito", // Condición
                FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                TipoDocumento = "Factura" // Asegúrate de establecer un valor para TipoDocumento
            };

            // Guardar la venta en la base de datos
            string mensaje;
            bool resultado = new CN_Venta().RegistrarCredito(nuevaVenta, out mensaje); // Asegúrate de tener este método en CN_Venta

            if (resultado)
            {
                MessageBox.Show("Credito registrado correctamente.");
                this.DialogResult = DialogResult.OK; // Establecer el resultado del diálogo
                this.Close(); // Cerrar el modal
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
                e.Handled = true; // Evitar la entrada
            }

            // Permitir solo un punto decimal
            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true; // Evitar la entrada
            }
        }

        private void txtsaldopendiente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true; // Evitar la entrada
            }

            // Permitir solo un punto decimal
            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true; // Evitar la entrada
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
