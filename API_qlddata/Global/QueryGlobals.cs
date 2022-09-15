using System;
namespace API_qlddata.Global
{
	//QUERY WITH PREFIX Function_*(FUNCTION_NAME)_(NUMBER OF PARAMETER)
	public class QueryGlobals
	{

		public static string GET_DispatchInfor_list = "SELECT d.*, f.pathValue " +
			"FROM dispatch_detail d join fileinfo f " +
			"on d.pathId = f.pathId " +
			"where d.filename in ({0}) and d.handle in ({1});";

		public static string GET_AllItemInJobno = "SELECT d.*, f.pathValue " +
			"FROM dispatch_detail d join fileinfo f " +
			"on d.pathId = f.pathId " +
			"where d.jobno in ( {0} );";

	}
}

