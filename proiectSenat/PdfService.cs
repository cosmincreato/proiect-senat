using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;


namespace proiectSenat
{
    internal static class PdfService
    {

        private static readonly string BaseDirPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string PdfDirPath = Path.Combine(BaseDirPath, "input");
        private static readonly string TxtDirPath = Path.Combine(BaseDirPath, "output");

        static PdfService()
        {
            if (!Directory.Exists(PdfDirPath))
            {
                Console.WriteLine("NOT EXISTA");
                Directory.CreateDirectory(PdfDirPath);
            }
            else
            {
                Console.WriteLine("EXISTA");
            }
        }

        public static void DownloadFromUrl(string url)
        {
            string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
            string filePath = Path.Combine(PdfDirPath, fileName);

            // Skip download if file already exists
            if (File.Exists(filePath))
            {
                Console.WriteLine($"Skipping download for {fileName}, already exists in input.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var fileBytes = response.Content.ReadAsByteArrayAsync().Result;
                File.WriteAllBytes(filePath, fileBytes);
            }
        }

        public static void ConvertToText()
        {
            if (!Directory.Exists(TxtDirPath))
            {
                Directory.CreateDirectory(TxtDirPath);
            }
            var pdfs = Directory.EnumerateFiles(PdfDirPath, "25*.pdf");
            Console.WriteLine($"{pdfs.Count<string>().ToString()} PDF files found.");

            // Initialize OCR processor for image-based PDFs
            var ocrProcessor = new PdfOcrProcessor();

            foreach (var pdf in pdfs)
            {
                string fileName = Path.GetFileNameWithoutExtension(pdf) + ".txt";
                string outputPath = Path.Combine(TxtDirPath, fileName);

                // Skip conversion if output file already exists
                if (File.Exists(outputPath))
                {
                    Console.WriteLine($"Skipping {pdf} because {outputPath} already exists.");
                    continue;
                }

                var sb = new StringBuilder();
                bool hasTextContent = false;

                // First, try extracting text directly using PdfPig
                try
                {
                    using (PdfDocument document = PdfDocument.Open(pdf))
                    {
                        foreach (Page page in document.GetPages())
                        {
                            IEnumerable<Word> words = page.GetWords();
                            foreach (var word in words)
                            {
                                sb.Append(word.Text);
                                sb.Append(' ');
                                hasTextContent = true;
                            }
                            sb.AppendLine();
                        }
                    }

                    // If no text content was found, use OCR
                    if (!hasTextContent || sb.Length < 50) // Minimal text threshold
                    {
                        Console.WriteLine($"No text content found in {pdf}, using OCR . . .");
                        sb.Clear();
                        string ocrText = ocrProcessor.ExtractTextFromPdf(pdf);
                        sb.Append(ocrText);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error with PdfPig extraction for {pdf}: {e.Message}");
                    Console.WriteLine("Falling back to OCR . . .");
                    
                    // Fallback to OCR if PdfPig fails
                    try
                    {
                        sb.Clear();
                        string ocrText = ocrProcessor.ExtractTextFromPdf(pdf);
                        sb.Append(ocrText);
                    }
                    catch (Exception ocrEx)
                    {
                        Console.WriteLine($"OCR also failed for {pdf}: {ocrEx.Message}");
                        sb.AppendLine($"Error processing PDF: {pdf}");
                    }
                }

                // Save the extracted text
                File.WriteAllText(outputPath, sb.ToString());
                Console.WriteLine($"{outputPath}");
            }
        }
    }
}
