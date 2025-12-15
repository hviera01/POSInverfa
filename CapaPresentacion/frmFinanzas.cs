using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text;

namespace CapaPresentacion
{
    public partial class frmFinanzas : Form
    {
        private CN_Reporte cnReporte = new CN_Reporte();
        private string reporteActual = "UTILIDAD_BRUTA";
        private System.Drawing.Font fuenteOriginalLabels;
        private System.Drawing.Font fuenteOriginalValores;



        public frmFinanzas()
        {
            InitializeComponent();
            fuenteOriginalLabels = labelTotal1.Font;
            fuenteOriginalValores = labelValor1.Font;

        }

        private void frmFinanzas_Load(object sender, EventArgs e)
        {
            dtpInicio.Value = DateTime.Today;
            dtpFin.Value = DateTime.Today;
            cboMetodoPago.Items.AddRange(new string[] { "Todos", "Efectivo", "Tarjeta", "Transferencia" });
            cboMetodoPago.SelectedIndex = 0;
            CargarUsuarios();
            cboCondicion.Items.AddRange(new string[] { "Todos", "Contado", "Credito" });
            cboCondicion.SelectedIndex = 0;
            MostrarReporteActual();

            cboEstadoFactura.Items.Add("Todas");
            cboEstadoFactura.Items.Add("CANCELADA");
            cboEstadoFactura.Items.Add("PENDIENTE");
            cboEstadoFactura.SelectedIndex = 0;

            foreach (System.Windows.Forms.Control ctrl in this.Controls)

            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }
            HabilitarDoubleBuffering(dgvReporte);
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
        private void CargarUsuarios()
        {
            // Suponiendo que tienes un método que devuelve una lista de usuarios
            List<Usuario> usuarios = new CN_Usuario().Listar(); // Asegúrate de tener este método

            cboUsuario.Items.Add("Todos");
            foreach (var user in usuarios)
                cboUsuario.Items.Add(user.NombreCompleto);

            cboUsuario.SelectedIndex = 0;
        }

        private void MostrarReporteActual()
        {
            DateTime desde = dtpInicio.Value.Date;
            DateTime hasta = dtpFin.Value.Date.AddDays(1).AddTicks(-1);

            string metodoPago = cboMetodoPago.SelectedItem.ToString();
            string usuario = cboUsuario.SelectedItem.ToString();
            string condicion = cboCondicion.SelectedItem.ToString();
            string estadoFactura = cboEstadoFactura.SelectedItem?.ToString() ?? "Todas";



            dgvReporte.Columns.Clear();
            dgvReporte.Rows.Clear();

            // Ocultar y restaurar tamaño de fuente de los labels 4
            labelTotal4.Visible = false;
            labelValor4.Visible = false;
            labelTotal4.Font = fuenteOriginalLabels;
            labelValor4.Font = fuenteOriginalValores;

            labelTotal1.Font = fuenteOriginalLabels;
            labelTotal2.Font = fuenteOriginalLabels;
            labelTotal3.Font = fuenteOriginalLabels;
            labelValor1.Font = fuenteOriginalValores;
            labelValor2.Font = fuenteOriginalValores;
            labelValor3.Font = fuenteOriginalValores;

            switch (reporteActual)
            {
                case "UTILIDAD_BRUTA":
                    MostrarUtilidadBruta(desde, hasta, metodoPago, usuario, condicion, estadoFactura); // ← USA estadoFactura
                    break;
                case "UTILIDAD_NETA":
                    MostrarUtilidadNeta(desde, hasta);
                    break;
                case "GANANCIA_PRODUCTO":
                    MostrarGananciaPorProducto(desde, hasta);
                    break;
                case "CIERRE_CAJA":
                    MostrarReporteCierreCaja(desde, hasta);
                    break;
            }
        }



        private void MostrarGananciaPorProducto(DateTime desde, DateTime hasta)
        {
            var lista = cnReporte.ObtenerGananciaPorProducto(
                desde.ToString("yyyy-MM-dd HH:mm:ss"),
                hasta.ToString("yyyy-MM-dd HH:mm:ss"));

            dgvReporte.Columns.Clear();
            dgvReporte.Rows.Clear();

            dgvReporte.Columns.Add("Producto", "Nombre del Producto");
            dgvReporte.Columns.Add("Cantidad", "Cantidad Vendida");
            dgvReporte.Columns.Add("Costo", "Costo Total");
            dgvReporte.Columns.Add("Venta", "Ingresos Totales");
            dgvReporte.Columns.Add("Ganancia", "Utilidad");

            dgvReporte.Columns["Producto"].Width = 450;
            dgvReporte.Columns["Cantidad"].Width = 120;
            dgvReporte.Columns["Costo"].Width = 150;
            dgvReporte.Columns["Venta"].Width = 150;
            dgvReporte.Columns["Ganancia"].Width = 150;


            decimal totalVenta = 0, totalCosto = 0, totalGanancia = 0;

            foreach (var item in lista)
            {
                dgvReporte.Rows.Add(item.NombreProducto, item.CantidadVendida, item.CostoTotal, item.TotalVentas, item.Ganancia);

                totalVenta += Convert.ToDecimal(item.TotalVentas);
                totalCosto += Convert.ToDecimal(item.CostoTotal);
                totalGanancia += Convert.ToDecimal(item.Ganancia);
            }

            labelTitulo.Text = "Ganancia por Producto";
            labelTotal1.Text = "TOTAL VENTAS:";
            labelTotal2.Text = "TOTAL COSTOS:";
            labelTotal3.Text = "TOTAL GANANCIA:";

            labelValor1.Text = "L. " + totalVenta.ToString("N2");
            labelValor2.Text = "L. " + totalCosto.ToString("N2");
            labelValor3.Text = "L. " + totalGanancia.ToString("N2");

        }



        private void MostrarUtilidadBruta(DateTime inicio, DateTime fin, string metodo, string usuario, string condicion, string estadoFactura)
        {
            string fechainicio = inicio.ToString("yyyy-MM-dd HH:mm:ss");
            string fechafin = fin.ToString("yyyy-MM-dd HH:mm:ss");

            var lista = cnReporte.ObtenerUtilidadBruta(fechainicio, fechafin, metodo, usuario, condicion, estadoFactura);

            dgvReporte.Columns.Clear();
            dgvReporte.Rows.Clear();

            dgvReporte.Columns.Add("Fecha", "Fecha de la Venta");
            dgvReporte.Columns.Add("Numero", "N° Documento");
            dgvReporte.Columns.Add("Cliente", "Nombre del Cliente");
            dgvReporte.Columns.Add("Vendedor", "Usuario Responsable");
            dgvReporte.Columns.Add("MetodoPago", "Forma de Pago");
            dgvReporte.Columns.Add("TotalVenta", "Monto Total");
            dgvReporte.Columns.Add("CostoTotal", "Costo total");
            dgvReporte.Columns.Add("Utilidad", "Ganancia Bruta");
            dgvReporte.Columns.Add("EstadoFactura", "Estado de la Factura");

            // Anchos
            dgvReporte.Columns["Fecha"].Width = 120;
            dgvReporte.Columns["Numero"].Width = 150;
            dgvReporte.Columns["Cliente"].Width = 250;
            dgvReporte.Columns["Vendedor"].Width = 200;
            dgvReporte.Columns["MetodoPago"].Width = 120;
            dgvReporte.Columns["TotalVenta"].Width = 100;
            dgvReporte.Columns["CostoTotal"].Width = 100;
            dgvReporte.Columns["Utilidad"].Width = 100;
            dgvReporte.Columns["EstadoFactura"].Width = 110;

            decimal totalVenta = 0, totalCosto = 0, totalUtilidad = 0;

            foreach (var item in lista)
            {
                dgvReporte.Rows.Add(
                    item.Fecha,
                    item.Numero,
                    item.Cliente,
                    item.Vendedor,
                    item.MetodoPago,
                    item.TotalVenta.ToString("N2"),
                    item.CostoTotal.ToString("N2"),
                    item.UtilidadBruta.ToString("N2"),
                    item.EstadoFactura
                );

                totalVenta += item.TotalVenta;
                totalCosto += item.CostoTotal;
                totalUtilidad += item.UtilidadBruta;
            }

            labelTitulo.Text = "Reporte de Utilidad Bruta";
            labelTotal1.Text = "TOTAL VENTA:";
            labelTotal2.Text = "TOTAL COSTO:";
            labelTotal3.Text = "TOTAL UTILIDAD:";
            labelValor1.Text = "L. " + totalVenta.ToString("N2");
            labelValor2.Text = "L. " + totalCosto.ToString("N2");
            labelValor3.Text = "L. " + totalUtilidad.ToString("N2");
        }


        private void MostrarUtilidadNeta(DateTime inicio, DateTime fin)
        {
            string fechainicio = inicio.ToString("yyyy-MM-dd HH:mm:ss");
            string fechafin = fin.ToString("yyyy-MM-dd HH:mm:ss");

            var lista = cnReporte.ObtenerUtilidadNeta(fechainicio, fechafin);

            dgvReporte.Columns.Clear();
            dgvReporte.Rows.Clear();

            dgvReporte.Columns.Add("TotalVentas", "Total Ingresos");
            dgvReporte.Columns.Add("TotalCostos", "Total Costos");
            dgvReporte.Columns.Add("TotalEgresos", "Total Egresos");
            dgvReporte.Columns.Add("UtilidadNeta", "Utilidad Neta");

            dgvReporte.Columns["TotalVentas"].Width = 200;
            dgvReporte.Columns["TotalCostos"].Width = 200;
            dgvReporte.Columns["TotalEgresos"].Width = 200;
            dgvReporte.Columns["UtilidadNeta"].Width = 200;

            foreach (var item in lista)
            {
                dgvReporte.Rows.Add(
                    item.TotalVentas.ToString("N2"),
                    item.TotalCostos.ToString("N2"),
                    item.TotalEgresos.ToString("N2"),
                    item.UtilidadNeta.ToString("N2")
                );
            }

            labelTitulo.Text = "Reporte de Utilidad Neta por flujo de efectivo";
            labelTotal1.Text = "TOTAL INGRESOS:";
            labelTotal2.Text = "TOTAL COSTOS:";
            labelTotal3.Text = "TOTAL EGRESOS:";
            labelTotal4.Text = "UTILIDAD NETA:";

            if (lista.Any())
            {
                labelValor1.Text = "L. " + lista[0].TotalVentas.ToString("N2");
                labelValor2.Text = "L. " + lista[0].TotalCostos.ToString("N2");
                labelValor3.Text = "L. " + lista[0].TotalEgresos.ToString("N2");
                labelValor4.Text = "L. " + lista[0].UtilidadNeta.ToString("N2");
            }
            labelTotal4.Visible = true;
            labelValor4.Visible = true;

            // Cambiar el tamaño de fuente solo en este caso
            labelValor1.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelValor2.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelValor3.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelTotal1.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelTotal2.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelTotal3.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelTotal4.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
            labelValor4.Font = new System.Drawing.Font(labelTotal4.Font.FontFamily, 10, FontStyle.Bold);
        }



        private void btnUtilidadBruta_Click(object sender, EventArgs e)
        {
            
            reporteActual = "UTILIDAD_BRUTA";
            MostrarReporteActual();
        }

        private void btnUtilidadNeta_Click(object sender, EventArgs e)
        {
            reporteActual = "UTILIDAD_NETA";
            MostrarReporteActual();
        }

        private void btnGananciaProducto_Click(object sender, EventArgs e)
        {
            reporteActual = "GANANCIA_PRODUCTO";
            MostrarReporteActual();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            MostrarReporteActual();
        }

        private void MostrarReporteCierreCaja(DateTime desde, DateTime hasta)
        {
            CN_Reporte reporte = new CN_Reporte();
            var lista = reporte.ObtenerReporteCierresCaja(desde.ToString("yyyy-MM-dd"), hasta.AddDays(1).ToString("yyyy-MM-dd"));

            dgvReporte.Columns.Clear();
            dgvReporte.Rows.Clear();

            dgvReporte.Columns.Add("FechaInicio", "Inicio");
            dgvReporte.Columns.Add("FechaFin", "Cierre");
            dgvReporte.Columns.Add("Usuario", "Usuario");
            dgvReporte.Columns.Add("MontoInicial", "Monto Inicial Efectivo");
            dgvReporte.Columns.Add("IngEfectivo", "Ingresos Efectivo");
            dgvReporte.Columns.Add("IngTarjeta", "Ingresos Tarjeta");
            dgvReporte.Columns.Add("IngTransferencia", "Ingresos Transferencia");
            dgvReporte.Columns.Add("EgrEfectivo", "Egresos Efectivo");
            dgvReporte.Columns.Add("EgrTransferencia", "Egresos Transferencia");
            dgvReporte.Columns.Add("TotalCalculado", "Total Calculado Efectivo");
            dgvReporte.Columns.Add("TotalTransferencia", "Total Transferencia");
            dgvReporte.Columns.Add("GranTotal", "Gran Total");
            dgvReporte.Columns.Add("TotalReal", "Total Real Efectivo");
            dgvReporte.Columns.Add("Diferencia", "Diferencia");
            dgvReporte.Columns.Add("Observaciones", "Observaciones");

            dgvReporte.Columns["FechaInicio"].Width = 120;
            dgvReporte.Columns["FechaFin"].Width = 120;
            dgvReporte.Columns["Usuario"].Width = 130;
            dgvReporte.Columns["MontoInicial"].Width = 90;
            dgvReporte.Columns["IngEfectivo"].Width = 90;
            dgvReporte.Columns["IngTarjeta"].Width = 90;
            dgvReporte.Columns["IngTransferencia"].Width = 110;
            dgvReporte.Columns["EgrEfectivo"].Width = 90;
            dgvReporte.Columns["EgrTransferencia"].Width = 110;
            dgvReporte.Columns["TotalCalculado"].Width = 110;
            dgvReporte.Columns["TotalTransferencia"].Width = 110;
            dgvReporte.Columns["GranTotal"].Width = 90;
            dgvReporte.Columns["TotalReal"].Width = 90;
            dgvReporte.Columns["Diferencia"].Width = 90;
            dgvReporte.Columns["Observaciones"].Width = 200;

            foreach (var item in lista)
            {
                dgvReporte.Rows.Add(
        item.FechaInicio,
        item.FechaFin,
        item.UsuarioResponsable,
        item.MontoInicial,
        item.IngresosEfectivo,
        item.IngresosTarjeta,
        item.IngresosTransferencia,
        item.Egresos,
        item.EgresosTransferencia,
        item.TotalCalculado,
        item.TotalTransferencia,
        item.GranTotal,
        item.TotalReal,
        item.Diferencia,
        item.Observaciones
    );
                labelTitulo.Text = "REPORTE COMPLETO DE CIERRES DE CAJA";
                labelTotal1.Text = "Inicio";
                labelTotal2.Text = "Cierre";
                labelTotal3.Text = "Usuario Responsable";
            }
        }

        private void btnCierreCaja_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpInicio.Value.Date;
            DateTime hasta = dtpFin.Value.Date;

            reporteActual = "CIERRE_CAJA"; // <<<<< ESTA LÍNEA ES CLAVE

            MostrarReporteCierreCaja(desde, hasta);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            switch (reporteActual)
            {
                case "UTILIDAD_BRUTA":
                    ExportarUtilidadBrutaPDF();
                    break;
                case "UTILIDAD_NETA":
                    ExportarUtilidadNetaPDF();
                    break;
                case "GANANCIA_PRODUCTO":
                    ExportarGananciaProductoPDF();
                    break;
                case "CIERRE_CAJA":
                    ExportarCierreCajaFiltradoPDF();
                    break;
            }
        }

        private void ExportarUtilidadBrutaPDF()
        {
            string plantilla = Properties.Resources.PlantillaUtilidadBruta; // HTML guardado como recurso
            StringBuilder filas = new StringBuilder();

            foreach (DataGridViewRow row in dgvReporte.Rows)
            {
                filas.AppendLine("<tr>");
                for (int i = 0; i < dgvReporte.Columns.Count; i++)
                {
                    filas.AppendLine($"<td>{row.Cells[i].Value?.ToString()}</td>");
                }
                filas.AppendLine("</tr>");
            }

            string htmlFinal = plantilla
                .Replace("@fechainicio", dtpInicio.Value.ToString("dd/MM/yyyy"))
                .Replace("@fechafin", dtpFin.Value.ToString("dd/MM/yyyy"))
                .Replace("@tabla", filas.ToString())
                .Replace("@totalventa", labelValor1.Text)
                .Replace("@totalcosto", labelValor2.Text)
                .Replace("@totalutilidad", labelValor3.Text);

            GuardarPDF("UtilidadBruta", htmlFinal);
        }

        private void GuardarPDF(string nombreBase, string contenidoHTML)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = nombreBase + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
            save.Filter = "Archivo PDF|*.pdf";

            if (save.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(save.FileName, FileMode.Create))
                using (Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25))
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    using (StringReader sr = new StringReader(contenidoHTML))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    pdfDoc.Close();
                }

                MessageBox.Show("PDF generado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnInformeIndividual_Click(object sender, EventArgs e)
        {
            if (dgvReporte.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná un cierre de caja.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvReporte.SelectedRows[0];

            CierreCaja cierre = new CierreCaja
            {
                FechaInicio = DateTime.Parse(fila.Cells["FechaInicio"].Value.ToString()),
                FechaFin = DateTime.Parse(fila.Cells["FechaFin"].Value.ToString()),
                MontoInicial = Convert.ToDecimal(fila.Cells["MontoInicial"].Value),
                GranTotal = Convert.ToDecimal(fila.Cells["GranTotal"].Value),
                TotalReal = Convert.ToDecimal(fila.Cells["TotalReal"].Value),
                Diferencia = Convert.ToDecimal(fila.Cells["Diferencia"].Value),
                UsuarioResponsable = fila.Cells["Usuario"].Value.ToString(),
                // Otros campos como ingresos/egresos deben consultarse si los querés, o dejarlos en 0 si son opcionales
            };

            GenerarReporteCierreCajaPDF(cierre);
        }

        private void ExportarUtilidadNetaPDF()
        {
            string plantilla = Properties.Resources.PlantillaUtilidadNeta;

            string htmlFinal = plantilla
                .Replace("@fechainicio", dtpInicio.Value.ToString("dd/MM/yyyy"))
                .Replace("@fechafin", dtpFin.Value.ToString("dd/MM/yyyy"))
                .Replace("@totalventas", labelValor1.Text)   // TOTAL VENTAS
                .Replace("@totalcostos", labelValor2.Text)   // TOTAL COSTOS
                .Replace("@totalegresos", labelValor3.Text)  // TOTAL EGRESOS
                .Replace("@utilidadneta", labelValor4.Text); // UTILIDAD NETA

            GuardarPDF("UtilidadNeta", htmlFinal);
        }


        private void ExportarGananciaProductoPDF()
        {
            string plantilla = Properties.Resources.Plantilla_GananciaPorducto;
            StringBuilder filas = new StringBuilder();

            foreach (DataGridViewRow row in dgvReporte.Rows)
            {
                filas.AppendLine("<tr>");
                for (int i = 0; i < dgvReporte.Columns.Count; i++)
                {
                    filas.AppendLine($"<td>{row.Cells[i].Value?.ToString()}</td>");
                }
                filas.AppendLine("</tr>");
            }

            string htmlFinal = plantilla
                .Replace("@fechainicio", dtpInicio.Value.ToString("dd/MM/yyyy"))
                .Replace("@fechafin", dtpFin.Value.ToString("dd/MM/yyyy"))
                .Replace("@tabla", filas.ToString())
                .Replace("@totalventa", labelValor1.Text)
                .Replace("@totalcosto", labelValor2.Text)
                .Replace("@totalganancia", labelValor3.Text);

            GuardarPDF("GananciaPorProducto", htmlFinal);
        }

        private void ExportarCierreCajaFiltradoPDF()
        {
            string plantilla = Properties.Resources.Plantilla_CierreCajaFiltrado;
            StringBuilder filas = new StringBuilder();

            foreach (DataGridViewRow row in dgvReporte.Rows)
            {
                // Solo estas columnas, en el mismo orden de la plantilla
                string inicio = row.Cells["FechaInicio"].Value?.ToString();
                string cierre = row.Cells["FechaFin"].Value?.ToString();
                string usuario = row.Cells["Usuario"].Value?.ToString();
                string montoini = row.Cells["MontoInicial"].Value?.ToString();
                string grantotal = row.Cells["GranTotal"].Value?.ToString();
                string totalreal = row.Cells["TotalReal"].Value?.ToString();
                string diferencia = row.Cells["Diferencia"].Value?.ToString();

                // Formatear valores numéricos
                string Formatear(string valor)
                {
                    return decimal.TryParse(valor, out decimal num) ? "L. " + num.ToString("N2") : valor;
                }

                filas.AppendLine("<tr>");
                filas.AppendLine($"<td>{inicio}</td>");
                filas.AppendLine($"<td>{cierre}</td>");
                filas.AppendLine($"<td>{usuario}</td>");
                filas.AppendLine($"<td>{Formatear(montoini)}</td>");
                filas.AppendLine($"<td>{Formatear(grantotal)}</td>");
                filas.AppendLine($"<td>{Formatear(totalreal)}</td>");
                filas.AppendLine($"<td>{Formatear(diferencia)}</td>");
                filas.AppendLine("</tr>");
            }

            string htmlFinal = plantilla
                .Replace("@fechainicio", dtpInicio.Value.ToString("dd/MM/yyyy"))
                .Replace("@fechafin", dtpFin.Value.ToString("dd/MM/yyyy"))
                .Replace("@tabla", filas.ToString());

            GuardarPDF("CierresCajaFiltrado", htmlFinal);
        }


        private void ExportarInformeFinancieroCompletoPDF()
        {
            var utilidadBruta = cnReporte.ObtenerUtilidadBruta(
     dtpInicio.Value.ToString("yyyy-MM-dd HH:mm:ss"),
     dtpFin.Value.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss"),
     cboMetodoPago.SelectedItem.ToString(),
     cboUsuario.SelectedItem.ToString(),
     cboCondicion.SelectedItem.ToString(),
     cboEstadoFactura.SelectedItem.ToString()  // ← ESTE FALTABA
 );

            var utilidadNeta = cnReporte.ObtenerUtilidadNeta(
                dtpInicio.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                dtpFin.Value.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss"));

            var gananciaProducto = cnReporte.ObtenerGananciaPorProducto(
                dtpInicio.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                dtpFin.Value.AddDays(1).AddTicks(-1).ToString("yyyy-MM-dd HH:mm:ss"));

            var cierres = cnReporte.ObtenerReporteCierresCaja(
                dtpInicio.Value.ToString("yyyy-MM-dd"),
                dtpFin.Value.AddDays(1).ToString("yyyy-MM-dd"));

            // === UTILIDAD BRUTA ===
            StringBuilder tablaBruta = new StringBuilder();
            decimal totalVentaBruta = 0, totalCostoBruta = 0, totalUtilidadBruta = 0;

            foreach (var item in utilidadBruta)
            {
                tablaBruta.AppendLine($"<tr><td>{item.Fecha}</td><td>{item.Numero}</td><td>{item.Cliente}</td><td>{item.Vendedor}</td><td>{item.MetodoPago}</td><td>{item.TotalVenta:N2}</td><td>{item.CostoTotal:N2}</td><td>{item.UtilidadBruta:N2}</td></tr>");
                totalVentaBruta += item.TotalVenta;
                totalCostoBruta += item.CostoTotal;
                totalUtilidadBruta += item.UtilidadBruta;
            }

            // === GANANCIA POR PRODUCTO ===
            StringBuilder tablaProducto = new StringBuilder();
            decimal totalVentaProd = 0, totalCostoProd = 0, totalGananciaProd = 0;

            foreach (var item in gananciaProducto)
            {
                tablaProducto.AppendLine($"<tr><td>{item.NombreProducto}</td><td>{item.CantidadVendida}</td><td>{item.CostoTotal}</td><td>{item.TotalVentas}</td><td>{item.Ganancia}</td></tr>");
                totalVentaProd += Convert.ToDecimal(item.TotalVentas);
                totalCostoProd += Convert.ToDecimal(item.CostoTotal);
                totalGananciaProd += Convert.ToDecimal(item.Ganancia);
            }

            // === CIERRES DE CAJA ===
            StringBuilder tablaCierre = new StringBuilder();
            foreach (var item in cierres)
            {
                tablaCierre.AppendLine($"<tr><td>{item.FechaInicio}</td><td>{item.FechaFin}</td><td>{item.UsuarioResponsable}</td><td>{item.MontoInicial}</td><td>{item.GranTotal}</td><td>{item.TotalReal}</td><td>{item.Diferencia}</td></tr>");
            }

            // === UTILIDAD NETA ===
            var datosNeta = utilidadNeta.FirstOrDefault();

            string plantilla = Properties.Resources.Plantilla_InformeCompleto;
            string htmlFinal = plantilla
                .Replace("@fechainicio", dtpInicio.Value.ToString("dd/MM/yyyy"))
                .Replace("@fechafin", dtpFin.Value.ToString("dd/MM/yyyy"))
                .Replace("@tabla_bruta", tablaBruta.ToString())
                .Replace("@totalventa_bruta", totalVentaBruta.ToString("N2"))
                .Replace("@totalcosto_bruta", totalCostoBruta.ToString("N2"))
                .Replace("@totalutilidad_bruta", totalUtilidadBruta.ToString("N2"))
                .Replace("@totalventa_neta", datosNeta?.TotalVentas.ToString("N2") ?? "0.00")
                .Replace("@totalcostos_neta", datosNeta?.TotalCostos.ToString("N2") ?? "0.00") // NUEVA LÍNEA AÑADIDA
                .Replace("@totalegresos_neta", datosNeta?.TotalEgresos.ToString("N2") ?? "0.00")
                .Replace("@utilidadneta", datosNeta?.UtilidadNeta.ToString("N2") ?? "0.00")
                .Replace("@tabla_producto", tablaProducto.ToString())
                .Replace("@totalventa_producto", totalVentaProd.ToString("N2"))
                .Replace("@totalcosto_producto", totalCostoProd.ToString("N2"))
                .Replace("@totalganancia_producto", totalGananciaProd.ToString("N2"))
                .Replace("@tabla_cierre", tablaCierre.ToString());

            GuardarPDF("InformeFinancieroCompleto", htmlFinal);
        }


        private void GenerarReporteCierreCajaPDF(CierreCaja cierre)
        {
            string plantilla = Properties.Resources.Plantilla_CierreCaja;

            string htmlFinal = plantilla
                .Replace("@usuario", cierre.UsuarioResponsable)
                .Replace("@fechainicio", cierre.FechaInicio.ToString("dd/MM/yyyy HH:mm"))
                .Replace("@fechafin", cierre.FechaFin.ToString("dd/MM/yyyy HH:mm"))
                .Replace("@montoinicial", cierre.MontoInicial.ToString("0.00"))
                .Replace("@ingefectivo", cierre.IngresosEfectivo.ToString("0.00"))
                .Replace("@ingtarjeta", cierre.IngresosTarjeta.ToString("0.00"))
                .Replace("@ingtransferencia", cierre.IngresosTransferencia.ToString("0.00"))
                .Replace("@egr_efectivo", cierre.Egresos.ToString("0.00"))
                .Replace("@egr_transferencia", cierre.EgresosTransferencia.ToString("0.00"))
                .Replace("@total_efectivo", cierre.TotalCalculado.ToString("0.00"))
                .Replace("@total_transferencia", cierre.TotalTransferencia.ToString("0.00"))
                .Replace("@grantotal", cierre.GranTotal.ToString("0.00"))
                .Replace("@totalreal", cierre.TotalReal.ToString("0.00"))
                .Replace("@diferencia", cierre.Diferencia.ToString("0.00"))
                .Replace("@observaciones", cierre.Observaciones);

            GuardarPDF("CierreCaja_" + cierre.FechaFin.ToString("yyyyMMdd_HHmm"), htmlFinal);
            
        }

        private void btnReporteGeneral_Click(object sender, EventArgs e)
        {
            ExportarInformeFinancieroCompletoPDF();
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvReporte.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná una fila del cierre de caja.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fila = dgvReporte.SelectedRows[0];
            string formato = "dd/MM/yyyy HH:mm"; // El formato exacto que ves en la celda

            // Intentar convertir las fechas
            bool inicioOK = DateTime.TryParseExact(fila.Cells["FechaInicio"].Value?.ToString(), formato, null, System.Globalization.DateTimeStyles.None, out DateTime fechaInicio);
            bool cierreOK = DateTime.TryParseExact(fila.Cells["FechaFin"].Value?.ToString(), formato, null, System.Globalization.DateTimeStyles.None, out DateTime fechaFin);

            if (!inicioOK || !cierreOK)
            {
                MessageBox.Show("La fecha de inicio o cierre no es válida.", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CierreCaja cierre = new CierreCaja()
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                UsuarioResponsable = fila.Cells["Usuario"].Value?.ToString(),
                MontoInicial = Convert.ToDecimal(fila.Cells["MontoInicial"].Value),
                IngresosEfectivo = Convert.ToDecimal(fila.Cells["IngEfectivo"].Value),
                IngresosTarjeta = Convert.ToDecimal(fila.Cells["IngTarjeta"].Value),
                IngresosTransferencia = Convert.ToDecimal(fila.Cells["IngTransferencia"].Value),
                Egresos = Convert.ToDecimal(fila.Cells["EgrEfectivo"].Value),
                EgresosTransferencia = Convert.ToDecimal(fila.Cells["EgrTransferencia"].Value),
                TotalCalculado = Convert.ToDecimal(fila.Cells["TotalCalculado"].Value),
                TotalTransferencia = Convert.ToDecimal(fila.Cells["TotalTransferencia"].Value),
                GranTotal = Convert.ToDecimal(fila.Cells["GranTotal"].Value),
                TotalReal = Convert.ToDecimal(fila.Cells["TotalReal"].Value),
                Diferencia = Convert.ToDecimal(fila.Cells["Diferencia"].Value),
                Observaciones = fila.Cells["Observaciones"].Value?.ToString() ?? ""
            };

            GenerarReporteCierreCajaPDF(cierre);
        }

        private void ExportarExcelDesdeDataGridView()
        {
            if (dgvReporte.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                Filter = "Archivo Excel (*.xlsx)|*.xlsx",
                FileName = "ReporteFinanciero_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                using (var wb = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Reporte");

                    int columnas = dgvReporte.Columns.Count;
                    int filas = dgvReporte.Rows.Count;

                    // Escribir encabezados
                    for (int col = 0; col < columnas; col++)
                    {
                        ws.Cell(1, col + 1).Value = dgvReporte.Columns[col].HeaderText;
                        ws.Cell(1, col + 1).Style.Font.Bold = true;
                        ws.Cell(1, col + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                        ws.Cell(1, col + 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        ws.Cell(1, col + 1).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    }

                    // Escribir datos
                    for (int row = 0; row < filas; row++)
                    {
                        for (int col = 0; col < columnas; col++)
                        {
                            var value = dgvReporte.Rows[row].Cells[col].Value?.ToString();
                            ws.Cell(row + 2, col + 1).Value = value;
                            ws.Cell(row + 2, col + 1).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        }
                    }

                    // Aplicar formato de tabla
                    var rango = ws.Range(1, 1, filas + 1, columnas);
                    var tabla = rango.CreateTable();
                    tabla.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium2;

                    ws.Columns().AdjustToContents();

                    wb.SaveAs(saveFile.FileName);
                }

                MessageBox.Show("Reporte exportado correctamente a Excel.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportarExcelDesdeDataGridView();
        }
    }
}
