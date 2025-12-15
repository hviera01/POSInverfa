using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Modales
{
    public partial class md_CobrarFactura : Form
    {
        public decimal Total { get; private set; }
        public decimal PagoCon { get; private set; }
        public decimal Cambio { get; private set; }
        private const string PREFIJO = "L. ";
        public md_CobrarFactura(decimal total)
        {
            InitializeComponent();
            Total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        private void md_CobrarFactura_Load(object sender, EventArgs e)
        {
            txtTotal.ReadOnly = true;
            txtCambio.ReadOnly = true;

            txtTotal.Text = FormatearL(Total);
            txtPagoCon.Text = "";
            txtCambio.Text = FormatearL(0m);

            txtPagoCon.Select();
            this.AcceptButton = btnCobrarFactura;
        }

        private static string FormatearL(decimal v) => PREFIJO + v.ToString("0.00", CultureInfo.InvariantCulture);

        private static decimal ParsearL(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0m;

            // Quita "L", ".", "L.", espacios, comas de miles
            var limpio = Regex.Replace(texto, @"[Ll]|\.?(?=\s)|^\s*L\.\s*|[ ,]", "");
            // Si quitó demasiado, vuelve al original sin prefijo explícito
            if (!decimal.TryParse(limpio.Replace(PREFIJO, "").Trim(),
                                  NumberStyles.Number | NumberStyles.AllowDecimalPoint,
                                  CultureInfo.InvariantCulture, out var v))
            {
                // último intento: quita el prefijo tal cual y parsea
                var s = texto.Replace("L", "")
             .Replace("l", "")
             .Replace(PREFIJO, "")
             .Trim();

                decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowDecimalPoint,
                                 CultureInfo.InvariantCulture, out v);
            }
            return Math.Round(v, 2, MidpointRounding.AwayFromZero);
        }

        private void txtPagoCon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            bool esPunto = e.KeyChar == '.';
            if (!char.IsDigit(e.KeyChar) && !esPunto)
            {
                e.Handled = true;
                return;
            }
            if (esPunto && ((TextBox)sender).Text.Contains(".")) e.Handled = true;
        }

        private void txtPagoCon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Ya se calculó en TextChanged; aquí dejamos que el AcceptButton (btnCobrarFactura) se dispare.
                // No pongas e.SuppressKeyPress ni e.Handled = true.
            }
        }

        private void CalcularCambioSoloVista()
        {
            var pago = ParsearL(txtPagoCon.Text);
            PagoCon = pago;

            if (PagoCon < Total)
                Cambio = 0m;
            else
                Cambio = PagoCon - Total;

            // NO tocar txtPagoCon aquí, solo mostrar el cambio
            txtCambio.Text = FormatearL(Cambio);
        }

        private void btnCobrarFactura_Click(object sender, EventArgs e)
        {
            CalcularCambioSoloVista();

            if (PagoCon < Total)
            {
                MessageBox.Show("El monto pagado no puede ser menor al total.",
                                "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPagoCon.Focus();
                txtPagoCon.SelectAll();
                return;
            }

            // OK -> cerrar devolviendo montos a frmVentas
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtPagoCon_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPagoCon.Text)) return;
            var pago = ParsearL(txtPagoCon.Text);
            txtPagoCon.Text = FormatearL(pago);
        }

        private void txtPagoCon_TextChanged(object sender, EventArgs e)
        {
            var pago = ParsearL(txtPagoCon.Text);
            PagoCon = pago;

            if (PagoCon < Total)
                Cambio = 0m;
            else
                Cambio = PagoCon - Total;

            txtCambio.Text = FormatearL(Cambio);
        }
    }
}
