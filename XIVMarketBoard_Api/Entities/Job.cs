using System.ComponentModel.DataAnnotations;
namespace XIVMarketBoard_Api.Entities
{
    public class Job
    {
        [Key]
        public string JobName { get; set; }

    }
}
