using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using SF.Snowball.Ext;

namespace WI120917
{
    //Corners cut: Using Snowball Stemmer to stem any token.
    //Corners cut: Replacing all - and _ with a whitespace, which can therefore not be recognized as a query in our search.
    //Corners cut: Using HtmlAgilityPack to load the html content of a page.
    //Corners cut: Using HtmlAgilityPack to remove any unwanted tags from the content.
    class Indexer
    {
        //Tokenize will add tokens to each webpage one after another.
        public void Tokenize(List<Webpage> htmlPages)
        {

            foreach (Webpage htmlPage in htmlPages)
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlPage.htmlContent);
                htmlDocument = RemoveScriptsAndComments(htmlDocument);

                string cleanText = htmlDocument.DocumentNode.SelectSingleNode("/html/body").InnerText;
                cleanText = Regex.Replace(cleanText, @"\t+\n+\r+\s+", " ");
                string tagCleanedText = Regex.Replace(cleanText, @"<[^>]*>", "");
                tagCleanedText = tagCleanedText.Replace("-", " ").Replace("_", " ");
                tagCleanedText = tagCleanedText.ToLower();

                string[] tokens = tagCleanedText.Split(" ");
                tokens = StringStemmer(tokens);

                for (int i = 0; i < tokens.Length; i++)
                {
                    htmlPage.AddToTokenList( tokens[i]);
                }
                Console.WriteLine(htmlPage.uri.ToString());
            }
        }

        //Method to remove any script or comment node as defined by the HtmlAgilityPack.
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

        //Snowball Stemmer. Stems an input string array according to the English language.
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
