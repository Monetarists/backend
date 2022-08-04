namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseJob
    {
        public int Id { get; set; }
        public string Name_en { get; set; } = "";
        public string Name_fr { get; set; } = "";
        public string Name_ja { get; set; } = "";
        public string Name_de { get; set; } = "";
        public string Abbreviation { get; set; } = "";
        public int? ClassJobCategoryTargetID { get; set; }
        public int? DohDolJobIndex { get; set; }

    }
}
