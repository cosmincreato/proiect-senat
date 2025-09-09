using System.Net;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;


namespace proiectSenat
{
    internal static class PdfService
    {

        private const string PDF_DIR_PATH = @"C:\Users\diap\source\repos\proiectSenat\proiectSenat\input\";
        private const string OUTPUT_DIR_PATH = @"C:\Users\diap\source\repos\proiectSenat\proiectSenat\output\";

        public static void DownloadFromUrl(string url)
        {
            if (!Directory.Exists(PDF_DIR_PATH))
            {
                Directory.CreateDirectory(PDF_DIR_PATH);
            }

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var fileBytes = response.Content.ReadAsByteArrayAsync().Result;
                string fileName = Path.GetFileName(new Uri(url).AbsolutePath);
                string filePath = Path.Combine(PDF_DIR_PATH, fileName);
                File.WriteAllBytes(filePath, fileBytes);
            }
        }

        public static void ConvertToText()
        {
            if (!Directory.Exists(OUTPUT_DIR_PATH))
            {
                Directory.CreateDirectory(OUTPUT_DIR_PATH);
            }
            var pdfs = Directory.EnumerateFiles(PDF_DIR_PATH, "*.pdf");
            Console.WriteLine($"{pdfs.Count<string>().ToString()} PDF files found.");
            foreach (var pdf in pdfs)
            {
                var sb = new StringBuilder();
                // Extragem textul din PDF folosind PdfPig
                // am putea folosi iText, dar nu este free for commercial use
                using (PdfDocument document = PdfDocument.Open(pdf))
                {
                    foreach (Page page in document.GetPages())
                    {
                        IEnumerable<Word> words = page.GetWords();
                        foreach (var word in words)
                        {
                            sb.Append(word.Text);
                            sb.Append(' ');
                        }
                        sb.AppendLine("");
                    }
                }
                // Adaugam fisierul .txt in folderul de output
                string fileName = Path.GetFileNameWithoutExtension(pdf) + ".txt";
                string outputPath = Path.Combine(OUTPUT_DIR_PATH, fileName);
                File.WriteAllText(outputPath, sb.ToString());
                Console.WriteLine($"{outputPath}");
            }
            // RAG Architecture cu n8n si MCP
            // Antrenam modelul pe GPU
        }

        public static bool IsDirectoryEmpty(string path)
        {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
            using (IEnumerator<string> en = items.GetEnumerator())
            {
                return !en.MoveNext();
            }
        }

        public static bool SetupNeeded()
        {
            return (!Directory.Exists(PDF_DIR_PATH) || IsDirectoryEmpty(PDF_DIR_PATH));
        }
    }
}
