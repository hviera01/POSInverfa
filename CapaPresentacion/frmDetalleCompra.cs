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
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace CapaPresentacion
{
    public partial class frmDetalleCompra : Form
    {
        public frmDetalleCompra()
        {
            InitializeComponent();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtbusqueda.Text);

            if (oCompra.IdCompra != 0)
            {

                txtnumerodocumento.Text = oCompra.NumeroDocumento;

                txtfecha.Text = oCompra.FechaRegistro;
                txttipodocumento.Text = oCompra.TipoDocumento;
                txtusuario.Text = oCompra.oUsuario.NombreCompleto;
                txtnproveedor.Text = oCompra.oProveedor.Documento;
                txtnombreproveedor.Text = oCompra.oProveedor.RazonSocial;
                txtcondicion.Text = oCompra.Condicion;
                txtfechavencimiento.Text = oCompra.FechaVencimiento ?? ""; 
                txtnfactura.Text = oCompra.NoFactura;
                txtmetodopago.Text = oCompra.MetodoPago;
                txtImpuesto.Text = oCompra.Impuesto.ToString("0.00");
                txtEstado.Text = oCompra.Estado;

                dgvdata.Rows.Clear();
                foreach (Detalle_Compra dc in oCompra.oDetalleCompra)
                {
                    dgvdata.Rows.Add(new object[] {
                dc.oProducto.Nombre,
                dc.PrecioCompra.ToString("0.00"),
                dc.Cantidad.ToString("0.00"),
                dc.MontoTotal.ToString("0.00"),
                dc.DescuentoPorcentaje.ToString("0.00")
                 });
                }


                txtmontototal.Text = oCompra.MontoTotal.ToString("0.00");

            }
        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmDetalleCompra_Load(object sender, EventArgs e)
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

        public void BuscarPorNumeroDocumento(string numeroDocumento)
        {
            txtbusqueda.Text = numeroDocumento;
            btnbuscar.PerformClick();
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
            txtnproveedor.Text = "";
            txtnombreproveedor.Text = "";
            txtcondicion.Text = "";
            txtfechavencimiento.Text = "";
            txtmetodopago.Text = "";
            txtnfactura.Text = "";

            dgvdata.Rows.Clear();
            txtmontototal.Text = "0.00";
            txtbusqueda.Select();
            txtImpuesto.Text = "0.00";
            txtEstado.Text = "";
        }

        private void btndescargar_Click(object sender, EventArgs e)
        {
            string Condicion = txtcondicion.Text;
            string Texto_Html = string.Empty;
            if (txttipodocumento.Text == "")
            {
                MessageBox.Show("No se encontraron resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (Condicion == "Credito")
            {
                Texto_Html = Properties.Resources.PlantillaCompraCedito;
            }
            else
            {
                Texto_Html = Properties.Resources.PlantillaCompra;
            }

           
            Negocio odatos = new CN_Negocio().ObtenerDatos();

            Texto_Html = Texto_Html.Replace("@nombrenegocio", odatos.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", odatos.RUC);
            Texto_Html = Texto_Html.Replace("@direcnegocio", odatos.Direccion);
            Texto_Html = Texto_Html.Replace("@eslogan", odatos.Eslogan);
            Texto_Html = Texto_Html.Replace("@telefono", odatos.Telefono);
            Texto_Html = Texto_Html.Replace("@correo", odatos.CorreoElectronico);
            Texto_Html = Texto_Html.Replace("@cai", odatos.CAI);
            Texto_Html = Texto_Html.Replace("@tipodocumento", txttipodocumento.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtnumerodocumento.Text);


            Texto_Html = Texto_Html.Replace("@docproveedor", txtnproveedor.Text);
            Texto_Html = Texto_Html.Replace("@nombreproveedor", txtnombreproveedor.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtfecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtusuario.Text);
            Texto_Html = Texto_Html.Replace("@condicion", txtcondicion.Text);
            Texto_Html = Texto_Html.Replace("@fechavence", txtfechavencimiento.Text);
            Texto_Html = Texto_Html.Replace("@nofactura", txtnfactura.Text);
            Texto_Html = Texto_Html.Replace("@metodopago", txtmetodopago.Text);
            Texto_Html = Texto_Html.Replace("@Impuesto", txtImpuesto.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>L." + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>L." + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtmontototal.Text);

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Compra_{0}.pdf", txtnumerodocumento.Text);
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

        private void btnAnular_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtbusqueda.Text);

            if (oCompra == null || oCompra.IdCompra == 0)
            {
                MessageBox.Show("Debe buscar y cargar una compra válida antes de anular.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de anular esta compra?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                string mensaje;

                if (new CN_Compra().AnularCompra(oCompra.IdCompra, out mensaje))
                {
                    MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtbusqueda.Text = "";
                    txtfecha.Text = "";
                    txttipodocumento.Text = "";
                    txtusuario.Text = "";
                    txtnproveedor.Text = "";
                    txtnombreproveedor.Text = "";
                    txtmetodopago.Text = "";
                    txtnumerodocumento.Text = "";
                    txtmontototal.Text = "0.00";
                    txtImpuesto.Text = "0.00";
                    txtfechavencimiento.Text = "";
                    txtcondicion.Text = "";
                    dgvdata.Rows.Clear();
                    txtnfactura.Text = "";
                    txtEstado.Text = "";
                }
                else
                {
                    MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
