using TextProcessingLab;
using Xunit;

namespace TextProcessingLab.Tests;

public class TextConcordanceTests
{
    [Fact]
    public void BuildConcordance_CountsOccurrencesAndKeepsUniqueLineNumbers()
    {
        Text text = TextParser.Parse("Piper piper picked.\nPICKED a piper!\n");

        IReadOnlyList<ConcordanceEntry> concordance = text.BuildConcordance();
        ConcordanceEntry piper = Assert.Single(concordance.Where(
            entry => entry.Word == "piper"));

        Assert.Equal(3, piper.Frequency);
        Assert.Equal(new[] { 1, 2 }, piper.LineNumbers);
    }

    [Fact]
    public void BuildConcordance_ReturnsWordsInAlphabeticalOrder()
    {
        Text text = TextParser.Parse("zebra apple Zebra");

        IReadOnlyList<ConcordanceEntry> concordance = text.BuildConcordance();

        Assert.Equal(new[] { "apple", "zebra" },
            concordance.Select(entry => entry.Word));
        Assert.Equal(2, concordance.Single(entry => entry.Word == "zebra").Frequency);
    }

    [Fact]
    public void BuildConcordance_UsesPhysicalLineNumbers()
    {
        Text text = TextParser.Parse("One two.\nOne!\n");

        IReadOnlyList<ConcordanceEntry> concordance = text.BuildConcordance();
        ConcordanceEntry one = concordance.Single(entry => entry.Word == "one");
        ConcordanceEntry two = concordance.Single(entry => entry.Word == "two");

        Assert.Equal(2, one.Frequency);
        Assert.Equal(new[] { 1, 2 }, one.LineNumbers);
        Assert.Equal(new[] { 1 }, two.LineNumbers);
    }

    [Fact]
    public void BuildConcordance_MatchesTheAssignmentExample()
    {
        const string source =
            "Peter Piper picked a peck of pickled peppers. A peck of pickled\n" +
            "peppers Peter Piper picked. If Peter Piper picked a peck of\n" +
            "pickled peppers, where is the peck that Peter Piper picked?";

        Text text = TextParser.Parse(source);
        IReadOnlyList<ConcordanceEntry> concordance = text.BuildConcordance();

        string[] actual = concordance
            .Select(entry => $"{entry.Word} {entry.Frequency}: {string.Join(" ", entry.LineNumbers)}")
            .ToArray();

        string[] expected =
        {
            "a 3: 1 2",
            "if 1: 2",
            "is 1: 3",
            "of 3: 1 2",
            "peck 4: 1 2 3",
            "peppers 3: 1 2 3",
            "peter 4: 1 2 3",
            "picked 4: 1 2 3",
            "pickled 3: 1 3",
            "piper 4: 1 2 3",
            "that 1: 3",
            "the 1: 3",
            "where 1: 3"
        };

        Assert.Equal(expected, actual);
    }

}
