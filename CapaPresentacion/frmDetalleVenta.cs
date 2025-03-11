using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;



namespace CapaPresentacion
{
    public partial class frmDetalleVenta : Form
    {
        private Negocio negocio;
        public frmDetalleVenta()
        {
            negocio = new CN_Negocio().ObtenerDatos();
            InitializeComponent();
         
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            Venta oVenta = new CN_Venta().ObtenerVenta(txtbusqueda.Text);

            if (oVenta.IdVenta != 0)
            {
                txtnumerodocumento.Text = oVenta.NumeroDocumento;
                txtfecha.Text = oVenta.FechaRegistro;
                txttipodocumento.Text = oVenta.TipoDocumento;
                txtusuario.Text = oVenta.oUsuario.NombreCompleto;
                txtdoccliente.Text = oVenta.DocumentoCliente;
                txtnombrecliente.Text = oVenta.NombreCliente;
                txtmetodopago.Text = oVenta.MetodoPago;
                txtcondicion.Text = oVenta.Condicion;
                txtfechavencimiento.Text = oVenta.FechaVencimiento?.ToString("dd/MM/yyyy");

                dgvdata.Rows.Clear();
                foreach (Detalle_Venta dv in oVenta.oDetalle_Venta)
                {
                    // Verifica el valor de cantidad
                   
                    dgvdata.Rows.Add(new object[] {
                    dv.NombreProducto,
                    dv.PrecioVenta.ToString("0.00"),
                    dv.Cantidad.ToString("0.00"),
                    dv.SubTotal.ToString("0.00")
                   });
                }

                txtmontototal.Text = oVenta.MontoTotal.ToString("0.00");
                txtmontopago.Text = oVenta.MontoPago.ToString("0.00");
                txtmontocambio.Text = oVenta.MontoCambio.ToString("0.00");
                txtimpuesto.Text = oVenta.Impuesto.ToString("0.00");
            }
        }

        private void frmDetalleVenta_Load(object sender, EventArgs e)
        {
           
            txtbusqueda.Select();

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

        private void btnborrar_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            txtfecha.Text = "";
            txttipodocumento.Text = "";
            txtusuario.Text = "";
            txtdoccliente.Text = "";
            txtnombrecliente.Text = "";
            txtimpuesto.Text = "";
            txtcondicion.Text = "";
            txtfechavencimiento.Text = "";
            txtcondicion.Text = "";
            txtnumerodocumento.Text = "";
            txtcondicion.Text = "";

            dgvdata.Rows.Clear();
            txtmontototal.Text = "0.00";
            txtmontopago.Text = "0.00";
            txtmontocambio.Text = "0.00";
            txtbusqueda.Select();
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
        private void SendFileViaWhatsApp(string filePath)
        {
            // Abre WhatsApp Web con el archivo adjunto
            string url = $"https://web.whatsapp.com/send?text=Hola! Aquí está el archivo que quería compartir: {filePath}";
            System.Diagnostics.Process.Start(url);
        }



        private void btndescargar_Click(object sender, EventArgs e)
        {
            if (txttipodocumento.Text == "")
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Obtener el método de pago seleccionado (Efectivo o Tarjeta)
            string metodoPago = txtmetodopago.Text;
            string Condicion = txtcondicion.Text;
            string tipodocumento = txttipodocumento.Text;
            decimal TotalPagar = decimal.Parse(txtmontototal.Text);

            // Calcular subtotal antes de impuestos (base imponible)
            decimal subtotalsinimpuesto = TotalPagar / 1.15m;

            // Calcular el impuesto (15% del subtotal)
            decimal impuesto = subtotalsinimpuesto * 0.15m;

            // Convertir el total a pagar en letras
            string totalEnLetras = ConvertirNumeroALetras.Convertir(TotalPagar);

            // Seleccionar la plantilla según el método de pago
            string Texto_Html = string.Empty;

            if (metodoPago == "Efectivo")
            {
                Texto_Html = Properties.Resources.PlantillaVenta;
            }
           if (metodoPago == "Tarjeta")
            {
                Texto_Html = Properties.Resources.PlantillaVentaTarjeta;
            }
            if (tipodocumento == "Cotizacion")
            {
                Texto_Html = Properties.Resources.PlantillaCotizacion;
            }
            else if (metodoPago == "Transferencia")
            {
                Texto_Html = Properties.Resources.PlantillaVentaTransferencia;
            }
            else if (Condicion == "Credito")
            {
                Texto_Html = Properties.Resources.PlantillaVentaCredito; 
            }

            Negocio odatos = new CN_Negocio().ObtenerDatos();

            // Reemplazar los valores con la información de la venta
            Texto_Html = Texto_Html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", odatos.RUC);
            Texto_Html = Texto_Html.Replace("@direcnegocio", odatos.Direccion);
            Texto_Html = Texto_Html.Replace("@tipodocumento", txttipodocumento.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento",txtnumerodocumento.Text);
            Texto_Html = Texto_Html.Replace("@condicion", txtcondicion.Text);
            Texto_Html = Texto_Html.Replace("@Fechavencimiento", txtfechavencimiento.Text);
            Texto_Html = Texto_Html.Replace("@rangoin", odatos.RangoInicial);

            // Suponiendo que odatos es un objeto que contiene la propiedad FechaLimiteEmision
            Texto_Html = Texto_Html.Replace("@fechalimite", odatos.FechaLimiteEmision.ToString("dd/MM/yyyy")); // Cambia el formato según sea necesario
            Texto_Html = Texto_Html.Replace("@rangoautorizado", odatos.RangoAutorizado);
  
            Texto_Html = Texto_Html.Replace("@oc", txtoc.Text);
            Texto_Html = Texto_Html.Replace("@sag", txtregsag.Text);
            Texto_Html = Texto_Html.Replace("@exonerado", txtregexo.Text);
            Texto_Html = Texto_Html.Replace("@doccliente", txtdoccliente.Text);
            Texto_Html = Texto_Html.Replace("@nombrecliente", txtnombrecliente.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtfecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtusuario.Text);
            Texto_Html = Texto_Html.Replace("@telefono", odatos.Telefono);
            Texto_Html = Texto_Html.Replace("@cai", odatos.CAI);
            Texto_Html = Texto_Html.Replace("@correo", odatos.CorreoElectronico);
            Texto_Html = Texto_Html.Replace("@eslogan", odatos.Eslogan);

            // Crear las filas para la tabla de productos de la venta
            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>L. " + row.Cells["Precio"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>L. " + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }

            // Reemplazar las filas de productos en el HTML
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtmontototal.Text);
            Texto_Html = Texto_Html.Replace("@totalenletras", totalEnLetras);


            Texto_Html = Texto_Html.Replace("@totalsinimpuesto", subtotalsinimpuesto.ToString("N2"));
            Texto_Html = Texto_Html.Replace("@impuesto", impuesto.ToString("N2"));
            Texto_Html = Texto_Html.Replace("@totalpagar", TotalPagar.ToString("N2"));
           

            Texto_Html = Texto_Html.Replace("@pagocon", txtmontopago.Text);
            Texto_Html = Texto_Html.Replace("@cambio", txtmontocambio.Text);


            // Guardar como PDF
            string prefijoDocumento = (txttipodocumento.Text.Trim().ToUpper() == "COTIZACION") ? "Cotizacion" : "Venta";


            // Guardar como PDF con el nombre correcto
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("{0}_{1}.pdf", prefijoDocumento, txtnumerodocumento.Text);
            savefile.Filter = "Pdf Files|*.pdf";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    bool obtenido = true;
                    byte[] byteImage = new CN_Negocio().ObtenerLogo(out obtenido);

                    if (obtenido)
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(byteImage);
                        img.ScaleToFit(60, 60);
                        img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        img.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                        pdfDoc.Add(img);
                    }

                    using (StringReader sr = new StringReader(Texto_Html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }

                    pdfDoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (MessageBox.Show("¿Desea enviar el archivo por WhatsApp?", "Enviar por WhatsApp", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SendFileViaWhatsApp(savefile.FileName);
                }
            }


        }

        private void btnAnular_Click(object sender, EventArgs e)
        {
            CD_Venta objVenta = new CD_Venta(); 
            if (txtnumerodocumento.Text.Trim() == "")
            {
           
                MessageBox.Show("Debe ingresar un número de documento.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string numeroDocumento = txtnumerodocumento.Text.Trim(); 

            int idVenta = objVenta.ObtenerIdVentaPorNumeroDocumento(numeroDocumento); 

            if (idVenta == 0) 
            {
                MessageBox.Show("No se encontró la venta con ese número de documento.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de anular esta factura?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                string mensaje; 

                if (objVenta.AnularVenta(idVenta, out mensaje))
                {
                
                    MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                    txtbusqueda.Text = "";
                    txtfecha.Text = "";
                    txttipodocumento.Text = "";
                    txtusuario.Text = "";
                    txtdoccliente.Text = "";
                    txtnombrecliente.Text = "";
                    txtmetodopago.Text = "";
                    txtnumerodocumento.Text = "";

               
                    dgvdata.Rows.Clear();
                    txtmontototal.Text = "0.00";
                    txtmontopago.Text = "0.00";
                    txtmontocambio.Text = "0.00";
                }
                else
                {
                  
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txttipodocumento_TextChanged(object sender, EventArgs e)
        {
            VerificarTipoDocumento();
        }

        private void VerificarTipoDocumento()
        {
            if (txttipodocumento.Text.Trim().ToUpper() == "COTIZACION")
            {
                btnAnular.Visible = false;
                txtmetodopago.Visible = false;
                label13.Visible = false;
                iconButton1.Visible = false;
            }
            else
            {
                btnAnular.Visible = true;
                txtmetodopago.Visible = true;
                label13.Visible = true;
                iconButton1.Visible = true;
            }
        }

        private void CargarDatos()
        {
            // Código donde asignas datos desde la BD...
            txttipodocumento.Text = "Cotización"; // Simulación de carga desde BD

            // Llamar al método para aplicar la lógica
            VerificarTipoDocumento();
        }


        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txtmontototal.Text));

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
            string tipoDocumento = txttipodocumento.Text;
            string cliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrEmpty(txtdoccliente.Text) ? "N/A" : txtdoccliente.Text;

            // Mostrar el tipo de documento en la impresión
            e.Graphics.DrawString(tipoDocumento.ToUpper() + " " + negocio.RangoInicial + txtnumerodocumento.Text, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"), fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Atendido por: " + txtusuario.Text, fontBold, Brushes.Black, 10, y);
            y += 13;
            if (txtcondicion.Text == "Credito")
            {
                e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontBold, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString($"Fecha de vencimiento: {txtfechavencimiento.Text}", fontBold, Brushes.Black, 10, y);
                y += 13;
            }
            if (txtcondicion.Text == "Contado")
            {
                e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontBold, Brushes.Black, 10, y);
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

                int maxCaracteresPorLineaProductos = 35; // Límite de caracteres por línea
                int inicioDescripcion = 10;  // Posición inicial de la descripción
                List<string> lineasDescripcion = new List<string>();

                // Dividir la descripción en líneas de hasta 35 caracteres
                for (int i = 0; i < producto.Length; i += maxCaracteresPorLineaProductos)
                {
                    lineasDescripcion.Add(producto.Substring(i, Math.Min(maxCaracteresPorLineaProductos, producto.Length - i)));
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
            int xAlignRight = pageWidth - 1; // Límite derecho
            int columnWidth = 100; // Ancho de la columna de valores

            // Ajustar la posición de los encabezados más a la derecha
            int xAlignHeader = xAlignRight - columnWidth - 1; // Ajuste para que los encabezados estén más cerca del borde derecho
            int xAlignCurrency = xAlignRight - 100; // Posición de la "L." para evitar que se monte en los números
            int xAlignValues = xAlignRight - 1;

            void PrintAlignedText(string header, string value, PrintPageEventArgs printEventArgs, ref int lineY, bool isTotal = false)
            {
                // Medir el ancho del encabezado
                SizeF headerSize = printEventArgs.Graphics.MeasureString(header, fontBold);
                float xHeader = xAlignHeader - headerSize.Width; // Asegura que los encabezados estén alineados a la derecha
                printEventArgs.Graphics.DrawString(header, fontBold, Brushes.Black, xHeader, lineY);

                // Dibuja la "L." en la posición correcta
                printEventArgs.Graphics.DrawString("L.", fontBold, Brushes.Black, xAlignCurrency, lineY); // Esto debería imprimir "L."

                // Dibuja el valor, alineado a la derecha
                printEventArgs.Graphics.DrawString(value, isTotal ? fontSubTitle : fontBold, Brushes.Black, xAlignValues, lineY, rightAlign);

                lineY += 13; // Espaciado entre líneas
            }

            // Calcular totales
            decimal subtotalSinImpuesto = 0;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                subtotalSinImpuesto += Convert.ToDecimal(row.Cells["SubTotal"].Value);
            }

            // Llamadas alineadas con "L."
            PrintAlignedText("SUBTOTAL:", subtotalSinImpuesto.ToString("0.00"), e, ref y);
            PrintAlignedText("Descuentos:", "0.00", e, ref y);
            PrintAlignedText("Importe Exento:", "0.00", e, ref y);
            PrintAlignedText("Importe Exonerado:", "0.00", e, ref y);
            PrintAlignedText("Gravado 15%:", subtotalSinImpuesto.ToString("0.00"), e, ref y);
            PrintAlignedText("Gravado 18%:", "0.00", e, ref y);
            PrintAlignedText("ISV 15%:", txtimpuesto.Text, e, ref y);
            PrintAlignedText("ISV 18%:", "0.00", e, ref y);

            // Resaltar TOTAL A PAGAR
            PrintAlignedText("TOTAL A PAGAR:", txtmontototal.Text, e, ref y, true);
            y += 18;

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Imprimir total en letras
            int maxCaracteresPorLineaTotal = 40; // Límite de caracteres por línea
            List<string> lineasTotalEnLetras = new List<string>();

            for (int i = 0; i < totalEnLetras.Length; i += maxCaracteresPorLineaTotal)
            {
                lineasTotalEnLetras.Add(totalEnLetras.Substring(i, Math.Min(maxCaracteresPorLineaTotal, totalEnLetras.Length - i)));
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


            if (txtmetodopago.Text == "Efectivo")
            {
                e.Graphics.DrawString($"Efectivo:  L. {txtmontopago.Text}", fontBold, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString($"Cambio:    L. {txtmontocambio.Text}", fontBold, Brushes.Black, 10, y);
                y += 13;
            }
            // Si el método de pago es tarjeta
            else if (txtmetodopago.Text == "Tarjeta")
            {
                e.Graphics.DrawString("Terminal Atlantida: L." + txtmontototal.Text, fontBold, Brushes.Black, 10, y);
                y += 13;
            }
           
            else if (txtmetodopago.Text == "Transferencia")
            {
                e.Graphics.DrawString("Transferencia", fontBold, Brushes.Black, 10, y);
                y += 13;
            }

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            // Información adicional
            string rangoAutorizado = "Rango Aut.: " + negocio.RangoAutorizado; // Reemplaza con el valor real
            string fechaLimite = "Fecha Límite: " + negocio.FechaLimiteEmision.ToString("dd/MM/yyyy"); // Reemplaza con el valor real

            // Dividir el rango autorizado en dos líneas
            string[] rangoLines = SplitText(rangoAutorizado, 32);
            foreach (string line in rangoLines)
            {
                e.Graphics.DrawString(line, fontBold, Brushes.Black, 10, y);
                y += 13;
            }
            e.Graphics.DrawString("Fecha Límite: " + negocio.FechaLimiteEmision.ToString("dd/MM/yyyy"), fontBold, Brushes.Black, 10, y);
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

            y += 20;

            string etiqueta = "COPIA";
            Font fontEtiqueta = new Font("MS Gothic", 8, FontStyle.Bold);
            SizeF etiquetaSize = e.Graphics.MeasureString(etiqueta, fontEtiqueta);

            // Calcular la posición X para alinearlo completamente a la derecha
            float xEtiqueta = pageWidth - etiquetaSize.Width - 5; // Margen derecho pequeño

            // Definir la posición Y (ajústala según sea necesario)
            float yEtiqueta = y;

            // Dibujar la etiqueta "COPIA" alineada a la derecha
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

       
        private void iconButton1_Click(object sender, EventArgs e)
        {

            Venta objVenta = new Venta();
            objVenta.MetodoPago = txtmetodopago.Text;
            if (txttipodocumento.Text == "")
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = "POS-80"; // Asegúrate que este sea el nombre correcto
                printDoc.PrintPage -= PrintPage; // Desasignar si ya estaba asignado

                // Asignar el evento PrintPage
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);

                // Imprimir una copia
                printDoc.Print();

                MessageBox.Show("Impresión enviada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnbuscar.PerformClick(); // Simula un clic en el botón btnbuscar
                e.SuppressKeyPress = true; // Evita el sonido de "ding" al presionar Enter
            }
        }
    }
    }

