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
    class ProgramNaiveBayes
    {

        public static int NumberOfTrainingCases;

        static void MainNaiveBayes(string[] args)
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

            //Matrix<double> diagonalMatrix;

            //Matrix<double> associationMatrix = FillMatrix(userList, out diagonalMatrix);

            //Matrix<double> laplacianMatrix = diagonalMatrix - associationMatrix;

            //Evd<double> eigen = laplacianMatrix.Evd(Symmetricity.Symmetric);
            //var eigenVectors = eigen.EigenVectors.ToRowArrays();
            //var sortedVectors = eigenVectors.OrderBy(x => x[1]).ToList();

            //List<List<double[]>> communities = MaxCutCommunity(9, sortedVectors);

            //builder1.AppendLine(eigen.EigenVectors.ToMatrixString(userList.Count, userList.Count));
            //builder2.AppendLine(eigen.EigenValues.ToVectorString(eigen.EigenValues.Count, Int32.MaxValue));

            //for (int i = 0; i < sortedVectors.Count; i++)
            //{
            //    builder3.AppendLine(sortedVectors[i][1].ToString(CultureInfo.InvariantCulture));
            //}


            //string destPath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EvdEigenVectors.txt");
            //string destPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EvdEigenValues.txt");
            //string destPath3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SortedValues.txt");


            //File.WriteAllText(destPath1, builder1.ToString());
            //File.WriteAllText(destPath2, builder2.ToString());
            //File.WriteAllText(destPath3, builder3.ToString());

            string path2 = AppDomain.CurrentDomain.BaseDirectory + @"SentimentTrainingData.txt";
            string[] dataSet = System.IO.File.ReadAllLines(path2);
            List<TrainingCase> trainingCases = new List<TrainingCase>();
            string destPath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KnownTrainingWords.txt");
            StringBuilder builder = new StringBuilder();
            List<Catagory> catagories = new List<Catagory>();

            for (int i = 4; i < dataSet.Length; i += 9)
            {
                var test = dataSet[i].Split("review/score: ");
                int reviewScore = (int)double.Parse(dataSet[i].Split("review/score: ")[1], CultureInfo.InvariantCulture);
                string review = dataSet[i + 3].Split("review/text: ")[1];
                TrainingCase newcase = new TrainingCase(reviewScore, review);
                trainingCases.Add(newcase);
            }

            NumberOfTrainingCases = trainingCases.Count;

            List<string> knownWords = new List<string>();

            if (File.Exists(destPath1))
            {
                StreamReader reader = new StreamReader(destPath1);
                knownWords = reader.ReadToEnd().Split(',').Distinct().ToList();
                knownWords.RemoveAll(x => x == "");
            }
            else
            {
                FindUniqueWords(trainingCases, knownWords, destPath1);
            }

            for (int i = 1; i < 6; i++)
            {
                Catagory newCatagory = new Catagory(i, knownWords);
                catagories.Add(newCatagory);
            }

            foreach (var trainingcase in trainingCases)
            {
                catagories.First(x => x.Score == trainingcase.Score).NumOfTrainingCases++;

                foreach (var word in trainingcase.Review)
                {
                    switch (trainingcase.Score)
                    {
                        case 1:
                            if (catagories[0].Occurences.ContainsKey(word))
                            {
                                catagories[0].Occurences[word]++;
                            }
                            break;
                        case 2:
                            if (catagories[1].Occurences.ContainsKey(word))
                            {
                                catagories[1].Occurences[word]++;
                            }
                            break;
                        case 3:
                            if (catagories[2].Occurences.ContainsKey(word))
                            {
                                catagories[2].Occurences[word]++;
                            }
                            break;
                        case 4:
                            if (catagories[3].Occurences.ContainsKey(word))
                            {
                                catagories[3].Occurences[word]++;
                            }
                            break;
                        case 5:
                            if (catagories[4].Occurences.ContainsKey(word))
                            {
                                catagories[4].Occurences[word]++;
                            }
                            break;
                        default:
                            throw new IndexOutOfRangeException("Unkown score: " + trainingcase.Score);
                            break;
                    }
                }
            }

            var result = Classify(
                "I bought this almost a month ago and my dog a small Corgi mix still chews on it every day. He seems to like antlers more than rawhides and I like that they are much healthier for him. I would definitely recommend antlers",
                catagories);

            Console.ReadKey();
        }

        public static int Classify(string reviewString, List<Catagory> catagories)
        {
            List<string> reviewfeatures = reviewString.Split(' ').ToList();
            
            Dictionary<int, decimal> catagoryEvidence = new Dictionary<int, decimal>();
            Dictionary<int, decimal> nastyEvidence = new Dictionary<int, decimal>();


            for (int i = 1; i < 6; i++)
            {
                catagoryEvidence.Add(i, 1);
                nastyEvidence.Add(i, 1);
            }

            foreach (var catagory in catagoryEvidence)
            {
                decimal likelyhood = 1;
                decimal evidence = 1;
                decimal prior = catagories[catagory.Key-1].NumOfTrainingCases / (decimal)NumberOfTrainingCases;

                foreach (var feature in reviewfeatures)
                {
                    int numberOfTotalOccurences = 0;

                    var newLikelyhood = catagories[catagory.Key - 1].Occurences[feature] /
                                  (decimal)catagories[catagory.Key - 1].NumOfTrainingCases;
                    if (newLikelyhood == 0)
                    {
                        newLikelyhood = 1;
                    }
                    likelyhood *= newLikelyhood;

                    foreach (var specifiCatagory in catagories)
                    {
                        numberOfTotalOccurences += specifiCatagory.Occurences[feature];
                    }

                    var newEvidence = numberOfTotalOccurences / (decimal)NumberOfTrainingCases;

                    if (newEvidence == 0)
                    {
                        newEvidence = 1;
                    }

                    evidence *= newEvidence;
                }

                nastyEvidence[catagory.Key] = (likelyhood * prior) / evidence;
            }

            var comul = nastyEvidence.Values.Sum();
            return catagoryEvidence.OrderByDescending(x => x.Value).First().Key;
        }

        private static void FindUniqueWords(List<TrainingCase> trainingCases, List<string> knownWords, string destPath)
        {
            var count = 0;

            foreach (var trainingcase in trainingCases)
            {
                foreach (var word in trainingcase.Review)
                {
                    if (!knownWords.Contains(word))
                    {
                        knownWords.Add(word);
                    }
                }

                if (count % 1000 == 0)
                {
                    Console.WriteLine(count + " out of " + trainingCases.Count);
                }
                count++;
            }

            StreamWriter writer = new StreamWriter(destPath);
            foreach (var knownWord in knownWords)
            {
                writer.Write(knownWord + " ");
            }
        }

        public void TrainDataSet(List<TrainingCase> trainingCases)
        {

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
