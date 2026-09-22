using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSearch.Models;

namespace GeneticSearch.Services
{
    public class GeneticSearchEngine
    {
        public List<GeneticData> Search(List<GeneticData> dataset, string query)
        {
            var matches = new List<GeneticData>();
            foreach (var item in dataset)
            {
                if (item.amino_acids.Contains(query))
                {
                    matches.Add(item);
                }
            }
            return matches;
        }

        public int CalculateDiff(GeneticData p1, GeneticData p2)
        {
            string s1 = p1.amino_acids;
            string s2 = p2.amino_acids;

            int minLen = Math.Min(s1.Length, s2.Length);
            int diff = Math.Abs(s1.Length - s2.Length);

            for (int i = 0; i < minLen; i++)
            {
                if (s1[i] != s2[i])
                    diff++;
            }
            return diff;
        }

        public (char aa, int count) CalculateMode(GeneticData protein)
        {
            var counts = new Dictionary<char, int>();
            foreach (char c in protein.amino_acids)
            {
                if (counts.ContainsKey(c)) counts[c]++;
                else counts[c] = 1;
            }

            var top = counts
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .First();

            return (top.Key, top.Value);
        }
    }
}