namespace TextProcessingLab;

/// <summary>
/// Stores the total number of occurrences of a word and the source lines
/// in which the word appears.
/// </summary>
public sealed class ConcordanceEntry
{
    private readonly SortedSet<int> _lineNumbers = new();

    public string Word { get; }

    public int Frequency { get; private set; }

    /// <summary>
    /// Unique line numbers in ascending order.
    /// </summary>
    public IReadOnlyCollection<int> LineNumbers => _lineNumbers;

    internal ConcordanceEntry(string word)
    {
        Word = word;
    }

    internal void AddOccurrence(int lineNumber)
    {
        Frequency++;
        _lineNumbers.Add(lineNumber);
    }
}
