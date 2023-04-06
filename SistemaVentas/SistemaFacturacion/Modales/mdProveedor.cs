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
    public partial class mdProveedor : Form
    {
        public Proveedor _Proveedor { get; set; }
        public mdProveedor()
        {
            InitializeComponent();
        }

        private void mdProveedor_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dataProveedor.Columns)
            {
                if (columna.Visible == true)
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;

            //Mostrar todos los Proveedores en la tabla de la vista  modal Proveedor.
            List<Proveedor> listaProveedor = new CN_Proveedor().Listar();
            foreach (Proveedor item in listaProveedor)
            {
                dataProveedor.Rows.Add(new object[] {item.IdProveedor,item.Documento, item.RazonSocial});
            }
        }

        private void dataProveedor_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Validamos que la dar doble clip sobre una fila el formulario modal Proveedor se cierre.
            int iRow = e.RowIndex;
            int iCol = e.ColumnIndex;
            if(iRow >= 0 && iCol > 0)
            {
                _Proveedor = new Proveedor()
                {
                    IdProveedor = Convert.ToInt32(dataProveedor.Rows[iRow].Cells["IdProveedor"].Value.ToString()),
                    Documento = dataProveedor.Rows[iRow].Cells["Documento"].Value.ToString(),
                    RazonSocial = dataProveedor.Rows[iRow].Cells["RazonSocial"].Value.ToString(),
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbxBuscar.SelectedItem).Valor.ToString();
            if (dataProveedor.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataProveedor.Rows)
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
            foreach (DataGridViewRow row in dataProveedor.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
