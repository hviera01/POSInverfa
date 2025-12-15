using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Modales
{
    public partial class MdAcceso : Form
    {
        public string ClaveIngresada { get; private set; }
        public MdAcceso()
        {
            InitializeComponent();
        }

        private void MdAcceso_Load(object sender, EventArgs e)
        {
            TxtClave.Focus();
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            ClaveIngresada = TxtClave.Text.Trim();
            this.DialogResult = DialogResult.OK; // Indica que se cerró correctamente
            this.Close();
        }

        private void TxtClave_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                BtnIngresar.PerformClick(); 
                e.SuppressKeyPress = true; 
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
