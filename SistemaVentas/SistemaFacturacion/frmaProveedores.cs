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
    public partial class frmaProveedores : Form
    {
        public frmaProveedores()
        {
            InitializeComponent();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtIdProveedor.Text) != 0)
            {
                if (MessageBox.Show("¿Realmentee desea eliminar al Proveedor?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Proveedor obj_Proveedor = new Proveedor()
                    {
                        IdProveedor = Convert.ToInt32(txtIdProveedor.Text),

                    };
                    bool respuesta = new CN_Proveedor().Eliminar(obj_Proveedor, out mensaje);
                    if (respuesta)
                    {
                        dataProveedor.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void frmaProveedores_Load(object sender, EventArgs e)
        {
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cbxEstado.DisplayMember = "Texto";
            cbxEstado.ValueMember = "Valor";
            cbxEstado.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dataProveedor.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnBuscar")
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;

            //Mostrar todos los Proveedores en la tabla de la vista Proveedor.
            List<Proveedor> listaProveedor = new CN_Proveedor().Listar();
            foreach (Proveedor item in listaProveedor)
            {
                dataProveedor.Rows.Add(new object[] {"", item.IdProveedor,item.Documento, item.RazonSocial, item.Correo, item.Telefono,
                        item.Estado == true ? 1 : 0,
                        item.Estado == true ? "Activo" : "No Activo"
                    });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Proveedor obj_Proveedor = new Proveedor()
            {
                IdProveedor = Convert.ToInt32(txtIdProveedor.Text.ToString()),
                Documento = txtDocumento.Text,
                RazonSocial = txtRazonSocial.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            int IdProveedorGenerado = new CN_Proveedor().Registrar(obj_Proveedor, out mensaje);
            if (IdProveedorGenerado != 0)
            {
                dataProveedor.Rows.Add(new object[] {"",IdProveedorGenerado, txtDocumento.Text, txtRazonSocial.Text,
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
            txtIdProveedor.Text = "0";
            txtDocumento.Text = "";
            txtRazonSocial.Text = "";
            txtCorreo.Text = "";
            txtTelefono.Text = "";
            cbxEstado.SelectedIndex = 0;
            txtDocumento.Select();
        }

        private void dataProveedor_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataProveedor.Columns[e.ColumnIndex].Name == "btnSelecionar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtIdProveedor.Text = dataProveedor.Rows[indice].Cells["IdProveedor"].Value.ToString();
                    txtDocumento.Text = dataProveedor.Rows[indice].Cells["Documento"].Value.ToString();
                    txtRazonSocial.Text = dataProveedor.Rows[indice].Cells["RazonSocial"].Value.ToString();
                    txtCorreo.Text = dataProveedor.Rows[indice].Cells["Correo"].Value.ToString();
                    txtTelefono.Text = dataProveedor.Rows[indice].Cells["Telefono"].Value.ToString();

                    foreach (OpcionCombo cbx in cbxEstado.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataProveedor.Rows[indice].Cells["EstadoValor"].Value.ToString()))
                        {
                            int indice_combo = cbxEstado.Items.IndexOf(cbx);
                            cbxEstado.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Proveedor obj_Proveedor = new Proveedor()
            {
                IdProveedor = Convert.ToInt32(txtIdProveedor.Text.ToString()),
                Documento = txtDocumento.Text,
                RazonSocial = txtRazonSocial.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            bool resultado = new CN_Proveedor().Editar(obj_Proveedor, out mensaje);
            if (resultado)
            {
                DataGridViewRow row = dataProveedor.Rows[Convert.ToInt32(txtIndice.Text)];
                row.Cells["IdProveedor"].Value = txtIndice.Text;
                row.Cells["Documento"].Value = txtDocumento.Text;
                row.Cells["RazonSocial"].Value = txtRazonSocial.Text;
                row.Cells["Correo"].Value = txtCorreo.Text;
                row.Cells["Telefono"].Value = txtTelefono.Text;
                row.Cells["EstadoValor"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Valor.ToString();
                row.Cells["Estado"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Texto.ToString();
                limpiar();
            }
            else
            {
                MessageBox.Show(mensaje);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiar();
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
