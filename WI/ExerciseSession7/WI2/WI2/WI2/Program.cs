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

        public static List<List<double[]>> MaxCutCommunity(List<double[]> sortedVectors)
        {
            List<List<double[]>> communities = new List<List<double[]>>();
            List<double[]> firstCommunity = new List<double[]>();
            List<double[]> secondCommunity = new List<double[]>();

            var biggestIndex = 0;
            var biggestValue = 0.0;

            for (int i = 0; i < sortedVectors.Count; i++)
            {
                var currentDifference = Math.Abs(sortedVectors[i][1] - sortedVectors[i + 1][1]);
                if (currentDifference > biggestValue)
                {
                    biggestValue = currentDifference;
                    biggestIndex = i;
                }
            }

            firstCommunity = sortedVectors.Take(biggestIndex).ToList();
            secondCommunity = sortedVectors.Skip(biggestIndex).ToList();

            communities.Add(firstCommunity);
            communities.Add(secondCommunity);

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
