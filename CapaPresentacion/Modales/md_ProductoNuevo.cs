using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Modales
{
    public partial class md_ProductoNuevo : Form
    {
        private readonly Usuario _usuario;
        private readonly Producto _productoOriginal;   // null = nuevo
        private readonly CN_Producto _cnProducto = new CN_Producto();
        private bool _huboCambios = false;
        public md_ProductoNuevo(Usuario usuario, Producto producto = null)
        {
            _usuario = usuario;
            _productoOriginal = producto;

            InitializeComponent();
        }

        private void md_ProductoNuevo_Load(object sender, EventArgs e)
        {
            CargarCombos();

            

            if (_productoOriginal != null)
            {
                CargarProductoEnControles(_productoOriginal);
            }
            else
            {
                txtid.Text = "0";
                txtPrima.Text = "0.00";
                TxtNuevoStock.Text = "0";
            }
        }

        private void CargarCombos()
        {
            // ===== ESTADO =====
            CboEstado.Items.Clear();
            CboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            CboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No activo" });
            CboEstado.DisplayMember = "Texto";
            CboEstado.ValueMember = "Valor";
            CboEstado.SelectedIndex = 0;

            // ===== CATEGORÍA =====
            CboCategoria.Items.Clear();

            List<Categoria> listaCategoria = new CN_Categoria().Listar();
            foreach (Categoria item in listaCategoria)
            {
                CboCategoria.Items.Add(new OpcionCombo()
                {
                    Valor = item.IdCategoria,
                    Texto = item.Descripcion
                });
            }

            CboCategoria.DisplayMember = "Texto";
            CboCategoria.ValueMember = "Valor";

            if (CboCategoria.Items.Count > 0)
                CboCategoria.SelectedIndex = 0;
        }

        private void CargarProductoEnControles(Producto p)
        {
            txtid.Text = p.IdProducto.ToString();
            TxtCodigo.Text = p.Codigo;
            TxtNombre.Text = p.Nombre;
            TxtDescripcion.Text = p.Descripcion ?? "";

            txtprecio.Text = p.PrecioVenta.ToString("0.00");
            
            txtcosto.Text = p.PrecioCompra.ToString("0.00");
            TxtNuevoStock.Text = p.Stock.ToString("0.##");

            txtPrima.Text = p.Prima.ToString("0.00");

            // Categoría
            if (p.oCategoria != null)
            {
                foreach (OpcionCombo oc in CboCategoria.Items)
                {
                    if (Convert.ToInt32(oc.Valor) == p.oCategoria.IdCategoria)
                    {
                        CboCategoria.SelectedIndex = CboCategoria.Items.IndexOf(oc);
                        break;
                    }
                }
            }

            // Estado
            int valEstado = p.Estado ? 1 : 0;
            foreach (OpcionCombo oc in CboEstado.Items)
            {
                if (Convert.ToInt32(oc.Valor) == valEstado)
                {
                    CboEstado.SelectedIndex = CboEstado.Items.IndexOf(oc);
                    break;
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje;

            // Código de barras automático si viene vacío
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
                TxtCodigo.Text = GenerarCodigoBarrasUnico();

            decimal.TryParse(txtPrima.Text.Trim(), out decimal prima);
            if (prima < 0) prima = 0;

            decimal.TryParse(txtprecio.Text.Trim(), out decimal precioVenta);
            decimal.TryParse(txtcosto.Text.Trim(), out decimal precioCompra);
            decimal.TryParse(TxtNuevoStock.Text.Trim(), out decimal stockNuevo);

            if (stockNuevo < 0) stockNuevo = 0;

            Producto obj = new Producto()
            {
                IdProducto = int.TryParse(txtid.Text, out int idP) ? idP : 0,
                Codigo = TxtCodigo.Text.Trim(),
                Nombre = TxtNombre.Text.Trim(),
                Descripcion = TxtDescripcion.Text.Trim(),
                Prima = prima,
                Stock = stockNuevo,
                PrecioVenta = precioVenta,
                PrecioCompra = precioCompra,
                oCategoria = new Categoria()
                {
                    IdCategoria = Convert.ToInt32(((OpcionCombo)CboCategoria.SelectedItem).Valor)
                },
                Estado = Convert.ToInt32(((OpcionCombo)CboEstado.SelectedItem).Valor) == 1
            };

            int idGenerado = _cnProducto.GuardarProductoCompleto(obj, _usuario.IdUsuario, out mensaje);

            if (idGenerado != 0)
            {
                if (obj.IdProducto == 0)
                    MessageBox.Show("Producto registrado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Producto actualizado correctamente.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // el SP devuelve el Id definitivo
                txtid.Text = idGenerado.ToString();

                // 👉 MARCAR QUE HUBO CAMBIOS
                _huboCambios = true;

                // Limpiar para permitir crear/editar otro sin cerrar el modal
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LimpiarCampos()
        {
            txtid.Text = "0";
            TxtCodigo.Clear();
            TxtNombre.Clear();
            TxtDescripcion.Clear();
            txtprecio.Clear();
            
            txtcosto.Clear();
            TxtNuevoStock.Text = "0";
            txtPrima.Text = "0.00";

            if (CboCategoria.Items.Count > 0)
                CboCategoria.SelectedIndex = 0;

            CboEstado.SelectedIndex = 0;

            TxtCodigo.Focus();
        }

        private string GenerarCodigoBarrasUnico()
        {
            string caracteres = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rnd = new Random();
            string codigo = new string(Enumerable.Repeat(caracteres, 12)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
            return codigo;
        }

        private void txtPrima_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;

            if ((e.KeyChar == '.') && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_productoOriginal == null || _productoOriginal.IdProducto == 0)
            {
                MessageBox.Show("Este producto todavía no está guardado en la base de datos.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var r = MessageBox.Show(
                "¿Desea eliminar el producto:\n\n" + _productoOriginal.Nombre + " ?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (r != DialogResult.Yes)
                return;

            string mensaje = string.Empty;
            Producto obj = new Producto()
            {
                IdProducto = _productoOriginal.IdProducto
            };

            bool respuesta = new CN_Producto().Eliminar(obj, out mensaje);

            if (respuesta)
            {
                MessageBox.Show("Producto eliminado correctamente.", "Mensaje",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 👉 MARCAR CAMBIO Y CERRAR
                _huboCambios = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "No se pudo eliminar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void md_ProductoNuevo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_huboCambios && this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
