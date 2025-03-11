using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace CapaPresentacion
{
    public partial class FrmPedidos : Form
    {
        public FrmPedidos()
        {
            InitializeComponent();
        }
        private void LimpiarFormulario()
        {
            // Limpiar DataGridView
            dgvdata.Rows.Clear();

            // Limpiar TextBox y controles relacionados
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtproducto.Text = "";
            txtcantidad.Value = 1;
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
                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.BackColor = Color.MistyRose;
                    txtidproducto.Text = "0";
                    txtproducto.Text = "";
                    txtcantidad.Value = 1;
                }
            }
        }
        private void limpiarProducto()
        {
            txtidproducto.Text = "0";
            txtcodproducto.Text = "";
            txtproducto.Text = "";
            txtcantidad.Value = 1;
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            bool producto_existe = false;

            if (int.Parse(txtidproducto.Text) == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            foreach (DataGridViewRow fila in dgvdata.Rows)
            {
                // Cambia "IdProducto" por el índice correcto si no tienes esa columna
                if (fila.Cells["IdProducto"].Value.ToString() == txtidproducto.Text)
                {
                    producto_existe = true;
                    break;
                }
            }
            if (!producto_existe)
            {
                // Agregar el producto al DataGridView
                dgvdata.Rows.Add(new object[] {
            txtidproducto.Text, // Asegúrate de que este es el ID del producto
            txtcodproducto.Text, // Código del producto
            txtproducto.Text, // Nombre del producto
            txtcantidad.Value.ToString(), // Cantidad
        });

                limpiarProducto();
                txtcodproducto.Select();
            }
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 4)
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
                    txtcantidad.Select();
                }
                else
                {
                    txtcodproducto.Select();
                }

            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dt = new DataTable();

            // Agregar columnas al DataTable, omitiendo IdProducto y BtnEliminar
            dt.Columns.Add("Código", typeof(string));
            dt.Columns.Add("Producto", typeof(string));
            dt.Columns.Add("Cantidad", typeof(string));

            // Recorrer las filas del DataGridView
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.Visible)
                {
                    // Verificar si las celdas son null antes de intentar acceder a su valor
                    dt.Rows.Add(new object[] {
                row.Cells["Codigo"].Value?.ToString() ?? "",
                row.Cells["Producto"].Value?.ToString() ?? "",
                row.Cells["Cantidad"].Value?.ToString() ?? "",
            });
                }
            }

            // Crear el diálogo para guardar el archivo
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Pedido_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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
                    MessageBox.Show("Pedido listo", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Error al generar pedido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            if (MessageBox.Show("¿Desea enviar el archivo por WhatsApp?", "Enviar por WhatsApp", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SendFileViaWhatsApp(savefile.FileName);
            }
        }

        private void btndescargar_Click(object sender, EventArgs e)
        {
            string Texto_Html = Properties.Resources.PlantillaPedido.ToString();
            Negocio odatos = new CN_Negocio().ObtenerDatos();

            // Reemplazar los valores con la información de la venta
            Texto_Html = Texto_Html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", odatos.RUC);
            Texto_Html = Texto_Html.Replace("@direcnegocio", odatos.Direccion);
            Texto_Html = Texto_Html.Replace("@cai", "2B4EDC-41CCD8-55AFE0-63BE03-09094A-1B");

            // Crear las filas para la tabla de productos de la venta
            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Codigo"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "</tr>";
            }

            // Reemplazar las filas de productos en el HTML
            Texto_Html = Texto_Html.Replace("@filas", filas);

            // Guardar como PDF
            string prefijoDocumento = "PEDIDO";

            // Guardar como PDF con el nombre correcto
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("{0}_{1}.pdf", prefijoDocumento, DateTime.Now.ToString("ddMMyyyyHHmmss")); // Agregar la fecha y hora
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
                    MessageBox.Show("PEDIDO LISTO", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Preguntar si desea enviar por WhatsApp
                    if (MessageBox.Show("¿Desea enviar el archivo por WhatsApp?", "Enviar por WhatsApp", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SendFileViaWhatsApp(savefile.FileName);
                    }
                }
            }
        }

        private void SendFileViaWhatsApp(string filePath)
        {
            // Abre WhatsApp Web con el archivo adjunto
            string url = $"https://web.whatsapp.com/send?text=Hola! Aquí está el archivo que quería compartir: {filePath}";
            System.Diagnostics.Process.Start(url);
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void FrmPedidos_Load(object sender, EventArgs e)
        {

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
    }
}
