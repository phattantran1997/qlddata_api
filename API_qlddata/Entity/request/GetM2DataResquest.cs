﻿using System;
using System.Collections.Generic;

namespace API_qlddata.Entity.request
{
    public class GetM2DataResquest
    {
        public string station { get; set; }
        public List<string> jobNo { get; set; }
        public GetM2DataResquest()
        {
        }
    }
}
