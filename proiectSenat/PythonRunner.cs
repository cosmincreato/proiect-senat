namespace proiectSenat;

using System.Diagnostics;

public class PythonRunner
{
    public static void RunEmbeddingScript()
    {
        var process = new Process();
        process.StartInfo.FileName = "python3";
        process.StartInfo.Arguments = Path.Combine(Directory.GetParent(Directories.BaseDirPath).FullName, "embedding.py");
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WorkingDirectory = Directory.GetParent(Directories.BaseDirPath).FullName;

        process.Start();
        process.WaitForExit();
    }
}