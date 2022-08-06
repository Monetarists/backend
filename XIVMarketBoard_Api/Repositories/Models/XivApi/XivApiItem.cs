namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{
    public class XivApiItem
    {
        public int? ID { get; set; }
        public string? Name_de { get; set; } = "";
        public string? Name_en { get; set; } = "";
        public string? Name_fr { get; set; } = "";
        public string? Name_ja { get; set; } = "";
        public int? IconID { get; set; }
        public bool? CanBeHq { get; set; }
        public int? IsUntradable { get; set; }
        public XivApiItemSearchCategory? ItemSearchCategory { get; set; }
        public XivApiItemUiCategory? ItemUICategory { get; set; }

    }

}
