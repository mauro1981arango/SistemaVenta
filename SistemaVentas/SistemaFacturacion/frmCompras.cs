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
using CapaNegocio;
using SistemaFacturacion.Modales;
using SistemaFacturacion.Utilidades;

namespace SistemaFacturacion
{
    public partial class frmCompras : Form
    {
        //Creamos una variable para pasarle el usuario que se ha logiado al formulario de ventas.
        private Usuario _Usuario;
        public frmCompras(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            cbxTipoDocumento.Items.Add("Seleciona una opción");
            cbxTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbxTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cbxTipoDocumento.DisplayMember = "Texto";
            cbxTipoDocumento.ValueMember = "Valor";
            cbxTipoDocumento.SelectedIndex = 0;
            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtIdProveedor.Text = "0";
            txtIdProducto.Text = "0";

        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            //Hacemos llamado al formulario mdProveedor.
            using(var modal = new mdProveedor())
            {
                var resultado = modal.ShowDialog();
                if(resultado == DialogResult.OK)
                {
                    //Obtenemos los datos Documento y la RazOnSocial del Proveedor al hacer doble clip soblre la celda selecionada.
                    txtIdProveedor.Text = Convert.ToInt32(modal._Proveedor.IdProveedor).ToString();
                    txtNumeroDocumento.Text = modal._Proveedor.Documento;
                    txtNnmbreProveedor.Text = modal._Proveedor.RazonSocial;
                }
                else
                {
                    txtNumeroDocumento.Select();
                }
            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            //Hacemos llamado al formulario mdProducto.
            using (var modal = new mdProducto())
            {
                var resultado = modal.ShowDialog();
                if (resultado == DialogResult.OK)
                { 
                    //Obtenemos los datos Codigo y el Nombre del Producto al hacer doble clip soblre la celda selecionada//.
                    txtIdProducto.Text = Convert.ToInt32(modal._Producto.IdProducto).ToString();
                    txtCodigo.BackColor = Color.GreenYellow;
                    txtCodigo.ForeColor = Color.Black;
                    txtNombreProducto.BackColor = Color.GreenYellow;
                    txtNombreProducto.ForeColor = Color.Black;
                    txtCodigo.Text = modal._Producto.Codigo;
                    txtNombreProducto.Text = modal._Producto.Nombre;
                    //txtPrecioVenta.Text = Convert.ToDecimal(modal._Producto.PrecioVenta).ToString();
                    txtPrecioCompra.Select();

                }
                else
                {
                    txtCodigo.Select();
                }
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            //Cunado se presiona la tecla enter nosotros debemos hacer la búsqueda del producto.
            //Esta funcionalidad es útil cuando vamos a utilizar un lector de códigos de barra.
            if(e.KeyData == Keys.Enter)
            {
                //Hacemos uso de las expresiones Landa para verificar el estado actual del producto.
                Producto oProducto = (Producto) new CN_Producto().Listar().Where(p => p.Codigo == txtCodigo.Text && p.Estado == true).FirstOrDefault();
                if (oProducto != null)
                {
                    //Si se encuentra el prooducto la caja de texto txtCodigo se pone de color verde.
                    txtCodigo.BackColor = Color.GreenYellow;
                    txtCodigo.ForeColor = Color.Black;
                    txtIdProducto.Text=oProducto.IdProducto.ToString();
                    txtNombreProducto.Text = oProducto.Nombre;
                    txtPrecioCompra.Select();
                }
                else
                {
                    //Si se encuentra el prooducto la caja de texto txtCodigo se pone de color rojo.
                    txtCodigo.BackColor = Color.Red;
                    txtCodigo.ForeColor = Color.White;
                    txtIdProducto.Text = "0";
                    txtNombreProducto.Text = "";
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //Declaramos las variables que vamos a utilizar para realizar una compra.
            decimal preciocompra;
            decimal precioventa;
            bool producto_existe = false;
            if(int.Parse(txtIdProducto.Text) == 0)
            {
                //Validamos que se haya selecionado un producto para poder proceder con la compra.
                MessageBox.Show("Debe selecionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if(!decimal.TryParse(txtPrecioCompra.Text, out precioventa))
            {
                //Validamos que el campo de texto txtPrecioCompra tenga el formato correcto de moneda.
                //No se debe agregar carácteres especiales ni letras.
                MessageBox.Show("Precio compra - Formato moneda incorrecta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioCompra.Select();
                return;
            }
            if (!decimal.TryParse(txtPrecioVenta.Text, out preciocompra))
            {
                //Validamos que el campo de texto txtPrecioVenta tenga el formato correcto de moneda.
                //No se debe agregar carácteres especiales ni letras.
                MessageBox.Show("Precio venta - Formato moneda incorrecta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioVenta.Select();
                return;
            }
            foreach(DataGridViewRow fila in dataCompra.Rows)
            {
                //Recorremos las celdas de la tabla dataVenta para validar que el IdProducto existe.
                if (fila.Cells["IdProducto"].ToString() == txtIdProducto.Text)

                {
                    producto_existe =true;
                    break;
                }
            }
            if (!producto_existe)
            {
                //Si el productoo existe se puede proceder a registrar el producto en dataVenta, tabla dónde se enlistan los productos.
                 dataCompra.Rows.Add(new object[]
                {
                    //Procedemos a añadir el producto correspondiente en dataVenta.
                    txtIdProducto.Text,
                    txtNombreProducto.Text,
                    txtPrecioCompra.Text.ToString(),
                    txtPrecioVenta.Text.ToString(),
                    txtCantidad.Value.ToString(),
                    //txtMontoTotal.Text.ToString(),
                    (txtCantidad.Value * preciocompra).ToString()
                });
                calcularTotal();
                limpiarProducto();
                txtCodigo.Select();
            }
        }
        public void limpiarProducto()
        {
            txtIdProducto.Text = "0";
            txtCodigo.Text = "";
            txtNombreProducto.Text = "";
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            txtCantidad.Value = 1;
            txtCodigo.BackColor = System.Drawing.Color.White;
            txtNombreProducto.BackColor = System.Drawing.Color.White;
        }
        private void calcularTotal()
        {
            decimal total = 0;
            //Validamos que si tengamos registros en nuestra tabla dataVenta, si tiene registros
            //se procede hacer el ciclo foreach, de los contrario entra a pintar lo que tiene el campo txtTotal.
            if (dataCompra.Rows.Count > 0)
            {
                foreach(DataGridViewRow row in dataCompra.Rows)
                {
                    total += Convert.ToDecimal(row.Cells["MontoTotal"].Value);

                }
                txtMontoTotal.Text = total.ToString();
            }
        }

        private void dataVenta_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //Pintamos la imagen de eliminar en la columna 6 btnEliminar de la tabla dataVenta.
            if (e.RowIndex < 0)
                return;
            if(e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                var w = Properties.Resources.img_Delecte.Width;
                var h = Properties.Resources.img_Delecte.Width;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Width - h) / 2;
                e.Graphics.DrawImage(Properties.Resources.img_Delecte, x, y, w, h);
                e.Handled = true;
            }
        }

        private void dataVenta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataCompra.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                int indice = e.RowIndex;
                if(indice >= 0)
                {
                    dataCompra.Rows.RemoveAt(indice);
                    calcularTotal();
                }
            }
        }

        private void txtPrecioCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Validacones para el campo de texto txtPrecioCompra.
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                //Primero validamos que no ponga un punto al inicio
                if(txtPrecioCompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    //Vlidamoos que la tecla borrar no se desactive para poder borrar en case de ser necesario.
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled=false;
                    }
                    else
                    {
                        //Si pasa todas las validaciones anteriores el controlador toma el valor de verdadero y nos deja insertar números en la caja de texto.
                        e.Handled=true;
                    }
                }
            }
        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Validacones para el campo de texto txtPrecioVenta.
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                //Primero validamos que no ponga un punto al inicio
                if (txtPrecioVenta.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    //Vlidamoos que la tecla borrar no se desactive para poder borrar en case de ser necesario.
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        //Si pasa todas las validaciones anteriores el controlador toma el valor de verdadero y nos deja insertar números en la caja de texto.
                        e.Handled = true;
                    }
                }
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            //Hacemos la validacion de que el txtIdProveedor no esté vacío.
            if(Convert.ToInt32(txtIdProveedor.Text) == 0)
            {
                MessageBox.Show("Debe selecionar un Proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if(dataCompra.Rows.Count < 1)
            {
                //Validamos que se haya seleconado un producto.
                MessageBox.Show("Debe ingresar productos en la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //Declaramos un nuevo DataTable y le asignamos los tipos de datos que va a recibir.
               DataTable detalle_compra = new DataTable();
                detalle_compra.Columns.Add("IdProducto",typeof(int));
                detalle_compra.Columns.Add("PrecioCompra", typeof(decimal));
                detalle_compra.Columns.Add("PrecioVenta", typeof(decimal));
                detalle_compra.Columns.Add("Cantidad", typeof(int));
                detalle_compra.Columns.Add("MontoTotal", typeof(decimal));
            foreach (DataGridViewRow row in dataCompra.Rows)
            {
                //Capturamos los valores de las columnas a traves de un array tipo object.
                detalle_compra.Rows.Add(
                    new object[] { 
                      Convert.ToInt32(row.Cells["IdProducto"].Value.ToString()),
                        row.Cells["PrecioCompra"].Value.ToString(),
                        row.Cells["PrecioVenta"].Value.ToString(),
                        row.Cells["Cantidad"].Value.ToString(),
                        row.Cells["MontoTotal"].Value.ToString()
                    });
            }
            int idcorrelativo = new CN_Compra().obtenerCorrelativo();
            string numerodocumento = string.Format("{0:00000}", idcorrelativo);
            //Creamos los objetos necesarios para completar el registro  de una compra.
            Compra oCompra = new Compra()
            {
                //Este es el usuario que se ha registrado en el sistema para relizar la compra.
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                //Con estos atributos terminamos el objeto compra.
                oProveedor = new Proveedor() { IdProveedor = Convert.ToInt32(txtIdProveedor.Text) },
                TipoDocumento = ((OpcionCombo)cbxTipoDocumento.SelectedItem).Texto,
                NumeroDocumento = numerodocumento,
                MontoTotal = Convert.ToDecimal(txtMontoTotal.Text)

            };
            string mensaje = string.Empty;
            //Declaramoos la respuesta que nos va a traer registrar una compra.
            bool respuesta = new CN_Compra().Registrar(oCompra,detalle_compra,out mensaje);
            //Validamos que la respuesta sea verdadera debemos mostrar un mensaje que diga que se ha generado un número d compra.
            if (respuesta)
            {
                var resultado = MessageBox.Show("Número de compra generado:\n" + numerodocumento + "\n\n¿Desea copiarlo en el prtapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if(resultado == DialogResult.Yes)
                {
                    //Al registra la compra limpiamos las cajas de texto y el dataVenta, la tabla deonde almacenamos losdatos de la comprea.
                    Clipboard.SetText(numerodocumento);
                    txtIdProveedor.Text = "0";
                    txtNombreProducto.Text = "0";
                    txtPrecioVenta.Text = "0";
                    txtPrecioCompra.Text = "0";
                    cbxTipoDocumento.SelectedIndex = 0;
                    txtNumeroDocumento.Text = "";
                    txtNnmbreProveedor.Text="";
                    dataCompra.Rows.Clear();
                    calcularTotal();
                }
                else
                {
                    //Si el registro de la compra no es satisfactoria nos muestra un mensaje.
                    MessageBox.Show(mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    
                }
            }

        }
    }
} 
