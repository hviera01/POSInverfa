using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Modales
{
    public partial class MdAbonosCompra : Form
    {
        private int idCompra;
        public MdAbonosCompra(int idCompra)
        {
            InitializeComponent();
            this.idCompra = idCompra;
        }

        private void MdAbonosCompra_Load(object sender, EventArgs e)
        {
            CargarHistorialAbonos();
            txtMontoAbono.Select();

          
            if (!cmbMetodoPago.Items.Contains("Efectivo"))
            {
                cmbMetodoPago.Items.Insert(0, "Efectivo");
            }

            cmbMetodoPago.SelectedIndex = 0;

           
            cmbAplicarInteres.Items.Add("Sí");
            cmbAplicarInteres.Items.Add("No");
            cmbAplicarInteres.SelectedIndex = 1; 

            cmbInteresAplicadoA.Items.Add("Saldo Pendiente");
            cmbInteresAplicadoA.Items.Add("Monto Total");
            cmbInteresAplicadoA.SelectedIndex = 0;

            cmbAplicarInteres.SelectedIndexChanged += cmbAplicarInteres_SelectedIndexChanged;
            cmbAplicarInteres_SelectedIndexChanged(null, null);

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
        public void SetFacturaData(string numeroDocumento, string nombreProveedor, decimal montoTotal, DateTime fechaVencimiento, decimal saldoPendiente, string estado)
        {
            TxtNumeroDocumento.Text = numeroDocumento; 
            TxtNombreCliente.Text = nombreProveedor;
            TxtMontoTotal.Text = montoTotal.ToString("0.00");
            TxtFechaVencimiento.Text = fechaVencimiento.ToString("dd/MM/yyyy"); 
            TxtSaldoPendiente.Text = saldoPendiente.ToString("0.00");
            TxtEstado.Text = estado;
        }

        public void SetidCompra(int id)
        {
            idCompra = id; 
        }

        private void btnregistrarabono_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMontoAbono.Text))
            {
                MessageBox.Show("Por favor, ingrese un monto de abono.", "Campo Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoAbono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtSaldoPendiente.Text))
            {
                MessageBox.Show("El saldo pendiente no puede estar vacío.", "Campo Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoAbono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTotalAPagar.Text))
            {
                MessageBox.Show("Debe de presionar enter despues de poner el monto del abono.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoAbono.Focus();
                return;
            }

            if (cmbMetodoPago.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un método de pago.", "Campo Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbMetodoPago.Focus();
                return;
            }


            if (string.IsNullOrWhiteSpace(txtMontoAbono.Text))
            {
                MessageBox.Show("Por favor, ingrese un monto de abono.", "Campo Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (TxtEstado.Text == "Liquidada")
            {
                MessageBox.Show("No se puede registrar un abono en una factura liquidada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
            decimal montoAbono = Convert.ToDecimal(txtMontoAbono.Text);

           
            if (montoAbono > saldoPendiente)
            {
                MessageBox.Show("El monto del abono no puede ser mayor que el saldo pendiente.");
                return;
            }
            if (TxtEstado.Text == "Liquidada")
            {
                MessageBox.Show("No se puede registrar un abono en una factura liquidada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            if (string.IsNullOrWhiteSpace(txtrecibo.Text))
            {
                txtrecibo.Text = "N/A";
            }

            decimal interes = 0;
            if (cmbAplicarInteres.SelectedItem.ToString() == "Sí")
            {
                decimal porcentajeInteres;
                if (!decimal.TryParse(txtPorcentajeInteres.Text, out porcentajeInteres))
                {
                    MessageBox.Show("Ingrese un porcentaje de interés válido.");
                    return; 
                }

                if (cmbInteresAplicadoA.SelectedItem.ToString() == "Saldo Pendiente")
                {
                    interes = (saldoPendiente * porcentajeInteres) / 100;
                }
                else if (cmbInteresAplicadoA.SelectedItem.ToString() == "Monto Total")
                {
                    decimal montoTotal = Convert.ToDecimal(TxtMontoTotal.Text);
                    interes = (montoTotal * porcentajeInteres) / 100;
                }
            }

           
            decimal nuevoSaldoPendiente = saldoPendiente - montoAbono + interes;


            AbonoCompra nuevoAbono = new AbonoCompra
            {
                IdCompra = idCompra,
                MontoAbono = montoAbono,
                FechaAbono = DateTime.Now,
                NuevoSaldoPendiente = nuevoSaldoPendiente,
                InteresAplicado = interes,
                MetodoPago = cmbMetodoPago.SelectedItem.ToString(),
                NoRecibo = txtrecibo.Text
            }; 

            
            string mensaje;
            bool resultado = new CN_AbonoCompra().Registrar(nuevoAbono, out mensaje);

            if (resultado)
            {
                
                bool saldoActualizado = new CN_Compra().ActualizarSaldoPendiente(idCompra, nuevoSaldoPendiente, out mensaje);

                if (saldoActualizado)
                {
                    MessageBox.Show("Abono registrado correctamente.");
                    CargarHistorialAbonos();
                    LimpiarCampos();
                    txtMontoAbono.Select();

                   
                    TxtSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                    (this.Owner as FrmComprasCredito)?.RefrescarFormulario();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el saldo pendiente: " + mensaje);
                }
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LimpiarCampos()
        {
            txtMontoAbono.Text = string.Empty; 
            txtNuevoSaldoPendiente.Text = string.Empty; 
            txtInteresAplicado.Text = "0.00";
            txtTotalAPagar.Text = string.Empty; 
            cmbMetodoPago.SelectedIndex = 0; 
            txtMontoAbono.Select(); 
            txtPorcentajeInteres.Text = "";
            txtrecibo.Text = "";
        }

        private void txtMontoAbono_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal montoAbono;
                if (decimal.TryParse(txtMontoAbono.Text, out montoAbono))
                {
                    decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
                    decimal nuevoSaldoPendiente = saldoPendiente - montoAbono;

                   
                    if (cmbAplicarInteres.SelectedItem.ToString() == "Sí")
                    {
                        decimal porcentajeInteres;
                        if (decimal.TryParse(txtPorcentajeInteres.Text, out porcentajeInteres))
                        {
                            decimal interes = 0;

                            if (cmbInteresAplicadoA.SelectedItem.ToString() == "Saldo Pendiente")
                            {
                                interes = (saldoPendiente * porcentajeInteres) / 100;
                            }
                            else if (cmbInteresAplicadoA.SelectedItem.ToString() == "Monto Total")
                            {
                                decimal montoTotal = Convert.ToDecimal(TxtMontoTotal.Text);
                                interes = (montoTotal * porcentajeInteres) / 100;
                            }

                            nuevoSaldoPendiente += interes;
                            txtInteresAplicado.Text = interes.ToString("0.00"); 
                        }
                    }

                    txtNuevoSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                    txtTotalAPagar.Text = nuevoSaldoPendiente.ToString("0.00");
                }
                else
                {
                    MessageBox.Show("Ingrese un monto válido para el abono.");
                }
            }
        }

        private void CargarHistorialAbonos()
        {
            if (idCompra <= 0)
            {
                MessageBox.Show("ID de compra no válido.");
                return;
            }

            try
            {
                var historialAbonos = new CN_AbonoCompra().ObtenerHistorialAbonos(idCompra);
                dgvHistorial.Rows.Clear();

                foreach (var abono in historialAbonos)
                {
                    dgvHistorial.Rows.Add(
                        abono.FechaAbono.ToString("dd/MM/yyyy"), 
                        abono.MontoAbono.ToString("0.00"),
                        abono.NuevoSaldoPendiente.ToString("0.00"),
                        abono.InteresAplicado.ToString("0.00"),
                        abono.MetodoPago ?? "No especificado"
                        , abono.NoRecibo ?? "N/A"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial de abonos: " + ex.Message);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtMontoAbono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; 
            }

           
            if (e.KeyChar == '.' && txtMontoAbono.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void cmbAplicarInteres_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool aplicarInteres = cmbAplicarInteres.SelectedItem.ToString() == "Sí";
            txtPorcentajeInteres.Enabled = aplicarInteres;
            cmbInteresAplicadoA.Enabled = aplicarInteres;
        }
    }
}
