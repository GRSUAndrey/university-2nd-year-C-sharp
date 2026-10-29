using System.Xml.Serialization;

namespace TextProcessingLab;

[XmlInclude(typeof(Word))]
[XmlInclude(typeof(Punctuation))]
public abstract class Token
{
    // Whitespace that appeared before this token in the original text.
    [XmlAttribute("spaceBefore")]
    public string WhitespaceBefore { get; set; } = string.Empty;

    [XmlIgnore]
    public abstract string Value { get; }

    protected Token()
    {
    }

    protected Token(string whitespaceBefore)
    {
        WhitespaceBefore = whitespaceBefore;
    }

    public override string ToString() => WhitespaceBefore + Value;
}
