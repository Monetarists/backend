namespace XIVMarketBoard_Api.Repositories.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterRequest
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
