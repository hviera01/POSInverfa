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

namespace CapaPresentacion.Modales
{
    public partial class Md_ProductoCompras : Form
    {
        private int _idProducto;
        public Md_ProductoCompras(int idProducto)
        {
            InitializeComponent();
            _idProducto = idProducto;
        }

        private void Md_ProductoCompras_Load(object sender, EventArgs e)
        {
            CargarCompras();

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                    btn.Click += (s, args) => this.ActiveControl = null;
            }
        }

        private void CargarCompras()
        {
            dgvCompras.Columns.Clear();
            dgvCompras.Rows.Clear();

            dgvCompras.Columns.Add("IdCompra", "Id Compra");
            dgvCompras.Columns.Add("NumeroDocumento", "Número de Documento");
            dgvCompras.Columns.Add("FechaRegistro", "Fecha");

            dgvCompras.Columns["IdCompra"].Visible = false;
            dgvCompras.Columns["FechaRegistro"].Width = 200;

            List<Compra> compras = new CN_Compra().ObtenerComprasPorProducto(_idProducto);

            foreach (var compra in compras)
            {
                dgvCompras.Rows.Add(compra.IdCompra, compra.NumeroDocumento, compra.FechaRegistro);
            }
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {

            if (dgvCompras.SelectedRows.Count > 0)
            {
                string numeroDocumento = dgvCompras.SelectedRows[0].Cells["NumeroDocumento"].Value.ToString();

                frmDetalleCompra detalleForm = new frmDetalleCompra();
                detalleForm.Load += (s, args) =>
                {
                    detalleForm.BuscarPorNumeroDocumento(numeroDocumento);
                };

                detalleForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione una fila primero", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
