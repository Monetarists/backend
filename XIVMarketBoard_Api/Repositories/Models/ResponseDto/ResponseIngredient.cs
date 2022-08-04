namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseIngredient
    {
        public int Amount { get; set; }
        public ResponseItem Item { get; set; } = new ResponseItem();
    }
}
