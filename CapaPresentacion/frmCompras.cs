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

using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion
{
    public partial class frmCompras : Form
    {
        private Usuario _Usuario;
        private decimal ultimoImpuesto = 0;
        private string bufferCodigoScanner = "";
        private DateTime ultimaTecla = DateTime.Now;
        private decimal ultimaRebaja = 0;

        public frmCompras(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
            this.KeyPreview = true;
        }




        private void frmCompras_Load(object sender, EventArgs e)
        {
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cbotipodocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbotipodocumento.DisplayMember = "Texto";
            cbotipodocumento.ValueMember = "Valor";
            cbotipodocumento.SelectedIndex = 0;

            cbometodopago.Items.Add("Efectivo");
            cbometodopago.Items.Add("Tarjeta");
            cbometodopago.Items.Add("Transferencia");
            cbometodopago.SelectedIndex = 0;


            cbocondicion.Items.Add("Contado");
            cbocondicion.Items.Add("Credito");
            cbocondicion.SelectedIndex = 0;


            dtpFechaVencimiento.Visible = false;

            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtidproveedor.Text = "0";
            txtidproducto.Text = "0";

            foreach (System.Windows.Forms.Control ctrl in this.Controls)

            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }

            this.Focus();
        }

      

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (int.TryParse(txtidproducto.Text, out int idProdExistente) && idProdExistente != 0)
            {
                Producto productoBD = new CN_Producto().Listar().FirstOrDefault(p => p.IdProducto == idProdExistente);
                if (productoBD != null && productoBD.Descripcion != txtdescripcion.Text.Trim())
                {
                    string mensaje = "";
                    bool actualizado = new CN_Producto().ActualizarDescripcion(idProdExistente, txtdescripcion.Text.Trim(), out mensaje);
                    if (!actualizado)
                    {
                        MessageBox.Show("No se pudo actualizar la descripción: " + mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            // Verificar si ya fue agregado
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                if (fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    MessageBox.Show("El producto ya fue agregado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (!decimal.TryParse(txtpreciocompra.Text, out decimal precioCompra))
            {
                MessageBox.Show("Precio de compra inválido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!decimal.TryParse(txtprecioventa.Text, out decimal precioVenta))
            {
                MessageBox.Show("Precio de venta inválido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            decimal cantidad = txtcantidad.Value;
            decimal descuento = 0;
            decimal.TryParse(txtdescuento.Text, out descuento);

            decimal subtotal = precioCompra * cantidad;

            dgvdata.Rows.Add(
                txtidproducto.Text,
                txtproducto.Text,            // Producto
                txtdescripcion.Text,         // Descripción
                precioCompra.ToString("0.00"), // Precio Compra
                precioVenta.ToString("0.00"),  // Precio Venta
                cantidad.ToString(),         // Cantidad
                subtotal.ToString("0.00"),   // SubTotal
                descuento.ToString("0.00")   // % D
            );

            limpiarProducto();
            calcularTotal();
        }

        private void limpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtcodproducto.BackColor = Color.White;
            txtproducto.Text = "";
            txtpreciocompra.Text = "";
            txtprecioventa.Text = "";
            txtcantidad.Value = 1;
            txtdescripcion.Text = "";
        }

        private void calcularTotal()
        {
            decimal subtotal = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Cells["SubTotal"].Value != null)
                {
                    subtotal += Convert.ToDecimal(row.Cells["SubTotal"].Value);
                }
            }

            txtsubtotal.Text = subtotal.ToString("0.00");

            decimal rebaja = 0;
            decimal.TryParse(txtrebaja.Text, out rebaja);

            decimal impuesto = 0;
            decimal.TryParse(txtimpuesto.Text, out impuesto);

            decimal total = subtotal - rebaja + impuesto;
            txttotalpagar.Text = total.ToString("0.00");
        }


        private void btnbuscarproveedor_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProveedor())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtidproveedor.Text = modal._Proveedor.IdProveedor.ToString();
                    txtdocproveedor.Text = modal._Proveedor.Documento;
                    txtnombreproveedor.Text = modal._Proveedor.RazonSocial;
                    
                }
                else
                {
                    txtdocproveedor.Select();
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
                    txtcodproducto.Text = modal._Producto.Codigo;
                    txtproducto.Text = modal._Producto.Nombre;   
                    txtdescripcion.Text = modal._Producto.Descripcion;
                    txtpreciocompra.Select();
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
                Producto oProducto = new CN_Producto().Listar()
                    .FirstOrDefault(p => p.Codigo == txtcodproducto.Text && p.Estado == true);

                if (oProducto != null)
                {
                    txtcodproducto.BackColor = Color.Honeydew;
                    txtidproducto.Text = oProducto.IdProducto.ToString();
                    txtproducto.Text = oProducto.Nombre;
                    txtdescripcion.Text = oProducto.Descripcion;
                    txtpreciocompra.Select();
                }
                else
                {
                    var modal = new mdNuevoProductoDesdeCompra(txtcodproducto.Text);
                    var result = modal.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        Producto nuevo = modal.ProductoNuevo;
                        txtcodproducto.BackColor = Color.Honeydew;
                        txtidproducto.Text = nuevo.IdProducto.ToString();
                        txtproducto.Text = nuevo.Nombre;
                        txtdescripcion.Text = nuevo.Descripcion;
                        txtpreciocompra.Select();
                    }
                    else
                    {
                        txtcodproducto.BackColor = Color.MistyRose;
                        txtidproducto.Text = "0";
                        txtproducto.Text = "";
                    }
                }
            }
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Usa el índice real de la columna por nombre
            int colEliminar = dgvdata.Columns["btneliminar"].Index;

            if (e.ColumnIndex == colEliminar)
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
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    dgvdata.Rows.RemoveAt(indice);
                    calcularTotal();

                }
            }
        }

        private void txtpreciocompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtpreciocompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void txtprecioventa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtprecioventa.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void btnregistrar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtidproveedor.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable detalle_compra = new DataTable();
            detalle_compra.Columns.Add("IdProducto", typeof(int));
            detalle_compra.Columns.Add("PrecioCompra", typeof(decimal));
            detalle_compra.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_compra.Columns.Add("Cantidad", typeof(decimal));
            detalle_compra.Columns.Add("MontoTotal", typeof(decimal));
            detalle_compra.Columns.Add("DescuentoPorcentaje", typeof(decimal)); // NUEVO

            decimal subtotal = 0;

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                decimal precioCompra = Convert.ToDecimal(row.Cells["PrecioCompra"].Value);
                decimal cantidad = Convert.ToDecimal(row.Cells["Cantidad"].Value);
                decimal descuento = 0;
                decimal.TryParse(row.Cells["DescuentoPorcentaje"].Value?.ToString(), out descuento);

                decimal montoTotal = precioCompra * cantidad;

                detalle_compra.Rows.Add(
                    Convert.ToInt32(row.Cells["IdProducto"].Value),
                    precioCompra,
                    Convert.ToDecimal(row.Cells["PrecioVenta"].Value),
                    cantidad,
                    montoTotal,
                    descuento
                );

                subtotal += montoTotal;
            }

            int idcorrelativo = new CN_Compra().ObtenerCorrelativo();
            string numerodocumento = string.Format("{0:00000}", idcorrelativo);

            decimal impuesto = 0;
            decimal.TryParse(txtimpuesto.Text, out impuesto);

            decimal montoTotalFinal = subtotal;

            if (impuesto > 0)
                montoTotalFinal += impuesto;

            Compra oCompra = new Compra()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                oProveedor = new Proveedor() { IdProveedor = Convert.ToInt32(txtidproveedor.Text) },
                TipoDocumento = ((OpcionCombo)cbotipodocumento.SelectedItem).Texto,
                NumeroDocumento = numerodocumento,
                NoFactura = txtnfactura.Text.ToString(),
                MontoTotal = montoTotalFinal,
                Impuesto = impuesto,
                MetodoPago = cbocondicion.SelectedItem.ToString() == "Credito" ? "N/A" : cbometodopago.SelectedItem.ToString(),
                Condicion = cbocondicion.SelectedItem.ToString(),
                FechaVencimiento = cbocondicion.SelectedItem.ToString() == "Credito" ? dtpFechaVencimiento.Value.ToString("dd/MM/yyyy") : null
            };

            string mensaje = string.Empty;
            bool respuesta = new CN_Compra().Registrar(oCompra, detalle_compra, out mensaje);

            if (respuesta)
            {
                var result = MessageBox.Show("Número de compra generada:\n" + numerodocumento + "\n\n¿Desea copiar al portapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    Clipboard.SetText(numerodocumento);

                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cbocondicion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbocondicion.SelectedItem.ToString() == "Credito")
            {
                dtpFechaVencimiento.Visible = true; 
                cbometodopago.Enabled = false; 
                cbometodopago.SelectedItem = "N/A"; 
            }
            else
            {
                dtpFechaVencimiento.Visible = false; 
                cbometodopago.Enabled = true; 
            }
        }

        private void LimpiarFormulario()
        {
            txtidproveedor.Text = "0";
            txtdocproveedor.Text = "";
            txtnombreproveedor.Text = "";
            txtnfactura.Text = "";
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtproducto.Text = "";
            txtdescripcion.Text = "";
            txtpreciocompra.Text = "";
            txtprecioventa.Text = "";
            txtcantidad.Value = 1;
            txttotalpagar.Text = "0.00";
            txtimpuesto.Text = "0.00";
            dgvdata.Rows.Clear();
            cbometodopago.SelectedIndex = 0;
            cbocondicion.SelectedIndex = 0;
            dtpFechaVencimiento.Visible = false;
            txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtimpuesto.Text = "0.00";
            ultimoImpuesto = 0;
            ultimaRebaja = 0;
            txtdescuento.Text = "0";
            txtrebaja.Text = "0";
        }

        private void btnrefrescar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void txtimpuesto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                decimal impuesto = 0;
                if (decimal.TryParse(txtimpuesto.Text, out impuesto))
                {
   
                    decimal totalActual = Convert.ToDecimal(txttotalpagar.Text);
                    decimal nuevoTotal = totalActual - ultimoImpuesto + impuesto; 
                    txttotalpagar.Text = nuevoTotal.ToString("0.00");
                    ultimoImpuesto = impuesto; 
                    txtimpuesto.Text = impuesto.ToString("0.00");
                    txtimpuesto.Select();
                }
                else
                {
                    MessageBox.Show("Formato de impuesto incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtimpuesto.Select();
                }
            }
        }

        private void txtimpuesto_Leave(object sender, EventArgs e)
        {
            decimal impuesto;
            if (decimal.TryParse(txtimpuesto.Text, out impuesto))
            {
                txtimpuesto.Text = impuesto.ToString("0.00");
            }
            else
            {
                txtimpuesto.Text = "0.00"; 
            }
        }

        private void frmCompras_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {

                btnregistrar.PerformClick();

                e.Handled = true;
            }
        }

        private void frmCompras_Activated(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void frmCompras_KeyPress(object sender, KeyPressEventArgs e)
        {
            TimeSpan diff = DateTime.Now - ultimaTecla;

            if (diff.TotalMilliseconds > 500)
                bufferCodigoScanner = "";

            ultimaTecla = DateTime.Now;

            if (e.KeyChar == (char)Keys.Enter)
            {
                string codigoEscaneado = bufferCodigoScanner.Trim();
                bufferCodigoScanner = "";

                if (!string.IsNullOrEmpty(codigoEscaneado))
                {
                    BuscarProductoPorCodigo(codigoEscaneado);
                }
            }
            else
            {
                bufferCodigoScanner += e.KeyChar;
            }
        }
        private void BuscarProductoPorCodigo(string codigo)
        {
            Producto oProducto = new CN_Producto().ObtenerPorCodigo(codigo);

            if (oProducto != null)
            {
                txtidproducto.Text = oProducto.IdProducto.ToString(); // <-- ESTA ES LA QUE FALTABA
                txtcodproducto.Text = oProducto.Codigo;
                txtproducto.Text = oProducto.Nombre;
                txtdescripcion.Text = oProducto.Descripcion;
                txtcodproducto.BackColor = Color.Honeydew;
                txtpreciocompra.Select(); // Para que sigas el flujo normal
            }
            else
            {
                txtidproducto.Text = "0"; // IMPORTANTE: poner en 0 si no se encontró
                txtcodproducto.Text = codigo; // Muestra el código que se escaneó
                txtproducto.Text = "";
                txtdescripcion.Text = "";
                txtcodproducto.BackColor = Color.MistyRose;
            }
        }

        private void txtrebaja_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                decimal rebaja = 0;
                if (decimal.TryParse(txtrebaja.Text, out rebaja))
                {
                    decimal totalActual = Convert.ToDecimal(txttotalpagar.Text);
                    decimal nuevoTotal = totalActual + ultimaRebaja - rebaja; // Se resta la nueva y se suma la anterior para ajustar correctamente
                    txttotalpagar.Text = nuevoTotal.ToString("0.00");
                    ultimaRebaja = rebaja;
                    txtrebaja.Text = rebaja.ToString("0.00");
                    txtrebaja.Select();
                }
                else
                {
                    MessageBox.Show("Formato de rebaja incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtrebaja.Select();
                }
            }
        }

        private void txtrebaja_Leave(object sender, EventArgs e)
        {
            decimal rebaja;
            if (decimal.TryParse(txtrebaja.Text, out rebaja))
            {
                txtrebaja.Text = rebaja.ToString("0.00");
            }
            else
            {
                txtrebaja.Text = "0.00";
            }
        }
    }
}
