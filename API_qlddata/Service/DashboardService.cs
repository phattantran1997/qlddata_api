using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using API_qlddata.Entity.request;
using API_qlddata.Entity.response;
using DTO_PremierDucts;
using DTO_PremierDucts.DBClient;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace API_qlddata.Service
{

    public class DashboardService
    {
		static DBConnection dbCon;
        public DashboardService()
        {
			dbCon = DBConnection.Instance(Startup.StaticConfig.GetConnectionString("ConnectionForDatabase"));

        }

        public ResponseData getAllQLDdataByListJobno(List<string> jobNo)
        {
            ResponseData responseData = new ResponseData();
            List<Qlddataresponse> response = new List<Qlddataresponse>();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            var listJobNoRequest = string.Join(",", jobNo.Select(x => "'" + x + "'").ToArray());

            //string queryForDispatch = "select * from dispatch_detail where jobno in  (" + joinedPenalty + ");";

            string queryForDispatch = "SELECT t1.jobno, t1.pathId,t2.pathValue, t1.metalarea, t1.insulationarea, t1.cuttype, t1.stationName FROM dispatch_detail as t1 left JOIN fileinfo as t2 ON t1.pathId = t2.pathId where jobno in (" + listJobNoRequest + ") ;";
            string queryForFactory = "SELECT jobno, pathId, metalarea, insulationarea, cuttype, stationName from factory_fit where jobno in  (" + listJobNoRequest + ")";


            if (dbCon.IsConnect())
            {
                try
                {

                    MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(queryForDispatch, dbCon.Connection);

                    myDataAdapter.Fill(dt);

                    myDataAdapter = new MySqlDataAdapter(queryForFactory, dbCon.Connection);
                    myDataAdapter.Fill(dt1);

                    dt.Merge(dt1);
                    foreach (DataRow row in dt.Rows)
                    {
                        Qlddataresponse qlddataresponse = new Qlddataresponse();
                        qlddataresponse.isu_m2 = Convert.ToDouble(row.Field<string>("insulationarea"));
                        qlddataresponse.meta_m2 = Convert.ToDouble(row.Field<string>("metalarea")); 
                        qlddataresponse.jobNO = row.Field<string>("jobno");
                        qlddataresponse.pathID = row.Field<UInt32>("pathId");
                        qlddataresponse.pathValue = row.Field<string>("pathValue");
                        qlddataresponse.cuttype = row.Field<string>("cuttype");
                        qlddataresponse.stationName = row.Field<string>("stationName");
                        response.Add(qlddataresponse);
                    }
                }
                catch(Exception e)
                {
                    responseData.Code = ERROR_CODE.FAIL;
                    responseData.Data = e.Message.ToString();

                }
                finally
                {
                    dbCon.Close();
                }


              
            }
            responseData.Code = ERROR_CODE.SUCCESS;
            responseData.Data = response ;

            return responseData;
        }

        internal ResponseData getM2DataWithoutStation(List<string> jobnoes)
        {
            ResponseData responseData = new ResponseData();
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            var joinedPenalty = string.Join(",", jobnoes.Select(x => "'" + x + "'").ToArray());

            string queryForDispatch = "SELECT t1.jobno, t1.pathId,t2.pathValue, sum(metalarea) as metal_m2, sum(insulationarea) as insu_m2 FROM dispatch_detail as t1 left JOIN fileinfo as t2 ON t1.pathId = t2.pathId where jobno in (" + joinedPenalty + ") group by jobno, pathId;";
            string queryForFactory = "SELECT jobno,sum(metalarea) as metal_m2, sum(insulationarea) as insu_m2 FROM factory_fit  where jobno in (" + joinedPenalty + ") group by jobno;";

            if (dbCon.IsConnect())
            {

                try
                {
                    MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(queryForDispatch, dbCon.Connection);

                    myDataAdapter.Fill(dt);

                    myDataAdapter = new MySqlDataAdapter(queryForFactory, dbCon.Connection);
                    myDataAdapter.Fill(dt1);

                    dt.Merge(dt1);

                    DataTable dtFinal = dt.AsEnumerable()
            .GroupBy(r => r["jobno"])
            .Select(x =>
            {
                var row = dt.NewRow();
                row["jobno"] = x.Key;

                string[] fileinfo = x.Select(r => r["pathValue"]).First().ToString().Split("/");
                fileinfo = fileinfo.Where((item, index) => index != 0 && index != fileinfo.Count() - 1).ToArray();

                row["pathValue"] = string.Join("/", fileinfo);

                row["metal_m2"] = x.Sum(r => Convert.ToDouble(r["metal_m2"]));
                row["insu_m2"] = x.Sum(r => Convert.ToDouble(r["insu_m2"]))/1000000;

                return row;
            }).CopyToDataTable();


                    List<M2DataResponse> responses = new List<M2DataResponse>();

                    foreach (DataRow dr in dtFinal.Rows)
                    {
                        responses.Add(new M2DataResponse(dr["jobno"].ToString(), Convert.ToDouble(dr["metal_m2"]), Convert.ToDouble(dr["insu_m2"]), dr["pathValue"].ToString()));

                    }
                    responseData.Code = ERROR_CODE.SUCCESS;
                    responseData.Data = responses;

                    
                }
                catch(Exception e)
                {
                    responseData.Code = ERROR_CODE.FAIL;
                    responseData.Data = e.Message;
                }

                finally
                {
                    dbCon.Close(); // return connection to the pool
                }
            }
            return responseData;
        }

        public List<M2DataResponse> getM2DataWithStation(GetM2DataResquest resquest)
        {


            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();

            string queryForDispatch = "";
            string queryForFactory = "";

            var joinedPenalty = string.Join(",", resquest.jobNo.Select(x => "'" + x + "'").ToArray());


            if (resquest.station.Equals("Plasma 1") || resquest.station.Equals("Roll Form") || resquest.station.Equals("Folding")
                  || resquest.station.Equals("Specialty") || resquest.station.Equals("Knock - up")
                  || resquest.station.Equals("Seal Tape") || resquest.station.Equals("Plasma 2") || resquest.station.Equals("Plasma 3"))
            {
                queryForDispatch = "SELECT t1.jobno, t1.pathId,t2.pathValue,sum(metalarea) as metal_m2, sum(insulationarea) as insu_m2 FROM dispatch_detail as t1 left JOIN fileinfo as t2 ON t1.pathId = t2.pathId where jobno in (" + joinedPenalty + ") and cuttype='MachineCut' group by jobno,pathId;";
                queryForFactory = "SELECT jobno,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM factory_fit  where jobno in (" + joinedPenalty + ") and cuttype='MachineCut' group by jobno;";


            }
            else if (resquest.station.Equals("Insulation Cutting") || resquest.station.Equals("Insulation Pinning") || resquest.station.Equals("Insulation Sorting"))
            {

                queryForDispatch = "SELECT t1.jobno, t1.pathId,t2.pathValue,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM dispatch_detail as t1 left JOIN fileinfo as t2 ON t1.pathId = t2.pathId where jobno in (" + joinedPenalty + ") and insulationarea >0 group by jobno,pathId;";
                queryForFactory = "SELECT jobno,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM factory_fit  where jobno in (" + joinedPenalty + ") and insulationarea >0 group by jobno;";

            }

            else if (resquest.station.Equals("Wrapping") || resquest.station.Equals("Packing") || resquest.station.Equals("Decoil Sheet"))
            {
                queryForDispatch = "SELECT t1.jobno, t1.pathId,t2.pathValue,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM dispatch_detail as t1 left JOIN fileinfo as t2 ON t1.pathId = t2.pathId where jobno in (" + joinedPenalty + ") group by jobno,pathId;";
                queryForFactory = "SELECT jobno,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM factory_fit  where jobno in (" + joinedPenalty + ") group by jobno;";

            }
            else if (resquest.station.Equals("Coil Straight"))
            {

                queryForDispatch = "SELECT t1.jobno, t1.pathId,t2.pathValue,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM dispatch_detail as t1 left JOIN fileinfo as t2 ON t1.pathId = t2.pathId where t1.jobno in (" + joinedPenalty + ") and t1.cuttype='DecoiledStraight' group by jobno,pathId;";

                queryForFactory = "SELECT jobno,sum(metalarea) as metal_m2,sum(insulationarea) as insu_m2 FROM factory_fit  where jobno in (" + joinedPenalty + ") and cuttype='DecoiledStraight' group by jobno;";

            }




            //const string DB_CONN_STR = "server=127.0.0.1;user=phatAdmin;password=phatTest@123;port=33061;database=qlddata;";

            //MySqlConnection cn = new MySqlConnection(configuration);
            if (dbCon.IsConnect())
            {
                try
                {
                    MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(queryForDispatch, dbCon.Connection);
                    myDataAdapter.Fill(dt);

                    myDataAdapter = new MySqlDataAdapter(queryForFactory, dbCon.Connection);
                    myDataAdapter.Fill(dt1);

                    dt.Merge(dt1);


                }
                catch (Exception ex)
                {
                    Console.WriteLine("{oops - {0}", ex.Message);
                }
                finally
                {
                    dbCon.Close(); // return connection to the pool
                }
            }


            DataTable dtFinal = dt.AsEnumerable()
           .GroupBy(r => r["jobno"])
           .Select(x =>
           {
               var row = dt.NewRow();
               row["jobno"] = x.Key;

               string[] fileinfo = x.Select(r => r["pathValue"]).First().ToString().Split("/");
               fileinfo = fileinfo.Where((item, index) => index != 0 && index != fileinfo.Count() - 1).ToArray();

               row["pathValue"] = string.Join("/", fileinfo);

               row["metal_m2"] = x.Sum(r => Convert.ToDouble(r["metal_m2"]));
               row["insu_m2"] = x.Sum(r => Convert.ToDouble(r["insu_m2"]));

               return row;
           }).CopyToDataTable();

            List<M2DataResponse> responses = new List<M2DataResponse>();

            foreach (DataRow dr in dtFinal.Rows)
            {
                responses.Add(new M2DataResponse(dr["jobno"].ToString(), Convert.ToDouble(dr["metal_m2"]), Convert.ToDouble(dr["insu_m2"]), dr["pathValue"].ToString()));

            }


            //List<DispatchDetail> dispatchDetails = _qLDdatacontext.DispatchDetails.Where(item => resquest.jobNo.Contains(item.jobno)).ToList();

            //List<FactoryFit> factoryFits = _qLDdatacontext.FactoryFits.Where(item => resquest.jobNo.Contains(item.jobno)).ToList();



            //dispatchDetails.AddRange(factoryFits.Select(item => new DispatchDetail(item)).ToList());

            //B: Run stuff you want timed

            //Dung query string select jobno,cuttype,insulationarea,metalarea from dispatch_detail where jobno in ('QC038','QC040');
            //foreach (String jobno in resquest.jobNo)
            //{
            //    var result = new List<M2DataResponse>();
            //    M2DataResponse response = new M2DataResponse();

            //    if (resquest.station.Equals("Plasma 1") || resquest.station.Equals("Roll Form") || resquest.station.Equals("Folding")
            //        || resquest.station.Equals("Specialty") || resquest.station.Equals("Knock - up")
            //        || resquest.station.Equals("Seal Tape") || resquest.station.Equals("Plasma 2") || resquest.station.Equals("Plasma 3"))
            //    {
            //        result = (from o in dispatchDetails
            //                  where o.jobno.Equals(jobno) && o.cuttype.Equals("MachineCut")
            //                  select new M2DataResponse
            //                  {
            //                      meta_m2 = o.metalarea,
            //                      isu_m2 = o.insulationarea,
            //                  }
            //                     ).ToList();

            //        //lay metal m2


            //    }
            //    else if (resquest.station.Equals("Insulation Cutting") || resquest.station.Equals("Insulation Pinning") || resquest.station.Equals("Insulation Sorting"))
            //    {
            //        result = (from o in dispatchDetails
            //                  where o.jobno.Equals(jobno) && o.insulationarea > 0
            //                  select new M2DataResponse
            //                  {
            //                      meta_m2 = o.metalarea,
            //                      isu_m2 = o.insulationarea,
            //                  }
            //                     ).ToList();

            //        //chi lay insu


            //    }

            //    else if (resquest.station.Equals("Wrapping") || resquest.station.Equals("Packing") || resquest.station.Equals("Decoil Sheet"))
            //    {
            //        result = (from o in dispatchDetails
            //                  where o.jobno.Equals(jobno) && o.jobno == jobno
            //                  select new M2DataResponse
            //                  {
            //                      meta_m2 = o.metalarea,
            //                      isu_m2 = o.insulationarea,
            //                  }
            //                      ).ToList();

            //        //lay metal m2 



            //    }
            //    else if (resquest.station.Equals("Coil Straight"))
            //    {
            //        result = (from o in dispatchDetails
            //                  where o.jobno.Equals(jobno) && o.cuttype.Equals("DecoiledStraight")
            //                  select new M2DataResponse
            //                  {
            //                      meta_m2 = o.metalarea,
            //                      isu_m2 = o.insulationarea,
            //                  }
            //                      ).ToList();

            //        //lay metal m2


            //    }
            //    response.jobNO = jobno;
            //    response.meta_m2 = result.Select(i => i.meta_m2).Sum();
            //    response.isu_m2 = result.Select(i => i.isu_m2).Sum() / 1000;
            //    response.pathID = getPathIdForJobNo(jobno);
            //    responses.Add(response);

            //}


            return responses;

        }

    }
}
