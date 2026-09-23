using System.Collections.Generic;
using System.IO;
using GeneticSearch.Models;

namespace GeneticSearch.Services
{
    public class GeneDataReporter
    {
        private const string Separator = "--------------------------------------------------------------------------";

        public void WriteHeader(TextWriter writer, string author)
        {
            writer.WriteLine(author);
            writer.WriteLine("Genetic Searching");
            writer.WriteLine(Separator);
        }

        public void WriteSearch(TextWriter writer, int index, string query, List<GeneticData> matches)
        {
            writer.WriteLine($"{index:D3}   search   {query}");
            writer.WriteLine("organism\t\t\t\tprotein");

            if (matches == null || matches.Count == 0)
            {
                writer.WriteLine("NOT FOUND");
            }
            else
            {
                foreach (var m in matches)
                {
                    writer.WriteLine($"{m.organism}\t\t{m.protein}");
                }
            }
            writer.WriteLine(Separator);
        }

        public void WriteDiff(TextWriter writer, int index, string p1Name, string p2Name, GeneticData? p1, GeneticData? p2, int? diffVal)
        {
            writer.WriteLine($"{index:D3}   diff   {p1Name}   {p2Name}");
            writer.WriteLine("amino-acids difference:");

            if (!p1.HasValue || !p2.HasValue)
            {
                var missing = new List<string>();
                if (!p1.HasValue) missing.Add(p1Name);
                if (!p2.HasValue) missing.Add(p2Name);

                writer.WriteLine($"MISSING: {string.Join(" ", missing)}");
            }
            else
            {
                writer.WriteLine(diffVal.Value);
            }
            writer.WriteLine(Separator);
        }

        public void WriteMode(TextWriter writer, int index, string pName, GeneticData? protein, (char aa, int count)? modeVal)
        {
            writer.WriteLine($"{index:D3}   mode   {pName}");
            writer.WriteLine("amino-acid occurs:");

            if (!protein.HasValue)
            {
                writer.WriteLine($"MISSING: {pName}");
            }
            else
            {
                writer.WriteLine($"{modeVal.Value.aa}\t\t{modeVal.Value.count}");
            }
            writer.WriteLine(Separator);
        }
    }
}