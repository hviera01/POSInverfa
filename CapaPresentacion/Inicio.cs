using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using FontAwesome.Sharp;


namespace CapaPresentacion
{
    public partial class Inicio : ResponsiveFormBase
    {
        protected override Size DesignSize => new Size(1370, 450);
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

        private async void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> ListaPermisos = new CN_Permiso().Listar(usuarioActual.IdUsuario);

            // 2. Recorrer el menú principal y mostrar u ocultar según permisos
            foreach (IconMenuItem iconmenu in menu.Items)
            {
                bool tienePermisoMenu = ListaPermisos.Any(m => m.NombreMenu == iconmenu.Name);

                iconmenu.Visible = tienePermisoMenu;

                if (tienePermisoMenu)
                {
                    // Validar submenús
                    foreach (ToolStripItem subItem in iconmenu.DropDownItems)
                    {
                        if (subItem is ToolStripMenuItem subMenu)
                        {
                            bool tienePermisoSubMenu = ListaPermisos.Any(p => p.NombreMenu == subMenu.Name);
                            subMenu.Visible = tienePermisoSubMenu;
                        }
                    }
                }
            }

            // 3. Validar visibilidad del menú AcercaDe
            var acercaDeMenu = menu.Items.OfType<IconMenuItem>().FirstOrDefault(m => m.Name == "menuacercade");
            if (acercaDeMenu != null && !acercaDeMenu.Visible)
            {
                MessageBox.Show("El menú 'Acerca de' no está visible. Verifica los permisos.");
            }

            // 4. Mostrar el nombre del usuario
            lblusuario.Text = usuarioActual.NombreCompleto;

            // 5. (Opcional) Agregar eventos click a todos los botones del formulario principal
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }

            // 6. Precargar formularios comunes en segundo plano para mejorar velocidad al abrir
            await Task.Run(() =>
            {
                var frmVentasPrecarga = new frmVentas(usuarioActual);
                var frmClientesPrecarga = new frmProducto(usuarioActual);

                this.Invoke(new Action(() =>
                {
                    if (!formulariosAbiertos.ContainsKey("frmVentas"))
                        formulariosAbiertos["frmVentas"] = frmVentasPrecarga;

                    if (!formulariosAbiertos.ContainsKey("frmProducto"))
                        formulariosAbiertos["frmProducto"] = frmClientesPrecarga;

                    // 🔽 Aquí es donde se aplica el bloque optimizado
                    contenedor.SuspendLayout();
                    foreach (var form in formulariosAbiertos.Values)
                    {
                        form.TopLevel = false;
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.Dock = DockStyle.Fill;
                        contenedor.Controls.Add(form);
                        form.Hide(); // no mostrar aún, solo dejar en memoria
                    }
                    contenedor.ResumeLayout();
                }));
            });
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
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
            if (MenuActivo != null) MenuActivo.BackColor = Color.White;
            menu.BackColor = Color.Silver;
            MenuActivo = menu;

            if (formulariosAbiertos.ContainsKey(formulario.Name))
            {
                var f = formulariosAbiertos[formulario.Name];
                f.Dock = DockStyle.Fill;        // <- reafirma
                f.Show();
                f.BringToFront();
                FormularioActivo = f;
                return;
            }

            if (FormularioActivo != null && FormularioActivo != formulario)
                FormularioActivo.Hide();

            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;   // <- importante
            contenedor.Controls.Add(formulario);
            formulariosAbiertos[formulario.Name] = formulario;
            FormularioActivo = formulario;
            formulario.Show();
            formulario.BringToFront();
        }

        private void RegistrarFormulario(string key, IconMenuItem menu, Func<Form> creadorFormulario)
        {
            if (MenuActivo != null)
                MenuActivo.BackColor = Color.White;

            menu.BackColor = Color.Silver;
            MenuActivo = menu;

            if (!formulariosAbiertos.ContainsKey(key))
            {
                var form = creadorFormulario.Invoke();
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                contenedor.Controls.Add(form);
                formulariosAbiertos[key] = form;
            }

            var f = formulariosAbiertos[key];

            if (FormularioActivo != null && FormularioActivo != f)
                FormularioActivo.Hide();

            f.Show();
            f.BringToFront();
            FormularioActivo = f;
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
            RegistrarFormulario("frmProducto", menumantenedor, () => new frmProducto(usuarioActual));
        }

        private void SubMenuRegistrarVenta_Click(object sender, EventArgs e)
        {
            RegistrarFormulario("frmVentas", menuventas, () => new frmVentas(usuarioActual));
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
            AbrirForulario(menureportes, new frmReporteVentas());
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

    
       

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucreditos, new FrmVentasCredito());
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            AbrirForulario(menucreditos, new FrmComprasCredito());
        }

       

        private void subMenuCierre_Click(object sender, EventArgs e)
        {
            using (frmCierreCaja frm = new frmCierreCaja(usuarioActual))
            {
                frm.ShowDialog();
            }
        }

        private void subMenuEgresos_Click(object sender, EventArgs e)
        {
            using (FrmEgresos frm = new FrmEgresos(usuarioActual))
            {
                frm.ShowDialog();
            }
        }

        private void rEPORTEFINANCIEROToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirForulario(menureportes, new frmFinanzas());
        }

        private void rEGISTRODECOLORESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}

