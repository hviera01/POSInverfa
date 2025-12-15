using System;
using System.Drawing;
using System.Windows.Forms;

namespace CapaPresentacion
{
    /// <summary>
    /// Form base que:
    ///  - Activa escalado DPI real
    ///  - Escala proporcionalmente según la resolución actual vs tamaño de diseño
    ///  - Maximiza y centra al abrir
    /// </summary>
    public class ResponsiveFormBase : Form
    {
        /// <summary>
        /// Tamaño en el que diseñaste el formulario (ajústalo si tu diseño base es otro).
        /// Para frmUsuarios, tu Designer indica 1370x531.
        /// </summary>
        protected virtual Size DesignSize => new Size(1370, 531);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // DPI-aware
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Resolución disponible (área de trabajo, sin la barra de tareas)
            Rectangle wa = Screen.FromControl(this).WorkingArea;

            // Factor de escala proporcional (para no “salirse” por ancho/alto)
            float scaleX = (float)wa.Width / DesignSize.Width;
            float scaleY = (float)wa.Height / DesignSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            // Escalar todo el árbol de controles (incluye fuentes)
            if (Math.Abs(scale - 1f) > 0.01f)
            {
                this.Scale(new SizeF(scale, scale));
            }

            // Centrar y maximizar (así nunca se corta en monitores más chicos)
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Sugerencia: deja AutoScaleDimensions en 96x96 en el Designer
            // y no lo toques a mano, para que el escalado DPI sea estable.
        }
    }
}
