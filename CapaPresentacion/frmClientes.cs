using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace CapaPresentacion
{
    public partial class frmClientes : Form
    {
        private System.Windows.Forms.Timer _timerBusquedaClientes;

        private List<Cliente> _listaClientesCompleta = new List<Cliente>();
        private const int MAX_FILAS_MOSTRAR = 300;
        private bool _cargandoClientes = false;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.BeginInvoke(new Action(async () =>
            {
                await CargarClientesAsync();
            }));
        }

        private async Task CargarClientesAsync()
        {
            if (_cargandoClientes)
                return;

            Cursor anterior = Cursor.Current;

            try
            {
                _cargandoClientes = true;
                Cursor = Cursors.WaitCursor;

                // Cargar desde la BD UNA sola vez
                List<Cliente> lista = await Task.Run(() =>
                {
                    return new CN_Cliente().Listar();
                });

                _listaClientesCompleta = lista ?? new List<Cliente>();

                // Pintar en el grid (máx 300 filas)
                CargarGridClientes(_listaClientesCompleta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
            finally
            {
                Cursor = anterior;
                _cargandoClientes = false;
            }
        }

        private void CargarGridClientes(IEnumerable<Cliente> origen)
        {
            dgvdata.SuspendLayout();

            try
            {
                dgvdata.Rows.Clear();

                var autoSizeOriginal = dgvdata.AutoSizeRowsMode;
                dgvdata.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgvdata.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

                foreach (Cliente item in origen.Take(MAX_FILAS_MOSTRAR))
                {
                    dgvdata.Rows.Add(new object[] {
                "",
                item.idCliente,
                item.CodCliente,
                item.Documento,
                item.NombreCompleto,
                item.Correo,
                item.Direccion,
                item.Telefono,
                item.Estado ? 1 : 0,
                item.Estado ? "Activo" : "No activo"
            });
                }

                dgvdata.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            }
            finally
            {
                dgvdata.ResumeLayout();
            }
        }



        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            CboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            CboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No activo" });
            CboEstado.DisplayMember = "Texto";
            CboEstado.ValueMember = "Valor";
            CboEstado.SelectedIndex = 0;

            cbobusqueda.Items.Clear();
            cbobusqueda.Items.Add(new OpcionCombo() { Valor = "Todo", Texto = "Buscar en todo" });
            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible == true && columna.Name != "BtnSeleccionar")
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

            dgvdata.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvdata.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;

            if (dgvdata.Columns.Contains("NombreCompleto"))
                dgvdata.Columns["NombreCompleto"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (dgvdata.Columns.Contains("Correo"))
                dgvdata.Columns["Correo"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (dgvdata.Columns.Contains("Direccion"))
                dgvdata.Columns["Direccion"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
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

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            // Si el usuario lo deja vacío, se manda 0 para que el SP lo genere.
            int codCliente = 0;
            if (!string.IsNullOrWhiteSpace(txtCodCliente.Text))
            {
                int.TryParse(txtCodCliente.Text.Trim(), out codCliente);
            }

            Cliente obj = new Cliente()
            {
                idCliente = Convert.ToInt32(txtid.Text),
                CodCliente = codCliente,                     // 0 = que lo genere el SP
                Documento = TxtDocumento.Text,
                NombreCompleto = TxtNombreCompleto.Text,
                Correo = TxtCorreo.Text,                 // Profesión/Oficio
                Telefono = TxtTelefono.Text,
                Direccion = TxtDireccion.Text,              // Dirección
                Estado = Convert.ToInt32(((OpcionCombo)CboEstado.SelectedItem).Valor) == 1
            };

            if (obj.idCliente == 0)
            {
                // REGISTRAR
                int idgenerado = new CN_Cliente().Registrar(obj, out mensaje);
                // IMPORTANTE: en CN_Cliente.Registrar se debe manejar @CodCliente como InputOutput
                // y asignar al objeto obj.CodCliente el valor final.

                if (idgenerado != 0)
                {
                    dgvdata.Rows.Add(new object[] {
                "",
                idgenerado,
                obj.CodCliente,                    // aquí ya debe venir el código final (manual o generado)
                TxtDocumento.Text,
                TxtNombreCompleto.Text,
                TxtCorreo.Text,                    // Profesión/Oficio
                TxtDireccion.Text,                 // Dirección
                TxtTelefono.Text,
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
                // EDITAR
                bool resultado = new CN_Cliente().Editar(obj, out mensaje);

                if (resultado)
                {
                    DataGridViewRow row = dgvdata.Rows[Convert.ToInt32(txtindice.Text)];
                    row.Cells["Id"].Value = txtid.Text;
                    row.Cells["CodCliente"].Value = obj.CodCliente;
                    row.Cells["Documento"].Value = TxtDocumento.Text;
                    row.Cells["NombreCompleto"].Value = TxtNombreCompleto.Text;
                    row.Cells["Correo"].Value = TxtCorreo.Text;      // Profesión/Oficio
                    row.Cells["Direccion"].Value = TxtDireccion.Text;   // Dirección
                    row.Cells["Telefono"].Value = TxtTelefono.Text;
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
            TxtDocumento.Text = "";
            TxtNombreCompleto.Text = "";
            TxtCorreo.Text = "";
            TxtTelefono.Text = "";
            TxtDireccion.Text = "";
            CboEstado.SelectedIndex = 0;
            txtCodCliente.Text = "";
            TxtDocumento.Select();
        }
        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                int w = Properties.Resources.anydo_104098.Width;
                int h = Properties.Resources.anydo_104098.Height;
                int x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                int y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(
                    Properties.Resources.anydo_104098,
                    new System.Drawing.Rectangle(x, y, w, h)
                );

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
                    txtCodCliente.Text = dgvdata.Rows[indice].Cells["CodCliente"].Value.ToString();
                    TxtDocumento.Text = dgvdata.Rows[indice].Cells["Documento"].Value.ToString();
                    TxtNombreCompleto.Text = dgvdata.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    TxtCorreo.Text = dgvdata.Rows[indice].Cells["Correo"].Value.ToString();
                    TxtDireccion.Text = dgvdata.Rows[indice].Cells["Direccion"].Value.ToString();
                    TxtTelefono.Text = dgvdata.Rows[indice].Cells["Telefono"].Value.ToString();

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
                if (MessageBox.Show("¿Desea eliminar el cliente?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string mensaje = string.Empty;
                    Cliente obj = new Cliente()
                    {
                        idCliente = Convert.ToInt32(txtid.Text),

                    };

                    bool Respuesta = new CN_Cliente().Eliminar(obj, out mensaje);

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

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnbuscar_Click(object sender, EventArgs e)
        {
            FiltrarDatos();
        }

        private void btnlimpiarbuscador_Click(object sender, EventArgs e)
        {
            txtbusqueda.Text = "";
            cbobusqueda.SelectedIndex = 0;

            CargarGridClientes(_listaClientesCompleta);

            txtbusqueda.Focus();
        }

        private void TxtDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }

            if (TxtDocumento.Text.Length >= 14 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                FiltrarDatos();
            }
        }

        private bool ContieneTodasLasPalabras(string campo, string[] terminosUpper)
        {
            if (string.IsNullOrWhiteSpace(campo))
                return false;

            string valor = campo.ToUpperInvariant();

            foreach (string t in terminosUpper)
            {
                if (valor.IndexOf(t, StringComparison.Ordinal) < 0)
                    return false;
            }

            return true;
        }


        private void FiltrarDatos()
        {
            if (_listaClientesCompleta == null || _listaClientesCompleta.Count == 0)
                return;

            if (cbobusqueda.SelectedItem == null)
                return;

            string valorFiltro = ((OpcionCombo)cbobusqueda.SelectedItem).Valor.ToString();
            string texto = (txtbusqueda.Text ?? "").Trim();

            // Sin texto => mostrar todo (máx 300)
            if (string.IsNullOrEmpty(texto))
            {
                CargarGridClientes(_listaClientesCompleta);
                return;
            }

            // Partir en palabras para búsqueda inteligente: "OSCAR AVILA"
            // => ["OSCAR", "AVILA"]
            string[] terminosUpper = texto
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToUpperInvariant())
                .ToArray();

            var lista = _listaClientesCompleta.AsEnumerable();

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
                    lista = lista.Where(c =>
                        ContieneTodasLasPalabras(c.Documento, terminosUpper));
                    break;

                case "NombreCompleto":
                    lista = lista.Where(c =>
                        ContieneTodasLasPalabras(c.NombreCompleto, terminosUpper));
                    break;

                case "Correo":
                    lista = lista.Where(c =>
                        ContieneTodasLasPalabras(c.Correo, terminosUpper));
                    break;

                case "Direccion":
                    lista = lista.Where(c =>
                        ContieneTodasLasPalabras(c.Direccion, terminosUpper));
                    break;

                case "Telefono":
                    lista = lista.Where(c =>
                        ContieneTodasLasPalabras(c.Telefono, terminosUpper));
                    break;

                case "Estado":
                    lista = lista.Where(c =>
                        ContieneTodasLasPalabras(
                            c.Estado ? "ACTIVO" : "NO ACTIVO",
                            terminosUpper
                        ));
                    break;

                case "Todo":
                default:
                    lista = lista.Where(c =>
                           ContieneTodasLasPalabras(c.Documento, terminosUpper)
                        || ContieneTodasLasPalabras(c.NombreCompleto, terminosUpper)
                        || ContieneTodasLasPalabras(c.Correo, terminosUpper)
                        || ContieneTodasLasPalabras(c.Direccion, terminosUpper)
                        || ContieneTodasLasPalabras(c.Telefono, terminosUpper)
                        || c.CodCliente.ToString()
                             .IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0
                    );
                    break;
            }

            CargarGridClientes(lista);
        }



        private void TimerBusquedaClientes_Tick(object sender, EventArgs e)
        {
            _timerBusquedaClientes.Stop();
            FiltrarDatos();
        }


        private void txtbusqueda_TextChanged(object sender, EventArgs e)
        {
          
        }

        private async Task RefrescarFormularioAsync()
        {
            if (_cargandoClientes)
                return;

            // Guardar texto y campo de búsqueda
            string texto = txtbusqueda.Text;
            var opcion = cbobusqueda.SelectedItem as OpcionCombo;

            btnrefrescar.Enabled = false;
            try
            {
                await CargarClientesAsync();

                // Restaurar filtro
                if (opcion != null)
                {
                    int idx = cbobusqueda.Items.IndexOf(opcion);
                    if (idx >= 0) cbobusqueda.SelectedIndex = idx;
                }

                txtbusqueda.Text = texto;
                FiltrarDatos();
            }
            finally
            {
                btnrefrescar.Enabled = true;
            }
        }



        private async void btnrefrescar_Click(object sender, EventArgs e)
        {
            await RefrescarFormularioAsync();
        }

        private async void iconButton2_Click(object sender, EventArgs e)
        {
            await RefrescarFormularioAsync();
        }

        private void btnPuntos_Click(object sender, EventArgs e)
        {
          
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                    ofd.Title = "Seleccionar archivo de clientes (Clientes_Excel.xlsx)";

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;

                    // Cargar clientes existentes para no repetir Documento
                    List<Cliente> clientesExistentes = new CN_Cliente().Listar();
                    var documentosUsados = new HashSet<string>(
                        clientesExistentes
                            .Where(c => !string.IsNullOrWhiteSpace(c.Documento))
                            .Select(c => c.Documento.Trim()),
                        StringComparer.OrdinalIgnoreCase
                    );

                    int importados = 0;
                    int fallidos = 0;

                    using (var wb = new XLWorkbook(ofd.FileName))
                    {
                        var ws = wb.Worksheet(1); 

                        var filas = ws.RangeUsed().RowsUsed().Skip(1); // Saltar encabezado

                        foreach (var row in filas)
                        {
                            string cuenta = row.Cell(1).GetString().Trim();
                            string nombre = row.Cell(2).GetString().Trim();
                            string identidad = row.Cell(3).GetString().Trim();
                            string profesion = row.Cell(4).GetString().Trim();
                            string direccion = row.Cell(5).GetString().Trim();
                            string celular = row.Cell(6).GetString().Trim();
                            // decimal saldo  = row.Cell(7).GetDecimal(); // si lo ocupás luego

                            // Si no hay nombre, no tiene sentido importar
                            if (string.IsNullOrWhiteSpace(nombre))
                            {
                                fallidos++;
                                continue;
                            }

                            // Documento base: identidad del Excel (puede ser "0703")
                            string documentoBase = !string.IsNullOrWhiteSpace(identidad)
                                                    ? identidad
                                                    : cuenta; // si no trae identidad, usamos la cuenta

                            if (string.IsNullOrWhiteSpace(documentoBase))
                            {
                                // Nada con qué identificar, lo saltamos
                                fallidos++;
                                continue;
                            }

                            // Resolver duplicados: 0703, 0703 repetido, etc.
                            string documentoFinal = documentoBase;
                            int sufijo = 1;

                            while (documentosUsados.Contains(documentoFinal))
                            {
                                // Ejemplo: 0703-001, 0703-002, etc.
                                documentoFinal = $"{documentoBase}-{sufijo.ToString("D3")}";
                                sufijo++;
                            }

                            Cliente obj = new Cliente()
                            {
                                idCliente = 0,
                                Documento = documentoFinal,   // Identidad ajustada única
                                NombreCompleto = nombre,
                                Correo = profesion,        // Profesión / Oficio
                                Direccion = direccion,
                                Telefono = celular,
                                Estado = true              // Se importan como Activos
                            };

                            string mensaje;
                            int idgenerado = new CN_Cliente().Registrar(obj, out mensaje);

                            if (idgenerado != 0)
                            {
                                // Agregar al DataGridView
                                dgvdata.Rows.Add(new object[]
                                {
                            "",
                            idgenerado,
                            obj.Documento,
                            obj.NombreCompleto,
                            obj.Correo,
                            obj.Direccion,
                            obj.Telefono,
                            1,
                            "Activo"
                                });

                                documentosUsados.Add(obj.Documento);
                                importados++;
                            }
                            else
                            {
                                // Puede fallar por validación interna
                                fallidos++;
                            }
                        }
                    }

                    MessageBox.Show(
                        $"Importación finalizada.\n" +
                        $"Clientes importados: {importados}\n" +
                        $"Registros omitidos / fallidos: {fallidos}",
                        "Importar clientes",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al importar: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable dt = new DataTable();

            foreach (DataGridViewColumn columna in dgvdata.Columns)
            {
                if (columna.Visible && columna.Name != "BtnSeleccionar")
                    dt.Columns.Add(columna.HeaderText, typeof(string));
            }

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                if (row.IsNewRow || !row.Visible) continue;

                List<object> valores = new List<object>();
                foreach (DataGridViewColumn col in dgvdata.Columns)
                {
                    if (!col.Visible || col.Name == "BtnSeleccionar") continue;
                    valores.Add(row.Cells[col.Index].Value == null ? "" : row.Cells[col.Index].Value.ToString());
                }
                dt.Rows.Add(valores.ToArray());
            }

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Clientes_ComercialInverfa_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "Excel Files|*.xlsx";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var hoja = wb.Worksheets.Add(dt, "Clientes");
                        hoja.ColumnsUsed().AdjustToContents();
                        wb.SaveAs(savefile.FileName);
                    }
                    MessageBox.Show("Reporte generado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar reporte: " + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            if (dgvdata.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Clientes_ComercialInverfa_{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmmss"));
            savefile.Filter = "PDF Files|*.pdf";

            if (savefile.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25f, 25f, 30f, 30f);
                    PdfWriter.GetInstance(doc, stream);
                    doc.Open();

                    iTextSharp.text.Font fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14f);
                    iTextSharp.text.Font fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f);
                    iTextSharp.text.Font fuenteTexto = FontFactory.GetFont(FontFactory.HELVETICA, 9f);

                    Paragraph titulo = new Paragraph("COMERCIAL INVERFA", fuenteTitulo);
                    titulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(titulo);

                    Paragraph subtitulo = new Paragraph("REPORTE DE CLIENTES", fuenteSubtitulo);
                    subtitulo.Alignment = Element.ALIGN_CENTER;
                    subtitulo.SpacingAfter = 5f;
                    doc.Add(subtitulo);

                    Paragraph fecha = new Paragraph("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fuenteTexto);
                    fecha.Alignment = Element.ALIGN_RIGHT;
                    fecha.SpacingAfter = 10f;
                    doc.Add(fecha);

                    List<DataGridViewColumn> columnas = new List<DataGridViewColumn>();
                    foreach (DataGridViewColumn col in dgvdata.Columns)
                    {
                        if (col.Visible && col.Name != "BtnSeleccionar")
                            columnas.Add(col);
                    }

                    PdfPTable tabla = new PdfPTable(columnas.Count);
                    tabla.WidthPercentage = 100f;

                    foreach (var col in columnas)
                    {
                        PdfPCell celda = new PdfPCell(new Phrase(col.HeaderText, fuenteSubtitulo));
                        celda.HorizontalAlignment = Element.ALIGN_CENTER;
                        celda.BackgroundColor = new BaseColor(230, 230, 230);
                        celda.Padding = 3f;
                        tabla.AddCell(celda);
                    }

                    foreach (DataGridViewRow row in dgvdata.Rows)
                    {
                        if (row.IsNewRow || !row.Visible) continue;

                        foreach (var col in columnas)
                        {
                            string texto = row.Cells[col.Index].Value == null ? "" : row.Cells[col.Index].Value.ToString();
                            PdfPCell celda = new PdfPCell(new Phrase(texto, fuenteTexto));
                            celda.Padding = 2f;
                            tabla.AddCell(celda);
                        }
                    }

                    doc.Add(tabla);
                    doc.Close();
                    stream.Close();
                }

                MessageBox.Show("Reporte PDF generado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
