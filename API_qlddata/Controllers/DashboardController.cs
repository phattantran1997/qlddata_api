using System;
using System.Collections.Generic;
using API_qlddata.Entity.request;
using API_qlddata.Entity.response;
using API_qlddata.Service;
using DTO_PremierDucts;
using Microsoft.AspNetCore.Mvc;

namespace API_qlddata.Controllers
{
    [ApiController]
    [Authorize]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {

        private DashboardService dashboardService;
        public DashboardController()
        {
            dashboardService = new DashboardService();
        }

        [HttpPost("total/all/m2/by-station")]
        public List<M2DataResponse> getAllM2DataWithoutStation([FromBody] GetM2DataResquest resquest)
        {
            return dashboardService.getM2DataWithStation(resquest);

        }

        [HttpPost("get/all/qldata")]
        public ResponseData getAllQLDdata([FromBody] List<string> jobno)
        {

            return dashboardService.getAllQLDdataByListJobno(jobno);
        }

        [HttpPost("total/all/m2")]
        public ResponseData getAllM2DataWithoutStation([FromBody] List<string> jobnoes)
        {

            return dashboardService.getM2DataWithoutStation(jobnoes);
        }
    }
}
