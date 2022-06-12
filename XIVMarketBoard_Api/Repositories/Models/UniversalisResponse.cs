namespace XIVMarketBoard_Api.Repositories.Models;
public class UniversalisResponse
{
    public IEnumerable<string> itemIDs { get; set; } = new string[] { };
    public IEnumerable<UniversalisResponseItems> items { get; set; } = new UniversalisResponseItems[] { };
    public string worldID { get; set; } = "";
    public string WorldName { get; set; } = "";
}
