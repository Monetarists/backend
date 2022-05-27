using System.ComponentModel.DataAnnotations;
namespace XIVMarketBoard_Api.Entities
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
