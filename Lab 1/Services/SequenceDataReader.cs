using System;
using System.Collections.Generic;
using System.IO;
using GeneticSearch.Models;

namespace GeneticSearch.Services
{
    public class SequenceDataReader
    {
        private readonly IRleService _rleService;

        public SequenceDataReader(IRleService rleService)
        {
            _rleService = rleService;
        }

        public List<GeneticData> ReadSequences(string filePath)
        {
            var list = new List<GeneticData>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('\t');
                    if (parts.Length >= 3)
                    {
                        var data = new GeneticData
                        {
                            protein = parts[0].Trim(),
                            organism = parts[1].Trim(),
                            amino_acids = _rleService.Decode(parts[2].Trim())
                        };
                        list.Add(data);
                    }
                }
            }
            return list;
        }
    }
}