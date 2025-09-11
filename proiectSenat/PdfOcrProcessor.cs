using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using PdfiumViewer;
using Tesseract;

namespace proiectSenat
{
    public class PdfOcrProcessor
    {
        private readonly string _tessdataPath;

        public PdfOcrProcessor(string tessdataPath = @".\tessdata")
        {
            _tessdataPath = tessdataPath;
        }

        public string ExtractTextFromPdf(string pdfPath)
        {
            var extractedText = new StringBuilder();

            try
            {
                // Convert PDF to images
                using (var document = PdfDocument.Load(pdfPath))
                {
                    for (int i = 0; i < document.PageCount; i++)
                    {
                        using (var image = document.Render(i, 300, 300, true))
                        {
                            string pageText = ExtractTextFromImage(image);
                            extractedText.AppendLine($"=== Page {i + 1} ===");
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

        private string ExtractTextFromImage(Image image)
        {
            try
            {
                using (var engine = new TesseractEngine(_tessdataPath, "ron", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist",
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                        "ĂÂÎȘȚăâîșț0123456789.,;:!?()-/\\ ");

                    using (var pix = Pix.LoadFromMemory(GetImageBytes((Bitmap)image)))
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

        private byte[] GetImageBytes(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}