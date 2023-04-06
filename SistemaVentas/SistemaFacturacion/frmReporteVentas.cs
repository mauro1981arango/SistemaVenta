using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using SistemaFacturacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaFacturacion
{
    public partial class frmReporteVentas : Form
    {
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            cbxBuscar.Items.Add(new OpcionCombo() { Valor = 0, Texto = "Selecionar" });
            foreach (DataGridViewColumn column in dataReporteVentas.Columns)
            {
                cbxBuscar.Items.Add(new OpcionCombo() { Valor = column.Name, Texto = column.HeaderText });
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<ReporteVentas> lista = new List<ReporteVentas>();
            lista = new CN_Reporte().Ventas(
                dateInicio.Value.ToString(),
                dateFin.Value.ToString());

            dataReporteVentas.Rows.Clear();
            foreach (ReporteVentas rv in lista)
            {
                dataReporteVentas.Rows.Add(new object[]
                {
                    rv.FechaRegistro,
                    rv.TipoDocumento,
                    rv.NumeroDocumento,
                    rv.MontoTotal,
                    rv.UsuarioRegistro,
                    rv.DocumentoCliente,
                    rv.NombreCliente,
                    rv.CodigoProducto,
                    rv.NombreProducto,
                    rv.Categoria,
                    rv.PrecioVenta,
                    rv.Cantidad,
                    rv.SubTotal
                });
            }
        }

        private void btnDescargarExcel_Click(object sender, EventArgs e)
        {
            if (dataReporteVentas.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn columna in dataReporteVentas.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }
                foreach (DataGridViewRow row in dataReporteVentas.Rows)
                {
                    if (row.Visible)
                    {
                        dt.Rows.Add(new object[] {
                                row.Cells[0].Value.ToString(),
                                row.Cells[1].Value.ToString(),
                                row.Cells[2].Value.ToString(),
                                row.Cells[3].Value.ToString(),
                                row.Cells[4].Value.ToString(),
                                row.Cells[5].Value.ToString(),
                                row.Cells[6].Value.ToString(),
                                row.Cells[7].Value.ToString(),
                                row.Cells[8].Value.ToString(),
                                row.Cells[9].Value.ToString(),
                                row.Cells[10].Value.ToString(),
                                row.Cells[11].Value.ToString(),
                                row.Cells[12].Value.ToString(),
                                
                            });
                        SaveFileDialog savefile = new SaveFileDialog();
                        savefile.FileName = string.Format("ReporteVenta_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                        savefile.Filter = "Excel Files | *.xlsx";
                        if (savefile.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                XLWorkbook wb = new XLWorkbook();
                                var hoja = wb.Worksheets.Add(dt, "Informe");
                                hoja.ColumnsUsed().AdjustToContents();
                                wb.SaveAs(savefile.FileName);
                                MessageBox.Show("Reporte generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Error al generar el reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                            }
                        }
                    }
                }
            }
        }

        private void btnBusqueda_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbxBuscar.SelectedItem).Valor.ToString();
            if (dataReporteVentas.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataReporteVentas.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;

                    else

                        row.Visible = false;
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            cbxBuscar.SelectedIndex = 0;
            foreach (DataGridViewRow row in dataReporteVentas.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
