using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WI2
{
    public class WITools
    {
        public List<User> InitUsers(string[] data)
        {
            List<User> uList = new List<User>();
            int idCounter = 0;

            for (int i = 0; i < data.Length; i += 5)
            {
                User u = new User(idCounter,
                    data[i].Split(" ")[1],
                    data[i + 1].TrimStart("friends: ".ToCharArray()).TrimStart().Split("\t"),
                    data[i + 2].Split("summary: ")[1],
                    data[i + 3].Split("review: ")[1]);
                uList.Add(u);
                idCounter++;
            }

            return uList;
        }

        public void AssignCommunities(int numbOfCuts, List<User> uList)
        {
            List<double> EVs = new List<double>();
            foreach (User user in uList)
            {
                EVs.Add(user.GetEV());
            }

            List<int> cutIndexes = MaxCutsCommunity(numbOfCuts, EVs);

            List<User> sortedUserList = uList.OrderBy(x => x.GetEV()).ToList();

            for (int i = 0; i < cutIndexes.Count; i++)
            {
                List<User> usersInCommunity = sortedUserList.Take(cutIndexes[i]).ToList();
                foreach (User user in usersInCommunity)
                {
                    user.communityID = i;
                }
                if(i == cutIndexes.Count)
                {
                    List<User> usersInRemainingCommunity = sortedUserList.Skip(cutIndexes[i]).ToList();
                    foreach (User user in usersInRemainingCommunity)
                    {
                        user.communityID = i+1;
                    }
                    usersInRemainingCommunity.Clear();
                }
                usersInCommunity.Clear();
                sortedUserList.RemoveRange(0, cutIndexes[i]);
            }
        }

        public List<int> MaxCutsCommunity(int numOfCuts, List<double> eigenVectors)
        {
            List<int> indexOffsets = new List<int>();
            List<double> biggestValuesList = new List<double>();

            for (int i = 0; i < numOfCuts; i++)
            {
                double biggestValue = 0.0;
                int biggestIndex = 0;
                for (int j = 0; j < eigenVectors.Count - 1; j++)
                {
                    var currentDifference = Math.Abs(eigenVectors[j] - eigenVectors[j + 1]);
                    if (currentDifference > biggestValue && !biggestValuesList.Exists(x => x == currentDifference))
                    {
                        biggestValue = currentDifference;
                        biggestIndex = j + 1;
                    }
                }
                indexOffsets.Add(biggestIndex);
                biggestValuesList.Add(biggestValue);
            }

            indexOffsets.Sort();
            List<int> cutIndexesSorted = new List<int>();

            int offset = 0;
            for (int i = 0; i < indexOffsets.Count; i++)
            {
                if (i == 0)
                {
                    cutIndexesSorted.Add(indexOffsets[i]);
                    offset = indexOffsets[i];
                }
                else
                {
                    cutIndexesSorted.Add(indexOffsets[i] - offset);
                    offset += indexOffsets[i] - indexOffsets[i - 1];
                }
            }

            return cutIndexesSorted;
        }
                
        //Earlier implementation of MaxCutCommunity. Finds the communities, without assigning a community for each user.
        public List<List<double[]>> MaxCutCommunity(int numbOfCuts, List<double[]> sortedVectors)
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

        //Finds all the needed cut indexes for the earlier implementation of MaxCutCommunity described above.
        private List<KeyValuePair<double, int>> FindNCuts(int numbOfCuts, List<double[]> sortedVectors, Dictionary<double, int> biggestDifferences)
        {
            for (int i = 0; i < sortedVectors.Count - 1; i++)
            {
                var currentDifference = Math.Abs(sortedVectors[i][1] - sortedVectors[i + 1][1]);
                biggestDifferences.Add(currentDifference, i + 1);
            }
            var communityCuts = biggestDifferences.OrderByDescending(x => x.Key).ToList().Take(numbOfCuts).ToList();

            return communityCuts;
        }

        public double[][] GetEigenVectors(List<User> uList)
        {
            Matrix<double> diagonalMatrix;

            Matrix<double> associationMatrix = FillMatrix(uList, out diagonalMatrix);

            Matrix<double> laplacianMatrix = diagonalMatrix - associationMatrix;

            Evd<double> eigen = laplacianMatrix.Evd(Symmetricity.Symmetric);
            var eigenVectors = eigen.EigenVectors.ToRowArrays();

            return eigenVectors;
        }

        public Matrix<double> FillMatrix(List<User> userList, out Matrix<double> diagonalMatrix)
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
