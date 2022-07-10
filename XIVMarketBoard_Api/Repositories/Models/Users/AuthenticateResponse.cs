using XIVMarketBoard_Api.Entities;

namespace XIVMarketBoard_Api.Repositories.Models.Users
{

    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool ApiAdmin { get; set; }
        public bool WebAdmin { get; set; }
        public string Token { get; set; }
    }
}