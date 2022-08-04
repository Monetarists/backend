namespace XIVMarketBoard_Api.Repositories.Models.ResponseDto
{
    public class ResponseRecipe
    {
        public int Id { get; set; }
        public string Name_en { get; set; } = "";
        public string Name_de { get; set; } = "";
        public string Name_fr { get; set; } = "";
        public string Name_ja { get; set; } = "";
        public ResponseItem? Item { get; set; }
        public int AmountResult { get; set; }
        public virtual ICollection<ResponseIngredient>? Ingredients { get; set; }
        public virtual ResponseJob job { get; set; } = new ResponseJob();
        public bool IsExpert { get; set; }
        public bool IsSpecializationRequired { get; set; }
    }
}
