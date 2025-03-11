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
using DocumentFormat.OpenXml.Wordprocessing;

namespace CapaPresentacion
{
    public partial class FrmVentasCredito : Form
    {
        public FrmVentasCredito()
        {
            InitializeComponent();
            
        }

        private void btnregistrarabono_Click(object sender, EventArgs e)
        {
            if (dgvFacturasCredito.SelectedRows.Count > 0)
            {
                // Obtener la información de la fila seleccionada
                var filaSeleccionada = dgvFacturasCredito.SelectedRows[0];
                int idVenta = Convert.ToInt32(filaSeleccionada.Cells["IdVenta"].Value); // Obtener el IdVenta
                string numeroDocumento = filaSeleccionada.Cells["NumeroDocumento"].Value.ToString();
                string nombreCliente = filaSeleccionada.Cells["NombreCliente"].Value.ToString();
                decimal montoTotal = Convert.ToDecimal(filaSeleccionada.Cells["MontoTotal"].Value);

                // Intentar convertir la fecha de vencimiento de manera segura
                DateTime fechaVencimiento;
                string fechaVencimientoStr = filaSeleccionada.Cells["FechaVencimiento"].Value.ToString();

                if (!DateTime.TryParseExact(fechaVencimientoStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaVencimiento))
                {
                    MessageBox.Show("La fecha de vencimiento no es válida. Asegúrese de que esté en el formato dd/MM/yyyy.");
                    return; // Salir del método si la fecha no es válida
                }

                decimal saldoPendiente = Convert.ToDecimal(filaSeleccionada.Cells["SaldoPendiente"].Value);
                string estado = filaSeleccionada.Cells["Estado"].Value.ToString();

                // Crear una instancia del formulario modal
                FrmCredito frmCredito = new FrmCredito();

                // Rellenar los campos del formulario modal
                frmCredito.SetFacturaData(numeroDocumento, nombreCliente, montoTotal, fechaVencimiento, saldoPendiente, estado);
                frmCredito.SetIdVenta(idVenta); // Método que debes implementar en FrmCredito

                // Establecer el propietario del formulario modal
                frmCredito.Owner = this;

                // Mostrar el formulario modal
                frmCredito.ShowDialog();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una venta a crédito.");
            }
        }

        private void FrmVentasCredito_Load(object sender, EventArgs e)
        {
            txtbusqueda.Select();
            CargarFacturasCredito();

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
            txtbusqueda.Select();
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
        public void CargarFacturasCredito()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("IdVenta"); // Agregar columna IdVenta
                dt.Columns.Add("FechaRegistro");
                dt.Columns.Add("NumeroDocumento");
                dt.Columns.Add("NombreCliente");
                dt.Columns.Add("MontoTotal");
                dt.Columns.Add("SaldoPendiente");
                dt.Columns.Add("FechaVencimiento");
                dt.Columns.Add("Estado");
                dt.Columns.Add("Vencida"); // Nueva columna para indicar si está vencida

                // Obtener las facturas a crédito
                var facturasCredito = new CN_Venta().ObtenerFacturasCredito();

                decimal totalSaldoPendiente = 0; // Variable para almacenar el total de saldo pendiente

                foreach (var factura in facturasCredito)
                {
                    string estado = factura.SaldoPendiente > 0 ? "Debe" : "Liquidada";
                    string vencida = DateTime.Now > factura.FechaVencimiento ? "Vencida" : "Vigente"; // Cambiar a texto

                    // Asegúrate de que el valor de FechaRegistro sea un DateTime
                    DateTime fechaRegistroDateTime;
                    string fechaRegistroFormateada = DateTime.TryParse(factura.FechaRegistro, out fechaRegistroDateTime)
                        ? fechaRegistroDateTime.ToString("dd/MM/yyyy")
                        : "Fecha no válida"; // Manejo de error

                    // Formatear FechaVencimiento
                    string fechaVencimientoFormateada = factura.FechaVencimiento?.ToString("dd/MM/yyyy") ?? "Sin fecha"; // Manejo de null

                    dt.Rows.Add(factura.IdVenta, fechaRegistroFormateada, factura.NumeroDocumento, factura.NombreCliente, factura.MontoTotal, factura.SaldoPendiente, fechaVencimientoFormateada, estado, vencida);

                    // Sumar el saldo pendiente
                    totalSaldoPendiente += factura.SaldoPendiente;
                }

                // Asignar el DataTable como fuente de datos del DataGridView
                dgvFacturasCredito.DataSource = dt;
                dgvFacturasCredito.Columns["IdVenta"].Visible = false;

                // Modificar encabezados y anchos de columnas
                dgvFacturasCredito.Columns["FechaRegistro"].HeaderText = "Fecha de Registro";
                dgvFacturasCredito.Columns["NumeroDocumento"].HeaderText = "No.Factura";
                dgvFacturasCredito.Columns["NombreCliente"].HeaderText = "Cliente";
                dgvFacturasCredito.Columns["MontoTotal"].HeaderText = "Monto Total";
                dgvFacturasCredito.Columns["SaldoPendiente"].HeaderText = "Saldo Pendiente";
                dgvFacturasCredito.Columns["FechaVencimiento"].HeaderText = "Fecha de Vencimiento";
                dgvFacturasCredito.Columns["Estado"].HeaderText = "Estado";
                dgvFacturasCredito.Columns["Vencida"].HeaderText = "Vencida"; // Encabezado para la nueva columna

                // Establecer anchos de columnas
                dgvFacturasCredito.Columns["FechaRegistro"].Width = 200;
                dgvFacturasCredito.Columns["NumeroDocumento"].Width = 120;
                dgvFacturasCredito.Columns["NombreCliente"].Width = 200;
                dgvFacturasCredito.Columns["MontoTotal"].Width = 130;
                dgvFacturasCredito.Columns["SaldoPendiente"].Width = 130;
                dgvFacturasCredito.Columns["FechaVencimiento"].Width = 200;
                dgvFacturasCredito.Columns["Estado"].Width = 135;
                dgvFacturasCredito.Columns["Vencida"].Width = 135; // Ancho para la columna de vencida

                // Cambiar el color de las celdas según el estado
                foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
                {
                    if (row.Cells["Vencida"].Value.ToString() == "Vencida")
                    {
                        row.Cells["Vencida"].Style.BackColor = System.Drawing.Color.Red; // Vencida
                    }
                    else if (row.Cells["Estado"].Value.ToString() == "Liquidada")
                    {
                        row.Cells["Estado"].Style.BackColor = System.Drawing.Color.Green; // Liquidada
                    }
                }

                // Mostrar el total de saldo pendiente en el TextBox
                txtTotalSaldoPendiente.Text = totalSaldoPendiente.ToString("C2", new CultureInfo("es-HN"));

                // Ordenar por Fecha de Registro
                if (dt.Columns.Contains("FechaRegistro"))
                {
                    dgvFacturasCredito.Sort(dgvFacturasCredito.Columns["FechaRegistro"], System.ComponentModel.ListSortDirection.Descending);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las facturas a crédito: " + ex.Message);
            }
        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            string filter = txtbusqueda.Text.Trim();
            (dgvFacturasCredito.DataSource as DataTable).DefaultView.RowFilter = string.Format("NumeroDocumento LIKE '%{0}%' OR NombreCliente LIKE '%{0}%' OR Estado LIKE '%{0}%'", filter);
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            txtbusqueda.Select();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvFacturasCredito.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();

                // Agregar columnas al DataTable
                foreach (DataGridViewColumn columna in dgvFacturasCredito.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                // Recorrer las filas del DataGridView
                foreach (DataGridViewRow row in dgvFacturasCredito.Rows)
                {
                    if (row.Visible)
                    {
                        // Verificar si las celdas son null antes de intentar acceder a su valor
                        dt.Rows.Add(new object[] {
                    row.Cells[0].Value?.ToString() ?? "",  // Si es null, poner cadena vacía
                    row.Cells[1].Value?.ToString() ?? "",
                    row.Cells[2].Value?.ToString() ?? "",
                    row.Cells[3].Value?.ToString() ?? "",
                    row.Cells[4].Value?.ToString() ?? "",
                    row.Cells[5].Value?.ToString() ?? "",
                    row.Cells[6].Value?.ToString() ?? "",
                    row.Cells[7].Value?.ToString() ?? "",
                    
                });
                    }
                }

                // Crear el diálogo para guardar el archivo
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteVentasCredito_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        private void btnregistrarventacredito_Click(object sender, EventArgs e)
        {
            using (var modal = new md_RegistrarVentaCredito())
            {
                // Mostrar el modal como un diálogo
                var result = modal.ShowDialog();

                // Verificar si se registró la venta correctamente
                if (result == DialogResult.OK)
                {
                    // Aquí puedes actualizar la lista de ventas o realizar otras acciones necesarias
                    MessageBox.Show("Venta registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarFacturasCredito(); // Llama a tu método para recargar las facturas si es necesario
                }
            }
        }
    }
}
