namespace XIVMarketBoard_Api.Repositories.Models.Universalis;
public class UniversalisResponse
{
    public IEnumerable<string> itemIDs { get; set; } = Array.Empty<string>();
    public IEnumerable<UniversalisResponseItems> items { get; set; } = Array.Empty<UniversalisResponseItems>();
    public string worldID { get; set; } = "";
    public string WorldName { get; set; } = "";
}
