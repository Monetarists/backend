﻿namespace XIVMarketBoard_Api.Repositories.Models
{
    public class XivApiWorldDetailResult
    {
        public int Id;
        public string Name = "";
        public string Name_en = "";
        public bool InGame;
        public XivApiDataCenterResult DataCenter = new XivApiDataCenterResult();
    }
}
