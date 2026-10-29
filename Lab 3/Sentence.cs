using System.Xml.Serialization;

namespace TextProcessingLab;

[XmlType("Sentence")]
public class Sentence
{
    [XmlElement("Word", typeof(Word))]
    [XmlElement("Punctuation", typeof(Punctuation))]
    public List<Token> Tokens { get; set; } = new();

    [XmlIgnore]
    public int WordCount => Tokens.OfType<Word>().Count(word => word.Text.Length > 0);

    [XmlIgnore]
    public int Length => ToString().Trim().Length;

    [XmlIgnore]
    public bool IsQuestion => Tokens
        .OfType<Punctuation>()
        .Any(punctuation => punctuation.Symbol.Contains('?'));

    public override string ToString() => string.Concat(Tokens.Select(token => token.ToString()));
}
