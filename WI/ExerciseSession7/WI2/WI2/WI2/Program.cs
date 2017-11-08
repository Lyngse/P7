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

            Matrix<double> associationMatrix = fillMatrix(userList, out diagonalMatrix);

            Matrix<double> laplacianMatrix = diagonalMatrix - associationMatrix;

            Evd<double> eigen = laplacianMatrix.Evd(Symmetricity.Symmetric);
            var eigenVectors = eigen.EigenVectors.ToRowArrays();
            var sortedVectors = eigenVectors.OrderBy(x => x[1]).ToList();

            List<List<double[]>> communities = MaxCutCommunity(sortedVectors);

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

        public static List<int> MaxCutCommunity(int nrOfCuts, List<double[]> sortedVectors)
        {
            List<int> biggestIndexes = new List<int>();
            List<double> biggestValuesList = new List<double>();            

            for (int i = 0; i < nrOfCuts; i++)
            {
                double biggestValue = 0.0;
                for (int j = 0; j < sortedVectors.Count; j++)
                {
                    var currentDifference = Math.Abs(sortedVectors[j][1] - sortedVectors[j + 1][1]);
                    if (currentDifference > biggestValue && biggestValuesList.Exists(x => x == biggestValue))
                    {
                        biggestValue = currentDifference;
                        biggestValuesList.Add(biggestValue);
                        biggestIndexes.Add(j);
                    }
                }
            }

            return biggestIndexes;
        }

        public List<List<double[]>> FindCommunities(int numberOfCommunities, List<double[]> sortedVectors)
        {
            List<List<double[]>> communities = new List<List<double[]>>();
            List<int> cutIndexes = MaxCutCommunity(numberOfCommunities - 1, sortedVectors);
            List<double[]> deltaSortedVectors = sortedVectors;

            cutIndexes.OrderBy(x => x);

            for (int i = 0; i < numberOfCommunities; i++)
            {
                communities.Add(deltaSortedVectors.Take(cutIndexes[i]).ToList());

                if(i == numberOfCommunities - 1)
                {
                    communities.Add(deltaSortedVectors.Skip(cutIndexes[i]).ToList());
                }

                deltaSortedVectors = sortedVectors.Skip(cutIndexes[i]).ToList();
            }

            return communities;
        }

        public static Matrix<double> fillMatrix(List<User> userList, out Matrix<double> diagonalMatrix)
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
