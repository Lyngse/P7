using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VaderSharp;

namespace WI2
{
    public class Classifier
    {
        public void Classify(List<User> userList)
        {
            foreach (User user in userList)
            {
                double score = 0.0;

                if (user.GetReview() != "*")
                {                    
                    SentimentIntensityAnalyzer sentimentAnalyzer = new SentimentIntensityAnalyzer();
                    string deltaReview = Regex.Replace(user.GetReview(), "<.*?>", "");

                    var results = sentimentAnalyzer.PolarityScores(deltaReview);

                    if (results.Compound >= 0.5)
                    {
                        if (results.Compound >= 0.75)
                            score = 5;
                        else
                            score = 4;
                    }
                    else if (results.Compound <= -0.5)
                    {
                        if (results.Compound <= -0.75)
                            score = 1;
                        else
                            score = 2;
                    }
                    else
                    {
                        score = 3.0;
                    }
                }
                user.reviewScore = score;
            }
        }

        public Dictionary<User, string> WillUsersBuyProduct(List<User> userList)
        {
            Dictionary<User, string> analyzedUsers = new Dictionary<User, string>();

            foreach (User user in userList)
            {
                if (user.GetReview() == "*")
                {
                    analyzedUsers.Add(user, "Will not buy product!!");

                    double averageFriendScore = 0.0;
                    foreach (User friend in user.friends)
                    {
                        if(friend.GetName() == "kyle")
                        {
                            averageFriendScore += friend.reviewScore * 10;
                        }
                        else if(friend.reviewScore != 0.0)
                        {
                            if (user.communityID != friend.communityID)
                            {
                                averageFriendScore += friend.reviewScore * 10;
                            }
                            else
                            {
                                averageFriendScore += friend.reviewScore;
                            }                            
                        }
                            
                    }
                    averageFriendScore = averageFriendScore / user.friends.Count;
                    if (averageFriendScore > 3.0)
                    {
                        analyzedUsers[user] = "Will buy product!!";
                    }
                }
            }

            return analyzedUsers;
        }

        public List<Feature> SentenceSentiment(string reviewSentence)
        {
            List<Feature> sentenceFeatures = new List<Feature>();

            string[] words = reviewSentence.Split(" ");
            bool isNegated = false;

            for (int i = 0; i < words.Length; i++)
            {
                if(isNegated == false)
                {
                    isNegated = SentimentValue(words[i]);
                }
                Feature feature = new Feature(words[i], isNegated);
                sentenceFeatures.Add(feature);
            }

            return sentenceFeatures;
        }

        public bool SentimentValue(string word)
        {
            bool sentimentValue = 
                word == "never" || 
                word == "no" ||
                word == "nothing" ||
                word == "nowhere" ||
                word == "noone" ||
                word == "none" ||
                word == "not" ||
                word == "havent" ||
                word == "hasnt" ||
                word == "cant" ||
                word == "couldnt" ||
                word == "shouldnt" ||
                word == "wont" ||
                word == "wouldnt" ||
                word == "dont" ||
                word == "doesnt" ||
                word == "didnt" ||
                word == "isnt" ||
                word == "arent" ||
                word == "aint"
                ? true : false;

            return sentimentValue;
        }
    }
}
