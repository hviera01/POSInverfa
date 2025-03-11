using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using FontAwesome.Sharp;


namespace CapaPresentacion
{
    public partial class Inicio : Form
    {
        private static Usuario usuarioActual;
        private static IconMenuItem MenuActivo = null;
        private static Form FormularioActivo = null;
        private Dictionary<string, Form> formulariosAbiertos = new Dictionary<string, Form>();

        public Inicio(Usuario objusuario)
        {
            usuarioActual = objusuario;
            InitializeComponent();
            this.IsMdiContainer = true;
            AjustarAnchoBotones();
            this.Resize += Inicio_Resize;
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> ListaPermisos = new CN_Permiso().Listar(usuarioActual.IdUsuario);

            foreach (IconMenuItem iconmenu in menu.Items)
            {
                bool encontrado = ListaPermisos.Any(m => m.NombreMenu == iconmenu.Name);

                // Asegúrate de que el menú "Acerca de" esté incluido en los permisos
                if (!encontrado)
                {
                    iconmenu.Visible = false;
                }
            }

            // Verifica si el menú "Acerca de" está visible
            var acercaDeMenu = menu.Items.OfType<IconMenuItem>().FirstOrDefault(m => m.Name == "menuacercade");
            if (acercaDeMenu != null && !acercaDeMenu.Visible)
            {
                MessageBox.Show("El menú 'Acerca de' no está visible. Verifica los permisos.");
            }

            lblusuario.Text = usuarioActual.NombreCompleto;
        }

        private void AjustarAnchoBotones()
        {
            int anchoPorBoton = menu.Width / menu.Items.Count;

            foreach (ToolStripItem item in menu.Items)
            {
                item.AutoSize = false; // Desactivar tamaño automático
                item.Width = anchoPorBoton; // Establecer el ancho calculado
                item.TextAlign = ContentAlignment.MiddleCenter; // Centrar el texto
            }
        }

        private void Inicio_Resize(object sender, EventArgs e)
        {
            // Recalcular el ancho de los botones al redimensionar
           AjustarAnchoBotones();
        }

        private void AbrirForulario(IconMenuItem menu, Form formulario)
        {
            if (MenuActivo != null)
            {
                MenuActivo.BackColor = Color.White;
            }
            menu.BackColor = Color.Silver;
            MenuActivo = menu;

            if (FormularioActivo != null && FormularioActivo != formulario)
            {
                FormularioActivo.Hide();
            }

            if (!formulariosAbiertos.ContainsKey(formulario.Name))
            {
                formulario.TopLevel = false;
                formulario.FormBorderStyle = FormBorderStyle.None;
                formulario.Dock = DockStyle.Fill;
                contenedor.Controls.Add(formulario);
                formulariosAbiertos[formulario.Name] = formulario;
            }
            FormularioActivo = formulario;
            formulario.Show();
            formulario.BringToFront();
        }

        private void menuusuarios_Click(object sender, EventArgs e)
        {
            AbrirForulario((IconMenuItem)sender, new frmUsuarios());
        }

        private void iconMenuItem3_Click(object sender, EventArgs e)
        {
            AbrirForulario(menumantenedor, new frmCategoria());
        }

        private void SubMenuProducto_Click(object sender, EventArgs e)
        {
            AbrirForulario(menumantenedor, new frmProducto(usuarioActual));
        }

        private void SubMenuRegistrarVenta_Click(object sender, EventArgs e)
        {
            AbrirForulario(menuventas, new frmVentas(usuarioActual));
        }

        private void SubMenuVerDetalleVenta_Click(object sender, EventArgs e)
        {
            AbrirForulario(menuventas, new frmDetalleVenta());
        }

        private void SubMenuRegistrarCompra_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucompras, new frmCompras(usuarioActual));
        }

        private void SubMenuVerDetalleCompra_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucompras, new frmDetalleCompra());
        }

        private void iconMenuItmenuclientesem6_Click(object sender, EventArgs e)
        {
            AbrirForulario((IconMenuItem)sender, new frmClientes());
        }

     

     

        private void contenedor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void submenunegocio_Click(object sender, EventArgs e)
        {
            AbrirForulario(menumantenedor, new frmNegocio());
        }

        private void menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Recorremos todos los elementos del menú
            foreach (ToolStripMenuItem item in menu.Items)
            {
                // Desmarcamos todos los elementos
                item.Checked = false;
            }

            // Marcamos solo el elemento seleccionado
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            if (selectedItem != null)
            {
                selectedItem.Checked = true;
            }
        }

        private void submenureportecompras_Click(object sender, EventArgs e)
        {
            AbrirForulario(menureportes, new frmReporteCompras());
        }

        private void submenureporteventas_Click(object sender, EventArgs e)
        {
            frmReporteVentas frm = new frmReporteVentas();
            AbrirForulario(menureportes, frm);
        }

        private void menuproveedores_Click(object sender, EventArgs e)
        {
            AbrirForulario(menuproveedores, new frmProveedores());
        }

        private void menuacercade_Click(object sender, EventArgs e)
        {
            mdAcercade md = new mdAcercade();
            md.ShowDialog();
        }

        private void btnsalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Desea salir?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

     

        private void hACERPEDIDOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucompras, new FrmPedidos());
        }

        private void rEGISTRODECOLORESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirForulario(menumantenedor, new FrmRegistroColores());
        }

       

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucreditos, new FrmVentasCredito());
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucreditos, new FrmComprasCredito());
        }
    }
}

