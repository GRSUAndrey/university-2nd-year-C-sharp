using System.Text;

namespace GeneticSearch.Services
{
    public class RleService : IRleService
    {
        public string Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded)) return string.Empty;

            var sb = new StringBuilder();
            for (int i = 0; i < encoded.Length; i++)
            {
                if (char.IsDigit(encoded[i]))
                {
                    int count = encoded[i] - '0';
                    char letter = encoded[i + 1];
                    sb.Append(letter, count);
                    i++;
                }
                else
                {
                    sb.Append(encoded[i]);
                }
            }
            return sb.ToString();
        }

        public string Encode(string plain)
        {
            if (string.IsNullOrEmpty(plain)) return string.Empty;

            var sb = new StringBuilder();
            for (int i = 0; i < plain.Length; i++)
            {
                char ch = plain[i];
                int count = 1;
                while (i + 1 < plain.Length && plain[i + 1] == ch)
                {
                    count++;
                    i++;
                }

                if (count > 2)
                    sb.Append(count).Append(ch);
                else
                    sb.Append(ch, count);
            }
            return sb.ToString();
        }
    }
}