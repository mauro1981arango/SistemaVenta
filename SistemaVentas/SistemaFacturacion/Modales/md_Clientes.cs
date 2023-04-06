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
    public partial class md_Clientes : Form
    {
        //Creamos una propiedae única para este formulario md_Clientes.Podremos almacenar al cliente al cual hemos selecionado.
        public Cliente _Cliente { get; set; }
        public md_Clientes()
        {
            InitializeComponent();
        }

        private void md_Clientes_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dataCliente.Columns)
            {               
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });               
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;
            //Mostrar todos los Clientes en la tabla de la vista cliente.
            List<Cliente> listaCliente = new CN_Cliente().Listar();
            foreach (Cliente item in listaCliente)
            {
                dataCliente.Rows.Add(new object[] {item.Documento, item.NombreCompleto});
            }
        }

        private void dataCliente_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Validamos que la dar doble clip sobre una fila el formulario modal mdProducto se cierre.
            int iRow = e.RowIndex;
            int iCol = e.ColumnIndex;
            if (iRow >= 0 && iCol >= 0)
            {
                _Cliente = new Cliente()
                {
                    Documento = dataCliente.Rows[iRow].Cells["Documento"].Value.ToString(),
                    NombreCompleto = dataCliente.Rows[iRow].Cells["NombreCompleto"].Value.ToString(),
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbxBuscar.SelectedItem).Valor.ToString();
            if (dataCliente.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataCliente.Rows)
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
            foreach (DataGridViewRow row in dataCliente.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
