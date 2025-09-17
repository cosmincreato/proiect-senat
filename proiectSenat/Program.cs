namespace proiectSenat
{
    class Program
    {
        private static List<Dictionary<string, string>> _projects = new List<Dictionary<string, string>>();

        // luam toate proiectele din 1990-2025, le descarcam PDF-urile si le convertim in text
        static async Task DataSetup()
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

            PdfService.ConvertToText();
            ChunkTextFiles.ChunkText();


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
            Console.WriteLine("1. Setup data (download PDFs and convert to text)");
            Console.WriteLine("2. Exit");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    DataSetup().Wait();
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
            PdfService.ConvertToText();
            // await Task.Run(() => StartupMenu());
            Console.WriteLine("Application finished.");
        }
    }
}