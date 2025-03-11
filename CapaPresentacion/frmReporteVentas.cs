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
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;

namespace CapaPresentacion
{
    public partial class frmReporteVentas : Form
    {
        private string _busqueda;
        private string _busqueda1;
        private string _busqueda2;
        private string _busqueda3;
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                OpcionCombo opcion = new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText };
                cbobusqueda.Items.Add(opcion);
                cbobusqueda1.Items.Add(opcion);
                cbobusqueda2.Items.Add(opcion);
                cbobusqueda3.Items.Add(opcion);
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            cbobusqueda1.DisplayMember = "Texto";
            cbobusqueda1.ValueMember = "Valor";
            cbobusqueda1.SelectedIndex = 0;

            cbobusqueda2.DisplayMember = "Texto";
            cbobusqueda2.ValueMember = "Valor";
            cbobusqueda2.SelectedIndex = 0;

            cbobusqueda3.DisplayMember = "Texto";
            cbobusqueda3.ValueMember = "Valor";
            cbobusqueda3.SelectedIndex = 0;

            foreach (System.Windows.Forms.Control ctrl in this.Controls)

            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }
            txtbusqueda.Text = _busqueda;
            txtbusqueda1.Text = _busqueda1;
            txtbusqueda2.Text = _busqueda2;
            txtbusqueda3.Text = _busqueda3;

            if (!string.IsNullOrEmpty(_busqueda) || !string.IsNullOrEmpty(_busqueda1) || !string.IsNullOrEmpty(_busqueda2) || !string.IsNullOrEmpty(_busqueda3))
            {
                btnbuscarresultado.PerformClick();
            }

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private void CalcularTotalFacturado()
        {
            decimal totalFacturado = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Visible) // Solo sumar filas visibles
                {
                    // Verificar que el tipo de documento no sea "Cotizacion"
                    if (row.Cells["TipoDocumento"].Value?.ToString() != "Cotizacion")
                    {
                        // Asegurarse de que el valor de TotalAPagar sea un número válido
                        if (decimal.TryParse(row.Cells["TotalAPagar"].Value?.ToString(), out decimal totalAPagar))
                        {
                            totalFacturado += totalAPagar;
                        }
                    }
                }
            }

            txtTotalFacturado.Text = "L. " + totalFacturado.ToString("N2");
        }

        
        private async void btnbuscarresultado_Click(object sender, EventArgs e)
        {
            btnbuscarresultado.Enabled = false; // Deshabilitar el botón para evitar múltiples clics

            try
            {
                // Ejecutar la búsqueda en un hilo de fondo
                ResultadoReporteVenta resultado = await Task.Run(() =>
                {
                    return new CN_Reporte().Venta(
                        txtfechainicio.Value.ToString("dd/MM/yyyy"),
                        txtfechafin.Value.ToString("dd/MM/yyyy")
                    );
                });

                // Actualizar el DataGridView en el hilo de la UI
                dgvdata.Rows.Clear();

                // Asignar el producto más vendido al TextBox en el hilo de la UI
                txtProductoMasVendido.Invoke((MethodInvoker)delegate {
                    txtProductoMasVendido.Text = resultado.ProductoMasVendido; // Asignar el producto más vendido al TextBox
                });

                if (resultado.Ventas == null || resultado.Ventas.Count == 0)
                {
                    MessageBox.Show("No se encontraron resultados.");
                    txtTotalFacturado.Text = "0.00"; // Asegúrate de limpiar el total si no hay resultados
                }
                else
                {
                    // Filtrar por los tres criterios
                    var listaFiltrada = FiltrarResultados(resultado.Ventas);

                    foreach (ReporteVenta rv in listaFiltrada)
                    {
                        dgvdata.Rows.Add(new object[] {
                    rv.FechaRegistro,
                    rv.TipoDocumento,
                    rv.NumeroDocumento,
                    rv.TotalAPagar,
                    rv.CantidadProductos,
                    rv.MetodoPago,
                    rv.UsuarioRegistro,
                    rv.DocumentoCliente,
                    rv.NombreCliente,
                    rv.Impuesto,
                    rv.Condicion,
                    rv.FechaVencimiento,
                    rv.Estado
                });
                    }

                    // Calcular el total facturado y actualizar el TextBox
                    CalcularTotalFacturado();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la búsqueda: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnbuscarresultado.Enabled = true; // Volver a habilitar el botón
            }
        }

        private List<ReporteVenta> FiltrarResultados(List<ReporteVenta> lista)
        {
            // Filtrar por el primer ComboBox y TextBox
            string columnaFiltro1 = ((OpcionCombo)cbobusqueda1.SelectedItem).Valor.ToString();
            string busqueda1 = txtbusqueda1.Text.Trim().ToUpper();

            // Filtrar por el segundo ComboBox y TextBox
            string columnaFiltro2 = ((OpcionCombo)cbobusqueda2.SelectedItem).Valor.ToString();
            string busqueda2 = txtbusqueda2.Text.Trim().ToUpper();

            // Filtrar por el tercer ComboBox y TextBox
            string columnaFiltro3 = ((OpcionCombo)cbobusqueda3.SelectedItem).Valor.ToString();
            string busqueda3 = txtbusqueda3.Text.Trim().ToUpper();

            // Aplicar los filtros
            var resultadoFiltrado = lista.Where(rv =>
                (string.IsNullOrEmpty(busqueda1) || rv.GetType().GetProperty(columnaFiltro1).GetValue(rv, null).ToString().ToUpper().Contains(busqueda1)) &&
                (string.IsNullOrEmpty(busqueda2) || rv.GetType().GetProperty(columnaFiltro2).GetValue(rv, null).ToString().ToUpper().Contains(busqueda2)) &&
                (string.IsNullOrEmpty(busqueda3) || rv.GetType().GetProperty(columnaFiltro3).GetValue(rv, null).ToString().ToUpper().Contains(busqueda3))
            ).ToList();

            return resultadoFiltrado;
        }

        private void btnbuscar_Click(object sender, EventArgs e)
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
            txtbusqueda1.Text = "";
            txtbusqueda2.Text = "";
            txtbusqueda3.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();

                // Agregar columnas al DataTable
                foreach (DataGridViewColumn columna in dgvdata.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                // Recorrer las filas del DataGridView
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                    {
                        // Verificar si las celdas son null antes de intentar acceder a su valor
                        dt.Rows.Add(new object[] {
                    row.Cells[0].Value?.ToString() ?? "",  // FechaRegistro
                    row.Cells[1].Value?.ToString() ?? "",  // TipoDocumento
                    row.Cells[2].Value?.ToString() ?? "",  // NumeroDocumento
                    row.Cells[3].Value?.ToString() ?? "",  // TotalAPagar
                    row.Cells[4].Value?.ToString() ?? "",  // CantidadProductos
                    row.Cells[5].Value?.ToString() ?? "",  // MetodoPago
                    row.Cells[6].Value?.ToString() ?? "",  // UsuarioRegistro
                    row.Cells[7].Value?.ToString() ?? "",  // DocumentoCliente
                    row.Cells[8].Value?.ToString() ?? "",  // NombreCliente
                    row.Cells[9].Value?.ToString() ?? "",  // Impuesto
                    row.Cells[10].Value?.ToString() ?? "", // Condicion
                    row.Cells[11].Value?.ToString() ?? "", // FechaVencimiento
                    row.Cells[12].Value?.ToString() ?? ""  // Estado
                });
                    }
                }

                // Crear el diálogo para guardar el archivo
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteVentas_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        private void dgvdata_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Verifica si la columna que contiene el estado es la correcta (ajusta el índice de columna si es necesario)
            if (dgvdata.Columns[12].Name == "Estado")
            {
                // Verifica si el estado es "Anulada"
                if (e.Value != null && e.Value.ToString() == "Anulada")
                {
                    // Cambia el fondo de la celda a rojo
                    e.CellStyle.BackColor = Color.Red;
                    // Cambia el color del texto (opcional, para que sea más legible)
                    e.CellStyle.ForeColor = Color.White;
                }
            }
            if (dgvdata.Columns[1].Name == "TipoDocumento")
            {
                // Verifica si el estado es "Anulada"
                if (e.Value != null && e.Value.ToString() == "Cotizacion")
                {
                    // Cambia el fondo de la celda a rojo
                    e.CellStyle.BackColor = Color.Yellow;
                    // Cambia el color del texto (opcional, para que sea más legible)
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnbuscar.PerformClick(); // Simula un clic en el botón btnbuscar
                e.SuppressKeyPress = true; // Evita el sonido de "ding" al presionar Enter
            }
        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltrarDatos();
        }
        private void FiltrarDatos()
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            string busqueda = txtbusqueda.Text.Trim().ToUpper();

            string columnaFiltro1 = ((OpcionCombo)cbobusqueda1.SelectedItem).Valor.ToString();
            string busqueda1 = txtbusqueda1.Text.Trim().ToUpper();

            string columnaFiltro2 = ((OpcionCombo)cbobusqueda2.SelectedItem).Valor.ToString();
            string busqueda2 = txtbusqueda2.Text.Trim().ToUpper();

            string columnaFiltro3 = ((OpcionCombo)cbobusqueda3.SelectedItem).Valor.ToString();
            string busqueda3 = txtbusqueda3.Text.Trim().ToUpper();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    bool visible = true;

                    // Filtrar por el primer ComboBox y TextBox
                    if (!string.IsNullOrEmpty(busqueda) &&
                       !row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(busqueda))
                    {
                        visible = false;
                    }
                    if (!string.IsNullOrEmpty(busqueda1) &&
                        !row.Cells[columnaFiltro1].Value.ToString().Trim().ToUpper().Contains(busqueda1))
                    {
                        visible = false;
                    }

                    // Filtrar por el segundo ComboBox y TextBox
                    if (!string.IsNullOrEmpty(busqueda2) &&
                        !row.Cells[columnaFiltro2].Value.ToString().Trim().ToUpper().Contains(busqueda2))
                    {
                        visible = false;
                    }

                    // Filtrar por el tercer ComboBox y TextBox
                    if (!string.IsNullOrEmpty(busqueda3) &&
                        !row.Cells[columnaFiltro3].Value.ToString().Trim().ToUpper().Contains(busqueda3))
                    {
                        visible = false;
                    }

                    // Establecer la visibilidad de la fila
                    row.Visible = visible;
                }
            }

            // Actualizar el total facturado después de aplicar los filtros
            CalcularTotalFacturado();
        }

        private void txtbusqueda1_TextChanged(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void txtbusqueda2_TextChanged(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void txtbusqueda3_TextChanged(object sender, EventArgs e)
        {
            FiltrarDatos();
        }
    }
}
