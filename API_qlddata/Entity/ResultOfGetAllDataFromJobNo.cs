using System;
namespace API_premierductsqld.Entities
{
    public class ResultOfGetAllDataFromJobNo
    {
        public string jobday { get; set; }
        public string jobno { get; set; }
        public string stationName { get; set; }
        public int stationNo { get; set; }
        public int id { get; set; }

        public ResultOfGetAllDataFromJobNo()
        {
        }
    }
}
