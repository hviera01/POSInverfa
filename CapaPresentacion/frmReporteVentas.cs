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
        public string Busqueda { get; set; }
        public string Busqueda1 { get; set; }
        public string Busqueda2 { get; set; }
        public string Busqueda3 { get; set; }
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            txtbusqueda.Text = Busqueda;
            txtbusqueda1.Text = Busqueda1;
            txtbusqueda2.Text = Busqueda2;
            txtbusqueda3.Text = Busqueda3;

            // Inicializar los ComboBoxes
            cbobusqueda.Items.Clear();
            cbobusqueda1.Items.Clear();
            cbobusqueda2.Items.Clear();
            cbobusqueda3.Items.Clear();

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                OpcionCombo opcion = new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText };
                cbobusqueda.Items.Add(opcion);
                cbobusqueda1.Items.Add(opcion);
                cbobusqueda2.Items.Add(opcion);
                cbobusqueda3.Items.Add(opcion);
            }

            // Configurar los ComboBoxes
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

            // Asignar el evento Click a todos los botones
            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }

            // Realizar búsqueda si hay valores en los filtros
            if (!string.IsNullOrEmpty(Busqueda) || !string.IsNullOrEmpty(Busqueda1) ||
                !string.IsNullOrEmpty(Busqueda2) || !string.IsNullOrEmpty(Busqueda3))
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
                    string tipoDocumento = row.Cells["TipoDocumento"].Value?.ToString();
                    string estado = row.Cells["Estado"].Value?.ToString();

                    // Excluir "Cotizacion" y "Canje", y solo contar si el estado es "Activa"
                    if (tipoDocumento != "Cotizacion" && tipoDocumento != "Canje" && estado == "Activa")
                    {
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
            Busqueda = txtbusqueda.Text;
            Busqueda1 = txtbusqueda1.Text;
            Busqueda2 = txtbusqueda2.Text;
            Busqueda3 = txtbusqueda3.Text;
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


                dgvdata.Rows.Clear();


                txtProductoMasVendido.Invoke((MethodInvoker)delegate {
                    txtProductoMasVendido.Text = resultado.ProductoMasVendido;
                });

                if (resultado.Ventas == null || resultado.Ventas.Count == 0)
                {
                    MessageBox.Show("No se encontraron resultados.");
                    txtTotalFacturado.Text = "0.00";
                }
                else
                {

                    var listaFiltrada = FiltrarResultados(resultado.Ventas);

                    foreach (ReporteVenta rv in listaFiltrada)
                    {
                        dgvdata.Rows.Add(new object[] {
                    rv.FechaRegistro,
                    rv.TipoDocumento,
                    rv.NumeroDocumento,
                    rv.PorcentajeTarjeta,
                    rv.ComisionBancaria,
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


                    CalcularTotalFacturado();
                    FiltrarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la búsqueda: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnbuscarresultado.Enabled = true;
            }
        }

        private List<ReporteVenta> FiltrarResultados(List<ReporteVenta> lista)
        {
           
            string columnaFiltro1 = ((OpcionCombo)cbobusqueda1.SelectedItem).Valor.ToString();
            string busqueda1 = txtbusqueda1.Text.Trim().ToUpper();

            
            string columnaFiltro2 = ((OpcionCombo)cbobusqueda2.SelectedItem).Valor.ToString();
            string busqueda2 = txtbusqueda2.Text.Trim().ToUpper();

          
            string columnaFiltro3 = ((OpcionCombo)cbobusqueda3.SelectedItem).Valor.ToString();
            string busqueda3 = txtbusqueda3.Text.Trim().ToUpper();

            
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

               
                foreach (DataGridViewColumn columna in dgvdata.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }

               
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                    {
                       
                        dt.Rows.Add(new object[] {
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
                    row.Cells[10].Value?.ToString() ?? "",
                    row.Cells[11].Value?.ToString() ?? "", 
                    row.Cells[12].Value?.ToString() ?? "",
                    row.Cells[13].Value?.ToString() ?? "",
                    row.Cells[14].Value?.ToString() ?? ""
                });
                    }
                }

                
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = string.Format("ReporteVentas_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                savefile.Filter = "Excel Files | *.xlsx";

                
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                       
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
           
            if (dgvdata.Columns[12].Name == "Estado")
            {
             
                if (e.Value != null && e.Value.ToString() == "Anulada")
                {
                   
                    e.CellStyle.BackColor = Color.Red;
                    
                    e.CellStyle.ForeColor = Color.White;
                }
            }
            if (dgvdata.Columns[1].Name == "TipoDocumento")
            {
                if (e.Value != null && e.Value.ToString() == "Cotizacion")
                {
                  
                    e.CellStyle.BackColor = Color.Yellow;
                    
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnbuscar.PerformClick(); 
                e.SuppressKeyPress = true; 
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

                    
                    if (!string.IsNullOrEmpty(busqueda2) &&
                        !row.Cells[columnaFiltro2].Value.ToString().Trim().ToUpper().Contains(busqueda2))
                    {
                        visible = false;
                    }

                    
                    if (!string.IsNullOrEmpty(busqueda3) &&
                        !row.Cells[columnaFiltro3].Value.ToString().Trim().ToUpper().Contains(busqueda3))
                    {
                        visible = false;
                    }

                   
                    row.Visible = visible;
                }
            }

           
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

        private void RefrescarFormulario()
        {
            
            txtbusqueda.Text = "";
            txtbusqueda1.Text = "";
            txtbusqueda2.Text = "";
            txtbusqueda3.Text = "";

          
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }

         
            CargarDatos();
        }

        private void CargarDatos()
        {
            string fechaInicio = txtfechainicio.Value.ToString("dd/MM/yyyy");
            string fechaFin = txtfechafin.Value.ToString("dd/MM/yyyy");

            
            ResultadoReporteVenta resultado = new CN_Reporte().Venta(fechaInicio, fechaFin);

            
            dgvdata.Rows.Clear();

            
            txtProductoMasVendido.Text = resultado.ProductoMasVendido;

           
            if (resultado.Ventas == null || resultado.Ventas.Count == 0)
            {
                MessageBox.Show("No se encontraron resultados.");
                txtTotalFacturado.Text = "0.00"; 
            }
            else
            {
                foreach (ReporteVenta rv in resultado.Ventas)
                {
                    dgvdata.Rows.Add(new object[] {
                rv.FechaRegistro,
                rv.TipoDocumento,
                rv.NumeroDocumento,
                 rv.PorcentajeTarjeta,
                    rv.ComisionBancaria,
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

                CalcularTotalFacturado();
            }
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
            FiltrarDatos();
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if (dgvdata.CurrentRow != null && dgvdata.CurrentRow.Cells["NumeroDocumento"].Value != null)
            {
                // Obtener el Número de Documento de la fila seleccionada
                string numeroDocumento = dgvdata.CurrentRow.Cells["NumeroDocumento"].Value.ToString();

                // Crear instancia del formulario de detalle
                frmDetalleVenta detalleVentaForm = new frmDetalleVenta();

                // Al cargar el formulario, buscar la venta
                detalleVentaForm.Load += (s, args) =>
                {
                    detalleVentaForm.BuscarPorNumeroDocumento(numeroDocumento);
                };

                // Mostrar el formulario como diálogo
                detalleVentaForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione una fila válida.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnInsumos_Click(object sender, EventArgs e)
        {
            if (dgvdata.CurrentRow == null || dgvdata.CurrentRow.Cells["NumeroDocumento"].Value == null)
            {
                MessageBox.Show("Seleccione una venta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string numero = dgvdata.CurrentRow.Cells["NumeroDocumento"].Value.ToString();
            int idVenta = new CN_Venta().ObtenerIdVentaPorNumeroDocumento(numero);

            if (idVenta <= 0)
            {
                MessageBox.Show("No se encontró la venta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dt = new CN_Venta().ObtenerPreparacionPorVenta(idVenta);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Esta venta no tiene preparación registrada.", "Detalle de insumo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form dlg = new Form();
            dlg.Text = "Detalle de insumos usados";
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.MinimizeBox = false;
            dlg.MaximizeBox = false;
            dlg.ShowIcon = false;
            dlg.Size = new Size(720, 420);

            DataGridView g = new DataGridView();
            g.Dock = DockStyle.Fill;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.DataSource = dt;

            // Formato de la columna CantidadUsada con 2 decimales
            if (g.Columns.Contains("CantidadUsada"))
            {
                g.Columns["CantidadUsada"].DefaultCellStyle.Format = "N2";
            }

            dlg.Controls.Add(g);
            dlg.ShowDialog(this);
        }
    }
}
