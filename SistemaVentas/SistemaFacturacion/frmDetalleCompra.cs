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
    public partial class frmDetalleCompra : Form
    {
        public frmDetalleCompra()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtBusqueda.Text);
            if(oCompra.IdCompra != 0)
            {
                //Si el objeto oCompra es diferente de null quiere decir que se han obtenido resultados, entonnces se pintarán loos datos en las c
                txtNumeroDocumento.Text = oCompra.NumeroDocumento;
                txtFecha.Text = oCompra.FechaRegistro;
                txtTipoDocumento.Text = oCompra.TipoDocumento;
                txtUsuario.Text = oCompra.oUsuario.NombreCompleto;
                txtdocProveedor.Text = oCompra.oProveedor.Documento;
                txtNombreProveedor.Text = oCompra.oProveedor.RazonSocial;

                //Pasamos los datos a la tabla llamada dataDetalleCompra
                //dataDetalleCompra.Rows.Clear();
                foreach (Detalle_Compra dc in oCompra.oDetalleCompra)
                {
                    dataDetalleCompra.Rows.Add(new object[] { dc.oProducto.Nombre.ToString(),
                    dc.PrecioCompra.ToString(), dc.Cantidad.ToString(), dc.MontoTotal.ToString() });
                }
                txtMontoTotal.Text = oCompra.MontoTotal.ToString();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFecha.Text = "";
            txtTipoDocumento.Text = "";
            txtUsuario.Text = "";
            txtNombreProveedor.Text = "";
            txtdocProveedor.Text = "";
            txtBusqueda.Text = "";
            dataDetalleCompra.Rows.Clear();
            txtMontoTotal.Text = "0";
        }

        private void btnDescargarPdf_Click(object sender, EventArgs e)
        {
            //Validamos que existan datos para proceder a generar el documento pdf.
            if(txtTipoDocumento.Text == "")
            {
                MessageBox.Show("No se encontraro resultados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //Creamos un método que nos permite remplazar todo el texto Html por el vlaor de las cajas de texto.
            string Texto_Html = Properties.Resources.PlantillaCompra.ToString();
            Negocio datosNegocio = new CN_Negocio().ObtenerDatos();
            Texto_Html = Texto_Html.Replace("@nombrenegocio", datosNegocio.Nombre.ToUpper());
            Texto_Html = Texto_Html.Replace("@ruc", datosNegocio.RUC);
            Texto_Html = Texto_Html.Replace("@direccion", datosNegocio.Direccion);

            Texto_Html = Texto_Html.Replace("@tipodocumento", txtTipoDocumento.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtNumeroDocumento.Text);

            Texto_Html = Texto_Html.Replace("@docproveedor", txtdocProveedor.Text);
            Texto_Html = Texto_Html.Replace("@nombreproveedor", txtNombreProveedor.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtFecha.Text);
            Texto_Html = Texto_Html.Replace("@usuario", txtUsuario.Text);

            string filas = string.Empty;
            foreach(DataGridViewRow row in dataDetalleCompra.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Nombre"].Value.ToString() + "/<td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "/<td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "/<td>";
                filas += "<td>" + row.Cells["MontoTotal"].Value.ToString() + "/<td>";
                filas += "</tr>";
            }
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtMontoTotal.Text);
            //Creamos una ventana de dialogo que nos pregunta en que ubicación deseamos guardar el archivo pdf.
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = string.Format("Compra_{0}.pdf", txtNumeroDocumento.Text);
            savefile.Filter = "Pdf Files|*.pdf";
            if(savefile.ShowDialog() == DialogResult.OK)
            {
                //Creamos un archivo en memoria
                using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4,25,25,25,25);
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
                        image.SetAbsolutePosition(pdfDoc.Left,pdfDoc.GetTop(51));
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
