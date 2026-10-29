using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace TextProcessingLab;

[XmlRoot("Text")]
public class Text
{
    [XmlElement("Sentence")]
    public List<Sentence> Sentences { get; set; } = new();

    // Preserves whitespace at the very end of the input, if present.
    [XmlElement("TrailingWhitespace")]
    public string TrailingWhitespace { get; set; } = string.Empty;

    public override string ToString() =>
        string.Concat(Sentences.Select(sentence => sentence.ToString())) + TrailingWhitespace;

    public IEnumerable<Sentence> GetSentencesByWordCount() =>
        Sentences.OrderBy(sentence => sentence.WordCount);

    public IEnumerable<Sentence> GetSentencesByLength() =>
        Sentences.OrderBy(sentence => sentence.Length);

    public List<string> FindDistinctWordsInQuestions(int wordLength)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Sentence sentence in Sentences.Where(sentence => sentence.IsQuestion))
        {
            foreach (Word word in sentence.Tokens.OfType<Word>())
            {
                if (word.Text.Length == wordLength && seen.Add(word.Text))
                {
                    result.Add(word.Text);
                }
            }
        }

        return result;
    }

    public int RemoveWordsOfLengthStartingWithConsonant(int wordLength)
    {
        int removed = 0;

        foreach (Sentence sentence in Sentences)
        {
            removed += sentence.Tokens.RemoveAll(token =>
                token is Word word &&
                word.Text.Length == wordLength &&
                StartsWithConsonant(word.Text));
        }

        return removed;
    }

    // sentenceNumber is 1-based for easier use from the console menu.
    public int ReplaceWordsOfLengthInSentence(
        int sentenceNumber,
        int wordLength,
        string replacement)
    {
        if (sentenceNumber < 1 || sentenceNumber > Sentences.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sentenceNumber), "Номер предложения вне диапазона.");
        }

        Sentence sentence = Sentences[sentenceNumber - 1];
        int changed = 0;

        for (int i = sentence.Tokens.Count - 1; i >= 0; i--)
        {
            if (sentence.Tokens[i] is not Word word || word.Text.Length != wordLength)
            {
                continue;
            }

            changed++;
            if (replacement.Length == 0)
            {
                sentence.Tokens.RemoveAt(i);
            }
            else
            {
                word.Text = replacement;
            }
        }

        return changed;
    }

    public int RemoveStopWords(ISet<string> stopWords)
    {
        ArgumentNullException.ThrowIfNull(stopWords);
        int removed = 0;

        foreach (Sentence sentence in Sentences)
        {
            removed += sentence.Tokens.RemoveAll(token =>
                token is Word word && stopWords.Contains(word.Text));
        }

        return removed;
    }

    private static bool StartsWithConsonant(string word)
    {
        char firstLetter = word.FirstOrDefault(char.IsLetter);
        if (firstLetter == '\0')
        {
            return false;
        }

        // Normalize accented Latin letters and Russian ё before checking vowels.
        string normalized = firstLetter.ToString().ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);
        char baseLetter = normalized.FirstOrDefault(character =>
            CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark);

        const string vowels = "aeiouаеёиоуыэюя";
        return baseLetter != '\0' && !vowels.Contains(baseLetter);
    }
}
