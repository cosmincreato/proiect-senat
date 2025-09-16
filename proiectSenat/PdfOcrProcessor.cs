namespace proiectSenat;

using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using PdfiumViewer;
using Tesseract;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

public class PdfOcrProcessor
{
    private static readonly string BuildDirPath = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string BaseDirPath = Directory.GetParent(BuildDirPath).Parent.Parent.Parent.FullName;
    private readonly string _tessdataPath = Path.Combine(BaseDirPath, "tessdata");

    public PdfOcrProcessor(string tessdataPath = @".\tessdata")
    {
        _tessdataPath = tessdataPath;
    }

    public string ExtractTextFromPdf(string pdfPath)
    {
        var extractedText = new StringBuilder();

        try
        {
            using (var document = PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    using (var image = document.Render(i, 300, 300, true))
                    {
                        string pageText = ExtractTextFromImage(image);
                        extractedText.AppendLine(pageText);
                        extractedText.AppendLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing PDF: {ex.Message}");
        }

        return extractedText.ToString();
    }

    private string ExtractTextFromImage(object image)
    {
        try
        {
            using (var engine = new TesseractEngine(_tessdataPath, "ron", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist",
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                    "ĂÂÎȘȚăâîșț0123456789.,;:!?()-/\\ ");

                byte[] imageBytes = GetImageBytes(image);
                using (var pix = Pix.LoadFromMemory(imageBytes))
                {
                    using (var page = engine.Process(pix))
                    {
                        return page.GetText();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OCR Error: {ex.Message}");
            return string.Empty;
        }
    }

    private byte[] GetImageBytes(object image)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using (var ms = new MemoryStream())
            {
                ((Bitmap)image).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        else
        {
            using (var ms = new MemoryStream())
            {
                // Convert System.Drawing.Bitmap to BMP stream
                using (var bmpStream = new MemoryStream())
                {
                    ((Bitmap)image).Save(bmpStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    bmpStream.Position = 0;
                    using (var imgSharp = SixLabors.ImageSharp.Image.Load<Rgba32>(bmpStream))
                    {
                        imgSharp.Save(ms, new PngEncoder());
                    }
                }

                return ms.ToArray();
            }
        }
    }
}