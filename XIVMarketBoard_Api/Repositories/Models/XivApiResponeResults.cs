namespace XIVMarketBoard_Api.Repositories.Models
{
    public class XivApiResponeResults
    {
        public XivApiPagination Pagination { get; set; } = new XivApiPagination();
        public XivApiResult[] Results { get; set; } = new XivApiResult[] {};
    }
}
