using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;                // Para SizeF, FontStyle, RectangleF
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;                    // Para HttpUtility.HtmlEncode
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;

// iTextSharp (PDF)
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

// ===== ALIASES para evitar ambigüedad entre System.Drawing.* e iTextSharp.text.* =====
using GdiImage = System.Drawing.Image;
using GdiFont = System.Drawing.Font;
using GdiRectangle = System.Drawing.Rectangle;
using GdiBrushes = System.Drawing.Brushes;
using PdfImage = iTextSharp.text.Image;
using PdfFont = iTextSharp.text.Font;


namespace CapaPresentacion
{
    public partial class frmProducto : Form
    {
        private Usuario _Usuario;
        private class InvItem
        {
            public string Nombre { get; set; }
            public decimal Stock { get; set; }
        }

        private List<InvItem> _itemsAImprimir = new List<InvItem>();
        private int _printIndex = 0;
     
 

        public frmProducto(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            dgvdata.SuspendLayout();
            dgvdata.ResumeLayout();
            this.DoubleBuffered = true;

        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            txtbusqueda.Select();
            dgvdata.DefaultCellStyle.SelectionBackColor = Color.Blue;
            dgvdata.DefaultCellStyle.SelectionForeColor = Color.White;

            

            // Agregar opciones al ComboBox de búsqueda
            List<string> columnasOrdenadas = new List<string>
    {
        "Nombre",
        "Codigo",
        "IdProducto",
        "Descripcion",
        "Categoria",
        "Stock",
        "PrecioVenta",
        "Precio2",
        "Precio3",
         "PrecioPuntos",
        "PrecioCompra"
    };

            foreach (string columna in columnasOrdenadas)
            {
                // Verificar si la columna existe en el DataGridView
                if (dgvdata.Columns.Contains(columna))
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna, Texto = dgvdata.Columns[columna].HeaderText });
                }
            }

            

            // Agregar opciones al ComboBox de búsqueda
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible)
                {
                    cbobusqueda.Items.Add(new OpcionCombo()
                    {
                        Valor = columna.Name,
                        Texto = columna.HeaderText
                    });
                }
            }


            cbobusqueda.Items.Insert(0, new OpcionCombo() { Valor = "Todo", Texto = "Buscar en Todo" });

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            // Agregar opciones al ComboBox de filtro
            CboFiltro.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Productos Filtrados" });
            CboFiltro.Items.Add(new OpcionCombo() { Valor = 2, Texto = "Mostrar Todos los Productos" });
            CboFiltro.Items.Add(new OpcionCombo() { Valor = 3, Texto = "Productos con Bajo Stock" }); // Nueva opción
            CboFiltro.DisplayMember = "Texto";
            CboFiltro.ValueMember = "Valor";
            CboFiltro.SelectedIndex = 0;

            // Cargar solo 30 productos inicialmente
            CargarProductos(0);

            // Configurar el evento de cambio de selección del ComboBox
            CboFiltro.SelectedIndexChanged += CboFiltro_SelectedIndexChanged;

            // Configurar el evento de búsqueda
            txtbusqueda.TextChanged += txtbusqueda_TextChanged;

            dgvdata.Columns["Nombre"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvdata.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvdata.Columns["Nombre"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }
            MostrarTotalProductosRegistrados();

            HabilitarDoubleBuffering(dgvdata);
            CalcularTotalesInventario();

            dgvdata.MultiSelect = false;
            dgvdata.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvdata.ClearSelection();
            dgvdata.CurrentCell = null;

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
           
        }

        private void HabilitarDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private void AccionInventario(string accion /* "print" | "pdf" */)
        {
            var r = MessageBox.Show(
                "¿Usar lo visible del GRID (Sí) o SELECCIONAR con el modal (No)?",
                "Fuente de datos",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (r == DialogResult.Cancel) return;

            List<InvItem> items = (r == DialogResult.Yes)
                ? ObtenerDesdeGridVisible()
                : SeleccionarConModal();

            if (items.Count == 0)
            {
                MessageBox.Show("No hay productos para procesar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (accion == "print")
                ImprimirTicketInventario(items);
            else
                ExportarInventarioPDF(items);
        }

        private int ObtenerIdProductoSeleccionado()
        {
            if (dgvdata.CurrentRow == null || dgvdata.CurrentRow.Index < 0)
                return 0;

            object val = dgvdata.CurrentRow.Cells["Id"].Value; // Columna Id del producto

            if (val == null)
                return 0;

            int id;
            return int.TryParse(val.ToString(), out id) ? id : 0;
        }

        private List<InvItem> ObtenerDesdeGridVisible()
        {
            var items = new List<InvItem>();

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.IsNewRow || !row.Visible) continue;

                string nombre = row.Cells["Nombre"]?.Value?.ToString() ?? "";
                string stockStr = row.Cells["Stock"]?.Value?.ToString() ?? "0";

                if (decimal.TryParse(stockStr, out decimal stock))
                {
                    items.Add(new InvItem { Nombre = nombre, Stock = stock });
                }
            }
            return items;
        }

        private List<InvItem> SeleccionarConModal()
        {
            var items = new List<InvItem>();
            var idsAgregados = new HashSet<int>();

            while (true)
            {
                using (var m = new CapaPresentacion.Modales.mdProducto())
                {
                    var dr = m.ShowDialog();
                    if (dr != DialogResult.OK || m._Producto == null)
                        break;

                    if (!idsAgregados.Contains(m._Producto.IdProducto))
                    {
                        items.Add(new InvItem
                        {
                            Nombre = m._Producto.Nombre ?? "",
                            Stock = m._Producto.Stock
                        });
                        idsAgregados.Add(m._Producto.IdProducto);
                    }

                    var seguir = MessageBox.Show("Producto agregado.\n¿Deseas agregar otro?",
                                                 "Seleccionar más",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);
                    if (seguir == DialogResult.No) break;
                }
            }
            return items;
        }

        // ======== Impresión térmica 80mm: Nombre (envuelve) + Stock (derecha) ========
        private void ImprimirTicketInventario(List<InvItem> items)
        {
            _itemsAImprimir = items ?? new List<InvItem>();
            _printIndex = 0;

            using (PrintDocument pd = new PrintDocument())
            {
                // Ajusta al nombre de tu impresora
                pd.PrinterSettings.PrinterName = @"\\192.168.0.9\POS-80 (copy 1)";

                // 80mm ≈ 315 px; alto 0 = auto
                pd.DefaultPageSettings.PaperSize = new PaperSize("Receipt80", 315, 0);
                pd.PrintPage += Pd_PrintPage_Inventario;

                if (!pd.PrinterSettings.IsValid)
                {
                    MessageBox.Show("No se encontró la impresora POS-80. Selecciónala manualmente.");
                    using (var dlg = new PrintDialog { Document = pd, UseEXDialog = true })
                    {
                        if (dlg.ShowDialog() != DialogResult.OK) return;
                    }
                }

                pd.Print();
            }
        }

        private void Pd_PrintPage_Inventario(object sender, PrintPageEventArgs e)
        {
            int pageWidth = 280;
            int marginLeft = 10;
            int y = 0;

            using (GdiFont fTitle = new GdiFont("MS Gothic", 11, FontStyle.Bold))
            using (GdiFont fBold = new GdiFont("MS Gothic", 9, FontStyle.Bold))
            using (GdiFont f = new GdiFont("MS Gothic", 9, FontStyle.Regular))
            {
                string titulo = "INFORME DE INVENTARIO";
                e.Graphics.DrawString(titulo, fTitle, GdiBrushes.Black,
                    (pageWidth - e.Graphics.MeasureString(titulo, fTitle).Width) / 2, y);
                y += 18;

                string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                e.Graphics.DrawString(fecha, f, GdiBrushes.Black,
                    (pageWidth - e.Graphics.MeasureString(fecha, f).Width) / 2, y);
                y += 14;

                e.Graphics.DrawString(new string('-', 72), f, GdiBrushes.Black, marginLeft, y);
                y += 10;

                int xNombre = marginLeft;
                int anchoNombre = 200;
                int xStock = marginLeft + anchoNombre + 10;
                int yMax = e.MarginBounds.Height > 0 ? e.MarginBounds.Height : 1000;

                while (_printIndex < _itemsAImprimir.Count)
                {
                    var it = _itemsAImprimir[_printIndex];

                    SizeF szNombre = e.Graphics.MeasureString(it.Nombre, f, anchoNombre);
                    if (y + szNombre.Height + 8 > yMax)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    var rectNombre = new RectangleF(xNombre, y, anchoNombre, szNombre.Height);
                    e.Graphics.DrawString(it.Nombre, f, GdiBrushes.Black, rectNombre);

                    string stockTxt = it.Stock.ToString("0.##");
                    float wStock = e.Graphics.MeasureString(stockTxt, f).Width;
                    e.Graphics.DrawString(stockTxt, fBold, GdiBrushes.Black, xStock + 60 - wStock, y);

                    y += (int)szNombre.Height + 6;
                    _printIndex++;
                }

                e.Graphics.DrawString(new string('-', 72), f, GdiBrushes.Black, marginLeft, y);
                e.HasMorePages = false;
            }
        }


        // ======== Exportar a PDF con plantilla HTML formal (a prueba de nombres largos) ========
        private void ExportarInventarioPDF(List<InvItem> items)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = $"Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                sfd.Filter = "PDF|*.pdf";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string empresa = "AMERICAN COLORS";
                    string subtitulo = "Informe de Inventario";
                    string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                    string html = GenerarHtmlInventario(items, empresa, subtitulo, fecha);

                    using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var doc = new Document(PageSize.A4, 36, 36, 36, 36))
                    {
                        var writer = PdfWriter.GetInstance(doc, fs);
                        doc.Open();

                        // 👉 Tu XMLWorker requiere STREAMS, no StringReader
                        byte[] htmlBytes = Encoding.UTF8.GetBytes(html);
                        using (var msHtml = new MemoryStream(htmlBytes))
                        using (var msCss = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty))) // css vacío
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(
                                writer,
                                doc,
                                msHtml,
                                msCss,
                                Encoding.UTF8,
                                new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS)
                            );
                        }

                        doc.Close();
                    }

                    MessageBox.Show("PDF generado correctamente.", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar PDF:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private string GenerarHtmlInventario(List<InvItem> items, string empresa, string subtitulo, string fecha)
        {
            string H(string s) => string.IsNullOrEmpty(s) ? "" : HttpUtility.HtmlEncode(s);

            var sb = new StringBuilder();
            sb.AppendLine(@"<!DOCTYPE html>
<html lang='es'>
<head>
<meta charset='utf-8' />
<title>Inventario</title>
<style>
    :root{ --brand:#0f172a; --accent:#1e293b; --muted:#6b7280; --line:#e5e7eb; --bg:#ffffff; --thead:#111827; --thead-text:#ffffff; --row-alt:#f8fafc; }
    *{ box-sizing:border-box }
    html,body{ margin:0; padding:0; background:var(--bg); font-family: Arial, Helvetica, sans-serif; }
    .wrap{ max-width:900px; margin:0 auto; padding:24px; }
    .header{ text-align:center; margin-bottom:16px; }
    .empresa{ font-size:20px; font-weight:700; color:var(--brand); letter-spacing:0.5px; }
    .subtitulo{ font-size:14px; color:var(--accent); margin-top:4px; }
    .fecha{ font-size:12px; color:var(--muted); margin-top:2px; }
    .line{ height:1px; background:var(--line); margin:16px 0; }

    table{ width:100%; border-collapse:separate; border-spacing:0; table-layout: fixed; border:1px solid var(--line);
           border-radius:8px; overflow:hidden; page-break-inside:auto; }
    thead{ background:var(--thead); color:var(--thead-text); }
    th, td{ padding:10px 12px; border-bottom:1px solid var(--line); vertical-align:top; font-size:12px; line-height:1.35; }
    th{ text-align:left; font-weight:700; font-size:12.5px; letter-spacing:0.2px; }
    tr:nth-child(even) td{ background:var(--row-alt); }
    tr:last-child td{ border-bottom:none; }

    .col-prod{ width:75%; word-wrap:break-word; overflow-wrap:break-word; hyphens:auto; white-space:normal; }
    .col-stock{ width:25%; text-align:right; white-space:nowrap; }

    tr { page-break-inside:avoid; }
    .foot{ margin-top:14px; text-align:center; color:var(--muted); font-size:11px; }
</style>
</head>
<body>
<div class='wrap'>
    <div class='header'>
        <div class='empresa'>").Append(H(empresa)).Append(@"</div>
        <div class='subtitulo'>").Append(H(subtitulo)).Append(@"</div>
        <div class='fecha'>").Append(H(fecha)).Append(@"</div>
    </div>
    <div class='line'></div>
    <table>
        <thead>
            <tr>
                <th class='col-prod'>Producto</th>
                <th class='col-stock'>Stock</th>
            </tr>
        </thead>
        <tbody>");

            foreach (var it in items)
            {
                sb.Append("<tr>");
                sb.Append("<td class='col-prod'>").Append(H(it.Nombre ?? "")).Append("</td>");
                sb.Append("<td class='col-stock'>").Append(it.Stock.ToString("0.##")).Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append(@"</tbody>
    </table>
    <div class='foot'>Documento generado automáticamente.</div>
</div>
</body>
</html>");

            return sb.ToString();
        }

        private string HtmlEncodeLocal(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&#39;");
        }



       





        private void CalcularTotalesInventario()
        {
            decimal totalVenta = 0;
            decimal totalCompra = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (!row.IsNewRow && row.Visible)
                {
                    if (decimal.TryParse(row.Cells["PrecioVenta"].Value?.ToString(), out decimal precioVenta) &&
                        decimal.TryParse(row.Cells["PrecioCompra"].Value?.ToString(), out decimal precioCompra) &&
                        decimal.TryParse(row.Cells["Stock"].Value?.ToString(), out decimal stock))
                    {
                        totalVenta += precioVenta * stock;
                        totalCompra += precioCompra * stock;
                    }
                }
            }

           
            txtValorInventarioCompra.Text = totalCompra.ToString("N2");
        }

        private void CargarProductos(int cantidad)
        {
            dgvdata.Rows.Clear(); // Limpiar el DataGridView

            List<Producto> lista = new CN_Producto().Listar(); // Obtener todos los productos

            // Cargar solo los primeros 'cantidad' productos
            foreach (Producto item in lista.Take(cantidad))
            {
                dgvdata.Rows.Add(new object[] {

            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.IdCategoria,
            item.oCategoria.Descripcion,
            item.Stock,
            item.PrecioVenta,
            item.Prima,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos,
            item.PrecioCompra,
            item.Estado == true ? 1 : 0,
            item.Estado == true ? "Activo" : "No activo"
        });
            }
            MostrarTotalProductosRegistrados();
            CalcularTotalesInventario();
        }

        private void CargarTodosLosProductos()
        {
            dgvdata.Rows.Clear(); // Limpiar el DataGridView

            List<Producto> lista = new CN_Producto().Listar(); // Obtener todos los productos

            // Cargar todos los productos
            foreach (Producto item in lista)
            {
                dgvdata.Rows.Add(new object[] {
         
            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.IdCategoria,
            item.oCategoria.Descripcion,
            item.Stock,
            item.PrecioVenta,
            item.Prima,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos,
            item.PrecioCompra,
            item.Estado == true ? 1 : 0,
            item.Estado == true ? "Activo" : "No activo"
        });
            }
            MostrarTotalProductosRegistrados();
            CalcularTotalesInventario();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            
        }

        



        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
           
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {

                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))

                        row.Visible = true;
                    else
                        row.Visible = false;
                }

            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";

            if (CboFiltro.SelectedItem != null && Convert.ToInt32(((OpcionCombo)CboFiltro.SelectedItem).Valor) == 1)
            {
                CargarProductos(0);
            }
            else
            {
                // Si está en Mostrar Todos, cargar todos los productos
                CargarTodosLosProductos();
            }

            txtbusqueda.Select();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();

                

                // Agrega columnas de salida (cabeceras legibles)
                dt.Columns.Add("Código", typeof(string));
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Descripción", typeof(string));
                dt.Columns.Add("Categoría", typeof(string));
                dt.Columns.Add("Stock", typeof(string));
                dt.Columns.Add("Precio Venta", typeof(string));
                dt.Columns.Add("Precio 2", typeof(string));
                dt.Columns.Add("Precio 3", typeof(string));
                dt.Columns.Add("Precio Puntos", typeof(string));
                dt.Columns.Add("Precio Compra", typeof(string));
                dt.Columns.Add("Estado", typeof(string));

                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (!row.IsNewRow && row.Visible)
                    {
                        dt.Rows.Add(new object[] {
            row.Cells["Codigo"].Value?.ToString() ?? "",
            row.Cells["Nombre"].Value?.ToString() ?? "",
            row.Cells["Descripcion"].Value?.ToString() ?? "",
            row.Cells["Categoria"].Value?.ToString() ?? "",
            row.Cells["Stock"].Value?.ToString() ?? "",
            row.Cells["PrecioVenta"].Value?.ToString() ?? "",
            row.Cells["Precio2"].Value?.ToString() ?? "",
            row.Cells["Precio3"].Value?.ToString() ?? "",
            row.Cells["PrecioPuntos"].Value?.ToString() ?? "",
            row.Cells["PrecioCompra"].Value?.ToString() ?? "",
            row.Cells["Estado"].Value?.ToString() ?? ""
        });
                    }
                }


                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = string.Format("ReporteProducto_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                saveFile.Filter = "Excel files | *.xlsx";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(saveFile.FileName);
                        MessageBox.Show("Reporte generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
        }

        private void dgvdata_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string colName = dgvdata.Columns[e.ColumnIndex].Name;

            // Formato de color por stock y categoría
            if (colName == "Stock")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal stock))
                {
                    int idCategoria = 0;

                    if (dgvdata.Rows[e.RowIndex].Cells["IdCategoria"].Value != null)
                        int.TryParse(dgvdata.Rows[e.RowIndex].Cells["IdCategoria"].Value.ToString(), out idCategoria);

                    bool marcarRojo = false;

                    // Categoría 22: rojo si hay 1 o menos
                    if (idCategoria == 22 && stock <= 1)
                        marcarRojo = true;

                    // Categoría 45: rojo si hay menos de 5
                    else if (idCategoria == 45 && stock < 5)
                        marcarRojo = true;

                    if (marcarRojo)
                    {
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        e.CellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                        e.CellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
                    }
                }
            }

            // Colorear fila seleccionada
            if (dgvdata.Rows[e.RowIndex].Selected)
            {
                e.CellStyle.BackColor = Color.Blue;
                e.CellStyle.ForeColor = Color.White;
            }

            // Formato L.#.## para columnas de precio
            if (colName == "PrecioVenta" || colName == "Precio2" || colName == "Precio3" || colName == "PrecioCompra" || colName == "Prima")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal precio))
                {
                    e.Value = "L." + precio.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                iconButton1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void FiltrarDatos()
        {
            if (cbobusqueda.SelectedItem == null) return;

            // 1) Guarda scroll y fila seleccionada
            int scrollIndex = dgvdata.FirstDisplayedScrollingRowIndex >= 0
                ? dgvdata.FirstDisplayedScrollingRowIndex
                : 0;
            int selectedIndex = dgvdata.CurrentRow != null
                ? dgvdata.CurrentRow.Index
                : -1;

            // 2) Si no hay texto, recarga según CboFiltro
            string texto = txtbusqueda.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                CboFiltro_SelectedIndexChanged(null, null);
                return;
            }

            // 3) Tokeniza en mayúsculas
            var tokens = texto
                .ToUpper()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // 4) Trae toda la lista
            var lista = new CN_Producto().Listar();

            // 5) Determina columna a filtrar
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            // 6) Aplica el filtro multitérmino
            var filtrados = lista.Where(item =>
            {
                var valores = new List<string>();

                // Si "Todo", mete todas las propiedades en valores
                if (columnaFiltro == "Todo")
                {
                    valores.Add(item.IdProducto.ToString().ToUpper());
                    valores.Add(item.Codigo.ToUpper());
                    valores.Add(item.Nombre.ToUpper());
                    valores.Add(item.Descripcion.ToUpper());
                    valores.Add(item.oCategoria.Descripcion.ToUpper());
                    valores.Add(item.Stock.ToString().ToUpper());
                    valores.Add(item.PrecioVenta.ToString().ToUpper());
                    valores.Add(item.Precio2.ToString().ToUpper());
                    valores.Add(item.Precio3.ToString().ToUpper());
                    valores.Add(item.PrecioPuntos.ToString().ToUpper());
                    valores.Add(item.PrecioCompra.ToString().ToUpper());
                }
                else
                {
                    // Solo la columna concreta
                    string val = "";
                    switch (columnaFiltro)
                    {
                        case "Nombre": val = item.Nombre; break;
                        case "Codigo": val = item.Codigo; break;
                        case "IdProducto": val = item.IdProducto.ToString(); break;
                        case "Descripcion": val = item.Descripcion; break;
                        case "Categoria": val = item.oCategoria.Descripcion; break;
                        case "Stock": val = item.Stock.ToString(); break;
                        case "PrecioVenta": val = item.PrecioVenta.ToString(); break;
                        case "Precio2": val = item.Precio2.ToString(); break;
                        case "Precio3": val = item.Precio3.ToString(); break;
                        case "PrecioPuntos": val = item.PrecioPuntos.ToString(); break;
                        case "PrecioCompra": val = item.PrecioCompra.ToString(); break;
                    }
                    valores.Add(val.ToUpper());
                }

                // Cada token debe aparecer en al menos uno de los valores
                return tokens.All(tok => valores.Any(v => v.Contains(tok)));
            }).ToList();

            // 7) Vuelca al grid
            dgvdata.Rows.Clear();
            foreach (var item in filtrados)
            {
                dgvdata.Rows.Add(new object[] {
                                      // columna ícono
            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.IdCategoria,
            item.oCategoria.Descripcion,
            item.Stock,
            item.PrecioVenta,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos,
            item.PrecioCompra,
            item.Estado ? 1 : 0,
            item.Estado ? "Activo" : "No activo"
        });
            }

            // 8) Restaura scroll y selección
            if (dgvdata.RowCount > 0)
            {
                try
                {
                    dgvdata.FirstDisplayedScrollingRowIndex =
                        Math.Min(scrollIndex, dgvdata.RowCount - 1);
                }
                catch { }

                if (selectedIndex >= 0 && selectedIndex < dgvdata.RowCount)
                {
                    var row = dgvdata.Rows[selectedIndex];
                    row.Selected = true;
                    dgvdata.CurrentCell = row.Cells
                        .Cast<DataGridViewCell>()
                        .First(c => c.Visible);
                }
            }
            CalcularTotalesInventario();
        }


        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            timerBuscar.Stop();
            timerBuscar.Start();
            
        }

        private void BtnActualizarStock_Click(object sender, EventArgs e)
        {
           
        }

        private void BtnHistorialStock_Click(object sender, EventArgs e)
        {
            int idProducto = ObtenerIdProductoSeleccionado();

            if (idProducto == 0)
            {
                MessageBox.Show("Seleccione un producto para ver el historial de stock.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (frmHistorialStock historialStock = new frmHistorialStock(idProducto))
            {
                historialStock.ShowDialog();
            }
        }

        private void txtactualizarprecio_Click(object sender, EventArgs e)
        {
            
        }

        private void btnrefrescar_Click(object sender, EventArgs e)
        {
            RefrescarFormulario();
        }

        private void RefrescarFormulario()
        {
            dgvdata.Rows.Clear();
            CargarDatos();
            FiltrarDatos();
        }

        private void CargarDatos()
        {
            // Obtener la lista de productos nuevamente
            List<Producto> lista = new CN_Producto().Listar();

            // Llenar el DataGridView con los datos
            foreach (Producto item in lista)
            {
                dgvdata.Rows.Add(new object[] {
           
            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.IdCategoria,
            item.oCategoria.Descripcion,
            item.Stock,
            item.PrecioVenta,
            item.Prima,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos,
            item.PrecioCompra,
            item.Estado == true ? 1 : 0,
            item.Estado == true ? "Activo" : "No activo"
        });
            }

           
            CalcularTotalesInventario();
        }

        

        private void iconButton2_Click(object sender, EventArgs e)
        {
            RefrescarFormulario();
        }

        private void btnHistorialVentas_Click(object sender, EventArgs e)
        {
            int idProducto = ObtenerIdProductoSeleccionado();

            if (idProducto == 0)
            {
                MessageBox.Show("Seleccione un producto para ver el historial de ventas.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Md_ProductoVentas modal = new Md_ProductoVentas(idProducto))
            {
                modal.ShowDialog();
            }
        }

        private void CargarProductosBajoStock()
        {
            dgvdata.Rows.Clear(); // Limpiar el DataGridView

            List<Producto> lista = new CN_Producto().Listar(); // Obtener todos los productos

            // Filtrar productos según condiciones especiales por categoría
            var filtrados = lista.Where(p =>
                (p.oCategoria.IdCategoria == 22 && p.Stock <= 1) || // Categoría 22: stock <= 1
                (p.oCategoria.IdCategoria == 45 && p.Stock < 5)      // Categoría 45: stock < 5
            ).ToList();

            // Agregar al DataGridView
            foreach (Producto item in filtrados)
            {
                dgvdata.Rows.Add(new object[] {
           
            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.IdCategoria,
            item.oCategoria.Descripcion,
            item.Stock,
            item.PrecioVenta,
            item.Prima,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos,
            item.PrecioCompra,
            item.Estado == true ? 1 : 0,
            item.Estado == true ? "Activo" : "No activo"
        });
            }

            MostrarTotalProductosRegistrados();
            CalcularTotalesInventario();
        }



        private void CboFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CboFiltro.SelectedItem != null)
            {
                // Convertir el valor a int
                int valorSeleccionado = Convert.ToInt32(((OpcionCombo)CboFiltro.SelectedItem).Valor);

                if (valorSeleccionado == 1) // Productos Filtrados
                {
                    CargarProductos(0); 
                }
                else if (valorSeleccionado == 2) // Mostrar Todos los Productos
                {
                    CargarTodosLosProductos(); // Cargar todos los productos
                }
                else if (valorSeleccionado == 3) // Productos con Bajo Stock
                {
                    CargarProductosBajoStock(); // Cargar productos con bajo stock
                }
            }
        }

        private void timerBuscar_Tick(object sender, EventArgs e)
        {
            timerBuscar.Stop();
            FiltrarDatos();
        }

        private void btnCosto_Click(object sender, EventArgs e)
        {
           
        }

        private void btnBackUp_Click(object sender, EventArgs e)
        {
            string nombreBD = "DB_INVERFA"; // Tu base de datos
            string carpetaDrive = @"C:\BackUp\"; 

            // Asegurar que la carpeta exista
            if (!Directory.Exists(carpetaDrive))
            {
                Directory.CreateDirectory(carpetaDrive);
            }

            // Eliminar todos los .bak antiguos
            try
            {
                foreach (var archivo in Directory.GetFiles(carpetaDrive, "*.bak"))
                {
                    File.Delete(archivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar backups anteriores:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string nombreArchivo = $"{nombreBD}.bak";
            string rutaCompleta = Path.Combine(carpetaDrive, nombreArchivo);


            string connectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;";

            string query = $"BACKUP DATABASE [{nombreBD}] TO DISK = N'{rutaCompleta}' WITH INIT, FORMAT";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Backup creado y enviado a Google Drive correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear el backup:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       

        private bool ImpresoraExiste(string nombre)
        {
            foreach (string impresora in PrinterSettings.InstalledPrinters)
            {
                if (impresora.Trim().Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }


        private PrintDocument GenerarPrintDocumentCodigoBarras(string codigo, string nombreProducto = "", int idCategoria = 0)
        {
            var barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;

            // Ajuste dinámico de altura:
            int barcodeHeight = 35;

            GdiImage imagenOriginal = barcode.Draw(codigo, barcodeHeight); // ← alias

            PrintDocument pd = new PrintDocument();
            string nombreImpresora = @"\\192.168.0.17\LabelPrinter 3xx 200DPI TSC";

            // Validar existencia de la impresora
            if (!ImpresoraExiste(nombreImpresora))
            {
                MessageBox.Show("La impresora no está disponible o el nombre es incorrecto:\n" + nombreImpresora,
                                "Error de impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            pd.PrinterSettings.PrinterName = nombreImpresora;

            PaperSize paperSize = new PaperSize("Etiqueta3x3", 300, 300);
            pd.DefaultPageSettings.PaperSize = paperSize;
            pd.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

            pd.PrintPage += (sender, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                int anchoTotal = paperSize.Width - pd.DefaultPageSettings.Margins.Left - pd.DefaultPageSettings.Margins.Right;
                int y = pd.DefaultPageSettings.Margins.Top + 5; // REDUCIDO para 3 etiquetas

                GdiFont fontTitulo = new GdiFont("Arial", 9, FontStyle.Bold);
                GdiFont fontCodigo = new GdiFont("Arial", 7, FontStyle.Regular);
                GdiFont fontProducto = new GdiFont("Arial", 5, FontStyle.Italic);

                string titulo = "VARIEDADES LOPSI";
                SizeF sizeTitulo = e.Graphics.MeasureString(titulo, fontTitulo);
                float sizeTituloWidthAjustado = sizeTitulo.Width - 5;

                GdiImage iconoPulgar = Properties.Resources.image_modified.ToBitmap(); // ← alias
                int iconoSize = 16;

                float totalWidth = sizeTituloWidthAjustado + iconoSize;
                float startX = (paperSize.Width - totalWidth) / 2;
                float iconOffsetY = (sizeTitulo.Height - iconoSize) / 2;

                SizeF sizeCodigo = e.Graphics.MeasureString(codigo, fontCodigo);
                SizeF sizeProducto = SizeF.Empty;
                if (!string.IsNullOrWhiteSpace(nombreProducto))
                    sizeProducto = e.Graphics.MeasureString(nombreProducto, fontProducto);

                int cantidadEtiquetas = 3;

                for (int etiqueta = 0; etiqueta < cantidadEtiquetas; etiqueta++)
                {
                    int initialY = y;

                    int paddingTop = 2;
                    int paddingBottom = 5;

                    y += paddingTop;

                    // Título + ícono
                    e.Graphics.DrawString(titulo, fontTitulo, GdiBrushes.Black, startX, y);
                    e.Graphics.DrawImage(iconoPulgar, startX + sizeTituloWidthAjustado + 0, y + iconOffsetY, iconoSize, iconoSize);

                    y += Math.Max((int)sizeTitulo.Height, iconoSize) + 3;

                    // Código de barras
                    e.Graphics.DrawImage(imagenOriginal, (paperSize.Width - imagenOriginal.Width) / 2, y);
                    y += imagenOriginal.Height + 3;

                    // Código de texto
                    e.Graphics.DrawString(codigo, fontCodigo, GdiBrushes.Black, (paperSize.Width - sizeCodigo.Width) / 2, y);
                    y += (int)sizeCodigo.Height + 3;

                    // Nombre producto
                    if (!string.IsNullOrWhiteSpace(nombreProducto))
                    {
                        e.Graphics.DrawString(nombreProducto, fontProducto, GdiBrushes.Black, (paperSize.Width - sizeProducto.Width) / 2, y);
                        y += (int)sizeProducto.Height + paddingBottom;
                    }
                    else
                    {
                        y += paddingBottom;
                    }

                    // Recuadro de la etiqueta (usa GdiRectangle)
                    int etiquetaWidth = paperSize.Width - 20; // márgenes
                    int etiquetaHeight = y - initialY;

                    int recuadroX = 10;
                    int recuadroY = initialY - 3;

                    GdiRectangle recuadro = new GdiRectangle(recuadroX, recuadroY, etiquetaWidth, etiquetaHeight);
                    e.Graphics.DrawRectangle(Pens.Black, recuadro);

                    y += 2;
                }
            };
            return pd;
        }




        private void VistaPreviaCodigoBarras(string codigo, string nombreProducto = "", int idCategoria = 0)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                MessageBox.Show("El código de barras no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PrintDocument pd = GenerarPrintDocumentCodigoBarras(codigo, nombreProducto, idCategoria);

            if (pd != null)
            {
                PrintPreviewDialog previewDialog = new PrintPreviewDialog
                {
                    Document = pd,
                    Width = 800,
                    Height = 600
                };

                previewDialog.ShowDialog();
            }
        }

        private void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Podés validar o buscar si ya existe un producto con ese código
                e.SuppressKeyPress = true;
            }
        }

        private void btnImprimirCodigo_Click(object sender, EventArgs e)
        {
            
        }
       
        

        private void iconButton3_Click(object sender, EventArgs e)
        {
            
        }

        private void TxtNuevoStock_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtprecio_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtPrecio2_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtPrecio3_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtcosto_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtPrecioPuntos_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void MostrarTotalProductosRegistrados()
        {
            List<Producto> lista = new CN_Producto().Listar(); // obtiene todos los productos registrados
            txtTotalProductosRegistrados.Text = lista.Count.ToString("N0"); // muestra el total de productos distintos
        }


        private void TxtNuevoStock_TextChanged(object sender, EventArgs e)
        {

        }

        private string PedirStockInicial()
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 300;
                prompt.Height = 150;
                prompt.Text = "Stock inicial";

                Label textLabel = new Label() { Left = 10, Top = 20, Text = "¿Cuánto stock hay?", AutoSize = true };
                TextBox inputBox = new TextBox() { Left = 10, Top = 50, Width = 250 };
                Button confirmation = new Button() { Text = "Aceptar", Left = 180, Width = 80, Top = 80, DialogResult = DialogResult.OK };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
            }
        }

        private void dgvdata_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dgvdata_SelectionChanged(object sender, EventArgs e)
        {
           
        }

       

        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvdata.Rows[e.RowIndex];

                // selecciona visualmente la fila
                dgvdata.ClearSelection();
                row.Selected = true;

                
            }
        }

        private void dgvdata_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string colName = dgvdata.Columns[e.ColumnIndex].Name;

            if (colName == "Stock")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal stock))
                {
                    int idCategoria = 0;
                    if (dgvdata.Rows[e.RowIndex].Cells["IdCategoria"].Value != null)
                        int.TryParse(dgvdata.Rows[e.RowIndex].Cells["IdCategoria"].Value.ToString(), out idCategoria);

                    bool marcarRojo = false;

                    // 🔴 Regla general: stock menor a 3
                    if (stock < 3)
                        marcarRojo = true;

                    // 🔴 Reglas especiales por categoría
                    if (idCategoria == 22 && stock <= 1)
                        marcarRojo = true;
                    else if (idCategoria == 45 && stock < 5)
                        marcarRojo = true;

                    if (marcarRojo)
                    {
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else
                    {
                        e.CellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor;
                        e.CellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor;
                    }
                }
            }

            // Colorear fila seleccionada
            if (dgvdata.Rows[e.RowIndex].Selected)
            {
                e.CellStyle.BackColor = Color.Blue;
                e.CellStyle.ForeColor = Color.White;
            }

            // Formato L.#.## para columnas de precio
            if (colName == "PrecioVenta" || colName == "Precio2" || colName == "Precio3" || colName == "PrecioCompra" || colName == "Prima")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal precio))
                {
                    e.Value = "L." + precio.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }

        private void btnHistorialCompras_Click(object sender, EventArgs e)
        {
            int idProducto = ObtenerIdProductoSeleccionado();

            if (idProducto == 0)
            {
                MessageBox.Show("Seleccione un producto para ver el historial de compras.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Md_ProductoCompras modal = new Md_ProductoCompras(idProducto))
            {
                modal.ShowDialog();
            }
        }

        private void btnExportarInventarioPDF_Click(object sender, EventArgs e)
        {
            AccionInventario("pdf");
        }

        private void btnImprimirInventario_Click(object sender, EventArgs e)
        {
            AccionInventario("print");
        }

        private void txtPrima_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;

            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            using (var m = new CapaPresentacion.Modales.md_ProductoNuevo(_Usuario, null))
            {
                var dr = m.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    RefrescarFormulario();
                }
            }
        }

        private void btnEditarProducto_Click(object sender, EventArgs e)
        {
            if (dgvdata.CurrentRow == null || dgvdata.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccione un producto primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = dgvdata.CurrentRow;

            decimal TryDec(object v)
            {
                decimal d;
                decimal.TryParse(v?.ToString() ?? "0", out d);
                return d;
            }

            int TryInt(object v)
            {
                int i;
                int.TryParse(v?.ToString() ?? "0", out i);
                return i;
            }

            Producto p = new Producto()
            {
                IdProducto = TryInt(row.Cells["Id"].Value),
                Codigo = row.Cells["Codigo"].Value?.ToString() ?? "",
                Nombre = row.Cells["Nombre"].Value?.ToString() ?? "",
                Descripcion = row.Cells["Descripcion"].Value?.ToString() ?? "",
                Stock = TryDec(row.Cells["Stock"].Value),
                PrecioVenta = TryDec(row.Cells["PrecioVenta"].Value),
                Precio2 = TryDec(row.Cells["Precio2"].Value),
                Precio3 = TryDec(row.Cells["Precio3"].Value),
                PrecioPuntos = TryInt(row.Cells["PrecioPuntos"].Value),
                PrecioCompra = TryDec(row.Cells["PrecioCompra"].Value),
                Prima = TryDec(row.Cells["Prima"].Value),
                oCategoria = new Categoria()
                {
                    IdCategoria = TryInt(row.Cells["IdCategoria"].Value),
                    Descripcion = row.Cells["Categoria"].Value?.ToString() ?? ""
                },
                Estado = TryInt(row.Cells["EstadoValor"].Value) == 1
            };

            using (var m = new CapaPresentacion.Modales.md_ProductoNuevo(_Usuario, p))
            {
                var dr = m.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    RefrescarFormulario();
                }
            }
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            if (dgvdata.CurrentRow == null || dgvdata.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccione un producto primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = dgvdata.CurrentRow;

            int idProducto = Convert.ToInt32(row.Cells["Id"].Value);
            string nombre = row.Cells["Nombre"].Value?.ToString() ?? "";

            var r = MessageBox.Show(
                "¿Desea eliminar el producto:\n\n" + nombre + " ?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            string mensaje = string.Empty;
            Producto obj = new Producto()
            {
                IdProducto = idProducto
            };

            bool respuesta = new CN_Producto().Eliminar(obj, out mensaje);

            if (respuesta)
            {
                // Quitar la fila del grid
                dgvdata.Rows.Remove(row);

               
                MostrarTotalProductosRegistrados();

                MessageBox.Show("Producto eliminado correctamente.", "Mensaje",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Si la BD no deja eliminar porque hay historial de stock,
                // el SP / capa de datos debe devolver el detalle en 'mensaje'
                MessageBox.Show(mensaje, "No se pudo eliminar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            if (dgvdata.CurrentRow == null || dgvdata.CurrentRow.Index < 0)
            {
                MessageBox.Show("Seleccione un producto primero.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow row = dgvdata.CurrentRow;

            decimal TryDec(object v)
            {
                decimal d;
                decimal.TryParse(v?.ToString() ?? "0", out d);
                return d;
            }

            int TryInt(object v)
            {
                int i;
                int.TryParse(v?.ToString() ?? "0", out i);
                return i;
            }

            Producto p = new Producto()
            {
                IdProducto = TryInt(row.Cells["Id"].Value),
                Codigo = row.Cells["Codigo"].Value?.ToString() ?? "",
                Nombre = row.Cells["Nombre"].Value?.ToString() ?? "",
                Descripcion = row.Cells["Descripcion"].Value?.ToString() ?? "",
                Stock = TryDec(row.Cells["Stock"].Value),
                PrecioVenta = TryDec(row.Cells["PrecioVenta"].Value),
                Precio2 = TryDec(row.Cells["Precio2"].Value),
                Precio3 = TryDec(row.Cells["Precio3"].Value),
                PrecioPuntos = TryInt(row.Cells["PrecioPuntos"].Value),
                PrecioCompra = TryDec(row.Cells["PrecioCompra"].Value),
                Prima = TryDec(row.Cells["Prima"].Value),
                oCategoria = new Categoria()
                {
                    IdCategoria = TryInt(row.Cells["IdCategoria"].Value),
                    Descripcion = row.Cells["Categoria"].Value?.ToString() ?? ""
                },
                Estado = TryInt(row.Cells["EstadoValor"].Value) == 1
            };

            using (var m = new CapaPresentacion.Modales.md_ProductoNuevo(_Usuario, p))
            {
                if (m.ShowDialog() == DialogResult.OK)
                {
                    // Vuelves a traer todo desde BD para reflejar cambios
                    RefrescarFormulario();
                }
            }
        }
    }
}