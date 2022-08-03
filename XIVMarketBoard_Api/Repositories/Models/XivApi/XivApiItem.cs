namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{
    public class XivApiItem
    {
        public int? ID;
        public string? Name_de = "";
        public string? Name_en = "";
        public string? Name_fr = "";
        public string? Name_ja = "";
        public int? IconID;
        public bool? CanBeHq;
        public int? IsUntradable;
        public XivApiItemSearchCategory? ItemSearchCategory;
        public XivApiItemUiCategory? ItemUICategory;

    }

}
