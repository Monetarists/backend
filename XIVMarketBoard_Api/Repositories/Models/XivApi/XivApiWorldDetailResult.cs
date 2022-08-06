namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{
    public class XivApiWorldDetailResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Name_en { get; set; } = "";
        public bool InGame { get; set; }
        public XivApiDataCenterResult DataCenter { get; set; } = new XivApiDataCenterResult();
    }
}
