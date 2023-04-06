using CapaEntidad;
using CapaNegocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaFacturacion
{
    public partial class frmDetalleVenta : Form
    {
        public frmDetalleVenta()
        {
            InitializeComponent();
        }
        private void frmDetalleVenta_Load(object sender, EventArgs e)
        {
            txtBusqueda.Select();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Venta oVenta = new CN_Venta().ObtenerVenta(txtBusqueda.Text);
            if(oVenta.IdVenta != 0)
            {
                //Si el objeto oVenta es diferente de null quiere decir que se han obtenido resultados.
                txtNumeroDocumento.Text = oVenta.NumeroDocumento;
                txtFecha.Text = oVenta.FechaRegistro;
                txtTipoDocumento.Text = oVenta.TipoDocumento;
                txtNombreUsuario.Text = oVenta.oUsuario.NombreCompleto;
                txtNumeroDocumentoCliente.Text = oVenta.DocumentoCliente;
                txtNombreCliente.Text = oVenta.NombreCliente;
                //Pasamos los datos a la tabla llamada dataDetalleVenta
                dataDetalleVenta.Rows.Clear();
                foreach (Detalle_Venta dc in oVenta.oDetalleVenta)
                {
                    dataDetalleVenta.Rows.Add(new object[] { dc.oProducto.Nombre.ToString(),
                    dc.PrecioVenta.ToString(), dc.Cantidad.ToString(), dc.SubTotal.ToString() });
                }
                txtMontoTotal.Text = oVenta.MontoTotal.ToString("$0.0");
                txtMontoPago.Text = oVenta.MontoPago.ToString("$0.0");
                txtCambio.Text = oVenta.MontoCambio.ToString("$0.0");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFecha.Text = "";
            txtTipoDocumento.Text = "";
            txtNombreUsuario.Text = "";
            txtNumeroDocumentoCliente.Text = "";
            txtNombreCliente.Text = "";
            dataDetalleVenta.Rows.Clear();
            txtMontoTotal.Text = "$0.0";
            txtMontoPago.Text = "$0.0";
            txtCambio.Text = "$0.0";
        }

        private void btnDescargarPdf_Click(object sender, EventArgs e)
        {
            //Validamos que existan datos para proceder a generar el documento pdf.
            if (txtTipoDocumento.Text == "")
            {
                MessageBox.Show("No se encontraro resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //Creamos un método que nos permite remplazar todo el texto Html por el vlaor de las cajas de texto.
            string Texto_Html = Properties.Resources.PlantillaVenta.ToString();
            Negocio datosNegocio = new CN_Negocio().ObtenerDatos();
            Texto_Html = Texto_Html.Replace("@nombrenegocio", datosNegocio.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@ruc", datosNegocio.RUC);
            Texto_Html = Texto_Html.Replace("@direccion", datosNegocio.Direccion);

            Texto_Html = Texto_Html.Replace("@tipodocumento", txtTipoDocumento.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtNumeroDocumento.Text);

            Texto_Html = Texto_Html.Replace("@doccliente", txtTipoDocumento.Text);
            Texto_Html = Texto_Html.Replace("@nombrecliente", txtNombreCliente.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtFecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtNombreUsuario.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dataDetalleVenta.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "/<td>";
                filas += "<td>" + row.Cells["Precio"].Value.ToString() + "/<td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "/<td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "/<td>";
                filas += "</tr>";
            }
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtMontoTotal.Text);
            Texto_Html = Texto_Html.Replace("@montopago", txtMontoPago.Text);
            Texto_Html = Texto_Html.Replace("@cambio", txtCambio.Text);
            //Creamos una ventana de dialogo que nos pregunta en que ubicación deseamos guardar el archivo pdf.
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Venta_{0}.pdf", txtNumeroDocumento.Text);
            savefile.Filter = "Pdf Files|*.pdf";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                //Creamos un archivo en memoria
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    // Procedemos a insertar el logo del negocio en este pdf.
                    bool obtenido = true;
                    //Creamos un Array de bytes para obtener el logo.
                    byte[] byteimage = new CN_Negocio().ObtenerLogo(out obtenido);
                    if (obtenido)
                    {
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(byteimage);
                        image.ScaleToFit(60, 60);
                        //Mostaramos la imagen sobre el texto html que vamos a imprimir en pdf.
                        image.Alignment = iTextSharp.text.Image.UNDERLYING;
                        //Le decimos en qué posición x,y va estar pegada nuestra imagen.
                        image.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                        //Agregamos todas las caraterísticas de la imagen que hemos configurado a nuestro pdf.
                        pdfDoc.Add(image);
                    }
                    //Vamos a pegar todo nuestro texto html que hemos configurado en nuestro pdf.
                    using (StringReader sr = new StringReader(Texto_Html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }
                    //Cerramoos nuestros métodos.
                    pdfDoc.Close();
                    stream.Close();
                    //Procedemos a mostrar un mensaje en pantalla.
                    MessageBox.Show("Documento generado correctamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
