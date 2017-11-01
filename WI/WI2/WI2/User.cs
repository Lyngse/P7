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
        public int id;
        public double distance;
        public double weight;

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
    }
}
