using MyFace.Models.Database;

namespace MyFace.Models.Response
{
    public class UserResponse
    {
        private readonly User _user;

        public UserResponse(User user)
        {
            _user = user;
        }

        public int Id => _user.Id;
        public string FirstName => _user.FirstName;
        public string LastName => _user.LastName;
        public string DisplayName => $"{FirstName} {LastName}";
        public string Username => _user.Username;
        public string Email => _user.Email;
        public string ProfileImageUrl => _user.ProfileImageUrl;
        public string CoverImageUrl => _user.CoverImageUrl;

        // public string Password => _user.Password;
        // public string Hashed_Password => _user.Hashed_Password;
        // public string Salt => _user.Salt;
        // //we need to add hashed password and salt here :)
    }
}