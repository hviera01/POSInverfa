using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Resources;
using CapaPresentacion.Utilidades;
using ESCPOS_NET;
using ESCPOS_NET.Printers;
using ESCPOS_NET.Utilities;



namespace CapaPresentacion
{
    public partial class frmVentas : Form
    {
        private readonly Dictionary<int, List<ConsumoItem>> _prepPorItem = new Dictionary<int, List<ConsumoItem>>();


        public class ConsumoItem
        {
            public int NumeroItem { get; set; }
            public int IdProductoInsumo { get; set; }
            public string NombreInsumo { get; set; }
            public string UnidadElegida { get; set; }    
            public decimal CantidadUnidad { get; set; }  
            public string EquivalenteEn { get; set; }   
            public decimal CantidadUsada { get; set; }    
        }

        private Usuario _Usuario;

        private Negocio negocio;
        List<Producto> listaProductos = new List<Producto>();

        private int idCliente;
        private string bufferCodigoScanner = "";
        private DateTime ultimaTecla = DateTime.Now;
        private bool permisoModificarPrecio = false;
        private bool _bloqueandoPrecio = false;
        private string _ultimoTipoDocumentoValor = "VentaSinFacturar";



        public frmVentas(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            negocio = new CN_Negocio().ObtenerDatos();
            this.KeyPreview = true;
            dgvdata.SuspendLayout();
            dgvdata.ResumeLayout();
            this.DoubleBuffered = true;
        }
        private string numeroDocumentoImpresion;
        private void frmVentas_Load(object sender, EventArgs e)
        {
            dtpFecha.Value = DateTime.Today;
            comboMetodoPago.Items.Add("Efectivo");
            comboMetodoPago.Items.Add("Tarjeta");
            comboMetodoPago.Items.Add("Transferencia");
            comboMetodoPago.SelectedIndex = 0;
            cbocondicion.Items.Add("Contado");
            cbocondicion.Items.Add("Credito");
            cbocondicion.SelectedIndex = 0;
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "VentaSinFacturar", Texto = "Venta" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Cotizacion", Texto = "Cotizacion" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Canje", Texto = "Canje" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 1;

            cmbAplicarRecargo.Items.Clear(); // Limpiar elementos existentes
            cmbAplicarRecargo.Items.Add("Sí");
            cmbAplicarRecargo.Items.Add("No");
            cmbAplicarRecargo.SelectedIndex = 1;
            label18.Visible = false;
            cmbAplicarRecargo.Visible = false;
            label19.Visible = false;
            txtrecargo.Visible = false;

            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";

            cmbNivelPrecio.Items.Add("Nivel 1");
            cmbNivelPrecio.Items.Add("Nivel 2");
            cmbNivelPrecio.Items.Add("Nivel 3");
            cmbNivelPrecio.SelectedIndex = 0;


            
            txtidproducto.Text = "0";

            txtpagocon.Text = "";
            txtcambio.Text = "";
            txttotalpagar.Text = "0";

            foreach (System.Windows.Forms.Control ctrl in this.Controls)

            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }

            this.Focus();
           
            cmbPorcentajeTarjeta.Items.Add("2.9%");
            cmbPorcentajeTarjeta.Items.Add("3.5%");
            cmbPorcentajeTarjeta.SelectedIndex = 0;
            cmbPorcentajeTarjeta.Visible = false;

            CD_Producto oCD_Producto = new CD_Producto();
            listaProductos = oCD_Producto.Listar();
            _ultimoTipoDocumentoValor = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();


        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private void AjustarGridSegunCambioTipo(string tipoAnterior, string tipoNuevo)
        {
            // Tipos que muestran PRECIO sin ISV (se aplica ISV en el total)
            HashSet<string> tiposConISVSeparado = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { "Factura", "Boleta", "Cotizacion" };

            bool antesConISVSeparado = tiposConISVSeparado.Contains(tipoAnterior);
            bool ahoraConISVSeparado = tiposConISVSeparado.Contains(tipoNuevo);

            // Si es canje, solo recalcular totales de puntos
            if (string.Equals(tipoNuevo, "Canje", StringComparison.OrdinalIgnoreCase))
            {
                calcularTotalCanje();
                return;
            }

            // Recorremos filas y ajustamos Precio y SubTotal cuando cambia de régimen
            if (antesConISVSeparado != ahoraConISVSeparado)
            {
                decimal factor = ahoraConISVSeparado ? (1 / 1.15m) : 1.15m; // Venta->Factura: /1.15 ; Factura->Venta: *1.15

                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Evita errores si faltan columnas
                    if (row.Cells["Precio"]?.Value == null || row.Cells["Cantidad"]?.Value == null) continue;

                    if (!decimal.TryParse(row.Cells["Precio"].Value.ToString(), out decimal precio)) continue;
                    if (!decimal.TryParse(row.Cells["Cantidad"].Value.ToString(), out decimal cantidad)) continue;

                    decimal nuevoPrecio = Math.Round(precio * factor, 2, MidpointRounding.AwayFromZero);
                    decimal nuevoSubTotal = Math.Round(nuevoPrecio * cantidad, 2, MidpointRounding.AwayFromZero);

                    row.Cells["Precio"].Value = nuevoPrecio.ToString("0.00");
                    row.Cells["SubTotal"].Value = nuevoSubTotal.ToString("0.00");
                }
            }

            // Recalcular totales globales según el tipo nuevo
            calcularTotal();
        }


        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new mdCliente())
            {
                var result = modal.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtdocumentocliente.Text = modal._Cliente.Documento;
                    txtnombrecliente.Text = modal._Cliente.NombreCompleto;
                    txtIdCliente.Text = modal._Cliente.idCliente.ToString(); // Captura el ID del cliente


                    txtcodproducto.Select();
                }
                else
                {
                    txtdocumentocliente.Select();
                }
            }
        }

        private void btnbuscarproducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtidproducto.Text = modal._Producto.IdProducto.ToString();
                    txtIdCategoria.Text = modal._Producto.oCategoria.IdCategoria.ToString();
                    txtcodproducto.Text = modal._Producto.Codigo;
                    txtproducto.Text = modal._Producto.Nombre;

                    string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem)?.Texto;

                    decimal precioSeleccionado = 0;

                    if (tipoDocumento == "Canje")
                    {
                        precioSeleccionado = modal._Producto.PrecioPuntos;
                    }
                    else
                    {
                        string nivelSeleccionado = cmbNivelPrecio.SelectedItem.ToString();

                        switch (nivelSeleccionado)
                        {
                            case "Nivel 1":
                                precioSeleccionado = modal._Producto.PrecioVenta;
                                break;
                            case "Nivel 2":
                                precioSeleccionado = modal._Producto.Precio2;
                                break;
                            case "Nivel 3":
                                precioSeleccionado = modal._Producto.Precio3;
                                break;
                        }
                    }

                    txtprecio.Text = precioSeleccionado.ToString("0.00");
                    txtstock.Text = modal._Producto.Stock.ToString();

                    // 🔥 Cargar la PRIMA del producto en el textbox de prima
                    if (txtPrimaProducto != null)           // si tu textbox se llama txtPrima, cambia aquí el nombre
                        txtPrimaProducto.Text = modal._Producto.Prima.ToString("0.00");

                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.Select();
                }
            }
        }



        private void txtcodproducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Producto oProducto = new CN_Producto()
                                        .Listar()
                                        .Where(p => p.Codigo == txtcodproducto.Text.Trim() && p.Estado == true)
                                        .FirstOrDefault();

                if (oProducto != null)
                {
                    txtcodproducto.BackColor = Color.Honeydew;

                    txtidproducto.Text = oProducto.IdProducto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtprecio.Text = oProducto.PrecioVenta.ToString("0.00");
                    txtstock.Text = oProducto.Stock.ToString();

                    // 🔥 Cargar la PRIMA del producto
                    if (txtPrimaProducto != null)   // o txtPrima
                        txtPrimaProducto.Text = oProducto.Prima.ToString("0.00");

                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.BackColor = Color.MistyRose;

                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                    txtprecio.Text = "";
                    txtstock.Text = "";

                    if (txtPrimaProducto != null)
                        txtPrimaProducto.Text = "0.00";

                    txtcantidad.Value = 1;
                }
            }
        }

        private void RestarStockDeVenta(int idProducto, decimal cantidad)
        {

            Producto producto = new CN_Producto().Listar().FirstOrDefault(p => p.IdProducto == idProducto);
            if (producto != null)
            {
                producto.Stock -= cantidad;
            }
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            decimal precioBase = 0m;

            if (!int.TryParse(txtidproducto.Text, out int idProducto) || idProducto == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!decimal.TryParse(txtprecio.Text, out precioBase))
            {
                MessageBox.Show("Precio - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecio.Select();
                return;
            }
            if (!decimal.TryParse(txtstock.Text, out decimal stock))
            {
                stock = 0m;
            }
            if (!decimal.TryParse(txtcantidad.Value.ToString(), out decimal cantidad))
            {
                MessageBox.Show("Cantidad - Formato incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtcantidad.Select();
                return;
            }

            string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
            bool esCotizacion = string.Equals(tipoDocumento, "Cotizacion", StringComparison.OrdinalIgnoreCase);


            string color = txtCodigoColor.Text.Trim();
            string nombreProducto = txtproducto.Text;
            if (!string.IsNullOrWhiteSpace(color))
                nombreProducto += $" {color}";

            int idProductoFinal = idProducto;

            string idCategoria = txtIdCategoria.Text?.Trim() ?? "0";

            if (idCategoria == "52")
                MessageBox.Show("Producto externo a la tienda facturado.", "Mensaje");
            else if (idCategoria == "51")
                MessageBox.Show("Envío a domicilio facturado.", "Mensaje");
            else if (idCategoria == "50")
                MessageBox.Show("Servicio facturado.", "Mensaje");

            int rowIndex;

            // ============================
            // CANJE
            // ============================
            if (string.Equals(tipoDocumento, "Canje", StringComparison.OrdinalIgnoreCase))
            {
                int precioPuntos = (int)Math.Round(precioBase, MidpointRounding.AwayFromZero);
                int subtotalPuntos = (int)Math.Round(precioPuntos * cantidad, MidpointRounding.AwayFromZero);

                rowIndex = dgvdata.Rows.Add(new object[] {
            idProductoFinal.ToString(),
            idCategoria,
            nombreProducto,
            precioPuntos.ToString(),
            cantidad.ToString("0.00"),
            subtotalPuntos.ToString()
        });

                DataGridViewRow row = dgvdata.Rows[rowIndex];
                row.Cells["PrimaProducto"].Value = "0.00";
                row.Cells["NumeroCuotasProducto"].Value = 0;
                row.Cells["FrecuenciaProducto"].Value = "N/A";
                row.Cells["FechaInicioCreditoProducto"].Value = "";
                row.Cells["FechaVencimientoProducto"].Value = "";
                row.Cells["MontoCuotaProducto"].Value = "0.00";
                row.Cells["SaldoPendienteProducto"].Value = "0.00";

                calcularTotalCanje();
            }
            else
            {
                // ============================
                // NORMAL (Contado / Crédito)
                // ============================
                decimal precioFinal =
                    (esCotizacion ||
                     tipoDocumento == "Factura" ||
                     tipoDocumento == "Boleta")
                     ? Math.Round(precioBase / 1.15m, 2)
                     : Math.Round(precioBase, 2);

                decimal subtotal = Math.Round(precioFinal * cantidad, 2);

                rowIndex = dgvdata.Rows.Add(new object[] {
            idProductoFinal.ToString(),
            idCategoria,
            nombreProducto,
            precioFinal.ToString("0.00"),
            cantidad.ToString("0.00"),
            subtotal.ToString("0.00")
        });

                DataGridViewRow row = dgvdata.Rows[rowIndex];

                // ==============================================
                // AQUI VA EXACTAMENTE LO QUE ME PREGUNTASTE
                // ==============================================

                if (cbocondicion.SelectedItem != null &&
    cbocondicion.SelectedItem.ToString() == "Credito")
                {
                    decimal primaDefecto = 0m;
                    if (txtPrimaProducto != null &&
                        decimal.TryParse(txtPrimaProducto.Text, out decimal ptmp))
                        primaDefecto = ptmp;

                    // SubTotal del grid está SIN ISV para Factura / Boleta / Cotización.
                    // Aquí calculamos el total de la línea PARA EL CRÉDITO.
                    string tipoDocLinea = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
                    bool conISVSeparado = tipoDocLinea == "Factura" ||
                                          tipoDocLinea == "Boleta" ||
                                          tipoDocLinea == "Cotizacion";

                    decimal totalProducto = subtotal;

                    if (conISVSeparado)
                    {
                        // Le sumamos el 15% de ISV para que el crédito sea sobre el monto con impuesto
                        totalProducto = Math.Round(subtotal * 1.15m, 2, MidpointRounding.AwayFromZero);
                    }

                    AbrirModalCreditoParaFila(row, totalProducto, primaDefecto, false);
                }

                else
                {
                    row.Cells["PrimaProducto"].Value = "0.00";
                    row.Cells["NumeroCuotasProducto"].Value = 0;
                    row.Cells["FrecuenciaProducto"].Value = "N/A";
                    row.Cells["FechaInicioCreditoProducto"].Value = "N/A";
                    row.Cells["FechaVencimientoProducto"].Value = "N/A";
                    row.Cells["MontoCuotaProducto"].Value = "0.00";
                    row.Cells["SaldoPendienteProducto"].Value = "0.00";
                }

                // ==============================================
                // FIN DEL BLOQUE QUE PREGUNTASTE
                // ==============================================

                calcularTotal();
            }

            if (!esCotizacion && idCategoria != "35" && idCategoria != "49" && stock >= cantidad)
            {
                RestarStockDeVenta(idProductoFinal, cantidad);
            }

            txtCodigoColor.Clear();
            limpiarProducto();
            txtcodproducto.Select();
        }

        private void calcularTotal()
        {
            decimal totalSinImpuesto = 0;
            decimal totalISV = 0;

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Cells["SubTotal"].Value != null && decimal.TryParse(row.Cells["SubTotal"].Value.ToString(), out decimal subtotal))
                    {
                        totalSinImpuesto += subtotal;
                    }
                }
            }

            string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem)?.Valor.ToString();
            decimal totalPagar;

            if (tipoDocumento == "VentaSinFacturar")
            {
                totalISV = 0;
                totalPagar = totalSinImpuesto;
            }
            else
            {
                totalISV = totalSinImpuesto * 0.15m;
                totalPagar = totalSinImpuesto + totalISV;
            }

            totalPagar = Math.Round(totalPagar, 2, MidpointRounding.ToEven);

            if (totalPagar % 1 >= 0.90m)
            {
                totalPagar = Math.Ceiling(totalPagar); // Redondear hacia arriba
            }
            else
            {
                totalPagar = Math.Floor(totalPagar); // Redondear hacia abajo a un entero
            }

            txtsubtotal.Text = totalSinImpuesto.ToString("0.00");
            txttotalpagar.Text = totalPagar.ToString("0.00");
            txtimpuesto.Text = totalISV.ToString("0.00");
        }


        private void limpiarProducto()
        {
            txtidproducto.Text = "0";
            txtIdCategoria.Text = "0";
            txtcodproducto.Text = "";
            txtproducto.Text = "";
            txtprecio.Text = "";
            txtstock.Text = "";
            txtcantidad.Value = 1;
            txtcolor.Text = "";
            txtPrimaProducto.Text = "";
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.ColumnIndex == dgvdata.Columns["btneliminar"].Index)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.delete25.Width;
                var h = Properties.Resources.delete25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete25, new Rectangle(x, y, w, h));
                e.Handled = true;
                return;
            }

            if (e.ColumnIndex == dgvdata.Columns["btnEditarCredito"].Index)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                e.Handled = true;
                return;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string nombreColumna = dgvdata.Columns[e.ColumnIndex].Name;

            if (nombreColumna == "btneliminar")
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    DataGridViewRow fila = dgvdata.Rows[index];
                    decimal cantidadActual = Convert.ToDecimal(fila.Cells["Cantidad"].Value);

                    if (cantidadActual > 1)
                    {
                        cantidadActual -= 1;
                        fila.Cells["Cantidad"].Value = cantidadActual.ToString("0.00");

                        decimal precio = Convert.ToDecimal(fila.Cells["Precio"].Value);
                        fila.Cells["SubTotal"].Value = (cantidadActual * precio).ToString("0.00");
                    }
                    else
                    {
                        dgvdata.Rows.RemoveAt(index);
                    }

                    calcularTotal();
                }
            }

            if (nombreColumna == "btnEditarCredito")
            {
                DataGridViewRow row = dgvdata.Rows[e.RowIndex];

                if (!decimal.TryParse(row.Cells["Precio"].Value?.ToString(), out decimal precioFila))
                    precioFila = 0m;

                if (!decimal.TryParse(row.Cells["Cantidad"].Value?.ToString(), out decimal cantFila))
                    cantFila = 1m;

                decimal subtotalLinea = Math.Round(precioFila * cantFila, 2, MidpointRounding.AwayFromZero);

                string tipoDocLinea = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
                bool conISVSeparado = tipoDocLinea == "Factura" ||
                                      tipoDocLinea == "Boleta" ||
                                      tipoDocLinea == "Cotizacion";

                decimal totalProducto = subtotalLinea;
                if (conISVSeparado)
                    totalProducto = Math.Round(subtotalLinea * 1.15m, 2, MidpointRounding.AwayFromZero);

                decimal primaDefecto = 0m;
                if (row.Cells["PrimaProducto"].Value != null)
                    decimal.TryParse(row.Cells["PrimaProducto"].Value.ToString(), out primaDefecto);

                AbrirModalCreditoParaFila(row, totalProducto, primaDefecto, true);
            }
        }

        private void txtprecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtprecio.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }

            }
        }

        private void txtpagocon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpagocon.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }

            }
        }

        private void calcularcambio()
        {
            if (string.IsNullOrEmpty(txttotalpagar.Text))
            {
                MessageBox.Show("No existen productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            decimal pagacon;
            decimal total = Convert.ToDecimal(txttotalpagar.Text);

            // Si el campo de pago está vacío, asigna 0
            if (string.IsNullOrEmpty(txtpagocon.Text))
            {
                txtpagocon.Text = "0";
            }

            // Intenta convertir el monto pagado a decimal
            if (decimal.TryParse(txtpagocon.Text.Trim(), out pagacon))
            {
                // Si el monto pagado es menor que el total, el cambio es 0
                if (pagacon < total)
                {
                    txtcambio.Text = "0.00";
                }
                else
                {
                    // Calcula el cambio
                    decimal cambio = pagacon - total;
                    txtcambio.Text = cambio.ToString("0.00");
                }
            }

            // Asegúrate de que el monto pagado se muestre con dos decimales
            txtpagocon.Text = pagacon.ToString("0.00");
        }


        private bool pagoconEnterPressed = false;

        private void txtpagocon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                calcularcambio();
                pagoconEnterPressed = true;
            }
        }

        private bool EnsurePagoConEfectivo()
        {
            // Solo aplica a ventas de contado con método Efectivo
            if (cbocondicion.SelectedItem?.ToString() != "Contado") return true;
            if (comboMetodoPago.SelectedItem?.ToString() != "Efectivo") return true;

            if (!decimal.TryParse(txttotalpagar.Text, out decimal total) || total <= 0)
            {
                MessageBox.Show("El monto total no puede estar vacío o en cero.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Si ya hay valor en txtpagocon, no hacemos nada
            if (!string.IsNullOrWhiteSpace(txtpagocon.Text)) return true;

            using (var modal = new CapaPresentacion.Modales.md_CobrarFactura(total))
            {
                var r = modal.ShowDialog();
                if (r == DialogResult.OK)
                {
                    txtpagocon.Text = modal.PagoCon.ToString("0.00");
                    txtcambio.Text = modal.Cambio.ToString("0.00");
                    return true;
                }
                else
                {
                    // Cajero canceló: abortar flujo
                    return false;
                }
            }
        }

        private void btncrearventa_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Validar que se haya seleccionado un método de pago
            if (comboMetodoPago.SelectedIndex == -1 && cbocondicion.SelectedItem.ToString() != "Credito")
            {
                MessageBox.Show("Seleccione un método de pago.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar que el monto total no esté vacío
            if (string.IsNullOrEmpty(txttotalpagar.Text))
            {
                MessageBox.Show("El monto total no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Crear objeto de venta (para manejo de pago)
            Venta objVenta = new Venta
            {
                MetodoPago = cbocondicion.SelectedItem.ToString() == "Credito" ? "N/A" : comboMetodoPago.SelectedItem.ToString(),
                MontoTotal = decimal.Parse(txttotalpagar.Text),
                Condicion = cbocondicion.SelectedItem.ToString()
            };

            // Si la condición es "Crédito", no se requiere monto pagado ni cambio
            if (objVenta.Condicion == "Credito")
            {
                objVenta.MontoPago = 0;
                objVenta.MontoCambio = 0;
            }
            else
            {
                if (objVenta.MetodoPago == "Efectivo")
                {
                    if (!EnsurePagoConEfectivo())
                        return; // usuario canceló

                    objVenta.MontoPago = decimal.Parse(txtpagocon.Text);
                    objVenta.MontoCambio = objVenta.MontoPago - objVenta.MontoTotal;

                    if (objVenta.MontoCambio < 0)
                    {
                        MessageBox.Show("El monto pagado no puede ser menor al total.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (objVenta.MetodoPago == "Tarjeta" || objVenta.MetodoPago == "Transferencia")
                {
                    objVenta.MontoPago = 0;
                    objVenta.MontoCambio = 0;
                }
            }

            // ==========================
            // ARMAR DETALLE_VENTA
            // ==========================
            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(decimal));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));
            detalle_venta.Columns.Add("NombreProducto", typeof(string));
            detalle_venta.Columns.Add("PrecioCompraUsado", typeof(decimal));
            detalle_venta.Columns.Add("PrimaProducto", typeof(decimal));
            detalle_venta.Columns.Add("NumeroCuotasProducto", typeof(int));
            detalle_venta.Columns.Add("FechaInicioCreditoProducto", typeof(DateTime));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["IdProducto"].Value == null)
                    continue;

                int idProducto = Convert.ToInt32(row.Cells["IdProducto"].Value);
                decimal precioVenta = Convert.ToDecimal(row.Cells["Precio"].Value);
                decimal cantidad = Convert.ToDecimal(row.Cells["Cantidad"].Value);
                decimal subtotal = Convert.ToDecimal(row.Cells["SubTotal"].Value);

                string nombreProducto = row.Cells["Producto"].Value.ToString();

                Producto prod = listaProductos.FirstOrDefault(p => p.IdProducto == idProducto);
                decimal precioCompraBase = prod != null ? prod.PrecioCompra : 0m;
                decimal precioCompraUsado = Math.Round(precioCompraBase * cantidad, 2);

                decimal primaProducto = 0m;
                if (row.Cells["PrimaProducto"].Value != null)
                    decimal.TryParse(row.Cells["PrimaProducto"].Value.ToString(), out primaProducto);

                int numeroCuotasProducto = 0;
                if (row.Cells["NumeroCuotasProducto"].Value != null)
                    int.TryParse(row.Cells["NumeroCuotasProducto"].Value.ToString(), out numeroCuotasProducto);

                DateTime fechaInicioCreditoProducto = DateTime.MinValue;
                bool tieneFechaInicio = false;
                if (row.Cells["FechaInicioCreditoProducto"].Value != null &&
                    DateTime.TryParse(row.Cells["FechaInicioCreditoProducto"].Value.ToString(),
                                      out fechaInicioCreditoProducto))
                {
                    tieneFechaInicio = true;
                }

                DataRow dr = detalle_venta.NewRow();
                dr["IdProducto"] = idProducto;
                dr["PrecioVenta"] = precioVenta;
                dr["Cantidad"] = cantidad;
                dr["SubTotal"] = subtotal;
                dr["NombreProducto"] = nombreProducto;
                dr["PrecioCompraUsado"] = precioCompraUsado;
                dr["PrimaProducto"] = primaProducto;
                dr["NumeroCuotasProducto"] = numeroCuotasProducto;
                dr["FechaInicioCreditoProducto"] = tieneFechaInicio ? (object)fechaInicioCreditoProducto : DBNull.Value;

                detalle_venta.Rows.Add(dr);
            }

            // ==========================
            // IMPUESTO
            // ==========================
            decimal totalSinImpuesto = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                totalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }
            decimal totalISV = totalSinImpuesto * 0.15m;

            objVenta.Impuesto = totalISV;

            // ==========================
            // CORRELATIVO DOCUMENTO
            // ==========================
            int idcorrelativo = new CN_Venta().ObtenerCorrelativo();
            numeroDocumentoImpresion = string.Format("{0:00000000}", idcorrelativo);

            decimal porcentajeTarjeta = 0;
            decimal montoFinal = decimal.Parse(txttotalpagar.Text);

            if (comboMetodoPago.SelectedItem.ToString() == "Tarjeta")
            {
                string seleccion = cmbPorcentajeTarjeta.SelectedItem.ToString();
                porcentajeTarjeta = seleccion == "2.9%" ? 2.9m : 3.5m;
                decimal descuento = montoFinal * (porcentajeTarjeta / 100);
                montoFinal = montoFinal - descuento;
            }

            // ==========================
            // OBJETO VENTA PARA BD
            // ==========================
            Venta oVenta = new Venta
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumentoImpresion,
                DocumentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text,
                NombreCliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text,
                MetodoPago = objVenta.MetodoPago,
                MontoPago = objVenta.MontoPago,
                MontoCambio = objVenta.MontoCambio,
                MontoTotal = Math.Round(montoFinal, 2),
                Impuesto = objVenta.Impuesto,
                Condicion = cbocondicion.SelectedItem.ToString(),
                FechaVencimiento = objVenta.Condicion == "Credito" ? (DateTime?)dtpFechaVencimiento.Value : null,
                FechaRegistro = dtpFecha.Value
            };

            // ==========================
            // REGISTRAR VENTA
            // ==========================
            string mensaje = string.Empty;
            int idVentaGenerada = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);
            if (idVentaGenerada > 0 &&
    cbocondicion.SelectedItem != null &&
    cbocondicion.SelectedItem.ToString() == "Credito")
            {
                // numeroDocumentoImpresion es el que ya usás para la factura
                CrearCreditosPorProducto(numeroDocumentoImpresion);
            }
           

            // ==========================
            // MENSAJE / LIMPIEZA
            // ==========================
            if (idVentaGenerada > 0)
            {
                var result = MessageBox.Show("TRANSACCION EXITOSA\n\nNúmero de venta generada:\n" +
                                             numeroDocumentoImpresion +
                                             "\n\n¿Desea copiar al portapapeles?",
                                             "Mensaje",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumentoImpresion);

                txtdocumentocliente.Text = "";
                txtnombrecliente.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();
                txtpagocon.Text = "";
                txtcambio.Text = "";
                txtimpuesto.Text = "";
                comboMetodoPago.SelectedIndex = 0;
                cbocondicion.Text = "Contado";
                label14.Visible = false;
                dtpFechaVencimiento.Visible = false;
                cbotipodocumento.SelectedIndex = 0;
                cmbAplicarRecargo.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ImprimirFactura(bool esCopia)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = @"\\192.168.0.9\POS-80 (copy 1)".Normalize().Replace('\u00A0', ' ').Trim();
            printDoc.PrintPage += (s, e) => PrintPage(s, e, esCopia);

            // Asegúrate de que el formulario esté en primer plano
            this.Activate();

            // Imprimir directamente sin mostrar el PrintDialog
            try
            {
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }

            // Restaurar el formulario después de la impresión
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void ImprimirFacturaV(bool esCopia)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = @"\\192.168.0.9\POS-80 (copy 1)".Normalize().Replace('\u00A0', ' ').Trim();
            printDoc.PrintPage += (s, e) => PrintPagVSF(s, e, esCopia);

            // Asegúrate de que el formulario esté en primer plano
            this.Activate();

            // Imprimir directamente sin mostrar el PrintDialog
            try
            {
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }

            // Restaurar el formulario después de la impresión
            this.WindowState = FormWindowState.Normal;
            this.Activate();
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

        private void CrearCreditosPorProducto(string numeroDocumentoVenta)
        {
            if (cbocondicion.SelectedItem == null ||
                cbocondicion.SelectedItem.ToString() != "Credito")
                return;

            CN_Venta cnVenta = new CN_Venta();
            CN_CuotaCredito cnCuota = new CN_CuotaCredito();

            string docCliente = string.IsNullOrWhiteSpace(txtdocumentocliente.Text)
                ? "N/A"
                : txtdocumentocliente.Text.Trim();

            string nombreCliente = string.IsNullOrWhiteSpace(txtnombrecliente.Text)
                ? "CONSUMIDOR FINAL"
                : txtnombrecliente.Text.Trim();

            string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto;
            DateTime fechaRegistroVenta = dtpFecha.Value.Date;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["IdProducto"].Value == null)
                    continue;

                int numeroCuotas = 0;
                if (row.Cells["NumeroCuotasProducto"].Value == null ||
                    !int.TryParse(row.Cells["NumeroCuotasProducto"].Value.ToString(), out numeroCuotas) ||
                    numeroCuotas <= 0)
                {
                    continue;
                }

                decimal saldoPendienteProducto = 0m;
                if (row.Cells["SaldoPendienteProducto"].Value == null ||
                    !decimal.TryParse(row.Cells["SaldoPendienteProducto"].Value.ToString(), out saldoPendienteProducto) ||
                    saldoPendienteProducto <= 0)
                {
                    continue;
                }

                decimal primaProducto = 0m;
                if (row.Cells["PrimaProducto"].Value != null)
                    decimal.TryParse(row.Cells["PrimaProducto"].Value.ToString(), out primaProducto);

                if (primaProducto < 0) primaProducto = 0;

                decimal montoTotalProducto = saldoPendienteProducto + primaProducto;

                string frecuencia = row.Cells["FrecuenciaProducto"].Value != null
                    ? row.Cells["FrecuenciaProducto"].Value.ToString()
                    : "Mensual";

                if (string.IsNullOrWhiteSpace(frecuencia) || frecuencia == "N/A")
                    frecuencia = "Mensual";

                DateTime fechaInicio = fechaRegistroVenta;
                if (row.Cells["FechaInicioCreditoProducto"].Value != null)
                {
                    if (row.Cells["FechaInicioCreditoProducto"].Value is DateTime dtIni)
                    {
                        fechaInicio = dtIni.Date;
                    }
                    else
                    {
                        DateTime tmp;
                        if (DateTime.TryParse(row.Cells["FechaInicioCreditoProducto"].Value.ToString(), out tmp))
                            fechaInicio = tmp.Date;
                    }
                }

                DateTime fechaVencimiento;
                if (row.Cells["FechaVencimientoProducto"].Value != null &&
                    DateTime.TryParse(row.Cells["FechaVencimientoProducto"].Value.ToString(), out DateTime tmpVence))
                {
                    fechaVencimiento = tmpVence.Date;
                }
                else
                {
                    fechaVencimiento = cnCuota.CalcularFechaUltimoPago(fechaInicio, frecuencia, numeroCuotas);
                }

                string nombreProducto = row.Cells["Producto"].Value != null
                    ? row.Cells["Producto"].Value.ToString()
                    : "PRODUCTO";

                string descripcion = nombreProducto;

                Venta creditoFila = new Venta
                {
                    NumeroDocumento = numeroDocumentoVenta,
                    DocumentoCliente = docCliente,
                    NombreCliente = nombreCliente,
                    MontoTotal = montoTotalProducto,
                    SaldoPendiente = saldoPendienteProducto,
                    FechaVencimiento = fechaVencimiento,
                    MetodoPago = "N/A",
                    Condicion = "Credito",
                    FechaRegistro = fechaRegistroVenta,
                    TipoDocumento = tipoDocumento,
                    Descripcion = descripcion
                };

                string mensajeCredito;
                bool ok = cnVenta.RegistrarCredito(creditoFila, out mensajeCredito);
                if (!ok)
                {
                    MessageBox.Show(
                        "No se pudo registrar el crédito para el producto: " + nombreProducto + "\n" + mensajeCredito,
                        "Error al registrar crédito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    continue;
                }

                int idVentaCredito = cnVenta.ObtenerIdVentaPorNumeroDocumento(creditoFila.NumeroDocumento);
                if (idVentaCredito > 0)
                {
                    cnCuota.GenerarPlanCuotas(
                        idVentaCredito,
                        saldoPendienteProducto,
                        numeroCuotas,
                        frecuencia,
                        fechaInicio
                    );
                }
            }
        }



        private void comboMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboMetodoPago.SelectedItem != null)
            {
                string metodo = comboMetodoPago.SelectedItem.ToString();

                if (metodo == "Tarjeta")
                {
                    txtpagocon.Enabled = false;
                    txtcambio.Enabled = false;
                    txtpagocon.Text = "0";
                    txtcambio.Text = "0";

                    cmbPorcentajeTarjeta.Visible = true;
                    lblPorcentajeTarjeta.Visible = true;
                }
                else
                {
                    txtpagocon.Enabled = metodo == "Efectivo";
                    txtcambio.Enabled = metodo == "Efectivo";
                    txtpagocon.Text = metodo == "Efectivo" ? "" : "0";
                    txtcambio.Text = metodo == "Efectivo" ? "" : "0";

                    cmbPorcentajeTarjeta.Visible = false;
                    lblPorcentajeTarjeta.Visible = false;
                }
            }
        }

        private void cbotipodocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbotipodocumento.SelectedItem != null)
            {
                var selectedItem = (OpcionCombo)cbotipodocumento.SelectedItem;

                if (selectedItem.Valor == "Cotizacion")
                {
                    btncrearventa.Visible = false;
                    btnCrearCotizacion.Visible = true;
                    btnVentaSinFacturar.Visible = false;

                    comboMetodoPago.Enabled = false;
                    comboMetodoPago.Text = "";

                    txtpagocon.Enabled = false;
                    txtpagocon.Text = "0";
                    txtcambio.Enabled = false;
                    txtcambio.Text = "0";

                    txtoc.Enabled = false;
                    txtregexo.Enabled = false;
                    txtregsag.Enabled = false;
                    cbocondicion.Enabled = false;
                    cbocondicion.Text = "Contado";

                    dtpFechaVencimiento.Visible = false;
                    label14.Visible = false;

                    label12.Visible = true;
                    txtimpuesto.Visible = true;
                    label27.Visible = true;
                    txtsubtotal.Visible = true;
                    lblPagocon.Visible = false;
                    lblCambio.Visible = false;
                    txtpagocon.Visible = false;
                    txtcambio.Visible = false;
                    label20.Visible = true;
                    label25.Visible = true;
                    label22.Visible = false;
                    label21.Visible = false;
                    label32.Visible = false;
                  
                }
                else if (selectedItem.Valor == "VentaSinFacturar")
                {
                    btncrearventa.Visible = false;
                    btnCrearCotizacion.Visible = false;
                    btnVentaSinFacturar.Visible = true;

                    comboMetodoPago.Enabled = true;
                    comboMetodoPago.Text = "Efectivo";

                    txtpagocon.Enabled = true;
                    txtcambio.Enabled = true;

                    txtoc.Enabled = true;
                    txtregexo.Enabled = true;
                    txtregsag.Enabled = true;
                    cbocondicion.Enabled = true;

                    dtpFechaVencimiento.Visible = false;
                    label14.Visible = false;

                    label12.Visible = true;
                    txtimpuesto.Visible = true;
                    label27.Visible = true;
                    txtsubtotal.Visible = true;
                    lblPagocon.Visible = true;
                    lblCambio.Visible = true;
                    txtpagocon.Visible = true;
                    txtcambio.Visible = true;
                    label20.Visible = true;
                    label25.Visible = true;
                    label22.Visible = true;
                    label21.Visible = true;
                    label32.Visible = false;
                    label16.Visible = true;
                    label15.Visible = true;

                    txtimpuesto.Text = "0.00";
                    calcularTotal();
                }
                else
                {
                    btncrearventa.Visible = true;
                    btnCrearCotizacion.Visible = false;
                    btnVentaSinFacturar.Visible = false;

                    comboMetodoPago.Enabled = true;
                    comboMetodoPago.Text = "Efectivo";

                    txtpagocon.Enabled = true;
                    txtpagocon.Text = "";
                    txtcambio.Enabled = true;
                    txtcambio.Text = "";

                    txtoc.Enabled = true;
                    txtregexo.Enabled = true;
                    txtregsag.Enabled = true;
                    cbocondicion.Enabled = true;

                    dtpFechaVencimiento.Visible = false;
                    label14.Visible = false;

                    cmbNivelPrecio.Enabled = true;

                    label12.Visible = true;
                    txtimpuesto.Visible = true;
                    label27.Visible = true;
                    txtsubtotal.Visible = true;
                    lblPagocon.Visible = true;
                    lblCambio.Visible = true;
                    txtpagocon.Visible = true;
                    txtcambio.Visible = true;
                    label20.Visible = true;
                    label25.Visible = true;
                    label22.Visible = true;
                    label21.Visible = true;
                    label32.Visible = false;
                    label15.Visible = true;
                    label16.Visible = true;
                    
                }

                if (cmbNivelPrecio.Items.Count > 0 && cmbNivelPrecio.SelectedItem == null)
                {
                    cmbNivelPrecio.SelectedIndex = 0;
                }

                var nuevoTipo = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
                AjustarGridSegunCambioTipo(_ultimoTipoDocumentoValor, nuevoTipo);
                _ultimoTipoDocumentoValor = nuevoTipo;

                cbocondicion_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        
        private void btnCrearCotizacion_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la Cotizacion", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Crear objeto de venta
            Venta objVenta = new Venta
            {
                MetodoPago = "N/A", // Asignar "N/A" para cotización
                MontoTotal = decimal.Parse(txttotalpagar.Text),
                MontoPago = 0,
                MontoCambio = 0,
                Condicion = "Cotización" // Asignar un valor por defecto
            };

            // Crear DataTable para el detalle de la cotización
            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(decimal));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));
            detalle_venta.Columns.Add("NombreProducto", typeof(string));
            detalle_venta.Columns.Add("PrecioCompraUsado", typeof(decimal));

            // Llenar el DataTable con los datos de los productos
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["IdProducto"].Value != null)
                {
                    int idProducto = Convert.ToInt32(row.Cells["IdProducto"].Value);
                    decimal precioVenta = Convert.ToDecimal(row.Cells["Precio"].Value);
                    decimal cantidad = Convert.ToDecimal(row.Cells["Cantidad"].Value);
                    decimal subtotal = Convert.ToDecimal(row.Cells["SubTotal"].Value);
                    string nombreProducto = row.Cells["Producto"].Value.ToString();

                    Producto prod = listaProductos.FirstOrDefault(p => p.IdProducto == idProducto);
                    decimal precioCompraBase = prod != null ? prod.PrecioCompra : 0;

                    // ✅ Cálculo proporcional según cantidad vendida
                    decimal precioCompraUsado = Math.Round(precioCompraBase * cantidad, 2);

                    detalle_venta.Rows.Add(idProducto, precioVenta, cantidad, subtotal, nombreProducto, precioCompraUsado);
                }
            }


            // Obtener el correlativo para el número de documento
            int idcorrelativo = new CN_Venta().ObtenerCorrelativoCotizacion();
            string numeroDocumento = string.Format("{0:00000000}", idcorrelativo);

            // Crear el objeto de cotización para registrar
            Venta oVenta = new Venta()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumento,
                DocumentoCliente = txtdocumentocliente.Text,
                NombreCliente = txtnombrecliente.Text,
                MetodoPago = objVenta.MetodoPago,
                MontoPago = objVenta.MontoPago,
                MontoCambio = objVenta.MontoCambio,
                MontoTotal = objVenta.MontoTotal,
                 FechaRegistro = dtpFecha.Value
            };

            // Registrar la cotización
            string mensaje = string.Empty;
            int idVentaGenerada = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            // Manejo de respuesta
            if (idVentaGenerada > 0)
            {
                var result = MessageBox.Show("Número de Cotización generada:\n" + numeroDocumento + "\n\n¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumento);

                // Limpiar campos después de registrar la cotización
                txtdocumentocliente.Text = "";
                txtnombrecliente.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();
                txtpagocon.Text = "";
                txtcambio.Text = "";
                cbotipodocumento.SelectedIndex = 0;
                txtsubtotal.Text = "0.00";
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void cbocondicion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbocondicion.SelectedItem == null)
                return;

            string condicion = cbocondicion.SelectedItem.ToString();
            bool esCredito = condicion.Equals("Credito", StringComparison.OrdinalIgnoreCase);

            // ====== COMPORTAMIENTO DE CONTROLES (LO QUE YA TENÍAS) ======
            if (esCredito)
            {
                dtpFechaVencimiento.Visible = false;
                label14.Visible = false;
                comboMetodoPago.Enabled = false;
                txtpagocon.Enabled = false;
                txtcambio.Enabled = false;
                txtpagocon.Text = "0";
                txtcambio.Text = "0";
                comboMetodoPago.Text = "";
               
                cmbAplicarRecargo.SelectedIndex = 1;
            }
            else
            {
                dtpFechaVencimiento.Visible = false;
                label14.Visible = false;
                comboMetodoPago.Enabled = true;
                txtpagocon.Enabled = true;
                txtcambio.Enabled = true;
                txtpagocon.Text = "";
                txtcambio.Text = "";
                comboMetodoPago.Text = "Efectivo";
                
            }

            // ====== SINCRONIZAR LAS FILAS DEL GRID SEGÚN LA CONDICIÓN ======
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (!esCredito)
                {
                    // Pasar a CONTADO: ocultar datos de crédito visibles
                    if (dgvdata.Columns.Contains("Prima"))
                        row.Cells["Prima"].Value = "N/A";
                    if (dgvdata.Columns.Contains("NoCuotas"))
                        row.Cells["NoCuotas"].Value = "N/A";
                    if (dgvdata.Columns.Contains("Frecuencia"))
                        row.Cells["Frecuencia"].Value = "N/A";
                    if (dgvdata.Columns.Contains("FechaInicioCredito"))
                        row.Cells["FechaInicioCredito"].Value = "N/A";
                    if (dgvdata.Columns.Contains("FechaVence"))
                        row.Cells["FechaVence"].Value = "N/A";
                    if (dgvdata.Columns.Contains("Cuota"))
                        row.Cells["Cuota"].Value = "N/A";
                    if (dgvdata.Columns.Contains("Saldo"))
                        row.Cells["Saldo"].Value = "N/A";
                }
                else
                {
                    // Volver a CRÉDITO: recuperar desde las columnas internas *_Producto
                    decimal prima = 0m;
                    int nCuotas = 0;
                    DateTime fechaIni;

                    if (dgvdata.Columns.Contains("PrimaProducto") &&
                        row.Cells["PrimaProducto"].Value != null)
                    {
                        decimal.TryParse(row.Cells["PrimaProducto"].Value.ToString(), out prima);
                    }

                    if (dgvdata.Columns.Contains("NumeroCuotasProducto") &&
                        row.Cells["NumeroCuotasProducto"].Value != null)
                    {
                        int.TryParse(row.Cells["NumeroCuotasProducto"].Value.ToString(), out nCuotas);
                    }

                    if (!DateTime.TryParse(
                            dgvdata.Columns.Contains("FechaInicioCreditoProducto")
                                ? Convert.ToString(row.Cells["FechaInicioCreditoProducto"].Value)
                                : string.Empty,
                            out fechaIni))
                    {
                        // si no hay fecha guardada, usar la fecha de la venta
                        fechaIni = dtpFecha.Value;
                    }

                    if (prima > 0 || nCuotas > 0)
                    {
                        if (dgvdata.Columns.Contains("Prima"))
                            row.Cells["Prima"].Value = prima.ToString("0.00");

                        if (dgvdata.Columns.Contains("NoCuotas"))
                            row.Cells["NoCuotas"].Value = nCuotas > 0 ? nCuotas.ToString() : "1";

                        if (dgvdata.Columns.Contains("FechaInicioCredito"))
                            row.Cells["FechaInicioCredito"].Value = fechaIni.ToString("dd/MM/yyyy");
                        // Frecuencia / FechaVence / Cuota / Saldo quedan como ya las tenga la fila
                    }
                }
            }
        }

        private void btnVentaSinFacturar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cbocondicion.SelectedItem == null)
            {
                MessageBox.Show("Seleccione la condición de la venta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool esCredito = cbocondicion.SelectedItem.ToString() == "Credito";

            if (comboMetodoPago.SelectedIndex == -1 && !esCredito)
            {
                MessageBox.Show("Seleccione un método de pago.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txttotalpagar.Text))
            {
                MessageBox.Show("El monto total no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Venta objVenta = new Venta
            {
                MetodoPago = esCredito ? "N/A" : comboMetodoPago.SelectedItem.ToString(),
                MontoTotal = decimal.Parse(txttotalpagar.Text),
                Condicion = cbocondicion.SelectedItem.ToString()
            };

            if (esCredito)
            {
                objVenta.MontoPago = 0;
                objVenta.MontoCambio = 0;
            }
            else
            {
                if (objVenta.MetodoPago == "Efectivo")
                {
                    if (!EnsurePagoConEfectivo())
                        return;

                    objVenta.MontoPago = decimal.Parse(txtpagocon.Text);
                    objVenta.MontoCambio = objVenta.MontoPago - objVenta.MontoTotal;

                    if (objVenta.MontoCambio < 0)
                    {
                        MessageBox.Show("El monto pagado no puede ser menor al total.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (objVenta.MetodoPago == "Tarjeta" || objVenta.MetodoPago == "Transferencia")
                {
                    objVenta.MontoPago = 0;
                    objVenta.MontoCambio = 0;
                }
            }

            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(decimal));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));
            detalle_venta.Columns.Add("NombreProducto", typeof(string));
            detalle_venta.Columns.Add("PrecioCompraUsado", typeof(decimal));
            detalle_venta.Columns.Add("PrimaProducto", typeof(decimal));
            detalle_venta.Columns.Add("NumeroCuotasProducto", typeof(int));
            detalle_venta.Columns.Add("FechaInicioCreditoProducto", typeof(DateTime));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["IdProducto"].Value == null)
                    continue;

                int idProducto = Convert.ToInt32(row.Cells["IdProducto"].Value);
                decimal precioVenta = Convert.ToDecimal(row.Cells["Precio"].Value);
                decimal cantidad = Convert.ToDecimal(row.Cells["Cantidad"].Value);
                decimal subtotal = Convert.ToDecimal(row.Cells["SubTotal"].Value);

                string nombreProducto = row.Cells["Producto"].Value.ToString();

                Producto prod = listaProductos.FirstOrDefault(p => p.IdProducto == idProducto);
                decimal precioCompraBase = prod != null ? prod.PrecioCompra : 0m;
                decimal precioCompraUsado = Math.Round(precioCompraBase * cantidad, 2);

                decimal primaProducto = 0m;
                if (row.Cells["PrimaProducto"].Value != null)
                    decimal.TryParse(row.Cells["PrimaProducto"].Value.ToString(), out primaProducto);

                int numeroCuotasProducto = 0;
                if (row.Cells["NumeroCuotasProducto"].Value != null)
                    int.TryParse(row.Cells["NumeroCuotasProducto"].Value.ToString(), out numeroCuotasProducto);

                DateTime fechaInicioCreditoProducto = DateTime.MinValue;
                bool tieneFechaInicio = false;
                if (row.Cells["FechaInicioCreditoProducto"].Value != null &&
                    DateTime.TryParse(row.Cells["FechaInicioCreditoProducto"].Value.ToString(),
                                      out fechaInicioCreditoProducto))
                {
                    tieneFechaInicio = true;
                }

                DataRow dr = detalle_venta.NewRow();
                dr["IdProducto"] = idProducto;
                dr["PrecioVenta"] = precioVenta;
                dr["Cantidad"] = cantidad;
                dr["SubTotal"] = subtotal;
                dr["NombreProducto"] = nombreProducto;
                dr["PrecioCompraUsado"] = precioCompraUsado;
                dr["PrimaProducto"] = primaProducto;
                dr["NumeroCuotasProducto"] = numeroCuotasProducto;
                dr["FechaInicioCreditoProducto"] = tieneFechaInicio ? (object)fechaInicioCreditoProducto : DBNull.Value;

                detalle_venta.Rows.Add(dr);
            }

            decimal totalSinImpuesto = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                totalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }
            decimal totalISV = totalSinImpuesto * 0.15m;

            objVenta.Impuesto = totalISV;

            int idcorrelativo = new CN_Venta().ObtenerCorrelativoVentaSinFacturar();
            numeroDocumentoImpresion = string.Format("{0:00000000}", idcorrelativo);

            decimal porcentajeTarjeta = 0;
            decimal montoFinal = decimal.Parse(txttotalpagar.Text);

            if (comboMetodoPago.SelectedItem.ToString() == "Tarjeta")
            {
                string seleccion = cmbPorcentajeTarjeta.SelectedItem.ToString();
                porcentajeTarjeta = seleccion == "2.9%" ? 2.9m : 3.5m;
                decimal descuento = montoFinal * (porcentajeTarjeta / 100);
                montoFinal = montoFinal - descuento;
            }

            Venta oVenta = new Venta
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumentoImpresion,
                DocumentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text,
                NombreCliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text,
                MetodoPago = objVenta.MetodoPago,
                MontoPago = objVenta.MontoPago,
                MontoCambio = objVenta.MontoCambio,
                MontoTotal = Math.Round(montoFinal, 2),
                Impuesto = objVenta.Impuesto,
                Condicion = cbocondicion.SelectedItem.ToString(),
                FechaVencimiento = esCredito ? (DateTime?)dtpFechaVencimiento.Value : null,
                FechaRegistro = dtpFecha.Value
            };

            string mensaje = string.Empty;
            int idVentaGenerada = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            if (idVentaGenerada > 0 && esCredito)
            {
                CrearCreditosPorProducto(numeroDocumentoImpresion);
            }

            if (idVentaGenerada > 0)
            {
                var result = MessageBox.Show("TRANSACCION EXITOSA\n\nNúmero de venta generada:\n" +
                                             numeroDocumentoImpresion +
                                             "\n\n¿Desea copiar al portapapeles?",
                                             "Mensaje",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumentoImpresion);

                txtdocumentocliente.Text = "";
                txtnombrecliente.Text = "";
                dgvdata.Rows.Clear();
                calcularTotal();
                txtpagocon.Text = "";
                txtcambio.Text = "";
                txtimpuesto.Text = "";
                comboMetodoPago.SelectedIndex = 0;
                cbocondicion.Text = "Contado";
                label14.Visible = false;
                dtpFechaVencimiento.Visible = false;
                cbotipodocumento.SelectedIndex = 0;
                cmbAplicarRecargo.SelectedIndex = 1;
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

       
        private void LimpiarCampos()
        {
            txtdocumentocliente.Text = "";
            txtnombrecliente.Text = "";
            txtcodproducto.Text = "";
            txtproducto.Text = "";
            txtprecio.Text = "";
            txtstock.Text = "";
            txtcantidad.Value = 1;
            txtpagocon.Text = "";
            txtcambio.Text = "";
            txttotalpagar.Text = "0.00";
            dgvdata.Rows.Clear();
            comboMetodoPago.SelectedIndex = 0;
            cbocondicion.SelectedIndex = 0;
            dtpFechaVencimiento.Visible = false;
            cbotipodocumento.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Today;
            txtimpuesto.Text = "0.00";
            cmbAplicarRecargo.SelectedIndex = 1;
            txtsubtotal.Text = "0.00";
            txtIdCliente.Text = "0";
            label12.Visible = true;
            txtimpuesto.Visible = true;
            label27.Visible = true;
            txtsubtotal.Visible = true;
            lblPagocon.Visible = true;
            lblCambio.Visible = true;
            txtpagocon.Visible = true;
            txtcambio.Visible = true;
            label20.Visible = true;
            label25.Visible = true;
            label22.Visible = true;
            label21.Visible = true;
            label32.Visible = false; // Ocultar label de puntos
            
            label15.Visible = true;
            label16.Visible = true;
           
            cmbNivelPrecio.SelectedIndex = 0;

        }

        private void btnrefrescar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void frmVentas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {

                btnbuscarproducto.PerformClick();

                e.Handled = true;
            }

            if (e.KeyCode == Keys.F9)
            {

                btnbuscarproveedor.PerformClick();

                e.Handled = true;
            }

            if (e.KeyCode == Keys.F11)
            {

                btnagregarproducto.PerformClick();

                e.Handled = true;
            }



            if (e.KeyCode == Keys.F12)
            {

                if (btnVentaSinFacturar.Visible)
                {
                    btnVentaSinFacturar.PerformClick();
                }
                else if (btnCrearCotizacion.Visible)
                {
                    btnCrearCotizacion.PerformClick();
                }
                else if (btncrearventa.Visible) 
                {
                    btncrearventa.PerformClick();
                }



                    e.Handled = true;
            }
        }

        private void frmVentas_Activated(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void cmbAplicarRecargo_SelectedIndexChanged(object sender, EventArgs e)
        {

            calcularTotal();
        }


     

        private string[] SplitText(string text, int maxLength)
        {
            List<string> lines = new List<string>();
            for (int i = 0; i < text.Length; i += maxLength)
            {
                if (i + maxLength < text.Length)
                {
                    lines.Add(text.Substring(i, maxLength));
                }
                else
                {
                    lines.Add(text.Substring(i));
                }
            }
            return lines.ToArray();
        }


        public static class ConvertirNumeroALetras
        {
            private static readonly string[] Unidades = { "", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez", "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve", "veinte" };
            private static readonly string[] Decenas = { "", "", "veinte", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };
            private static readonly string[] Centenas = { "", "cien", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos" };
            private static readonly string[] Miles = { "", "mil", "millón", "mil millones", "billón" };

            public static string Convertir(decimal numero)
            {
                if (numero == 0)
                    return "cero";

                int enteros = (int)numero;
                int centavos = (int)((numero - enteros) * 100);

                string letras = "";

                // Manejar los millones
                if (enteros >= 1000000)
                {
                    int millones = enteros / 1000000;
                    letras += ConvertirMenosDeMillon(millones) + " " + Miles[2] + " ";
                    enteros %= 1000000;
                }

                // Manejar los miles
                if (enteros >= 1000)
                {
                    int miles = enteros / 1000;
                    if (miles == 1)
                        letras += "mil ";
                    else
                        letras += ConvertirMenosDeMillon(miles) + " mil ";
                    enteros %= 1000;
                }

                // Centenas
                if (enteros >= 100)
                {
                    int centenas = enteros / 100;
                    if (centenas == 1 && enteros == 100) // Para el 100 exacto
                        letras += "cien ";
                    else
                        letras += Centenas[centenas] + " ";
                    enteros %= 100;
                }

                // Decenas
                if (enteros >= 20)
                {
                    int decenas = enteros / 10;
                    letras += Decenas[decenas] + " ";
                    enteros %= 10;
                }

                // Unidades
                if (enteros > 0)
                {
                    letras += Unidades[enteros] + " ";
                }

                // Agregar "y" si es necesario
                if (letras.Contains("treinta") && enteros > 0)
                    letras = letras.Replace("treinta", "treinta y");
                else if (letras.Contains("cuarenta") && enteros > 0)
                    letras = letras.Replace("cuarenta", "cuarenta y");
                else if (letras.Contains("cincuenta") && enteros > 0)
                    letras = letras.Replace("cincuenta", "cincuenta y");
                else if (letras.Contains("sesenta") && enteros > 0)
                    letras = letras.Replace("sesenta", "sesenta y");
                else if (letras.Contains("setenta") && enteros > 0)
                    letras = letras.Replace("setenta", "setenta y");
                else if (letras.Contains("ochenta") && enteros > 0)
                    letras = letras.Replace("ochenta", "ochenta y");
                else if (letras.Contains("noventa") && enteros > 0)
                    letras = letras.Replace("noventa", "noventa y");

                letras = letras.Trim();
                return letras + " lempiras con " + centavos.ToString("D2") + "/100 centavos"; // Centavos con dos dígitos
            }

            private static string ConvertirMenosDeMillon(int numero)
            {
                string letras = "";

                // Centenas
                if (numero >= 100)
                {
                    int centenas = numero / 100;
                    if (centenas == 1 && numero == 100) // Para el 100 exacto
                        letras += "cien ";
                    else
                        letras += Centenas[centenas] + " ";
                    numero %= 100;
                }

                // Decenas
                if (numero >= 20)
                {
                    int decenas = numero / 10;
                    letras += Decenas[decenas] + " ";
                    numero %= 10;
                }

                // Unidades
                if (numero > 0)
                {
                    letras += Unidades[numero] + " ";
                }

                // Agregar "y" si es necesario
                if (letras.Contains("treinta") && numero > 0)
                    letras = letras.Replace("treinta", "treinta y");
                else if (letras.Contains("cuarenta") && numero > 0)
                    letras = letras.Replace("cuarenta", "cuarenta y");
                else if (letras.Contains("cincuenta") && numero > 0)
                    letras = letras.Replace("cincuenta", "cincuenta y");
                else if (letras.Contains("sesenta") && numero > 0)
                    letras = letras.Replace("sesenta", "sesenta y");
                else if (letras.Contains("setenta") && numero > 0)
                    letras = letras.Replace("setenta", "setenta y");
                else if (letras.Contains("ochenta") && numero > 0)
                    letras = letras.Replace("ochenta", "ochenta y");
                else if (letras.Contains("noventa") && numero > 0)
                    letras = letras.Replace("noventa", "noventa y");

                return letras.Trim();
            }
        }

        private void cmbNivelPrecio_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void cmbNivelPrecio_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void txtIdCliente_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void calcularTotalCanje()
        {
            int totalPuntos = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (int.TryParse(row.Cells["Precio"].Value.ToString(), out int precio) &&
                        decimal.TryParse(row.Cells["Cantidad"].Value.ToString(), out decimal cantidad))
                    {
                        totalPuntos += (int)(precio * cantidad);
                    }
                }
            }

            txtsubtotal.Text = "0.00";
            txtimpuesto.Text = "0.00";
            txttotalpagar.Text = totalPuntos.ToString(); // Mostrar total en puntos enteros
        }


        private void btnRealizarCanje_Click(object sender, EventArgs e)
        {
           
        }

        private void PrintPagVSF(object sender, PrintPageEventArgs e, bool esCopia)
        {
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txttotalpagar.Text));

            int pageWidth = 280; // Ancho de la impresora térmica
            Font fontNormal = new Font("MS Gothic", 8, FontStyle.Regular);
            Font fontBold = new Font("MS Gothic", 8, FontStyle.Bold);
            Font fontTitle = new Font("MS Gothic", 11, FontStyle.Regular);
            Font fontSubTitle = new Font("MS Gothic", 9, FontStyle.Regular);
            int y = 0;

            try
            {
                if (negocio.LogoBlancoNegro != null)
                {
                    using (MemoryStream ms = new MemoryStream(negocio.LogoBlancoNegro))
                    {
                        Image logo = Image.FromStream(ms);
                        int logoWidth = 300; // Ajusta según sea necesario
                        int logoHeight = 150; // Ajusta según sea necesario
                        e.Graphics.DrawImage(logo, (pageWidth - logoWidth) / 2, y, logoWidth, logoHeight);
                        y += logoHeight + 5; // Espacio después del logo
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el logo: " + ex.Message);
            }

            string nombreNegocio = negocio.Nombre.ToUpper();
            SizeF titleSize = e.Graphics.MeasureString(nombreNegocio, fontTitle);

            // Dibujar el nombre del negocio centrado
            float xNombre = (pageWidth - titleSize.Width) / 2;
            e.Graphics.DrawString(nombreNegocio, fontTitle, Brushes.Black, xNombre, y);

            // Avanzar la posición vertical para que no se monte con el eslogan
            y += (int)titleSize.Height + 5;

            // Esloganes
            string[] esloganes = { "Mejor Precio", "Mejor Pintura", "Interiores y Exteriores" };

            foreach (string slogan in esloganes)
            {
                SizeF sloganSize = e.Graphics.MeasureString(slogan, fontNormal);
                float xSlogan = (pageWidth - sloganSize.Width) / 2;
                e.Graphics.DrawString(slogan, fontNormal, Brushes.Black, xSlogan, y);
                y += (int)sloganSize.Height + 2; // Avanza un poco para la siguiente línea
            }

            string direccionCompleta = "Dirección: " + negocio.Direccion;
            string[] direccionLines = SplitText(direccionCompleta, 37); // Dividir la dirección en líneas de 32 caracteres
            foreach (string line in direccionLines)
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontNormal);
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14; // Reducí el espaciado entre líneas
            }

            // Imprimir RTN centrado
            string rtn = "RTN: " + negocio.RUC;
            string ciudad = "Danli, El Paraiso";
            SizeF rtnSize = e.Graphics.MeasureString(rtn, fontNormal);
            e.Graphics.DrawString(ciudad, fontNormal, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;
            e.Graphics.DrawString(rtn, fontNormal, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;
           

            // WhatsApp con ícono centrado
            Bitmap wtIcono = Properties.Resources.wtIcono;
            Bitmap fbIcono = Properties.Resources.fbIcono;
            Bitmap igIcono = Properties.Resources.igIcono;
            Bitmap ttIcono = Properties.Resources.ttIcono;

            int iconSize = 10;
            int iconYAdjust = 2;
            int textXOffset = 15;

            // Calcular posición centrada
            string whatsText = "Cel: " + negocio.Telefono + " / (+504) 3376-6564";

            SizeF whatsSize = e.Graphics.MeasureString(whatsText, fontNormal);
            float totalWhatsWidth = iconSize + textXOffset + whatsSize.Width;
            float startXWhats = (pageWidth - totalWhatsWidth) / 2;

            e.Graphics.DrawImage(wtIcono, startXWhats, y + iconYAdjust, iconSize, iconSize);
            e.Graphics.DrawString(whatsText, fontNormal, Brushes.Black, startXWhats + iconSize + textXOffset, y);
            y += 14;

            // Encabezado de redes sociales
            string redesHeader = "Síguenos en nuestras redes sociales:";
            SizeF redesHeaderSize = e.Graphics.MeasureString(redesHeader, fontNormal);
            e.Graphics.DrawString(redesHeader, fontNormal, Brushes.Black, (pageWidth - redesHeaderSize.Width) / 2, y);
            y += 14;

            // Lista de redes (centrado con íconos)
            void DibujarRedSocial(Bitmap icono, string texto)
            {
                SizeF textoSize = e.Graphics.MeasureString(texto, fontNormal);
                float totalWidth = iconSize + 4 + textoSize.Width; // 4 px de separación
                float startX = (pageWidth - totalWidth) / 2;
                float iconY = y + 1; // ajuste fino para que estén alineados

                e.Graphics.DrawImage(icono, startX, iconY, iconSize, iconSize);
                e.Graphics.DrawString(texto, fontNormal, Brushes.Black, startX + iconSize + 4, y);
                y += 14;
            }



            // Dibujar cada red
            DibujarRedSocial(fbIcono, "Facebook: American colors Honduras");
            DibujarRedSocial(igIcono, "Instagram: @american.colors33");
            DibujarRedSocial(ttIcono, "TikTok: @americancolololorshn");


            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información de la facturaKEY
            string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
            string cliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text;

            // Mostrar el tipo de documento en la impresión
            e.Graphics.DrawString("VENTA " + numeroDocumentoImpresion, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"), fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Atendido por: " + _Usuario.NombreCompleto, fontNormal, Brushes.Black, 10, y);
            y += 13;

            if (cbocondicion.Text == "Credito")
            {
                e.Graphics.DrawString("Condicion: " + cbocondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Fecha de vencimiento: " + dtpFechaVencimiento.Value.ToString("dd/MM/yyyy"), fontNormal, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }
            if (cbocondicion.Text == "Contado")
            {
                e.Graphics.DrawString("Condicion: " + cbocondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información del cliente
            int maxCaracteresPorLineaCliente = 45; // Límite de caracteres por línea para el cliente
            List<string> lineasCliente = new List<string>();

            // Agregar "Cliente: " a la primera línea
            string clienteTexto = "Cliente: " + cliente;
            for (int i = 0; i < clienteTexto.Length; i += maxCaracteresPorLineaCliente)
            {
                lineasCliente.Add(clienteTexto.Substring(i, Math.Min(maxCaracteresPorLineaCliente, clienteTexto.Length - i)));
            }

            // Imprimir la primera línea con "Cliente: "
            if (lineasCliente.Count > 0)
            {
                e.Graphics.DrawString(lineasCliente[0], fontNormal, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }

            // Imprimir las líneas restantes sin "Cliente: "
            for (int i = 1; i < lineasCliente.Count; i++)
            {
                e.Graphics.DrawString(lineasCliente[i], fontNormal, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }

            e.Graphics.DrawString("ID/RTN Cliente: " + documentoCliente, fontNormal, Brushes.Black, 10, y);
            y += 13;
          
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Detalle de producto
            e.Graphics.DrawString("CANT  DESCRIPCION                     IMPORTE", fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
            Font fontBoldProductos = new Font("MS Gothic", 7, FontStyle.Bold); // Fuente de productos
            Font fontBoldTotales = new Font("MS Gothic", 7, FontStyle.Bold); // Fuente de totales

            // Crear StringFormat para alinear los totales a la derecha
            StringFormat rightAlign = new StringFormat();
            rightAlign.Alignment = StringAlignment.Far;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                string cantidad = row.Cells["Cantidad"].Value.ToString();
                string producto = row.Cells["Producto"].Value.ToString();
                string precioUnitario = Convert.ToDecimal(row.Cells["Precio"].Value).ToString("0.00");
                string subtotal = "L. " + Convert.ToDecimal(row.Cells["SubTotal"].Value).ToString("0.00");

                int maxxCaracteresPorLinea = 35; // Límite de caracteres por línea
                int inicioDescripcion = 10;  // Posición inicial de la descripción
                List<string> lineasDescripcion = new List<string>();

                // Dividir la descripción en líneas de hasta 35 caracteres
                for (int i = 0; i < producto.Length; i += maxxCaracteresPorLinea)
                {
                    lineasDescripcion.Add(producto.Substring(i, Math.Min(maxxCaracteresPorLinea, producto.Length - i)));
                }

                // Imprimir cada línea de la descripción
                foreach (string linea in lineasDescripcion)
                {
                    e.Graphics.DrawString(linea, fontBoldProductos, Brushes.Black, inicioDescripcion, y);
                    y += 12;
                }

                // Segunda línea: Cantidad x Precio Unitario + Subtotal alineado a la derecha
                e.Graphics.DrawString($"{cantidad} x L. {precioUnitario}", fontBoldProductos, Brushes.Black, inicioDescripcion + 5, y);
                e.Graphics.DrawString(subtotal, fontBoldTotales, Brushes.Black, 275, y, rightAlign);
                y += 14; // Espaciado entre productos
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
            // Variables principales
            decimal montoTotal = decimal.Parse(txttotalpagar.Text);
            decimal pagoCon = string.IsNullOrEmpty(txtpagocon.Text) ? 0 : decimal.Parse(txtpagocon.Text);
            decimal cambio = pagoCon - montoTotal;

            // Parámetros de alineación (Nombres nuevos)
            int anchoPaginaTicket = e.PageBounds.Width;
            int limiteDerechoTicket = anchoPaginaTicket - 1;
            int anchoColumnaValoresTicket = 100;

            int xEncabezadoTicket = limiteDerechoTicket - anchoColumnaValoresTicket - 1;
            int xMonedaTicket = limiteDerechoTicket - 100;
            int xValorTicket = limiteDerechoTicket - 1;

            // Fuente en negrita (Nombre nuevo)
            Font fuenteNegritaTicket = new Font(fontNormal, FontStyle.Bold);

            // Función para imprimir textos alineados
            void ImprimirTextoAlineadoTicket(string texto, string valor, bool esTotal = false)
            {
                Font fuenteUsar = esTotal ? fuenteNegritaTicket : fontNormal;

                SizeF tamTexto = e.Graphics.MeasureString(texto, fuenteUsar);
                float xTexto = xEncabezadoTicket - tamTexto.Width;

                e.Graphics.DrawString(texto, fuenteUsar, Brushes.Black, xTexto, y);
                e.Graphics.DrawString("L.", fuenteUsar, Brushes.Black, xMonedaTicket, y);
                e.Graphics.DrawString(valor, fuenteUsar, Brushes.Black, xValorTicket, y, rightAlign);

                y += 13;
            }

            // Calcular valores
            decimal subtotalSinISV = 0;
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                subtotalSinISV += Convert.ToDecimal(fila.Cells["SubTotal"].Value);
            }
            decimal totalISV = subtotalSinISV * 0.15m;

            // Imprimir datos
            ImprimirTextoAlineadoTicket("SUBTOTAL:", subtotalSinISV.ToString("0.00"));
            ImprimirTextoAlineadoTicket("Descuentos:", "0.00");
            

            // Total a pagar (resaltado en negrita)
            ImprimirTextoAlineadoTicket("TOTAL A PAGAR:", txttotalpagar.Text, true);

            y += 18;


            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            Venta objVenta = new Venta();
            objVenta.MetodoPago = comboMetodoPago.SelectedItem.ToString();
            int maxCaracteresPorLinea = 40; // Límite de caracteres por línea
            List<string> lineasTotalEnLetras = new List<string>();

            for (int i = 0; i < totalEnLetras.Length; i += maxCaracteresPorLinea)
            {
                lineasTotalEnLetras.Add(totalEnLetras.Substring(i, Math.Min(maxCaracteresPorLinea, totalEnLetras.Length - i)));
            }

            // Imprimir la primera línea con "Son:"
            if (lineasTotalEnLetras.Count > 0)
            {
                e.Graphics.DrawString("Son: " + lineasTotalEnLetras[0], fontNormal, Brushes.Black, 10, y);
                y += 13;
            }

            // Imprimir las líneas restantes sin "Son:"
            for (int i = 1; i < lineasTotalEnLetras.Count; i++)
            {
                e.Graphics.DrawString(lineasTotalEnLetras[i], fontNormal, Brushes.Black, 10, y);
                y += 13;
            }
            string pagoConValor = Convert.ToDecimal(txtpagocon.Text).ToString("0.00");
            string cambioValor = Convert.ToDecimal(txtcambio.Text).ToString("0.00");

            if (cbocondicion.Text != "Credito")
            {
                // Si el método de pago es efectivo
                if (comboMetodoPago.Text == "Efectivo")
                {
                    string efectivoTexto = $"Efectivo:  L. {pagoConValor.PadLeft(10)}"; // Alinea el número a la derecha
                    string cambioTexto = $"Cambio:    L. {cambioValor.PadLeft(10)}";
                    e.Graphics.DrawString(efectivoTexto, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                    e.Graphics.DrawString(cambioTexto, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es tarjeta
                else if (comboMetodoPago.Text == "Tarjeta")
                {
                    e.Graphics.DrawString("Terminal POS: L." + txttotalpagar.Text, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es transferencia
                else if (comboMetodoPago.Text == "Transferencia")
                {
                    e.Graphics.DrawString("Transferencia", fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
            if (int.TryParse(txtIdCliente.Text, out int idClientePuntos) && cmbNivelPrecio.SelectedItem.ToString() == "Nivel 1")
            {
                int puntosAcumulados = (int)(decimal.Parse(txttotalpagar.Text) / 100) * 4;
               

              
            }
            e.Graphics.DrawString("¡GRACIAS POR SU COMPRA!", new Font("Lucida Console", 8, FontStyle.Bold), Brushes.Black, 10, y);

            y += 20; // Ajusta este valor para controlar el espaciado

            string etiqueta = esCopia ? "COPIA" : "ORIGINAL";
            Font fontEtiqueta = new Font("MS Gothic", 8, FontStyle.Bold);
            SizeF etiquetaSize = e.Graphics.MeasureString(etiqueta, fontEtiqueta);

            // Calcular la posición X para alinearlo completamente a la derecha
            float xEtiqueta = pageWidth - etiquetaSize.Width - 5; // Se resta un pequeño margen (5)

            // Usar la posición Y actual
            float yEtiqueta = y;

            // Dibuja la etiqueta alineada completamente a la derecha
            e.Graphics.DrawString(etiqueta, fontEtiqueta, Brushes.Black, xEtiqueta, yEtiqueta);
        }

        private void PrintPage(object sender, PrintPageEventArgs e, bool esCopia)
        {
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txttotalpagar.Text));

            int pageWidth = 280; // Ancho de la impresora térmica
            Font fontNormal = new Font("MS Gothic", 8, FontStyle.Regular);
            Font fontBold = new Font("MS Gothic", 8, FontStyle.Bold);
            Font fontTitle = new Font("MS Gothic", 11, FontStyle.Regular);
            Font fontSubTitle = new Font("MS Gothic", 9, FontStyle.Regular);
            int y = 0;

            try
            {
                if (negocio.LogoBlancoNegro != null)
                {
                    using (MemoryStream ms = new MemoryStream(negocio.LogoBlancoNegro))
                    {
                        Image logo = Image.FromStream(ms);
                        int logoWidth = 300; // Ajusta según sea necesario
                        int logoHeight = 150; // Ajusta según sea necesario
                        e.Graphics.DrawImage(logo, (pageWidth - logoWidth) / 2, y, logoWidth, logoHeight);
                        y += logoHeight + 3; // Espacio después del logo
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el logo: " + ex.Message);
            }

            string nombreNegocio = negocio.Nombre.ToUpper();
            SizeF titleSize = e.Graphics.MeasureString(nombreNegocio, fontTitle);

            // Dibujar el nombre del negocio centrado
            float xNombre = (pageWidth - titleSize.Width) / 2;
            e.Graphics.DrawString(nombreNegocio, fontTitle, Brushes.Black, xNombre, y);

            // Avanzar la posición vertical para que no se monte con el eslogan
            y += (int)titleSize.Height + 5;

            // Esloganes
            string[] esloganes = { "Mejor Precio", "Mejor Pintura", "Interiores y Exteriores" };

            foreach (string slogan in esloganes)
            {
                SizeF sloganSize = e.Graphics.MeasureString(slogan, fontNormal);
                float xSlogan = (pageWidth - sloganSize.Width) / 2;
                e.Graphics.DrawString(slogan, fontNormal, Brushes.Black, xSlogan, y);
                y += (int)sloganSize.Height + 2; // Avanza un poco para la siguiente línea
            }

            string direccionCompleta = "Dirección: " + negocio.Direccion;
            string[] direccionLines = SplitText(direccionCompleta, 37); // Dividir la dirección en líneas de 32 caracteres
            foreach (string line in direccionLines)
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontNormal);
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14; // Reducí el espaciado entre líneas
            }

            string ciudad = "Danli, El Paraiso";
            string rtn = "RTN: " + negocio.RUC;
            string cai = "CAI: " + negocio.CAI;
            SizeF rtnSize = e.Graphics.MeasureString(rtn, fontNormal);
            e.Graphics.DrawString(ciudad, fontNormal, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;
            e.Graphics.DrawString(rtn, fontNormal, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;
            e.Graphics.DrawString(cai, fontNormal, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;

            // WhatsApp con ícono centrado
            Bitmap wtIcono = Properties.Resources.wtIcono;
            Bitmap fbIcono = Properties.Resources.fbIcono;
            Bitmap igIcono = Properties.Resources.igIcono;
            Bitmap ttIcono = Properties.Resources.ttIcono;

            int iconSize = 10;
            int iconYAdjust = 2;
            int textXOffset = 15;

            // Calcular posición centrada
            string whatsText = "Cel: " + negocio.Telefono + " / (+504) 3376-6564";

            SizeF whatsSize = e.Graphics.MeasureString(whatsText, fontNormal);
            float totalWhatsWidth = iconSize + textXOffset + whatsSize.Width;
            float startXWhats = (pageWidth - totalWhatsWidth) / 2;

            e.Graphics.DrawImage(wtIcono, startXWhats, y + iconYAdjust, iconSize, iconSize);
            e.Graphics.DrawString(whatsText, fontNormal, Brushes.Black, startXWhats + iconSize + textXOffset, y);
            y += 14;

            // Encabezado de redes sociales
            string redesHeader = "Síguenos en nuestras redes sociales:";
            SizeF redesHeaderSize = e.Graphics.MeasureString(redesHeader, fontNormal);
            e.Graphics.DrawString(redesHeader, fontNormal, Brushes.Black, (pageWidth - redesHeaderSize.Width) / 2, y);
            y += 14;

            // Lista de redes (centrado con íconos)
            void DibujarRedSocial(Bitmap icono, string texto)
            {
                SizeF textoSize = e.Graphics.MeasureString(texto, fontNormal);
                float totalWidth = iconSize + 4 + textoSize.Width; // 4 px de separación
                float startX = (pageWidth - totalWidth) / 2;
                float iconY = y + 1; // ajuste fino para que estén alineados

                e.Graphics.DrawImage(icono, startX, iconY, iconSize, iconSize);
                e.Graphics.DrawString(texto, fontNormal, Brushes.Black, startX + iconSize + 4, y);
                y += 14;
            }



            // Dibujar cada red
            DibujarRedSocial(fbIcono, "Facebook: American colors Honduras");
            DibujarRedSocial(igIcono, "Instagram: @american.colors33");
            DibujarRedSocial(ttIcono, "TikTok: @americancolololorshn");
            y += 5;
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información de la factura
            string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
            string cliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text;

            // Mostrar el tipo de documento en la impresión
            e.Graphics.DrawString(tipoDocumento.ToUpper() + " " + negocio.RangoInicial + numeroDocumentoImpresion, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Fecha: " + dtpFecha.Value.ToString("dd/MM/yyyy HH:mm tt"), fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Atendido por: " + _Usuario.NombreCompleto, fontNormal, Brushes.Black, 10, y);
            y += 13;

            if (cbocondicion.Text == "Credito")
            {
                e.Graphics.DrawString("Condicion: " + cbocondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Fecha de vencimiento: " + dtpFechaVencimiento.Value.ToString("dd/MM/yyyy"), fontNormal, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }
            if (cbocondicion.Text == "Contado")
            {
                e.Graphics.DrawString("Condicion: " + cbocondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información del cliente
            int maxCaracteresPorLineaCliente = 45; // Límite de caracteres por línea para el cliente
            List<string> lineasCliente = new List<string>();

            // Agregar "Cliente: " a la primera línea
            string clienteTexto = "Cliente: " + cliente;
            for (int i = 0; i < clienteTexto.Length; i += maxCaracteresPorLineaCliente)
            {
                lineasCliente.Add(clienteTexto.Substring(i, Math.Min(maxCaracteresPorLineaCliente, clienteTexto.Length - i)));
            }

            // Imprimir la primera línea con "Cliente: "
            if (lineasCliente.Count > 0)
            {
                e.Graphics.DrawString(lineasCliente[0], fontNormal, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }

            // Imprimir las líneas restantes sin "Cliente: "
            for (int i = 1; i < lineasCliente.Count; i++)
            {
                e.Graphics.DrawString(lineasCliente[i], fontNormal, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }

            e.Graphics.DrawString("ID/RTN Cliente: " + documentoCliente, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("No.O/C exenta: " + txtoc.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("No.Reg de exonerado: " + txtregexo.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("No.De reg de la SAG: " + txtregsag.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Detalle de producto
            e.Graphics.DrawString("CANT  DESCRIPCION                     IMPORTE", fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
            Font fontBoldProductos = new Font("MS Gothic", 7, FontStyle.Bold); // Fuente de productos
            Font fontBoldTotales = new Font("MS Gothic", 7, FontStyle.Bold); // Fuente de totales

            // Crear StringFormat para alinear los totales a la derecha
            StringFormat rightAlign = new StringFormat();
            rightAlign.Alignment = StringAlignment.Far;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                string cantidad = row.Cells["Cantidad"].Value.ToString();
                string producto = row.Cells["Producto"].Value.ToString();
                string precioUnitario = Convert.ToDecimal(row.Cells["Precio"].Value).ToString("0.00");
                string subtotal = "L. " + Convert.ToDecimal(row.Cells["SubTotal"].Value).ToString("0.00");

                int maxxCaracteresPorLinea = 35; // Límite de caracteres por línea
                int inicioDescripcion = 10;  // Posición inicial de la descripción
                List<string> lineasDescripcion = new List<string>();

                // Dividir la descripción en líneas de hasta 35 caracteres
                for (int i = 0; i < producto.Length; i += maxxCaracteresPorLinea)
                {
                    lineasDescripcion.Add(producto.Substring(i, Math.Min(maxxCaracteresPorLinea, producto.Length - i)));
                }

                // Imprimir cada línea de la descripción
                foreach (string linea in lineasDescripcion)
                {
                    e.Graphics.DrawString(linea, fontBoldProductos, Brushes.Black, inicioDescripcion, y);
                    y += 12;
                }

                // Segunda línea: Cantidad x Precio Unitario + Subtotal alineado a la derecha
                e.Graphics.DrawString($"{cantidad} x L. {precioUnitario}", fontBoldProductos, Brushes.Black, inicioDescripcion + 5, y);
                e.Graphics.DrawString(subtotal, fontBoldTotales, Brushes.Black, 275, y, rightAlign);
                y += 14; // Espaciado entre productos
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
            // Variables principales
            decimal montoTotal = decimal.Parse(txttotalpagar.Text);
            decimal pagoCon = string.IsNullOrEmpty(txtpagocon.Text) ? 0 : decimal.Parse(txtpagocon.Text);
            decimal cambio = pagoCon - montoTotal;

            // Parámetros de alineación (Nombres nuevos)
            int anchoPaginaTicket = e.PageBounds.Width;
            int limiteDerechoTicket = anchoPaginaTicket - 1;
            int anchoColumnaValoresTicket = 100;

            int xEncabezadoTicket = limiteDerechoTicket - anchoColumnaValoresTicket - 1;
            int xMonedaTicket = limiteDerechoTicket - 100;
            int xValorTicket = limiteDerechoTicket - 1;

            // Fuente en negrita (Nombre nuevo)
            Font fuenteNegritaTicket = new Font(fontNormal, FontStyle.Bold);

            // Función para imprimir textos alineados
            void ImprimirTextoAlineadoTicket(string texto, string valor, bool esTotal = false)
            {
                Font fuenteUsar = esTotal ? fuenteNegritaTicket : fontNormal;

                SizeF tamTexto = e.Graphics.MeasureString(texto, fuenteUsar);
                float xTexto = xEncabezadoTicket - tamTexto.Width;

                e.Graphics.DrawString(texto, fuenteUsar, Brushes.Black, xTexto, y);
                e.Graphics.DrawString("L.", fuenteUsar, Brushes.Black, xMonedaTicket, y);
                e.Graphics.DrawString(valor, fuenteUsar, Brushes.Black, xValorTicket, y, rightAlign);

                y += 13;
            }

            // Calcular valores
            decimal subtotalSinISV = 0;
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                subtotalSinISV += Convert.ToDecimal(fila.Cells["SubTotal"].Value);
            }
            decimal totalISV = subtotalSinISV * 0.15m;

            // Imprimir datos
            ImprimirTextoAlineadoTicket("SUBTOTAL:", subtotalSinISV.ToString("0.00"));
            ImprimirTextoAlineadoTicket("Descuentos:", "0.00");
            ImprimirTextoAlineadoTicket("Importe Exento:", "0.00");
            ImprimirTextoAlineadoTicket("Importe Exonerado:", "0.00");
            ImprimirTextoAlineadoTicket("Gravado 15%:", subtotalSinISV.ToString("0.00"));
            ImprimirTextoAlineadoTicket("Gravado 18%:", "0.00");
            ImprimirTextoAlineadoTicket("ISV 15%:", txtimpuesto.Text);
            ImprimirTextoAlineadoTicket("ISV 18%:", "0.00");

            // Total a pagar (resaltado en negrita)
            ImprimirTextoAlineadoTicket("TOTAL A PAGAR:", txttotalpagar.Text, true);

            y += 18;


            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            Venta objVenta = new Venta();
            objVenta.MetodoPago = comboMetodoPago.SelectedItem.ToString();
            int maxCaracteresPorLinea = 40; // Límite de caracteres por línea
            List<string> lineasTotalEnLetras = new List<string>();

            for (int i = 0; i < totalEnLetras.Length; i += maxCaracteresPorLinea)
            {
                lineasTotalEnLetras.Add(totalEnLetras.Substring(i, Math.Min(maxCaracteresPorLinea, totalEnLetras.Length - i)));
            }

            // Imprimir la primera línea con "Son:"
            if (lineasTotalEnLetras.Count > 0)
            {
                e.Graphics.DrawString("Son: " + lineasTotalEnLetras[0], fontNormal, Brushes.Black, 10, y);
                y += 13;
            }

            // Imprimir las líneas restantes sin "Son:"
            for (int i = 1; i < lineasTotalEnLetras.Count; i++)
            {
                e.Graphics.DrawString(lineasTotalEnLetras[i], fontNormal, Brushes.Black, 10, y);
                y += 13;
            }
            string pagoConValor = Convert.ToDecimal(txtpagocon.Text).ToString("0.00");
            string cambioValor = Convert.ToDecimal(txtcambio.Text).ToString("0.00");

            if (cbocondicion.Text != "Credito")
            {
                // Si el método de pago es efectivo
                if (comboMetodoPago.Text == "Efectivo")
                {
                    string efectivoTexto = $"Efectivo:  L. {pagoConValor.PadLeft(10)}"; // Alinea el número a la derecha
                    string cambioTexto = $"Cambio:    L. {cambioValor.PadLeft(10)}";
                    e.Graphics.DrawString(efectivoTexto, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                    e.Graphics.DrawString(cambioTexto, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es tarjeta
                else if (comboMetodoPago.Text == "Tarjeta")
                {
                    e.Graphics.DrawString("Terminal POS: L." + txttotalpagar.Text, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es transferencia
                else if (comboMetodoPago.Text == "Transferencia")
                {
                    e.Graphics.DrawString("Transferencia", fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
            }

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
            string rangoAutorizado = "Rango Aut.: " + negocio.RangoAutorizado; // Reemplaza con el valor real
            string fechaLimite = "Fecha Límite: " + negocio.FechaLimiteEmision.ToString("dd/MM/yyyy"); // Reemplaza con el valor real

            // Dividir el rango autorizado en dos líneas
            string[] rangoLines = SplitText(rangoAutorizado, 32);
            foreach (string line in rangoLines)
            {
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, 10, y);
                y += 13;
            }

            // Imprimir la fecha límite
            e.Graphics.DrawString(fechaLimite, fontNormal, Brushes.Black, 10, y);
            y += 15;

            e.Graphics.DrawString("ORIGINAL: CLIENTE", fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("COPIA: OBLIGADO TRIBUTARIO EMISOR", fontNormal, Brushes.Black, 10, y);
            y += 15;

            // Mensaje final
            string mensajeFinal = "LA FACTURA ES BENEFICIO DE TODOS, ¡EXÍJALA!";
            SizeF mensajeSize = e.Graphics.MeasureString(mensajeFinal, fontNormal);
            e.Graphics.DrawString(mensajeFinal, fontBold, Brushes.Black, (pageWidth - mensajeSize.Width) / 2, y);
            y += 20;

            e.Graphics.DrawString("¡GRACIAS POR SU COMPRA!", new Font("Lucida Console", 8, FontStyle.Bold), Brushes.Black, 10, y);

            y += 20; // Ajusta este valor para controlar el espaciado

            string etiqueta = esCopia ? "COPIA" : "ORIGINAL";
            Font fontEtiqueta = new Font("MS Gothic", 8, FontStyle.Bold);
            SizeF etiquetaSize = e.Graphics.MeasureString(etiqueta, fontEtiqueta);

            // Calcular la posición X para alinearlo completamente a la derecha
            float xEtiqueta = pageWidth - etiquetaSize.Width - 5; // Se resta un pequeño margen (5)

            // Usar la posición Y actual
            float yEtiqueta = y;

            // Dibuja la etiqueta alineada completamente a la derecha
            e.Graphics.DrawString(etiqueta, fontEtiqueta, Brushes.Black, xEtiqueta, yEtiqueta);
        }



        private void frmVentas_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }
       
        private void txtprecio_Enter(object sender, EventArgs e)
        {
            if (_bloqueandoPrecio || permisoModificarPrecio)
                return;

            _bloqueandoPrecio = true; // activamos el bloqueo

            bool requiereClave = true;

            if (_Usuario != null && _Usuario.oRol != null && _Usuario.oRol.Descripcion == "Administrador")
            {
                requiereClave = false;
            }

            if (requiereClave)
            {
                using (MdAcceso claveAccesoForm = new MdAcceso())
                {
                    var resultado = claveAccesoForm.ShowDialog();

                    if (resultado == DialogResult.OK)
                    {
                        string claveCorrecta = new CN_ClaveAcceso().ObtenerClave("ModificarPrecio");

                        if (claveAccesoForm.ClaveIngresada == claveCorrecta)
                        {
                            permisoModificarPrecio = true;
                            txtprecio.ReadOnly = false;
                        }
                        else
                        {
                            MessageBox.Show("Clave de acceso incorrecta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            permisoModificarPrecio = false;
                            txtprecio.ReadOnly = true;
                            MoverFocoSeguro();
                        }
                    }
                    else
                    {
                        permisoModificarPrecio = false;
                        txtprecio.ReadOnly = true;
                        MoverFocoSeguro();
                    }
                }
            }
            else
            {
                permisoModificarPrecio = true;
                txtprecio.ReadOnly = false;
            }

            _bloqueandoPrecio = false;
        }

        private void txtprecio_Leave(object sender, EventArgs e)
        {
            permisoModificarPrecio = false;
            txtprecio.ReadOnly = true;
        }

        private void MoverFocoSeguro()
        {
            
            this.ActiveControl = txtcodproducto;

        }

        private void dgvdata_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "Precio")
            {
                // Verificar si el usuario tiene permiso o es administrador
                int[] usuariosPermitidos = { 25, 40, 41 };

                bool tienePermiso = usuariosPermitidos.Contains(_Usuario.IdUsuario) ||
                                    (_Usuario.oRol != null && _Usuario.oRol.Descripcion == "Administrador");

                if (!tienePermiso)
                {
                    // Mostrar formulario para ingresar clave
                    using (MdAcceso claveAccesoForm = new MdAcceso())
                    {
                        var resultado = claveAccesoForm.ShowDialog();

                        if (resultado == DialogResult.OK)
                        {
                            string claveCorrecta = new CN_ClaveAcceso().ObtenerClave("ModificarPrecio");

                            if (claveAccesoForm.ClaveIngresada != claveCorrecta)
                            {
                                MessageBox.Show("Clave incorrecta. No se puede modificar el precio.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                e.Cancel = true;
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
            else
            {
                // Cancelar edición en otras columnas
                e.Cancel = true;
            }
        }

        private void dgvdata_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "Precio")
            {
                DataGridViewRow row = dgvdata.Rows[e.RowIndex];

                if (decimal.TryParse(row.Cells["Precio"].Value?.ToString(), out decimal nuevoPrecio) &&
                    decimal.TryParse(row.Cells["Cantidad"].Value?.ToString(), out decimal cantidad))
                {
                    // ✅ Reasignar el precio formateado a dos decimales
                    row.Cells["Precio"].Value = nuevoPrecio.ToString("0.00");

                    // Calcular nuevo subtotal
                    row.Cells["SubTotal"].Value = (nuevoPrecio * cantidad).ToString("0.00");

                    // Recalcular totales generales
                    calcularTotal();
                }
                else
                {
                    MessageBox.Show("Formato de precio no válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["Precio"].Value = "0.00";
                }
            }
        }

        private void cmbPorcentajeTarjeta_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtPrimaProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;

            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }



        private void AbrirModalCreditoParaFila(DataGridViewRow row, decimal precioBaseProducto, decimal primaDefecto, bool esEdicion)
        {
            try
            {
                using (var modal = new md_ProductoCredito())
                {
                    decimal cantidad = 1m;
                    if (row.Cells["Cantidad"].Value != null)
                        decimal.TryParse(row.Cells["Cantidad"].Value.ToString(), out cantidad);
                    if (cantidad <= 0) cantidad = 1m;

                    decimal precioTotal = precioBaseProducto;
                    if (precioTotal <= 0 && row.Cells["SubTotal"].Value != null)
                        decimal.TryParse(row.Cells["SubTotal"].Value.ToString(), out precioTotal);

                    int numeroCuotas = 0;
                    string frecuencia = "Mensual";
                    DateTime fechaInicio = dtpFecha.Value.Date;
                    DateTime fechaVencimiento = DateTime.MinValue;
                    decimal saldoPendienteInicial = 0m;
                    decimal montoCuotaInicial = 0m;

                    if (esEdicion)
                    {
                        if (row.Cells["NumeroCuotasProducto"].Value != null)
                            int.TryParse(row.Cells["NumeroCuotasProducto"].Value.ToString(), out numeroCuotas);

                        if (row.Cells["FrecuenciaProducto"].Value != null)
                            frecuencia = row.Cells["FrecuenciaProducto"].Value.ToString();

                        if (row.Cells["FechaInicioCreditoProducto"].Value != null &&
                            DateTime.TryParse(row.Cells["FechaInicioCreditoProducto"].Value.ToString(), out DateTime fIni))
                            fechaInicio = fIni;

                        if (row.Cells["PrimaProducto"].Value != null &&
                            decimal.TryParse(row.Cells["PrimaProducto"].Value.ToString(), out decimal primaGuardada))
                            primaDefecto = primaGuardada;

                        if (row.Cells["SaldoPendienteProducto"].Value != null &&
                            decimal.TryParse(row.Cells["SaldoPendienteProducto"].Value.ToString(), out decimal saldoGuardado))
                            saldoPendienteInicial = saldoGuardado;

                        if (row.Cells["FechaVencimientoProducto"].Value != null &&
                            DateTime.TryParse(row.Cells["FechaVencimientoProducto"].Value.ToString(), out DateTime fVence))
                            fechaVencimiento = fVence;

                        if (row.Cells["MontoCuotaProducto"].Value != null &&
                            decimal.TryParse(row.Cells["MontoCuotaProducto"].Value.ToString(), out decimal cuotaGuardada))
                            montoCuotaInicial = cuotaGuardada;
                    }

                    if (numeroCuotas <= 0) numeroCuotas = 1;
                    if (string.IsNullOrWhiteSpace(frecuencia)) frecuencia = "Mensual";
                    if (fechaInicio == DateTime.MinValue) fechaInicio = dtpFecha.Value.Date;
                    if (saldoPendienteInicial <= 0)
                        saldoPendienteInicial = precioTotal - primaDefecto;
                    if (saldoPendienteInicial < 0) saldoPendienteInicial = 0;

                    modal.PrecioInicial = precioTotal;
                    modal.PrimaInicial = primaDefecto;
                    modal.SaldoPendienteInicial = saldoPendienteInicial;
                    modal.NumeroCuotasInicial = numeroCuotas;
                    modal.FrecuenciaInicial = frecuencia;
                    modal.FechaInicioInicial = fechaInicio;
                    modal.FechaVencimientoInicial = fechaVencimiento;
                    modal.MontoCuotaInicial = montoCuotaInicial;

                    if (modal.ShowDialog() == DialogResult.OK)
                    {
                        row.Cells["PrimaProducto"].Value = modal.Prima;
                        row.Cells["NumeroCuotasProducto"].Value = modal.NumeroCuotas;
                        row.Cells["FrecuenciaProducto"].Value = modal.Frecuencia;
                        row.Cells["FechaInicioCreditoProducto"].Value = modal.FechaInicioCredito;
                        row.Cells["FechaVencimientoProducto"].Value = modal.FechaVencimiento;
                        row.Cells["SaldoPendienteProducto"].Value = modal.SaldoPendiente;
                        row.Cells["MontoCuotaProducto"].Value = modal.MontoCuota;

                        if (dgvdata.Columns.Contains("Prima"))
                            row.Cells["Prima"].Value = modal.Prima.ToString("0.00");
                        if (dgvdata.Columns.Contains("NoCuotas"))
                            row.Cells["NoCuotas"].Value = modal.NumeroCuotas.ToString();
                        if (dgvdata.Columns.Contains("Frecuencia"))
                            row.Cells["Frecuencia"].Value = modal.Frecuencia;
                        if (dgvdata.Columns.Contains("FechaInicioCredito"))
                            row.Cells["FechaInicioCredito"].Value = modal.FechaInicioCredito.ToString("dd/MM/yyyy");
                        if (dgvdata.Columns.Contains("FechaVence"))
                            row.Cells["FechaVence"].Value = modal.FechaVencimiento.ToString("dd/MM/yyyy");
                        if (dgvdata.Columns.Contains("Cuota"))
                            row.Cells["Cuota"].Value = modal.MontoCuota.ToString("0.00");
                        if (dgvdata.Columns.Contains("Saldo"))
                            row.Cells["Saldo"].Value = modal.SaldoPendiente.ToString("0.00");

                        string tipoDocLinea = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
                        bool conISVSeparado = tipoDocLinea == "Factura" || tipoDocLinea == "Boleta" || tipoDocLinea == "Cotizacion";

                        decimal cantidadFila = 1m;
                        if (row.Cells["Cantidad"].Value != null)
                            decimal.TryParse(row.Cells["Cantidad"].Value.ToString(), out cantidadFila);
                        if (cantidadFila <= 0) cantidadFila = 1m;

                        decimal totalCredito = modal.Precio;
                        decimal nuevoSubTotal;
                        decimal nuevoPrecio;

                        if (conISVSeparado)
                        {
                            nuevoSubTotal = Math.Round(totalCredito / 1.15m, 2, MidpointRounding.AwayFromZero);
                            nuevoPrecio = Math.Round(nuevoSubTotal / cantidadFila, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            nuevoSubTotal = Math.Round(totalCredito, 2, MidpointRounding.AwayFromZero);
                            nuevoPrecio = Math.Round(nuevoSubTotal / cantidadFila, 2, MidpointRounding.AwayFromZero);
                        }

                        row.Cells["Precio"].Value = nuevoPrecio.ToString("0.00");
                        row.Cells["SubTotal"].Value = nuevoSubTotal.ToString("0.00");

                        calcularTotal();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al configurar el crédito del producto:\n" + ex.Message,
                                "Error Crédito", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvdata_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgvdata.Columns[e.ColumnIndex].Name;

            if (colName == "Precio" ||
                colName == "SubTotal" ||
                colName == "PrimaProducto" ||
                colName == "MontoCuotaProducto" ||
                colName == "SaldoPendienteProducto")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal valor))
                {
                    e.Value = "L. " + valor.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }
    }

}
