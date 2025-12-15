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
    public partial class frmReporteCompras : Form
    {
        public frmReporteCompras()
        {
            InitializeComponent();
        }

        private void frmReporteCompras_Load(object sender, EventArgs e)
        {

            List<Proveedor> lista = new CN_Proveedor().Listar();

            cboproveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach (Proveedor item in lista)
            {
                cboproveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });
            }
            cboproveedor.DisplayMember = "Texto";
            cboproveedor.ValueMember = "Valor";
            cboproveedor.SelectedIndex = 0;

            // Agregar opciones a los ComboBox de búsqueda
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

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }


        private void btnbuscarresultado_Click(object sender, EventArgs e)
        {
            int idproveedor = Convert.ToInt32(((OpcionCombo)cboproveedor.SelectedItem).Valor);

            // Asegúrate de que las fechas se pasen en el formato correcto para SQL Server
            string fechaInicio = txtfechainicio.Value.ToString("yyyy-MM-dd"); // Cambia a formato YYYY-MM-DD
            string fechaFin = txtfechafin.Value.ToString("yyyy-MM-dd"); // Cambia a formato YYYY-MM-DD

            // Si la fecha de fin es menor que la fecha de inicio, ajusta la fecha de fin a hoy
            if (txtfechafin.Value.Date < txtfechainicio.Value.Date)
            {
                fechaFin = DateTime.Now.ToString("yyyy-MM-dd"); // Cambiar a la fecha actual
            }

            // Llamar al método de la capa de negocio para obtener los datos
            List<ReporteCompra> lista = new CN_Reporte().Compra(fechaInicio, fechaFin, idproveedor);

            // Limpiar el DataGridView antes de agregar nuevos datos
            dgvdata.Rows.Clear();

            if (lista == null || lista.Count == 0)
            {
                MessageBox.Show("No se encontraron resultados.");
            }
            else
            {
                foreach (ReporteCompra rc in lista)
                {
                    dgvdata.Rows.Add(new object[] {
                rc.FechaRegistro, // Ya está en formato DD/MM/YYYY
                rc.TipoDocumento,
                rc.NoFactura,
                rc.NumeroDocumento,
                rc.MontoTotal,
                rc.CantidadProductos,
                rc.UsuarioRegistro,
                rc.DocumentoProveedor,
                rc.RazonSocial,
                rc.Condicion,
                rc.MetodoPago,
                rc.FechaVencimiento,
               rc.Impuesto,
        rc.Estado
            });
                }
            }
            PintarFilasAnuladas();
            CalcularTotalFacturado();
            FiltrarDatos();
        }

        private void CalcularTotalFacturado()
        {
            decimal totalFacturado = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Visible) // Solo sumar filas visibles
                {
                    // Asegúrate de que el valor de MontoTotal sea un número válido
                    if (decimal.TryParse(row.Cells["MontoTotal"].Value?.ToString(), out decimal montoTotal))
                    {
                        totalFacturado += montoTotal;
                    }
                }
            }

            txtTotalFacturado.Text = "L. " + totalFacturado.ToString("N2");  // Muestra el total en formato numérico
        }

        private void PintarFilasAnuladas()
        {
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["Estado"].Value != null && row.Cells["Estado"].Value.ToString() != "Activa")
                {
                    row.Cells["Estado"].Style.BackColor = Color.Red;
                    row.Cells["Estado"].Style.ForeColor = Color.White;
                }
                else
                {
                    // Restaurar color para activas
                    row.Cells["Estado"].Style.BackColor = Color.White;
                    row.Cells["Estado"].Style.ForeColor = Color.Black;
                }
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
                    row.Cells[0].Value?.ToString() ?? "",  // Si es null, poner cadena vacía
                    row.Cells[1].Value?.ToString() ?? "",
                    row.Cells[2].Value?.ToString() ?? "",
                    row.Cells[3].Value?.ToString() ?? "",
                    row.Cells[4].Value?.ToString() ?? "",
                    row.Cells[5].Value?.ToString() ?? "",
                    row.Cells[6].Value?.ToString() ?? "",
                    row.Cells[7].Value?.ToString() ?? "",
                    row.Cells[8].Value?.ToString() ?? "",
                    row.Cells[9].Value?.ToString() ?? "",
                    row.Cells[10].Value?.ToString() ?? "",
                    row.Cells[11].Value?.ToString() ?? "",
                    row.Cells[12].Value?.ToString() ?? "",
                    row.Cells[13].Value?.ToString() ?? "",
                    row.Cells[14].Value?.ToString() ?? ""

                });
                    }
                }


                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteCompras_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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
            txtbusqueda1.Text = "";
            txtbusqueda2.Text = "";
            txtbusqueda3.Text = "";

            // Hacer visibles todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }

            // Recalcular el total después de limpiar
            CalcularTotalFacturado();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void FiltrarDatos()
        {
            // Obtener los valores de los ComboBox y TextBox
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

                    // Filtrar por el segundo ComboBox y TextBox
                    if (!string.IsNullOrEmpty(busqueda1) &&
                        !row.Cells[columnaFiltro1].Value.ToString().Trim().ToUpper().Contains(busqueda1))
                    {
                        visible = false;
                    }

                    // Filtrar por el tercer ComboBox y TextBox
                    if (!string.IsNullOrEmpty(busqueda2) &&
                        !row.Cells[columnaFiltro2].Value.ToString().Trim().ToUpper().Contains(busqueda2))
                    {
                        visible = false;
                    }

                    // Filtrar por el cuarto ComboBox y TextBox
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

        private void CargarDatos()
        {
            // Obtener las compras entre las fechas seleccionadas
            string fechaInicio = txtfechainicio.Value.ToString("yyyy-MM-dd"); // Cambia a formato YYYY-MM-DD
            string fechaFin = txtfechafin.Value.ToString("yyyy-MM-dd"); // Cambia a formato YYYY-MM-DD
            int idproveedor = Convert.ToInt32(((OpcionCombo)cboproveedor.SelectedItem).Valor);

            // Llamar al método de la capa de negocio para obtener los datos
            List<ReporteCompra> lista = new CN_Reporte().Compra(fechaInicio, fechaFin, idproveedor);

            // Limpiar el DataGridView antes de agregar nuevos datos
            dgvdata.Rows.Clear();

            if (lista == null || lista.Count == 0)
            {
                MessageBox.Show("No se encontraron resultados.");
            }
            else
            {
                foreach (ReporteCompra rc in lista)
                {
                    dgvdata.Rows.Add(new object[] {
                rc.FechaRegistro, // Ya está en formato DD/MM/YYYY
                rc.TipoDocumento,
                rc.NoFactura,
                rc.NumeroDocumento,
                rc.MontoTotal,
                rc.CantidadProductos,
                rc.UsuarioRegistro,
                rc.DocumentoProveedor,
                rc.RazonSocial,
                rc.Condicion,
                rc.MetodoPago,
                rc.FechaVencimiento,
               rc.Impuesto,
        rc.Estado
            });
                }
            }
            CalcularTotalFacturado();
            PintarFilasAnuladas();
        }

        private void RefrescarFormulario()
        {
            // Limpiar los campos de búsqueda
            txtbusqueda.Text = "";
            txtbusqueda1.Text = "";
            txtbusqueda2.Text = "";
            txtbusqueda3.Text = "";

            // Hacer visibles todas las filas del DataGridView
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }

            // Volver a cargar los datos
            CargarDatos();
        }

        private void btnrefrescar_Click(object sender, EventArgs e)
        {
            RefrescarFormulario();
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            txtbusqueda1.Text = "";
            txtbusqueda2.Text = "";
            txtbusqueda3.Text = "";

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }

            CalcularTotalFacturado();
            FiltrarDatos();
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if (dgvdata.CurrentRow != null && dgvdata.CurrentRow.Cells["NumeroDocumento"].Value != null)
            {
                // Obtener el Número de Documento de la fila seleccionada
                string numeroDocumento = dgvdata.CurrentRow.Cells["NumeroDocumento"].Value.ToString();

                // Crear instancia del formulario de detalle
                frmDetalleCompra detalleComptaForm = new frmDetalleCompra();

                // Al cargar el formulario, buscar la venta
                detalleComptaForm.Load += (s, args) =>
                {
                    detalleComptaForm.BuscarPorNumeroDocumento(numeroDocumento);
                };

                // Mostrar el formulario como diálogo
                detalleComptaForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione una fila válida.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
 }

