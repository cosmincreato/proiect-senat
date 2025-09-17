namespace proiectSenat
{
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;
    using BERTTokenizers;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

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
            // await Task.Run(() => StartupMenu());

            // Use the built-in multilingual tokenizer (no custom vocab possible)
            var tokenizer = new BertMultilingualTokenizer();

            // Load a compatible ONNX model (multilingual or base)
            var session = new InferenceSession(Path.Combine(Directories.ModelsDirPath, "model.onnx"));

            // Romanian text example
            string text = "Aceasta este o propoziție în limba română.";
            
            var tokens = tokenizer.Tokenize(text); // List<(string Token, int VocabularyIndex, long SegmentIndex)>
            var clsToken = tokenizer.Tokenize("[CLS]").First();
            var sepToken = tokenizer.Tokenize("[SEP]").First();
            tokens.Insert(0, clsToken);
            tokens.Add(sepToken);

            // Input IDs are the VocabularyIndex
            var inputIds = tokens.Select(t => t.VocabularyIndex).ToArray();
            
            
            // Attention mask (1s for all tokens)
            var attentionMask = Enumerable.Repeat(1L, inputIds.Length).ToArray();

            // Prepare tensors
            var inputIdsTensor = new DenseTensor<long>(new[] { 1, inputIds.Length });
            var attentionMaskTensor = new DenseTensor<long>(new[] { 1, inputIds.Length });
            for (int i = 0; i < inputIds.Length; i++)
            {
                inputIdsTensor[0, i] = inputIds[i];
                attentionMaskTensor[0, i] = attentionMask[i];
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
                NamedOnnxValue.CreateFromTensor("attention_mask", attentionMaskTensor)
            };

            // Run the model
            using var results = session.Run(inputs);
            var hiddenState = results.First().AsTensor<float>();

            // Extract [CLS] embedding (first token)
            int hiddenSize = hiddenState.Dimensions[2];
            float[] embedding = new float[hiddenSize];
            for (int i = 0; i < hiddenSize; i++)
                embedding[i] = hiddenState[0, 0, i];

            Console.WriteLine("Embedding (first 10 dims): " + string.Join(", ", embedding.Take(10)));

            Console.WriteLine("Application finished.");
        }
    }
}