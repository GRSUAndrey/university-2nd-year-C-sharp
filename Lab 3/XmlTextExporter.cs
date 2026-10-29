using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TextProcessingLab;

public static class XmlTextExporter
{
    public static void Export(Text text, string filePath)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var serializer = new XmlSerializer(typeof(Text));
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = true
        };

        using XmlWriter writer = XmlWriter.Create(filePath, settings);
        serializer.Serialize(writer, text);
    }
}
