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

namespace CapaPresentacion
{
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            CboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            CboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No activo" });
            CboEstado.DisplayMember = "Texto";
            CboEstado.ValueMember = "Valor";
            CboEstado.SelectedIndex = 0;

            List<Rol> listaRol = new CN_Rol().Listar();


            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "BtnSeleccionar")
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            //MOSTRAR TODOS LOS USUARIOS        
            List<Cliente> lista = new CN_Cliente().Listar();

            foreach (Cliente item in lista)
            {
                {
                    dgvdata.Rows.Add(new object[] { "", item.idCliente, 
                        item.Documento, 
                        item.NombreCompleto, 
                        item.Correo,
                        item.Telefono,
                        item.Estado == true ? 1 : 0, 
                        item.Estado == true ? "Activo" : "No activo"
                    });

                }
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
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Cliente obj = new Cliente()
            {
                idCliente = Convert.ToInt32(txtid.Text),
                Documento = TxtDocumento.Text,
                NombreCompleto = TxtNombreCompleto.Text,
                Correo = TxtCorreo.Text,
                Telefono = TxtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)CboEstado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.idCliente == 0)
            {
                int idgenerado = new CN_Cliente().Registrar(obj, out mensaje);

                if (idgenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] { "", idgenerado, TxtDocumento.Text, TxtNombreCompleto.Text, TxtCorreo.Text, TxtTelefono.Text, ((OpcionCombo)CboEstado.SelectedItem).Valor.ToString(), ((OpcionCombo)CboEstado.SelectedItem).Texto.ToString() });
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else
            {
                bool resultado = new CN_Cliente().Editar(obj, out mensaje);

                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Documento"].Value = TxtDocumento.Text;
                    row.Cells["NombreCompleto"].Value = TxtNombreCompleto.Text;
                    row.Cells["Correo"].Value = TxtCorreo.Text;
                    row.Cells["Telefono"].Value = TxtTelefono.Text;
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)CboEstado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)CboEstado.SelectedItem).Texto.ToString();

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
        }
        private void Limpiar()
        {
            txtindice.Text = "-1";
            txtid.Text = "0";
            TxtDocumento.Text = "";
            TxtNombreCompleto.Text = "";
            TxtCorreo.Text = "";
            TxtTelefono.Text = "";
            CboEstado.SelectedIndex = 0;

            TxtDocumento.Select();
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.anydo_104098.Width;
                var h = Properties.Resources.anydo_104098.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.anydo_104098, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "BtnSeleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtindice.Text = indice.ToString();
                    txtid.Text = dgvdata.Rows[indice].Cells["Id"].Value.ToString();
                    TxtDocumento.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    TxtNombreCompleto.Text = dgvdata.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    TxtCorreo.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    TxtTelefono.Text = dgvdata.Rows[indice].Cells["Telefono"].Value.ToString();

                    foreach (OpcionCombo oc in CboEstado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indice_combo = CboEstado.Items.IndexOf(oc);
                            CboEstado.SelectedIndex = indice_combo;
                            break;
                        }
                    }

                }
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                if (MessageBox.Show("¿Desea eliminar el cliente?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Cliente obj = new Cliente()
                    {
                        idCliente = Convert.ToInt32(txtid.Text),

                    };

                    bool Respuesta = new CN_Cliente().Eliminar(obj, out mensaje);

                    if (Respuesta)
                    {
                        dgvdata.Rows.RemoveAt(Convert.ToInt32(txtindice.Text));
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    Limpiar();
                }
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
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

        private void TxtDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (TxtDocumento.Text.Length >= 14 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnbuscar.PerformClick(); // Simula un clic en el botón btnbuscar
                e.SuppressKeyPress = true; // Evita el sonido de "ding" al presionar Enter
            }
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

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltrarDatos();
        }
    }
}
