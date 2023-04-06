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
    public partial class frmReporteCompras : Form
    {
        public frmReporteCompras()
        {
            InitializeComponent();
        }

        private void frmReporteCompras_Load(object sender, EventArgs e)
        {
            List<Proveedor> lista = new CN_Proveedor().Listar();
            cbxProveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "Todos" });
            foreach (Proveedor proveedor in lista)
            {
                cbxProveedor.Items.Add(new OpcionCombo() { Valor = proveedor.IdProveedor, Texto = proveedor.RazonSocial });
            }
            cbxProveedor.DisplayMember = "Texto";
            cbxProveedor.ValueMember = "Valor";
            cbxProveedor.SelectedIndex = 0;

            cbxBuscar.Items.Add(new OpcionCombo() { Valor = 0, Texto = "Selecionar" });
            foreach (DataGridViewColumn column in dataReporteCompra.Columns)
            {
                cbxBuscar.Items.Add(new OpcionCombo() { Valor = column.Name, Texto = column.HeaderText });
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            int idproveedor = Convert.ToInt32(((OpcionCombo)cbxProveedor.SelectedItem).Valor.ToString());
            List<ReporteCompras> lista = new List<ReporteCompras>();
            lista = new CN_Reporte().Compras(
                dateInicio.Value.ToString(),
                dateFin.Value.ToString(),
                idproveedor);
            dataReporteCompra.Rows.Clear();
            foreach (ReporteCompras rc in lista)
            {
                dataReporteCompra.Rows.Add(new object[]
                {
                    rc.FechaRegistro,
                    rc.TipoDocumento,
                    rc.NumeroDocumento,
                    rc.MontoTotal,
                    rc.UsuarioRegistro,
                    rc.DocumentoProveedor,
                    rc.RazonSocial,
                    rc.CodigoProducto,
                    rc.NombreProducto,
                    rc.Categoria,
                    rc.PrecioCompra,
                    rc.PrecioVenta,
                    rc.Cantidad,
                    rc.SubTotal
                });
            }
        }

        private void btnDescargarExcel_Click(object sender, EventArgs e)
        {
            if(dataReporteCompra.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn columna in dataReporteCompra.Columns)
                {
                    dt.Columns.Add(columna.HeaderText, typeof(string));
                }
                foreach (DataGridViewRow row in dataReporteCompra.Rows)
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
                                row.Cells[13].Value.ToString(),
                            });
                        SaveFileDialog savefile = new SaveFileDialog();
                        savefile.FileName = string.Format("ReporteCompra_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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
                            }catch (Exception)
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
            if (dataReporteCompra.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataReporteCompra.Rows)
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
            foreach (DataGridViewRow row in dataReporteCompra.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
