using System.Xml.Linq;
using Xunit;
using TextProcessingLab;

namespace TextProcessingLab.Tests;

public class TextProcessingTests
{
    [Fact]
    public void Parse_SeparatesWordsAndPunctuation_AndPreservesOriginalText()
    {
        const string source = "Hello, world!\nКак дела?  ";

        Text text = TextParser.Parse(source);

        Assert.Equal(source, text.ToString());
        Assert.Equal(2, text.Sentences.Count);
        Assert.Collection(
            text.Sentences[0].Tokens,
            token => Assert.IsType<Word>(token),
            token => Assert.IsType<Punctuation>(token),
            token => Assert.IsType<Word>(token),
            token => Assert.IsType<Punctuation>(token));
        Assert.True(text.Sentences[1].IsQuestion);
    }

    [Fact]
    public void Parse_DoesNotSplitDecimalNumberAtDecimalPoint()
    {
        Text text = TextParser.Parse("Value is 3.14. Done.");

        Assert.Equal(2, text.Sentences.Count);
        Assert.Equal("Value is 3.14.", text.Sentences[0].ToString());
        Assert.Equal("Done.", text.Sentences[1].ToString().Trim());
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsTextWithoutSentences()
    {
        Text text = TextParser.Parse(string.Empty);

        Assert.Empty(text.Sentences);
        Assert.Equal(string.Empty, text.ToString());
    }

    [Fact]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TextParser.Parse(null!));
    }

    [Fact]
    public void GetSentencesByWordCount_ReturnsAscendingWordCount()
    {
        Text text = TextParser.Parse("One? Big red fox! A bird flies.");

        int[] counts = text.GetSentencesByWordCount()
            .Select(sentence => sentence.WordCount)
            .ToArray();

        Assert.Equal(new[] { 1, 3, 3 }, counts);
    }

    [Fact]
    public void GetSentencesByLength_ReturnsAscendingSentenceLength()
    {
        Text text = TextParser.Parse("Longer sentence! A? Done.");

        string[] sorted = text.GetSentencesByLength()
            .Select(sentence => sentence.ToString().Trim())
            .ToArray();

        Assert.Equal(new[] { "A?", "Done.", "Longer sentence!" }, sorted);
    }

    [Fact]
    public void FindDistinctWordsInQuestions_ReturnsUniqueWordsOfRequestedLength()
    {
        Text text = TextParser.Parse("Why now? Why not? This is done.");

        List<string> words = text.FindDistinctWordsInQuestions(3);

        Assert.Equal(new[] { "Why", "now", "not" }, words);
    }

    [Fact]
    public void FindDistinctWordsInQuestions_IsCaseInsensitiveWhenRemovingDuplicates()
    {
        Text text = TextParser.Parse("Why? why? WHY!");

        List<string> words = text.FindDistinctWordsInQuestions(3);

        Assert.Equal(new[] { "Why" }, words);
    }

    [Fact]
    public void RemoveWordsOfLengthStartingWithConsonant_RemovesMatchingEnglishWords()
    {
        Text text = TextParser.Parse("cat age dog.");

        int removed = text.RemoveWordsOfLengthStartingWithConsonant(3);
        string[] remainingWords = text.Sentences
            .SelectMany(sentence => sentence.Tokens)
            .OfType<Word>()
            .Select(word => word.Text)
            .ToArray();

        Assert.Equal(2, removed);
        Assert.Equal(new[] { "age" }, remainingWords);
    }

    [Fact]
    public void RemoveWordsOfLengthStartingWithConsonant_SupportsRussianWords()
    {
        Text text = TextParser.Parse("кот ами дом.");

        int removed = text.RemoveWordsOfLengthStartingWithConsonant(3);
        string[] remainingWords = text.Sentences
            .SelectMany(sentence => sentence.Tokens)
            .OfType<Word>()
            .Select(word => word.Text)
            .ToArray();

        Assert.Equal(2, removed);
        Assert.Equal(new[] { "ами" }, remainingWords);
    }

    [Fact]
    public void ReplaceWordsOfLengthInSentence_ReplacesOnlySelectedSentence()
    {
        Text text = TextParser.Parse("This test works. Stay safe.");

        int replaced = text.ReplaceWordsOfLengthInSentence(1, 4, "longer");

        Assert.Equal(2, replaced);
        Assert.Equal("longer longer works. Stay safe.", text.ToString());
    }

    [Fact]
    public void ReplaceWordsOfLengthInSentence_EmptyReplacementRemovesMatchingWords()
    {
        Text text = TextParser.Parse("This test works.");

        int changed = text.ReplaceWordsOfLengthInSentence(1, 4, string.Empty);
        string[] remainingWords = text.Sentences
            .SelectMany(sentence => sentence.Tokens)
            .OfType<Word>()
            .Select(word => word.Text)
            .ToArray();

        Assert.Equal(2, changed);
        Assert.Equal(new[] { "works" }, remainingWords);
    }

    [Fact]
    public void ReplaceWordsOfLengthInSentence_InvalidSentenceNumberThrows()
    {
        Text text = TextParser.Parse("Only one sentence.");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            text.ReplaceWordsOfLengthInSentence(0, 4, "word"));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            text.ReplaceWordsOfLengthInSentence(2, 4, "word"));
    }

    [Fact]
    public void RemoveStopWords_RemovesWordsFromProvidedSetCaseInsensitively()
    {
        Text text = TextParser.Parse("The cat and DOG.");
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the",
            "AND"
        };

        int removed = text.RemoveStopWords(stopWords);
        string[] remainingWords = text.Sentences
            .SelectMany(sentence => sentence.Tokens)
            .OfType<Word>()
            .Select(word => word.Text)
            .ToArray();

        Assert.Equal(2, removed);
        Assert.Equal(new[] { "cat", "DOG" }, remainingWords);
    }

    [Fact]
    public void RemoveStopWords_NullSetThrowsArgumentNullException()
    {
        Text text = TextParser.Parse("Some text.");

        Assert.Throws<ArgumentNullException>(() => text.RemoveStopWords(null!));
    }

    [Fact]
    public void StopWordLoader_LoadsWordsAndIgnoresMissingFiles()
    {
        string path = Path.Combine(Path.GetTempPath(), $"stopwords-{Guid.NewGuid():N}.txt");
        try
        {
            File.WriteAllLines(path, new[] { "the", "and", "  ", "THE" });

            HashSet<string> words = StopWordLoader.LoadFromFiles(new[]
            {
                path,
                path + ".missing"
            });

            Assert.Equal(2, words.Count);
            Assert.Contains("the", words);
            Assert.Contains("THE", words);
            Assert.Contains("and", words);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [Fact]
    public void XmlTextExporter_WritesXmlThatCanBeLoaded()
    {
        Text text = TextParser.Parse("Hello? Как дела!");
        string path = Path.Combine(Path.GetTempPath(), $"text-{Guid.NewGuid():N}.xml");

        try
        {
            XmlTextExporter.Export(text, path);

            XDocument document = XDocument.Load(path);

            Assert.Equal("Text", document.Root?.Name.LocalName);
            Assert.Equal(2, document.Root!.Elements("Sentence").Count());
            Assert.Contains(document.Descendants("Word"), element => element.Value == "Hello");
            Assert.Contains(document.Descendants("Punctuation"), element => element.Value == "?");
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
