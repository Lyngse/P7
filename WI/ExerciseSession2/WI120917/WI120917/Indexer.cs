using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace WI120917
{
    class Indexer
    {
        public void Tokenize(List<Webpage> htmlPages)
        {

            foreach (Webpage htmlPage in htmlPages)
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlPage.htmlContent);
                htmlDocument = RemoveScriptsAndComments(htmlDocument);

                string cleanText = htmlDocument.DocumentNode.SelectSingleNode("/html/body").InnerText;
                cleanText = Regex.Replace(cleanText, @"\t+\n+\r+\s+", " ");

                //cleanText = HttpUtility.HtmlDecode(cleanText);

                string tagCleanedText = Regex.Replace(cleanText, @"<[^>]*>", "");
                tagCleanedText = tagCleanedText.Replace("-", " ").Replace("_", " ");

                /*tagCleanedText = tagCleanedText.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ").Replace(":", "")
                    .Replace("'", "").Replace(".", "").Replace(",", "").Replace(";", "").Replace("-", " ").Replace("_", " ").Replace("/","");*/
                //tagCleanedText = Regex.Replace(tagCleanedText, @"[^a-z|A-Z|\s]", "");
                //tagCleanedText = Regex.Replace(tagCleanedText, @"[0-9]", "");
                //tagCleanedText = Regex.Replace(tagCleanedText, @"\s+", " ");
                tagCleanedText = tagCleanedText.ToLower();

                string[] tokens = tagCleanedText.Split(" ");

                for (int i = 0; i < tokens.Length; i++)
                {
                    htmlPage.AddToTokenList(tokens[i]);
                }
                Console.WriteLine(htmlPage.uri.ToString());
            }
        }
        private HtmlDocument RemoveScriptsAndComments(HtmlDocument webDocument)
        {
            try
            {
                foreach (HtmlNode script in webDocument.DocumentNode.SelectNodes("//script"))
                {
                    script.Remove();
                }
                foreach (HtmlNode comment in webDocument.DocumentNode.SelectNodes("//comment()"))
                {
                    comment.Remove();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("RemoveScriptsAndComments failed.... ");
            }

            return webDocument;
        }
    }
}
