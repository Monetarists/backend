namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{
    public class XivApiResult
    {
        public int Id;
        public string Name_de;
        public string Name_en;
        public string Name_fr;
        public string Name_ja;
        public int IconID;
        public int IsExpert;
        public int IsSpecializationRequired;
        public XivApiClassJob ClassJob = new XivApiClassJob();
        public XivApiItem ItemResult = new XivApiItem();
        public int AmountResult;
        public int AmountIngredient0;
        public int AmountIngredient1;
        public int AmountIngredient2;
        public int AmountIngredient3;
        public int AmountIngredient4;
        public int AmountIngredient5;
        public int AmountIngredient6;
        public int AmountIngredient7;
        public int AmountIngredient8;
        public int AmountIngredient9;
        public XivApiItem ItemIngredient0 = new XivApiItem();
        public XivApiItem ItemIngredient1 = new XivApiItem();
        public XivApiItem ItemIngredient2 = new XivApiItem();
        public XivApiItem ItemIngredient3 = new XivApiItem();
        public XivApiItem ItemIngredient4 = new XivApiItem();
        public XivApiItem ItemIngredient5 = new XivApiItem();
        public XivApiItem ItemIngredient6 = new XivApiItem();
        public XivApiItem ItemIngredient7 = new XivApiItem();
        public XivApiItem ItemIngredient8 = new XivApiItem();
        public XivApiItem ItemIngredient9 = new XivApiItem();

    }
}
