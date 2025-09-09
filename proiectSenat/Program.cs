using proiectSenat;

class Program
{
    private static List<Dictionary<string, string>>? projects = new List<Dictionary<string, string>>();

    static async Task Main()
    {
        PdfService.ConvertToText();
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
    }
}