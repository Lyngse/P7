using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SF.Snowball;
using SF.Snowball.Ext;

namespace WI120917
{
    class RankSearch
    {
        //public void RankSearch(string query)
        //{
        //    //List<string> files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\docs\").ToList();
        //    //int docCount = files.Count;
        //    //List<int> documentIds = new List<int>();
        //    //foreach (var file in files)
        //    //{
        //    //    int fileID = int.Parse(file.Split("doc").Last().Replace(".html", ""));
        //    //    documentIds.Add(fileID);
        //    //}

        //    string[] queryWords = query.Split(" ");
        //    queryWords = StringStemmer(queryWords);
        //    List<double> queryVector = new List<double>();
        //    Dictionary<int, double> results = new Dictionary<int, double>();
        //    Dictionary<string, int> termFrequency = new Dictionary<string, int>();

        //    foreach (var qw in queryWords)
        //    {
        //        if (!termFrequency.ContainsKey(qw))
        //        {
        //            termFrequency.Add(qw, 1);
        //        }
        //        else
        //        {
        //            termFrequency[qw]++;
        //        }
        //    }

        //    foreach (var tf in termFrequency)
        //    {
        //        foreach (var page in pages)
        //        {
                    
        //        }

        //        if (!.ContainsKey(tf.Key))
        //        {
        //            queryVector.Add(0);
        //            continue;
        //        }

        //        double weightedTermFrequency = 1 + Math.Log10(tf.Value);
        //        int docFrequency = index[tf.Key].Count;
        //        double iDocFrequency = Math.Log10(docCount / (double)docFrequency);
        //        double wt = weightedTermFrequency * iDocFrequency;

        //        queryVector.Add(wt);
        //    }
        //    double queryVectorLength = Math.Sqrt(queryVector.Sum(wt => wt = Math.Pow(wt, 2)));
        //    for (int i = 0; i < queryVector.Count; i++)
        //    {
        //        queryVector[i] /= queryVectorLength;
        //    }

        //    foreach (var id in documentIds)
        //    {
        //        List<double> documentVector = new List<double>();

        //        foreach (var term in termFrequency)
        //        {
        //            if (index.ContainsKey(term.Key))
        //            {
        //                if (index[term.Key].ContainsKey(id))
        //                {
        //                    documentVector.Add(1 + Math.Log10(index[term.Key][id].Count));
        //                }
        //                else
        //                {
        //                    documentVector.Add(0);
        //                }
        //            }
        //            else
        //            {
        //                documentVector.Add(0);
        //            }
        //        }
        //        double documentVectorLength = Math.Sqrt(documentVector.Sum(wt => wt = Math.Pow(wt, 2)));
        //        if (documentVectorLength != 0.0)
        //        {
        //            for (int i = 0; i < documentVector.Count; i++)
        //            {
        //                documentVector[i] /= documentVectorLength;
        //            }
        //        }

        //        double documentScore = 0;

        //        for (int i = 0; i < documentVector.Count; i++)
        //        {
        //            documentScore += documentVector[i] * queryVector[i];
        //        }
        //        results.Add(id, documentScore);
        //    }
        //    Console.WriteLine("Are we done yet? no");

        //    var sortedResults = results.OrderByDescending(x => x.Value);
        //    sortedResults.Take(10).ToList().ForEach(x => Console.WriteLine(x.Key + " - " + x.Value));
        //}

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
