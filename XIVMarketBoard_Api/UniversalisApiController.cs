using XIVMarketBoard_Api.Entities;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using XIVMarketBoard_Api.Data;
using System;
using Microsoft.EntityFrameworkCore;
namespace XIVMarketBoard_Api
{
    public class UniversalisApiController
    {
        public static List<MbPost> getPostsForItem()
        {
            List<MbPost> list = new List<MbPost>();
            using (var xivContext = new XivDbContext())
            {
                // Perform data access using the context
            }
            return list;
        }
    }
}
