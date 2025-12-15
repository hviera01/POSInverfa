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
    public partial class mdNuevoProductoDesdeCompra : Form
    {
        public Producto ProductoNuevo { get; private set; }
        public mdNuevoProductoDesdeCompra(string codigo)
        {
            InitializeComponent();
            txtcodigo.Text = codigo;
        }

        private void mdNuevoProductoDesdeCompra_Load(object sender, EventArgs e)
        {
            List<Categoria> categorias = new CN_Categoria().Listar();
            foreach (Categoria item in categorias)
            {
                cbocategoria.Items.Add(new OpcionCombo() { Valor = item.IdCategoria, Texto = item.Descripcion });
            }

            cbocategoria.DisplayMember = "Texto";
            cbocategoria.ValueMember = "Valor";
            cbocategoria.SelectedIndex = 0;

            // Estado fijo como Activo (opcional si no quieres que elija)
            cboestado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboestado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboestado.DisplayMember = "Texto";
            cboestado.ValueMember = "Valor";
            cboestado.SelectedIndex = 0;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Producto obj = new Producto()
            {
                Codigo = txtcodigo.Text.Trim(),
                Nombre = txtnombre.Text.Trim(),
                Descripcion = txtdescripcion.Text.Trim(),
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cbocategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboestado.SelectedItem).Valor) == 1
            };

            int idGenerado = new CN_Producto().Registrar(obj, out mensaje);

            if (idGenerado != 0)
            {
                obj.IdProducto = idGenerado;
                ProductoNuevo = obj;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnbuscarproducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto())
            {
                modal.Text = "Consulta de Productos";
                modal.ShowDialog();

            }
        }
    }
}
