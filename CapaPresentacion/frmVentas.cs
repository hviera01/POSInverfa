using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using ESCPOS_NET;
using ESCPOS_NET.Utilities;
using ESCPOS_NET.Printers;
using System.IO.Ports;
using System.Drawing.Printing;



namespace CapaPresentacion
{
    public partial class frmVentas : Form
    {

        private Usuario _Usuario;

        private Negocio negocio;
        public frmVentas(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            negocio = new CN_Negocio().ObtenerDatos();

        }
        private string numeroDocumentoImpresion;
        private void frmVentas_Load(object sender, EventArgs e)
        {
             comboMetodoPago.Items.Add("Efectivo");
            comboMetodoPago.Items.Add("Tarjeta");
            comboMetodoPago.Items.Add("Transferencia");
            comboMetodoPago.SelectedIndex = 0;
            cbocondicion.Items.Add("Contado");
            cbocondicion.Items.Add("Credito");
            cbocondicion.SelectedIndex = 0;
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Cotizacion", Texto = "Cotizacion" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "VentaSinFacturar", Texto = "Venta Sin" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;

            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
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

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
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
                    txtprecio.Text = modal._Producto.PrecioVenta.ToString("0.00");
                    txtstock.Text = modal._Producto.Stock.ToString();
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
            if (e.KeyData == Keys.Enter)
            {

                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtcodproducto.Text && p.Estado == true).FirstOrDefault();

                if (oProducto != null)
                {
                    txtcodproducto.BackColor = Color.Honeydew;
                    txtidproducto.Text = oProducto.IdProducto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtprecio.Text = oProducto.PrecioVenta.ToString("0.00");
                    txtstock.Text = oProducto.Stock.ToString();
                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.BackColor = Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                    txtprecio.Text = "";
                    txtstock.Text = "";
                    txtcantidad.Value = 1;
                }
            }
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            decimal precioConImpuesto = 0;
            decimal precioSinImpuesto = 0;
            bool producto_existe = false;

            // Verificar si se ha seleccionado un producto
            if (int.TryParse(txtidproducto.Text, out int idProducto) && idProducto == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Validar el precio
            if (!decimal.TryParse(txtprecio.Text, out precioConImpuesto))
            {
                MessageBox.Show("Precio - Formato moneda incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtprecio.Select();
                return;
            }

            // Calcular el precio sin impuesto
            precioSinImpuesto = precioConImpuesto / 1.15m;

            // Validar el stock y la cantidad
            if (!decimal.TryParse(txtstock.Text, out decimal stock))
            {
                MessageBox.Show("Stock - Formato incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtstock.Select();
                return;
            }

            if (!decimal.TryParse(txtcantidad.Value.ToString(), out decimal cantidad))
            {
                MessageBox.Show("Cantidad - Formato incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtcantidad.Select();
                return;
            }

            // Concatenar el color al nombre del producto si se ha ingresado
            string color = txtcolor.Text.Trim();
            string nombreProducto = txtproducto.Text;
            if (!string.IsNullOrEmpty(color))
            {
                nombreProducto += $" color {color}";
            }

            int idProductoFinal = idProducto; // Inicializamos con el ID del producto original

            // Verificar si el producto es de la categoría 35 (Pintura Preparada)
            if (txtIdCategoria.Text == "35")
            {
                // Agregar el producto sin importar el stock
                MessageBox.Show("Recuerde registrar las pinturas que utilizó.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (stock < cantidad)
            {
                // Si el stock es insuficiente, mostrar mensaje
                MessageBox.Show($"El producto {txtproducto.Text} no tiene suficiente stock.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Preguntar si desea hacer un reembasado
                DialogResult result = MessageBox.Show("¿Desea realizar un reembasado?", "Reembasado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Opción "Reembasar"
                    using (var modal = new mdProducto())
                    {
                        var modalResult = modal.ShowDialog();
                        if (modalResult == DialogResult.OK)
                        {
                            // Obtener el ID del producto seleccionado para reembasado
                            idProductoFinal = modal._Producto.IdProducto;
                            decimal cantidadReembasada = cantidad;

                            // Obtener los datos del producto reembasado
                            Producto productoReembasado = new CN_Producto().Listar().FirstOrDefault(p => p.IdProducto == idProductoFinal);
                            if (productoReembasado != null && productoReembasado.Stock >= cantidadReembasada)
                            {
                                // Restar el stock temporalmente del producto reembasado
                                productoReembasado.Stock -= cantidadReembasada;
                            }
                            else
                            {
                                MessageBox.Show("El producto seleccionado para reembasado no tiene suficiente stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return; // Salir si el producto reembasado no tiene stock suficiente
                            }
                        }
                        else
                        {
                            return; // Salir si no se selecciona un producto alternativo
                        }
                    }
                }
                else
                {
                    return; // Salir si el usuario elige "No"
                }
            }

            // Verificar si el producto ya existe en el DataGridView
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["IdProducto"].Value.ToString() == idProductoFinal.ToString())
                {
                    producto_existe = true;
                    break;
                }
            }

            // Si el producto no existe en la lista, agregarlo con el ID final (reembasado o no)
            if (!producto_existe)
            {
                decimal subtotalSinImpuesto;

                // Verificar si es un reembasado
                if (cantidad < 1)
                {
                    // Si la cantidad es menor a 1, calcular el precio multiplicando por el factor de reembasado
                    subtotalSinImpuesto = precioSinImpuesto * (cantidad * 4);
                }
                else
                {
                    // Si es cantidad normal, hacer la multiplicación estándar
                    subtotalSinImpuesto = precioSinImpuesto * cantidad;
                }

                // Agregar el producto al DataGridView
                dgvdata.Rows.Add(new object[] {
                idProductoFinal.ToString(),
                txtIdCategoria.Text,
                nombreProducto, // Usar el nombre del producto con el color
                precioSinImpuesto.ToString("0.00"),
                cantidad.ToString("0.00"),
                subtotalSinImpuesto.ToString("0.00")
               });

                // Si se seleccionó "Pintura Preparada", no restamos stock
                if (txtIdCategoria.Text != "35" && stock >= cantidad)
                {
                    // Restar el stock de manera temporal en la venta (sin afectar la base de datos)
                    RestarStockDeVenta(idProductoFinal, cantidad);
                }

                // Calcular el total
                calcularTotal();
                limpiarProducto();
                txtcodproducto.Select();
            }
        }

        // Método para restar el stock de venta temporalmente
        private void RestarStockDeVenta(int idProducto, decimal cantidad)
        {
           
            Producto producto = new CN_Producto().Listar().FirstOrDefault(p => p.IdProducto == idProducto);
            if (producto != null)
            {
                producto.Stock -= cantidad;
            }
        }

        private void calcularTotal()
        {
            decimal totalSinImpuesto = 0;
            decimal totalISV = 0;

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    totalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString());
                }
            }

         
            totalISV = totalSinImpuesto * 0.15m; 

          
            decimal totalPagar = totalSinImpuesto + totalISV; 

           
            totalPagar = Math.Round(totalPagar, 2, MidpointRounding.ToEven); 
          
            if (totalPagar % 1 >= 0.90m) 
            {
                totalPagar = Math.Ceiling(totalPagar); // Redondear hacia arriba
            }
            else
            {
                totalPagar = Math.Floor(totalPagar); // Redondear hacia abajo a un entero
            }

            // Mostrar siempre dos decimales
            txttotalpagar.Text = totalPagar.ToString("0.00"); // Asegurarse de que se muestre con dos decimales
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
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.delete25.Width;
                var h = Properties.Resources.delete25.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.delete25, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btneliminar")
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    // Simplemente eliminar la fila del DataGridView sin restar stock
                    dgvdata.Rows.RemoveAt(index);
                }
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

        private void txtpagocon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                calcularcambio(); // Llama al método para calcular el cambio
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

            // Crear objeto de venta
            Venta objVenta = new Venta
            {
                MetodoPago = cbocondicion.SelectedItem.ToString() == "Credito" ? "N/A" : comboMetodoPago.SelectedItem.ToString(),
                MontoTotal = decimal.Parse(txttotalpagar.Text),
                Condicion = cbocondicion.SelectedItem.ToString()
            };

            // Si la condición es "Crédito", no se requiere monto pagado ni cambio
            if (objVenta.Condicion == "Credito")
            {
                objVenta.MontoPago = 0; // No se requiere monto pagado
                objVenta.MontoCambio = 0; // No se requiere cambio
            }
            else
            {
                // Verificar el método de pago y calcular el cambio si es efectivo
                if (objVenta.MetodoPago == "Efectivo")
                {
                    if (string.IsNullOrEmpty(txtpagocon.Text))
                    {
                        MessageBox.Show("Ingrese el monto con el que pagó el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    objVenta.MontoPago = decimal.Parse(txtpagocon.Text);
                    objVenta.MontoCambio = objVenta.MontoPago - objVenta.MontoTotal;

                    if (objVenta.MontoCambio < 0)
                    {
                        MessageBox.Show("El monto pagado no puede ser menor al total.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (objVenta.MetodoPago == "Tarjeta" || objVenta.MetodoPago == "Transferencia")
                {
                    objVenta.MontoPago = 0;
                    objVenta.MontoCambio = 0;
                }
            }

            // Crear DataTable para el detalle de la venta
            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(decimal));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));
            detalle_venta.Columns.Add("NombreProducto", typeof(string)); // Nueva columna

            // Llenar el DataTable con los datos de los productos
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (!row.IsNewRow)
                {
                    detalle_venta.Rows.Add(
                        Convert.ToInt32(row.Cells["IdProducto"].Value),
                        Convert.ToDecimal(row.Cells["Precio"].Value),
                        Convert.ToDecimal(row.Cells["Cantidad"].Value.ToString()),
                        Convert.ToDecimal(row.Cells["SubTotal"].Value),
                        row.Cells["Producto"].Value.ToString() // Asegúrate de incluir el nombre del producto
                    );
                }
            }

            // Calcular el impuesto
            decimal totalSinImpuesto = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                totalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }
            decimal totalISV = totalSinImpuesto * 0.15m; // Calcular el ISV

            // Asignar el impuesto al objeto de venta
            objVenta.Impuesto = totalISV;

            // Obtener el correlativo para el número de documento
            int idcorrelativo = new CN_Venta().ObtenerCorrelativo();
            numeroDocumentoImpresion = string.Format("{0:00000000}", idcorrelativo);

            // Crear el objeto de venta para registrar
            Venta oVenta = new Venta
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumentoImpresion,
                DocumentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text,
                NombreCliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text,
                MetodoPago = objVenta.MetodoPago, // Esto ahora será "N/A" si es crédito
                MontoPago = objVenta.MontoPago,
                MontoCambio = objVenta.MontoCambio,
                MontoTotal = Math.Round(decimal.Parse(txttotalpagar.Text), 2), // Asegurarse de que se redondee a dos decimales
                Impuesto = objVenta.Impuesto,// Asignar el impuesto
                Condicion = cbocondicion.SelectedItem.ToString(),  // Aquí asumes que el ComboBox tiene las opciones "Contado" o "Crédito"
                FechaVencimiento = objVenta.Condicion == "Credito" ? (DateTime?)dtpFechaVencimiento.Value : null // Asignar la fecha de vencimiento
            };

            string rangoAutorizado = negocio.RangoAutorizado; // Obtener el rango autorizado completo
            string[] partesRango = rangoAutorizado.Split(new[] { " al " }, StringSplitOptions.None);

            if (partesRango.Length == 2)
            {
                // Obtener el rango final
                string rangoFinal = partesRango[1].Substring(partesRango[1].Length - 8); // Obtener los últimos 8 dígitos del rango final

                // Comparar el correlativo de la venta con el rango final
                if (int.Parse(numeroDocumentoImpresion) > int.Parse(rangoFinal))
                {
                    DialogResult result = MessageBox.Show("¡Alerta! Se ha alcanzado el rango autorizado para las facturas. ¿Desea continuar con la venta?",
                                                            "Alerta",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return; // Si el usuario elige "No", salir del método y no continuar con la venta
                    }
                }
            }

            // Verificar si se ha alcanzado la fecha límite de emisión
            if (DateTime.Now.Date >= negocio.FechaLimiteEmision.Date)
            {
                DialogResult result = MessageBox.Show("¡Alerta! Se ha alcanzado la fecha límite de emisión. ¿Desea continuar con la venta?",
                                             "Alerta",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return; // Si el usuario elige "No", salir del método y no continuar con la venta
                }
            }

            // Registrar la venta
            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            if (respuesta)
            {
                // Restar el stock de los productos vendidos
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        int idProducto = Convert.ToInt32(row.Cells["IdProducto"].Value);
                        decimal cantidadVendida = Convert.ToDecimal(row.Cells["Cantidad"].Value);
                        int idCategoria = Convert.ToInt32(row.Cells["IdCategoria"].Value); // Asegúrate de que la columna IdCategoria esté presente

                        // Lógica para restar el stock solo si el IdCategoria es 35
                        if (idCategoria == 35)
                        {
                            // No restar stock para productos con IdCategoria 35
                            continue; // Salta a la siguiente iteración
                        }

                        // Lógica para restar el stock
                        bool stockRestado = new CN_Venta().RestarStock(idProducto, cantidadVendida);
                        if (!stockRestado)
                        {
                            MessageBox.Show("Error al restar stock para el producto ID: " + idProducto, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            // Manejo de impresión
            try
            {
                // Imprimir la original
                ImprimirFactura(false);

                // Imprimir la copia
                ImprimirFactura(true);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }

            // Manejo de respuesta
            if (respuesta)
            {
                var result = MessageBox.Show("TRANSACCION EXITOSA\n\nNúmero de venta generada:\n" + numeroDocumentoImpresion + "\n\n¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumentoImpresion);

                // Limpiar campos después de registrar la venta
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
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ImprimirFactura(bool esCopia)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = "POS-80"; // Asegúrate de que este sea el nombre correcto
            printDoc.PrintPage += (s, e) => PrintPage(s, e, esCopia); // Llama a PrintPage con el parámetro adecuado

            // Imprimir la factura
            printDoc.Print();
        }

        private void comboMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboMetodoPago.SelectedItem != null)
            {
                if (comboMetodoPago.SelectedItem.ToString() == "Efectivo")
                {
                    txtpagocon.Enabled = true;
                    txtcambio.Enabled = true;
                    txtpagocon.Text = "";
                    txtcambio.Text = "";
                }
                else if (comboMetodoPago.SelectedItem.ToString() == "Tarjeta" || comboMetodoPago.SelectedItem.ToString() == "Transferencia")
                {
                    txtpagocon.Enabled = false;
                    txtcambio.Enabled = false;
                    txtpagocon.Text = "0";
                    txtcambio.Text = "0";
                }
            }
        }

        private void cbotipodocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbotipodocumento.SelectedItem != null)
            {
                var selectedItem = (OpcionCombo)cbotipodocumento.SelectedItem;

                if ( selectedItem.Valor == "Cotizacion")
                {
                    btncrearventa.Visible = false;
                    btnCrearCotizacion.Visible = true;
                    comboMetodoPago.Enabled = false;
                    txtpagocon.Enabled = false;
                    txtcambio.Enabled = false;
                    txtpagocon.Text = "0";
                    txtcambio.Text = "0";
                    comboMetodoPago.Text = "";
                    txtoc.Enabled = false;
                    txtregexo.Enabled = false;
                    txtregsag.Enabled = false;
                    cbocondicion.Enabled = false;
                    cbocondicion.Text = "Contado";
                    dtpFechaVencimiento.Visible = false;
                    label14.Visible = false;

                }
                else if (selectedItem.Valor == "VentaSinFacturar")
                {
                    // Si se selecciona Venta Sin Facturar
                    btncrearventa.Visible = false; // Ocultar botón de crear venta
                    btnCrearCotizacion.Visible = false; // Ocultar botón de crear cotización
                                                        // Aquí puedes mostrar el botón específico para "Venta Sin Facturar" si lo tienes
                    btnVentaSinFacturar.Visible = true; // Mostrar botón de crear venta sin facturar
                    comboMetodoPago.Enabled = true; // Habilitar métodos de pago
                    txtpagocon.Enabled = true; // Habilitar campo de pago
                    txtcambio.Enabled = true; 
                    txtoc.Enabled = true;
                    txtregexo.Enabled = true; 
                    txtregsag.Enabled = true; 
                    cbocondicion.Enabled = true;
                    comboMetodoPago.Text = "Efectivo";
                    dtpFechaVencimiento.Visible = false; 
                    label14.Visible = false; 
                }
                else
                {
                    // Si se selecciona otro tipo de documento
                    btncrearventa.Visible = true; // Mostrar botón de crear venta
                    btnCrearCotizacion.Visible = false; // Ocultar botón de crear cotización
                                                        // Asegúrate de ocultar el botón de venta sin facturar
                    btnVentaSinFacturar.Visible = false; // Ocultar botón de crear venta sin facturar
                    comboMetodoPago.Enabled = true; // Habilitar métodos de pago
                    txtpagocon.Enabled = true; // Habilitar campo de pago
                    txtcambio.Enabled = true; // Habilitar campo de cambio
                    txtoc.Enabled = true; // Habilitar campo O/C
                    txtregexo.Enabled = true; // Habilitar campo de registro de exonerado
                    txtregsag.Enabled = true; // Habilitar campo de registro SAG
                    cbocondicion.Enabled = true; // Habilitar condición
                    comboMetodoPago.Text = "Efectivo"; // Establecer método de pago por defecto
                    dtpFechaVencimiento.Visible = false; // Ocultar fecha de vencimiento
                    label14.Visible = false; // Ocultar etiqueta de fecha de vencimiento
                }
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
            detalle_venta.Columns.Add("NombreProducto", typeof(string)); // Nueva columna

            // Llenar el DataTable con los datos de los productos
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (!row.IsNewRow)
                {
                    detalle_venta.Rows.Add(
                        Convert.ToInt32(row.Cells["IdProducto"].Value),
                        Convert.ToDecimal(row.Cells["Precio"].Value),
                        Convert.ToDecimal(row.Cells["Cantidad"].Value.ToString()),
                        Convert.ToDecimal(row.Cells["SubTotal"].Value),
                        row.Cells["Producto"].Value.ToString() // Asegúrate de incluir el nombre del producto
                    );
                }
            }

            // Obtener el correlativo para el número de documento
            int idcorrelativo = new CN_Venta().ObtenerCorrelativoCotizacion();
            string numeroDocumento = string.Format("{0:0000000000}", idcorrelativo);

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
                MontoTotal = objVenta.MontoTotal
            };

            // Registrar la cotización
            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            // Manejo de respuesta
            if (respuesta)
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
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }



        private void PrintPage(object sender, PrintPageEventArgs e, bool esCopia)
        {
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txttotalpagar.Text));

            int pageWidth = 280; // Ancho de la impresora térmica
            Font fontNormal = new Font("MS Gothic", 8, FontStyle.Regular);
            Font fontBold = new Font("MS Gothic", 8, FontStyle.Regular);
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
                        y += logoHeight + 0; // Espacio después del logo
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el logo: " + ex.Message);
            }

            // CENTRAR "NOMBRE DEL NEGOCIO"
            string nombreNegocio = negocio.Nombre.ToUpper();
            SizeF titleSize = e.Graphics.MeasureString(nombreNegocio, fontTitle);
            e.Graphics.DrawString(nombreNegocio, fontTitle, Brushes.Black, (pageWidth - titleSize.Width) / 2, y);
            y += 18;

            // CENTRAR "ESLOGAN DEL NEGOCIO"
            string eslogan = negocio.Eslogan;
            SizeF sloganSize = e.Graphics.MeasureString(eslogan, fontSubTitle);
            e.Graphics.DrawString(eslogan, fontSubTitle, Brushes.Black, (pageWidth - sloganSize.Width) / 2, y);
            y += 14;

            // Datos de la empresa, centrados
            string[] companyInfo =
            {
        "RTN: " + negocio.RUC,
        "Tel: " + negocio.Telefono,
        "Email: " + negocio.CorreoElectronico,
        "CAI: " + negocio.CAI
    };

            string direccionCompleta = "Dirección: " + negocio.Direccion;
            string[] direccionLines = SplitText(direccionCompleta, 32); // Dividir la dirección en líneas de 32 caracteres
            foreach (string line in direccionLines)
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontNormal);
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14; // Reducí el espaciado entre líneas
            }

            // Imprimir la información de la empresa
            foreach (string line in companyInfo)
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontNormal);
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14; // Reducí el espaciado entre líneas
            }

            y += 5;
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información de la factura
            string tipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Valor.ToString();
            // Obtener el valor seleccionado de cbotipodocumento
            string cliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text;
            // Mostrar el tipo de documento en la impresión
            e.Graphics.DrawString(tipoDocumento.ToUpper() +" " + negocio.RangoInicial + numeroDocumentoImpresion, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"), fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Atendido por: " + _Usuario.NombreCompleto, fontBold, Brushes.Black, 10, y);
            y += 13;
            if (cbocondicion.Text == "Credito")
            {
                e.Graphics.DrawString("Condicion: " + cbocondicion.Text, fontBold, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Fecha de vencimiento: " + dtpFechaVencimiento.Value.ToString("dd/MM/yyyy"), fontBold, Brushes.Black, 10, y);
                y += 13; // Aumentar la posición Y para la siguiente línea
            }
            if (cbocondicion.Text == "Contado")
            {
                e.Graphics.DrawString("Condicion: " + cbocondicion.Text, fontBold, Brushes.Black, 10, y);
                y += 13;
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información del cliente
            e.Graphics.DrawString("Cliente: " + cliente, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("ID/RTN Cliente: " + documentoCliente, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("No.O/C exenta: " + txtoc.Text, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("No.Reg de exonerado: " + txtregexo.Text, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("No.De reg de la SAG: " + txtregsag.Text, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Detalle de producto
            e.Graphics.DrawString("CANT  DESCRIPCION                     IMPORTE", fontBold, Brushes.Black, 10, y);
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
            decimal montoTotal = decimal.Parse(txttotalpagar.Text);
            decimal pagoCon = string.IsNullOrEmpty(txtpagocon.Text) ? 0 : decimal.Parse(txtpagocon.Text);
            decimal cambio = pagoCon - montoTotal;
            int xAlignRight = pageWidth - 1; // Límite derecho
            int columnWidth = 100; // Ancho de la columna de valores

            // Ajustar la posición de los encabezados más a la derecha
            int xAlignHeader = xAlignRight - columnWidth - 1; // Ajuste para que los encabezados estén más cerca del borde derecho
            int xAlignCurrency = xAlignRight - 100; // Posición de la "L." para evitar que se monte en los números
            int xAlignValues = xAlignRight - 1; // Los valores se pegarán más al margen derecho sin que se salgan

            // Función para imprimir encabezado y valor alineados
            void PrintAlignedText(string header, string value, bool isTotal = false)
            {
                // Medir el ancho del encabezado
                SizeF headerSize = e.Graphics.MeasureString(header, fontBold);
                float xHeader = xAlignHeader - headerSize.Width; // Asegura que los encabezados estén alineados a la derecha
                e.Graphics.DrawString(header, fontBold, Brushes.Black, xHeader, y);

                // Dibuja la "L." en la posición correcta
                e.Graphics.DrawString("L.", fontBold, Brushes.Black, xAlignCurrency, y); // Esto debería imprimir "L."

                // Dibuja el valor, alineado a la derecha
                e.Graphics.DrawString(value, isTotal ? fontSubTitle : fontBold, Brushes.Black, xAlignValues, y, rightAlign);

                y += 13; // Espaciado entre líneas
            }
            decimal subtotalSinImpuesto = 0;
            decimal totalISV = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                subtotalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }
            totalISV = subtotalSinImpuesto * 0.15m;
            // Llamadas alineadas con "L."
            PrintAlignedText("SUBTOTAL:", subtotalSinImpuesto.ToString("0.00"));
            PrintAlignedText("Descuentos:", "0.00");
            PrintAlignedText("Importe Exento:", "0.00");
            PrintAlignedText("Importe Exonerado:", "0.00");
            PrintAlignedText("Gravado 15%:", subtotalSinImpuesto.ToString("0.00"));
            PrintAlignedText("Gravado 18%:", "0.00");
            PrintAlignedText("ISV 15%:", txtimpuesto.Text);
            PrintAlignedText("ISV 18%:", "0.00");

            // Resaltar TOTAL A PAGAR (usando el flag isTotal para ajuste fino)

            PrintAlignedText("TOTAL A PAGAR:", txttotalpagar.Text, true);
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
                e.Graphics.DrawString("Son: " + lineasTotalEnLetras[0], fontBold, Brushes.Black, 10, y);
                y += 13;
            }

            // Imprimir las líneas restantes sin "Son:"
            for (int i = 1; i < lineasTotalEnLetras.Count; i++)
            {
                e.Graphics.DrawString(lineasTotalEnLetras[i], fontBold, Brushes.Black, 10, y);
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
                    e.Graphics.DrawString(efectivoTexto, fontBold, Brushes.Black, 10, y);
                    y += 13;
                    e.Graphics.DrawString(cambioTexto, fontBold, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es tarjeta
                else if (comboMetodoPago.Text == "Tarjeta")
                {
                    e.Graphics.DrawString("Terminal Atlantida: L." + txttotalpagar.Text, fontBold, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es transferencia
                else if (comboMetodoPago.Text == "Transferencia")
                {
                    e.Graphics.DrawString("Transferencia", fontBold, Brushes.Black, 10, y);
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
                e.Graphics.DrawString(line, fontBold, Brushes.Black, 10, y);
                y += 13;
            }

            // Imprimir la fecha límite
            e.Graphics.DrawString(fechaLimite, fontBold, Brushes.Black, 10, y);
            y += 15;

            e.Graphics.DrawString("ORIGINAL: CLIENTE", fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("COPIA: OBLIGADO TRIBUTARIO EMISOR", fontBold, Brushes.Black, 10, y);
            y += 15;

            // Mensaje final
            string mensajeFinal = "LA FACTURA ES BENEFICIO DE TODOS, ¡EXÍJALA!";
            SizeF mensajeSize = e.Graphics.MeasureString(mensajeFinal, fontBold);
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

        private void cbocondicion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbocondicion.SelectedItem.ToString() == "Credito")
            {
                dtpFechaVencimiento.Visible = true;
                label14.Visible = true;
                comboMetodoPago.Enabled = false;
                txtpagocon.Enabled = false;
                txtcambio.Enabled = false;
                txtpagocon.Text = "0";
                txtcambio.Text = "0";
                comboMetodoPago.Text = "";
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
        }

        private void btnVentaSinFacturar_Click(object sender, EventArgs e)
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

            // Crear objeto de venta
            Venta objVenta = new Venta
            {
                MetodoPago = cbocondicion.SelectedItem.ToString() == "Credito" ? "N/A" : comboMetodoPago.SelectedItem.ToString(),
                MontoTotal = decimal.Parse(txttotalpagar.Text),
                Condicion = cbocondicion.SelectedItem.ToString()
            };

            // Si la condición es "Crédito", no se requiere monto pagado ni cambio
            if (objVenta.Condicion == "Credito")
            {
                objVenta.MontoPago = 0; // No se requiere monto pagado
                objVenta.MontoCambio = 0; // No se requiere cambio
            }
            else
            {
                // Verificar el método de pago y calcular el cambio si es efectivo
                if (objVenta.MetodoPago == "Efectivo")
                {
                    if (string.IsNullOrEmpty(txtpagocon.Text))
                    {
                        MessageBox.Show("Ingrese el monto con el que pagó el cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    objVenta.MontoPago = decimal.Parse(txtpagocon.Text);
                    objVenta.MontoCambio = objVenta.MontoPago - objVenta.MontoTotal;

                    if (objVenta.MontoCambio < 0)
                    {
                        MessageBox.Show("El monto pagado no puede ser menor al total.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (objVenta.MetodoPago == "Tarjeta" || objVenta.MetodoPago == "Transferencia")
                {
                    objVenta.MontoPago = 0;
                    objVenta.MontoCambio = 0;
                }
            }

            // Crear DataTable para el detalle de la venta
            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(decimal));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));
            detalle_venta.Columns.Add("NombreProducto", typeof(string)); // Nueva columna

            // Llenar el DataTable con los datos de los productos
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (!row.IsNewRow)
                {
                    detalle_venta.Rows.Add(
                        Convert.ToInt32(row.Cells["IdProducto"].Value),
                        Convert.ToDecimal(row.Cells["Precio"].Value),
                        Convert.ToDecimal(row.Cells["Cantidad"].Value.ToString()),
                        Convert.ToDecimal(row.Cells["SubTotal"].Value),
                        row.Cells["Producto"].Value.ToString() // Asegúrate de incluir el nombre del producto
                    );
                }
            }

            // Calcular el impuesto
            decimal totalSinImpuesto = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                totalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }
            decimal totalISV = totalSinImpuesto * 0.15m; // Calcular el ISV

            // Asignar el impuesto al objeto de venta
            objVenta.Impuesto = totalISV;

            // Obtener el correlativo para el número de documento
            int idcorrelativo = new CN_Venta().ObtenerCorrelativoVentaSinFacturar();
            numeroDocumentoImpresion = string.Format("{0:0000}", idcorrelativo);

            // Crear el objeto de venta para registrar
            Venta oVenta = new Venta
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numeroDocumentoImpresion,
                DocumentoCliente = string.IsNullOrEmpty(txtdocumentocliente.Text) ? "N/A" : txtdocumentocliente.Text,
                NombreCliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text,
                MetodoPago = objVenta.MetodoPago, // Esto ahora será "N/A" si es crédito
                MontoPago = objVenta.MontoPago,
                MontoCambio = objVenta.MontoCambio,
                MontoTotal = Math.Round(decimal.Parse(txttotalpagar.Text), 2), // Asegurarse de que se redondee a dos decimales
                Impuesto = objVenta.Impuesto,// Asignar el impuesto
                Condicion = cbocondicion.SelectedItem.ToString(),  // Aquí asumes que el ComboBox tiene las opciones "Contado" o "Crédito"
                FechaVencimiento = objVenta.Condicion == "Credito" ? (DateTime?)dtpFechaVencimiento.Value : null // Asignar la fecha de vencimiento
            };

            // Registrar la venta
            string mensaje = string.Empty;
            bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

            if (respuesta)
            {
                // Restar el stock de los productos vendidos
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        int idProducto = Convert.ToInt32(row.Cells["IdProducto"].Value);
                        decimal cantidadVendida = Convert.ToDecimal(row.Cells["Cantidad"].Value);

                        // Obtener el IdCategoria del producto
                        int idCategoria = Convert.ToInt32(row.Cells["IdCategoria"].Value); // Asegúrate de que la columna IdCategoria esté presente

                        // Lógica para restar el stock solo si el IdCategoria es 35
                        if (idCategoria == 35)
                        {
                            // No restar stock para productos con IdCategoria 35
                            continue; // Salta a la siguiente iteración
                        }

                        // Lógica para restar el stock
                        bool stockRestado = new CN_Venta().RestarStock(idProducto, cantidadVendida);
                        if (!stockRestado)
                        {
                            MessageBox.Show("Error al restar stock para el producto ID: " + idProducto, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            if (respuesta)
            {
                var result = MessageBox.Show("Número de venta sin facturar generada:\n" + numeroDocumentoImpresion + "\n\n¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numeroDocumentoImpresion);

                // Limpiar campos después de registrar la venta
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
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

      
    }

}

    

