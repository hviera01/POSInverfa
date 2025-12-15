using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Modales
{
    public partial class mdCliente : Form
    {
        public Cliente _Cliente { get; set; }
        private List<Cliente> _listaClientesCompleta;
        public mdCliente()
        {
            InitializeComponent();
        }

        private void mdCliente_Load(object sender, EventArgs e)
        {
            txtbusqueda.Select();

            cbobusqueda.Items.Clear();
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Todo", Texto = "Buscar en todo" });

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible)
                {
                    cbobusqueda.Items.Add(new OpcionCombo()
                    {
                        Valor = columna.Name,
                        Texto = columna.HeaderText
                    });
                }
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0;

            List<Cliente> lista = new CN_Cliente().Listar();
            _listaClientesCompleta = lista.Where(c => c.Estado).ToList();

            CargarGridClientes(_listaClientesCompleta);

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }

            HabilitarDoubleBuffering(dgvdata);
        }

        private void HabilitarDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
        }

        private bool ContieneTodasLasPalabras(string campo, string[] terminosUpper)
        {
            if (string.IsNullOrWhiteSpace(campo))
                return false;

            string valor = campo.ToUpperInvariant();

            for (int i = 0; i < terminosUpper.Length; i++)
            {
                if (valor.IndexOf(terminosUpper[i], StringComparison.Ordinal) < 0)
                    return false;
            }

            return true;
        }

        private void CargarGridClientes(IEnumerable<Cliente> lista)
        {
            dgvdata.SuspendLayout();
            dgvdata.Rows.Clear();

            foreach (Cliente item in lista)
            {
                dgvdata.Rows.Add(new object[]
                {
                    item.idCliente,
                    item.CodCliente,
                    item.Documento,
                    item.NombreCompleto
                });
            }

            dgvdata.ResumeLayout();
        }

        private void FiltrarDatos()
        {
            if (_listaClientesCompleta == null || _listaClientesCompleta.Count == 0)
                return;

            if (cbobusqueda.SelectedItem == null)
                return;

            string valorFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            string texto = (txtbusqueda.Text ?? "").Trim();

            if (string.IsNullOrEmpty(texto))
            {
                CargarGridClientes(_listaClientesCompleta);
                return;
            }

            string[] terminosUpper = texto
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToUpperInvariant())
                .ToArray();

            IEnumerable<Cliente> lista = _listaClientesCompleta;

            switch (valorFiltro)
            {
                case "CodCliente":
                    if (int.TryParse(texto, out int cod))
                    {
                        lista = lista.Where(c => c.CodCliente == cod);
                    }
                    else
                    {
                        lista = Enumerable.Empty<Cliente>();
                    }
                    break;

                case "Documento":
                    lista = lista.Where(c => ContieneTodasLasPalabras(c.Documento, terminosUpper));
                    break;

                case "NombreCompleto":
                    lista = lista.Where(c => ContieneTodasLasPalabras(c.NombreCompleto, terminosUpper));
                    break;

                case "IdCliente":
                    if (int.TryParse(texto, out int idc))
                    {
                        lista = lista.Where(c => c.idCliente == idc);
                    }
                    else
                    {
                        lista = Enumerable.Empty<Cliente>();
                    }
                    break;

                case "Todo":
                default:
                    lista = lista.Where(c =>
                           ContieneTodasLasPalabras(c.Documento, terminosUpper)
                        || ContieneTodasLasPalabras(c.NombreCompleto, terminosUpper)
                        || c.CodCliente.ToString().IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                        || c.idCliente.ToString().IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                    );
                    break;
            }

            CargarGridClientes(lista.ToList());
        }

        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int iColum = e.ColumnIndex;

            if (iRow >= 0 && iColum >= 0)
            {
                _Cliente = new Cliente()
                {
                    idCliente = Convert.ToInt32(dgvdata.Rows[iRow].Cells["IdCliente"].Value), // ASIGNACIÓN CLAVE
                    Documento = dgvdata.Rows[iRow].Cells["Documento"].Value.ToString(),
                    NombreCompleto = dgvdata.Rows[iRow].Cells["NombreCompleto"].Value.ToString()
                };

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            if (txtbusqueda.TextLength == 0 && cbobusqueda.SelectedIndex == 0)
                return;

            txtbusqueda.Clear();
            cbobusqueda.SelectedIndex = 0;

            dgvdata.SuspendLayout();

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (!row.IsNewRow && !row.Visible)
                    row.Visible = true;
            }

            dgvdata.ResumeLayout();
            txtbusqueda.Focus();
        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                FiltrarDatos();
            }
        }

        private void dgvdata_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dgvdata.CurrentRow != null)
                {
                    dgvdata_CellDoubleClick(
                        sender,
                        new DataGridViewCellEventArgs(
                            dgvdata.CurrentCell.ColumnIndex,
                            dgvdata.CurrentCell.RowIndex));
                }
            }
        }
    }
}
