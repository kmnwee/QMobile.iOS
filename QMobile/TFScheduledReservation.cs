using System;

namespace QMobile
{
	public class TFScheduledReservation
	{
		public int id { get; set;}
		public string mobile_no { get; set;}
		public string cust_name { get; set;}
		public string queue_no { get; set;}
		public string status { get; set;}
		public string date_in { get; set;}
		public string date_called { get; set;}
		public string date_served { get; set;}
		public string date_out { get; set;}
		public string remarks { get; set;}
		public string awt { get; set;}
		public string reserve_type { get; set;}
		public int company_id { get; set;}
		public int branch_id { get; set;}
		public string reservation_status { get; set;}
		public string confirmation_no { get; set;}
		public string email { get; set;}
		public string tran_type_name { get; set;}
		public string tran_id_local { get; set;}
		public string subtran { get; set;}
		public string reservation_date { get; set;}
		public string reservation_time { get; set;}
		public string servedBy { get; set;}
		public string counterNo { get; set;}
		public bool __deleted { get; set;}
		public string entryLatitude { get; set;}
		public string entryLongitude { get; set;}
		public string image_icon { get; set;}
		public string reservation_time_string {get;set;}
		public string local_guid {get;set;}
	}
}

