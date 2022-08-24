using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_qlddata.Entity.request
{
    public class GetJobDataByIntervalOfDateRequest
    {
        [Required]
        [MinLength(1)]
        public List<string> JobNo { get; set; }

        [Required]
        public string DateStart { get; set; }

        [Required]
        public string DateEnd { get; set; }

        public GetJobDataByIntervalOfDateRequest()
        {
        }

    }
}

