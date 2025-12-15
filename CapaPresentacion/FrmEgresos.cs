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
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace CapaPresentacion
{
    public partial class FrmEgresos : Form
    {
        private CN_Egreso cn_egreso = new CN_Egreso();
        private Usuario _Usuario;
        private int _idEgresoEdit = 0;
        public FrmEgresos(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1024, 700);
            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void FrmEgresos_Load(object sender, EventArgs e)
        {
            txtfechainicio.Value = DateTime.Today;
            txtfechafin.Value = DateTime.Today;

            // Controles de registro
            dtpFechaEgreso.Format = DateTimePickerFormat.Custom;
            dtpFechaEgreso.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpFechaEgreso.Value = DateTime.Now;

            cboMetodoPago.Items.Clear();
            cboMetodoPago.Items.AddRange(new[] { "Efectivo", "Transferencia", "Tarjeta" });
            cboMetodoPago.SelectedIndex = 0;

            cboCategoria.Items.Clear();
            cboCategoria.Items.AddRange(new[] { "Negocio", "Casa" });
            cboCategoria.SelectedIndex = 0;

            cboPagado.Items.Clear();
            cboPagado.Items.AddRange(new[] { "Pagado", "No pagado" });
            cboPagado.SelectedIndex = 0;
            dtpFechaPago.Enabled = (cboPagado.SelectedIndex == 0);

            // Filtros
            cboVista.Items.Clear();
            cboVista.Items.AddRange(new[] { "Flujo", "Devengado" });
            cboVista.SelectedIndex = 0;

            cboFiltroTipo.Items.Clear();
            cboFiltroTipo.Items.AddRange(new[]
            {
        "Todos","Venta (Contado)","Compra (Contado)","Abono a Crédito","Abono Compra Crédito","Egreso Manual"
    });
            cboFiltroTipo.SelectedIndex = 0;

            cboFiltroMetodo.Items.Clear();
            cboFiltroMetodo.Items.AddRange(new[] { "Todos", "Efectivo", "Transferencia", "Tarjeta" });
            cboFiltroMetodo.SelectedIndex = 0;

            cboFiltroCategoria.Items.Clear();
            cboFiltroCategoria.Items.AddRange(new[] { "Todos", "Negocio", "Casa" });
            cboFiltroCategoria.SelectedIndex = 0;

            cboFiltroPagado.Items.Clear();
            cboFiltroPagado.Items.AddRange(new[] { "Todos", "Pagado", "No pagado" });
            cboFiltroPagado.SelectedIndex = 0;

            // Grid: asegúrate de tener estas columnas (IdEgreso oculta)
            // Fecha, TipoMovimiento, Descripcion, Ingreso, Egreso, MetodoPago, Usuario, Categoria, EsPagado, FechaPago, IdEgreso

            CargarLibroFinanciero();

            if (dgvLibroFinanciero.Columns.Contains("Fecha"))
            {
                dgvLibroFinanciero.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvLibroFinanciero.Columns["Fecha"].ValueType = typeof(DateTime);
            }

            if (dgvLibroFinanciero.Columns.Contains("FechaPago"))
            {
                dgvLibroFinanciero.Columns["FechaPago"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvLibroFinanciero.Columns["FechaPago"].ValueType = typeof(DateTime);
            }

            AplicarAnchorsYGrid();

            ConfigurarTotalesUI();
            AjustarAnchoTotales();
            this.Resize += (s, ev) => AjustarAnchoTotales();


            var totales = new[] { lblTotIngresos, lblTotProveedores, lblGastosNegocio, lblGastosCasa, lblUtilidad };
            foreach (var l in totales)
            {
                if (l == null) continue;
                l.AutoSize = false;
                l.TextAlign = ContentAlignment.MiddleRight;
                l.Font = new System.Drawing.Font("Segoe UI", 10.5f, System.Drawing.FontStyle.Bold);
                l.Padding = new Padding(0, 2, 6, 2);
            }


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

        private void AplicarAnchorsYGrid()
        {
            // --- GRID: que crezca a lo ancho y alto ---
            dgvLibroFinanciero.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvLibroFinanciero.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLibroFinanciero.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // --- Campos largos que deben estirarse horizontalmente ---
            if (txtDescripcion != null)
                txtDescripcion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            if (txtFiltroDescripcion != null)
                txtFiltroDescripcion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // --- Botones que deben quedar pegados al lado derecho ---
            if (btnRegistrar != null)
                btnRegistrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            if (btnEliminar != null)
                btnEliminar.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Si tienes botones de exportar a la izquierda, déjalos anclados arriba/izquierda
            if (btndescargar != null)
                btndescargar.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            if (BtnExportar != null)
                BtnExportar.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // --- Totales: pegados a la parte inferior izquierda para que no se corten ---
            if (lblTotIngresos != null) lblTotIngresos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            if (lblTotProveedores != null) lblTotProveedores.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            if (lblGastosNegocio != null) lblGastosNegocio.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            if (lblGastosCasa != null) lblGastosCasa.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            if (lblUtilidad != null) lblUtilidad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
            { MessageBox.Show("Monto inválido"); return; }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            { MessageBox.Show("Ingrese una descripción"); return; }

            var eg = new Egreso
            {
                IdEgreso = _idEgresoEdit,
                Fecha = dtpFechaEgreso.Value,
                Monto = monto,
                Descripcion = txtDescripcion.Text.Trim(),
                Usuario = _Usuario?.NombreCompleto ?? "Sistema",
                MetodoPago = cboMetodoPago.SelectedItem.ToString(),
                Categoria = cboCategoria.SelectedItem.ToString(),
                EsPagado = (cboPagado.SelectedItem.ToString() == "Pagado"),
                FechaPago = (cboPagado.SelectedItem.ToString() == "Pagado") ? dtpFechaPago.Value : (DateTime?)null
            };

            string mensaje;
            bool ok = (_idEgresoEdit == 0)
                ? cn_egreso.Registrar(eg, out mensaje)
                : cn_egreso.Actualizar(eg, out mensaje);

            if (ok)
            {
                MessageBox.Show(_idEgresoEdit == 0 ? "Egreso registrado" : "Egreso actualizado");
                LimpiarCamposEdicion();
                CargarLibroFinanciero();
            }
            else MessageBox.Show("Error: " + mensaje);
        }

        private void LimpiarCamposEdicion()
        {
            _idEgresoEdit = 0;
            dtpFechaEgreso.Value = DateTime.Now;
            txtMonto.Text = "";
            txtDescripcion.Text = "";
            cboMetodoPago.SelectedIndex = 0;
            cboCategoria.SelectedIndex = 0;
            cboPagado.SelectedIndex = 0;
            dtpFechaPago.Value = DateTime.Now;
        }
        private void CargarLibroFinanciero()
        {
            dgvLibroFinanciero.Rows.Clear();

            DateTime fechaInicio = txtfechainicio.Value.Date;
            DateTime fechaFin = txtfechafin.Value.Date.AddDays(1).AddSeconds(-1);

            string vista = cboVista.SelectedItem?.ToString() ?? "Flujo";
            string tipo = cboFiltroTipo.SelectedItem?.ToString(); if (tipo == "Todos") tipo = null;
            string metodo = cboFiltroMetodo.SelectedItem?.ToString(); if (metodo == "Todos") metodo = null;
            string categoria = cboFiltroCategoria.SelectedItem?.ToString(); if (categoria == "Todos") categoria = null;

            bool? pagado = null;
            if (cboFiltroPagado.SelectedItem?.ToString() == "Pagado") pagado = true;
            else if (cboFiltroPagado.SelectedItem?.ToString() == "No pagado") pagado = false;

            string desc = string.IsNullOrWhiteSpace(txtFiltroDescripcion.Text) ? null : txtFiltroDescripcion.Text.Trim();

            DataTable dt = cn_egreso.ObtenerLibroFinanciero(fechaInicio, fechaFin, vista, tipo, desc, metodo, categoria, pagado);

            decimal totIngresos = 0, totProv = 0, gastosNeg = 0, gastosCasa = 0;

            foreach (DataRow r in dt.Rows)
            {
                string tipoMov = r["TipoMovimiento"].ToString();
                decimal ing = r.Field<decimal>("Ingreso");
                decimal egr = r.Field<decimal>("Egreso");
                string cat = r["Categoria"] == DBNull.Value ? "" : r["Categoria"].ToString();

                if (tipoMov == "Venta (Contado)" || tipoMov == "Abono a Crédito") totIngresos += ing;
                else if (tipoMov == "Compra (Contado)" || tipoMov == "Abono Compra Crédito") totProv += egr;
                else if (tipoMov == "Egreso Manual")
                {
                    if (cat == "Negocio") gastosNeg += egr;
                    else if (cat == "Casa") gastosCasa += egr;
                }

                dgvLibroFinanciero.Rows.Add(
    Convert.ToDateTime(r["FechaRegistro"]),                  // <-- SIEMPRE la fecha de registro
    r.Table.Columns.Contains("IdEgreso") ? r["IdEgreso"] : DBNull.Value,
    r["Categoria"],
    (r["EsPagado"] != DBNull.Value && Convert.ToBoolean(r["EsPagado"])) ? "Pagado" : "No pagado",
    (r["FechaPago"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(r["FechaPago"])),
    r["TipoMovimiento"],
    r["Descripcion"],
    (ing == 0 ? "" : $"L. {ing:0.00}"),
    (egr == 0 ? "" : $"L. {egr:0.00}"),
    r["MetodoPago"],
    r["Usuario"]
);

            }

            var inv = System.Globalization.CultureInfo.InvariantCulture;
            lblTotIngresos.Text = $"Ingresos: L. {totIngresos.ToString("#,##0.00", inv)}";
            lblTotProveedores.Text = $"A Proveedores: L. {totProv.ToString("#,##0.00", inv)}";
            lblGastosNegocio.Text = $"Gastos Operativos: L. {gastosNeg.ToString("#,##0.00", inv)}";
            lblGastosCasa.Text = $"Gastos Casa: L. {gastosCasa.ToString("#,##0.00", inv)}";
            lblUtilidad.Text = $"Utilidad: L. {(totIngresos - (totProv + gastosNeg + gastosCasa)).ToString("#,##0.00", inv)}";

        }

        private void btnbuscarresultado_Click(object sender, EventArgs e)
        {
            CargarLibroFinanciero();
        }

        private void btndescargar_Click(object sender, EventArgs e)
        {
            if (dgvLibroFinanciero.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = $"Libro_Financiero_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                sfd.Filter = "PDF Files|*.pdf";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                string html = Properties.Resources.PlantillaEgresos; // plantilla con desglose

                html = html.Replace("@usuario", _Usuario?.NombreCompleto ?? "Sistema")
                           .Replace("@fecha", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                           .Replace("@fechainicio", txtfechainicio.Value.ToString("dd/MM/yyyy"))
                           .Replace("@fechafin", txtfechafin.Value.ToString("dd/MM/yyyy"))
                           .Replace("@vista", cboVista.SelectedItem?.ToString() ?? "")
                           .Replace("@filtrotipo", cboFiltroTipo.SelectedItem?.ToString() ?? "Todos")
                           .Replace("@filtrometodo", cboFiltroMetodo.SelectedItem?.ToString() ?? "Todos")
                           .Replace("@filtrocategoria", cboFiltroCategoria.SelectedItem?.ToString() ?? "Todos")
                           .Replace("@filtropagado", cboFiltroPagado.SelectedItem?.ToString() ?? "Todos")
                           .Replace("@filtrodesc", string.IsNullOrWhiteSpace(txtFiltroDescripcion.Text) ? "(todas)" : txtFiltroDescripcion.Text.Trim());

                var sbRows = new StringBuilder();
                int index = 1;

                decimal totIngresos = 0m;
                decimal totProveedores = 0m; // Compras contado + Abonos compra crédito
                decimal gastosNegocio = 0m;
                decimal gastosCasa = 0m;

                foreach (DataGridViewRow row in dgvLibroFinanciero.Rows)
                {
                    if (row.IsNewRow) continue;

                    string tipoMov = row.Cells["TipoMovimiento"]?.Value?.ToString() ?? "";
                    string categoria = row.Cells["Categoria"]?.Value?.ToString() ?? "";

                    // Fechas
                    string fechaTxt = "";
                    if (DateTime.TryParse(row.Cells["Fecha"]?.Value?.ToString(), out var f))
                        fechaTxt = f.ToString("dd/MM/yyyy HH:mm");
                    string fechaPagoTxt = "";
                    if (DateTime.TryParse(row.Cells["FechaPago"]?.Value?.ToString(), out var fp))
                        fechaPagoTxt = fp.ToString("dd/MM/yyyy HH:mm");

                    // Valores
                    decimal ingreso = ParseMoneyCell(row.Cells["Ingreso"]?.Value);
                    decimal egreso = ParseMoneyCell(row.Cells["Egreso"]?.Value);

                    // Clasificación para desglose
                    if (tipoMov == "Venta (Contado)" || tipoMov == "Abono a Crédito")
                        totIngresos += ingreso;
                    else if (tipoMov == "Compra (Contado)" || tipoMov == "Abono Compra Crédito")
                        totProveedores += egreso;
                    else if (tipoMov == "Egreso Manual")
                    {
                        if (categoria == "Negocio") gastosNegocio += egreso;
                        else if (categoria == "Casa") gastosCasa += egreso;
                    }

                    // Otras columnas
                    string desc = row.Cells["Descripcion"]?.Value?.ToString() ?? "";
                    string metodo = row.Cells["MetodoPago"]?.Value?.ToString() ?? "";
                    string estado = row.Cells["EsPagado"]?.Value?.ToString() ?? "";
                    string usuario = row.Cells["Usuario"]?.Value?.ToString() ?? "";

                    // Fila detalle
                    sbRows.AppendLine("<tr>");
                    sbRows.AppendFormat("<td class='center'>{0}</td>", index++);
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(fechaTxt));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(tipoMov));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(desc));
                    sbRows.AppendFormat("<td class='right'>{0}</td>", ingreso == 0 ? "" : ingreso.ToString("#,##0.00"));
                    sbRows.AppendFormat("<td class='right'>{0}</td>", egreso == 0 ? "" : egreso.ToString("#,##0.00"));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(metodo));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(categoria));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(estado));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(fechaPagoTxt));
                    sbRows.AppendFormat("<td>{0}</td>", System.Net.WebUtility.HtmlEncode(usuario));
                    sbRows.AppendLine("</tr>");
                }

                decimal utilidad = totIngresos - (totProveedores + gastosNegocio + gastosCasa);

                html = html.Replace("@filas", sbRows.ToString())
                           .Replace("@totalregistros", dgvLibroFinanciero.Rows.Count.ToString())
                           .Replace("@totalingresos", totIngresos.ToString("#,##0.00"))
                           .Replace("@totalproveedores", totProveedores.ToString("#,##0.00"))
                           .Replace("@gastosnegocio", gastosNegocio.ToString("#,##0.00"))
                           .Replace("@gastoscasa", gastosCasa.ToString("#,##0.00"))
                           .Replace("@utilidad", utilidad.ToString("#,##0.00"));

                // PDF
                using (var fs = new FileStream(sfd.FileName, FileMode.Create))
                using (var pdfDoc = new Document(PageSize.A4.Rotate(), 25, 25, 35, 35))
                {
                    var writer = PdfWriter.GetInstance(pdfDoc, fs);
                    writer.PageEvent = new PdfPageEventsSimple("Libro Financiero");
                    pdfDoc.Open();

                    using (var sr = new StringReader(html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }

                    pdfDoc.Close();
                }

                MessageBox.Show("PDF generado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public class PdfPageEventsSimple : PdfPageEventHelper
        {
            private readonly string _title;
            public PdfPageEventsSimple(string title) { _title = title; }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var cb = writer.DirectContent;
                var font = FontFactory.GetFont(FontFactory.HELVETICA, 8, iTextSharp.text.BaseColor.GRAY);

                // Encabezado
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT,
                    new Phrase(_title, font),
                    document.LeftMargin, document.PageSize.Height - 20, 0);

                // Pie (página x de y)
                ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT,
                    new Phrase($"Página {writer.PageNumber}", font),
                    document.PageSize.Width - document.RightMargin, document.BottomMargin - 10, 0);
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvLibroFinanciero.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var dt = BuildDataTableFromGrid();

            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = $"Libro_Financiero_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                sfd.Filter = "Excel files | *.xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(dt, "Libro");
                        ws.Columns().AdjustToContents();

                        // Formatos: usar Column(índice) (¡no Columns(índice)!).
                        var used = ws.RangeUsed();
                        if (used != null)
                        {
                            used.Column(1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm"; // Fecha
                            used.Column(9).Style.DateFormat.Format = "dd/MM/yyyy HH:mm"; // FechaPago
                            used.Column(4).Style.NumberFormat.Format = "#,##0.00";      // Ingreso
                            used.Column(5).Style.NumberFormat.Format = "#,##0.00";      // Egreso
                        }

                        // Metadata / filtros a la derecha
                        ws.Cell(1, 12).Value = "Vista:"; ws.Cell(1, 13).Value = cboVista.SelectedItem?.ToString() ?? "";
                        ws.Cell(2, 12).Value = "Rango:"; ws.Cell(2, 13).Value = $"{txtfechainicio.Value:dd/MM/yyyy} - {txtfechafin.Value:dd/MM/yyyy}";
                        ws.Cell(3, 12).Value = "Tipo:"; ws.Cell(3, 13).Value = cboFiltroTipo.SelectedItem?.ToString() ?? "Todos";
                        ws.Cell(4, 12).Value = "Método:"; ws.Cell(4, 13).Value = cboFiltroMetodo.SelectedItem?.ToString() ?? "Todos";
                        ws.Cell(5, 12).Value = "Categoría:"; ws.Cell(5, 13).Value = cboFiltroCategoria.SelectedItem?.ToString() ?? "Todos";
                        ws.Cell(6, 12).Value = "Estado:"; ws.Cell(6, 13).Value = cboFiltroPagado.SelectedItem?.ToString() ?? "Todos";
                        ws.Cell(7, 12).Value = "Descripción:"; ws.Cell(7, 13).Value = string.IsNullOrWhiteSpace(txtFiltroDescripcion.Text) ? "(todas)" : txtFiltroDescripcion.Text.Trim();

                        ws.Range("L1:L7").Style.Font.Bold = true;

                        wb.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Excel generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar Excel: " + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cboPagado_SelectedIndexChanged(object sender, EventArgs e)
        {
            dtpFechaPago.Enabled = (cboPagado.SelectedIndex == 0);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idEgresoEdit == 0) { MessageBox.Show("Seleccione un egreso manual."); return; }
            if (MessageBox.Show("¿Eliminar egreso?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            if (cn_egreso.Eliminar(_idEgresoEdit, out string msg))
            {
                MessageBox.Show("Eliminado");
                LimpiarCamposEdicion();
                CargarLibroFinanciero();
            }
            else MessageBox.Show("Error: " + msg);
        }

        private void dgvLibroFinanciero_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var row = dgvLibroFinanciero.Rows[e.RowIndex];

            string tipo = row.Cells["TipoMovimiento"]?.Value?.ToString() ?? "";
            if (!string.Equals(tipo, "Egreso Manual", StringComparison.OrdinalIgnoreCase))
            {
                _idEgresoEdit = 0;
                return;
            }

            object idVal = row.Cells["IdEgreso"]?.Value;
            if (idVal == null || idVal == DBNull.Value || !int.TryParse(idVal.ToString(), out _idEgresoEdit))
            {
                _idEgresoEdit = 0;
                MessageBox.Show("No se encontró el Id del egreso manual. Asegúrate de incluir IdEgreso en el SELECT del SP.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fechaEgreso;
            if (!DateTime.TryParse(row.Cells["Fecha"]?.Value?.ToString(), out fechaEgreso))
                fechaEgreso = DateTime.Now;
            dtpFechaEgreso.Value = fechaEgreso;

            decimal monto = 0m;
            var egresoVal = row.Cells["Egreso"]?.Value;
            if (egresoVal is decimal decOk) monto = decOk;
            else
            {
                var s = (egresoVal?.ToString() ?? "0").Trim();
                if (s.StartsWith("L.", StringComparison.OrdinalIgnoreCase)) s = s.Substring(2).Trim();
                else if (s.StartsWith("L", StringComparison.OrdinalIgnoreCase)) s = s.Substring(1).Trim();
                s = s.Replace(",", "").Replace(" ", "");
                if (!decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out monto))
                    decimal.TryParse(s, NumberStyles.Number, new CultureInfo("es-HN"), out monto);
            }
            txtMonto.Text = monto.ToString("0.00");

            txtDescripcion.Text = row.Cells["Descripcion"]?.Value?.ToString() ?? "";

            var metodo = row.Cells["MetodoPago"]?.Value?.ToString() ?? "Efectivo";
            var metodoItem = cboMetodoPago.Items.Cast<object>()
                                .FirstOrDefault(i => string.Equals(i.ToString(), metodo, StringComparison.OrdinalIgnoreCase));
            cboMetodoPago.SelectedItem = metodoItem ?? (cboMetodoPago.Items.Count > 0 ? cboMetodoPago.Items[0] : null);

            var categoria = row.Cells["Categoria"]?.Value?.ToString() ?? "Negocio";
            var categoriaItem = cboCategoria.Items.Cast<object>()
                                  .FirstOrDefault(i => string.Equals(i.ToString(), categoria, StringComparison.OrdinalIgnoreCase));
            cboCategoria.SelectedItem = categoriaItem ?? (cboCategoria.Items.Count > 0 ? cboCategoria.Items[0] : null);

            bool esPagado = string.Equals(row.Cells["EsPagado"]?.Value?.ToString(), "Pagado", StringComparison.OrdinalIgnoreCase);
            cboPagado.SelectedItem = esPagado ? "Pagado" : "No pagado";
            dtpFechaPago.Enabled = esPagado;

            DateTime fechaPago;
            if (esPagado && DateTime.TryParse(row.Cells["FechaPago"]?.Value?.ToString(), out fechaPago))
                dtpFechaPago.Value = fechaPago;
            else
                dtpFechaPago.Value = DateTime.Now;
        }

        private static decimal ParseMoneyCell(object cellVal)
        {
            if (cellVal == null || cellVal == DBNull.Value) return 0m;
            if (cellVal is decimal d) return d;
            var s = cellVal.ToString().Trim();
            if (string.IsNullOrEmpty(s)) return 0m;
            if (s.StartsWith("L.", StringComparison.OrdinalIgnoreCase)) s = s.Substring(2).Trim();
            else if (s.StartsWith("L", StringComparison.OrdinalIgnoreCase)) s = s.Substring(1).Trim();
            s = s.Replace(",", "").Replace(" ", "");
            if (decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var v))
                return v;
            if (decimal.TryParse(s, NumberStyles.Number, new CultureInfo("es-HN"), out v))
                return v;
            return 0m;
        }

        private DataTable BuildDataTableFromGrid()
        {
            var dt = new DataTable("Movimientos");
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Movimiento", typeof(string));
            dt.Columns.Add("Descripción", typeof(string));
            dt.Columns.Add("Ingreso", typeof(decimal));
            dt.Columns.Add("Egreso", typeof(decimal));
            dt.Columns.Add("Método", typeof(string));
            dt.Columns.Add("Categoría", typeof(string));
            dt.Columns.Add("Estado", typeof(string));
            dt.Columns.Add("FechaPago", typeof(DateTime));
            dt.Columns.Add("Usuario", typeof(string));

            foreach (DataGridViewRow row in dgvLibroFinanciero.Rows)
            {
                if (row.IsNewRow) continue;

                DateTime fecha = DateTime.MinValue;
                DateTime.TryParse(row.Cells["Fecha"]?.Value?.ToString(), out fecha);

                string mov = row.Cells["TipoMovimiento"]?.Value?.ToString() ?? "";
                string desc = row.Cells["Descripcion"]?.Value?.ToString() ?? "";
                decimal ingreso = ParseMoneyCell(row.Cells["Ingreso"]?.Value);
                decimal egreso = ParseMoneyCell(row.Cells["Egreso"]?.Value);
                string metodo = row.Cells["MetodoPago"]?.Value?.ToString() ?? "";
                string categoria = row.Cells["Categoria"]?.Value?.ToString() ?? "";
                string estado = row.Cells["EsPagado"]?.Value?.ToString() ?? "";
                DateTime fechaPago = DateTime.MinValue;
                DateTime.TryParse(row.Cells["FechaPago"]?.Value?.ToString(), out fechaPago);
                string usuario = row.Cells["Usuario"]?.Value?.ToString() ?? "";

                dt.Rows.Add(
                    fecha == DateTime.MinValue ? (object)DBNull.Value : fecha,
                    mov, desc, ingreso, egreso, metodo, categoria, estado,
                    fechaPago == DateTime.MinValue ? (object)DBNull.Value : fechaPago,
                    usuario
                );
            }
            return dt;
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            RefrescarFormulario();
        }

        private void ConfigurarTotalesUI()
        {
            var totales = new[] { lblTotIngresos, lblTotProveedores, lblGastosNegocio, lblGastosCasa, lblUtilidad };
            foreach (var l in totales)
            {
                if (l == null) continue;
                l.AutoSize = false;
                l.TextAlign = ContentAlignment.MiddleRight;
                l.Font = new System.Drawing.Font("Segoe UI", 10.5f, System.Drawing.FontStyle.Bold);
                l.Padding = new Padding(0, 2, 8, 2);
                l.Height = 28;              // alto fijo agradable
                l.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom; // que estiren
            }
        }

        private void AjustarAnchoTotales()
        {
            var totales = new[] { lblTotIngresos, lblTotProveedores, lblGastosNegocio, lblGastosCasa, lblUtilidad };
            foreach (var l in totales)
            {
                if (l?.Parent == null) continue;
                l.Width = l.Parent.ClientSize.Width - 12; // margen derecho
            }
        }

        private void RefrescarFormulario()
        {
            // No toques configuraciones iniciales (solo datos / UI ligera)
            this.ActiveControl = null;

            // Si estabas editando algo, lo descartas
            _idEgresoEdit = 0;
            LimpiarCamposEdicion();

            // Recalcular datos y totales con filtros actuales
            CargarLibroFinanciero();
            dgvLibroFinanciero.ClearSelection();

            AjustarAnchoTotales();
        }

        private void LimpiarTodo()
        {

            _idEgresoEdit = 0;
            LimpiarCamposEdicion();



            cboVista.SelectedIndex = 0;                // "Flujo"
            cboFiltroTipo.SelectedIndex = 0;           // "Todos"
            cboFiltroMetodo.SelectedIndex = 0;         // "Todos"
            cboFiltroCategoria.SelectedIndex = 0;      // "Todos"
            cboFiltroPagado.SelectedIndex = 0;         // "Todos"
            txtFiltroDescripcion.Clear();

            // 3) Recarga datos con las fechas actuales que ya están seleccionadas
            CargarLibroFinanciero();
            dgvLibroFinanciero.ClearSelection();

            // 4) UI sin foco
            this.ActiveControl = null;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            LimpiarTodo();
        }

        private void dgvLibroFinanciero_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
