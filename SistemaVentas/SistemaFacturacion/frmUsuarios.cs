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
    public partial class frmUsuarios : Form
    {
        
        public frmUsuarios()
        {
            InitializeComponent();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtIdUsuario.Text) != 0)
            {
                if (MessageBox.Show("¿Realmentee desea eliminar al usuario?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes)
                {
                    string mensaje = string.Empty;
                    Usuario obj_usuario = new Usuario()
                    {
                        IdUsuario = Convert.ToInt32(txtIdUsuario.Text),
                        
                    };
                    bool respuesta = new CN_Usuario().Eliminar(obj_usuario, out mensaje);
                    if (respuesta)
                    {
                        dataUsuarios.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                    }
                    else
                    {
                        MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            cbxEstado.Items.Add(new OpcionCombo() {Valor = 1, Texto = "Activo" });
            cbxEstado.Items.Add(new OpcionCombo() {Valor = 0, Texto = "No Activo" });
            cbxEstado.DisplayMember = "Texto";
            cbxEstado.ValueMember = "Valor";
            cbxEstado.SelectedIndex = 0;

            List<Rol> listaRol = new CN_Rol().Listar();
            foreach (var rol in listaRol)
            {
                cbxIdRol.Items.Add(new OpcionCombo() { Valor = rol.IdRol, Texto = rol.Descripcion });
            }
                    cbxIdRol.DisplayMember = "Texto";
                    cbxIdRol.ValueMember = "Valor";
                    cbxIdRol.SelectedIndex = 0;
            foreach (DataGridViewColumn columna in dataUsuarios.Columns)
            {
                if (columna.Visible == true && columna.Name !="btnBuscar") 
                {
                    cbxBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            }
                    cbxBuscar.DisplayMember = "Texto";
                    cbxBuscar.ValueMember = "Valor";
                    cbxBuscar.SelectedIndex = 0;

            //Mostrar todos los usuarios en la tabla de la vista usuario.
            List<Usuario> listaUsuario = new CN_Usuario().Listar();
            foreach (Usuario item in listaUsuario)
            {
                dataUsuarios.Rows.Add(new object[] {"", item.IdUsuario,item.Documento, item.NombreCompleto, item.Correo, item.Clave,
                        item.oRol.IdRol,
                        item.oRol.Descripcion,
                        item.Estado == true ? 1 : 0,
                        item.Estado == true ? "Activo" : "No Activo"
                    });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            Usuario obj_usuario = new Usuario() {
                IdUsuario = Convert.ToInt32(txtIdUsuario.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCompleto.Text,
                Correo = txtCorreo.Text,
                Clave = txtClave.Text,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cbxIdRol.SelectedItem).Valor)},
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };
            
            int IdUsuarioGenerado = new CN_Usuario().Registrar(obj_usuario, out mensaje);
            if (IdUsuarioGenerado != 0)
            {
                dataUsuarios.Rows.Add(new object[] {"",IdUsuarioGenerado, txtDocumento.Text, txtNombreCompleto.Text,
                txtCorreo.Text, txtClave.Text,
                ((OpcionCombo)cbxIdRol.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cbxIdRol.SelectedItem).Texto.ToString(),
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
            txtIdUsuario.Text = "0";
            txtDocumento.Text = "";
            txtNombreCompleto.Text = "";
            txtCorreo.Text = "";
            txtClave.Text = "";
            cbxIdRol.SelectedIndex=0;
            cbxEstado.SelectedIndex=0;
            txtDocumento.Select();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
       
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataUsuarios.Columns[e.ColumnIndex].Name == "btnSelecionar")
            {
                int indice = e.RowIndex;
                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtIdUsuario.Text = dataUsuarios.Rows[indice].Cells["IdUsuario"].Value.ToString();
                    txtDocumento.Text = dataUsuarios.Rows[indice].Cells["Documento"].Value.ToString();
                    txtNombreCompleto.Text = dataUsuarios.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    txtCorreo.Text = dataUsuarios.Rows[indice].Cells["Correo"].Value.ToString();
                    txtClave.Text = dataUsuarios.Rows[indice].Cells["Clave"].Value.ToString();
                    
                    foreach (OpcionCombo cbx in cbxIdRol.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataUsuarios.Rows[indice].Cells["IdRol"].Value))
                        {
                            int indice_combo = cbxIdRol.Items.IndexOf(cbx);
                            cbxIdRol.SelectedIndex = indice_combo;
                            break;
                        }
                    }
                    foreach (OpcionCombo cbx in cbxEstado.Items)
                    {
                        if (Convert.ToInt32(cbx.Valor) == Convert.ToInt32(dataUsuarios.Rows[indice].Cells["EstadoValor"].Value.ToString()))
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
            Usuario obj_usuario = new Usuario()
            {
                IdUsuario = Convert.ToInt32(txtIdUsuario.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCompleto.Text,
                Correo = txtCorreo.Text,
                Clave = txtClave.Text,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cbxIdRol.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cbxEstado.SelectedItem).Valor) == 1 ? true : false,
            };

            bool resultado = new CN_Usuario().Editar(obj_usuario, out mensaje);
            if (resultado)
            {
                DataGridViewRow row = dataUsuarios.Rows[Convert.ToInt32(txtIndice.Text)];
                row.Cells["IdUsuario"].Value = txtIndice.Text;
                row.Cells["Documento"].Value = txtDocumento.Text;
                row.Cells["NombreCompleto"].Value = txtNombreCompleto.Text;
                row.Cells["Correo"].Value = txtCorreo.Text;
                row.Cells["Clave"].Value = txtClave.Text;
                row.Cells["IdRol"].Value = ((OpcionCombo)cbxIdRol.SelectedItem).Valor.ToString();
                row.Cells["Rol"].Value = ((OpcionCombo)cbxIdRol.SelectedItem).Texto.ToString();
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
            if (dataUsuarios.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataUsuarios.Rows)
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
            foreach(DataGridViewRow row in dataUsuarios.Rows)
            {
                row.Visible = true;
            }
        }

        private void dataUsuarios_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                var w = Properties.Resources.m_Check.Width;
                var h = Properties.Resources.m_Check.Width;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Width - h) / 2;
                e.Graphics.DrawImage(Properties.Resources.m_Check, x, y, w, h);
                e.Handled = true;
            }
        }
    }
}
