using CapaEntidad;
using CapaNegocio;
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

namespace SistemaFacturacion.Modales
{
    public partial class mdProducto : Form
    {
        public Producto _Producto { get; set; }
        public mdProducto()
        {
            InitializeComponent();
        }

        private void mdProducto_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dataProducto.Columns)
            {
                if (columna.Visible == true)
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;

            //Mostrar todos los Productos en la tabla de la vista mdProducto.
            List<Producto> listaProducto = new CN_Producto().Listar();
            foreach (Producto item in listaProducto)
            {
                dataProducto.Rows.Add(new object[] {item.IdProducto,item.Codigo, item.Nombre,
                item.oCategoria.Descripcion, item.Stock, item.PrecioCompra, item.PrecioVenta});
            }
        }

        private void dataProducto_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Validamos que la dar doble clip sobre una fila el formulario modal mdProducto se cierre.
            int iRow = e.RowIndex;
            int iCol = e.ColumnIndex;
            if (iRow >= 0 && iCol > 0)
            {
                _Producto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dataProducto.Rows[iRow].Cells["IdProducto"].Value.ToString()),
                    Codigo = dataProducto.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dataProducto.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Stock = Convert.ToInt32(dataProducto.Rows[iRow].Cells["Stock"].Value.ToString()),
                    PrecioCompra = Convert.ToDecimal(dataProducto.Rows[iRow].Cells["PrecioCompra"].Value.ToString()),
                    PrecioVenta = Convert.ToDecimal(dataProducto.Rows[iRow].Cells["PrecioVenta"].Value.ToString())
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbxBuscar.SelectedItem).Valor.ToString();
            if (dataProducto.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataProducto.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().Contains(txtBuscar.Text.Trim().ToUpper()))
                        row.Visible = true;

                    else

                        row.Visible = false;
                }
            }
        }

        private void btnLimpiarBuscar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            foreach (DataGridViewRow row in dataProducto.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
