using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion.Modales
{
    public partial class FrmResumenAbonosProveedor : Form
    {
        public FrmResumenAbonosProveedor()
        {
            InitializeComponent();
        }

        private void FrmResumenAbonosProveedor_Load(object sender, EventArgs e)
        {
            dtpInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFin.Value = DateTime.Now.Date;

            CargarProveedoresEnCombo();
        }

        private void CargarProveedoresEnCombo()
        {
            // Mismo patrón que en frmReporteCompras
            var lista = new CN_Proveedor().Listar();

            cboproveedor.Items.Clear();
            cboproveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });
            foreach (Proveedor item in lista)
                cboproveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });

            cboproveedor.DisplayMember = "Texto";
            cboproveedor.ValueMember = "Valor";
            cboproveedor.SelectedIndex = 0;
            cboproveedor.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void CargarResumenAbonos()
        {
            try
            {
                DateTime fi = dtpInicio.Value.Date;
                DateTime ff = dtpFin.Value.Date;

                var lista = new CN_Compra().ObtenerResumenAbonosPorProveedor(fi, ff);

                DataTable dt = new DataTable();
                dt.Columns.Add("Proveedor", typeof(string));
                dt.Columns.Add("TotalAbonado", typeof(decimal));
                dt.Columns.Add("NumeroAbonos", typeof(int));

                decimal totalGeneral = 0;

                foreach (var x in lista)
                {
                    dt.Rows.Add(x.Proveedor, x.TotalAbonado, x.NumeroAbonos);
                    totalGeneral += x.TotalAbonado;
                }

                dgvResumenAbonos.DataSource = dt;

                dgvResumenAbonos.Columns["Proveedor"].Width = 250;
                dgvResumenAbonos.Columns["TotalAbonado"].Width = 120;
                dgvResumenAbonos.Columns["NumeroAbonos"].Width = 100;

                dgvResumenAbonos.Columns["TotalAbonado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvResumenAbonos.Columns["TotalAbonado"].DefaultCellStyle.FormatProvider = new CultureInfo("es-HN");
                dgvResumenAbonos.Columns["TotalAbonado"].DefaultCellStyle.Format = "C2";


                txtTotalAbonadoGeneral.Text = totalGeneral.ToString("C2", new CultureInfo("es-HN"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar resumen de abonos: " + ex.Message);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarResumenAbonos();
        }

        private void btnborrar_Click(object sender, EventArgs e)
        {
            cboproveedor.SelectedIndex = 0; // TODOS
            dtpInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFin.Value = DateTime.Now.Date;
            dgvResumenAbonos.DataSource = null;
            txtTotalAbonadoGeneral.Clear();
        }
    }
}
