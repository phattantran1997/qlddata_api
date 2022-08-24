using System;
namespace API_premierductsqld.Entities.response
{
    public class GetLastestJobTimingsResponse
    {
        public string username { get; set; }
        public string name { get; set; }
        public string time { get; set; }
        public string jobNo { get; set; }
        public string itemNo { get; set; }
        public int stationNo { get; set; }
        public string stationName { get; set; }

    }

}
