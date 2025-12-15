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
        private readonly CN_CuotaCredito _servicioCuotas = new CN_CuotaCredito();
        private int? _idClienteSeleccionado = null;
        private Cliente _clienteSeleccionado = null;
        public md_RegistrarVentaCredito()
        {
            InitializeComponent();
        }

        private void md_RegistrarVentaCredito_Load(object sender, EventArgs e)
        {
            txtnofactura.Select();

            if (cboFrecuencia.Items.Count == 0)
            {
                cboFrecuencia.Items.Add("Semanal");
                cboFrecuencia.Items.Add("Quincenal");
                cboFrecuencia.Items.Add("Mensual");
            }

            if (cboFrecuencia.Items.Count > 0 && cboFrecuencia.SelectedIndex < 0)
                cboFrecuencia.SelectedIndex = 0;

            dtpFechaInicio.Value = DateTime.Today;
            ActualizarFechaVencimiento();

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                    btn.Click += Btn_Click;
            }

          
            _idClienteSeleccionado = null;
            _clienteSeleccionado = null;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private void RecalcularSaldoYPagoCuota()
        {
            decimal montoTotal = 0m;
            decimal prima = 0m;
            int numeroCuotas = 0;

            decimal.TryParse(txtmontototal.Text, out montoTotal);
            decimal.TryParse(txtPrima.Text, out prima);
            int.TryParse(txtNoCuotas.Text, out numeroCuotas);

            if (montoTotal < 0) montoTotal = 0;
            if (prima < 0) prima = 0;

            decimal saldoPendiente = montoTotal - prima;
            if (saldoPendiente < 0) saldoPendiente = 0;

            txtsaldopendiente.Text = saldoPendiente > 0
                ? saldoPendiente.ToString("0.00")
                : string.Empty;

            if (numeroCuotas > 0 && saldoPendiente > 0)
            {
                decimal cuota = Math.Round(saldoPendiente / numeroCuotas, 2);
                txtCuota.Text = cuota.ToString("0.00");
            }
            else
            {
                txtCuota.Text = string.Empty;
            }

            ActualizarFechaVencimiento();
        }

        private void ActualizarFechaVencimiento()
        {
            if (!int.TryParse(txtNoCuotas.Text, out int numeroCuotas) || numeroCuotas <= 0)
            {
                dtpfechavencimiento.Value = dtpFechaInicio.Value.Date;
                return;
            }

            string frecuencia = cboFrecuencia.SelectedItem?.ToString() ?? "Mensual";
            DateTime fechaInicio = dtpFechaInicio.Value.Date;

            DateTime fechaUltimoPago = _servicioCuotas.CalcularFechaUltimoPago(
                fechaInicio,
                frecuencia,
                numeroCuotas
            );

            dtpfechavencimiento.Value = fechaUltimoPago;
        }


        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new mdCliente())
            {
                var result = modal.ShowDialog();
                if (result == DialogResult.OK && modal._Cliente != null)
                {
                    // guardamos selección
                    _clienteSeleccionado = modal._Cliente;
                    _idClienteSeleccionado = modal._Cliente.idCliente;

                    // rellenar campos del formulario
                    txtcliente.Text = modal._Cliente.NombreCompleto;
                    txtrtncliente.Text = modal._Cliente.Documento;

                    // si en la entidad tienes estos datos, los llenamos
                    txtCodCliente.Text = modal._Cliente.CodCliente > 0
                                            ? modal._Cliente.CodCliente.ToString()
                                            : string.Empty;
                    txtProfesion.Text = modal._Cliente.Correo;
                    txtTelefono.Text = modal._Cliente.Telefono;
                    txtDireccion.Text = modal._Cliente.Direccion;
                }
            }
        }

        private bool AsegurarClienteRegistrado(
             out string mensaje,
             out string documentoCliente,
             out string nombreCliente)
        {
            mensaje = string.Empty;
            documentoCliente = string.Empty;
            nombreCliente = string.Empty;

            // validación mínima de datos de cliente
            if (string.IsNullOrWhiteSpace(txtcliente.Text))
            {
                mensaje = "Ingrese el nombre del cliente.";
                return false;
            }

            nombreCliente = txtcliente.Text.Trim();
            documentoCliente = string.IsNullOrWhiteSpace(txtrtncliente.Text)
                                ? "N/A"
                                : txtrtncliente.Text.Trim();

            // Si ya se eligió del modal → no registrar de nuevo
            if (_idClienteSeleccionado.HasValue && _idClienteSeleccionado.Value > 0)
            {
                return true;
            }

            // === Registrar NUEVO CLIENTE ===

            // CodCliente puede venir vacío → se manda 0 para que el SP genere MAX+1
            int codCliente = 0;
            if (!string.IsNullOrWhiteSpace(txtCodCliente.Text))
                int.TryParse(txtCodCliente.Text.Trim(), out codCliente);

            Cliente nuevo = new Cliente()
            {
                idCliente = 0,
                CodCliente = codCliente,          // 0 => que el SP genere el correlativo
                Documento = documentoCliente,
                NombreCompleto = nombreCliente,
                Correo = txtProfesion.Text.Trim(),   // Profesión / Oficio
                Telefono = txtTelefono.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Estado = true
            };

            string msgCliente;
            int idGenerado = new CN_Cliente().Registrar(nuevo, out msgCliente);

            if (idGenerado == 0)
            {
                mensaje = "Error al registrar el cliente: " + msgCliente;
                return false;
            }

            // actualizamos info local con lo que quedó en BD
            _idClienteSeleccionado = idGenerado;
            _clienteSeleccionado = nuevo;

            if (nuevo.CodCliente > 0)
                txtCodCliente.Text = nuevo.CodCliente.ToString();

            // por si en el SP se ajustara algo
            documentoCliente = nuevo.Documento;
            nombreCliente = nuevo.NombreCompleto;

            return true;
        }

        private void btnregistrarcredito_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtnofactura.Text))
                    txtnofactura.Text = "N/A";

                // 1) Asegurarnos que el cliente está en tabla CLIENTE
                string mensajeCliente;
                string documentoCliente;
                string nombreCliente;

                if (!AsegurarClienteRegistrado(
                        out mensajeCliente,
                        out documentoCliente,
                        out nombreCliente))
                {
                    MessageBox.Show(mensajeCliente,
                                    "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2) Validaciones de datos del crédito
                if (string.IsNullOrWhiteSpace(txtmontototal.Text) ||
                    !decimal.TryParse(txtmontototal.Text, out decimal montoTotal))
                {
                    MessageBox.Show("Ingrese un monto total válido.",
                                    "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtmontototal.Focus();
                    return;
                }

                decimal prima = 0m;
                if (!string.IsNullOrWhiteSpace(txtPrima.Text) &&
                    !decimal.TryParse(txtPrima.Text, out prima))
                {
                    MessageBox.Show("La prima no es válida.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPrima.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNoCuotas.Text) ||
                    !int.TryParse(txtNoCuotas.Text, out int numeroCuotas) ||
                    numeroCuotas <= 0)
                {
                    MessageBox.Show("Ingrese un número de cuotas válido.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNoCuotas.Focus();
                    return;
                }

                decimal saldoBaseCredito = montoTotal - prima;
                if (saldoBaseCredito < 0)
                {
                    MessageBox.Show("La prima no puede ser mayor que el monto total.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtsaldopendiente.Text = saldoBaseCredito.ToString("0.00");

                decimal cuota = saldoBaseCredito > 0
                    ? Math.Round(saldoBaseCredito / numeroCuotas, 2)
                    : 0m;

                txtCuota.Text = cuota > 0 ? cuota.ToString("0.00") : string.Empty;

                string frecuencia = cboFrecuencia.SelectedItem?.ToString() ?? "Mensual";
                DateTime fechaInicio = dtpFechaInicio.Value.Date;

                DateTime fechaVencimiento = _servicioCuotas.CalcularFechaUltimoPago(
                    fechaInicio,
                    frecuencia,
                    numeroCuotas
                );

                dtpfechavencimiento.Value = fechaVencimiento;

                string descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text)
                    ? "N/A"
                    : txtDescripcion.Text.Trim();

                // 3) Registrar VENTA a Crédito
                CN_Venta cnVenta = new CN_Venta();
                string mensaje;

                Venta nuevaVenta = new Venta
                {
                    NumeroDocumento = txtnofactura.Text,
                    NombreCliente = nombreCliente,
                    DocumentoCliente = documentoCliente,
                    MontoTotal = montoTotal,
                    SaldoPendiente = saldoBaseCredito,
                    FechaVencimiento = fechaVencimiento,
                    MetodoPago = "N/A",
                    Condicion = "Credito",
                    FechaRegistro = DateTime.Now,
                    TipoDocumento = "Factura",
                    Descripcion = descripcion
                };

                bool ok = cnVenta.RegistrarCredito(nuevaVenta, out mensaje);
                if (!ok)
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idVenta = cnVenta.ObtenerIdVentaPorNumeroDocumento(nuevaVenta.NumeroDocumento);
                if (idVenta <= 0)
                {
                    MessageBox.Show("Crédito registrado pero no se pudo obtener el IdVenta.",
                                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                _servicioCuotas.GenerarPlanCuotas(
                    idVenta,
                    saldoBaseCredito,
                    numeroCuotas,
                    frecuencia,
                    fechaInicio
                );

                MessageBox.Show("Crédito y plan de cuotas registrados correctamente.",
                                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                                "Excepción", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtmontototal_TextChanged(object sender, EventArgs e)
        {
            RecalcularSaldoYPagoCuota();
        }

        private void txtPrima_TextChanged(object sender, EventArgs e)
        {
            RecalcularSaldoYPagoCuota();
        }

        private void txtNoCuotas_TextChanged(object sender, EventArgs e)
        {
            RecalcularSaldoYPagoCuota();
        }

        private void txtPrima_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;

            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void txtNoCuotas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void cboFrecuencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarFechaVencimiento();

        }

        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {
            ActualizarFechaVencimiento();

        }
    }
}
