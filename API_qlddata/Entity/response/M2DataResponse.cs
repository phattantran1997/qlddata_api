using System;
namespace API_qlddata.Entity.response
{
    public class M2DataResponse
    {
        public string jobNO { get; set; }
        public double meta_m2 { get; set; }
        public double isu_m2 { get; set; }
        public string pathID { get; set; }
        public M2DataResponse()
        {
        }

        public M2DataResponse(string jobNo, double meta_m2, double isu_m2,string pathID)
        {
            this.jobNO = jobNo;
            this.meta_m2 = meta_m2;
            this.isu_m2 = isu_m2;
            this.pathID = pathID;
        }
    }
}
