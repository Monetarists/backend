namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseItem
    {
        public int Id { get; set; }
        public string Name_en { get; set; } = "";
        public string Name_de { get; set; } = "";
        public string Name_fr { get; set; } = "";
        public string Name_ja { get; set; } = "";
        public bool? CanBeCrafted { get; set; }
        public bool CanBeHq { get; set; }
        public bool? IsMarketable { get; set; }
    }
}
