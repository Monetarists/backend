using Coravel.Events.Interfaces;
using XIVMarketBoard_Api.Controller;
using XIVMarketBoard_Api.Events;

namespace XIVMarketBoard_Api.Listeners
{
    public class SaveMarketBoardDataListener : IListener<SaveMarketBoardDataRequest>
    {
        private IMarketBoardController _marketBoardController;

        public SaveMarketBoardDataListener(IMarketBoardController marketBoardController)
        {
            _marketBoardController = marketBoardController;
        }

        public async Task HandleAsync(SaveMarketBoardDataRequest e)
        {
            var resultList = await _marketBoardController.GetOrCreateUniversalisQueries(e.UniversalisEntries);
        }
    }
}
