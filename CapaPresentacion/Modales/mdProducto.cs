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
        public int? FiltroIdCategoria { get; set; } = null;


        public mdProducto()
        {
            InitializeComponent();
        }

        private void mdProducto_Load(object sender, EventArgs e)
        {

            txtbusqueda.Select();
            cbobusqueda.Items.Clear();

            // Agregar opción Buscar en Todo al inicio
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Todo", Texto = "Buscar en Todo" });

            List<string> columnasOrdenadas = new List<string>
    {
        "Nombre",
        "Codigo",
        "IdProducto",
        "Descripcion",
        "Categoria",
        "Stock",
        "PrecioCompra",
        "PrecioVenta",
        "Precio1",
        "Precio2",
        "PrecioPuntos",
        "Prima"
    };

            foreach (string columna in columnasOrdenadas)
            {
                if (dgvdata.Columns.Contains(columna))
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna, Texto = dgvdata.Columns[columna].HeaderText });
                }
            }

            cbobusqueda.DisplayMember = "Texto";
            cbobusqueda.ValueMember = "Valor";
            cbobusqueda.SelectedIndex = 0; // Por defecto en "Buscar en Todo"

            List<Producto> lista = new CN_Producto().Listar();

            if (FiltroIdCategoria.HasValue)
            {
                lista = lista.Where(x => x.oCategoria.IdCategoria == FiltroIdCategoria.Value).ToList();
            }
            else
            {
                lista = lista.Take(30).ToList();
            }


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
            item.PrecioVenta,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos,
            item.Prima
        });
            }

            dgvdata.Columns["Nombre"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvdata.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvdata.Columns["Nombre"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }
            HabilitarDoubleBuffering(dgvdata);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }

            txtbusqueda.Select();
        }

        private void HabilitarDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private void FiltrarDatos()
        {
            if (cbobusqueda.SelectedItem == null) return;

            // 1. Guarda estado de scroll y fila seleccionada
            int scrollIndex = dgvdata.FirstDisplayedScrollingRowIndex >= 0
                ? dgvdata.FirstDisplayedScrollingRowIndex
                : 0;
            int selectedIndex = dgvdata.CurrentRow != null
                ? dgvdata.CurrentRow.Index
                : -1;

            // 2. Prepara tokens de búsqueda
            string texto = txtbusqueda.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(texto))
            {
                // Si buscas vacío, recarga los primeros 30
                dgvdata.Rows.Clear();
                foreach (var item in new CN_Producto().Listar().Take(30))
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
                item.PrecioVenta,
                item.Precio2,
                item.Precio3,
                item.PrecioPuntos,
                item.Prima
            });
                }
                return;
            }
            string[] tokens = texto
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // 3. Lista completa
            var lista = new CN_Producto().Listar();

            // 4. Columna de filtro
            string columnaFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();

            // 5. Filtrado multitérmino
            var filtrados = lista.Where(item =>
            {
                // Prepara lista de cadenas a inspeccionar
                var valores = new List<string>();
                if (columnaFiltro == "Todo")
                {
                    valores.Add(item.IdProducto.ToString().ToUpper());
                    valores.Add(item.Codigo.ToUpper());
                    valores.Add(item.Nombre.ToUpper());
                    valores.Add(item.Descripcion.ToUpper());
                    valores.Add(item.oCategoria.Descripcion.ToUpper());
                    valores.Add(item.Stock.ToString().ToUpper());
                    valores.Add(item.PrecioCompra.ToString().ToUpper());
                    valores.Add(item.PrecioVenta.ToString().ToUpper());
                    valores.Add(item.Precio2.ToString().ToUpper());
                    valores.Add(item.Precio3.ToString().ToUpper());
                    valores.Add(item.PrecioPuntos.ToString().ToUpper());
                    valores.Add(item.Prima.ToString().ToUpper());
                }
                else
                {
                    string val = "";
                    switch (columnaFiltro)
                    {
                        case "Nombre": val = item.Nombre; break;
                        case "Codigo": val = item.Codigo; break;
                        case "IdProducto": val = item.IdProducto.ToString(); break;
                        case "Descripcion": val = item.Descripcion; break;
                        case "Categoria": val = item.oCategoria.Descripcion; break;
                        case "Stock": val = item.Stock.ToString(); break;
                        case "PrecioCompra": val = item.PrecioCompra.ToString(); break;
                        case "PrecioVenta": val = item.PrecioVenta.ToString(); break;
                        case "Precio2": val = item.Precio2.ToString(); break;
                        case "Precio3": val = item.Precio3.ToString(); break;
                        case "PrecioPuntos": val = item.PrecioPuntos.ToString(); break;
                        case "Prima":
                            val = item.Prima.ToString();
                            break;
                    }
                    valores.Add(val.ToUpper());
                }

                // Comprueba que cada token aparezca en alguno de los valores
                return tokens.All(tok => valores.Any(v => v.Contains(tok)));
            })
            .ToList();

            // 6. Vuelca resultados
            dgvdata.Rows.Clear();
            foreach (var item in filtrados)
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
            item.PrecioVenta,
            item.Precio2,
            item.Precio3,
            item.PrecioPuntos
        });
            }

            // 7. Restaura scroll y selección
            if (dgvdata.RowCount > 0)
            {
                try
                {
                    dgvdata.FirstDisplayedScrollingRowIndex = Math.Min(scrollIndex, dgvdata.RowCount - 1);
                }
                catch { }

                if (selectedIndex >= 0 && selectedIndex < dgvdata.RowCount)
                {
                    dgvdata.Rows[selectedIndex].Selected = true;
                    dgvdata.CurrentCell = dgvdata.Rows[selectedIndex]
                                             .Cells
                                             .Cast<DataGridViewCell>()
                                             .First(c => c.Visible);
                }
            }
        }


        private void dgvdata_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            if (iRow >= 0)
            {
                _Producto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dgvdata.Rows[iRow].Cells["Id"].Value.ToString()),
                    Codigo = dgvdata.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dgvdata.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Descripcion = dgvdata.Rows[iRow].Cells["Descripcion"].Value.ToString(),
                    Stock = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["Stock"].Value ?? 0),
                    PrecioCompra = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioCompra"].Value.ToString()),
                    PrecioVenta = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["PrecioVenta"].Value.ToString()),
                    Precio2 = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["Precio2"].Value.ToString()),
                    Precio3 = Convert.ToDecimal(dgvdata.Rows[iRow].Cells["Precio3"].Value.ToString()),
                    PrecioPuntos = Convert.ToInt32(dgvdata.Rows[iRow].Cells["PrecioPuntos"].Value.ToString()),
                    Prima = dgvdata.Rows[iRow].Cells["Prima"] != null
                                ? Convert.ToDecimal(dgvdata.Rows[iRow].Cells["Prima"].Value ?? 0m)
                                : 0m,                                // 👈 NUEVO
                    oCategoria = new Categoria()
                    {
                        IdCategoria = Convert.ToInt32(dgvdata.Rows[iRow].Cells["IdCategoria"].Value.ToString())
                    }
                };

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
            cbobusqueda.SelectedIndex = 0; // Seleccionar "Buscar en Todo" o primera opción

            List<Producto> lista = new CN_Producto().Listar().Take(30).ToList();
            txtbusqueda.Select();

        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            timerBuscar.Stop();
            timerBuscar.Start();
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
                    dgvdata_CellDoubleClick(sender, new DataGridViewCellEventArgs(dgvdata.CurrentCell.ColumnIndex, dgvdata.CurrentCell.RowIndex));
                }
            }
        }

        private void timerBuscar_Tick(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void dgvdata_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string colName = dgvdata.Columns[e.ColumnIndex].Name;
            if (colName == "PrecioVenta" || colName == "Precio2" || colName == "Precio3"
                || colName == "PrecioCompra" || colName == "Prima")   // 👈 NUEVO
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal precio))
                {
                    e.Value = "L." + precio.ToString("N2");
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
