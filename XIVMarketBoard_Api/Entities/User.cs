using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIVMarketBoard_Api.Entities
{
    public class User
    {
        [Key]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? CharacterName { get; set; }
        public virtual List<Job>? Jobs { get; set; }
    }
}

