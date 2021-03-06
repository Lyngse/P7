﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WI2
{
    public class User
    {
        string username;
        string[] friendStringList;
        public List<User> friends = new List<User>();
        string summary;
        string review;
        private double reviewScore;
        public int id;
        public double distance;
        public double weight;
        private int communityID;

        public User(int _id, string uname, string[] friendsStrings, string summ, string rev)
        {
            this.id = _id;
            this.username = uname;
            this.friendStringList = friendsStrings;
            this.summary = summ;
            this.review = rev;
        }

        public void genFriends(List<User> uList)
        {
            for (int i = 0; i < friendStringList.Length; i++)
            {
                User u = uList.Find(x => x.username == friendStringList[i]);
                if(u != null)
                {
                    this.friends.Add(u);
                }                         
            }
        }

        public override string ToString()
        {
            string returnString = this.username + " has friends: " + this.friends.Count.ToString();
            return returnString;
        }

        public string GetName()
        {
            return this.username;
        }

        public string GetReview()
        {
            return this.review;
        }

        public double GetScore()
        {
            try
            {
                return this.reviewScore;
            }
            catch (Exception)
            {
                throw new Exception("GetScore threw an exception!");
            }
        }

        public void SetScore(double userScore)
        {
            this.reviewScore = userScore;
        }

        public void SetCommunity(int communityid)
        {
            this.communityID = communityid;
        }

        public int GetCommunity()
        {
            return this.communityID;
        }
    }
}
