using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;

namespace XIVMarketBoard_Api.Controller
{
    public interface IDataCentreController
    {
        Task<World?> GetWorldFromName(string worldName);
        Task<IEnumerable<DataCenter>> SaveDataCenters(IEnumerable<DataCenter> dcList);
        Task<IEnumerable<World>> SaveWorlds(IEnumerable<World> worldList);
    }
    public class DataCentreController : IDataCentreController
    {
        private readonly XivDbContext _xivContext;
        public DataCentreController(XivDbContext xivContext)
        {
            _xivContext = xivContext;
        }
        public async Task<World?> GetWorldFromName(string worldName) => await _xivContext.Worlds.FirstOrDefaultAsync(r => r.Name == worldName);

        public async Task<IEnumerable<DataCenter>> SaveDataCenters(IEnumerable<DataCenter> dcList)
        {

            foreach (var dc in dcList)
            {
                var dcToSave = await _xivContext.DataCenters.FirstOrDefaultAsync(r => r.Name == dc.Name);
                if (dcToSave == null)
                {
                    _xivContext.DataCenters.Add(dc);
                }
            }

            await _xivContext.SaveChangesAsync();
            return dcList;

        }
        public async Task<IEnumerable<World>> SaveWorlds(IEnumerable<World> worldList)
        {

            foreach (var world in worldList)
            {
                var worldToSave = await _xivContext.Worlds.FirstOrDefaultAsync(r => r.Id == world.Id);
                var datacenter = await _xivContext.DataCenters.FirstOrDefaultAsync(r => r.Id == world.DataCenter.Id);
                if (worldToSave == null)
                {
                    if (datacenter != null)
                    {
                        world.DataCenter = datacenter;
                    }
                    _xivContext.Worlds.Add(world);
                }
            }
            await _xivContext.SaveChangesAsync();
            return worldList;

        }
    }
}
