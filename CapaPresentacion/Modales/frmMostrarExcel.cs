using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace CapaPresentacion.Modales
{
    public partial class frmMostrarExcel : Form
    {
        private DataTable dataTableOriginal;
        public frmMostrarExcel()
        {
            InitializeComponent();
            CargarExcel(@"C:\Users\User\Desktop\Henry\Registro de colores Super Color.xlsx");

        }

        private void frmMostrarExcel_Load(object sender, EventArgs e)
        {
            txtBuscar.Select();
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            btnLimpiarBusqueda.Click += btnLimpiarBusqueda_Click;

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
        private void CargarExcel(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var range = worksheet.RangeUsed();

                    if (range == null)
                    {
                        MessageBox.Show("El archivo está vacío o no tiene datos.");
                        return;
                    }

                    dataTableOriginal = new DataTable();
                    var rows = range.RowsUsed().ToList();
                    var headerRow = rows.First();

                    foreach (var cell in headerRow.Cells())
                    {
                        dataTableOriginal.Columns.Add(cell.Value.ToString());
                    }

                    foreach (var row in rows.Skip(1))
                    {
                        var newRow = dataTableOriginal.NewRow();
                        for (int i = 0; i < dataTableOriginal.Columns.Count; i++)
                        {
                            newRow[i] = row.Cell(i + 1).Value.ToString();
                        }
                        dataTableOriginal.Rows.Add(newRow);
                    }

                    dgvExcel.DataSource = dataTableOriginal;

                    // Ajustar ancho de columnas automáticamente
                    dgvExcel.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el archivo de Excel: " + ex.Message);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(filtro))
            {
                dgvExcel.DataSource = dataTableOriginal;
                return;
            }

            // Aplicar filtro en todas las columnas
            DataTable dtFiltrado = dataTableOriginal.Clone();
            foreach (DataRow row in dataTableOriginal.Rows)
            {
                if (row.ItemArray.Any(field => field.ToString().IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    dtFiltrado.ImportRow(row);
                }
            }

            dgvExcel.DataSource = dtFiltrado;
        }

        private void btnLimpiarBusqueda_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            dgvExcel.DataSource = dataTableOriginal;
        }
    }
}

