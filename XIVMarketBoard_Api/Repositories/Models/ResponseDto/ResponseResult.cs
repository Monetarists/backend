
namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseResult
    {
        public const string noItemsMessage = "The Api Returned No Items";
        public string? message { get; set; }
        public IEnumerable<ResponseItem>? Items { get; set; }
        public IEnumerable<ResponseRecipe>? Recipes { get; set; }
        public IEnumerable<ResponseUniversalisEntry>? UniversalisEntries { get; set; }
    }
}
