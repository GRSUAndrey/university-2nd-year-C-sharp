using System.Xml.Serialization;

namespace TextProcessingLab;

[XmlType("Word")]
public class Word : Token
{
    [XmlText]
    public string Text { get; set; } = string.Empty;

    [XmlIgnore]
    public override string Value => Text;

    public Word()
    {
    }

    public Word(string text, string whitespaceBefore = "") : base(whitespaceBefore)
    {
        Text = text;
    }
}
