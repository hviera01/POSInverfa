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
using CapaEntidad;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            List<Usuario> TEST = new CN_Usuario().Listar();

            Usuario ousuario = new CN_Usuario().Listar().Where(u => u.Documento == TxtDocumento.Text && u.Clave == TxtClave.Text).FirstOrDefault();

            if (ousuario != null)
            {
                Inicio form = new Inicio(ousuario);

                form.Show();
                this.Hide();

                form.FormClosing += frm_closing;
            }
            else
            {
                MessageBox.Show("Usuario o clave incorrectos.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                TxtDocumento.Text = "";
                TxtClave.Text = "";

                // Asegurarse de que el control reciba el enfoque después de que se cierre el MessageBox
                Application.DoEvents();  // Permite que el formulario actualice el enfoque
                TxtDocumento.Select();
            }

        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            TxtDocumento.Text = "";
            TxtClave.Text = "";
            this.Show();
            TxtDocumento.Select();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += Login_KeyDown;

            TxtDocumento.KeyDown += Login_KeyDown;
            TxtClave.KeyDown += Login_KeyDown;

          

        }

      

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; 
                BtnIngresar.PerformClick(); 
            }
        }

        private void TxtDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla
            }
        }

        
    }
}