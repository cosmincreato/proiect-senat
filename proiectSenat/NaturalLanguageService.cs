using System.Text.Json;
using System.Text.RegularExpressions;

namespace proiectSenat
{
    public class DocumentSearchResult
    {
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public double RelevanceScore { get; set; }
        public List<string> MatchingPassages { get; set; } = new List<string>();
    }

    public class NaturalLanguageService
    {
        private readonly string _outputDirectory;
        private List<(string fileName, string content)> _documents;
        private Dictionary<string, List<string>> _invertedIndex;

        public NaturalLanguageService(string outputDirectory = "output")
        {
            _outputDirectory = outputDirectory;
            _documents = new List<(string, string)>();
            _invertedIndex = new Dictionary<string, List<string>>();
            IndexDocuments();
        }

        private void IndexDocuments()
        {
            Console.WriteLine("Indexing documents from output directory...");
            
            if (!Directory.Exists(_outputDirectory))
            {
                Console.WriteLine($"Output directory {_outputDirectory} does not exist. Creating it...");
                Directory.CreateDirectory(_outputDirectory);
                return;
            }

            var textFiles = Directory.GetFiles(_outputDirectory, "*.txt");
            Console.WriteLine($"Found {textFiles.Length} text files to index.");

            foreach (var filePath in textFiles)
            {
                try
                {
                    string content = File.ReadAllText(filePath);
                    string fileName = Path.GetFileName(filePath);
                    _documents.Add((fileName, content));

                    // Create inverted index
                    var words = ExtractWords(content);
                    foreach (var word in words)
                    {
                        if (!_invertedIndex.ContainsKey(word))
                            _invertedIndex[word] = new List<string>();
                        
                        if (!_invertedIndex[word].Contains(fileName))
                            _invertedIndex[word].Add(fileName);
                    }

                    Console.WriteLine($"Indexed: {fileName} ({content.Length} characters)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error indexing file {filePath}: {ex.Message}");
                }
            }

            Console.WriteLine($"Indexing complete. Total documents: {_documents.Count}");
            Console.WriteLine($"Total unique terms: {_invertedIndex.Keys.Count}");
        }

        private List<string> ExtractWords(string text)
        {
            // Convert to lowercase and extract words (including Romanian diacritics)
            var words = new List<string>();
            var regex = new Regex(@"[a-zA-ZăâîșțĂÂÎȘȚ]+", RegexOptions.IgnoreCase);
            var matches = regex.Matches(text.ToLower());
            
            foreach (Match match in matches)
            {
                if (match.Value.Length > 2) // Ignore very short words
                    words.Add(match.Value);
            }
            
            return words.Distinct().ToList();
        }

        public List<DocumentSearchResult> SearchDocuments(string query)
        {
            Console.WriteLine($"Searching for: '{query}'");
            
            var queryWords = ExtractWords(query);
            var results = new List<DocumentSearchResult>();

            foreach (var (fileName, content) in _documents)
            {
                double score = CalculateRelevanceScore(content, queryWords);
                if (score > 0)
                {
                    var matchingPassages = ExtractRelevantPassages(content, queryWords);
                    results.Add(new DocumentSearchResult
                    {
                        FileName = fileName,
                        Content = content,
                        RelevanceScore = score,
                        MatchingPassages = matchingPassages
                    });
                }
            }

            return results.OrderByDescending(r => r.RelevanceScore).ToList();
        }

        private double CalculateRelevanceScore(string content, List<string> queryWords)
        {
            double score = 0;
            string contentLower = content.ToLower();

            foreach (var word in queryWords)
            {
                // Count occurrences of each query word
                int count = Regex.Matches(contentLower, @"\b" + Regex.Escape(word) + @"\b").Count;
                score += count;
                
                // Bonus for words in title or important sections
                if (contentLower.Contains("proiect de lege") && contentLower.IndexOf(word) < 200)
                    score += count * 2;
            }

            // Normalize by document length
            return score / Math.Max(content.Length / 1000.0, 1);
        }

        private List<string> ExtractRelevantPassages(string content, List<string> queryWords)
        {
            var passages = new List<string>();
            var sentences = content.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sentence in sentences)
            {
                if (sentence.Trim().Length < 20) continue; // Skip very short sentences

                bool containsQueryWord = false;
                foreach (var word in queryWords)
                {
                    if (sentence.ToLower().Contains(word.ToLower()))
                    {
                        containsQueryWord = true;
                        break;
                    }
                }

                if (containsQueryWord)
                {
                    passages.Add(sentence.Trim() + ".");
                }
            }

            return passages.Take(3).ToList(); // Return top 3 most relevant passages
        }

        public string AnswerQuestion(string question)
        {
            var searchResults = SearchDocuments(question);
            
            if (!searchResults.Any())
            {
                return "Nu am găsit informații relevante în documentele procesate pentru întrebarea dumneavoastră.";
            }

            var response = new System.Text.StringBuilder();
            response.AppendLine($"Am găsit {searchResults.Count} document(e) relevant(e) pentru întrebarea: '{question}'\n");

            int displayCount = Math.Min(3, searchResults.Count);
            for (int i = 0; i < displayCount; i++)
            {
                var result = searchResults[i];
                response.AppendLine($"📄 **{result.FileName}** (Scor relevență: {result.RelevanceScore:F2})");
                
                if (result.MatchingPassages.Any())
                {
                    response.AppendLine("Pasaje relevante:");
                    foreach (var passage in result.MatchingPassages.Take(2))
                    {
                        response.AppendLine($"• {passage}");
                    }
                }
                else
                {
                    // Show first 200 characters if no specific passages found
                    var preview = result.Content.Length > 200 
                        ? result.Content.Substring(0, 200) + "..." 
                        : result.Content;
                    response.AppendLine($"Preview: {preview}");
                }
                response.AppendLine();
            }

            if (searchResults.Count > displayCount)
            {
                response.AppendLine($"... și încă {searchResults.Count - displayCount} document(e) relevant(e).");
            }

            return response.ToString();
        }

        public void ReindexDocuments()
        {
            _documents.Clear();
            _invertedIndex.Clear();
            IndexDocuments();
        }

        public List<string> GetAvailableDocuments()
        {
            return _documents.Select(d => d.fileName).ToList();
        }

        public Dictionary<string, object> GetStatistics()
        {
            return new Dictionary<string, object>
            {
                ["TotalDocuments"] = _documents.Count,
                ["TotalUniqueTerms"] = _invertedIndex.Keys.Count,
                ["IndexedFiles"] = _documents.Select(d => d.fileName).ToList(),
                ["AverageDocumentLength"] = _documents.Any() ? _documents.Average(d => d.content.Length) : 0
            };
        }
    }
}