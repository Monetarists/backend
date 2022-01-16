namespace XIVMarketBoard_Backend.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string CharacterName { get; set; }
        public virtual List<Job> Jobs { get; set; }
        public virtual List<Job> PrefferedJobs { get; set; }



    }
}
