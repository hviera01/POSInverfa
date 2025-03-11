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

    public partial class mdProducto : Form
    {

        public Producto _Producto { get; set; }

        public mdProducto()
        {
            InitializeComponent();
        }

        private void mdProducto_Load(object sender, EventArgs e)
        {
            txtbusqueda.Select();

            // Limpiar el ComboBox antes de agregar elementos
            cbobusqueda.Items.Clear();

            // Definir el orden de las columnas que deseas agregar al ComboBox
            List<string> columnasOrdenadas = new List<string>
    {
        "Nombre",
        "Codigo",
        "IdProducto",
        "Descripcion",
        "Categoria",
        "Stock",
        "PrecioCompra",
        "PrecioVenta"
    };

            // Agregar las columnas al ComboBox en el orden definido
            foreach (string columna in columnasOrdenadas)
            {
                // Verificar si la columna existe en el DataGridView
                if (dgvdata.Columns.Contains(columna))
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna, Texto = dgvdata.Columns[columna].HeaderText });
                }
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0; // Selecciona el primer elemento (Nombre)

            // Cargar los datos en el DataGridView
            List<Producto> lista = new CN_Producto().Listar();

            foreach (Producto item in lista)
            {
                dgvdata.Rows.Add(new object[] {
            item.IdProducto,
            item.Codigo,
            item.Nombre,
            item.Descripcion,
            item.oCategoria.Descripcion,
            item.oCategoria.IdCategoria,
            item.Stock,
            item.PrecioCompra,
            item.PrecioVenta
        });
            }

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

            txtbusqueda.Select();
        }

        private void FiltrarDatos()
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }
        }

        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            if (iRow >= 0)
            {
                // Crear una nueva instancia de Producto y asignar los valores de la fila seleccionada
                _Producto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dgvdata.Rows[iRow].Cells["Id"].Value.ToString()), // Asegúrate de que el nombre de la celda sea correcto
                    Codigo = dgvdata.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dgvdata.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Descripcion = dgvdata.Rows[iRow].Cells["Descripcion"].Value.ToString(),
                    Stock = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["Stock"].Value ?? 0),
                    PrecioCompra = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioCompra"].Value.ToString()),
                    PrecioVenta = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioVenta"].Value.ToString()),
                    oCategoria = new Categoria() // Asumiendo que tienes una clase Categoria
                    {
                        IdCategoria = Convert.ToInt32(dgvdata.Rows[iRow].Cells["IdCategoria"].Value.ToString()) // Asignar el IdCategoria
                    }
                };

                // Establecer el resultado del diálogo y cerrar el formulario
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            if (dgvdata.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvdata.Rows)
                {

                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtbusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                row.Visible = true;
            }

        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                iconButton1_Click(sender, e); // Llama al método del botón
            }
        }

        private void dgvdata_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvdata.CurrentRow != null)
                {
                    dgvdata_CellDoubleClick(sender, new DataGridViewCellEventArgs(dgvdata.CurrentCell.ColumnIndex, dgvdata.CurrentCell.RowIndex));
                }
            }
        }

     
    }
}
