using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;


using System.Globalization;

namespace CapaPresentacion.Modales
{
    public partial class FrmCredito : Form
    {
        private int idVenta;
        private bool editandoAbono = false;
        private int idAbonoEditando = 0;
        private decimal montoOriginalEditando = 0m;
        private readonly CultureInfo hn = new CultureInfo("es-HN");
        private decimal saldoBaseCredito = 0m;
        private decimal saldoAnteriorEditando = 0m;
        private decimal montoTotalFactura = 0m;
        private decimal saldoPendienteActual = 0m;
        private decimal primaCredito = 0m;
        private string descripcionCredito = "";


        private string FormatearLempiras(decimal valor)
        {
            // Resultado: L.1,000.00
            return valor.ToString("'L.'#,0.00", hn);
        }

        private decimal ParseDecimal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0m;

            texto = texto.Trim();

            // Quitar prefijo L. o L si viene formateado como "L.1,000.00" o "L 1,000.00"
            if (texto.StartsWith("L."))
                texto = texto.Substring(2).Trim();
            else if (texto.StartsWith("L"))
                texto = texto.Substring(1).Trim();

            decimal valor;

            // Ahora ya solo debería quedar algo como "1,000.00"
            if (decimal.TryParse(texto, NumberStyles.Any, hn, out valor))
                return valor;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                return valor;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.CurrentCulture, out valor))
                return valor;

            return 0m;
        }

        private decimal RecalcularSaldoPendienteDesdeAbonos()
        {
            // Obtenemos TODOS los abonos de esa venta
            var abonos = new CN_Abono().ObtenerHistorialAbonos(idVenta);

            // Si no hay abonos, el saldo correcto es el saldo base del crédito
            if (abonos == null || abonos.Count == 0)
                return saldoBaseCredito;

            decimal totalAbonos = 0m;
            decimal totalIntereses = 0m;

            foreach (var a in abonos)
            {
                totalAbonos += a.MontoAbono;
                totalIntereses += a.InteresAplicado;
            }

            // Fórmula: saldo = saldoBase - sum(Abonos) + sum(Intereses)
            return saldoBaseCredito - totalAbonos + totalIntereses;
        }





        public FrmCredito()
        {
            InitializeComponent();
        }

        private void FrmCredito_Load(object sender, EventArgs e)
        {
            ConfigurarHistorialAbonos();
            CargarHistorialAbonos();

            txtMontoAbono.Select();
            txtfechafin.Value = DateTime.Now;

            if (!cmbMetodoPago.Items.Contains("Efectivo"))
                cmbMetodoPago.Items.Insert(0, "Efectivo");

            if (cmbMetodoPago.SelectedIndex < 0)
                cmbMetodoPago.SelectedIndex = 0;

            if (cmbAplicarInteres.Items.Count == 0)
            {
                cmbAplicarInteres.Items.Add("Sí");
                cmbAplicarInteres.Items.Add("No");
            }
            if (cmbAplicarInteres.SelectedIndex < 0)
                cmbAplicarInteres.SelectedIndex = 1; // No

            if (cmbInteresAplicadoA.Items.Count == 0)
            {
                cmbInteresAplicadoA.Items.Add("Saldo Pendiente");
                cmbInteresAplicadoA.Items.Add("Monto Total");
            }
            if (cmbInteresAplicadoA.SelectedIndex < 0)
                cmbInteresAplicadoA.SelectedIndex = 0;

            cmbAplicarInteres.SelectedIndexChanged -= cmbAplicarInteres_SelectedIndexChanged;
            cmbAplicarInteres.SelectedIndexChanged += cmbAplicarInteres_SelectedIndexChanged;
            cmbAplicarInteres_SelectedIndexChanged(null, null);

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
                if (ctrl is Button btn)
                    btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private void ConfigurarGridCuotas()
        {
            dgvHistorial.AutoGenerateColumns = false;
            dgvHistorial.Columns.Clear();

            // Cuota
            var colCuota = new DataGridViewTextBoxColumn
            {
                Name = "NumeroCuota",
                HeaderText = "Cuota",
                Width = 60,
                ReadOnly = true
            };
            dgvHistorial.Columns.Add(colCuota);

            // Fecha pago
            var colFecha = new DataGridViewTextBoxColumn
            {
                Name = "FechaPago",
                HeaderText = "Fecha pago",
                Width = 90,
                ReadOnly = true
            };
            dgvHistorial.Columns.Add(colFecha);

            // Monto cuota
            var colMontoCuota = new DataGridViewTextBoxColumn
            {
                Name = "MontoCuota",
                HeaderText = "Monto cuota",
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            };
            dgvHistorial.Columns.Add(colMontoCuota);

            // Pagado
            var colPagado = new DataGridViewTextBoxColumn
            {
                Name = "MontoPagado",
                HeaderText = "Pagado",
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            };
            dgvHistorial.Columns.Add(colPagado);

            // Saldo cuota
            var colSaldo = new DataGridViewTextBoxColumn
            {
                Name = "SaldoCuota",
                HeaderText = "Saldo cuota",
                Width = 90,
                ReadOnly = true,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            };
            dgvHistorial.Columns.Add(colSaldo);

            // Estado
            var colEstado = new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                Width = 80,
                ReadOnly = true
            };
            dgvHistorial.Columns.Add(colEstado);
        }


        public void SetFacturaData(string numeroDocumento,
                            string nombreCliente,
                            decimal montoTotal,
                            DateTime fechaVencimiento,
                            decimal saldoPendiente,
                            string estado,
                            string descripcion)
        {
            TxtNumeroDocumento.Text = numeroDocumento;
            TxtNombreCliente.Text = nombreCliente;
            TxtMontoTotal.Text = FormatearLempiras(montoTotal);
            TxtFechaVencimiento.Text = fechaVencimiento.ToString("dd/MM/yyyy");
            TxtSaldoPendiente.Text = FormatearLempiras(saldoPendiente);
            TxtEstado.Text = estado;

            // Descripción igual a la del grid de créditos
            descripcionCredito = descripcion;
            txtDescripcion.Text = descripcionCredito;

            // Prima se calculará luego
            primaCredito = 0m;

            montoTotalFactura = montoTotal;
            saldoPendienteActual = saldoPendiente;
        }




        public void SetIdVenta(int id)
        {
            idVenta = id;

            CargarHistorialAbonos();
            CargarResumenPlan();
        }


        private void CargarResumenPlan()
        {
            if (idVenta <= 0) return;

            var cnCuotas = new CN_CuotaCredito();
            var cnAbono = new CN_Abono();

            var cuotas = cnCuotas.ObtenerCuotasPorVenta(idVenta)
                                 .OrderBy(c => c.NumeroCuota)
                                 .ToList();

            var resumen = cnCuotas.ObtenerResumen(idVenta);

            decimal totalCuotas = cuotas.Count > 0
                ? cuotas.Sum(c => c.MontoCuota)
                : 0m;

            saldoBaseCredito = totalCuotas;

            decimal primaCalculada = montoTotalFactura - totalCuotas;
            if (primaCalculada < 0) primaCalculada = 0m;
            txtPrima.Text = FormatearLempiras(primaCalculada);

            var historialAbonos = cnAbono.ObtenerHistorialAbonos(idVenta) ?? new List<Abono>();
            decimal sumaAbonos = historialAbonos.Sum(a => a.MontoAbono);
            decimal sumaInteres = historialAbonos.Sum(a => a.InteresAplicado);

            saldoPendienteActual = saldoBaseCredito - sumaAbonos + sumaInteres;
            if (saldoPendienteActual < 0) saldoPendienteActual = 0m;

            TxtSaldoPendiente.Text = FormatearLempiras(saldoPendienteActual);

            if (resumen != null)
            {
                txtNoCuotas.Text = resumen.NumeroCuotas.ToString();
                txtCuotaCurso.Text = resumen.CuotaActual.ToString();
                txtMontoCuota.Text = FormatearLempiras(resumen.MontoCuota);
            }
            else
            {
                txtNoCuotas.Text = "0";
                txtCuotaCurso.Text = "0";
                txtMontoCuota.Text = FormatearLempiras(0m);
            }
        }



        private void ConfigurarHistorialAbonos()
        {
            // El grid de abajo es SOLO historial de abonos
            dgvHistorial.AutoGenerateColumns = false;
            dgvHistorial.Columns.Clear();

            // Columna oculta: Id del abono
            var colId = new DataGridViewTextBoxColumn();
            colId.Name = "IdAbono";
            colId.HeaderText = "IdAbono";
            colId.Visible = false;
            dgvHistorial.Columns.Add(colId); // índice 0

            // 1) Fecha del abono
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaAbono",
                HeaderText = "Fecha abono",
                ReadOnly = true,
                Width = 90
            });

            // 2) Monto abonado
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MontoAbono",
                HeaderText = "Monto abonado",
                ReadOnly = true,
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // 3) Saldo anterior
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SaldoAnterior",
                HeaderText = "Saldo anterior",
                ReadOnly = true,
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // 4) Saldo pendiente después del abono
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NuevoSaldoPendiente",
                HeaderText = "Saldo pendiente",
                ReadOnly = true,
                Width = 110,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // 5) Interés aplicado
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InteresAplicado",
                HeaderText = "Interés aplicado",
                ReadOnly = true,
                Width = 100,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // 6) Método de pago
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MetodoPago",
                HeaderText = "Método de pago",
                ReadOnly = true,
                Width = 110
            });

            // 7) No. de recibo
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NoRecibo",
                HeaderText = "No. Recibo",
                ReadOnly = true,
                Width = 90
            });

            // 8) Cuotas a las que se aplicó el abono (texto: "Cuota 1-3 parcial", etc.)
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CuotasDescripcion",
                HeaderText = "Cuotas",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }


        private void txtMontoAbono_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal montoAbono;
                if (decimal.TryParse(txtMontoAbono.Text, out montoAbono))
                {
                    decimal saldoPendiente = ParseDecimal(TxtSaldoPendiente.Text);
                    decimal nuevoSaldoPendiente = saldoPendiente - montoAbono;
                    decimal interesAplicado = 0;
                    DateTime fechaVencimiento;

                    try
                    {
                        fechaVencimiento = DateTime.ParseExact(TxtFechaVencimiento.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        if (DateTime.Now > fechaVencimiento)
                        {
                            interesAplicado = saldoPendiente * 0.03m; 
                        }
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("La fecha de vencimiento no es válida.");
                        return; 
                    }

                    txtNuevoSaldoPendiente.Text = FormatearLempiras(nuevoSaldoPendiente);
                    txtInteresAplicado.Text = FormatearLempiras(interesAplicado);
                    txtTotalAPagar.Text = FormatearLempiras(nuevoSaldoPendiente + interesAplicado);

                }
                else
                {
                    MessageBox.Show("Ingrese un monto válido para el abono.");
                    
                }
            }
        }

        private void CargarHistorialAbonos()
        {
            if (idVenta <= 0) return;

            try
            {
                // Asegurar columnas historial
                if (dgvHistorial.Columns.Count == 0 || !dgvHistorial.Columns.Contains("FechaAbono"))
                {
                    ConfigurarHistorialAbonos();
                }

                dgvHistorial.Rows.Clear();

                var cnAbono = new CN_Abono();
                var historialAbonos = cnAbono.ObtenerHistorialAbonos(idVenta);
                if (historialAbonos == null || historialAbonos.Count == 0) return;

                // Ordenar por fecha / Id
                historialAbonos = historialAbonos
                    .OrderBy(a => a.FechaAbono)
                    .ThenBy(a => a.IdAbono)
                    .ToList();

                // Saldo que se muestra en pantalla (solo para formatear de nuevo)
                decimal saldoPantalla = ParseDecimal(TxtSaldoPendiente.Text);
                TxtSaldoPendiente.Text = FormatearLempiras(saldoPantalla);

                // Para armar el texto de cuotas por cada abono vamos a leer AbonoCuota
                var cnAbonoCuota = new CN_AbonoCuota();

                foreach (var abono in historialAbonos)
                {
                    // ---------- Texto de cuotas desde tabla AbonoCuota ----------
                    string cuotasTexto = string.Empty;
                    var dtDetalle = cnAbonoCuota.ObtenerDetallePorAbono(abono.IdAbono);

                    if (dtDetalle != null && dtDetalle.Rows.Count > 0)
                    {
                        var partes = new List<string>();

                        foreach (DataRow dr in dtDetalle.Rows)
                        {
                            int numeroCuota = Convert.ToInt32(dr["NumeroCuota"]);
                            decimal montoCuota = Convert.ToDecimal(dr["MontoCuota"]);
                            decimal montoAplicado = Convert.ToDecimal(dr["MontoAplicado"]);

                            bool completa = montoAplicado >= montoCuota;
                            string desc = completa
                                ? $"Cuota {numeroCuota} completa"
                                : $"Cuota {numeroCuota} parcial";

                            partes.Add(desc);
                        }

                        cuotasTexto = string.Join(", ", partes);
                    }

                    // ---------- Agregar fila al grid ----------
                    dgvHistorial.Rows.Add(
                        abono.IdAbono,                                   // IdAbono (oculto)
                        abono.FechaAbono.ToString("dd/MM/yyyy"),        // Fecha abono
                        FormatearLempiras(abono.MontoAbono),            // Monto abonado
                        FormatearLempiras(abono.SaldoAnterior),         // Saldo anterior
                        FormatearLempiras(abono.NuevoSaldoPendiente),   // Saldo pendiente
                        FormatearLempiras(abono.InteresAplicado),       // Interés aplicado
                        abono.MetodoPago ?? "No especificado",          // Método de pago
                        string.IsNullOrWhiteSpace(abono.NoRecibo) ? "N/A" : abono.NoRecibo,
                        cuotasTexto                                      // 👉 CuotasDescripcion
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial de abonos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LimpiarCampos()
        {
            txtMontoAbono.Text = "";
            txtNuevoSaldoPendiente.Text = "";
            txtInteresAplicado.Text = "0.00"; 
            txtTotalAPagar.Text = "";
            cmbMetodoPago.SelectedIndex = 0; 
            txtMontoAbono.Select(); 
            txtPorcentajeInteres.Text = "";
            txtrecibo.Text = "";
        }

        private void txtMontoAbono_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal montoAbono;
                if (decimal.TryParse(txtMontoAbono.Text, out montoAbono))
                {
                    // Saldo que está actualmente en la caja de texto
                    decimal saldoPendiente = ParseDecimal(TxtSaldoPendiente.Text);

                    decimal nuevoSaldoPendiente;

                    // ✅ Si estamos EDITANDO un abono:
                    // saldo nuevo = saldo actual + abono original - abono nuevo
                    if (editandoAbono)
                    {
                        nuevoSaldoPendiente = saldoPendiente + montoOriginalEditando - montoAbono;
                    }
                    else
                    {
                        // ✅ Si estamos creando un abono nuevo:
                        // saldo nuevo = saldo actual - abono nuevo
                        nuevoSaldoPendiente = saldoPendiente - montoAbono;
                    }

                    decimal interesAplicado = 0;

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
                                decimal montoTotal = ParseDecimal(TxtMontoTotal.Text);
                                interesAplicado = (montoTotal * porcentajeInteres) / 100;
                            }
                        }
                    }

                    txtNuevoSaldoPendiente.Text = FormatearLempiras(nuevoSaldoPendiente);
                    txtInteresAplicado.Text = FormatearLempiras(interesAplicado);
                    txtTotalAPagar.Text = FormatearLempiras(nuevoSaldoPendiente + interesAplicado);
                }
                else
                {
                    MessageBox.Show("Ingrese un monto válido para el abono.");
                }
            }
        }

        private void btnregistrarabono_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMontoAbono.Text))
            {
                MessageBox.Show("Por favor, ingrese un monto de abono.", "Campo Obligatorio",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoAbono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtSaldoPendiente.Text))
            {
                MessageBox.Show("El saldo pendiente no puede estar vacío.", "Campo Obligatorio",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoAbono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTotalAPagar.Text))
            {
                MessageBox.Show("Debe presionar Enter después de poner el monto del abono.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontoAbono.Focus();
                return;
            }

            if (cmbMetodoPago.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un método de pago.", "Campo Obligatorio",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbMetodoPago.Focus();
                return;
            }

            if (TxtEstado.Text == "Liquidada")
            {
                MessageBox.Show("No se puede registrar un abono en una factura liquidada.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtMontoAbono.Text, out decimal montoAbono))
            {
                MessageBox.Show("Ingrese un monto válido para el abono.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtrecibo.Text))
                txtrecibo.Text = "N/A";

            bool esEdicion = editandoAbono && idAbonoEditando > 0;

            decimal saldoBaseParaCalculo = esEdicion
                ? saldoAnteriorEditando
                : ParseDecimal(TxtSaldoPendiente.Text);

            if (montoAbono > saldoBaseParaCalculo)
            {
                MessageBox.Show("El monto del abono no puede ser mayor que el saldo base.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal interes = 0m;
            if (cmbAplicarInteres.SelectedItem.ToString() == "Sí")
            {
                if (!decimal.TryParse(txtPorcentajeInteres.Text, out decimal porcentajeInteres))
                {
                    MessageBox.Show("Ingrese un porcentaje de interés válido.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (cmbInteresAplicadoA.SelectedItem.ToString() == "Saldo Pendiente")
                    interes = (saldoBaseParaCalculo * porcentajeInteres) / 100m;
                else if (cmbInteresAplicadoA.SelectedItem.ToString() == "Monto Total")
                {
                    decimal montoTotalCredito = ParseDecimal(TxtMontoTotal.Text);
                    interes = (montoTotalCredito * porcentajeInteres) / 100m;
                }
            }

            decimal nuevoSaldoPendiente = saldoBaseParaCalculo - montoAbono + interes;

            var cnAbono = new CN_Abono();
            string mensaje;

            if (!esEdicion)
            {
                // NUEVO ABONO
                Abono nuevoAbono = new Abono
                {
                    IdVenta = idVenta,
                    MontoAbono = montoAbono,
                    FechaAbono = txtfechafin.Value.Date,
                    NuevoSaldoPendiente = nuevoSaldoPendiente,
                    InteresAplicado = interes,
                    MetodoPago = cmbMetodoPago.SelectedItem.ToString(),
                    NoRecibo = txtrecibo.Text,
                    SaldoAnterior = saldoBaseParaCalculo
                };

                int idAbonoGenerado = 0;
                bool ok = cnAbono.Registrar(nuevoAbono, out mensaje, out idAbonoGenerado);

                if (!ok)
                {
                    MessageBox.Show("Error al registrar el abono: " + mensaje, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                RepartirAbonoEnCuotas(idAbonoGenerado, montoAbono);

                // 🔴 AQUÍ SE REORGANIZA TODO EL PLAN COMPLETO
                RecalcularCuotasGlobal();

                MessageBox.Show("Abono registrado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // EDICIÓN DE ABONO
                var cnAbonoCuota = new CN_AbonoCuota();
                var cnCuotas = new CN_CuotaCredito();

                // 1) Revertir lo que este abono aplicó antes
                var dtDetalle = cnAbonoCuota.ObtenerDetallePorAbono(idAbonoEditando);
                if (dtDetalle.Rows.Count > 0)
                {
                    var listaCuotas = cnCuotas.ObtenerCuotasPorVenta(idVenta);

                    foreach (DataRow dr in dtDetalle.Rows)
                    {
                        int idCuota = Convert.ToInt32(dr["IdCuota"]);
                        decimal montoAplic = Convert.ToDecimal(dr["MontoAplicado"]);
                        decimal montoCuota = Convert.ToDecimal(dr["MontoCuota"]);

                        var cuota = listaCuotas.FirstOrDefault(x => x.IdCuota == idCuota);
                        if (cuota == null) continue;

                        cuota.MontoPagado -= montoAplic;
                        if (cuota.MontoPagado < 0) cuota.MontoPagado = 0m;

                        if (cuota.MontoPagado <= 0)
                            cuota.Estado = "Pendiente";
                        else if (cuota.MontoPagado >= montoCuota)
                            cuota.Estado = "Pagada";
                        else
                            cuota.Estado = "Parcial";

                        cnCuotas.ActualizarCuota(cuota);
                    }

                    cnAbonoCuota.EliminarPorAbono(idAbonoEditando);
                }

                // 2) Actualizar el registro del abono
                Abono abonoEditado = new Abono
                {
                    IdAbono = idAbonoEditando,
                    IdVenta = idVenta,
                    MontoAbono = montoAbono,
                    FechaAbono = txtfechafin.Value.Date,
                    NuevoSaldoPendiente = nuevoSaldoPendiente,
                    InteresAplicado = interes,
                    MetodoPago = cmbMetodoPago.SelectedItem.ToString(),
                    NoRecibo = txtrecibo.Text,
                    SaldoAnterior = saldoBaseParaCalculo
                };

                bool okActualizar = cnAbono.Actualizar(abonoEditado, out mensaje);
                if (!okActualizar)
                {
                    MessageBox.Show("Error al actualizar el abono: " + mensaje, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3) Volver a repartir este abono
                RepartirAbonoEnCuotas(idAbonoEditando, montoAbono);

                // 🔴 AQUÍ TAMBIÉN SE REORGANIZA TODO EL PLAN COMPLETO
                RecalcularCuotasGlobal();

                MessageBox.Show("Abono actualizado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                editandoAbono = false;
                idAbonoEditando = 0;
                btnregistrarabono.Text = "Registrar abono";
                btnEliminarAbono.Visible = false;
            }

            // Refrescar pantallas
            CargarHistorialAbonos();
            CargarResumenPlan();
            LimpiarCampos();

            if (this.Owner is FrmVentasCredito form)
                form.RefrescarFormulario();

            txtMontoAbono.Select();
        }

        private void PrintReciboAbono(
     decimal montoAbono,
     decimal saldoAnterior,          // 👈 este es el saldo pendiente ANTES del abono
     decimal nuevoSaldoPendiente,
     string metodoPago,
     string numeroFactura,
     decimal montoTotal,
     DateTime fechaVencimiento,
     string estado,
     decimal interesAplicado,
     string tipoRecibo,
     decimal totalAPagar,
     DateTime fechaAbono)
        {
            string estadoImpresion = nuevoSaldoPendiente <= 0 ? "Liquidada" : estado;

            using (PrintDocument printDocument = new PrintDocument()) // ← deja solo esta
            {
                string nombreImpresoraObjetivo = "POS-80".Normalize().Replace('\u00A0', ' ').Trim();

                printDocument.PrinterSettings = new PrinterSettings
                {
                    PrinterName = nombreImpresoraObjetivo
                };

                if (!printDocument.PrinterSettings.IsValid)
                {
                    foreach (string p in PrinterSettings.InstalledPrinters)
                    {
                        string limpio = p.Replace('\u00A0', ' ');
                        if (limpio.IndexOf(@"\\192.168.0.9\POS-80 (copy 1)", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            printDocument.PrinterSettings.PrinterName = p;
                            break;
                        }
                    }
                }

                if (!printDocument.PrinterSettings.IsValid)
                {
                    MessageBox.Show("No se encontró la impresora POS-80. Selecciónala manualmente.");
                    using (var dlg = new PrintDialog { Document = printDocument, UseEXDialog = true })
                    {
                        if (dlg.ShowDialog() != DialogResult.OK) return;
                    }
                }

                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Receipt80", 315, 0);
                printDocument.PrintPage += (sender, e) =>
            {
                int pageWidth = 280;
                Font fontNormal = new Font("MS Gothic", 8, FontStyle.Regular);
                Font fontBold = new Font("MS Gothic", 8, FontStyle.Bold);
                Font fontTitle = new Font("MS Gothic", 12, FontStyle.Bold);
                int y = 0;

                e.Graphics.DrawString("RECIBO DE ABONO", fontTitle, Brushes.Black,
                    (pageWidth - e.Graphics.MeasureString("RECIBO DE ABONO", fontTitle).Width) / 2, y);
                y += 18;

                e.Graphics.DrawString("AMERICAN COLORS", fontBold, Brushes.Black,
                    (pageWidth - e.Graphics.MeasureString("AMERICAN COLORS", fontBold).Width) / 2, y);
                y += 14;

                e.Graphics.DrawString("Teléfonos: (+504) 3312-9413 / (+504) 3376-6564", fontNormal, Brushes.Black,
                    (pageWidth - e.Graphics.MeasureString("Teléfonos: (+504) 3312-9413 / (+504) 3376-6564", fontNormal).Width) / 2, y);
                y += 20;

                e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
                y += 13;

                // Cliente
                string nombreCliente = TxtNombreCliente.Text;
                string clienteLabel = "Cliente: ";
                int maxLength = 45;
                if (clienteLabel.Length + nombreCliente.Length > maxLength)
                {
                    string primera = nombreCliente.Substring(0, Math.Min(maxLength - clienteLabel.Length, nombreCliente.Length));
                    string segunda = nombreCliente.Length > (maxLength - clienteLabel.Length)
                        ? nombreCliente.Substring(maxLength - clienteLabel.Length)
                        : "";
                    e.Graphics.DrawString(clienteLabel + primera, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                    if (!string.IsNullOrEmpty(segunda))
                    {
                        e.Graphics.DrawString(segunda, fontNormal, Brushes.Black, 10, y);
                        y += 13;
                    }
                }
                else
                {
                    e.Graphics.DrawString(clienteLabel + nombreCliente, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }

                e.Graphics.DrawString("Fecha de Abono: " + fechaAbono.ToString("dd/MM/yyyy"),
    fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Método de pago: " + metodoPago, fontNormal, Brushes.Black, 10, y);
                y += 13;

                // 👉 Aquí imprimimos el saldo anterior
                e.Graphics.DrawString("Saldo anterior: L. " + saldoAnterior.ToString("0.00"),
                    fontNormal, Brushes.Black, 10, y);
                y += 13;

                e.Graphics.DrawString("Monto abonado: L. " + montoAbono.ToString("0.00"), fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Interés aplicado: L. " + interesAplicado.ToString("0.00"), fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Nuevo saldo pendiente: L. " + nuevoSaldoPendiente.ToString("0.00"), fontBold, Brushes.Black, 10, y);
                y += 16;

                e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
                y += 13;

                // Datos de la factura
                e.Graphics.DrawString("Número de Factura: " + numeroFactura, fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Monto Total: L. " + montoTotal.ToString("0.00"), fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Fecha de Vencimiento: " + fechaVencimiento.ToString("dd/MM/yyyy"), fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Estado: " + estadoImpresion, fontNormal, Brushes.Black, 10, y);
                y += 13;

                e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
                y += 70;

                e.Graphics.DrawString("Firma del Cliente: _________________________", fontNormal, Brushes.Black, 10, y);
                y += 70;
                e.Graphics.DrawString("Firma AMERICAN COLORS: ____________________", fontNormal, Brushes.Black, 10, y);
                y += 50;

                e.Graphics.DrawString("¡GRACIAS POR SU PREFERENCIA!", fontBold, Brushes.Black,
                    (pageWidth - e.Graphics.MeasureString("¡GRACIAS POR SU PREFERENCIA!", fontBold).Width) / 2, y);
                y += 30;

                e.Graphics.DrawString(tipoRecibo, fontBold, Brushes.Black, pageWidth - 70, y);
            };

                printDocument.Print();
            }
        }



        private void BtnCancelar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvHistorial_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgvHistorial.Columns.Contains("IdAbono"))
                return;
            if (e.RowIndex < 0) return;

            var fila = dgvHistorial.Rows[e.RowIndex];

            // Id del abono que se va a editar / eliminar
            idAbonoEditando = Convert.ToInt32(fila.Cells["IdAbono"].Value);
            editandoAbono = true;

            // Fecha del abono (columna 1)
            txtfechafin.Value = DateTime.ParseExact(
                fila.Cells[1].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Monto original del abono (columna 2)
            montoOriginalEditando = ParseDecimal(fila.Cells[2].Value.ToString());

            // Saldo anterior (columna 3)
            saldoAnteriorEditando = ParseDecimal(fila.Cells[3].Value.ToString());

            // Cargamos los controles
            txtMontoAbono.Text = montoOriginalEditando.ToString("0.00");
            txtNuevoSaldoPendiente.Text = fila.Cells[4].Value.ToString();  // Nuevo saldo
            txtInteresAplicado.Text = fila.Cells[5].Value.ToString();      // Interés

            string metodo = fila.Cells[6].Value?.ToString() ?? "Efectivo";
            if (cmbMetodoPago.Items.Contains(metodo))
                cmbMetodoPago.SelectedItem = metodo;

            txtrecibo.Text = fila.Cells[7].Value?.ToString() ?? "N/A";

            // Cambiamos el texto del botón y mostramos Eliminar
            btnregistrarabono.Text = "Actualizar abono";
            btnEliminarAbono.Visible = true;
        }

        private void RepartirAbonoEnCuotas(int idAbonoGenerado, decimal montoAbonoSoloCapital)
        {
            var cnCuotas = new CN_CuotaCredito();
            var cnAbonoCuota = new CN_AbonoCuota();

            var cuotas = cnCuotas.ObtenerCuotasPorVenta(idVenta)
                                 .OrderBy(c => c.NumeroCuota)
                                 .ToList();


            decimal restante = montoAbonoSoloCapital;

            foreach (var c in cuotas)
            {
                if (restante <= 0) break;

                if (c.IdCuota <= 0)
                    continue;

                decimal saldoCuota = c.MontoCuota - c.MontoPagado;
                if (saldoCuota <= 0) continue;

                decimal aAplicar = Math.Min(saldoCuota, restante);
                if (aAplicar <= 0) continue;

                // ===== Detalle Abono–Cuota =====
                cnAbonoCuota.RegistrarDetalle(idAbonoGenerado, c.IdCuota, aAplicar);

                // ===== Actualizar cuota =====
                c.MontoPagado += aAplicar;

                if (c.MontoPagado >= c.MontoCuota)
                    c.Estado = "Pagada";
                else if (c.MontoPagado > 0)
                    c.Estado = "Parcial";
                else
                    c.Estado = "Pendiente";

                cnCuotas.ActualizarCuota(c);

                restante -= aAplicar;
            }
        }


        private void btnEliminarAbono_Click(object sender, EventArgs e)
        {
            if (idAbonoEditando <= 0)
            {
                MessageBox.Show("Seleccione un abono del historial para eliminar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var r = MessageBox.Show("¿Seguro que desea eliminar este abono?",
                                    "Confirmar eliminación",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            try
            {
                var cnAbonoCuota = new CN_AbonoCuota();
                var cnCuotas = new CN_CuotaCredito();

                // 1) Revertir lo que este abono aplicó a las cuotas
                var dtDetalle = cnAbonoCuota.ObtenerDetallePorAbono(idAbonoEditando);
                if (dtDetalle.Rows.Count > 0)
                {
                    var listaCuotas = cnCuotas.ObtenerCuotasPorVenta(idVenta);

                    foreach (DataRow dr in dtDetalle.Rows)
                    {
                        int idCuota = Convert.ToInt32(dr["IdCuota"]);
                        decimal montoAplic = Convert.ToDecimal(dr["MontoAplicado"]);
                        decimal montoCuota = Convert.ToDecimal(dr["MontoCuota"]);

                        var cuota = listaCuotas.FirstOrDefault(x => x.IdCuota == idCuota);
                        if (cuota == null) continue;

                        cuota.MontoPagado -= montoAplic;
                        if (cuota.MontoPagado < 0) cuota.MontoPagado = 0m;

                        if (cuota.MontoPagado <= 0)
                            cuota.Estado = "Pendiente";
                        else if (cuota.MontoPagado >= montoCuota)
                            cuota.Estado = "Pagada";
                        else
                            cuota.Estado = "Parcial";

                        cnCuotas.ActualizarCuota(cuota);
                    }

                    cnAbonoCuota.EliminarPorAbono(idAbonoEditando);
                }

                // 2) Eliminar abono
                string mensaje;
                bool ok = new CN_Abono().Eliminar(idAbonoEditando, out mensaje);

                if (!ok)
                {
                    MessageBox.Show("Error al eliminar el abono: " + mensaje, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 🔴 REORGANIZAR TODO EL PLAN COMPLETO DESPUÉS DE ELIMINAR
                RecalcularCuotasGlobal();

                if (this.Owner is FrmVentasCredito ventasCreditoForm)
                {
                    ventasCreditoForm.RefrescarFormulario();
                }

                MessageBox.Show("Abono eliminado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                editandoAbono = false;
                idAbonoEditando = 0;
                btnregistrarabono.Text = "Registrar abono";
                btnEliminarAbono.Visible = false;

                LimpiarCampos();
                CargarHistorialAbonos();
                CargarResumenPlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el abono: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RecalcularCuotasGlobal()
        {
            var cnCuotas = new CN_CuotaCredito();
            var cnAbono = new CN_Abono();
            var cnAbonoCuota = new CN_AbonoCuota();
            var cnVenta = new CN_Venta();

            // 1) Traer cuotas y abonos actuales
            var cuotas = cnCuotas.ObtenerCuotasPorVenta(idVenta)
                                 .OrderBy(c => c.NumeroCuota)
                                 .ToList();

            var abonos = cnAbono.ObtenerHistorialAbonos(idVenta)
                                ?.OrderBy(a => a.FechaAbono)
                                 .ThenBy(a => a.IdAbono)
                                .ToList()
                         ?? new List<Abono>();

            // 2) Resetear cuotas (sin pagos)
            foreach (var c in cuotas)
            {
                c.MontoPagado = 0m;
                c.Estado = "Pendiente";
                cnCuotas.ActualizarCuota(c);
            }

            // 3) Borrar TODOS los detalles Abono–Cuota
            foreach (var ab in abonos)
            {
                cnAbonoCuota.EliminarPorAbono(ab.IdAbono);
            }

            // 4) Aplicar cada abono en orden a las cuotas
            foreach (var ab in abonos)
            {
                decimal restante = ab.MontoAbono; // capital

                foreach (var c in cuotas)
                {
                    if (restante <= 0) break;

                    decimal saldoCuota = c.MontoCuota - c.MontoPagado;
                    if (saldoCuota <= 0) continue;

                    decimal aAplicar = Math.Min(saldoCuota, restante);

                    cnAbonoCuota.RegistrarDetalle(ab.IdAbono, c.IdCuota, aAplicar);

                    c.MontoPagado += aAplicar;

                    if (c.MontoPagado >= c.MontoCuota)
                        c.Estado = "Pagada";
                    else if (c.MontoPagado > 0)
                        c.Estado = "Parcial";
                    else
                        c.Estado = "Pendiente";

                    cnCuotas.ActualizarCuota(c);

                    restante -= aAplicar;
                }
            }

            // 5) Recalcular saldo pendiente del crédito
            decimal saldoPlan = cuotas.Sum(c => c.MontoCuota);
            decimal totalAbonos = abonos.Sum(a => a.MontoAbono);
            decimal totalInteres = abonos.Sum(a => a.InteresAplicado);

            saldoBaseCredito = saldoPlan;

            decimal saldoRecalculado = saldoPlan - totalAbonos + totalInteres;
            if (saldoRecalculado < 0) saldoRecalculado = 0m;

            saldoPendienteActual = saldoRecalculado;
            TxtSaldoPendiente.Text = FormatearLempiras(saldoRecalculado);

            cnVenta.ActualizarSaldoPendiente(idVenta, saldoRecalculado);

            // 6) Refrescar resumen (cuota en curso, próximo pago, etc.)
            CargarResumenPlan();
        }


        private void btnResumen_Click(object sender, EventArgs e)
        {
            try
            {
                var cnCuotas = new CN_CuotaCredito();

                var resumen = cnCuotas.ObtenerResumen(idVenta);
                var cuotas = cnCuotas.ObtenerCuotasPorVenta(idVenta)
                                     .OrderBy(c => c.NumeroCuota)
                                     .ToList();

                decimal totalCuotas = cuotas.Sum(c => c.MontoCuota);
                decimal totalPagado = cuotas.Sum(c => c.MontoPagado);
                decimal totalSaldo = totalCuotas - totalPagado;

                using (var dlg = new Form())
                {
                    dlg.Text = "Tabla de amortización del crédito";
                    dlg.StartPosition = FormStartPosition.CenterParent;
                    dlg.MinimizeBox = false;
                    dlg.MaximizeBox = false;
                    dlg.Size = new Size(720, 480);

                    var panelTop = new Panel
                    {
                        Dock = DockStyle.Top,
                        Height = 80,
                        Padding = new Padding(10)
                    };

                    var lblInfo = new Label
                    {
                        Dock = DockStyle.Fill,
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Font = new Font("Segoe UI", 9f),
                        Text =
                            "N° cuotas: " + resumen.NumeroCuotas +
                            "    Cuota actual: " + resumen.CuotaActual +
                            "    Próximo pago: " + (resumen.ProximoPago.HasValue
                                                    ? resumen.ProximoPago.Value.ToString("dd/MM/yyyy")
                                                    : "---") +
                            Environment.NewLine +
                            "Total cuotas: " + FormatearLempiras(totalCuotas) +
                            "    Pagado: " + FormatearLempiras(totalPagado) +
                            "    Saldo: " + FormatearLempiras(totalSaldo) +
                            (resumen.EnMora ? "    *** EN MORA ***" : "")
                    };

                    panelTop.Controls.Add(lblInfo);

                    var grid = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        ReadOnly = true,
                        AllowUserToAddRows = false,
                        AllowUserToDeleteRows = false,
                        AutoGenerateColumns = false
                    };

                    grid.Columns.Add("NumeroCuota", "Cuota");
                    grid.Columns.Add("FechaPago", "Fecha pago");
                    grid.Columns.Add("MontoCuota", "Monto cuota");
                    grid.Columns.Add("MontoPagado", "Pagado");
                    grid.Columns.Add("SaldoCuota", "Saldo");
                    grid.Columns.Add("Estado", "Estado");

                    foreach (DataGridViewColumn c in grid.Columns)
                    {
                        if (c.Name == "MontoCuota" ||
                            c.Name == "MontoPagado" ||
                            c.Name == "SaldoCuota")
                        {
                            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }

                    foreach (var c in cuotas)
                    {
                        decimal saldoCuota = c.MontoCuota - c.MontoPagado;

                        grid.Rows.Add(
                            c.NumeroCuota,
                            c.FechaPago.ToString("dd/MM/yyyy"),
                            FormatearLempiras(c.MontoCuota),
                            FormatearLempiras(c.MontoPagado),
                            FormatearLempiras(saldoCuota),
                            c.Estado
                        );
                    }

                    var btnCerrar = new Button
                    {
                        Text = "Cerrar",
                        DialogResult = DialogResult.OK,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(90, 32)
                    };

                    dlg.Controls.Add(grid);
                    dlg.Controls.Add(panelTop);
                    dlg.Controls.Add(btnCerrar);

                    dlg.AcceptButton = btnCerrar;

                    dlg.Load += (s, ev) =>
                    {
                        btnCerrar.Location = new System.Drawing.Point(
                            dlg.ClientSize.Width - btnCerrar.Width - 20,
                            dlg.ClientSize.Height - btnCerrar.Height - 15);
                    };

                    dlg.Resize += (s, ev) =>
                    {
                        btnCerrar.Location = new System.Drawing.Point(
                            dlg.ClientSize.Width - btnCerrar.Width - 20,
                            dlg.ClientSize.Height - btnCerrar.Height - 15);
                    };

                    dlg.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener resumen: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}
