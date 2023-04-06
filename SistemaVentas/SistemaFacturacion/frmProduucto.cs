using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaFacturacion.Utilidades;
using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;

namespace SistemaFacturacion
{
    public partial class frmProduucto : Form
    {
        public frmProduucto()
        {
            InitializeComponent();
        }

        private void frmProduucto_Load(object sender, EventArgs e)
        {
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cbxEstado.DisplayMember = "Texto";
            cbxEstado.ValueMember = "Valor";
            cbxEstado.SelectedIndex = 0;

            List<Categoria> lista = new CN_Categoria().Listar();
            foreach (var categoria in lista)
            {
                cbxIdCategoria.Items.Add(new OpcionCombo() { Valor = categoria.IdCategoria, Texto = categoria.Descripcion });
            }
            cbxIdCategoria.DisplayMember = "Texto";
            cbxIdCategoria.ValueMember = "Valor";
            cbxIdCategoria.SelectedIndex = 0;
            foreach (DataGridViewColumn columna in dataProducto.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnBuscar")
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;

            //Mostrar todos los Productos en la tabla de la vista Producto.
            List<Producto> listaProducto = new CN_Producto().Listar();
            foreach (Producto item in listaProducto)
            {
                dataProducto.Rows.Add(new object[] {"", item.IdProducto,item.Codigo, item.Nombre, item.Descripcion, item.oCategoria.IdCategoria,                       
                        item.oCategoria.Descripcion, item.Stock, item.PrecioCompra, item.PrecioVenta,
                        item.Estado == true ? 1 : 0,
                        item.Estado == true ? "Activo" : "No Activo"
                    });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Producto obj_Producto = new Producto()
            {
                IdProducto = Convert.ToInt32(txtIdProducto.Text),
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cbxIdCategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            int IdProductoGenerado = new CN_Producto().Registrar(obj_Producto, out mensaje);
            if (IdProductoGenerado != 0)
            {
                dataProducto.Rows.Add(new object[] {"",IdProductoGenerado, txtCodigo.Text, txtNombre.Text,
                txtDescripcion.Text,
                ((OpcionCombo)cbxIdCategoria.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cbxIdCategoria.SelectedItem).Texto.ToString(),
                ((OpcionCombo)cbxEstado.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cbxEstado.SelectedItem).Texto.ToString()});

                limpiar();
            }
            else
            {
                MessageBox.Show(mensaje);
            }
        }

        private void limpiar()
        {
            txtIndice.Text = "-1";
            txtIdProducto.Text = "0";
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            cbxIdCategoria.SelectedIndex = 0;
            cbxEstado.SelectedIndex = 0;
            txtCodigo.Select();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Producto obj_Producto = new Producto()
            {
                IdProducto = Convert.ToInt32(txtIdProducto.Text),
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cbxIdCategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            bool resultado = new CN_Producto().Editar(obj_Producto, out mensaje);
            if (resultado)
            {
                DataGridViewRow row = dataProducto.Rows[Convert.ToInt32(txtIndice.Text)];
                row.Cells["IdProducto"].Value = txtIndice.Text;
                row.Cells["Codigo"].Value = txtCodigo.Text;
                row.Cells["Nombre"].Value = txtNombre.Text;
                row.Cells["Descripcion"].Value = txtDescripcion.Text;
                row.Cells["IdCategoria"].Value = ((OpcionCombo)cbxIdCategoria.SelectedItem).Valor.ToString();
                row.Cells["Categoria"].Value = ((OpcionCombo)cbxIdCategoria.SelectedItem).Texto.ToString();
                row.Cells["EstadoValor"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Valor.ToString();
                row.Cells["Estado"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Texto.ToString();
                limpiar();
            }
            else
            {
                MessageBox.Show(mensaje);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtIdProducto.Text) != 0)
            {
                if (MessageBox.Show("¿Realmentee desea eliminar el Producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Producto obj_Producto = new Producto()
                    {
                        IdProducto = Convert.ToInt32(txtIdProducto.Text),

                    };
                    bool respuesta = new CN_Producto().Eliminar(obj_Producto, out mensaje);
                    if (respuesta)
                    {
                        dataProducto.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiar();       
        }

        private void dataProducto_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataProducto.Columns[e.ColumnIndex].Name == "btnSelecionar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtIdProducto.Text = dataProducto.Rows[indice].Cells["IdProducto"].Value.ToString();
                    txtCodigo.Text = dataProducto.Rows[indice].Cells["Codigo"].Value.ToString();
                    txtNombre.Text = dataProducto.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtDescripcion.Text = dataProducto.Rows[indice].Cells["Descripcion"].Value.ToString();

                    foreach (OpcionCombo cbx in cbxIdCategoria.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataProducto.Rows[indice].Cells["IdProducto"].Value))
                        {
                            int indice_combo = cbxIdCategoria.Items.IndexOf(cbx);
                            cbxIdCategoria.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                    foreach (OpcionCombo cbx in cbxEstado.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataProducto.Rows[indice].Cells["Estado"].Value))
                        {
                            int indice_combo = cbxEstado.Items.IndexOf(cbx);
                            cbxEstado.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbxBuscar.SelectedItem).Valor.ToString();
            if (dataProducto.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataProducto.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBuscar.Text.Trim().ToUpper()))
                        row.Visible = true;

                    else

                        row.Visible = false;
                }
            }
        }

        private void btnLimpiarBuscar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            cbxBuscar.SelectedIndex = 0;
            foreach (DataGridViewRow row in dataProducto.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dataProducto.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                foreach (DataGridViewColumn columna in dataProducto.Columns)
                {
                    if (columna.HeaderText != "" && columna.Visible)
                    {
                        dt.Columns.Add(columna.HeaderText);
                    }
                    foreach (DataGridViewRow row in dataProducto.Rows)
                    {
                        if (row.Visible)
                        {
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
                                SaveFileDialog savefile = new SaveFileDialog();
                                savefile.FileName = string.Format("ReporteProducto_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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
                                catch
                                {
                                    MessageBox.Show("Error al generar el reporte", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
