using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using CapaPresentacion.Modales;
using CapaDatos;

namespace CapaPresentacion
{
    public partial class frmProducto : Form
    {
        private Usuario _Usuario;
        public frmProducto(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            txtbusqueda.Select();
            dgvdata.DefaultCellStyle.SelectionBackColor = Color.Blue;  
            dgvdata.DefaultCellStyle.SelectionForeColor = Color.White;

            CboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            CboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No activo" });
            CboEstado.DisplayMember = "Texto";
            CboEstado.ValueMember = "Valor";
            CboEstado.SelectedIndex = 0;

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

            foreach (string columna in columnasOrdenadas)
            {
                // Verificar si la columna existe en el DataGridView
                if (dgvdata.Columns.Contains(columna))
                {
                    cbobusqueda.Items.Add(new OpcionCombo() { Valor = columna, Texto = dgvdata.Columns[columna].HeaderText });
                }
            }

            List<Categoria> listaCategoria = new CN_Categoria().Listar();

            foreach (Categoria item in listaCategoria)
            {
                CboCategoria.Items.Add(new OpcionCombo() { Valor = item.IdCategoria, Texto = item.Descripcion });
            }

            CboCategoria.DisplayMember = "Texto";
            CboCategoria.ValueMember = "Valor";
            CboCategoria.SelectedIndex = 0;


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

            //MOSTRAR TODOS LOS PRODUCTOS   
            List<Producto> lista = new CN_Producto().Listar();

            foreach (Producto item in lista)
            {
                {
                    dgvdata.Rows.Add(new object[] { "",
                        item.IdProducto,
                        item.Codigo,
                        item.Nombre,
                        item.Descripcion,
                        item.oCategoria.IdCategoria,
                        item.oCategoria.Descripcion,
                        item.Stock,
                        item.PrecioCompra,
                        item.PrecioVenta,
                        item.Estado == true ? 1 : 0,
                        item.Estado == true ? "Activo" : "No activo" });

                }
            }


            foreach (System.Windows.Forms.Control ctrl in this.Controls)

            {
                if (ctrl is Button btn)
                {
                    btn.Click += Btn_Click;
                }
            }
            txtbusqueda.Select();

        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {

                this.ActiveControl = null;
            }
            txtbusqueda.Select();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Producto obj = new Producto()
            {
                IdProducto = Convert.ToInt32(txtid.Text),
                Codigo = TxtCodigo.Text,
                Nombre = TxtNombre.Text,
                Descripcion = TxtDescripcion.Text,
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)CboCategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)CboEstado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.IdProducto == 0)
            {
                int idgenerado = new CN_Producto().Registrar(obj, out mensaje);

                if (idgenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {
                        "",
                        idgenerado,
                        TxtCodigo.Text,
                        TxtNombre.Text,
                        TxtDescripcion.Text,
                        ((OpcionCombo)CboCategoria.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)CboCategoria.SelectedItem).Texto.ToString(),
                        "0",
                        "0.00",
                        "0.00",
                        ((OpcionCombo)CboEstado.SelectedItem).Valor.ToString(),
                        ((OpcionCombo)CboEstado.SelectedItem).Texto.ToString()
                    });
                    Limpiar();
                }
                else
                {
                    MessageBox.Show(mensaje);
                }
            }
            else
            {
                bool resultado = new CN_Producto().Editar(obj, out mensaje);

                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["Codigo"].Value = TxtCodigo.Text;
                    row.Cells["Nombre"].Value = TxtNombre.Text;
                    row.Cells["Descripcion"].Value = TxtDescripcion.Text;
                    row.Cells["IdCategoria"].Value = ((OpcionCombo)CboCategoria.SelectedItem).Valor.ToString();
                    row.Cells["Categoria"].Value = ((OpcionCombo)CboCategoria.SelectedItem).Texto.ToString();
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
            TxtCodigo.Text = "";
            TxtNombre.Text = "";
            TxtDescripcion.Text = "";
            CboCategoria.SelectedIndex = 0;
            CboEstado.SelectedIndex = 0;

            TxtCodigo.Select();
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

                    TxtCodigo.Text = dgvdata.Rows[indice].Cells["Codigo"].Value.ToString();
                    TxtNombre.Text = dgvdata.Rows[indice].Cells["Nombre"].Value.ToString();
                    TxtDescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();

                    foreach (OpcionCombo oc in CboCategoria.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgvdata.Rows[indice].Cells["IdCategoria"].Value))
                        {
                            int indice_combo = CboCategoria.Items.IndexOf(oc);
                            CboCategoria.SelectedIndex = indice_combo;
                            break;
                        }
                    }

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
                if (MessageBox.Show("¿Desea eliminar el producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Producto obj = new Producto()
                    {
                        IdProducto = Convert.ToInt32(txtid.Text),

                    };

                    bool Respuesta = new CN_Producto().Eliminar(obj, out mensaje);

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
            txtbusqueda.Select();
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();

                foreach (DataGridViewColumn columna in dgvdata.Columns)
                {
                    if (columna.HeaderText != "" && columna.Visible)
                        dt.Columns.Add(columna.HeaderText, typeof(string));
                }

                foreach (DataGridViewRow row in dgvdata.Rows)
                {
                    if (row.Visible)
                        dt.Rows.Add(new object[] {
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                            row.Cells[9].Value.ToString(),
                            row.Cells[11].Value.ToString(),

                        });
                }

                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = string.Format("ReporteProducto_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                saveFile.Filter = "Excel files | *.xlsx";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XLWorkbook wb = new XLWorkbook();
                        var hoja = wb.Worksheets.Add(dt, "Informe");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(saveFile.FileName);
                        MessageBox.Show("Reporte generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Error al generar reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }

            }
        }

        private void dgvdata_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "Stock")
            {
                // Verifica si el valor de la celda es un decimal y menor que 2.00
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal stock) && stock < 2.00m)
                {
                    e.CellStyle.BackColor = Color.Red; // Cambia el color de fondo a rojo
                    e.CellStyle.ForeColor = Color.White; // Cambia el color del texto a blanco para mejor contraste
                }
                else
                {
                    e.CellStyle.BackColor = dgvdata.DefaultCellStyle.BackColor; // Restaura el color de fondo por defecto
                    e.CellStyle.ForeColor = dgvdata.DefaultCellStyle.ForeColor; // Restaura el color del texto por defecto
                }
            }

            // Cambia el color de la fila seleccionada
            if (dgvdata.Rows[e.RowIndex].Selected)
            {
                e.CellStyle.BackColor = Color.Blue;  // Cambia el color de la fila seleccionada
                e.CellStyle.ForeColor = Color.White; // Cambia el color del texto
            }

            // Formato para la columna "PrecioVenta"
            if (dgvdata.Columns[e.ColumnIndex].Name == "PrecioVenta")
            {
                if (e.Value != null)
                {
                    e.Value = string.Format("{0:F2}", e.Value); // Formato con dos decimales
                    e.FormattingApplied = true; // Indica que se aplicó el formato
                }
            }
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                iconButton1.PerformClick(); // Simula un clic en el botón btnbuscar
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

        private void BtnActualizarStock_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                string mensaje = string.Empty;
                decimal nuevoStock; // Cambiado a decimal

                // Usar decimal.TryParse para permitir valores decimales
                if (decimal.TryParse(TxtNuevoStock.Text, out nuevoStock))
                {
                    Producto obj = new Producto()
                    {
                        IdProducto = Convert.ToInt32(txtid.Text),
                        Stock = nuevoStock, // Asignar el nuevo stock como decimal
                        oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario } // Asegúrate de que _Usuario esté definido
                    };

                    // Obtener el stock anterior
                    decimal stockAnterior = Convert.ToDecimal(dgvdata.Rows[Convert.ToInt32(txtindice.Text)].Cells["Stock"].Value);

                    // Actualizar el stock en la base de datos
                    bool resultado = new CN_Producto().ActualizarStock(obj, stockAnterior, out mensaje);

                    if (resultado)
                    {
                        // Actualizar el DataGridView
                        dgvdata.Rows[Convert.ToInt32(txtindice.Text)].Cells["Stock"].Value = nuevoStock; // Actualizar el DataGridView con el nuevo stock
                        MessageBox.Show("Stock actualizado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        TxtNuevoStock.Text = ""; // Limpiar el TextBox
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Ingrese un valor numérico válido para el stock", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnHistorialStock_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                // Cargar el historial de stock del producto seleccionado
                frmHistorialStock historialStock = new frmHistorialStock(Convert.ToInt32(txtid.Text));
                historialStock.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un producto para ver el historial.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtactualizarprecio_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtid.Text) != 0)
            {
                string mensaje = string.Empty;
                decimal nuevoPrecio;

                // Validar que el precio ingresado sea un número válido
                if (decimal.TryParse(txtprecio.Text.Trim(), out nuevoPrecio))
                {
                    // Redondear el precio a dos decimales
                    nuevoPrecio = Math.Round(nuevoPrecio, 2);

                    Producto obj = new Producto()
                    {
                        IdProducto = Convert.ToInt32(txtid.Text),
                        PrecioVenta = nuevoPrecio,
                        oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario } // Asegúrate de que _Usuario esté definido
                    };

                    // Actualizar el precio en la base de datos
                    bool resultado = new CN_Producto().ActualizarPrecio(obj, out mensaje);

                    if (resultado)
                    {
                        // Actualizar el DataGridView
                        dgvdata.Rows[Convert.ToInt32(txtindice.Text)].Cells["PrecioVenta"].Value = nuevoPrecio;
                        MessageBox.Show("Precio actualizado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtprecio.Text = ""; // Limpiar el TextBox
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Ingrese un valor numérico válido para el precio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}