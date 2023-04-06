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

namespace SistemaFacturacion
{
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cbxEstado.DisplayMember = "Texto";
            cbxEstado.ValueMember = "Valor";
            cbxEstado.SelectedIndex = 0;
            foreach (DataGridViewColumn columna in dataCliente.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnBuscar")
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;

            //Mostrar todos los Clientes en la tabla de la vista cliente.
            List<Cliente> listaCliente = new CN_Cliente().Listar();
            foreach (Cliente item in listaCliente)
            {
                dataCliente.Rows.Add(new object[] {"", item.IdCliente,item.Documento, item.NombreCompleto, item.Correo, item.Telefono,
                        item.Estado == true ? 1 : 0,
                        item.Estado == true ? "Activo" : "No Activo"
                    });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Cliente obj_Cliente = new Cliente()
            {
                IdCliente = Convert.ToInt32(txtIdCliente.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCompleto.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            int IdClienteGenerado = new CN_Cliente().Registrar(obj_Cliente, out mensaje);
            if (IdClienteGenerado != 0)
            {
                dataCliente.Rows.Add(new object[] {"",IdClienteGenerado, txtDocumento.Text, txtNombreCompleto.Text,
                txtCorreo.Text, txtTelefono.Text,
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
            txtIdCliente.Text = "0";
            txtDocumento.Text = "";
            txtNombreCompleto.Text = "";
            txtCorreo.Text = "";
            txtTelefono.Text = "";
            cbxEstado.SelectedIndex = 0;
            txtDocumento.Select();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Cliente obj_Cliente = new Cliente()
            {
                IdCliente = Convert.ToInt32(txtIdCliente.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCompleto.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            bool Resultado = new CN_Cliente().Editar(obj_Cliente, out mensaje);
            if (Resultado)
            {
                DataGridViewRow row = dataCliente.Rows[Convert.ToInt32(txtIndice.Text)];
                row.Cells["IdCliente"].Value = txtIndice.Text;
                row.Cells["Documento"].Value = txtDocumento.Text;
                row.Cells["NombreCompleto"].Value = txtNombreCompleto.Text;
                row.Cells["Correo"].Value = txtCorreo.Text;
                row.Cells["Telefono"].Value = txtTelefono.Text;
                row.Cells["Estado"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Valor.ToString();
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
            if (Convert.ToInt32(txtIdCliente.Text) != 0)
            {
                if (MessageBox.Show("¿Realmentee desea eliminar el cliente?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Cliente obj_cliente = new Cliente()
                    {
                        IdCliente = Convert.ToInt32(txtIdCliente.Text),

                    };
                    bool respuesta = new CN_Cliente().Eliminar(obj_cliente, out mensaje);
                    if (respuesta)
                    {
                        dataCliente.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                        limpiar();
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

        private void dataCliente_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataCliente.Columns[e.ColumnIndex].Name == "btnSelecionar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtIdCliente.Text = dataCliente.Rows[indice].Cells["IdCliente"].Value.ToString();
                    txtDocumento.Text = dataCliente.Rows[indice].Cells["Documento"].Value.ToString();
                    txtNombreCompleto.Text = dataCliente.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    txtCorreo.Text = dataCliente.Rows[indice].Cells["Correo"].Value.ToString();
                    txtTelefono.Text = dataCliente.Rows[indice].Cells["Telefono"].Value.ToString();

                    foreach (OpcionCombo cbx in cbxEstado.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataCliente.Rows[indice].Cells["EstadoValor"].Value.ToString()))
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
            if (dataCliente.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataCliente.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBuscar.Text.Trim().ToUpper().ToString()))
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
