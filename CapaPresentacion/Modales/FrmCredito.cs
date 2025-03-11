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
    public partial class FrmCredito : Form
    {
        private int idVenta;
        public FrmCredito()
        {
            InitializeComponent();
        }

        private void FrmCredito_Load(object sender, EventArgs e)
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
            cmbInteresAplicadoA.SelectedIndex = 0; // Por defecto "Saldo Pendiente"

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
        public void SetFacturaData(string numeroDocumento, string nombreCliente, decimal montoTotal, DateTime fechaVencimiento, decimal saldoPendiente, string estado)
        {
            TxtNumeroDocumento.Text = numeroDocumento;
            TxtNombreCliente.Text = nombreCliente;
            TxtMontoTotal.Text = montoTotal.ToString("0.00");

            // Formatear la fecha de vencimiento en el formato dd/MM/yyyy
            TxtFechaVencimiento.Text = fechaVencimiento.ToString("dd/MM/yyyy"); // Asegúrate de que el formato sea correcto

            TxtSaldoPendiente.Text = saldoPendiente.ToString("0.00");
            TxtEstado.Text = estado;
        }

        public void SetIdVenta(int id)
        {
            idVenta = id; // Almacena el ID de la venta
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

                    // Calcular interés si la fecha de vencimiento ha pasado
                    decimal interesAplicado = 0;
                    DateTime fechaVencimiento;

                    // Intenta convertir la fecha de vencimiento usando ParseExact
                    try
                    {
                        fechaVencimiento = DateTime.ParseExact(TxtFechaVencimiento.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (DateTime.Now > fechaVencimiento)
                        {
                            interesAplicado = saldoPendiente * 0.03m; // 4% de interés
                        }
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("La fecha de vencimiento no es válida.");
                        return; // Salir del método si la fecha no es válida
                    }

                    txtNuevoSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                    txtInteresAplicado.Text = interesAplicado.ToString("0.00");
                    txtTotalAPagar.Text = (nuevoSaldoPendiente + interesAplicado).ToString("0.00");
                }
                else
                {
                    MessageBox.Show("Ingrese un monto válido para el abono.");
                }
            }
        }

        private void CargarHistorialAbonos()
        {
            if (idVenta <= 0)
            {
                MessageBox.Show("ID de venta no válido.");
                return;
            }

            try
            {
                var historialAbonos = new CN_Abono().ObtenerHistorialAbonos(idVenta);
                dgvHistorial.Rows.Clear();

                // Obtener el saldo pendiente original
                decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);

                // Actualizar el TextBox del saldo pendiente
                TxtSaldoPendiente.Text = saldoPendiente.ToString("0.00");

                foreach (var abono in historialAbonos)
                {
                    dgvHistorial.Rows.Add(
                        abono.FechaAbono.ToString("dd/MM/yyyy"),
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


        private void btnregistrarabono_Click(object sender, EventArgs e)
        {
            decimal montoAbono = Convert.ToDecimal(txtMontoAbono.Text);
            decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
            decimal TotalPagar = Convert.ToDecimal(txtTotalAPagar.Text);

            // Validar que el monto del abono no sea mayor que el saldo pendiente
            if (montoAbono > saldoPendiente)
            {
                MessageBox.Show("El monto del abono no puede ser mayor que el saldo pendiente.");
                return;
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

            // Crear un objeto Abono
            Abono nuevoAbono = new Abono
            {
                IdVenta = idVenta, // Asigna el ID de la venta
                MontoAbono = montoAbono,
                FechaAbono = DateTime.Now,
                NuevoSaldoPendiente = nuevoSaldoPendiente,
                InteresAplicado = interes, // Guardar el interés aplicado
                MetodoPago = cmbMetodoPago.SelectedItem.ToString()
            };

            // Guardar el abono en la base de datos
            string mensaje;
            bool resultado = new CN_Abono().Registrar(nuevoAbono, out mensaje);

            if (resultado)
            {
                // Actualizar el saldo pendiente en la tabla de ventas
                new CN_Venta().ActualizarSaldoPendiente(idVenta, nuevoSaldoPendiente);

                // Actualizar los TextBox
                TxtSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                MessageBox.Show("Abono registrado correctamente.");

                // Actualizar el DataGridView en el formulario modal
                CargarHistorialAbonos(); // Actualiza el DataGridView del modal

                // Actualizar el DataGridView en el formulario padre
                if (this.Owner is FrmVentasCredito ventasCreditoForm)
                {
                    ventasCreditoForm.ActualizarSaldoPendiente(idVenta, nuevoSaldoPendiente);
                    ventasCreditoForm.CargarFacturasCredito(); // Asegúrate de que este método esté disponible
                }

                // Limpiar campos
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Error al registrar el abono: " + mensaje);
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

        private void LimpiarCampos()
        {
            txtMontoAbono.Text = "";
            txtNuevoSaldoPendiente.Text = "";
            txtInteresAplicado.Text = "0.00"; // Restablecer interés a 0
            txtTotalAPagar.Text = "";
            cmbMetodoPago.SelectedIndex = 0; // Restablecer el método de pago a "Efectivo"
            txtMontoAbono.Select(); // Volver a seleccionar el campo de monto de abono
            txtPorcentajeInteres.Text = "";
        }

        private void txtMontoAbono_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal montoAbono;
                if (decimal.TryParse(txtMontoAbono.Text, out montoAbono))
                {
                    decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
                    decimal nuevoSaldoPendiente = saldoPendiente - montoAbono;

                    // Eliminar el cálculo automático de interés basado en la fecha de vencimiento
                    decimal interesAplicado = 0;

                    // Solo calcular interés si el usuario ha indicado que se aplique
                    if (cmbAplicarInteres.SelectedItem.ToString() == "Sí")
                    {
                        decimal porcentajeInteres;
                        if (decimal.TryParse(txtPorcentajeInteres.Text, out porcentajeInteres))
                        {
                            if (cmbInteresAplicadoA.SelectedItem.ToString() == "Saldo Pendiente")
                            {
                                interesAplicado = (saldoPendiente * porcentajeInteres) / 100;
                            }
                            else if (cmbInteresAplicadoA.SelectedItem.ToString() == "Monto Total")
                            {
                                decimal montoTotal = Convert.ToDecimal(TxtMontoTotal.Text);
                                interesAplicado = (montoTotal * porcentajeInteres) / 100;
                            }
                        }
                    }

                    txtNuevoSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                    txtInteresAplicado.Text = interesAplicado.ToString("0.00");
                    txtTotalAPagar.Text = (nuevoSaldoPendiente + interesAplicado).ToString("0.00");
                }
                else
                {
                    MessageBox.Show("Ingrese un monto válido para el abono.");
                }
            }
        }

        private void CalcularNuevoSaldoPendiente()
        {
            decimal montoAbono;
            if (decimal.TryParse(txtMontoAbono.Text, out montoAbono))
            {
                decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
                decimal nuevoSaldoPendiente = saldoPendiente - montoAbono;

                // Calcular interés si la fecha de vencimiento ha pasado
                decimal interesAplicado = 0;
                DateTime fechaVencimiento;

                // Intenta convertir la fecha de vencimiento usando ParseExact
                if (DateTime.TryParseExact(TxtFechaVencimiento.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaVencimiento))
                {
                    if (DateTime.Now > fechaVencimiento)
                    {
                        interesAplicado = saldoPendiente * 0.03m; // 3% de interés
                    }
                }
                else
                {
                    MessageBox.Show("La fecha de vencimiento no es válida.");
                    return; // Salir del método si la fecha no es válida
                }

                txtNuevoSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                txtInteresAplicado.Text = interesAplicado.ToString("0.00");
                txtTotalAPagar.Text = (nuevoSaldoPendiente + interesAplicado).ToString("0.00");
            }
            else
            {
                MessageBox.Show("Ingrese un monto válido para el abono.");
            }
        }

        private void btnregistrarabono_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMontoAbono.Text))
            {
                MessageBox.Show("Por favor, ingrese un monto de abono.", "Campo Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (TxtEstado.Text == "Liquidada")
            {
                MessageBox.Show("No se puede registrar un abono en una factura liquidada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si la factura está liquidada
            }


            decimal montoAbono;
            if (!decimal.TryParse(txtMontoAbono.Text, out montoAbono))
            {
                MessageBox.Show("Ingrese un monto válido para el abono.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal saldoPendiente = Convert.ToDecimal(TxtSaldoPendiente.Text);
            decimal TotalPagar = Convert.ToDecimal(txtTotalAPagar.Text);

            // Validar que el monto del abono no sea mayor que el saldo pendiente
            if (montoAbono > saldoPendiente)
            {
                MessageBox.Show("El monto del abono no puede ser mayor que el saldo pendiente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calcular el interés si se aplica
            decimal interes = 0;
            if (cmbAplicarInteres.SelectedItem.ToString() == "Sí")
            {
                decimal porcentajeInteres;
                if (!decimal.TryParse(txtPorcentajeInteres.Text, out porcentajeInteres))
                {
                    MessageBox.Show("Ingrese un porcentaje de interés válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Crear un objeto Abono
            Abono nuevoAbono = new Abono
            {
                IdVenta = idVenta, // Asigna el ID de la venta
                MontoAbono = montoAbono,
                FechaAbono = DateTime.Now,
                NuevoSaldoPendiente = nuevoSaldoPendiente,
                InteresAplicado = interes, // Guardar el interés aplicado
                MetodoPago = cmbMetodoPago.SelectedItem.ToString()
            };

            // Guardar el abono en la base de datos
            string mensaje;
            bool resultado = new CN_Abono().Registrar(nuevoAbono, out mensaje);

            if (resultado)
            {
                // Actualizar el saldo pendiente en la tabla de ventas
                new CN_Venta().ActualizarSaldoPendiente(idVenta, nuevoSaldoPendiente);

                // Actualizar los TextBox
                TxtSaldoPendiente.Text = nuevoSaldoPendiente.ToString("0.00");
                MessageBox.Show("Abono registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizar el DataGridView en el formulario modal
                CargarHistorialAbonos(); // Actualiza el DataGridView del modal

                // Actualizar el DataGridView en el formulario padre
                if (this.Owner is FrmVentasCredito ventasCreditoForm)
                {
                    ventasCreditoForm.ActualizarSaldoPendiente(idVenta, nuevoSaldoPendiente);
                    ventasCreditoForm.CargarFacturasCredito(); // Asegúrate de que este método esté disponible
                }

                // Limpiar campos
                LimpiarCampos();
                txtMontoAbono.Select();
            }
            else
            {
                MessageBox.Show("Error al registrar el abono: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
