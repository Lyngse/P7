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

            string path = AppDomain.CurrentDomain.BaseDirectory + @"friendships.reviews.txt";
            string data = System.IO.File.ReadAllText(path);
            string[] text = data.Split("\r\n");

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
                Console.WriteLine(user.ToString() + " - IS IN COMMUNITY: " + user.GetCommunity());
            }
            Console.ReadKey();

            Classifier classifier = new Classifier();
            classifier.Classify(userList);
            Dictionary<User, string> userDic = classifier.WillUsersBuyProduct(userList);

            Console.ReadKey();
        }
    }
}
