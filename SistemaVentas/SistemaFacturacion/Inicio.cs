using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using FontAwesome.Sharp;
using CapaNegocio;

namespace SistemaFacturacion
{
    public partial class Inicio : Form
    {
        private static Usuario usuarioActual;
        private static IconMenuItem MenuActivo = null;
        private static Form FormularioActivo = null;
        

        public Inicio(Usuario objusuario = null)
        {
            if (objusuario == null) usuarioActual = new Usuario() { NombreCompleto = "ADMIN PREDEFINIDO", IdUsuario = Convert.ToInt32("1") };
            else
            
            usuarioActual = objusuario;
            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> listaPermiso = new CN_Permiso().Listar(Convert.ToInt32(usuarioActual.IdUsuario));
            foreach (IconMenuItem iconMenu in menu.Items)
            {
                bool encontrado = listaPermiso.Any(m => m.NombreMenu == iconMenu.Name);
                if(encontrado == false)
                {
                    iconMenu.Visible = false;
                }
            }


            lblUsuario.Text = usuarioActual.NombreCompleto;
        }

        private void AbrirFormulario(IconMenuItem menu, Form formulario, IconMenuItem menuActivo)
        {
            if (menu != null)
            {
               formulario.BackColor = Color.PaleGreen;
            }
            menu.BackColor = Color.Silver;
            MenuActivo = menu;
            if (FormularioActivo != null)
            {
                FormularioActivo.Close();
            }
            FormularioActivo = formulario;
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            formulario.BackColor = Color.DarkGreen;
            subMenuCategoria.Controls.Add(formulario);
            formulario.Show();
        }

        private void Inicio_Load_1(object sender, EventArgs e)
        {

        }

        private void menuUsuario_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmUsuarios(), MenuActivo);
        }

        private void iconMenuItem1_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuMantenedor), new frmCategoria(), MenuActivo);
        }

        private void subMenuProducto_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuMantenedor), new frmProduucto(), MenuActivo);
        }

        private void subMenuRegistrar_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuVentas), new frmVentas(usuarioActual), MenuActivo);
        }

        private void subMenuDetalleVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuVentas), new frmDetalleVenta(), MenuActivo);
        }

        private void subMenuRegistrarCompraq_Click(object sender, EventArgs e)
        {
            //Le compartimos el usuario que se ha logiado en el sistema al formulario de compras con la variable (usuarioActual).
            AbrirFormulario((menuCompras), new frmCompras(usuarioActual), MenuActivo);
        }

        private void subMenuVerDetalleCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuCompras), new frmDetalleCompra(), MenuActivo);
        }

        private void menuClientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmClientes(), MenuActivo);
        }

        private void menuProveedores_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmaProveedores(), MenuActivo);
        }

        private void subMenuNegocio_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuMantenedor), new frmNegocio(), MenuActivo);
        }

        private void subMenuReporteCompras_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuReportes), new frmReporteCompras(), MenuActivo);
        }

        private void subMenuReporteVentas_Click(object sender, EventArgs e)
        {
            AbrirFormulario((menuReportes), new frmReporteVentas(), MenuActivo);
        }
    }
}
