using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;


namespace WI2
{
    class Program
    {
        static void Main(string[] args)
        {
            Control.UseNativeMKL();
            Control.UseMultiThreading();

            string buyResult = AppDomain.CurrentDomain.BaseDirectory + "buyResult.txt";
            string path = AppDomain.CurrentDomain.BaseDirectory + @"friendships.reviews.txt";
            string data = System.IO.File.ReadAllText(path);
            string[] text = data.Split("\r\n");

            string pathResults = AppDomain.CurrentDomain.BaseDirectory + @"friendships.reviews.results.txt";
            string dataResults = File.ReadAllText(pathResults);
            string[] textResults = dataResults.Split("\r\n");

            WITools tools = new WITools();
            List<User> userList = tools.InitUsers(text);
            //List<double[]> sortedVectors = tools.GetEigenVectors(userList).OrderBy(x => x[1]).ToList();
            //List<List<double[]>> communities = tools.MaxCutCommunity(9, sortedVectors);
            var ev = tools.GetEigenVectors(userList);

            int index = 0;
            foreach (User user in userList)
            {
                user.SetEV(ev[index]);
                index++;
            }

            tools.AssignCommunities(9, userList);

            foreach (User user in userList)
            {
                Console.WriteLine(user.ToString() + " - IS IN COMMUNITY: " + user.communityID);
            }

            Classifier classifier = new Classifier();
            classifier.Classify(userList);
            Dictionary<User, string> userDic = classifier.WillUsersBuyProduct(userList);

            StreamWriter sw = new StreamWriter(buyResult);
            int i = 3;
            foreach (var entry in userDic)
            {
                string assertionValue = "";
                if (textResults[i].Split("purchase: ")[1] == "yes")
                {
                    if (userDic[entry.Key] == "Will buy product!")
                    {
                        assertionValue = $"{entry.Key} assertion is CORRECT";
                        Console.WriteLine($"{entry.Key} assertion is CORRECT");
                    }
                    else
                    {
                        assertionValue = $"{entry.Key} assertion is NOT CORRECT";
                        Console.WriteLine($"{entry.Key} assertion is NOT CORRECT");
                    }
                }
                else if (textResults[i].Split("purchase: ")[1] == "no")
                {
                    if (userDic[entry.Key] == "Will not buy product!")
                    {
                        assertionValue = $"{entry.Key} assertion is CORRECT";
                        Console.WriteLine($"{entry.Key} assertion is CORRECT");
                    }
                    else
                    {
                        assertionValue = $"{entry.Key} assertion is NOT CORRECT";
                        Console.WriteLine($"{entry.Key} assertion is NOT CORRECT");
                    }
                }
                sw.WriteLine(entry.Key + " - " + entry.Key.communityID + " - " + userDic[entry.Key] + " - " + assertionValue);
                i = i + 8;
            }

            Console.ReadKey();
        }
    }
}