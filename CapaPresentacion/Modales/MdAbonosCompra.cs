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

            // Inicializar el ComboBox de métodos de pago
            if (!cmbMetodoPago.Items.Contains("Efectivo"))
            {
                cmbMetodoPago.Items.Insert(0, "Efectivo");
            }

            cmbMetodoPago.SelectedIndex = 0;

            // Inicializar el ComboBox de aplicar interés
            cmbAplicarInteres.Items.Add("Sí");
            cmbAplicarInteres.Items.Add("No");
            cmbAplicarInteres.SelectedIndex = 1; // Por defecto "No"

            cmbInteresAplicadoA.Items.Add("Saldo Pendiente");
            cmbInteresAplicadoA.Items.Add("Monto Total");
            cmbInteresAplicadoA.SelectedIndex = 0;

            // Suscribirse al evento de cambio de selección
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
            TxtNumeroDocumento.Text = numeroDocumento; // Aquí se muestra el NoFactura
            TxtNombreCliente.Text = nombreProveedor;
            TxtMontoTotal.Text = montoTotal.ToString("0.00");
            TxtFechaVencimiento.Text = fechaVencimiento.ToString("dd/MM/yyyy"); // Asegúrate de que el formato sea correcto
            TxtSaldoPendiente.Text = saldoPendiente.ToString("0.00");
            TxtEstado.Text = estado;
        }

        public void SetidCompra(int id)
        {
            idCompra = id; // Almacena el ID de la venta
        }

        private void btnregistrarabono_Click(object sender, EventArgs e)
        {
            decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
            decimal montoAbono = Convert.ToDecimal(txtMontoAbono.Text);

            // Validar que el monto del abono no sea mayor que el saldo pendiente
            if (montoAbono > saldoPendiente)
            {
                MessageBox.Show("El monto del abono no puede ser mayor que el saldo pendiente.");
                return;
            }
            if (TxtEstado.Text == "Liquidada")
            {
                MessageBox.Show("No se puede registrar un abono en una factura liquidada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si la factura está liquidada
            }


            // Calcular el interés si se aplica
            decimal interes = 0;
            if (cmbAplicarInteres.SelectedItem.ToString() == "Sí")
            {
                decimal porcentajeInteres;
                if (!decimal.TryParse(txtPorcentajeInteres.Text, out porcentajeInteres))
                {
                    MessageBox.Show("Ingrese un porcentaje de interés válido.");
                    return; // Salir del método si la conversión falla
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

            // Calcular el nuevo saldo pendiente después de aplicar el abono y el interés
            decimal nuevoSaldoPendiente = saldoPendiente - montoAbono + interes;

            // Crear el nuevo abono
            AbonoCompra nuevoAbono = new AbonoCompra
            {
                IdCompra = idCompra,
                MontoAbono = montoAbono,
                FechaAbono = DateTime.Now,
                NuevoSaldoPendiente = nuevoSaldoPendiente,
                InteresAplicado = interes, // Guardar el interés aplicado
                MetodoPago = cmbMetodoPago.SelectedItem.ToString()
            };

            // Guardar el abono en la base de datos
            string mensaje;
            bool resultado = new CN_AbonoCompra().Registrar(nuevoAbono, out mensaje);

            if (resultado)
            {
                // Actualizar el saldo pendiente en la tabla de compras
                bool saldoActualizado = new CN_Compra().ActualizarSaldoPendiente(idCompra, nuevoSaldoPendiente, out mensaje);

                if (saldoActualizado)
                {
                    MessageBox.Show("Abono registrado correctamente.");
                    CargarHistorialAbonos();
                    LimpiarCampos();
                    txtMontoAbono.Select();

                    // Actualizar el saldo pendiente en el campo del modal
                    TxtSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                    (this.Owner as FrmComprasCredito)?.ActualizarDataGrid();
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
            txtMontoAbono.Text = string.Empty; // Limpiar el campo de monto de abono
            txtNuevoSaldoPendiente.Text = string.Empty; // Limpiar el campo de nuevo saldo pendiente
            txtInteresAplicado.Text = "0.00"; // Restablecer interés a 0
            txtTotalAPagar.Text = string.Empty; // Limpiar el campo de total a pagar
            cmbMetodoPago.SelectedIndex = 0; // Restablecer el método de pago a "Efectivo"
            txtMontoAbono.Select(); // Volver a seleccionar el campo de monto de abono
            txtPorcentajeInteres.Text = "";
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

                    // Calcular el interés si se aplica
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

                            nuevoSaldoPendiente += interes; // Sumar el interés al nuevo saldo
                            txtInteresAplicado.Text = interes.ToString("0.00"); // Mostrar el interés aplicado
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
                        abono.FechaAbono.ToString("dd/MM/yyyy"), // Asegúrate de que el formato sea correcto
                        abono.MontoAbono.ToString("0.00"),
                        abono.NuevoSaldoPendiente.ToString("0.00"),
                        abono.InteresAplicado.ToString("0.00"),
                        abono.MetodoPago ?? "No especificado"
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
                e.Handled = true; // Bloquea caracteres no permitidos
            }

            // Solo permitir un punto decimal
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
