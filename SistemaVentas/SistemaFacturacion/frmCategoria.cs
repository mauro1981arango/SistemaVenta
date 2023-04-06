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
    public partial class frmCategoria : Form
    {
        public frmCategoria()
        {
            InitializeComponent();
        }

        private void frmCategoria_Load(object sender, EventArgs e)
        {
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cbxEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cbxEstado.DisplayMember = "Texto";
            cbxEstado.ValueMember = "Valor";
            cbxEstado.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dataCategoria.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnBuscar")
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
            cbxBuscar.DisplayMember = "Texto";
            cbxBuscar.ValueMember = "Valor";
            cbxBuscar.SelectedIndex = 0;

            //Mostrar todos las categorias en la tabla de la vista frmCategoria.
            List<Categoria> listaCategoria = new CN_Categoria().Listar();
            foreach (Categoria item in listaCategoria)
            {
                dataCategoria.Rows.Add(new object[] {"", item.IdCategoria,item.Descripcion,
                        item.Estado == true ? 1 : 0,
                        item.Estado == true ? "Activo" : "No Activo"
                    });
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Categoria obj_Categoria = new Categoria()
            {
                IdCategoria = Convert.ToInt32(txtIdCategoria.Text),
                Descripcion = txtDescripcion.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            int IdCategoriaGenerado = new CN_Categoria().Registrar(obj_Categoria, out mensaje);
            if (IdCategoriaGenerado != 0)
            {
                dataCategoria.Rows.Add(new object[] {"",IdCategoriaGenerado, txtDescripcion.Text,
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
            txtIdCategoria.Text = "0";
            txtDescripcion.Text = "";
            txtBusqueda.Text = "";
            cbxEstado.SelectedIndex = 0;
            txtDescripcion.Select();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiar();

        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Categoria obj_categoria = new Categoria()
            {
                IdCategoria = Convert.ToInt32(txtIdCategoria.Text),
                Descripcion = txtDescripcion.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            bool resultado = new CN_Categoria().Editar(obj_categoria, out mensaje);
            if (resultado)
            {
                DataGridViewRow row = dataCategoria.Rows[Convert.ToInt32(txtIndice.Text)];
                row.Cells["Id"].Value = txtIdCategoria.Text;
                row.Cells["Descripcion"].Value = txtDescripcion.Text;
                row.Cells["EstadoValor"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Valor.ToString();
                row.Cells["Estado"].Value = ((OpcionCombo)cbxEstado.SelectedItem).Texto.ToString();
                limpiar();

            }
            else
            {
                MessageBox.Show(mensaje);
            }

        }

        private void dataCategoria_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataCategoria.Columns[e.ColumnIndex].Name == "btnSelecionar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtIdCategoria.Text = dataCategoria.Rows[indice].Cells["Id"].Value.ToString();
                    txtDescripcion.Text = dataCategoria.Rows[indice].Cells["Descripcion"].Value.ToString();

                    foreach (OpcionCombo cbx in cbxEstado.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataCategoria.Rows[indice].Cells["EstadoValor"].Value.ToString()))
                        {
                            int indice_combo = cbxEstado.Items.IndexOf(cbx);
                            cbxEstado.SelectedIndex = indice_combo;
                            break;
                        }
                    }


                }
            }

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtIdCategoria.Text) != 0)
            {
                if (MessageBox.Show("¿Realmentee desea eliminar la categoria?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Categoria obj_categoria = new Categoria()
                    {
                        IdCategoria = Convert.ToInt32(txtIdCategoria.Text),

                    };
                    bool respuesta = new CN_Categoria().Eliminar(obj_categoria, out mensaje);
                    if (respuesta)
                    {
                        dataCategoria.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                        limpiar();
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cbxBuscar.SelectedItem).Valor.ToString();
            if (dataCategoria.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataCategoria.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()))
                        row.Visible = true;

                    else

                        row.Visible = false;

                }
            }

        }

        private void btnLimpiarBuscar_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            cbxBuscar.SelectedIndex = 0;
            foreach (DataGridViewRow row in dataCategoria.Rows)
            {
                row.Visible = true;
            }

        }
    }
}
