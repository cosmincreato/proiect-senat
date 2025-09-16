namespace proiectSenat;

public static class ChunkTextFiles
{
    public static void ChunkText()
    {
        Console.WriteLine("Starting text chunking process...");
        int chunkSize = 1000; // characters per chunk
        
        if (!Directory.Exists(Directories.ChunkedTxtDirPath))
            Directory.CreateDirectory(Directories.ChunkedTxtDirPath);

        foreach (var filePath in Directory.GetFiles(Directories.TxtDirPath, "*.txt"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string text = File.ReadAllText(filePath);

            // Optional cleaning step
            text = text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Trim();

            int totalChunks = (int)Math.Ceiling((double)text.Length / chunkSize);
            for (int i = 0; i < totalChunks; i++)
            {
                int startIndex = i * chunkSize;
                int length = Math.Min(chunkSize, text.Length - startIndex);
                string chunk = text.Substring(startIndex, length);

                // originalfilename_chunkN.txt
                string chunkFileName = $"{fileName}_chunk{i + 1}.txt";
                string outputPath = Path.Combine(Directories.ChunkedTxtDirPath, chunkFileName);

                File.WriteAllText(outputPath, chunk);
            }
        }
    }
}