using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;



namespace CapaPresentacion
{
    public partial class FrmVentasCredito : Form
    {
        public FrmVentasCredito()
        {
            InitializeComponent();
            
        }
        private System.Windows.Forms.Timer _timerBusquedaCredito;

        private bool _cargandoFacturas = false;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.BeginInvoke(new Action(async () =>
            {
                await CargarFacturasAsync();
            }));
        }
        private async Task CargarFacturasAsync()
        {
            if (_cargandoFacturas)   
                return;

            _cargandoFacturas = true;

            try
            {
                Cursor = Cursors.WaitCursor;

                DataTable dt = await Task.Run(() =>
                {
                    DataTable tabla = new DataTable();
                    tabla.Columns.Add("IdVenta", typeof(int));
                    tabla.Columns.Add("FechaRegistro", typeof(string));
                    tabla.Columns.Add("NumeroDocumento", typeof(string));
                    tabla.Columns.Add("CodCliente", typeof(int));
                    tabla.Columns.Add("NombreCliente", typeof(string));
                    tabla.Columns.Add("MontoTotal", typeof(decimal));
                    tabla.Columns.Add("SaldoPendiente", typeof(decimal));
                    tabla.Columns.Add("FechaVencimiento", typeof(string));
                    tabla.Columns.Add("Estado", typeof(string));
                    tabla.Columns.Add("Vencida", typeof(string));
                    tabla.Columns.Add("Descripcion", typeof(string));
                    tabla.Columns.Add("NumeroCuotas", typeof(int));
                    tabla.Columns.Add("ProximoPago", typeof(string));
                    tabla.Columns.Add("Mora", typeof(string));
                    tabla.Columns.Add("ProximoPagoDate", typeof(DateTime)); // solo ordenar

                    var cnVenta = new CN_Venta();
                    var facturasCredito = cnVenta.ObtenerFacturasCredito(); // UNA SOLA CONSULTA

                    foreach (var f in facturasCredito)
                    {
                        string fechaRegistroFormateada =
                            f.FechaRegistro == DateTime.MinValue
                                ? "N/A"
                                : f.FechaRegistro.ToString("dd/MM/yyyy");

                        string fechaVencimientoFormateada =
                            f.FechaVencimiento.HasValue
                                ? f.FechaVencimiento.Value.ToString("dd/MM/yyyy")
                                : "Sin fecha";

                        string proximoPagoFormateado = "Sin fecha";
                        DateTime proximoPagoDate = DateTime.MaxValue;

                        if (f.ProximoPago.HasValue)
                        {
                            proximoPagoDate = f.ProximoPago.Value.Date;
                            proximoPagoFormateado = proximoPagoDate.ToString("dd/MM/yyyy");
                        }

                        tabla.Rows.Add(
                            f.IdVenta,
                            fechaRegistroFormateada,
                            string.IsNullOrWhiteSpace(f.NumeroDocumento) ? "S/N" : f.NumeroDocumento,
                            f.CodCliente,
                            string.IsNullOrWhiteSpace(f.NombreCliente) ? "N/A" : f.NombreCliente,
                            f.MontoTotal,
                            f.SaldoPendiente,
                            fechaVencimientoFormateada,
                            f.Estado,
                            f.Vencida,
                            string.IsNullOrWhiteSpace(f.Descripcion) ? "N/A" : f.Descripcion,
                            f.NumeroCuotas,
                            proximoPagoFormateado,
                            string.IsNullOrWhiteSpace(f.Mora) ? "No" : f.Mora,
                            proximoPagoDate
                        );
                    }

                    return tabla;
                });

                dgvFacturasCredito.DataSource = dt;

                if (dgvFacturasCredito.Columns.Contains("IdVenta"))
                    dgvFacturasCredito.Columns["IdVenta"].Visible = false;
                if (dgvFacturasCredito.Columns.Contains("ProximoPagoDate"))
                    dgvFacturasCredito.Columns["ProximoPagoDate"].Visible = false;

                dgvFacturasCredito.Columns["FechaRegistro"].HeaderText = "Fecha de Registro";
                dgvFacturasCredito.Columns["NumeroDocumento"].HeaderText = "No.Factura";
                dgvFacturasCredito.Columns["CodCliente"].HeaderText = "Cod Cliente";
                dgvFacturasCredito.Columns["NombreCliente"].HeaderText = "Cliente";
                dgvFacturasCredito.Columns["MontoTotal"].HeaderText = "Monto Total";
                dgvFacturasCredito.Columns["SaldoPendiente"].HeaderText = "Saldo Pendiente";
                dgvFacturasCredito.Columns["FechaVencimiento"].HeaderText = "Fecha de Vencimiento";
                dgvFacturasCredito.Columns["Estado"].HeaderText = "Estado";
                dgvFacturasCredito.Columns["Vencida"].HeaderText = "Vencida";
                dgvFacturasCredito.Columns["Descripcion"].HeaderText = "Descripción";
                dgvFacturasCredito.Columns["NumeroCuotas"].HeaderText = "# Cuotas";
                dgvFacturasCredito.Columns["ProximoPago"].HeaderText = "Próximo pago";
                dgvFacturasCredito.Columns["Mora"].HeaderText = "Mora";

                dgvFacturasCredito.Columns["FechaRegistro"].Width = 90;
                dgvFacturasCredito.Columns["NumeroDocumento"].Width = 90;
                dgvFacturasCredito.Columns["CodCliente"].Width = 70;
                dgvFacturasCredito.Columns["NombreCliente"].Width = 200;
                dgvFacturasCredito.Columns["MontoTotal"].Width = 110;
                dgvFacturasCredito.Columns["SaldoPendiente"].Width = 110;
                dgvFacturasCredito.Columns["FechaVencimiento"].Width = 110;
                dgvFacturasCredito.Columns["Estado"].Width = 80;
                dgvFacturasCredito.Columns["Vencida"].Width = 80;
                dgvFacturasCredito.Columns["Descripcion"].Width = 180;
                dgvFacturasCredito.Columns["NumeroCuotas"].Width = 70;
                dgvFacturasCredito.Columns["ProximoPago"].Width = 110;
                dgvFacturasCredito.Columns["Mora"].Width = 70;

                dgvFacturasCredito.Columns["MontoTotal"].DefaultCellStyle.Format = "'L.'#,0.00";
                dgvFacturasCredito.Columns["SaldoPendiente"].DefaultCellStyle.Format = "'L.'#,0.00";
                dgvFacturasCredito.Columns["MontoTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvFacturasCredito.Columns["SaldoPendiente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvFacturasCredito.Columns["NumeroCuotas"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvFacturasCredito.Columns["CodCliente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
                {
                    if (row.Cells["Vencida"].Value?.ToString() == "Vencida")
                    {
                        row.Cells["Vencida"].Style.BackColor = System.Drawing.Color.Red;
                        row.Cells["Vencida"].Style.ForeColor = System.Drawing.Color.White;
                    }

                    if (row.Cells["Estado"].Value?.ToString() == "Liquidada")
                    {
                        row.Cells["Estado"].Style.BackColor = System.Drawing.Color.Green;
                        row.Cells["Estado"].Style.ForeColor = System.Drawing.Color.White;
                    }

                    var valorMora = (row.Cells["Mora"].Value ?? "").ToString().Trim();
                    if (valorMora.Equals("Si", StringComparison.OrdinalIgnoreCase) ||
                        valorMora.Equals("Sí", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;

                        var cell = row.Cells["Mora"];
                        cell.Style.BackColor = System.Drawing.Color.DarkRed;
                        cell.Style.ForeColor = System.Drawing.Color.White;
                        cell.Style.SelectionBackColor = System.Drawing.Color.Red;
                        cell.Style.SelectionForeColor = System.Drawing.Color.White;
                    }
                }

                if (dgvFacturasCredito.Columns.Contains("ProximoPagoDate"))
                {
                    dgvFacturasCredito.Sort(
                        dgvFacturasCredito.Columns["ProximoPagoDate"],
                        ListSortDirection.Ascending);
                }

                RecalcularTotalSaldoPendiente();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las facturas a crédito: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
                _cargandoFacturas = false;
            }
        }

        private void btnregistrarabono_Click(object sender, EventArgs e)
        {
            if (dgvFacturasCredito.SelectedRows.Count > 0)
            {
                var filaSeleccionada = dgvFacturasCredito.SelectedRows[0];

                int idVenta = Convert.ToInt32(filaSeleccionada.Cells["IdVenta"].Value);
                string numeroDocumento = filaSeleccionada.Cells["NumeroDocumento"].Value.ToString();
                string nombreCliente = filaSeleccionada.Cells["NombreCliente"].Value.ToString();
                decimal montoTotal = Convert.ToDecimal(filaSeleccionada.Cells["MontoTotal"].Value);

                // Descripción que ves en FrmVentasCredito
                string descripcion = filaSeleccionada.Cells["Descripcion"].Value?.ToString() ?? "N/A";

                // Fecha vencimiento
                DateTime fechaVencimiento;
                string fechaVencimientoStr = filaSeleccionada.Cells["FechaVencimiento"].Value.ToString();

                if (!DateTime.TryParseExact(fechaVencimientoStr, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVencimiento))
                {
                    MessageBox.Show("La fecha de vencimiento no es válida. Formato requerido: dd/MM/yyyy.");
                    return;
                }

                decimal saldoPendiente = Convert.ToDecimal(filaSeleccionada.Cells["SaldoPendiente"].Value);
                string estado = filaSeleccionada.Cells["Estado"].Value.ToString();

                // Abrir modal
                FrmCredito frmCredito = new FrmCredito();

                // Ahora solo pasamos descripción (la prima se calculará en FrmCredito)
                frmCredito.SetFacturaData(
                    numeroDocumento,
                    nombreCliente,
                    montoTotal,
                    fechaVencimiento,
                    saldoPendiente,
                    estado,
                    descripcion
                );

                frmCredito.SetIdVenta(idVenta);
                frmCredito.Owner = this;
                frmCredito.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una venta a crédito.");
            }
        }

        private void FrmVentasCredito_Load(object sender, EventArgs e)
        {
            _timerBusquedaCredito = new System.Windows.Forms.Timer();
            _timerBusquedaCredito.Interval = 1000; // milisegundos
            _timerBusquedaCredito.Tick += TimerBusquedaCredito_Tick;
            txtbusqueda.Select();
            

            // Opciones del estado
            if (cboEstadoFiltro.Items.Count == 0)
                cboEstadoFiltro.Items.AddRange(new string[] { "Debe", "Liquidada", "Todas" });
            cboEstadoFiltro.SelectedIndex = 0;

            

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
                if (ctrl is Button btn) btn.Click += Btn_Click;

            HabilitarDoubleBuffering(dgvFacturasCredito);
            AplicarFiltro();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
            txtbusqueda.Select();
        }

        private void HabilitarDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private decimal ParseDecimal(object val)
        {
            if (val == null) return 0m;

            // Si ya viene como decimal
            if (val is decimal d) return d;

            // Si viene como string, intentamos con culturas comunes
            var s = val.ToString().Trim();

            decimal r;
            // es-HN (coma como separador de miles, punto decimal)
            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("es-HN"), out r)) return r;
            // Invariant (punto decimal)
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out r)) return r;
            // Local
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out r)) return r;

            return 0m;
        }

        private void RecalcularTotalSaldoPendiente()
        {
            // Intentar obtener el DataTable desde el DataSource
            DataTable dt = dgvFacturasCredito.DataSource as DataTable;
            if (dt == null)
            {
                var dvSrc = dgvFacturasCredito.DataSource as DataView;
                if (dvSrc != null) dt = dvSrc.Table;
            }

            decimal total = 0m;

            if (dt != null)
            {
                // Si se está usando RowFilter, sumamos sobre DefaultView (respeta el filtro)
                var dv = dt.DefaultView;
                foreach (DataRowView drv in dv)
                    total += ParseDecimal(drv["SaldoPendiente"]);
            }
            else
            {
                // Fallback: sumar filas visibles del grid
                foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
                {
                    if (row.Visible)
                        total += ParseDecimal(row.Cells["SaldoPendiente"].Value);
                }
            }

            txtTotalSaldoPendiente.Text = total.ToString("C2", new CultureInfo("es-HN"));
        }



        public void ActualizarSaldoPendiente(int idVenta, decimal TotalPagar)
        {
            foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
            {
                if (Convert.ToInt32(row.Cells["IdVenta"].Value) == idVenta)
                {
                    row.Cells["SaldoPendiente"].Value = TotalPagar;
                    break;
                }
            }
        }
       
        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            if (txtbusqueda.TextLength == 0)
            {
                txtbusqueda.Focus();
                return;
            }

            txtbusqueda.Clear();

            if (_timerBusquedaCredito != null)
                _timerBusquedaCredito.Stop();

            AplicarFiltro();
            txtbusqueda.Focus();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvFacturasCredito.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dt = new DataTable();

            foreach (DataGridViewColumn columna in dgvFacturasCredito.Columns)
            {
                if (columna.Visible)
                    dt.Columns.Add(columna.HeaderText, typeof(string));
            }

            foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
            {
                if (row.IsNewRow || !row.Visible) continue;

                List<object> valores = new List<object>();
                foreach (DataGridViewColumn col in dgvFacturasCredito.Columns)
                {
                    if (!col.Visible) continue;
                    valores.Add(row.Cells[col.Index].Value == null ? "" : row.Cells[col.Index].Value.ToString());
                }
                dt.Rows.Add(valores.ToArray());
            }

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Creditos_ComercialInverfa_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "Excel Files|*.xlsx";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var hoja = wb.Worksheets.Add(dt, "Creditos");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(savefile.FileName);
                    }
                    MessageBox.Show("Reporte generado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar reporte: " + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnregistrarventacredito_Click(object sender, EventArgs e)
        {
            using (var modal = new md_RegistrarVentaCredito())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    MessageBox.Show("Crédito registrado correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar manteniendo filtros (texto + estado "Debe / Liquidada / Todas")
                    RefrescarFormulario();
                }
            }
        }

        public async void RefrescarFormulario()
        {
            if (_cargandoFacturas)
                return;

            // Guardar filtros actuales
            string texto = txtbusqueda.Text;
            string estado = cboEstadoFiltro.SelectedItem?.ToString() ?? "Todas";

            btnrefrescar.Enabled = false;
            try
            {
                await CargarFacturasAsync();

                // Restaurar filtros
                txtbusqueda.Text = texto;
                int idx = cboEstadoFiltro.Items.IndexOf(estado);
                if (idx >= 0) cboEstadoFiltro.SelectedIndex = idx;

                AplicarFiltro();
                RecalcularTotalSaldoPendiente();
            }
            finally
            {
                btnrefrescar.Enabled = true;
            }
        }



        private void btnrefrescar_Click(object sender, EventArgs e)
        {
            RefrescarFormulario();
        }

        private void cboEstadoFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            DataTable dt = dgvFacturasCredito.DataSource as DataTable;
            if (dt == null)
            {
                var dvSrc = dgvFacturasCredito.DataSource as DataView;
                if (dvSrc != null) dt = dvSrc.Table;
            }

            if (dt == null) return;

            string filtroTexto = (txtbusqueda.Text ?? "").Trim();
            string estadoSeleccionado = (cboEstadoFiltro.SelectedItem?.ToString() ?? "Todas").Trim();

            var sb = new StringBuilder();

            // ====== FILTRO DE TEXTO ======
            if (!string.IsNullOrEmpty(filtroTexto))
            {
                var partes = new List<string>();

                string[] tokens = filtroTexto
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int codBuscado = 0; // <- AQUÍ LO INICIALIZAMOS
                bool esCodCliente = dt.Columns.Contains("CodCliente") &&
                                    int.TryParse(filtroTexto, out codBuscado);

                if (esCodCliente)
                {
                    // SOLO por código, rápido
                    partes.Add($"CodCliente = {codBuscado}");
                }
                else
                {
                    void AddMultiLike(string col)
                    {
                        if (!dt.Columns.Contains(col) || tokens.Length == 0)
                            return;

                        var condTokens = tokens.Select(t =>
                        {
                            string safe = t.Replace("'", "''");
                            return $"Convert({col}, 'System.String') LIKE '%{safe}%'";
                        });

                        partes.Add("(" + string.Join(" AND ", condTokens) + ")");
                    }

                    AddMultiLike("FechaRegistro");
                    AddMultiLike("NumeroDocumento");
                    AddMultiLike("NombreCliente");
                    AddMultiLike("MontoTotal");
                    AddMultiLike("SaldoPendiente");
                    AddMultiLike("FechaVencimiento");
                    AddMultiLike("Estado");
                    AddMultiLike("Vencida");
                    AddMultiLike("Descripcion");
                    AddMultiLike("NumeroCuotas");
                    AddMultiLike("ProximoPago");
                    AddMultiLike("Mora");
                }

                if (partes.Count > 0)
                    sb.Append("(" + string.Join(" OR ", partes) + ")");
            }

            // ====== FILTRO POR ESTADO (Debe / Liquidada / Todas) ======
            if (!string.Equals(estadoSeleccionado, "Todas", StringComparison.OrdinalIgnoreCase))
            {
                if (sb.Length > 0) sb.Append(" AND ");
                sb.AppendFormat("Estado = '{0}'", estadoSeleccionado.Replace("'", "''"));
            }

            dt.DefaultView.RowFilter = sb.ToString();

            RecalcularTotalSaldoPendiente();
        }


        private void dgvFacturasCredito_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvFacturasCredito_SelectionChanged(object sender, EventArgs e)
        {
            btnregistrarventacredito.Text = "Registrar crédito";
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvFacturasCredito.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un crédito para eliminar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvFacturasCredito.SelectedRows[0];
            int idVenta = Convert.ToInt32(fila.Cells["IdVenta"].Value);
            string numeroDocumento = fila.Cells["NumeroDocumento"].Value?.ToString() ?? "S/N";
            string cliente = fila.Cells["NombreCliente"].Value?.ToString() ?? "N/A";

            var r = MessageBox.Show(
                "ATENCIÓN:\n\n" +
                "Se eliminará DEFINITIVAMENTE el crédito seleccionado,\n" +
                "TODAS sus cuotas y TODOS los abonos registrados.\n\n" +
                "Esta acción es IRREVERSIBLE y no existe forma de recuperar la información.\n\n" +
                $"Crédito: {numeroDocumento} - {cliente}\n\n" +
                "¿Está completamente seguro de continuar?",
                "Eliminar crédito",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (r != DialogResult.Yes)
                return;

            try
            {
                string mensaje;
                bool ok = new CN_Venta().EliminarCreditoCompleto(idVenta, out mensaje);

                if (!ok)
                {
                    MessageBox.Show("Error al eliminar el crédito: " + mensaje, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Crédito eliminado correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

               RefrescarFormulario();
                RecalcularTotalSaldoPendiente();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el crédito: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel (*.xlsx)|*.xlsx";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;
                btnImportar.Enabled = false;

                // Ejecutar la importación en segundo plano
                string resumen = await Task.Run(() => ImportarArchivoExcel(ofd.FileName));

                // Mostrar resumen cuando termine
                MessageBox.Show(resumen,
                                "Importar",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                RefrescarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general en la importación: " + ex.Message,
                                "Importar",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                btnImportar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private string ImportarArchivoExcel(string rutaArchivo)
        {
            using (XLWorkbook wb = new XLWorkbook(rutaArchivo))
            {
                var hoja = wb.Worksheet(1);
                int fila = 2; // encabezados en fila 1

                CN_Cliente cnCliente = new CN_Cliente();
                CN_Venta cnVenta = new CN_Venta();
                CN_CuotaCredito cnCuota = new CN_CuotaCredito();

                // 1) Documentos ya existentes en BD
                var documentosUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var cliExistente in cnCliente.Listar())
                {
                    if (!string.IsNullOrWhiteSpace(cliExistente.Documento))
                        documentosUsados.Add(cliExistente.Documento.Trim());
                }

                int filasLeidas = 0;
                int filasOk = 0;
                int filasError = 0;
                var detalleErrores = new StringBuilder();

                // 2) Recorrer filas del Excel
                while (!hoja.Cell(fila, 1).IsEmpty())
                {
                    filasLeidas++;

                    try
                    {
                        // ====== LECTURA BASE DEL EXCEL (según tus columnas) ======
                        string codClienteRaw = hoja.Cell(fila, 1).GetString();   // CodCli
                        string nombre = hoja.Cell(fila, 2).GetString();    // NomCli
                        string documentoRaw = hoja.Cell(fila, 3).GetString();    // IdeCli

                        // FchNacCli (4), SexCli (5), EstCivCli (6) NO SE USAN

                        string codProf = hoja.Cell(fila, 7).GetString();    // CodProf
                        string tel = hoja.Cell(fila, 8).GetString();    // TelCli
                        string cel = hoja.Cell(fila, 9).GetString();    // CelCli
                                                                        // CorreoCli (10) NO SE USA

                        string direccion = hoja.Cell(fila, 11).GetString();   // DirCli
                                                                              // LugTraCli (12), DirTraCli (13), TelTraCli (14) NO SE USAN

                        decimal montoTotal = LeerDecimal(hoja.Cell(fila, 15));      // ComTotCli
                        decimal saldoPendiente = LeerDecimal(hoja.Cell(fila, 16));      // SalCli

                        // ====== CONVERSIÓN DEL CODCLIENTE SIN ".00" ======
                        int codCliente = 0;
                        if (!string.IsNullOrWhiteSpace(codClienteRaw))
                        {
                            decimal auxCod;
                            if (decimal.TryParse(codClienteRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out auxCod) ||
                                decimal.TryParse(codClienteRaw, NumberStyles.Any, CultureInfo.CurrentCulture, out auxCod))
                            {
                                codCliente = (int)auxCod;
                            }
                        }

                        // ====== DOCUMENTO ÚNICO (CON DISTINTIVO) ======
                        string docBase;

                        if (string.IsNullOrWhiteSpace(documentoRaw))
                        {
                            docBase = "SIN-DOC-" + fila.ToString("0000");
                        }
                        else
                        {
                            docBase = documentoRaw.Trim();

                            if (docBase.Length <= 4)
                            {
                                docBase = docBase + "-" + Guid.NewGuid().ToString("N").Substring(0, 4);
                            }
                        }

                        string docFinal = docBase;
                        int intento = 1;
                        while (documentosUsados.Contains(docFinal))
                        {
                            docFinal = $"{docBase}-{intento}";
                            intento++;
                        }
                        documentosUsados.Add(docFinal);

                        // ====== TELÉFONO (Tel / Cel) ======
                        string telefono = "";
                        if (!string.IsNullOrWhiteSpace(tel) && !string.IsNullOrWhiteSpace(cel))
                            telefono = tel + " / " + cel;
                        else if (!string.IsNullOrWhiteSpace(tel))
                            telefono = tel;
                        else if (!string.IsNullOrWhiteSpace(cel))
                            telefono = cel;

                        // ====== PROFESIÓN DESDE CÓDIGO ======
                        string profesion = ObtenerProfesionDesdeCodigo(codProf); // N/A si no existe

                        // ====== REGISTRAR CLIENTE ======
                        Cliente cli = new Cliente()
                        {
                            idCliente = 0,
                            CodCliente = codCliente,
                            Documento = docFinal,
                            NombreCompleto = nombre,
                            Correo = profesion,
                            Telefono = telefono,
                            Direccion = direccion,
                            Estado = true
                        };

                        string msgCliente;
                        int idClienteNuevo = cnCliente.Registrar(cli, out msgCliente);
                        if (idClienteNuevo == 0)
                            throw new Exception("Error registrando cliente: " + msgCliente);

                        // ====== REGISTRAR CRÉDITO ======
                        DateTime fechaVencimiento = DateTime.Now.AddDays(30);

                        Venta credito = new Venta()
                        {
                            NumeroDocumento = "N/A",
                            NombreCliente = nombre,
                            DocumentoCliente = docFinal,
                            MontoTotal = montoTotal,
                            SaldoPendiente = saldoPendiente,
                            FechaVencimiento = fechaVencimiento,
                            MetodoPago = "N/A",
                            Condicion = "Credito",
                            FechaRegistro = DateTime.Now,
                            TipoDocumento = "Factura",
                            Descripcion = "N/A"
                        };

                        string msgVenta;
                        bool ok = cnVenta.RegistrarCredito(credito, out msgVenta);
                        if (!ok)
                            throw new Exception("Error registrando crédito: " + msgVenta);

                        int idVenta = cnVenta.ObtenerIdVentaPorNumeroDocumento("N/A");
                        if (idVenta <= 0)
                            throw new Exception("No se pudo obtener el IdVenta para el crédito importado.");

                        // ====== GENERAR 1 CUOTA ======
                        cnCuota.GenerarPlanCuotas(
                            idVenta,
                            saldoPendiente,
                            1,
                            "mensual",
                            fechaVencimiento
                        );

                        filasOk++;
                    }
                    catch (Exception ex)
                    {
                        filasError++;
                        detalleErrores.AppendLine($"Fila {fila}: {ex.Message}");
                    }

                    fila++;
                }

                // 3) Resumen final
                var sbResumen = new StringBuilder();
                sbResumen.AppendLine("Importación finalizada.");
                sbResumen.AppendLine($"Filas leídas   : {filasLeidas}");
                sbResumen.AppendLine($"Filas correctas: {filasOk}");
                sbResumen.AppendLine($"Filas con error: {filasError}");

                if (filasError > 0)
                {
                    sbResumen.AppendLine();
                    sbResumen.AppendLine("Detalle de errores:");
                    sbResumen.AppendLine(detalleErrores.ToString());
                }

                return sbResumen.ToString();
            }
        }


        private decimal LeerDecimal(IXLCell cell)
        {
            string texto = cell.GetValue<string>();

            if (string.IsNullOrWhiteSpace(texto))
                return 0m;

            decimal valor;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                return valor;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.CurrentCulture, out valor))
                return valor;

            throw new Exception($"No se pudo convertir a decimal el valor '{texto}'.");
        }



        private string ObtenerProfesionDesdeCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return "N/A";

            codigo = codigo.Trim();

            var profesiones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"1","MAESTRO DE EDUCACION"},
        {"2","INGENIERO"},
        {"3","PERITO MERCANTIL"},
        {"4","COMERCIANTE"},
        {"5","MAESTRO"},
        {"6","CAFETALERO"},
        {"7","COMERCIANTE"},
        {"8","BACHILLER CIENCIAS"},
        {"9","AMA DE CASA"},
        {"10","POLICIA"},
        {"11","SECA PELO"},
        {"12","ENFERMERA"},
        {"13","CAMPESINO"},
        {"14","IGLESIA"},
        {"15","MECANICO"},
        {"16","CARNICERO"},
        {"17","COMEDOR"},
        {"18","BANCO"},
        {"19","TABACALERO"},
        {"20","BRIGADA 110"},
        {"21","PERIODISTA"},
        {"22","AGRICULTOR"},
        {"23","estudiante"},
        {"24","gerente"},
        {"25","0FTALMOLOGO"},
        {"26","VIGILANTE"},
        {"27","DOCTOR"},
        {"28","INVERFAM"},
        {"29","ABOGADO"},
        {"30","GUARDIA"},
        {"31","SANTA FE"},
        {"32","AMA DE CASA"},
        {"33","ASEADORA"},
        {"34","AMA DE CASA"},
    };

            if (profesiones.TryGetValue(codigo, out string desc) && !string.IsNullOrWhiteSpace(desc))
                return desc;

            return "N/A";
        }

        private int? BuscarColumna(Dictionary<string, int> headers, string patron)
        {
            patron = patron.ToLower();
            foreach (var kv in headers)
            {
                if (kv.Key.Contains(patron))
                    return kv.Value;
            }
            return null;
        }

        // -----------------------------------------------------------------
        // Helper para decimales
        // -----------------------------------------------------------------
        private decimal ParseDecimalDesdeTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return 0m;

            return ParseDecimal(texto); // reutiliza el que ya tenés en el form
        }

        // -----------------------------------------------------------------
        // Helper para fechas flexibles (dd/MM/yy, dd/MM/yyyy, etc.)
        // -----------------------------------------------------------------
        private bool TryParseFechaFlexible(string texto, out DateTime fecha)
        {
            fecha = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.Trim();

            string[] formatos = new[]
            {
        "dd/MM/yyyy", "d/M/yyyy",
        "dd/MM/yy",   "d/M/yy",
        "dd-MM-yyyy", "d-M-yyyy",
        "dd-MM-yy",   "d-M-yy"
    };

            // Intentos exactos con cultura hondureña
            foreach (var fmt in formatos)
            {
                if (DateTime.TryParseExact(
                        texto,
                        fmt,
                        new CultureInfo("es-HN"),
                        DateTimeStyles.None,
                        out fecha))
                    return true;
            }

            // Fallback: parse normal con es-HN e Invariant
            if (DateTime.TryParse(texto, new CultureInfo("es-HN"), DateTimeStyles.None, out fecha))
                return true;

            if (DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                return true;

            return false;
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            if (dgvFacturasCredito.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Creditos_ComercialInverfa_{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "PDF Files|*.pdf";

            if (savefile.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25f, 25f, 30f, 30f);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
                    doc.Open();

                    iTextSharp.text.Font fuenteTitulo = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 14f);
                    iTextSharp.text.Font fuenteSubtitulo = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 11f);
                    iTextSharp.text.Font fuenteTexto = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8f);

                    iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("COMERCIAL INVERFA", fuenteTitulo);
                    titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    doc.Add(titulo);

                    iTextSharp.text.Paragraph subtitulo = new iTextSharp.text.Paragraph("REPORTE DE CRÉDITOS", fuenteSubtitulo);
                    subtitulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    subtitulo.SpacingAfter = 5f;
                    doc.Add(subtitulo);

                    iTextSharp.text.Paragraph fecha = new iTextSharp.text.Paragraph("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fuenteTexto);
                    fecha.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    fecha.SpacingAfter = 10f;
                    doc.Add(fecha);

                    List<DataGridViewColumn> columnas = new List<DataGridViewColumn>();
                    foreach (DataGridViewColumn col in dgvFacturasCredito.Columns)
                    {
                        if (col.Visible)
                            columnas.Add(col);
                    }

                    iTextSharp.text.pdf.PdfPTable tabla = new iTextSharp.text.pdf.PdfPTable(columnas.Count);
                    tabla.WidthPercentage = 100f;

                    foreach (var col in columnas)
                    {
                        iTextSharp.text.pdf.PdfPCell celdaEncabezado = new iTextSharp.text.pdf.PdfPCell(
                            new iTextSharp.text.Phrase(col.HeaderText, fuenteSubtitulo)
                        );
                        celdaEncabezado.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        celdaEncabezado.BackgroundColor = new iTextSharp.text.BaseColor(230, 230, 230);
                        celdaEncabezado.Padding = 3f;
                        tabla.AddCell(celdaEncabezado);
                    }

                    foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
                    {
                        if (row.IsNewRow || !row.Visible) continue;

                        foreach (var col in columnas)
                        {
                            string texto = row.Cells[col.Index].Value == null ? "" : row.Cells[col.Index].Value.ToString();
                            iTextSharp.text.pdf.PdfPCell celda = new iTextSharp.text.pdf.PdfPCell(
                                new iTextSharp.text.Phrase(texto, fuenteTexto)
                            );
                            celda.Padding = 2f;
                            tabla.AddCell(celda);
                        }
                    }

                    doc.Add(tabla);
                    doc.Close();
                    stream.Close();
                }

                MessageBox.Show("Reporte PDF generado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void TimerBusquedaCredito_Tick(object sender, EventArgs e)
        {
            _timerBusquedaCredito.Stop();
            AplicarFiltro();
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (_timerBusquedaCredito != null)
                    _timerBusquedaCredito.Stop();

                AplicarFiltro();
            }
        }
    }
}
