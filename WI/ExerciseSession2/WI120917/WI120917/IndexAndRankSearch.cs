﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SF.Snowball;
using SF.Snowball.Ext;

namespace WI120917
{
    //Corners cut: Using Snowball Stemmer to stem the words of the query.
    //RankSearch is used to obtain the final rank of a search query, combined with the pageRank.
    class IndexAndRankSearch
    {
        public List<KeyValuePair<Webpage, double>> Rank(string query, List<Webpage> pages)
        {
            query = query.ToLower();
            string[] queryWords = query.Split(" ");
            queryWords = StringStemmer(queryWords);
            List<double> queryVector = new List<double>();

            // Contains the webpage and its document score
            Dictionary<Webpage, double> results = new Dictionary<Webpage, double>();

            // Contains the unique query words and the frequency in the query
            Dictionary<string, int> termFrequency = new Dictionary<string, int>();

            // Checks how many times each word appears in the query
            foreach (var qw in queryWords)
            {
                if (!termFrequency.ContainsKey(qw))
                {
                    termFrequency.Add(qw, 1);
                }
                else
                {
                    termFrequency[qw]++;
                }
            }

            // Indexing
            foreach (var term in termFrequency)
            {
                if (!pages.Exists(x => x.HasToken(term.Key)))
                {
                    queryVector.Add(0);
                }
                else
                {
                    double weightedTermFrequency = 1 + Math.Log10(term.Value);
                    int documentFreq = pages.FindAll(x => x.HasToken(term.Key)).Count;
                    double iDocFrequency = Math.Log10(pages.Count / (double)documentFreq);
                    double wt = weightedTermFrequency * iDocFrequency;
                    queryVector.Add(wt);
                }
            }

            // Normalizes the index
            double queryVectorLength = Math.Sqrt(queryVector.Sum(wt => wt = Math.Pow(wt, 2)));
            for (int i = 0; i < queryVector.Count; i++)
            {
                queryVector[i] /= queryVectorLength;
            }

            foreach (var page in pages)
            {
                List<double> documentVector = new List<double>();

                foreach (var term in termFrequency)
                {
                    if (page.HasToken(term.Key))
                    {
                        documentVector.Add(1 + Math.Log10(page.GetTokenFrequency(term.Key)));
                    }
                    else
                    {
                        documentVector.Add(0);
                    }
                }
                double documentVectorLength = Math.Sqrt(documentVector.Sum(wt => wt = Math.Pow(wt, 2)));

                if (documentVectorLength != 0.0)
                {
                    for (int i = 0; i < documentVector.Count; i++)
                    {
                        documentVector[i] /= documentVectorLength;
                    }
                }

                double documentScore = 0;

                for (int i = 0; i < documentVector.Count; i++)
                {
                    documentScore += documentVector[i] * queryVector[i];
                }

                documentScore *= page.pageRank;
                results.Add(page, documentScore);
            }

            return results.OrderByDescending(x => x.Value).ToList();
        }

        private string[] StringStemmer(string[] stringArray)
        {
            var stemmer = new EnglishStemmer();
            for (int i = 0; i < stringArray.Length; i++)
            {
                stemmer.SetCurrent(stringArray[i]);
                if (stemmer.Stem())
                {
                    stringArray[i] = stemmer.GetCurrent();
                }
            }

            return stringArray;
        }
    }
}
