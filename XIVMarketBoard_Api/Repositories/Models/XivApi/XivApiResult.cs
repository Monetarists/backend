namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{
    public class XivApiResult
    {
        public int Id { get; set; }
        public string Name_de { get; set; }
        public string Name_en { get; set; }
        public string Name_fr { get; set; }
        public string Name_ja { get; set; }
        public int IconID { get; set; }
        public int IsExpert { get; set; }
        public int IsSpecializationRequired { get; set; }
        public XivApiClassJob ClassJob { get; set; } = new XivApiClassJob();
        public XivApiItem ItemResult { get; set; } = new XivApiItem();
        public int AmountResult { get; set; }
        public int AmountIngredient0 { get; set; }
        public int AmountIngredient1 { get; set; }
        public int AmountIngredient2 { get; set; }
        public int AmountIngredient3 { get; set; }
        public int AmountIngredient4 { get; set; }
        public int AmountIngredient5 { get; set; }
        public int AmountIngredient6 { get; set; }
        public int AmountIngredient7 { get; set; }
        public int AmountIngredient8 { get; set; }
        public int AmountIngredient9 { get; set; }
        public XivApiItem ItemIngredient0 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient1 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient2 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient3 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient4 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient5 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient6 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient7 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient8 { get; set; } = new XivApiItem();
        public XivApiItem ItemIngredient9 { get; set; } = new XivApiItem();

    }
}
