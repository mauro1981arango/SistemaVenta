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
    public partial class frmVentas : Form
    {
        private Usuario _Usuario;
        public frmVentas(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
           
        }

        private void frmVentas_Load(object sender, EventArgs e)
        {
            cbxTipoDocumento.Items.Add("Seleciona una opción");
            cbxTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" });
            cbxTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cbxTipoDocumento.DisplayMember = "Texto";
            cbxTipoDocumento.ValueMember = "Valor";
            cbxTipoDocumento.SelectedIndex = 0;
            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtIdProducto.Text = "0";
            txtPagaCon.Text = "";
            txtCambio.Text = "";
            txtMontoTotal.Text = "0";
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            //Hacemos llamado al formulario md_Clientes.
            using (var modal = new md_Clientes())
            {
                var resultado = modal.ShowDialog();
                if (resultado == DialogResult.OK)
                {
                    //Obtenemos los datos Documento y el NombreCompleto del Cliente al hacer doble clip soblre la celda selecionada.
                    txtNumeroDocumentoCliente.Text = modal._Cliente.Documento;
                    txtNombreCompleto.Text = modal._Cliente.NombreCompleto;
                    txtCodigo.Select();
                }
                else
                {
                    txtNumeroDocumentoCliente.Select();
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
                    txtPrecioProducto.Text = modal._Producto.PrecioVenta.ToString();
                    txtStock.Text = modal._Producto.Stock.ToString();
                    txtCantidad.Select();

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
            if (e.KeyData == Keys.Enter)
            {
                //Hacemos uso de las expresiones Landa para verificar el estado actual del producto.
                Producto oProducto = (Producto)new CN_Producto().Listar().Where(p => p.Codigo == txtCodigo.Text && p.Estado == true).FirstOrDefault();
                if (oProducto != null)
                {
                    //Si se encuentra el prooducto la caja de texto txtCodigo se pone de color verde.
                    txtCodigo.BackColor = Color.GreenYellow;
                    txtCodigo.ForeColor = Color.Black;
                    txtIdProducto.Text = oProducto.IdProducto.ToString();
                    txtNombreProducto.Text = oProducto.Nombre;
                    txtPrecioProducto.Text=oProducto.PrecioVenta.ToString("0.0");
                    txtStock.Text = oProducto.Stock.ToString();
                    txtCantidad.Select();
                }
                else
                {
                    //Si se encuentra el prooducto la caja de texto txtCodigo se pone de color rojo.
                    txtCodigo.BackColor = Color.Red;
                    txtCodigo.ForeColor = Color.White;
                    txtIdProducto.Text = "0";
                    txtNombreProducto.Text = "";
                    txtPrecioProducto.Text = "";
                    txtCantidad.Value = 1;
                    txtStock.Text = "";
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //Declaramos las variables que vamos a utilizar para realizar una compra.
            decimal precio;
            bool producto_existe = false;

            if (int.Parse(txtIdProducto.Text) == 0)
            {
                //Validamos que se haya selecionado un producto para poder proceder con la venta.
                MessageBox.Show("Debe selecionar un producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!decimal.TryParse(txtPrecioProducto.Text, out precio))
            {
                //Validamos que el campo de texto txtPrecioCompra tenga el formato correcto de moneda.
                //No se debe agregar carácteres especiales ni letras.
                MessageBox.Show("Precio - Formato moneda incorrecta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioProducto.Select();
                return;
            }
            if (Convert.ToInt32(txtStock.Text) < Convert.ToInt32(txtCantidad.Value.ToString()))
            {
                //Validamos que el campo de texto txtPrecioVenta tenga el formato correcto de moneda.
                //No se debe agregar carácteres especiales ni letras.
                MessageBox.Show("La cantidad no puede ser mayor al stock", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;

            }
            foreach (DataGridViewRow fila in dataVenta.Rows)
            {
                //Recorremos las celdas de la tabla dataVenta para validar que el IdProducto existe.
                if (fila.Cells["IdProducto"].ToString() == txtIdProducto.Text)

                {
                    producto_existe = true;
                    break;
                }
            }
            if (!producto_existe)
            {
                bool respuesta = new CN_Venta().RestarStock(
                    Convert.ToInt32(txtIdProducto.Text), 
                    Convert.ToInt32(txtCantidad.Value.ToString())
                    );
                if (respuesta)
                {
                    //Si el productoo existe se puede proceder a registrar el producto en dataVenta, tabla dónde se enlistan los productos.
                    dataVenta.Rows.Add(new object[]
                   {
                    //Procedemos a añadir el producto correspondiente en dataVenta.
                    txtIdProducto.Text,
                    txtNombreProducto.Text,
                    precio.ToString("0.0"),
                    txtCantidad.Value.ToString(),
                    (txtCantidad.Value * precio).ToString("0.0")
                   });
                    calcularTotal();
                    limpiarProducto();
                    txtCodigo.Select();
                }
                
            }

        }
        private void calcularTotal()
        {
            decimal total = 0;
            //Validamos que si tengamos registros en nuestra tabla dataVenta, si tiene registros
            //se procede hacer el ciclo foreach, de los contrario entra a pintar lo que tiene el campo txtTotal.
            if (dataVenta.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataVenta.Rows)
                {
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value);

                }
                txtMontoTotal.Text = total.ToString();
            }
        }
        public void limpiarProducto()
        {
            txtIdProducto.Text = "0";
            txtCodigo.Text = "";
            txtNombreProducto.Text = "";
            txtPrecioProducto.Text = "";
            txtStock.Text = "";
            txtCantidad.Value = 1;
            txtCodigo.BackColor = System.Drawing.Color.White;
            txtNombreProducto.BackColor = System.Drawing.Color.White;
            txtCodigo.Select();
        }

        private void dataVenta_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //Pintamos la imagen de eliminar en la columna 6 btnEliminar de la tabla dataVenta.
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 5)
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
            if (dataVenta.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                int index = e.RowIndex;
                if (index >= 0)
                {
                    bool respuesta = new CN_Venta().SumarStock(
                        Convert.ToInt32(dataVenta.Rows[index].Cells["IdProducto"].Value.ToString()),
                        Convert.ToInt32(dataVenta.Rows[index].Cells["Cantidad"].Value.ToString())
                        );
                    if (respuesta)
                    {
                        dataVenta.Rows.RemoveAt(index);
                        calcularTotal();
                    }
                    
                }
            }
        }

        private void txtPrecioProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Validaciones para el campo de texto txtPrecioProducto.
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                //Primero validamos que no ponga un punto al inicio
                if (txtPrecioProducto.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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

        private void txtFormaDePago_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Validacones para el campo de texto txtFormaDePago.
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;

            }
            else
            {
                //Primero validamos que no ponga un punto al inicio
                if (txtPagaCon.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    //Validamos que la tecla borrar no se desactive para poder borrar en case de ser necesario.
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
        public void calcularCambio()
        {
            if (txtMontoTotal.Text.Trim() == "")
            {
                MessageBox.Show("No existen productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            decimal pagacon;
            decimal total = Convert.ToDecimal(txtMontoTotal.Text);
            if(txtPagaCon.Text.Trim() == "")
            {
                txtPagaCon.Text = "0";
            }
            if(decimal.TryParse(txtPagaCon.Text.Trim(), out pagacon))
            {
                if(pagacon < total)
                {
                    txtCambio.Text = "0.0";
                }
                else
                {
                    decimal cambio = pagacon - total;
                    txtCambio.Text = cambio.ToString("0.0");
                }
            }
        }

        private void txtFormaDePago_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                calcularCambio();
            }
        }

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if(txtNumeroDocumentoCliente.Text == "")
            {
                MessageBox.Show("Debe ingresar el número de documento del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (txtNombreCompleto.Text == "")
            {
                MessageBox.Show("Debe ingresar el nombre del cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if(dataVenta.Rows.Count < 1)
            {
                MessageBox.Show("Debe ingresar productos en la venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            DataTable detalle_venta = new DataTable();
            detalle_venta.Columns.Add("IdProducto", typeof(int));
            detalle_venta.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("SubTotal", typeof(decimal));
            foreach(DataGridViewRow row in dataVenta.Rows)
            {
                detalle_venta.Rows.Add(new object[]
                {
                    row.Cells["IdProducto"].Value.ToString(),
                    row.Cells["PrecioVenta"].Value.ToString(),
                    row.Cells["Cantidad"].Value.ToString(),
                    row.Cells["SubTotal"].Value.ToString(),
                });
            }
            int idcorrelativo = new CN_Venta().obtenerCorrelativo();
            string numerodocumento = string.Format("{0:00000}", idcorrelativo);
            calcularCambio();
            Venta oVenta = new Venta()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                TipoDocumento = ((OpcionCombo)cbxTipoDocumento.SelectedItem).Texto,
                NumeroDocumento = numerodocumento,
                DocumentoCliente = txtNumeroDocumentoCliente.Text,
                NombreCliente = txtNombreCompleto.Text,
                MontoPago = Convert.ToDecimal(txtPagaCon.Text),
                MontoCambio = Convert.ToDecimal(txtCambio.Text),
                MontoTotal = Convert.ToDecimal(txtMontoTotal.Text),
            };
            string Mensaje = string.Empty;
            bool Respuesta = new CN_Venta().Registrar(oVenta,detalle_venta,out Mensaje);
            if (Respuesta)
            {
                var result = MessageBox.Show("Número de venta generado:\n" + numerodocumento + "\n\n¿Desea copiarlo en el prtapapeles?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Clipboard.SetText(numerodocumento);
                    txtNumeroDocumentoCliente.Text = "";
                    txtNombreCompleto.Text = "";
                    dataVenta.Rows.Clear();
                    calcularTotal();
                    txtPagaCon.Text = "";
                    txtCambio.Text = "";
                }else
                    MessageBox.Show(Mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
