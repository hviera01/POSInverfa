using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class md_ProductoCredito : Form
    {
        public decimal PrecioInicial { get; set; }
        public decimal PrimaInicial { get; set; }
        public decimal SaldoPendienteInicial { get; set; }
        public int NumeroCuotasInicial { get; set; }
        public string FrecuenciaInicial { get; set; }
        public DateTime FechaInicioInicial { get; set; }
        public DateTime FechaVencimientoInicial { get; set; }
        public decimal MontoCuotaInicial { get; set; }

        // ==== Valores de salida (lo que se devuelve al frmVentas) ====
        public decimal Precio { get; private set; }
        public decimal Prima { get; private set; }
        public decimal SaldoPendiente { get; private set; }
        public int NumeroCuotas { get; private set; }
        public string Frecuencia { get; private set; }
        public DateTime FechaInicioCredito { get; private set; }
        public DateTime FechaVencimiento { get; private set; }
        public decimal MontoCuota { get; private set; }
        public md_ProductoCredito()
        {
            InitializeComponent();
        }

        private void md_ProductoCredito_Load(object sender, EventArgs e)
        {
            cboFrecuencia.Items.Clear();
            cboFrecuencia.Items.Add("Mensual");
            cboFrecuencia.Items.Add("Quincenal");
            cboFrecuencia.Items.Add("Semanal");
            cboFrecuencia.Items.Add("Diaria");

            if (string.IsNullOrWhiteSpace(FrecuenciaInicial))
                FrecuenciaInicial = "Semanal";

            // ---- Valores iniciales ----
            txtmontototal.Text = PrecioInicial.ToString("0.00");
            txtPrima.Text = PrimaInicial.ToString("0.00");

            if (SaldoPendienteInicial == 0)
                SaldoPendienteInicial = PrecioInicial - PrimaInicial;

            if (SaldoPendienteInicial < 0) SaldoPendienteInicial = 0;
            txtsaldopendiente.Text = SaldoPendienteInicial.ToString("0.00");
            txtsaldopendiente.ReadOnly = true;   // el saldo se calcula, no se escribe

            if (NumeroCuotasInicial <= 0)
                NumeroCuotasInicial = 1;

            txtNoCuotas.Text = NumeroCuotasInicial.ToString();

            FechaInicioInicial = (FechaInicioInicial == DateTime.MinValue)
                ? DateTime.Today
                : FechaInicioInicial;

            dtpFechaInicio.Value = FechaInicioInicial;

            // Seleccionar frecuencia inicial
            int idxFreq = cboFrecuencia.Items.IndexOf(FrecuenciaInicial);
            if (idxFreq < 0) idxFreq = 2;
            cboFrecuencia.SelectedIndex = idxFreq;

            // Fecha de vencimiento inicial
            if (FechaVencimientoInicial == DateTime.MinValue)
                FechaVencimientoInicial = CalcularFechaVencimiento(
                    FechaInicioInicial,
                    NumeroCuotasInicial,
                    FrecuenciaInicial
                );

            dtpfechavencimiento.Value = FechaVencimientoInicial;

            if (MontoCuotaInicial == 0 && NumeroCuotasInicial > 0)
                MontoCuotaInicial = SaldoPendienteInicial / NumeroCuotasInicial;

            txtCuota.Text = MontoCuotaInicial.ToString("0.00");

            // ==== Eventos para recalcular TODO automáticamente ====
            txtmontototal.TextChanged += RecalcularTodo;  // 👈 AHORA también si cambia el PRECIO
            txtPrima.TextChanged += RecalcularTodo;
            txtNoCuotas.TextChanged += RecalcularTodo;
            cboFrecuencia.SelectedIndexChanged += RecalcularTodo;
            dtpFechaInicio.ValueChanged += RecalcularTodo;

            // Ajuste final por si algo queda desfasado
            RecalcularTodo(null, null);
        }

        private void RecalcularTodo(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtmontototal.Text, out decimal precio))
                precio = 0;

            if (!decimal.TryParse(txtPrima.Text, out decimal prima))
                prima = 0;

            if (!int.TryParse(txtNoCuotas.Text, out int cuotas))
                cuotas = 0;

            if (cuotas <= 0)
                cuotas = 1;

            // Saldo pendiente
            decimal saldo = precio - prima;
            if (saldo < 0) saldo = 0;
            txtsaldopendiente.Text = saldo.ToString("0.00");

            // Monto cuota
            decimal montoCuota = Math.Round(saldo / cuotas, 2, MidpointRounding.AwayFromZero);
            txtCuota.Text = montoCuota.ToString("0.00");

            // Fecha vencimiento
            string freq = cboFrecuencia.SelectedItem?.ToString() ?? "Mensual";
            DateTime fechaIni = dtpFechaInicio.Value.Date;
            DateTime fechaVenc = CalcularFechaVencimiento(fechaIni, cuotas, freq);
            dtpfechavencimiento.Value = fechaVenc;
        }

        private DateTime CalcularFechaVencimiento(DateTime fechaInicio, int cuotas, string frecuencia)
        {
            if (cuotas <= 0) cuotas = 1;

            switch (frecuencia)
            {
                case "Diaria":
                    return fechaInicio.AddDays(cuotas);
                case "Semanal":
                    return fechaInicio.AddDays(7 * cuotas);
                case "Quincenal":
                    return fechaInicio.AddDays(15 * cuotas);
                case "Mensual":
                default:
                    return fechaInicio.AddMonths(cuotas);
            }
        }

        private void btnregistrarcredito_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtmontototal.Text, out decimal precio))
            {
                MessageBox.Show("Precio inválido.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrima.Text, out decimal prima))
            {
                MessageBox.Show("Prima inválida.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtNoCuotas.Text, out int cuotas) || cuotas <= 0)
            {
                MessageBox.Show("Ingrese un número de cuotas mayor a cero.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtCuota.Text, out decimal montoCuota))
                montoCuota = 0;

            if (!decimal.TryParse(txtsaldopendiente.Text, out decimal saldoPend))
                saldoPend = 0;

            // Asignar propiedades de salida
            Precio = precio;
            Prima = prima;
            SaldoPendiente = saldoPend;
            NumeroCuotas = cuotas;
            Frecuencia = cboFrecuencia.SelectedItem?.ToString() ?? "Mensual";
            FechaInicioCredito = dtpFechaInicio.Value.Date;
            FechaVencimiento = dtpfechavencimiento.Value.Date;
            MontoCuota = montoCuota;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
