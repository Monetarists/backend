namespace XIVMarketBoard_Api.Repositories.Models.XivApi
{
    public class XivApiResponeResults
    {
        public XivApiPagination Pagination { get; set; } = new XivApiPagination();
        public XivApiResult[] Results { get; set; } = Array.Empty<XivApiResult>();
    }
}
