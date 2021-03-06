﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using SF.Snowball;
using SF.Snowball.Ext;
using ShellProgressBar;
using MathNet.Numerics.LinearAlgebra.Double;

namespace WI120917
{
    class Program
    {
        static List<Webpage> _pages = new List<Webpage>();

        static void Main(string[] args)
        {
            //File locations for JSON objects
            string indexFile = AppDomain.CurrentDomain.BaseDirectory + "index.txt";
            string crawlerDataFile = AppDomain.CurrentDomain.BaseDirectory + "crawler.txt";
            string rankFile = AppDomain.CurrentDomain.BaseDirectory + "rank.txt";

            //Reads any JSON object files if present, else crawls, indexes and ranks.
            if (!File.Exists(rankFile))
            {
                if (!File.Exists(indexFile))
                {
                    if (!File.Exists(crawlerDataFile))
                    {
                        Crawler crawler = new Crawler(new Uri("http://en.wikipedia.org"));
                        _pages = crawler.Crawl();
                        WriteToBinaryFile(crawlerDataFile, _pages);
                    }
                    else
                    {
                        _pages = ReadFromBinaryFile<List<Webpage>>(crawlerDataFile);
                    }
                    Tokenizer tokenizer = new Tokenizer();
                    tokenizer.Tokenize(_pages);
                    WriteToBinaryFile(indexFile, _pages);
                }
                else
                {
                    _pages = ReadFromBinaryFile<List<Webpage>>(indexFile);                    
                }
                PageRanker pageRanker = new PageRanker(_pages);
                DenseVector pageRank = pageRanker.GeneratePageRank(100);

                _pages.OrderBy(x => x.Id);
                for (int i = 0; i < pageRank.Count; i++)
                {
                    _pages[i].pageRank = pageRank[i];
                }

                WriteToBinaryFile(rankFile, _pages);
            }

            else
            {
                _pages = ReadFromBinaryFile<List<Webpage>>(rankFile);
            }


            //Performs our Rank Search for the input string, and prints the results.
            IndexAndRankSearch ranker = new IndexAndRankSearch();
            var rankResults = ranker.Rank("Nova Scotia", _pages);
            rankResults.Take(10).ToList().ForEach(x => Console.WriteLine(x.Key.Id + " - " + x.Value));

            Console.Read();
        }

        //Writes content to the destination file
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        //Reads content from the destination file
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        

        //static Dictionary<int, List<string>> BooleanSearch(string query)
        //{
        //    Dictionary<int, List<string>> res = new Dictionary<int, List<string>>();            

        //    string[] queryWords = query.Split(" ");

        //    queryWords = StringStemmer(queryWords);

        //    foreach (var word in queryWords)
        //    {
        //        if (index.ContainsKey(word))
        //        {
        //            foreach (var key in index[word].Keys)
        //            {
        //                if (!res.ContainsKey(key))
        //                {
        //                    List<string> words = new List<string>();
        //                    words.Add(word);
        //                    res.Add(key, words);
        //                } else
        //                {
        //                    res[key].Add(word);
        //                }
        //            }
        //        }
        //    }

        //    Dictionary<int, List<string>> tempRes = new Dictionary<int, List<string>>();

        //    foreach (var key in res.Keys)
        //    {
        //        int matches = 0;
        //        for(int i = 0; i < queryWords.Length; i++)
        //        {
        //            if (res[key].Contains(queryWords[i]))
        //            {
        //                matches++;
        //            }
        //        }
        //        if(matches == queryWords.Length)
        //        {
        //            tempRes.Add(key, res[key]);
        //        }
        //    }

        //    return tempRes;
        //}

        
        /*
        static public List<string> getRestrictions(string baseUrl)
        {
            List<string> result;
            if (robotTxts.TryGetValue(baseUrl, out result))
            {
                return result;
            }
            result = new List<string>();
            var robotTxt = new WebClient().DownloadString("http://" + baseUrl + "/robots.txt");

            var robotTxtLines = robotTxt.Split('\n');


            int i = 0;
            while (!robotTxtLines[i].ToLower().StartsWith("user-agent:") || robotTxtLines[i].Split(' ')[1] != "*") {
                if (i == robotTxtLines.Length - 1)
                {
                    robotTxts.Add(baseUrl, result);
                    return result;
                }

                i++;
            }
            i++;
            while (true)
            {
                if (i == robotTxtLines.Length)
                    break;
                string line = robotTxtLines[i].ToLower();
                if (line.StartsWith("disallow"))
                {
                    result.Add(line.Split(' ')[1]);
                }
                //cutting corners by not considering allows in robots.txt
                else if (!line.StartsWith("#") && !line.StartsWith("allow"))
                {
                    break;
                }

                i++;
            }
            robotTxts.Add(baseUrl, result);
            return result;
        }*/


    }
}
