using Coravel.Events.Interfaces;
using XIVMarketBoard_Api.Entities;
namespace XIVMarketBoard_Api.Events
{
    public class SaveMarketBoardDataRequest : IEvent
    {
        public List<UniversalisEntry> UniversalisEntries { get; set; } = new List<UniversalisEntry>();
    }
}
