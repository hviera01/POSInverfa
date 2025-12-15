using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Printing;

using CapaNegocio;
using CapaEntidad;

// iTextSharp
using iTextSharp.text;
using iTextSharp.text.pdf;

// ===== ALIAS PARA EVITAR CHOQUE DE TIPOS =====
using PdfFont = iTextSharp.text.Font;
using GfxFont = System.Drawing.Font;
using GfxBrushes = System.Drawing.Brushes;
using GfxColor = System.Drawing.Color;

namespace CapaPresentacion.Modales
{
    public class FrmPlanCuotasPreview : Form
    {
        private readonly int _idVenta;
        private readonly string _numeroDocumento;
        private readonly string _nombreCliente;
        private readonly decimal _montoTotal;
        private readonly CultureInfo hn = new CultureInfo("es-HN");

        private Label lblTitulo;
        private Label lblInfoCliente;
        private DataGridView dgvCuotas;
        private Label lblKPIs;
        private Button btnExportPdf;
        private Button btnImprimir;
        private Button btnCerrar;

        public FrmPlanCuotasPreview(int idVenta, string numeroDocumento, string nombreCliente, decimal montoTotal)
        {
            _idVenta = idVenta;
            _numeroDocumento = numeroDocumento;
            _nombreCliente = nombreCliente;
            _montoTotal = montoTotal;

            Text = "Vista previa — Plan de cuotas";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(900, 600);

            BuildUI();
            Load += FrmPlanCuotasPreview_Load;
        }

        private void BuildUI()
        {
            lblTitulo = new Label
            {
                Text = "Plan de cuotas (Vista previa)",
                Font = new GfxFont("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 12)
            };

            lblInfoCliente = new Label
            {
                Text = "—",
                Font = new GfxFont("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(18, 44)
            };

            dgvCuotas = new DataGridView
            {
                Name = "dgvCuotas",
                Location = new Point(16, 80),
                Size = new Size(840, 380),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            ConfigurarGrid();

            lblKPIs = new Label
            {
                Text = "—",
                Font = new GfxFont("Segoe UI", 9, FontStyle.Bold),
                ForeColor = GfxColor.FromArgb(22, 101, 52),
                AutoSize = true,
                Location = new Point(16, 468),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };

            btnExportPdf = new Button
            {
                Text = "Exportar PDF",
                BackColor = GfxColor.FromArgb(37, 99, 235),
                ForeColor = GfxColor.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 32),
                Location = new Point(16, 505),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnExportPdf.Click += BtnExportPdf_Click;

            btnImprimir = new Button
            {
                Text = "Imprimir",
                BackColor = GfxColor.FromArgb(15, 118, 110),
                ForeColor = GfxColor.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 32),
                Location = new Point(146, 505),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnImprimir.Click += BtnImprimir_Click;

            btnCerrar = new Button
            {
                Text = "Cerrar",
                Size = new Size(90, 32),
                Location = new Point(766, 505),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnCerrar.Click += (s, e) => Close();

            Controls.Add(lblTitulo);
            Controls.Add(lblInfoCliente);
            Controls.Add(dgvCuotas);
            Controls.Add(lblKPIs);
            Controls.Add(btnExportPdf);
            Controls.Add(btnImprimir);
            Controls.Add(btnCerrar);
        }

        private void ConfigurarGrid()
        {
            var g = dgvCuotas;
            g.AutoGenerateColumns = false;
            g.Columns.Clear();

            g.Columns.Add("IdCuota", "IdCuota"); g.Columns["IdCuota"].Visible = false;

            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "NumeroCuota", HeaderText = "Cuota", FillWeight = 60 });
            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaPago", HeaderText = "Fecha pago", FillWeight = 110 });
            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "MontoCuota", HeaderText = "Monto cuota", FillWeight = 120 });
            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "MontoPagado", HeaderText = "Pagado", FillWeight = 120 });
            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "SaldoCuota", HeaderText = "Saldo", FillWeight = 120 });
            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 80 });
        }

        private string Lps(decimal v) => v.ToString("'L.'#,0.00", hn);

        private void FrmPlanCuotasPreview_Load(object sender, EventArgs e)
        {
            lblInfoCliente.Text = $"Factura: {_numeroDocumento}   |   Cliente: {_nombreCliente}   |   Total: {Lps(_montoTotal)}";

            var cuotas = new CN_CuotaCredito()
                            .ObtenerCuotasPorVenta(_idVenta)
                            .OrderBy(c => c.NumeroCuota)
                            .ToList();

            dgvCuotas.Rows.Clear();
            decimal sumMonto = 0, sumPagado = 0, sumSaldo = 0;

            foreach (var c in cuotas)
            {
                var saldo = c.MontoCuota - c.MontoPagado;
                sumMonto += c.MontoCuota;
                sumPagado += c.MontoPagado;
                sumSaldo += saldo;

                dgvCuotas.Rows.Add(
                    c.IdCuota,
                    c.NumeroCuota,
                    c.FechaPago.ToString("dd/MM/yyyy"),
                    Lps(c.MontoCuota),
                    Lps(c.MontoPagado),
                    Lps(saldo),
                    c.Estado
                );
            }

            var resumen = new CN_CuotaCredito().ObtenerResumen(_idVenta);
            string proximo = resumen.ProximoPago.HasValue ? resumen.ProximoPago.Value.ToString("dd/MM/yyyy") : "---";
            string mora = resumen.EnMora ? "🔴 En mora" : "🟢 Al día";

            lblKPIs.Text =
                $"Cuotas: {resumen.NumeroCuotas}   |   Próximo pago: {proximo}   |   Estado: {mora}   |   " +
                $"Total cuotas: {Lps(sumMonto)}   |   Pagado: {Lps(sumPagado)}   |   Saldo: {Lps(sumSaldo)}";
        }

        private void BtnExportPdf_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"PlanCuotas_{_numeroDocumento}.pdf"
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                ExportarPdf(sfd.FileName);
                MessageBox.Show("PDF generado correctamente.", "Listo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportarPdf(string path)
        {
            var doc = new Document(PageSize.A4, 36, 36, 36, 36);
            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                PdfFont fTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14f);
                PdfFont fSub = FontFactory.GetFont(FontFactory.HELVETICA, 9f);
                PdfFont fHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9f);
                PdfFont fCell = FontFactory.GetFont(FontFactory.HELVETICA, 9f);

                var title = new Paragraph("Plan de cuotas — Vista previa", fTitle) { Alignment = Element.ALIGN_LEFT };
                doc.Add(title);

                doc.Add(new Paragraph($"Factura: {_numeroDocumento}", fSub));
                doc.Add(new Paragraph($"Cliente: {_nombreCliente}", fSub));
                doc.Add(new Paragraph($"Total: {Lps(_montoTotal)}", fSub));
                doc.Add(new Paragraph(" ", fSub));

                var table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 10f, 18f, 18f, 18f, 18f, 18f });

                AddHeader(table, "Cuota", fHeader);
                AddHeader(table, "Fecha pago", fHeader);
                AddHeader(table, "Monto cuota", fHeader);
                AddHeader(table, "Pagado", fHeader);
                AddHeader(table, "Saldo", fHeader);
                AddHeader(table, "Estado", fHeader);

                decimal sumMonto = 0, sumPagado = 0, sumSaldo = 0;

                foreach (DataGridViewRow r in dgvCuotas.Rows)
                {
                    if (r.IsNewRow) continue;

                    string n = Convert.ToString(r.Cells["NumeroCuota"].Value);
                    string fp = Convert.ToString(r.Cells["FechaPago"].Value);
                    string mc = Convert.ToString(r.Cells["MontoCuota"].Value);
                    string mp = Convert.ToString(r.Cells["MontoPagado"].Value);
                    string sc = Convert.ToString(r.Cells["SaldoCuota"].Value);
                    string es = Convert.ToString(r.Cells["Estado"].Value);

                    AddCell(table, n, fCell);
                    AddCell(table, fp, fCell);
                    AddCell(table, mc, fCell);
                    AddCell(table, mp, fCell);
                    AddCell(table, sc, fCell);
                    AddCell(table, es, fCell);

                    sumMonto += ParseLps(mc);
                    sumPagado += ParseLps(mp);
                    sumSaldo += ParseLps(sc);
                }

                doc.Add(table);
                doc.Add(new Paragraph(" ", fSub));

                var resume = new Paragraph(
                    $"Totales — Monto cuotas: {Lps(sumMonto)}   |   Pagado: {Lps(sumPagado)}   |   Saldo: {Lps(sumSaldo)}",
                    fHeader);
                doc.Add(resume);

                doc.Close();
            }
        }

        private static void AddHeader(PdfPTable t, string text, PdfFont f)
        {
            var cell = new PdfPCell(new Phrase(text, f))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = new BaseColor(234, 240, 255),
                Padding = 5f
            };
            t.AddCell(cell);
        }

        private static void AddCell(PdfPTable t, string text, PdfFont f)
        {
            var cell = new PdfPCell(new Phrase(text ?? "", f)) { Padding = 5f };
            t.AddCell(cell);
        }

        private decimal ParseLps(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0m;
            s = s.Trim();
            if (s.StartsWith("L.")) s = s.Substring(2).Trim();
            else if (s.StartsWith("L")) s = s.Substring(1).Trim();
            decimal v;
            if (decimal.TryParse(s, NumberStyles.Any, hn, out v)) return v;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out v)) return v;
            return 0m;
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            using (var pd = new PrintDialog())
            {
                var doc = new PrintDocument();
                doc.PrintPage += (s, ev) =>
                {
                    int y = 20;
                    var f1 = new GfxFont("Segoe UI", 12, FontStyle.Bold);
                    var f2 = new GfxFont("Segoe UI", 9);
                    var fH = new GfxFont("Segoe UI", 8, FontStyle.Bold);
                    var fR = new GfxFont("Segoe UI", 8);

                    ev.Graphics.DrawString("Plan de cuotas — Vista previa", f1, GfxBrushes.Black, 20, y); y += 26;
                    ev.Graphics.DrawString($"Factura: {_numeroDocumento}", f2, GfxBrushes.Black, 20, y); y += 16;
                    ev.Graphics.DrawString($"Cliente: {_nombreCliente}", f2, GfxBrushes.Black, 20, y); y += 16;
                    ev.Graphics.DrawString($"Total: {Lps(_montoTotal)}", f2, GfxBrushes.Black, 20, y); y += 24;

                    int x = 20;
                    ev.Graphics.DrawString("Cuota", fH, GfxBrushes.Black, x, y); x += 60;
                    ev.Graphics.DrawString("Fecha", fH, GfxBrushes.Black, x, y); x += 110;
                    ev.Graphics.DrawString("Monto", fH, GfxBrushes.Black, x, y); x += 110;
                    ev.Graphics.DrawString("Pagado", fH, GfxBrushes.Black, x, y); x += 110;
                    ev.Graphics.DrawString("Saldo", fH, GfxBrushes.Black, x, y); x += 110;
                    ev.Graphics.DrawString("Estado", fH, GfxBrushes.Black, x, y); y += 16;

                    foreach (DataGridViewRow r in dgvCuotas.Rows)
                    {
                        if (r.IsNewRow) continue;
                        x = 20;
                        ev.Graphics.DrawString(Convert.ToString(r.Cells["NumeroCuota"].Value), fR, GfxBrushes.Black, x, y); x += 60;
                        ev.Graphics.DrawString(Convert.ToString(r.Cells["FechaPago"].Value), fR, GfxBrushes.Black, x, y); x += 110;
                        ev.Graphics.DrawString(Convert.ToString(r.Cells["MontoCuota"].Value), fR, GfxBrushes.Black, x, y); x += 110;
                        ev.Graphics.DrawString(Convert.ToString(r.Cells["MontoPagado"].Value), fR, GfxBrushes.Black, x, y); x += 110;
                        ev.Graphics.DrawString(Convert.ToString(r.Cells["SaldoCuota"].Value), fR, GfxBrushes.Black, x, y); x += 110;
                        ev.Graphics.DrawString(Convert.ToString(r.Cells["Estado"].Value), fR, GfxBrushes.Black, x, y); y += 16;

                        if (y > ev.MarginBounds.Bottom - 40)
                        {
                            ev.HasMorePages = true;
                            return;
                        }
                    }
                };

                pd.Document = doc;
                if (pd.ShowDialog() == DialogResult.OK) doc.Print();
            }
        }
    }
}
