namespace TextProcessingLab;

public static class TextParser
{
    // Parses English or Russian text while retaining whitespace and punctuation.
    public static Text Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new Text();
        var currentSentence = new Sentence();
        var pendingWhitespace = new System.Text.StringBuilder();
        int index = 0;

        while (index < source.Length)
        {
            char current = source[index];

            if (char.IsWhiteSpace(current))
            {
                pendingWhitespace.Append(current);
                index++;
                continue;
            }

            if (IsLetterOrDigitOrMark(current))
            {
                int start = index;
                index++;

                while (index < source.Length)
                {
                    char next = source[index];

                    if (IsLetterOrDigitOrMark(next))
                    {
                        index++;
                        continue;
                    }

                    // Keep apostrophes and hyphens inside words: don't, well-known.
                    if (IsWordJoiner(next) &&
                        index > start &&
                        index + 1 < source.Length &&
                        char.IsLetterOrDigit(source[index - 1]) &&
                        char.IsLetterOrDigit(source[index + 1]))
                    {
                        index++;
                        continue;
                    }

                    break;
                }

                string value = source[start..index];
                currentSentence.Tokens.Add(new Word(value, TakeWhitespace(pendingWhitespace)));
                continue;
            }

            currentSentence.Tokens.Add(
                new Punctuation(current.ToString(), TakeWhitespace(pendingWhitespace)));
            index++;

            if (!IsSentenceTerminator(source, index - 1))
            {
                continue;
            }

            // Keep closing quotes/brackets and a sequence such as "?!" in this sentence.
            while (index < source.Length &&
                   (IsTerminalPunctuation(source[index]) || IsClosingCharacter(source[index])))
            {
                currentSentence.Tokens.Add(
                    new Punctuation(source[index].ToString(), TakeWhitespace(pendingWhitespace)));
                index++;
            }

            if (currentSentence.Tokens.Count > 0)
            {
                result.Sentences.Add(currentSentence);
                currentSentence = new Sentence();
            }
        }

        if (currentSentence.Tokens.Count > 0)
        {
            result.Sentences.Add(currentSentence);
        }

        result.TrailingWhitespace = pendingWhitespace.ToString();
        return result;
    }

    private static string TakeWhitespace(System.Text.StringBuilder builder)
    {
        string value = builder.ToString();
        builder.Clear();
        return value;
    }

    private static bool IsLetterOrDigitOrMark(char character)
    {
        if (char.IsLetterOrDigit(character))
        {
            return true;
        }

        System.Globalization.UnicodeCategory category =
            System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character);

        return category is System.Globalization.UnicodeCategory.NonSpacingMark
            or System.Globalization.UnicodeCategory.SpacingCombiningMark
            or System.Globalization.UnicodeCategory.EnclosingMark;
    }

    private static bool IsWordJoiner(char character) => character is '\'' or '’' or '-';

    private static bool IsSentenceTerminator(string source, int index)
    {
        char character = source[index];
        if (!IsTerminalPunctuation(character))
        {
            return false;
        }

        // A period between digits is treated as a decimal point, not sentence ending.
        if (character == '.' && index > 0 && index + 1 < source.Length &&
            char.IsDigit(source[index - 1]) && char.IsDigit(source[index + 1]))
        {
            return false;
        }

        return true;
    }

    private static bool IsTerminalPunctuation(char character) => character is '.' or '!' or '?';

    private static bool IsClosingCharacter(char character) =>
        character is '"' or '\'' or '’' or '”' or '»' or ')' or ']' or '}' or '›';
}
