using CapaEntidad;
using CapaNegocio;
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

namespace CapaPresentacion
{
    public partial class frmNegocio : Form
    {

        public frmNegocio()
        {
            InitializeComponent();
         
        }

        public Image ByteToImage(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
        }


        public void frmNegocio_Load(object sender, EventArgs e)
        {
            
            bool obtenido = true;

            // Obtener y cargar el logo
            byte[] byteimage = new CN_Negocio().ObtenerLogo(out obtenido);
            if (obtenido)
                picLogo.Image = ByteToImage(byteimage);

            // Obtener los datos del negocio
            Negocio datos = new CN_Negocio().ObtenerDatos();
            if (datos.FechaLimiteEmision >= dtpfechalimite.MinDate && datos.FechaLimiteEmision <= dtpfechalimite.MaxDate)
            {
                dtpfechalimite.Value = datos.FechaLimiteEmision; // Asignar la fecha si es válida
            }
            else
            {
                dtpfechalimite.Value = dtpfechalimite.MinDate; // Asignar un valor por defecto si no es válida
            }

            // Cargar los datos en los TextBox y otros controles
            txtnombre.Text = datos.Nombre;
            txtruc.Text = datos.RUC;
            txtdireccion.Text = datos.Direccion;
            txttelefono.Text = datos.Telefono; // Nuevo campo
           
            txtcorreo.Text = datos.CorreoElectronico; // Nuevo campo
            txtcai.Text = datos.CAI;
            txteslogan.Text = datos.Eslogan;// Nuevo campo

            // Cargar el rango autorizado en los TextBox correspondientes
            if (!string.IsNullOrEmpty(datos.RangoAutorizado))
            {
                string[] partesRango = datos.RangoAutorizado.Split(new[] { " al " }, StringSplitOptions.None);
                if (partesRango.Length == 2)
                {
                    // Asignar la parte inicial del rango (sin los últimos 8 dígitos)
                    txtRangoInicial.Text = partesRango[0].Substring(0, partesRango[0].Length - 8); // Parte inicial del rango
                                                                                                   // Asignar los últimos 8 dígitos de la parte inicial
                    txtRangoInicialNumerico.Text = partesRango[0].Substring(partesRango[0].Length - 8); // Últimos 8 dígitos de la parte inicial
                                                                                                        // Asignar los últimos 8 dígitos de la parte final
                    txtRangoFinalNumerico.Text = partesRango[1].Substring(partesRango[1].Length - 8);
                }
            }  
            // Cargar logo en blanco y negro si es necesario
            byte[] logoBlancoNegro = new CN_Negocio().ObtenerLogoBlancoNegro(out obtenido);
            if (obtenido)
                picLogoBlancoNegro.Image = ByteToImage(logoBlancoNegro);


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





        private void btnsubir_Click_1(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            OpenFileDialog oOpenFileDialog = new OpenFileDialog();
            oOpenFileDialog.FileName = "Files|*.jpg;*.jpeg;*.png";

            if (oOpenFileDialog.ShowDialog() == DialogResult.OK)
            {

                byte[] byteimage = File.ReadAllBytes(oOpenFileDialog.FileName);
                bool respuesta = new CN_Negocio().ActualizarLogo(byteimage, out mensaje);

                if (respuesta)
                    picLogo.Image = ByteToImage(byteimage);
                else
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        private void btnguardarcambios_Click_1(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            string rangoAutorizado = txtRangoInicial.Text + txtRangoInicialNumerico.Text + " al " + txtRangoInicial.Text + txtRangoFinalNumerico.Text;

            Negocio obj = new Negocio()
            {
                Nombre = txtnombre.Text,
                RUC = txtruc.Text,
                Direccion = txtdireccion.Text,
                Telefono = txttelefono.Text,
                FechaLimiteEmision = dtpfechalimite.Value,
                CorreoElectronico = txtcorreo.Text,
                CAI = txtcai.Text,
                RangoAutorizado = rangoAutorizado,
                RangoInicial = txtRangoInicial.Text,
                Eslogan = txteslogan.Text
            };

            bool respuesta = new CN_Negocio().GuardarDatos(obj, out mensaje);

            if (respuesta)
                MessageBox.Show("Los cambios fueron guardados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudo guardar los cambios: " + mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnsubirlogobn_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            OpenFileDialog oOpenFileDialog = new OpenFileDialog();
            oOpenFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";

            if (oOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] byteimage = File.ReadAllBytes(oOpenFileDialog.FileName);
                bool respuesta = new CN_Negocio().ActualizarLogoBlancoNegro(byteimage, out mensaje);

                if (respuesta)
                    picLogoBlancoNegro.Image = ByteToImage(byteimage);
                else
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}