using System;
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
        public double reviewScore;
        public int id;
        public double distance;
        public double weight;
        public int communityID;
        private double[] userEV;

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
            return this.username;
        }

        public string GetName()
        {
            return this.username;
        }

        public string GetReview()
        {
            return this.review;
        }

        public void SetEV(double[] EV)
        {
            this.userEV = EV;
        }

        public double GetEV()
        {
            return this.userEV[1];
        }
    }
}
