﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using API_premierductsqld.Entities;
using API_premierductsqld.Entities.response;
using API_qlddata.Context;
using Microsoft.Extensions.Configuration;

namespace API_premierductsqld.Service
{
    public class ReportStationService
    {

        public readonly JobTimingService jobTimingService;

        public List<JobTimingResponse> jobTimingsListGroupBy = new List<JobTimingResponse>();
        public List<DispatchDetail> dispatchDetails = new List<DispatchDetail>();
        public List<FactoryFit> factoryFits = new List<FactoryFit>();

        string current = DateTime.Now.ToString("d/M/yyyy");
        //string current = "29/3/2022";

        private List<StationDTO> stations = new List<StationDTO>();
        IConfiguration configuration = null;

       

        public readonly QLDdatacontext qLDDataContext;
        public double total_sum_nonprod_time = 0;
        public double total_sum_prod_time = 0;
        public double total_total_working_time = 0;
        public ReportStationService( QLDdatacontext qLDDataContext)
        {

            this.qLDDataContext = qLDDataContext;

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            configuration = configurationBuilder.Build();
          
            //get all station 
            stations = UserService.GetStations(configuration.GetSection("URLForPremierAPI").Value).Where(item=>item.stationStatus=="active").ToList();

            //group all jobtiming in today
            jobTimingsListGroupBy = JobTimingService.getAllDataWithoutStation(configuration.GetSection("URLForPremierAPI").Value, current);

            //get all dispatch detail
            dispatchDetails = qLDDataContext.DispatchDetails.ToList();


            //get all factory fit
            factoryFits = qLDDataContext.FactoryFits.ToList();

          
     

        }


        public void ToCSV(DataTable dtDataTable)
        {


            StreamWriter sw = new StreamWriter("reportjobno.csv", false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        public void SendEmail()
        {
            try
            {   
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
                mail.From = new MailAddress("noreply@premierducts.com.au");
                mail.To.Add("phattantran123@gmail.com");

                mail.Subject = "Report Job No";
                mail.Body = "mail with attachment";

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment("reportjobno.csv");
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("noreply@premierducts.com.au", "Wondergood101!");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                Debug.Write("mail Send");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public string calculateDuration(string a, string b)
        {
            double time1 = TimeSpan.Parse(a).TotalSeconds;
            double time2 = TimeSpan.Parse(b).TotalSeconds;

            return Convert.ToString(time2-time1);


        }
        public void reportForEachJobNo()
        {
            DataTable dataTable = new DataTable("Report For Each JobNo");
            dataTable.Columns.Add("Date", typeof(string));
            dataTable.Columns.Add("Time", typeof(string));
            dataTable.Columns.Add("Job No", typeof(string));
            dataTable.Columns.Add("Metal Area (m2)", typeof(string));
            dataTable.Columns.Add("Item Fit Area (mm2)", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(string));

            //create column.
            foreach(StationDTO station in stations)
            {
                if (!dataTable.Columns.Contains(station.stationName))
                {
                    dataTable.Columns.Add(station.stationName, typeof(string));

                }
              
            }

            foreach(JobTimingResponse jobTiming in jobTimingsListGroupBy)
            {

                var item_dispatchDetail = dispatchDetails.Where(item => item.jobno == jobTiming.jobno).ToList();
                var item_factory_fit = factoryFits.Where(item => item.jobno == jobTiming.jobno).ToList();

                var metalaream2 = item_dispatchDetail.Sum(item => item.metalarea * item.qty) + item_factory_fit.Sum(item => item.metalarea * item.qty);
                var insuArea = item_dispatchDetail.Sum(item => item.insulationarea * item.qty) + item_factory_fit.Sum(item => item.insulationarea * item.qty);
                var qty = item_dispatchDetail.Sum(item => item.qty) + item_factory_fit.Sum(item => item.qty);

                //create new ROW
                DataRow row;
                row = dataTable.NewRow();
                row["Date"] =current;
                row["Time"] = DateTime.Now.ToString("hh:mm:ss");
                row["Job No"] = jobTiming.jobno;
                row["Metal Area (m2)"] = metalaream2;
                row["Item Fit Area (mm2)"] = insuArea;
                row["Quantity"] = qty;
                //get All Data from jobno
                List<ResultOfGetAllDataFromJobNo> rsult = JobTimingService.getAllDataByJobNo(configuration.GetSection("URLForPremierAPI").Value, jobTiming.jobno);

                var groupbydate = rsult.GroupBy(i => i.jobday).Select(i=>i.Key).ToList();

                List<JobTimingResponse> jobTimingsWithDateGroupBy = new List<JobTimingResponse>();
                //get all Job from date- > to date.
                foreach(string date in groupbydate)
                {
                    var temp = JobTimingService.getAllDataWithStation(configuration.GetSection("URLForPremierAPI").Value, date);
                    for (var i = 0; i < temp.Count; i++)
                    {
                        string duration = "0";
                        if (i == temp.Count - 1)
                        {
                            break;

                        }
                        if (temp[i + 1] != null)
                        {
                            temp[i].duration = calculateDuration(temp[i].jobtime, temp[i + 1].jobtime);

                        }
                    }

                    jobTimingsWithDateGroupBy.AddRange(temp);

                }
                //paste into row  if it exists will plus to 
                foreach (var item in rsult)
                {
                    var duration = jobTimingsWithDateGroupBy.Where(i => i.id == item.id).FirstOrDefault().duration;

                    double result = TimeSpan.Parse(row[item.stationName].ToString()==""?"00:00:00" : row[item.stationName].ToString()).TotalSeconds;
                    double result_item_duration = Double.TryParse(duration, out result_item_duration) ? result_item_duration : 0.00;                    
                 
                    var tttt =(int)TimeSpan.FromSeconds(result + result_item_duration).TotalHours + TimeSpan.FromSeconds(result + result_item_duration).ToString(@"\:mm\:ss");

                    row[item.stationName] = tttt;
                }
                dataTable.Rows.Add(row);
 
            }
            ToCSV(dataTable);
            SendEmail();
        }

    }
}
