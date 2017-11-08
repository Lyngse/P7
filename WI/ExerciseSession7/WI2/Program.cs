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
            string path = AppDomain.CurrentDomain.BaseDirectory + @"friendships.txt";
            string data = System.IO.File.ReadAllText(path);
            List<User> userList = new List<User>();
            int idCounter = 0;

            StringBuilder builder1 = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();



            Control.UseNativeMKL();
            Control.UseMultiThreading();

            string[] text = data.Split("\r\n");

            for (int i = 0; i < text.Length; i += 5)
            {
                User u = new User(idCounter,
                    text[i].Split(" ")[1],
                    text[i + 1].TrimStart("friends: ".ToCharArray()).TrimStart().Split("\t"),
                    text[i + 2].Split("summary: ")[1],
                    text[i + 3].Split("review: ")[1]);
                userList.Add(u);
                idCounter++;
            }

            Matrix<double> diagonalMatrix;

            Matrix<double> associationMatrix = FillMatrix(userList, out diagonalMatrix);

            Matrix<double> laplacianMatrix = diagonalMatrix - associationMatrix;

            Evd<double> eigen = laplacianMatrix.Evd(Symmetricity.Symmetric);
            var eigenVectors = eigen.EigenVectors.ToRowArrays();
            var sortedVectors = eigenVectors.OrderBy(x => x[1]).ToList();

            List<List<double[]>> communities = MaxCutCommunity(9, sortedVectors);

            builder1.AppendLine(eigen.EigenVectors.ToMatrixString(userList.Count, userList.Count));
            builder2.AppendLine(eigen.EigenValues.ToVectorString(eigen.EigenValues.Count, Int32.MaxValue));

            for (int i = 0; i < sortedVectors.Count; i++)
            {
                builder3.AppendLine(sortedVectors[i][1].ToString(CultureInfo.InvariantCulture));
            }


            string destPath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EvdEigenVectors.txt");
            string destPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EvdEigenValues.txt");
            string destPath3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SortedValues.txt");


            File.WriteAllText(destPath1, builder1.ToString());
            File.WriteAllText(destPath2, builder2.ToString());
            File.WriteAllText(destPath3, builder3.ToString());

            Console.WriteLine(laplacianMatrix.ToString());

            Console.ReadKey();
        }

        public static List<List<double[]>> MaxCutCommunity(int numbOfCuts, List<double[]> sortedVectors)
        {
            List<List<double[]>> communities = new List<List<double[]>>();
            Dictionary<double, int> biggestDifferences = new Dictionary<double, int>();

            var communityCuts = FindNCuts(numbOfCuts, sortedVectors, biggestDifferences);
            communityCuts = communityCuts.OrderBy(x => x.Value).ToList();

            var toSkip = 0;
            var sortedVectorCopy = sortedVectors.ToList();

            for (int i = 0; i < communityCuts.Count; i++)
            {
                var currentCommunity = sortedVectorCopy.Take(communityCuts[i].Value - toSkip).ToList();
                sortedVectorCopy = sortedVectorCopy.Skip(communityCuts[i].Value - toSkip).ToList();

                toSkip = communityCuts[i].Value;
                communities.Add(currentCommunity);
            }

            communities.Add(sortedVectorCopy);

            return communities;
        }

        private static List<KeyValuePair<double, int>> FindNCuts(int numbOfCuts, List<double[]> sortedVectors, Dictionary<double, int> biggestDifferences)
        {
            for (int i = 0; i < sortedVectors.Count - 1; i++)
            {
                var currentDifference = Math.Abs(sortedVectors[i][1] - sortedVectors[i + 1][1]);
                biggestDifferences.Add(currentDifference, i + 1);
            }
            var communityCuts = biggestDifferences.OrderByDescending(x => x.Key).ToList().Take(numbOfCuts).ToList();

            return communityCuts;
        }

        public static Matrix<double> FillMatrix(List<User> userList, out Matrix<double> diagonalMatrix)
        {
            Matrix<double> associationMatrix = Matrix<double>.Build.Dense(userList.Count, userList.Count);
            Matrix<double> dMatrix = Matrix<double>.Build.DenseDiagonal(userList.Count, userList.Count);

            foreach (User u in userList)
            {
                u.genFriends(userList);
                foreach (User friend in u.friends)
                {
                    associationMatrix[u.id, friend.id] = 1;
                }
                dMatrix[u.id, u.id] = u.friends.Count;
            }

            diagonalMatrix = dMatrix;

            return associationMatrix;
        }
    }
}
