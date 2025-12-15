using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
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
        // Redondeo personalizado a enteros y salida "xx.00"
        private static decimal RedondearAEntero00(decimal valor)
        {
            decimal entero = Math.Truncate(valor);
            decimal frac = valor - entero;

            if (frac >= 0.90m) return entero + 1m;         // .90 -> siguiente
            if (frac > 0m && frac <= 0.05m) return entero; // .01-.05 -> anterior

            // Resto: redondeo normal a entero (0.50 sube)
            return Math.Round(valor, 0, MidpointRounding.AwayFromZero);
        }
        private static string Fmt00(decimal v) => RedondearAEntero00(v).ToString("N2");
        private static string FmtN2(decimal v) => v.ToString("N2"); // para cantidades / precios ya .00

        private void PrintPage_Handler(object sender, PrintPageEventArgs e)
    => PrintPage(sender, e, false);

        private void PrintPage_Venta(object sender, PrintPageEventArgs e)
            => PrintPagVSF(sender, e, false);

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
                txtfecha.Text = oVenta.FechaRegistro.ToString("dd/MM/yyyy");
                txttipodocumento.Text = oVenta.TipoDocumento;
                txtusuario.Text = oVenta.oUsuario.NombreCompleto;
                txtdoccliente.Text = oVenta.DocumentoCliente;
                txtnombrecliente.Text = oVenta.NombreCliente;
                txtmetodopago.Text = oVenta.MetodoPago;
                txtcondicion.Text = oVenta.Condicion;
                txtfechavencimiento.Text = oVenta.FechaVencimiento?.ToString("dd/MM/yyyy");
                txtEstado.Text = oVenta.Estado;


                dgvdata.Rows.Clear();
                foreach (Detalle_Venta dv in oVenta.oDetalle_Venta)
                {
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
            txtmetodopago.Text = "";

            dgvdata.Rows.Clear();
            txtmontototal.Text = "0.00";
            txtmontopago.Text = "0.00";
            txtmontocambio.Text = "0.00";
            txtbusqueda.Select();
            txtEstado.Text = "";
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
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtnumerodocumento.Text);
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
            var objVenta = new CD_Venta();
            var objEgreso = new CD_Egreso();

            // 1) Validaciones básicas
            if (string.IsNullOrWhiteSpace(txtnumerodocumento.Text))
            {
                MessageBox.Show("Debe ingresar un número de documento.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string numeroDocumento = txtnumerodocumento.Text.Trim();
            int idVenta = objVenta.ObtenerIdVentaPorNumeroDocumento(numeroDocumento);

            if (idVenta == 0)
            {
                MessageBox.Show("No se encontró la venta con ese número de documento o no está Activa.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de anular esta factura?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            // 2) Anular venta en BD
            if (objVenta.AnularVenta(idVenta, out string msgVenta))
            {
                // 3) Registrar egreso (blindado: fechas válidas y todos los parámetros)
                var ahora = DateTime.Now;

                // monto total seguro (si falla el parseo, 0.00)
                decimal monto = 0m;
                decimal.TryParse(txtmontototal.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out monto);

                var egreso = new Egreso
                {
                    Fecha = ahora,                                  // ← fecha válida (evita overflow)
                    Monto = monto,
                    Descripcion = "Devolución - Factura anulada #" + numeroDocumento,
                    Usuario = txtusuario.Text ?? string.Empty,
                    MetodoPago = string.IsNullOrWhiteSpace(txtmetodopago.Text) ? "Efectivo" : txtmetodopago.Text,
                    Categoria = "Negocio",                              // o "Casa" si así lo manejas
                    EsPagado = true,                                   // egreso ya realizado
                    FechaPago = ahora                                   // consistente con @EsPagado
                };

                bool okEgreso = objEgreso.Registrar(egreso, out string msgEgreso);

                // 4) Feedback
                if (okEgreso)
                    MessageBox.Show($"{msgVenta}\nEgreso registrado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show($"{msgVenta}\nEl egreso NO se registró: {msgEgreso}",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // 5) Limpiar UI
                txtbusqueda.Text = "";
                txtfecha.Text = "";
                txttipodocumento.Text = "";
                txtusuario.Text = "";
                txtdoccliente.Text = "";
                txtnombrecliente.Text = "";
                txtmetodopago.Text = "";
                txtnumerodocumento.Text = "";
                txtfechavencimiento.Text = "";
                txtcondicion.Text = "";
                txtEstado.Text = "";
                dgvdata.Rows.Clear();
                txtmontototal.Text = "0.00";
                txtmontopago.Text = "0.00";
                txtmontocambio.Text = "0.00";
            }
            else
            {
                MessageBox.Show(msgVenta, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
               
            }
            else
            {
                btnAnular.Visible = true;
                txtmetodopago.Visible = true;
                label13.Visible = true;
                iconButton1.Visible = true;
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttipodocumento.Text))
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                string tipo = (txttipodocumento.Text ?? "").Trim().ToUpperInvariant();

                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = @"\\192.168.0.9\POS-80 (copy 1)".Normalize().Replace('\u00A0', ' ').Trim();

                // Limpia posibles suscripciones anteriores
                printDoc.PrintPage -= PrintPage_Handler;
                printDoc.PrintPage -= PrintPage_Venta;
                printDoc.PrintPage -= PrintPageDetalleVenta; // por si acaso

                // Selección del handler según el tipo
                switch (tipo)
                {
                    case "VENTA":
                        printDoc.PrintPage += PrintPage_Venta;      // => PrintPagVSF
                        break;

                    case "FACTURA":
                    case "COTIZACION":
                    case "COTIZACIÓN":
                        printDoc.PrintPage += PrintPage_Handler;    // => PrintPage
                        break;

                    default:
                        MessageBox.Show("Tipo de documento no soportado para impresión: " + tipo,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                this.Activate();
                printDoc.Print();
                this.WindowState = FormWindowState.Normal;
                this.Activate();

                MessageBox.Show("Impresión enviada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }
        }

        private void PrintPagVSF(object sender, PrintPageEventArgs e, bool esCopia)
        {
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txtmontototal.Text));

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
            string tipoDocumento = string.IsNullOrEmpty(txttipodocumento.Text) ? "N/A" : txttipodocumento.Text;
            string cliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrEmpty(txtdoccliente.Text) ? "N/A" : txtdoccliente.Text;

            // Mostrar el tipo de documento en la impresión
            e.Graphics.DrawString("VENTA " + txtnumerodocumento.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm tt"), fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Atendido por: " + txtusuario.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;

            if (txtcondicion.Text == "Credito")
            {
                e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;

                // Imprime la fecha solo si viene con valor
                var fechaTxt = (txtfecha?.Text ?? "").Trim();
                if (!string.IsNullOrEmpty(fechaTxt))
                {
                    e.Graphics.DrawString("Fecha de vencimiento: " + fechaTxt, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
            }
            else // Contado
            {
                e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontNormal, Brushes.Black, 10, y);
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
            decimal montoTotal = decimal.Parse(txtmontototal.Text);
            decimal pagoCon = string.IsNullOrEmpty(txtmontopago.Text) ? 0 : decimal.Parse(txtmontopago.Text);
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
            ImprimirTextoAlineadoTicket("TOTAL A PAGAR:", txtmontototal.Text, true);

            y += 18;


            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            Venta objVenta = new Venta();
            objVenta.MetodoPago = txtmetodopago.Text;
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
            string pagoConValor = Convert.ToDecimal(txtmontopago.Text).ToString("0.00");
            string cambioValor = Convert.ToDecimal(txtmontocambio.Text).ToString("0.00");

            if (txtcondicion.Text != "Credito")
            {
                // Si el método de pago es efectivo
                if (txtmetodopago.Text == "Efectivo")
                {
                    string efectivoTexto = $"Efectivo:  L. {pagoConValor.PadLeft(10)}"; // Alinea el número a la derecha
                    string cambioTexto = $"Cambio:    L. {cambioValor.PadLeft(10)}";
                    e.Graphics.DrawString(efectivoTexto, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                    e.Graphics.DrawString(cambioTexto, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es tarjeta
                else if (txtmetodopago.Text == "Tarjeta")
                {
                    e.Graphics.DrawString("Terminal POS: L." + txtmontototal.Text, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                // Si el método de pago es transferencia
                else if (txtmetodopago.Text == "Transferencia")
                {
                    e.Graphics.DrawString("Transferencia", fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
            }
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;
           
            e.Graphics.DrawString("¡GRACIAS POR SU COMPRA!", new Font("Lucida Console", 8, FontStyle.Bold), Brushes.Black, 10, y);

            y += 20; // Ajusta este valor para controlar el espaciado

            string etiqueta = "COPIA";
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
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txtmontototal.Text));

            int pageWidth = 280; // Ancho de la impresora térmica
            Font fontNormal = new Font("MS Gothic", 8, FontStyle.Bold);
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
                SizeF sloganSize = e.Graphics.MeasureString(slogan, fontBold);
                float xSlogan = (pageWidth - sloganSize.Width) / 2;
                e.Graphics.DrawString(slogan, fontBold, Brushes.Black, xSlogan, y);
                y += (int)sloganSize.Height + 2; // Avanza un poco para la siguiente línea
            }

            string direccionCompleta = "Dirección: " + negocio.Direccion;
            string[] direccionLines = SplitText(direccionCompleta, 37); // Dividir la dirección en líneas de 32 caracteres
            foreach (string line in direccionLines)
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontBold);
                e.Graphics.DrawString(line, fontBold, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14; // Reducí el espaciado entre líneas
            }

            string ciudad = "Danli, El Paraiso";
            string rtn = "RTN: " + negocio.RUC;
            string cai = "CAI: " + negocio.CAI;
            SizeF rtnSize = e.Graphics.MeasureString(rtn, fontBold);
            e.Graphics.DrawString(ciudad, fontBold, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;
            e.Graphics.DrawString(rtn, fontBold, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;
            e.Graphics.DrawString(cai, fontBold, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
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

            SizeF whatsSize = e.Graphics.MeasureString(whatsText, fontBold);
            float totalWhatsWidth = iconSize + textXOffset + whatsSize.Width;
            float startXWhats = (pageWidth - totalWhatsWidth) / 2;

            e.Graphics.DrawImage(wtIcono, startXWhats, y + iconYAdjust, iconSize, iconSize);
            e.Graphics.DrawString(whatsText, fontBold, Brushes.Black, startXWhats + iconSize + textXOffset, y);
            y += 14;

            // Encabezado de redes sociales
            string redesHeader = "Síguenos en nuestras redes sociales:";
            SizeF redesHeaderSize = e.Graphics.MeasureString(redesHeader, fontBold);
            e.Graphics.DrawString(redesHeader, fontBold, Brushes.Black, (pageWidth - redesHeaderSize.Width) / 2, y);
            y += 14;

            // Lista de redes (centrado con íconos)
            void DibujarRedSocial(Bitmap icono, string texto)
            {
                SizeF textoSize = e.Graphics.MeasureString(texto, fontBold);
                float totalWidth = iconSize + 4 + textoSize.Width; // 4 px de separación
                float startX = (pageWidth - totalWidth) / 2;
                float iconY = y + 1; // ajuste fino para que estén alineados

                e.Graphics.DrawImage(icono, startX, iconY, iconSize, iconSize);
                e.Graphics.DrawString(texto, fontBold, Brushes.Black, startX + iconSize + 4, y);
                y += 14;
            }



            // Dibujar cada red
            DibujarRedSocial(fbIcono, "Facebook: American colors Honduras");
            DibujarRedSocial(igIcono, "Instagram: @american.colors33");
            DibujarRedSocial(ttIcono, "TikTok: @americancolololorshn");
            y += 5;
            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            string tipoDocumento = txttipodocumento.Text?.Trim() ?? "";
            bool esCotizacion = tipoDocumento.Equals("COTIZACION", StringComparison.OrdinalIgnoreCase);

            string cliente = string.IsNullOrWhiteSpace(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrWhiteSpace(txtdoccliente.Text) ? "N/A" : txtdoccliente.Text;

            // Encabezado (con o sin rango fiscal)
            string encabezado = esCotizacion
                ? $"COTIZACIÓN {txtnumerodocumento.Text}"
                : $"{tipoDocumento.ToUpper()} {negocio.RangoInicial}{txtnumerodocumento.Text}";

            e.Graphics.DrawString(encabezado, fontBold, Brushes.Black, 10, y); y += 13;
            e.Graphics.DrawString("Fecha: " + txtfecha.Text, fontBold, Brushes.Black, 10, y); y += 13;
            e.Graphics.DrawString("Atendido por: " + txtusuario.Text, fontBold, Brushes.Black, 10, y); y += 13;

            // Condición / vencimiento solo si NO es cotización
            if (!esCotizacion)
            {
                if (txtcondicion.Text == "Credito")
                {
                    e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontBold, Brushes.Black, 10, y); y += 13;
                    e.Graphics.DrawString($"Fecha de vencimiento: {txtfechavencimiento.Text}", fontBold, Brushes.Black, 10, y); y += 13;
                }
                else if (txtcondicion.Text == "Contado")
                {
                    e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontBold, Brushes.Black, 10, y); y += 13;
                }
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
                SizeF headerSize = printEventArgs.Graphics.MeasureString(header, fontBold);
                float xHeader = xAlignHeader - headerSize.Width;
                printEventArgs.Graphics.DrawString(header, isTotal ? fontNormal : fontBold, Brushes.Black, xHeader, lineY);

                // Dibuja la "L." en la posición correcta
                printEventArgs.Graphics.DrawString("L.", isTotal ? fontNormal : fontBold, Brushes.Black, xAlignCurrency, lineY);

                // Dibuja el valor, alineado a la derecha
                printEventArgs.Graphics.DrawString(value, isTotal ? fontNormal : fontBold, Brushes.Black, xAlignValues, lineY, rightAlign);

                lineY += 13;
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


            if (!esCotizacion)
            {
                if (txtmetodopago.Text == "Efectivo")
                {
                    e.Graphics.DrawString($"Efectivo:  L. {txtmontopago.Text}", fontBold, Brushes.Black, 10, y);
                    y += 13;
                    e.Graphics.DrawString($"Cambio:    L. {txtmontocambio.Text}", fontBold, Brushes.Black, 10, y);
                    y += 13;
                }
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
            }


            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            if (!esCotizacion)
            {
                // FACTURA: datos fiscales
                string[] rangoLines = SplitText("Rango Aut.: " + negocio.RangoAutorizado, 32);
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

                string mensajeFinal = "LA FACTURA ES BENEFICIO DE TODOS, ¡EXÍJALA!";
                SizeF mensajeSize = e.Graphics.MeasureString(mensajeFinal, fontBold);
                e.Graphics.DrawString(mensajeFinal, fontBold, Brushes.Black, (pageWidth - mensajeSize.Width) / 2, y);
                y += 20;

                e.Graphics.DrawString("¡GRACIAS POR SU COMPRA!", new Font("Lucida Console", 8, FontStyle.Bold), Brushes.Black, 10, y);

                y += 20;
            }
            else
            {
                // COTIZACIÓN: leyendas no fiscales
                e.Graphics.DrawString("DOCUMENTO NO FISCAL - SOLO REFERENCIA", fontBold, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("VÁLIDA POR 15 DÍAS (o según políticas)", fontBold, Brushes.Black, 10, y);
                y += 15;
               
            }




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

        private void PrintPageDetalleVenta(object sender, PrintPageEventArgs e)
        {
            string totalEnLetras = ConvertirNumeroALetras.Convertir(decimal.Parse(txtmontototal.Text));

            int pageWidth = 280;
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
                        int logoWidth = 200;
                        int logoHeight = 200;
                        e.Graphics.DrawImage(logo, (pageWidth - logoWidth) / 2, y, logoWidth, logoHeight);
                        y += logoHeight + 14;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el logo: " + ex.Message);
            }

            string nombreNegocio = negocio.Nombre.ToUpper();
            SizeF titleSize = e.Graphics.MeasureString(nombreNegocio, fontTitle);
            float xNombre = (pageWidth - titleSize.Width) / 2;
            e.Graphics.DrawString(nombreNegocio, fontTitle, Brushes.Black, xNombre, y);

            Icon iconoOriginal = Properties.Resources.image_modified;
            Image iconoT = iconoOriginal.ToBitmap();
            int iconoAlto = (int)titleSize.Height;
            int iconoAncho = iconoT.Width * iconoAlto / iconoT.Height;
            float xIcono = xNombre + titleSize.Width;
            e.Graphics.DrawImage(iconoT, xIcono, y, iconoAncho, iconoAlto);
            y += iconoAlto + 4;

            string[] esloganlines = SplitText(negocio.Eslogan, 33);
            foreach (string line in esloganlines)
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontNormal);
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14;
            }

            string lineaExtra = "Celulares, Accesorios y Otros Productos Más";
            SizeF extraSize = e.Graphics.MeasureString(lineaExtra, fontNormal);
            e.Graphics.DrawString(lineaExtra, fontNormal, Brushes.Black, (pageWidth - extraSize.Width) / 2, y);
            y += 14;

            string direccionCompleta = "Dirección: " + negocio.Direccion;
            foreach (string line in SplitText(direccionCompleta, 33))
            {
                SizeF textSize = e.Graphics.MeasureString(line, fontNormal);
                e.Graphics.DrawString(line, fontNormal, Brushes.Black, (pageWidth - textSize.Width) / 2, y);
                y += 14;
            }

            string rtn = "RTN: " + negocio.RUC;
            SizeF rtnSize = e.Graphics.MeasureString(rtn, fontNormal);
            e.Graphics.DrawString(rtn, fontNormal, Brushes.Black, (pageWidth - rtnSize.Width) / 2, y);
            y += 14;

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            string tipoDocumento = txttipodocumento.Text;
            string cliente = string.IsNullOrEmpty(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text;
            string documentoCliente = string.IsNullOrEmpty(txtdoccliente.Text) ? "N/A" : txtdoccliente.Text;

            e.Graphics.DrawString(tipoDocumento.ToUpper() + " " + negocio.RangoInicial + txtnumerodocumento.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Estado: " + txtEstado.Text, fontBold, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Fecha: " + txtfecha.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("Atendido por: " + txtusuario.Text, fontNormal, Brushes.Black, 10, y);
            y += 13;

            if (txtcondicion.Text == "Credito")
            {
                e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
                e.Graphics.DrawString("Fecha de vencimiento: " + txtfechavencimiento.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
            }
            else
            {
                e.Graphics.DrawString("Condicion: " + txtcondicion.Text, fontNormal, Brushes.Black, 10, y);
                y += 13;
            }

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            e.Graphics.DrawString("Cliente: " + cliente, fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("ID/RTN Cliente: " + documentoCliente, fontNormal, Brushes.Black, 10, y);
            y += 13;

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            e.Graphics.DrawString("CANT  DESCRIPCION                     IMPORTE", fontNormal, Brushes.Black, 10, y);
            y += 13;
            e.Graphics.DrawString("------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            Font fontBoldProductos = new Font("MS Gothic", 7, FontStyle.Bold);
            Font fontBoldTotales = new Font("MS Gothic", 7, FontStyle.Bold);
            StringFormat rightAlign = new StringFormat() { Alignment = StringAlignment.Far };

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                string cantidad = row.Cells["Cantidad"].Value.ToString();
                string producto = row.Cells["Producto"].Value.ToString();
                string precioUnitario = Convert.ToDecimal(row.Cells["Precio"].Value).ToString("0.00");
                string subtotal = "L. " + Convert.ToDecimal(row.Cells["SubTotal"].Value).ToString("0.00");

                List<string> lineasDescripcion = new List<string>();
                for (int i = 0; i < producto.Length; i += 35)
                {
                    lineasDescripcion.Add(producto.Substring(i, Math.Min(35, producto.Length - i)));
                }

                foreach (string linea in lineasDescripcion)
                {
                    e.Graphics.DrawString(linea, fontBoldProductos, Brushes.Black, 10, y);
                    y += 12;
                }

                e.Graphics.DrawString($"{cantidad} x L. {precioUnitario}", fontBoldProductos, Brushes.Black, 15, y);
                e.Graphics.DrawString(subtotal, fontBoldTotales, Brushes.Black, 275, y, rightAlign);
                y += 14;
            }

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            decimal subtotalSinISV = dgvdata.Rows.Cast<DataGridViewRow>().Sum(r => Convert.ToDecimal(r.Cells["SubTotal"].Value));
            decimal totalISV = subtotalSinISV * 0.15m;

            int anchoPagina = e.PageBounds.Width;
            int limiteDerecho = anchoPagina - 1;
            int anchoColumna = 100;
            int xEncabezado = limiteDerecho - anchoColumna - 1;
            int xMoneda = limiteDerecho - 100;
            int xValor = limiteDerecho - 1;

            Font fuenteNegrita = new Font(fontNormal, FontStyle.Bold);

            void ImprimirTextoAlineado(string texto, string valor, bool esTotal = false)
            {
                Font fuenteUsar = esTotal ? fuenteNegrita : fontNormal;
                SizeF tamTexto = e.Graphics.MeasureString(texto, fuenteUsar);
                float xTexto = xEncabezado - tamTexto.Width;
                e.Graphics.DrawString(texto, fuenteUsar, Brushes.Black, xTexto, y);
                e.Graphics.DrawString("L.", fuenteUsar, Brushes.Black, xMoneda, y);
                e.Graphics.DrawString(valor, fuenteUsar, Brushes.Black, xValor, y, rightAlign);
                y += 13;
            }

            ImprimirTextoAlineado("SUBTOTAL:", subtotalSinISV.ToString("0.00"));
            ImprimirTextoAlineado("Descuentos:", "0.00");
            ImprimirTextoAlineado("TOTAL A PAGAR:", txtmontototal.Text, true);
            y += 18;

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            List<string> lineasTotalEnLetras = new List<string>();
            for (int i = 0; i < totalEnLetras.Length; i += 40)
            {
                lineasTotalEnLetras.Add(totalEnLetras.Substring(i, Math.Min(40, totalEnLetras.Length - i)));
            }

            if (lineasTotalEnLetras.Count > 0)
            {
                e.Graphics.DrawString("Son: " + lineasTotalEnLetras[0], fontNormal, Brushes.Black, 10, y);
                y += 13;
            }
            for (int i = 1; i < lineasTotalEnLetras.Count; i++)
            {
                e.Graphics.DrawString(lineasTotalEnLetras[i], fontNormal, Brushes.Black, 10, y);
                y += 13;
            }

            if (txtcondicion.Text != "Credito")
            {
                if (txtmetodopago.Text == "Efectivo")
                {
                    e.Graphics.DrawString($"Efectivo:  L. {txtmontopago.Text}", fontNormal, Brushes.Black, 10, y);
                    y += 13;
                    e.Graphics.DrawString($"Cambio:    L. {txtmontocambio.Text}", fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                else if (txtmetodopago.Text == "Tarjeta")
                {
                    e.Graphics.DrawString("Terminal POS: L." + txtmontototal.Text, fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
                else if (txtmetodopago.Text == "Transferencia")
                {
                    e.Graphics.DrawString("Transferencia", fontNormal, Brushes.Black, 10, y);
                    y += 13;
                }
            }

            e.Graphics.DrawString("------------------------------------------------------------------------", fontBold, Brushes.Black, 10, y);
            y += 13;

            string mensajeFinal = "¡GRACIAS POR SU COMPRA!";
            e.Graphics.DrawString(mensajeFinal, new Font("Lucida Console", 8, FontStyle.Bold), Brushes.Black, 10, y);
            y += 20;

            string etiqueta = "COPIA";
            Font fontEtiqueta = new Font("MS Gothic", 8, FontStyle.Bold);
            SizeF etiquetaSize = e.Graphics.MeasureString(etiqueta, fontEtiqueta);
            float xEtiqueta = pageWidth - etiquetaSize.Width - 5;
            e.Graphics.DrawString(etiqueta, fontEtiqueta, Brushes.Black, xEtiqueta, y);
        }

        public void BuscarPorNumeroDocumento(string numeroDocumento)
        {
            txtbusqueda.Text = numeroDocumento;
            btnbuscar.PerformClick();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttipodocumento.Text))
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Forzamos el tipo visual a COTIZACION (encabezado) pero con formato exonerado
            string tipoDocumento = "COTIZACION EXONERADA";
            decimal totalConISV = decimal.Parse(txtmontototal.Text); // Tu total actual incluye ISV (15%)
            decimal baseSinISV = Math.Round(totalConISV / 1.15m, 2, MidpointRounding.AwayFromZero);
            decimal isvHipotetico = Math.Round(totalConISV - baseSinISV, 2, MidpointRounding.AwayFromZero);

            // Totales que van en el documento exonerado
            string subtotalSin = baseSinISV.ToString("N2");
            string importeExonerado = isvHipotetico.ToString("N2");
            string isvCero = 0m.ToString("N2");
            string totalExonerado = baseSinISV.ToString("N2"); // Lo que realmente paga (sin ISV)

            // Texto en letras del total a pagar (exonerado)
            string totalEnLetrasExonerado = ConvertirNumeroALetras.Convertir(baseSinISV);

            // Selecciona la plantilla duplicada EXONERADA (recurso nuevo)
            string Texto_Html = Properties.Resources.PlantillaCotizacionExonerada;

            Negocio odatos = new CN_Negocio().ObtenerDatos();

            // Reemplazos de encabezados/datos del negocio
            Texto_Html = Texto_Html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", odatos.RUC);
            Texto_Html = Texto_Html.Replace("@direcnegocio", odatos.Direccion);
            Texto_Html = Texto_Html.Replace("@telefono", odatos.Telefono);
            Texto_Html = Texto_Html.Replace("@cai", odatos.CAI ?? "");
            Texto_Html = Texto_Html.Replace("@tipodocumento", tipoDocumento);
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtnumerodocumento.Text);
            Texto_Html = Texto_Html.Replace("@doccliente", txtdoccliente.Text);
            Texto_Html = Texto_Html.Replace("@nombrecliente", string.IsNullOrWhiteSpace(txtnombrecliente.Text) ? "CONSUMIDOR FINAL" : txtnombrecliente.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtfecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtusuario.Text);

            // Campos de exoneración que ya usas
            Texto_Html = Texto_Html.Replace("@oc", txtoc.Text);
            Texto_Html = Texto_Html.Replace("@sag", txtregsag.Text);
            Texto_Html = Texto_Html.Replace("@exonerado", txtregexo.Text);

            // ---- Filas de productos ----
            // Mostrar PRECIO UNITARIO y IMPORTE **CON ISV** (solo visual)
            // Si en tu grilla el Precio/SubTotal están sin ISV, multiplicamos *1.15.
            // Si ya vienen con ISV, comenta el *1.15 para que no dupliques.
            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.IsNewRow) continue;

                string nombre = Convert.ToString(row.Cells["Producto"].Value);
                decimal precioUnit = 0m;
                decimal.TryParse(Convert.ToString(row.Cells["Precio"].Value), out precioUnit);
                decimal cantidad = 0m;
                decimal.TryParse(Convert.ToString(row.Cells["Cantidad"].Value), out cantidad);
                decimal subTotal = 0m;
                decimal.TryParse(Convert.ToString(row.Cells["SubTotal"].Value), out subTotal);

                // Visual con ISV:
                decimal precioUnitConISV = Math.Round(precioUnit * 1.15m, 2, MidpointRounding.AwayFromZero);
                decimal subTotalConISV = Math.Round(subTotal * 1.15m, 2, MidpointRounding.AwayFromZero);

                filas += "<tr>";
                filas += "<td>" + System.Web.HttpUtility.HtmlEncode(nombre) + "</td>";
                filas += "<td>L. " + precioUnitConISV.ToString("N2") + "</td>";
                filas += "<td>" + cantidad.ToString("N2") + "</td>";
                filas += "<td>L. " + subTotalConISV.ToString("N2") + "</td>";
                filas += "</tr>";
            }

            // Reemplazos de totales (EXONERADO)
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@totalsinimpuesto", subtotalSin);      // Base
            Texto_Html = Texto_Html.Replace("@importeexonerado", importeExonerado); // ISV Exonerado
            Texto_Html = Texto_Html.Replace("@impuesto", isvCero);                  // ISV 15% mostrado = 0.00
            Texto_Html = Texto_Html.Replace("@totalpagar", totalExonerado);         // Total exonerado (= base)
            Texto_Html = Texto_Html.Replace("@totalenletras", totalEnLetrasExonerado);

            // Guardar como PDF
            using (SaveFileDialog savefile = new SaveFileDialog())
            {
                savefile.FileName = $"Cotizacion_Exonerada_{txtnumerodocumento.Text}.pdf";
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

                        if (obtenido && byteImage != null && byteImage.Length > 0)
                        {
                            var img = iTextSharp.text.Image.GetInstance(byteImage);
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
                    }

                    MessageBox.Show("Documento Exonerado Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (MessageBox.Show("¿Desea enviar el archivo por WhatsApp?", "Enviar por WhatsApp",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SendFileViaWhatsApp(savefile.FileName);
                    }
                }
            }
        }
    }
}

