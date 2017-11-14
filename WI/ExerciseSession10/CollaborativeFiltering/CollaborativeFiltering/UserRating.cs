using System;
using System.Collections.Generic;
using System.Text;

namespace CollaborativeFiltering
{
    public class UserRating
    {
        private int _userId;
        private int _movieId;
        private int _rating;

        public UserRating(int userId, int movieId, int rating)
        {
            _userId = userId;
            _movieId = movieId;
            _rating = rating;
        }

        public int GetUserId()
        {
            return _userId;
        }

        public int GetMovieId()
        {
            return _movieId;
        }

        public int GetRating()
        {
            return _rating;
        }
    }
}
