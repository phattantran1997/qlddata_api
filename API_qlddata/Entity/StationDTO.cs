﻿using System;
namespace API_premierductsqld.Entities
{
    public class StationDTO
    {
        public string stationName { get; set; }
        public string deviceID { get; set; }
        public int stationGroup { get; set; }
        public string stationStatus { get; set; }
        public int stationNo { get; set; }
        public string dispatchStatus { get; set; }
        public int updateByJobNo { get; set; }

        public int updateByItemNo { get; set; }
    }
}
