using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeneticSearch.Models;
using GeneticSearch.Services;

namespace GeneticSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            string seqFile = args.Length > 0 ? args[0] : "sequences.0.txt";
            string cmdFile = args.Length > 1 ? args[1] : "commands.0.txt";
            string outFile = args.Length > 2 ? args[2] : "genedata.txt";
            string authorName = args.Length > 3 ? args[3] : "Dwight Barnette";

            if (!File.Exists(seqFile)) seqFile = "sequences.txt";
            if (!File.Exists(cmdFile)) cmdFile = "commands.txt";

            IRleService rle = new RleService();
            SequenceDataReader reader = new SequenceDataReader(rle);
            GeneticSearchEngine engine = new GeneticSearchEngine();
            GeneDataReporter reporter = new GeneDataReporter();

            try
            {
                List<GeneticData> dataset = reader.ReadSequences(seqFile);

                using (var cmdReader = new StreamReader(cmdFile))
                using (var outWriter = new StreamWriter(outFile, false, Encoding.UTF8))
                {
                    reporter.WriteHeader(outWriter, authorName);

                    int commandCounter = 1;
                    string line;

                    while ((line = cmdReader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] parts = line.Split('\t');
                        string cmd = parts[0].Trim();

                        if (cmd.Equals("search", StringComparison.OrdinalIgnoreCase))
                        {
                            string rawQuery = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                            string decodedQuery = rle.Decode(rawQuery);
                            var matches = engine.Search(dataset, decodedQuery);
                            reporter.WriteSearch(outWriter, commandCounter, decodedQuery, matches);
                        }
                        else if (cmd.Equals("diff", StringComparison.OrdinalIgnoreCase))
                        {
                            string p1Name = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                            string p2Name = parts.Length > 2 ? parts[2].Trim() : string.Empty;

                            int idx1 = dataset.FindIndex(p => p.protein.Equals(p1Name, StringComparison.OrdinalIgnoreCase));
                            int idx2 = dataset.FindIndex(p => p.protein.Equals(p2Name, StringComparison.OrdinalIgnoreCase));

                            GeneticData? p1 = idx1 >= 0 ? dataset[idx1] : (GeneticData?)null;
                            GeneticData? p2 = idx2 >= 0 ? dataset[idx2] : (GeneticData?)null;

                            int? diff = (p1.HasValue && p2.HasValue) ? engine.CalculateDiff(p1.Value, p2.Value) : (int?)null;
                            reporter.WriteDiff(outWriter, commandCounter, p1Name, p2Name, p1, p2, diff);
                        }
                        else if (cmd.Equals("mode", StringComparison.OrdinalIgnoreCase))
                        {
                            string pName = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                            int idx = dataset.FindIndex(p => p.protein.Equals(pName, StringComparison.OrdinalIgnoreCase));
                            GeneticData? p = idx >= 0 ? dataset[idx] : (GeneticData?)null;

                            var modeVal = p.HasValue ? engine.CalculateMode(p.Value) : ((char, int)?)null;
                            reporter.WriteMode(outWriter, commandCounter, pName, p, modeVal);
                        }

                        commandCounter++;
                    }
                }

                Console.WriteLine($"[SUCCESS] Обработка завершена. Результат в файле: {outFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Ошибка: {ex.Message}");
            }
        }
    }
}