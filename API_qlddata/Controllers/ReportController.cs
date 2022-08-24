using System;
using API_premierductsqld.Service;
using API_qlddata.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace API_qlddata.Controllers
{
    [ApiController]
    [Route("report")]
    public class ReportController : ControllerBase
    {
        private ReportStationService reportStationService;
        public ReportController(QLDdatacontext qLDdatacontext)
        {

            reportStationService = new ReportStationService(qLDdatacontext);
        }

        [HttpGet("jobno")]
        public void reportForEachJobNo()
        {
            reportStationService.reportForEachJobNo();

        }
    }
}
