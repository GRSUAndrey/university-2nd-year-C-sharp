
Третья лаба по шарпам, обработчик текста.

# TextProcessingLab — Assignment #3

C# console application implementing the text-tokenisation assignment.

## Files
- `Text.cs`, `Sentence.cs`, `Word.cs`, `Punctuation.cs`, `Token.cs` — object model.
- `TextParser.cs` — parses Russian or English text and preserves token spacing and punctuation.
- `StopWordLoader.cs` — loads stopwords from the provided files.
- `XmlTextExporter.cs` — XML export with `System.Xml.Serialization`.
- `Program.cs` — interactive console menu for all required operations.
- `stopwords_en.txt`, `stopwords_ru(1).txt` — source stopword files supplied with the assignment.

## Run
Install the .NET 10 SDK, then open a terminal in this directory and run:

```bash
dotnet run
```

To load a text file immediately:

```bash
dotnet run -- "path/to/input.txt"
```

The program menu implements:
1. load a text file;
2. enter text manually (finish with a line containing `END`);
3. display the text;
4. sort sentences by ascending word count;
5. sort sentences by ascending length;
6. find distinct words of a specified length in interrogative sentences;
7. remove words of a specified length starting with a consonant;
8. replace words of a specified length in a 1-based selected sentence;
9. remove Russian and English stopwords;
10. export the `Text` object to XML.

