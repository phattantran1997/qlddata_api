using System.Collections.Generic;
using API_premierductsqld.Service;
using API_qlddata.Entity.request;
using DTO_PremierDucts.EntityResponse;
using Microsoft.AspNetCore.Mvc;

namespace API_qlddata.Controllers
{
    [ApiController]
    [Route("reportQLD")]
    public class ReportQLDController : ControllerBase
    {
        private ReportService reportService;
        public ReportQLDController()
        {

            reportService = new ReportService();
        }

        [HttpGet("jobno")]
        public void reportForEachJobNo()
        {
            reportService.reportForEachJobNo();

        }

        [HttpPost("dispatch/info/box")]
        public List<DispatchInforResponse> getDispatchInforByListBoxes(List<BoxeseRequest> box)
        {
            return reportService.getDispatchInforByListBoxes(box);

        }

        [HttpPost("dispatch/info/jobno")]
        public List<DispatchInforResponse> getDispatchInforByListJobno(List<string> jobno)
        {
            return reportService.getDispatchInforByListJobno(jobno);

        }
    }
}
