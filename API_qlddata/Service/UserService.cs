using System;
using System.Collections.Generic;
using System.Diagnostics;
using DTO_PremierDucts.Entities;
using Newtonsoft.Json;

namespace API_premierductsqld.Service
{
	public class UserService
    {
        static System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

        public static List<Station> GetStations(string urlFromBase)
        {

            try
            {

                string url = urlFromBase + "/user/station";
                //Sends request to retrieve data from the web service for the specified Uri
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {

                    var content = response.Content.ReadAsStringAsync(); //Returns the response as JSON string
                    List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(content.Result); //Converts JSON string to dynamic
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
