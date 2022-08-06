namespace XIVMarketBoard_Api.Repositories.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public class AuthenticateRequest
    {
        public AuthenticateRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
