using proiectSenat;

class Program
{
    private static List<Dictionary<string, string>> projects = new List<Dictionary<string, string>>();

    // luam toate proiectele din 1990-2025, le descarcam PDF-urile si le convertim in text
    static async Task DataSetup()
    {

        Console.WriteLine("Starting data setup...");

        foreach (int an in Enumerable.Range(1990, 36))
        {
            var result = await ProjectsService.GetProjectsAsync(an.ToString());
            if (result.Any())
            {
                projects.AddRange(result);
            }
            else
            {
                Console.WriteLine($"No projects found for year {an}");
            }
        }

        Console.WriteLine($"Total projects fetched: {projects.Count}");

        foreach (var project in projects)
        {
            var projectUrls = await ProjectsService.GetAllPdfUrlsAsync(project["nr_cls"], project["an_cls"]);
            foreach (var url in projectUrls)
            {
                Console.WriteLine($"Downloading PDF from URL: {url}");
                PdfService.DownloadFromUrl(url);
            }
        }

        PdfService.ConvertToText();


        Console.WriteLine("Processing complete.");
    }

    static void TestOcrProcessor()
    {
        try
        {
            Console.WriteLine("Testing PdfOcrProcessor . . .");

            // Test if the OCR processor can be instantiated
            var ocrProcessor = new PdfOcrProcessor();
            Console.WriteLine("PdfOcrProcessor instantiated successfully.");

            // Test if tessdata is accessible
            if (System.IO.Directory.Exists(@"..\..\..\tessdata"))
            {
                Console.WriteLine("Tessdata directory found.");
                var files = System.IO.Directory.GetFiles(@"..\..\..\tessdata", "*.traineddata");
                Console.WriteLine($"Found {files.Length} trained data files:");
                foreach (var file in files)
                {
                    Console.WriteLine($"  - {System.IO.Path.GetFileName(file)}");
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

    static void NaturalLanguageInterface()
    {
        Console.WriteLine("\n=== Natural Language Processing Interface ===");
        Console.WriteLine("Ask questions about the processed documents in Romanian or English.");
        Console.WriteLine("Type 'exit' to return to main menu, 'help' for examples, or 'stats' for statistics.\n");

        var nlpService = new NaturalLanguageService();
        var stats = nlpService.GetStatistics();
        Console.WriteLine($"Loaded {stats["TotalDocuments"]} documents with {stats["TotalUniqueTerms"]} unique terms.\n");

        while (true)
        {
            Console.Write("Ask a question: ");
            var question = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(question))
                continue;

            if (question.ToLower() == "exit")
                break;

            if (question.ToLower() == "help")
            {
                ShowNLPHelp();
                continue;
            }

            if (question.ToLower() == "stats")
            {
                ShowNLPStats(nlpService);
                continue;
            }

            if (question.ToLower() == "reindex")
            {
                Console.WriteLine("Reindexing documents...");
                nlpService.ReindexDocuments();
                Console.WriteLine("Reindexing complete.\n");
                continue;
            }

            try
            {
                Console.WriteLine("\nSearching...\n");
                var answer = nlpService.AnswerQuestion(question);
                Console.WriteLine(answer);
                Console.WriteLine(new string('-', 80));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing question: {ex.Message}");
            }
        }
    }

    static void ShowNLPHelp()
    {
        Console.WriteLine("\n=== Examples of questions you can ask ===");
        Console.WriteLine("• Ce legi se referă la educație?");
        Console.WriteLine("• Câți bani sunt alocați pentru digitalizare?");
        Console.WriteLine("• Care sunt modificările la Codul Muncii?");
        Console.WriteLine("• Ce măsuri sunt pentru protejarea mediului?");
        Console.WriteLine("• Când intră în vigoare legea educației?");
        Console.WriteLine("• Ce este Agenția Națională pentru Protecția Mediului?");
        Console.WriteLine("\n=== Available commands ===");
        Console.WriteLine("• 'help' - show this help");
        Console.WriteLine("• 'stats' - show document statistics");
        Console.WriteLine("• 'reindex' - reindex all documents");
        Console.WriteLine("• 'exit' - return to main menu\n");
    }

    static void ShowNLPStats(NaturalLanguageService nlpService)
    {
        var stats = nlpService.GetStatistics();
        Console.WriteLine("\n=== Document Statistics ===");
        Console.WriteLine($"Total Documents: {stats["TotalDocuments"]}");
        Console.WriteLine($"Total Unique Terms: {stats["TotalUniqueTerms"]}");
        Console.WriteLine($"Average Document Length: {stats["AverageDocumentLength"]:F0} characters");
        
        var files = (List<string>)stats["IndexedFiles"];
        if (files.Any())
        {
            Console.WriteLine("\nIndexed Files:");
            foreach (var file in files)
            {
                Console.WriteLine($"• {file}");
            }
        }
        Console.WriteLine();
    }

    static void StartupMenu()
    {
        Console.WriteLine("Select an option:");
        Console.WriteLine("1. Setup data (download PDFs and convert to text)");
        Console.WriteLine("2. Natural Language Q&A (ask questions about processed documents)");
        Console.WriteLine("3. Exit");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                DataSetup().Wait();
                break;
            case "2":
                NaturalLanguageInterface();
                break;
            case "3":
                System.Environment.Exit(1);
                break;
            default:
                Console.WriteLine("Invalid choice. Please select 1, 2, or 3.");
                break;
        }
        StartupMenu();
    }
    static async Task Main()
    {
        TestOcrProcessor();
        await Task.Run(() => StartupMenu());

        Console.WriteLine("Application finished.");
    }
}