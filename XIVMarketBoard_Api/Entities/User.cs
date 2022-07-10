using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIVMarketBoard_Api.Entities
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? CharacterName { get; set; }
        public virtual ICollection<Job>? Jobs { get; set; }
        public bool ApiAdmin { get; set; } = false;
        public bool WebAdmin { get; set; } = false;
        [JsonIgnore]
        public string PasswordHash { get; set; }
    }
}

 