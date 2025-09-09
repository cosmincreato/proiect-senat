using proiectSenat;
using System.Xml;
using System.Text;

class Program
{
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://www.senat.ro/"),
    };

    private static async Task GetProjects(string an = "", string nrSenat = "", string nrDeputati = "")
    {
        try
        {
            string endpoint = $"exportdata.asmx/ListaProiectelor?An={an}&NrSenat={nrSenat}&NrDeputati={nrDeputati}";
            HttpResponseMessage response = await sharedClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response received:");
                Console.WriteLine(PrintXML(content));
            }
            else
            {
                Console.WriteLine($"Request failed with status: {response.StatusCode}");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HTTP request error: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"General error: {e.Message}");
        }
    }

    
    static async Task Main()
    {
        PdfService.ConvertToText();
        await GetProjects("2025");
    }
}