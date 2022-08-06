namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{

    public class XivApiClassJob
    {
        public string Abbreviation { get; set; } = "";
        public int ClassJobCategoryTargetID { get; set; }
        public int? DohDolJobIndex { get; set; }
        public int Id { get; set; }
        public string Icon { get; set; } = "";
        public string Name_de { get; set; } = "";
        public string Name_en { get; set; } = "";
        public string Name_fr { get; set; } = "";
        public string Name_ja { get; set; } = "";
    }

}
