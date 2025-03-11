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
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CapaPresentacion
{
    public partial class FrmRegistroColores : Form
    {
        public FrmRegistroColores()
        {
            InitializeComponent();
        }

        private void FrmRegistroColores_Load(object sender, EventArgs e)
        {
            CargarDatos();
            txtbusqueda.Select();

            TxtFechaRegistro.Text = DateTime.Now.ToString("dd/MM/yyyy");

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
            txtbusqueda.Select();
        }

        private void CargarDatos()
        {
            List<RegistroColores> lista = new CN_RegistroColores().Listar();
            dgvdata.Rows.Clear(); // Limpiar el DataGridView antes de agregar nuevos datos

            foreach (RegistroColores item in lista)
            {
                // Formatear la fecha antes de agregarla al DataGridView
                string fechaRegistroFormateada = DateTime.TryParse(item.FechaRegistro, out DateTime fechaRegistro)
                    ? fechaRegistro.ToString("dd/MM/yyyy")
                    : item.FechaRegistro; // Si no se puede parsear, usar el valor original

                dgvdata.Rows.Add(new object[] {
            "",
            item.Id,
            item.Codigo,
            item.Cliente,
            item.Descripcion,
            item.UbicacionFisica,
            item.Pagina,
            fechaRegistroFormateada, // Usar la fecha formateada
            item.Observaciones
        });
            }
        }
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            CN_RegistroColores objCN = new CN_RegistroColores();

            RegistroColores objColor = new RegistroColores()
            {
                Codigo = TxtCodigo.Text.Trim(),
                Cliente = Txtcliente.Text.Trim(),
                Descripcion = TxtDescripcion.Text.Trim(),
                UbicacionFisica = TxtUbicacionfisica.Text.Trim(),
                Pagina = Txtpagina.Text.Trim(), // Ahora es un string
                Observaciones = Txtobvservaciones.Text.Trim()
            };

            // Validar y convertir la fecha
            if (DateTime.TryParseExact(TxtFechaRegistro.Text.Trim(), "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime fechaRegistro))
            {
                objColor.FechaRegistro = fechaRegistro.ToString("dd/MM/yyyy"); // Guardar en el formato deseado
            }
            else
            {
                MessageBox.Show("La fecha debe estar en el formato dd/MM/yyyy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir si la fecha no es válida
            }

            if (txtid.Text == "0") // Si txtId está vacío, es un registro nuevo
            {
                int idGenerado = objCN.Registrar(objColor, out mensaje);
                if (idGenerado > 0)
                {
                    MessageBox.Show("Color registrado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtid.Text = idGenerado.ToString();
                    CargarDatos();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al registrar: " + mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                objColor.Id = Convert.ToInt32(txtid.Text);
                bool resultado = objCN.Editar(objColor, out mensaje);

                if (resultado)
                {
                    MessageBox.Show("Color actualizado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos();
                }
                else
                {
                    MessageBox.Show("Error al actualizar: " + mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LimpiarCampos()
        {
            txtid.Text = "0"; // Restablecer el ID a 0 para indicar un nuevo registro
            TxtCodigo.Text = string.Empty;
            Txtcliente.Text = string.Empty;
            TxtDescripcion.Text = string.Empty;
            TxtUbicacionfisica.Text = string.Empty;
            Txtpagina.Text = string.Empty;
            TxtFechaRegistro.Text = string.Empty;
            Txtobvservaciones.Text = string.Empty;
            txtindice.Text = string.Empty; // Limpiar el índice si es necesario
            TxtFechaRegistro.Text = DateTime.Now.ToString("dd/MM/yyyy");

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
                    Txtcliente.Text = dgvdata.Rows[indice].Cells["Cliente"].Value.ToString();
                    TxtDescripcion.Text = dgvdata.Rows[indice].Cells["Descripcion"].Value.ToString();
                    TxtUbicacionfisica.Text = dgvdata.Rows[indice].Cells["UbicacionFisica"].Value.ToString();
                    Txtpagina.Text = dgvdata.Rows[indice].Cells["Pagina"].Value.ToString();
                    TxtFechaRegistro.Text = dgvdata.Rows[indice].Cells["FechaRegistro"].Value.ToString();
                    Txtobvservaciones.Text = dgvdata.Rows[indice].Cells["Observaciones"].Value.ToString();
                }
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            if (txtid.Text != "0") // Asegúrate de que hay un ID para eliminar
            {
                int idColor = Convert.ToInt32(txtid.Text);
                CN_RegistroColores objCN = new CN_RegistroColores();

                if (MessageBox.Show("¿Estás seguro de que deseas eliminar este registro?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool resultado = objCN.Eliminar(idColor, out mensaje);
                    if (resultado)
                    {
                        MessageBox.Show("Color eliminado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos(); // Limpiar campos después de eliminar
                        CargarDatos(); // Actualizar el DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar: " + mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecciona un registro para eliminar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = txtbusqueda.Text.Trim().ToLower(); // Obtener el término de búsqueda y convertirlo a minúsculas
            List<RegistroColores> lista = new CN_RegistroColores().Listar(); // Obtener la lista completa de registros
            dgvdata.Rows.Clear(); // Limpiar el DataGridView antes de agregar nuevos datos

            // Filtrar la lista según el término de búsqueda
            var filteredList = lista.Where(item =>
                item.Codigo.ToLower().Contains(searchTerm) ||
                item.Cliente.ToLower().Contains(searchTerm) ||
                item.Descripcion.ToLower().Contains(searchTerm) ||
                item.UbicacionFisica.ToLower().Contains(searchTerm) ||
                item.Pagina.ToLower().Contains(searchTerm) ||
                item.FechaRegistro.ToLower().Contains(searchTerm) ||
                item.Observaciones.ToLower().Contains(searchTerm)).ToList();

            // Agregar los registros filtrados al DataGridView
            foreach (RegistroColores item in filteredList)
            {
                dgvdata.Rows.Add(new object[] {
                    "",
                    item.Id,
                    item.Codigo,
                    item.Cliente,
                    item.Descripcion,
                    item.UbicacionFisica,
                    item.Pagina,
                    item.FechaRegistro,
                    item.Observaciones
                });
            }
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = string.Empty; 
            CargarDatos();
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
                            
                            row.Cells[1].Value.ToString(),
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString(),
                            row.Cells[4].Value.ToString(),
                            row.Cells[5].Value.ToString(),
                            row.Cells[6].Value.ToString(),
                            row.Cells[7].Value.ToString(),
                            row.Cells[8].Value.ToString(),
                        });
                }

                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.FileName = string.Format("Registro_Colores{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        async void Btnabrirexcel_Click(object sender, EventArgs e)
        {
            pbCargando.Visible = true;
            pbCargando.Image = Properties.Resources.loading; // Asegúrate de que el GIF esté en los recursos

            // Abrir el modal en un hilo separado para no bloquear la UI
            await Task.Run(() =>
            {
                frmMostrarExcel mostrarExcel = new frmMostrarExcel();
                mostrarExcel.ShowDialog();
            });

            // Ocultar el PictureBox de carga después de que el modal se cierre
            pbCargando.Visible = false;
        }
    }
}
