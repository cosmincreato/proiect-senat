namespace proiectSenat;

using System.Diagnostics;

public class PythonRunner
{
    public static void RunEmbeddingScript()
    {
        if (!File.Exists(Path.Combine(Directories.BaseDirPath, "embeddings.json")))
        {
            Console.WriteLine("Starting embedding generation via Python script...");
            
            var process = new Process();
            process.StartInfo.FileName = "python3";
            process.StartInfo.Arguments =
                Path.Combine(Directory.GetParent(Directories.BaseDirPath).FullName, "embedding.py");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Directory.GetParent(Directories.BaseDirPath).FullName;

            process.Start();
            process.WaitForExit();

            Console.WriteLine("Saved embeddings to embeddings.json");
        }
        else
            Console.WriteLine("Skipping embedding generation, embeddings.json already exists.");
    }
}