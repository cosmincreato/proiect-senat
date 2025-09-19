namespace proiectSenat
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    class Program
    {
        private static List<Dictionary<string, string>> _projects = new List<Dictionary<string, string>>();

        // Luam toate proiectele din 1990-2025, le descarcam PDF-urile si le convertim in text
        static async Task DataSetupAsync()
        {
            Console.WriteLine("== Data Setup ==");
            Console.WriteLine("Starting data setup...");

            foreach (int an in Enumerable.Range(1990, 36))
            {
                Console.WriteLine($"Fetching projects for year {an}...");
                var result = await ProjectsService.GetProjectsAsync(an.ToString());
                if (result.Any())
                {
                    _projects.AddRange(result);
                }
                else
                {
                    Console.WriteLine($"No projects found for year {an}");
                }
            }

            Console.WriteLine($"Total projects fetched: {_projects.Count}");

            foreach (var project in _projects)
            {
                var projectUrls = await ProjectsService.GetAllPdfUrlsAsync(project["nr_cls"], project["an_cls"]);
                foreach (var url in projectUrls)
                {
                    Console.WriteLine($"Downloading PDF from URL: {url}");
                    PdfService.DownloadFromUrl(url);
                }
            }

            // Convertim PDF-urile in fisiere text
            PdfService.ConvertToText();
            // Impartim fisierele text in bucati mai mici
            ChunkTextFiles.ChunkText();
            // Generam embedding-urile pentru fiecare bucata de text
            await EmbeddingApiClient.EmbedBatchAsync(Directories.ChunkedTxtDirPath);
            // Luam punctele din fisierul JSON si le urcam in baza de date vectoriala Qdrant
            var path = Path.Combine(Directories.BaseDirPath, "embeddings.json");
            List<QdrantPoint> points = PointService.LoadPoints(path);
            var uploader = new QdrantUploader("localhost", 6334, "proiect-senat");
            await uploader.UploadPointsAsync(points);
            Console.WriteLine("Processing complete.");
        }

        static void TestOcrProcessor()
        {
            try
            {
                Console.WriteLine("== OCR Processor Testing ==");
                Console.WriteLine("Testing PdfOcrProcessor...");

                // Test if tessdata is accessible
                if (Directory.Exists(Directories.TessdataDirPath))
                {
                    Console.WriteLine("Tessdata directory found.");
                    var files = Directory.GetFiles(Directories.TessdataDirPath, "*.traineddata");
                    Console.WriteLine($"Found {files.Length} trained data files:");
                    foreach (var file in files)
                    {
                        Console.WriteLine($"  - {Path.GetFileName(file)}");
                    }
                }
                else
                {
                    Console.WriteLine("Tessdata directory not found.");
                }

                Console.WriteLine("OCR test completed successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"OCR test failed: {e.Message}");
                Console.WriteLine($"Stack trace: {e.StackTrace}");
            }
        }

        static void StartupMenu()
        {
            Console.WriteLine("== Main Menu ==");
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Setup data (download PDFs, convert to text, chunk, embed, upload to vector DB)");
            Console.WriteLine("2. Exit");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    DataSetupAsync().Wait();
                    StartupMenu();
                    break;
                case "2":
                    Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select 1 or 2.");
                    StartupMenu();
                    break;
            }
        }

        static async Task Main()
        {
            TestOcrProcessor();
            // await Task.Run(() => StartupMenu());
            Console.WriteLine("Application finished.");
        }
    }
}