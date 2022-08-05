namespace XIVMarketBoard_Api.Repositories.Models.Users
{
    public class UpdateRequest
    {
        public UpdateRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
