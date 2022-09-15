using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API_premierductsqld.Entities;
using API_premierductsqld.Entities.request;
using API_premierductsqld.Entities.response;
using API_qlddata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API_premierductsqld.Service
{
    public class JobTimingService
    {

        static System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
       
        public static List<JobTimingResponse> getAllDataWithStation(string urlFromBase,string date)
        {

            try
            {
                string url = urlFromBase + "/jobtiming/getAllDataWithStation?currentDate=" + date;
                //Sends request to retrieve data from the web service for the specified Uri
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync(); //Returns the response as JSON string
                    List<JobTimingResponse> jobTimings = JsonConvert.DeserializeObject<List<JobTimingResponse>>(content.Result); //Converts JSON string to dynamic
                    return jobTimings;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
            }

            return null;


        }

        public static List<JobTimingResponse> getAllDataWithoutStation(string urlFromBase,string date)
        {

            try
            {

                string url = urlFromBase + "/jobtiming/getAllDataWithoutStation?currentDate=" + date;
                //Sends request to retrieve data from the web service for the specified Uri
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync(); //Returns the response as JSON string
                    List<JobTimingResponse> jobTimings = JsonConvert.DeserializeObject<List<JobTimingResponse>>(content.Result); //Converts JSON string to dynamic
                    return jobTimings;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
            }

            return null;
        }

        public static List<ResultOfGetAllDataFromJobNo> getAllDataByJobNo(string urlFromBase,string jobNo)
        {

            try
            {

                string url = urlFromBase + "/jobtiming/getAllDataByJobNo?jobNo=" + jobNo;
                //Sends request to retrieve data from the web service for the specified Uri
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync(); //Returns the response as JSON string
                    List<ResultOfGetAllDataFromJobNo> jobTimings = JsonConvert.DeserializeObject<List<ResultOfGetAllDataFromJobNo>>(content.Result); //Converts JSON string to dynamic
                    return jobTimings;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
            }

            return null;
        }

    }
}
