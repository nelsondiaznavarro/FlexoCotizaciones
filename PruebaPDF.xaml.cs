using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Microsoft.Maui.Storage;
using System.IO;

namespace FlexoCotizaciones;

public partial class PruebaPDF : ContentPage
{
	public PruebaPDF()
	{
		InitializeComponent();
        cmdPDF.Clicked += CmdPDF_Clicked;
	}

    private void CmdPDF_Clicked(object sender, EventArgs e)
    {
        // Create a new PDF document
        PdfDocument document = new PdfDocument();

        //Add a page to the document
        PdfPage page = document.Pages.Add();

        //Create PDF graphics for the page
        PdfGraphics graphics = page.Graphics;

        //Set the standard font
        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

        //Draw the text
        graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

        //Save the document to the stream
        MemoryStream stream = new MemoryStream();
        document.Save(stream);

        //Close the document
        document.Close(true);


        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

       
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "PRUEBA.PDF");
        using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
        using StreamWriter streamWriter = new StreamWriter(outputStream);
         streamWriter.WriteAsync(content);
        streamWriter.Close();
        //using (FileStream file = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
        //    file.CopyTo(stream);

        //Save the stream as a file in the device and invoke it for viewing
        //Xamarin.Forms.DependencyService.Get<ISave>().SaveAndView("Output.pdf", "application / pdf", stream);
    }
}