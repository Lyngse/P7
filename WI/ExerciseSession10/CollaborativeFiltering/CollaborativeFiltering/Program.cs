using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace CollaborativeFiltering
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"u.data";
            string data = System.IO.File.ReadAllText(path);
            List<UserRating> userRatings = new List<UserRating>();
            List<UserRating> testUserRatings = new List<UserRating>();
            //Den gennemsnitlige rating en user giver
            List<double> avgUsersRating = new List<double>();
            //Den gennemsnitlige rating en movie har
            List<double> avgMovieRating = new List<double>();
            string[] text = data.Split("\n");

            for (int i = 0; i < text.Length; i++)
            {
                var rating = text[i].Split("\t");

                UserRating ur = new UserRating(Int32.Parse(rating[0]),
                    Int32.Parse(rating[1]),
                    Int32.Parse(rating[2]));
                if (i < 80000)
                    userRatings.Add(ur);
                else
                    testUserRatings.Add(ur);
            }

            Matrix<double> userMovieMatrix = Matrix<double>.Build.Dense(944, 1682);

            foreach (var ur in userRatings)
            {
                userMovieMatrix[ur.GetUserId(), ur.GetMovieId()] = ur.GetRating();
            }

            Console.WriteLine(userMovieMatrix);

            for (int i = 0; i < userMovieMatrix.RowCount; i++)
            {
                int movieCount = 0;
                double movieRating = 0;
                for (int j = 0; j < userMovieMatrix.ColumnCount; j++)
                {
                    if (userMovieMatrix[i, j] != 0)
                    {
                        movieRating += userMovieMatrix[i, j];
                        movieCount++;
                    }
                }
                avgMovieRating.Add(movieRating / movieCount);
            }

            for (int i = 0; i < userMovieMatrix.ColumnCount; i++)
            {
                int userCount = 0;
                double usersRating = 0;
                for (int j = 0; j < userMovieMatrix.RowCount; j++)
                {
                    if (userMovieMatrix[i, j] != 0)
                    {
                        usersRating += userMovieMatrix[i, j];
                        userCount++;
                    }
                }
                avgUsersRating.Add(usersRating / userCount);
            }

            Console.Read();
        }
    }
}
