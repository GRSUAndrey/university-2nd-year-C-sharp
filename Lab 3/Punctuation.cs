using System.Xml.Serialization;

namespace TextProcessingLab;

[XmlType("Punctuation")]
public class Punctuation : Token
{
    [XmlText]
    public string Symbol { get; set; } = string.Empty;

    [XmlIgnore]
    public override string Value => Symbol;

    public Punctuation()
    {
    }

    public Punctuation(string symbol, string whitespaceBefore = "") : base(whitespaceBefore)
    {
        Symbol = symbol;
    }
}
