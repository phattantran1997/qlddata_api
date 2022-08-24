using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using API_premierductsqld.Entities;
using API_qlddata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API_premierductsqld.Service
{
    public class UserService
    {
        static System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

        public static List<StationDTO> GetStations(string urlFromBase)
        {

            try
            {
          

                string url = urlFromBase + "user/station";
                //Sends request to retrieve data from the web service for the specified Uri
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync(); //Returns the response as JSON string
                    List<StationDTO> stations = JsonConvert.DeserializeObject<List<StationDTO>>(content.Result); //Converts JSON string to dynamic
                    return stations;
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
