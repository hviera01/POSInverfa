using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;
using CapaPresentacion.Modales;
using ClosedXML.Excel;

namespace CapaPresentacion
{
    public partial class FrmComprasCredito : Form
    {
        public FrmComprasCredito()
        {
            InitializeComponent();
        }

        private void FrmComprasCredito_Load(object sender, EventArgs e)
        {
            txtbusqueda.Select();
            CargarComprasCredito();

            // Opciones del estado
            if (cboEstadoFiltro.Items.Count == 0)
                cboEstadoFiltro.Items.AddRange(new string[] { "Deuda", "Liquidada", "Todas" });
            cboEstadoFiltro.SelectedIndex = 0;

            // Eventos
            cboEstadoFiltro.SelectedIndexChanged += cboEstadoFiltro_SelectedIndexChanged;

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
                if (ctrl is Button btn) btn.Click += Btn_Click;

            // Aplica filtro inicial
            AplicarFiltro();
            HabilitarDoubleBuffering(dgvComprasCredito);
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

        private void RecalcularTotalSaldoPendiente()
        {
            DataTable dt = dgvComprasCredito.DataSource as DataTable;
            if (dt == null)
            {
                var dvSrc = dgvComprasCredito.DataSource as DataView;
                if (dvSrc != null) dt = dvSrc.Table;
            }

            decimal total = 0m;

            if (dt != null)
            {
                var dv = dt.DefaultView; // respeta RowFilter
                foreach (DataRowView drv in dv)
                    total += ParseDecimal(drv["SaldoPendiente"]);
            }
            else
            {
                foreach (DataGridViewRow row in dgvComprasCredito.Rows)
                {
                    if (row.Visible)
                        total += ParseDecimal(row.Cells["SaldoPendiente"].Value);
                }
            }

            txtTotalSaldoPendiente.Text = total.ToString("C2", new CultureInfo("es-HN"));
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


        private void AplicarFiltro()
        {
            DataTable dt = dgvComprasCredito.DataSource as DataTable;
            if (dt == null)
            {
                var dv = dgvComprasCredito.DataSource as DataView;
                if (dv != null)
                    dt = dv.Table;
            }

            if (dt == null) return;


            string filtroTexto = (txtbusqueda.Text ?? "").Trim().Replace("'", "''");
            string estadoSeleccionado = (cboEstadoFiltro.SelectedItem?.ToString() ?? "Todas").Trim();

            var sb = new StringBuilder();

            // Filtro de texto (en varias columnas)
            if (!string.IsNullOrEmpty(filtroTexto))
                sb.AppendFormat("(NumeroDocumento LIKE '%{0}%' OR NombreProveedor LIKE '%{0}%' OR NoFactura LIKE '%{0}%')", filtroTexto);

            // Filtro por estado
            if (!string.Equals(estadoSeleccionado, "Todas", StringComparison.OrdinalIgnoreCase))
            {
                if (sb.Length > 0) sb.Append(" AND ");
                sb.AppendFormat("Estado = '{0}'", estadoSeleccionado); // "Deuda" o "Liquidada"
            }

            dt.DefaultView.RowFilter = sb.ToString();
            RecalcularTotalSaldoPendiente();

        }


        public void CargarComprasCredito()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("IdCompra");
                dt.Columns.Add("FechaRegistro");
                dt.Columns.Add("NumeroDocumento");
                dt.Columns.Add("NoFactura");
                dt.Columns.Add("NombreProveedor");
                dt.Columns.Add("MontoTotal");
                dt.Columns.Add("SaldoPendiente");
                dt.Columns.Add("FechaVencimiento");
                dt.Columns.Add("Estado");
                dt.Columns.Add("Vencida");

                // Obtener las compras a crédito
                var comprasCredito = new CN_Compra().ObtenerComprasCredito();

                decimal totalSaldoPendiente = 0; // Variable para almacenar el total de saldo pendiente

                foreach (var compra in comprasCredito)
                {
                    string estado = compra.SaldoPendiente > 0 ? "Deuda" : "Liquidada";
                    string vencida;

                    // Convertir la fecha de vencimiento de string a DateTime
                    DateTime fechaVencimiento;
                    bool isValidDate = DateTime.TryParseExact(compra.FechaVencimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVencimiento);

                    if (isValidDate)
                    {
                        vencida = DateTime.Now > fechaVencimiento ? "Vencida" : "Vigente";
                    }
                    else
                    {
                        vencida = "Fecha no válida"; // Manejo de error si la fecha no es válida
                    }

                    // Asegúrate de que la fecha de vencimiento sea válida
                    string fechaVencimientoStr = isValidDate ? fechaVencimiento.ToString("dd/MM/yyyy") : "N/A"; // Manejo de null

                    dt.Rows.Add(compra.IdCompra,
                                 compra.FechaRegistro,
                                 compra.NumeroDocumento,
                                 compra.NoFactura,
                                 compra.oProveedor.RazonSocial,
                                 compra.MontoTotal,
                                 compra.SaldoPendiente,
                                 fechaVencimientoStr, // Usar la fecha formateada
                                 estado,
                                 vencida);

                    // Sumar el saldo pendiente
                    totalSaldoPendiente += compra.SaldoPendiente;
                }

                // Asignar el DataTable como fuente de datos del DataGridView
                dgvComprasCredito.DataSource = dt;
                dgvComprasCredito.Columns["IdCompra"].Visible = false;

                // Modificar encabezados y anchos de columnas
                dgvComprasCredito.Columns["FechaRegistro"].HeaderText = "Fecha de Registro";
                dgvComprasCredito.Columns["NumeroDocumento"].HeaderText = "Número de Documento";
                dgvComprasCredito.Columns["NoFactura"].HeaderText = "Número de Factura";
                dgvComprasCredito.Columns["NombreProveedor"].HeaderText = "Proveedor";
                dgvComprasCredito.Columns["MontoTotal"].HeaderText = "Monto Total";
                dgvComprasCredito.Columns["SaldoPendiente"].HeaderText = "Saldo Pendiente";
                dgvComprasCredito.Columns["FechaVencimiento"].HeaderText = "Fecha de Vencimiento";
                dgvComprasCredito.Columns["Estado"].HeaderText = "Estado";
                dgvComprasCredito.Columns["Vencida"].HeaderText = "Vencida";

                dgvComprasCredito.Columns["FechaRegistro"].Width = 150;
                dgvComprasCredito.Columns["NumeroDocumento"].Width = 120;
                dgvComprasCredito.Columns["NoFactura"].Width = 150;
                dgvComprasCredito.Columns["NombreProveedor"].Width = 250;
                dgvComprasCredito.Columns["MontoTotal"].Width = 130;
                dgvComprasCredito.Columns["SaldoPendiente"].Width = 130;
                dgvComprasCredito.Columns["FechaVencimiento"].Width = 150;
                dgvComprasCredito.Columns["Estado"].Width = 100;
                dgvComprasCredito.Columns["Vencida"].Width = 100;

                // Mostrar el total de saldo pendiente en el TextBox
                txtTotalSaldoPendiente.Text = totalSaldoPendiente.ToString("C2", new CultureInfo("es-HN"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las compras a crédito: " + ex.Message);
            }
            RecalcularTotalSaldoPendiente();

        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

        private void btnregistrarabono_Click(object sender, EventArgs e)
        {
            if (dgvComprasCredito.SelectedRows.Count > 0)
            {
                // Obtener la información de la fila seleccionada
                var filaSeleccionada = dgvComprasCredito.SelectedRows[0];
                int idCompra = Convert.ToInt32(filaSeleccionada.Cells["IdCompra"].Value); // Obtener el IdCompra
                string numeroDocumento = filaSeleccionada.Cells["NumeroDocumento"].Value.ToString();
                string noFactura = filaSeleccionada.Cells["NoFactura"].Value.ToString();
                string nombreProveedor = filaSeleccionada.Cells["NombreProveedor"].Value.ToString();
                decimal montoTotal = Convert.ToDecimal(filaSeleccionada.Cells["MontoTotal"].Value);

                // Obtener la fecha de vencimiento como DateTime
                DateTime fechaVencimiento;
                string formatoEsperado = "dd/MM/yyyy"; // Cambia esto según tu formato esperado
                if (!DateTime.TryParseExact(filaSeleccionada.Cells["FechaVencimiento"].Value.ToString(), formatoEsperado, CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVencimiento))
                {
                    MessageBox.Show("La fecha de vencimiento no es válida.");
                    return; // Salir si la fecha no es válida
                }

                decimal saldoPendiente = Convert.ToDecimal(filaSeleccionada.Cells["SaldoPendiente"].Value);
                string estado = filaSeleccionada.Cells["Estado"].Value.ToString();

                // Crear una instancia del formulario modal y pasar el idCompra
                MdAbonosCompra mdAbonosCompra = new MdAbonosCompra(idCompra);

                // Rellenar los campos del formulario modal
                mdAbonosCompra.SetFacturaData(noFactura, nombreProveedor, montoTotal, fechaVencimiento, saldoPendiente, estado);

                // Establecer el propietario del formulario modal
                mdAbonosCompra.Owner = this;

                // Mostrar el formulario modal
                mdAbonosCompra.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una compra a crédito.");
            }
        }

        public void ActualizarDataGrid()
        {
            // Llama al método que carga las compras a crédito nuevamente
            CargarComprasCredito();
        }

        private void btnregistrarcompracredito_Click(object sender, EventArgs e)
        {
            Md_RegistrarCredito mdRegistrarCredito = new Md_RegistrarCredito();

            // Establecer el propietario del formulario modal
            mdRegistrarCredito.Owner = this;

            // Mostrar el formulario modal
            var result = mdRegistrarCredito.ShowDialog();

            // Si el modal se cerró y se registró correctamente, actualizar el DataGrid
            if (result == DialogResult.OK)
            {
                ActualizarDataGrid();
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvComprasCredito.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();

                // Agregar columnas al DataTable
                foreach (DataGridViewColumn columna in dgvComprasCredito.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                // Recorrer las filas del DataGridView
                foreach (DataGridViewRow row in dgvComprasCredito.Rows)
                {
                    if (row.Visible)
                    {
                        // Verificar si las celdas son null antes de intentar acceder a su valor
                        dt.Rows.Add(new object[] {
                     // Si es null, poner cadena vacía
                     row.Cells[0].Value?.ToString() ?? "",
                    row.Cells[1].Value?.ToString() ?? "",
                    row.Cells[2].Value?.ToString() ?? "",
                    row.Cells[3].Value?.ToString() ?? "",
                    row.Cells[4].Value?.ToString() ?? "",
                    row.Cells[5].Value?.ToString() ?? "",
                    row.Cells[6].Value?.ToString() ?? "",
                    row.Cells[7].Value?.ToString() ?? "",
                    row.Cells[8].Value?.ToString() ?? "",
                    row.Cells[9].Value?.ToString() ?? "",
                   

                });
                    }
                }

                // Crear el diálogo para guardar el archivo
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteComprasCredito_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                savefile.Filter = "Excel Files | *.xlsx";

                // Si se selecciona una ruta para guardar el archivo
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Crear el archivo Excel con la librería ClosedXML
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(savefile.FileName);
                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            txtbusqueda.Select();
        }

        private void btnrefrescar_Click(object sender, EventArgs e)
        {
            RefrescarFormulario();
        }

        public void RefrescarFormulario()
        {
            // Guardar filtros actuales
            string texto = txtbusqueda.Text;
            string estado = cboEstadoFiltro.SelectedItem?.ToString() ?? "Todas";

            // Recargar
            CargarComprasCredito();

            // Restaurar filtros
            txtbusqueda.Text = texto;
            int idx = cboEstadoFiltro.Items.IndexOf(estado);
            if (idx >= 0) cboEstadoFiltro.SelectedIndex = idx;

            // Reaplicar
            AplicarFiltro();
        }


        private void LimpiarCampos()
        {
            // Limpiar los campos de búsqueda
            txtbusqueda.Text = "";
            txtbusqueda.Select(); // Seleccionar el campo de búsqueda
        }

        private void cboEstadoFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            FrmResumenAbonosProveedor frm = new FrmResumenAbonosProveedor();
            frm.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
