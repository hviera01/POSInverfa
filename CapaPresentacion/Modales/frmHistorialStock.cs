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
using CapaDatos;

namespace CapaPresentacion.Modales
{
    public partial class frmHistorialStock : Form
    {
        private int _idProducto;
        public frmHistorialStock(int idProducto)
        {
            InitializeComponent();
            _idProducto = idProducto;
        }

        private void frmHistorialStock_Load(object sender, EventArgs e)
        {
           

            // Cargar los datos en el DataGridView
            CargarHistorial(_idProducto);

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

        private void CargarHistorial(int idProducto)
        {
            List<HistorialStock> lista = new CN_HistorialStock().Listar(_idProducto);
            dgvHistorial.DataSource = lista;
            dgvHistorial.Columns["IdProducto"].Visible = false; // Ocultar columna si no es necesaria
            dgvHistorial.Columns["ID"].Visible = false;
            dgvHistorial.Columns["Stock_Anterior"].HeaderText = "Stock Anterior";
            dgvHistorial.Columns["Stock_Nuevo"].HeaderText = "Stock Nuevo";
            dgvHistorial.Columns["Fecha"].HeaderText = "Fecha";
            dgvHistorial.Columns["oUsuario"].Visible = false;

            dgvHistorial.Columns["Stock_Anterior"].DefaultCellStyle.Format = "N2"; // Formato decimal
            dgvHistorial.Columns["Stock_Nuevo"].DefaultCellStyle.Format = "N2";


            dgvHistorial.Columns["ID"].Width = 50; // Ancho de la columna ID
            dgvHistorial.Columns["Stock_Anterior"].Width = 100; // Ancho de la columna Stock Anterior
            dgvHistorial.Columns["Stock_Nuevo"].Width = 100; // Ancho de la columna Stock Nuevo
            dgvHistorial.Columns["Fecha"].Width = 150; // Ancho de la columna Fecha
            dgvHistorial.Columns["oUsuario"].Width = 150;
            dgvHistorial.Columns["Nombre"].Width = 200;
        }
    }
}
