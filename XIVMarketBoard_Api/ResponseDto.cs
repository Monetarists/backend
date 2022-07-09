using XIVMarketBoard_Api.Entities;

namespace XIVMarketBoard_Api
{
    public class ResponseDto
    {
        public const string noItemsMessage = "The Api Returned No Items";
        public string? message { get; set; }
        public IEnumerable<Item>? Items { get; set; }
        public IEnumerable<Recipe>? Recipes { get; set; }
        public IEnumerable<UniversalisEntry>? UniversalisEntry { get; set; }
        public Dictionary<int, string>? dict { get; set; }


    }
}
