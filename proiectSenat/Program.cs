using proiectSenat;

class Program
{
    private static List<Dictionary<string, string>>? projects = new List<Dictionary<string, string>>();

    // luam toate proiectele din 1990-2025, le descarcam PDF-urile si le convertim in text
    static async void DataSetup()
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
    
    static async Task Main()
    {
        // facem setupul daca nu avem niciun pdf drept documentatie
            DataSetup();
    }
}