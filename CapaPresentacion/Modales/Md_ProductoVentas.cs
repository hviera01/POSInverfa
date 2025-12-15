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
    public partial class Md_ProductoVentas : Form
    {
        private int _idProducto;
        public Md_ProductoVentas(int idProducto)
        {
            InitializeComponent();
            _idProducto = idProducto;
        }

        private void Md_ProductoVentas_Load(object sender, EventArgs e)
        {
            CargarVentas();
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

        private void CargarVentas()
        {
            dgvVentas.Columns.Clear();

            
            dgvVentas.Columns.Add("IdVenta", "Id Venta"); 
            dgvVentas.Columns.Add("NumeroDocumento", "Número de Documento");
            dgvVentas.Columns.Add("FechaRegistro", "Fecha");

            dgvVentas.Columns["FechaRegistro"].Width = 200;
            dgvVentas.Columns["IdVenta"].Visible = false;


            List<Venta> ventas = new CN_Venta().ObtenerVentasPorProducto(_idProducto);

           
            foreach (var venta in ventas)
            {
                string fechaFormateada = venta.FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss");

                dgvVentas.Rows.Add(venta.IdVenta, venta.NumeroDocumento, fechaFormateada);
            }
        }

        private void dgvVentas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if (dgvVentas.SelectedRows.Count > 0)
            {
                // Obtener el Número de Documento de la fila seleccionada
                string numeroDocumento = dgvVentas.SelectedRows[0].Cells["NumeroDocumento"].Value.ToString();

                // Crear instancia del formulario de detalle
                frmDetalleVenta detalleVentaForm = new frmDetalleVenta();

                // Llamar al método público para buscar y mostrar la venta
                detalleVentaForm.Load += (s, args) =>
                {
                    detalleVentaForm.BuscarPorNumeroDocumento(numeroDocumento);
                };

                // Mostrar el formulario como diálogo
                detalleVentaForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione una fila primero", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
