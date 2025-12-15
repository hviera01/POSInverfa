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
            var lista = new CN_HistorialStock().Listar(idProducto)
                .Select(h => new
                {
                    h.Fecha,
                    h.Stock_Anterior,
                    h.Stock_Nuevo,
                    Movimiento = h.TipoMovimiento == "AJUSTE"
                        ? "Ajuste manual"
                        : h.TipoMovimiento + " #" + h.NumeroDocumento,
                    Usuario = h.NombreUsuario
                }).ToList();

            dgvHistorial.DataSource = lista;

            dgvHistorial.Columns["Fecha"].HeaderText = "Fecha";
            dgvHistorial.Columns["Stock_Anterior"].HeaderText = "Stock Anterior";
            dgvHistorial.Columns["Stock_Nuevo"].HeaderText = "Stock Nuevo";
            dgvHistorial.Columns["Movimiento"].HeaderText = "Tipo Movimiento";
            dgvHistorial.Columns["Usuario"].HeaderText = "Usuario";

            dgvHistorial.Columns["Fecha"].Width = 150;
            dgvHistorial.Columns["Stock_Anterior"].Width = 100;
            dgvHistorial.Columns["Stock_Nuevo"].Width = 100;
            dgvHistorial.Columns["Movimiento"].Width = 250;
            dgvHistorial.Columns["Usuario"].Width = 200;

            dgvHistorial.Columns["Stock_Anterior"].DefaultCellStyle.Format = "N2";
            dgvHistorial.Columns["Stock_Nuevo"].DefaultCellStyle.Format = "N2";
        }


    }
}
