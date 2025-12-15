using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace CapaPresentacion
{
    public partial class frmCierreCaja : Form
    {
        private CN_CierreCaja cnCierre = new CN_CierreCaja();
        private CN_Egreso cnEgreso = new CN_Egreso();
        private Usuario _Usuario;
        private bool _silencioEventos = false;
        public frmCierreCaja(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmCierreCaja_Load(object sender, EventArgs e)
        {
            var ultimoCierre = cnCierre.ObtenerUltimoCierre();

            if (ultimoCierre.UltimaFecha.HasValue)
            {
                // 👇 solo fecha (sin hora)
                dtpFechaInicio.Value = ultimoCierre.UltimaFecha.Value.Date;
                txtMontoInicial.Text = $"L. {ultimoCierre.MontoInicial?.ToString("0.00") ?? "0.00"}";
            }
            else
            {
                dtpFechaInicio.Value = DateTime.Today;
                txtMontoInicial.Text = "L. 0.00";
            }

            dtpFechaFin.Value = DateTime.Now;
            txtUsuario.Text = _Usuario?.NombreCompleto ?? "SIN USUARIO";

            dtpFechaInicio.ValueChanged += (s, ev) => RefrescarAlCambiarFechas();
            dtpFechaFin.ValueChanged += (s, ev) => RefrescarAlCambiarFechas();

            txtMontoInicial.Leave += (s, ev) => NormalizarYRecalcularMontoInicial();
            txtMontoInicial.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    NormalizarYRecalcularMontoInicial();
                    ev.SuppressKeyPress = true;
                }
            };

            CargarDatosCierre();

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



        private void RefrescarAlCambiarFechas()
        {
            if (_silencioEventos) return;
            CargarDatosCierre();
        }

        private void NormalizarYRecalcularMontoInicial()
        {
            if (_silencioEventos) return;

            _silencioEventos = true;
            try
            {
                // Normaliza a formato "L. 0.00"
                decimal monto = ConvertirMoneda(txtMontoInicial.Text);
                txtMontoInicial.Text = $"L. {monto:0.00}";

                // Recalcula todo (ingresos/egresos y totales) con el monto actual
                CargarDatosCierre();
            }
            finally
            {
                _silencioEventos = false;
            }
        }


        private void CargarDatosCierre()
        {
            DateTime fechaInicio = dtpFechaInicio.Value;
            DateTime fechaFin = dtpFechaFin.Value;

            if (fechaFin <= fechaInicio)
                fechaFin = DateTime.Now;

            // Epsilon para cubrir BETWEEN/exclusiones
            fechaFin = fechaFin.AddSeconds(1);

            decimal montoInicial = ConvertirMoneda(txtMontoInicial.Text);

            decimal ingresosEfectivo = cnEgreso.ObtenerTotalVentasPorMetodo("Efectivo", fechaInicio, fechaFin);
            decimal ingresosTarjeta = cnEgreso.ObtenerTotalVentasPorMetodo("Tarjeta", fechaInicio, fechaFin);
            decimal ingresosTransferencia = cnEgreso.ObtenerTotalVentasPorMetodo("Transferencia", fechaInicio, fechaFin);

            decimal egresosEfectivo = cnEgreso.ObtenerTotalEgresosPorMetodo("Efectivo", fechaInicio, fechaFin);
            decimal egresosTransferencia = cnEgreso.ObtenerTotalEgresosPorMetodo("Transferencia", fechaInicio, fechaFin);

            decimal totalCalculadoEfectivo = montoInicial + ingresosEfectivo - egresosEfectivo;
            decimal totalCalculadoTransfer = ingresosTransferencia - egresosTransferencia;
            decimal granTotal = montoInicial + ingresosEfectivo + ingresosTarjeta + ingresosTransferencia
                                - egresosEfectivo - egresosTransferencia;

            txtEfectivo.Text = ingresosEfectivo.ToString("0.00");
            txtTarjeta.Text = ingresosTarjeta.ToString("0.00");
            txtTransferencia.Text = ingresosTransferencia.ToString("0.00");
            txtEgresosEfectivo.Text = egresosEfectivo.ToString("0.00");
            txtEgresosTrasnferencia.Text = egresosTransferencia.ToString("0.00");
            txtTotalCalculado.Text = totalCalculadoEfectivo.ToString("0.00");
            txtTotalTransferencia.Text = totalCalculadoTransfer.ToString("0.00");
            txtGranTotal.Text = granTotal.ToString("0.00");
        }





        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            try
            {
                CargarDatosCierre();

                decimal totalRealEfectivo = ConvertirMoneda(txtTotalReal.Text);
                if (totalRealEfectivo < 0)
                {
                    MessageBox.Show("El total real de efectivo no puede ser negativo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CierreCaja cierre = new CierreCaja()
                {
                    FechaInicio = dtpFechaInicio.Value,
                    FechaFin = dtpFechaFin.Value,
                    MontoInicial = ConvertirMoneda(txtMontoInicial.Text),
                    IngresosEfectivo = ConvertirMoneda(txtEfectivo.Text),
                    IngresosTarjeta = ConvertirMoneda(txtTarjeta.Text),
                    IngresosTransferencia = ConvertirMoneda(txtTransferencia.Text),
                    Egresos = ConvertirMoneda(txtEgresosEfectivo.Text),
                    EgresosTransferencia = ConvertirMoneda(txtEgresosTrasnferencia.Text),
                    TotalCalculado = ConvertirMoneda(txtTotalCalculado.Text),
                    TotalTransferencia = ConvertirMoneda(txtTotalTransferencia.Text),
                    GranTotal = ConvertirMoneda(txtGranTotal.Text),
                    TotalReal = totalRealEfectivo,
                    Diferencia = totalRealEfectivo - ConvertirMoneda(txtTotalCalculado.Text),
                    UsuarioResponsable = (txtUsuario.Text ?? string.Empty).Trim(),
                    Observaciones = (txtObservaciones.Text ?? string.Empty).Trim()
                };

                bool resultado = cnCierre.Registrar(cierre, out string mensaje);
                if (!resultado)
                {
                    MessageBox.Show("Error: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                DialogResult opcion = MessageBox.Show("¿Desea generar el reporte de cierre de caja?", "Impresión y PDF", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (opcion == DialogResult.Yes)
                {
                    DialogResult opcionTipo = MessageBox.Show("¿Desea imprimir en térmica?", "¿Imprimir térmica?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (opcionTipo == DialogResult.Yes) ImprimirCierreCajaTermica(cierre);
                    else if (opcionTipo == DialogResult.No) GenerarReporteCierreCajaPDF(cierre);
                    else if (opcionTipo == DialogResult.Cancel) { GenerarReporteCierreCajaPDF(cierre); ImprimirCierreCajaTermica(cierre); }
                }

                // Guardar el monto inicial del siguiente ciclo como el total real contado (por día)
                cnCierre.GuardarMontoInicial(cierre.FechaFin.Date, cierre.TotalReal, cierre.UsuarioResponsable, out string _);

                MessageBox.Show("Cierre de caja registrado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Error en el formato de campos numéricos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cerrar caja: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtTotalReal_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTotalReal.Text, out decimal totalReal) &&
             decimal.TryParse(txtTotalCalculado.Text, out decimal totalCalculado))
            {
                txtDiferencia.Text = (totalReal - totalCalculado).ToString("0.00");
            }
            else
            {
                txtDiferencia.Text = "0.00";
            }
        }

        private decimal ConvertirMoneda(string texto)
        {
            return decimal.TryParse(texto.Replace("L.", "").Replace("L", "").Trim(), out decimal valor)
                ? valor
                : 0;
        }


        private void btnGuardarMontoInicial_Click(object sender, EventArgs e)
        {
            decimal montoInicial = ConvertirMoneda(txtMontoInicial.Text);
            if (montoInicial < 0)
            {
                MessageBox.Show("Monto inicial inválido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guardamos el monto inicial para el día del FIN actual
            bool ok = cnCierre.GuardarMontoInicial(dtpFechaFin.Value.Date, montoInicial, txtUsuario.Text.Trim(), out string mensaje);
            if (!ok)
            {
                MessageBox.Show("Error: " + mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Monto inicial guardado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Ajustar rango inmediatamente (sin dejarlo vacío)
            _silencioEventos = true;
            try
            {
                // Nuevo inicio = lo que acabamos de fijar como “punto de corte”
                var nuevoInicio = dtpFechaFin.Value.Date;
                dtpFechaInicio.Value = nuevoInicio;

                // Fin = ahora mismo; si por alguna razón queda <= inicio, lo empujamos 1 seg
                var ahora = DateTime.Now;
                dtpFechaFin.Value = (ahora <= nuevoInicio) ? nuevoInicio.AddSeconds(1) : ahora;

                // Normalizar texto
                txtMontoInicial.Text = $"L. {montoInicial:0.00}";
            }
            finally
            {
                _silencioEventos = false;
            }

            // Recalcular cuando la UI ya quedó estable
            this.BeginInvoke(new Action(() =>
            {
                CargarDatosCierre();
                txtTotalReal.Text = string.Empty;
                txtDiferencia.Text = "0.00";
            }));
        }

        private void ImprimirCierreCajaTermica(CierreCaja cierre)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = @"\\192.168.0.9\POS-80 (copy 1)".Normalize().Replace('\u00A0', ' ').Trim();
            printDoc.PrintPage += (s, e) => PrintCierreCajaTermico(s, e, cierre);

            this.Activate();

            try
            {
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir el cierre de caja: " + ex.Message);
            }

            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void PrintCierreCajaTermico(object sender, PrintPageEventArgs e, CierreCaja cierre)
        {
            System.Drawing.Font font = new System.Drawing.Font("Consolas", 8);
            int y = 20;

            void DrawText(string text)
            {
                e.Graphics.DrawString(text, font, Brushes.Black, 10, y);
                y += 15;
            }

            DrawText("********** CIERRE DE CAJA **********");
            DrawText($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
            DrawText($"Usuario: {cierre.UsuarioResponsable}");
            DrawText("-----------------------------------------------------------------------------------------");
            DrawText($"Monto Inicial efectivo:     L. {cierre.MontoInicial:0.00}");
            DrawText($"Ingreso Efectivo:           L. {cierre.IngresosEfectivo:0.00}");
            DrawText($"Ingreso Tarjeta:            L. {cierre.IngresosTarjeta:0.00}");
            DrawText($"Ingreso Transfer:           L. {cierre.IngresosTransferencia:0.00}");
            DrawText($"Egreso Efectivo:            L. {cierre.Egresos:0.00}");
            DrawText($"Egreso Transfer:            L. {cierre.EgresosTransferencia:0.00}");
            DrawText("-----------------------------------------------------------------------------------------");
            DrawText($"Total Efectivo:             L. {cierre.TotalCalculado:0.00}");
            DrawText($"Total Transfer.:            L. {cierre.TotalTransferencia:0.00}");
            DrawText($"Gran Total:                 L. {cierre.GranTotal:0.00}");
            DrawText($"Total Real efectivo:        L. {cierre.TotalReal:0.00}");
            DrawText($"Diferencia:                 L. {cierre.Diferencia:0.00}");
            DrawText("-----------------------------------------------------------------------------------------");
            DrawText("Observaciones:");
            DrawText(cierre.Observaciones);
            DrawText("-----------------------------------------------------------------------------------------");
            DrawText("REPORTE CIERRE DE CAJA AMERICAN COLORS.");
        }


        private void GenerarReporteCierreCajaPDF(CierreCaja cierre)
        {
            string plantilla = Properties.Resources.Plantilla_CierreCaja;
            // ⚠️ Usa la ruta correcta

            string htmlFinal = plantilla
                .Replace("@usuario", txtUsuario.Text)
                .Replace("@fechainicio", dtpFechaInicio.Value.ToString("dd/MM/yyyy HH:mm"))
                .Replace("@fechafin", dtpFechaFin.Value.ToString("dd/MM/yyyy HH:mm"))
                .Replace("@montoinicial", txtMontoInicial.Text)
                .Replace("@ingefectivo", txtEfectivo.Text)
                .Replace("@ingtarjeta", txtTarjeta.Text)
                .Replace("@ingtransferencia", txtTransferencia.Text)
                .Replace("@egr_efectivo", txtEgresosEfectivo.Text)
                .Replace("@egr_transferencia", txtEgresosTrasnferencia.Text)
                .Replace("@total_efectivo", txtTotalCalculado.Text)
                .Replace("@total_transferencia", txtTotalTransferencia.Text)
                .Replace("@grantotal", txtGranTotal.Text)
                .Replace("@totalreal", txtTotalReal.Text)
                .Replace("@diferencia", txtDiferencia.Text)
                .Replace("@observaciones", txtObservaciones.Text);

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = "CierreCaja_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
            saveFile.Filter = "Archivo PDF|*.pdf";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFile.FileName, FileMode.Create))
                {
                    using (Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        using (StringReader sr = new StringReader(htmlFinal))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        }
                        pdfDoc.Close();
                    }
                }

                MessageBox.Show("Reporte PDF generado correctamente.", "Cierre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
