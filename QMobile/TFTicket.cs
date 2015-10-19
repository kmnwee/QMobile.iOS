using System;

namespace QMobile
{
	public class TFTicket
	{
		public int company_id { get; set; } 
		public int branch_id { get; set; } 
		public string type { get; set; } 
		public string id { get; set; } 
		public string cust_name { get; set; } 
		public string queue_no { get; set; } 
		public string status { get; set; } 
		public string date { get; set; } 
		public string time { get; set; } 
		public string timeString { get; set;}
		public string image_icon { get; set; } 
		public string tran_type_name { get; set; } 
		public string tran_id_local { get; set; } 
		public TFMerchants merchant {get;set;}
	}
}

