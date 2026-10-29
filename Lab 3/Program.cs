using System.Text;

namespace TextProcessingLab;

internal static class Program
{
    private static Text _text = new();

    private static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length > 0)
        {
            LoadTextFile(args[0]);
        }

        bool exit = false;
        while (!exit)
        {
            PrintMenu();
            Console.Write("Выберите пункт меню: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": LoadTextFile(Ask("Путь к текстовому файлу: ")); break;
                case "2": EnterTextManually(); break;
                case "3": PrintCurrentText(); break;
                case "4": PrintSentences(_text.GetSentencesByWordCount(), "По возрастанию числа слов"); break;
                case "5": PrintSentences(_text.GetSentencesByLength(), "По возрастанию длины предложения"); break;
                case "6": FindWordsInQuestions(); break;
                case "7": RemoveConsonantWords(); break;
                case "8": ReplaceWordsInSentence(); break;
                case "9": RemoveStopWords(); break;
                case "10": ExportToXml(); break;
                case "0": exit = true; break;
                default: Console.WriteLine("Неизвестный пункт меню."); break;
            }

            if (!exit)
            {
                Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
                Console.ReadLine();
            }
        }
    }

    private static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("Лабораторная работа №3 — обработка текста");
        Console.WriteLine($"Предложений в тексте: {_text.Sentences.Count}");
        Console.WriteLine("1. Загрузить текст из файла");
        Console.WriteLine("2. Ввести текст вручную");
        Console.WriteLine("3. Вывести текущий текст");
        Console.WriteLine("4. Предложения по возрастанию количества слов");
        Console.WriteLine("5. Предложения по возрастанию длины");
        Console.WriteLine("6. Уникальные слова заданной длины в вопросительных предложениях");
        Console.WriteLine("7. Удалить слова заданной длины, начинающиеся с согласной");
        Console.WriteLine("8. Заменить слова заданной длины в указанном предложении");
        Console.WriteLine("9. Удалить стоп-слова (русские и английские)");
        Console.WriteLine("10. Экспортировать объект Text в XML");
        Console.WriteLine("0. Выход");
        Console.WriteLine();
    }

    private static void LoadTextFile(string path)
    {
        path = NormalizePath(path);
        try
        {
            string source = File.ReadAllText(path, Encoding.UTF8);
            _text = TextParser.Parse(source);
            Console.WriteLine($"Файл загружен. Предложений: {_text.Sentences.Count}.");
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            Console.WriteLine($"Не удалось загрузить файл: {exception.Message}");
        }
    }

    private static void EnterTextManually()
    {
        Console.WriteLine("Вводите текст построчно. Для завершения введите отдельной строкой END:");
        var lines = new List<string>();

        while (true)
        {
            string? line = Console.ReadLine();
            if (line is null || line.Equals("END", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            lines.Add(line);
        }

        _text = TextParser.Parse(string.Join(Environment.NewLine, lines));
        Console.WriteLine($"Текст принят. Предложений: {_text.Sentences.Count}.");
    }

    private static void PrintCurrentText()
    {
        Console.WriteLine("Текущий текст:");
        Console.WriteLine(_text.ToString());
    }

    private static void PrintSentences(IEnumerable<Sentence> sentences, string title)
    {
        Console.WriteLine(title + ":");
        int number = 1;

        foreach (Sentence sentence in sentences)
        {
            Console.WriteLine($"{number++}. {sentence.ToString().Trim()}");
        }
    }

    private static void FindWordsInQuestions()
    {
        int length = AskPositiveOrZeroInt("Длина слова: ");
        List<string> words = _text.FindDistinctWordsInQuestions(length);

        if (words.Count == 0)
        {
            Console.WriteLine("Подходящие слова не найдены.");
            return;
        }

        Console.WriteLine("Уникальные слова:");
        Console.WriteLine(string.Join(", ", words));
    }

    private static void RemoveConsonantWords()
    {
        int length = AskPositiveOrZeroInt("Длина удаляемых слов: ");
        int removed = _text.RemoveWordsOfLengthStartingWithConsonant(length);
        Console.WriteLine($"Удалено слов: {removed}.");
        PrintCurrentText();
    }

    private static void ReplaceWordsInSentence()
    {
        if (_text.Sentences.Count == 0)
        {
            Console.WriteLine("Сначала загрузите или введите текст.");
            return;
        }

        int sentenceNumber = AskInt("Номер предложения (начиная с 1): ");
        int length = AskPositiveOrZeroInt("Длина заменяемых слов: ");
        string replacement = Ask("Строка-замена (пустая строка удалит эти слова): ");

        try
        {
            int changed = _text.ReplaceWordsOfLengthInSentence(sentenceNumber, length, replacement);
            Console.WriteLine($"Заменено/удалено слов: {changed}.");
            PrintCurrentText();
        }
        catch (ArgumentOutOfRangeException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    private static void RemoveStopWords()
    {
        string englishPath = FindDataFile("stopwords_en.txt");
        string russianPath = FindDataFile("stopwords_ru(1).txt");
        var existingPaths = new[] { englishPath, russianPath }.Where(File.Exists).ToList();

        if (existingPaths.Count == 0)
        {
            Console.WriteLine("Файлы стоп-слов не найдены. Поместите stopwords_en.txt и stopwords_ru(1).txt рядом с программой.");
            return;
        }

        HashSet<string> stopWords = StopWordLoader.LoadFromFiles(existingPaths);
        int removed = _text.RemoveStopWords(stopWords);
        Console.WriteLine($"Загружено стоп-слов: {stopWords.Count}. Удалено слов из текста: {removed}.");
        PrintCurrentText();
    }

    private static void ExportToXml()
    {
        string path = Ask("Имя XML-файла (по умолчанию text.xml): ");
        if (string.IsNullOrWhiteSpace(path))
        {
            path = "text.xml";
        }

        path = NormalizePath(path);
        try
        {
            XmlTextExporter.Export(_text, path);
            Console.WriteLine($"XML сохранён: {Path.GetFullPath(path)}");
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"Не удалось экспортировать XML: {exception.Message}");
        }
    }

    private static string Ask(string message)
    {
        Console.Write(message);
        return Console.ReadLine() ?? string.Empty;
    }

    private static int AskInt(string message)
    {
        while (true)
        {
            if (int.TryParse(Ask(message), out int value))
            {
                return value;
            }

            Console.WriteLine("Введите целое число.");
        }
    }

    private static int AskPositiveOrZeroInt(string message)
    {
        while (true)
        {
            int value = AskInt(message);
            if (value >= 0)
            {
                return value;
            }

            Console.WriteLine("Число не может быть отрицательным.");
        }
    }

    private static string FindDataFile(string fileName)
    {
        string besideExecutable = Path.Combine(AppContext.BaseDirectory, fileName);
        if (File.Exists(besideExecutable))
        {
            return besideExecutable;
        }

        return Path.Combine(Directory.GetCurrentDirectory(), fileName);
    }

    private static string NormalizePath(string path)
    {
        string result = path.Trim();
        if (result.Length >= 2 && result[0] == '"' && result[^1] == '"')
        {
            result = result[1..^1];
        }

        return result;
    }
}
