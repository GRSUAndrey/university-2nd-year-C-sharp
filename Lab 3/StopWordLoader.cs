namespace TextProcessingLab;

public static class StopWordLoader
{
    public static HashSet<string> LoadFromFiles(IEnumerable<string> filePaths)
    {
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string path in filePaths)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            foreach (string line in File.ReadLines(path))
            {
                string word = line.Trim().Trim('\uFEFF');
                if (word.Length > 0)
                {
                    stopWords.Add(word);
                }
            }
        }

        return stopWords;
    }
}
