using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;

namespace CapaPresentacion.Modales
{
    public partial class MdCambiarClaveMenu : Form
    {
        
        public MdCambiarClaveMenu()
        {
            InitializeComponent();
            CargarTiposClave();
        }

        private void MdCambiarClaveMenu_Load(object sender, EventArgs e)
        {

        }

        private void CargarTiposClave()
        {
            // Agregar opciones al ComboBox
            cboTipoClave.Items.Add("ModificarStock");
            cboTipoClave.Items.Add("ModificarPrecio");
            cboTipoClave.Items.Add("ActualizarPuntos");

            cboTipoClave.SelectedIndex = 0; // Seleccionar el primer elemento por defecto
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            // Validar que los campos no estén vacíos
            if (string.IsNullOrEmpty(txtclaveactual.Text) || string.IsNullOrEmpty(txtnuevaclave.Text) || string.IsNullOrEmpty(txtconfirmarclave.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar que la nueva clave y la confirmación coincidan
            if (txtnuevaclave.Text != txtconfirmarclave.Text)
            {
                MessageBox.Show("La nueva clave y la confirmación no coinciden.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obtener el tipo de clave seleccionado
            string tipoClave = cboTipoClave.SelectedItem.ToString();

            // Llamar al método de la capa de negocio para cambiar la clave
            bool resultado = new CN_ClaveAcceso().CambiarClave(tipoClave, txtclaveactual.Text, txtnuevaclave.Text, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Clave cambiada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Indicar que se cerró correctamente
                this.Close(); // Cerrar el formulario
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    }

