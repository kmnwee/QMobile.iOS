using System;

namespace QMobile
{
	public class ScheduleDetails
	{
		public object awt { get; set; }

		public int branch_id { get; set; }

		public int company_id { get; set; }

		public string confirmation_no { get; set; }

		public object counterNo { get; set; }

		public string cust_name { get; set; }

		public object date_called { get; set; }

		public object date_in { get; set; }

		public object date_out { get; set; }

		public object date_served { get; set; }

		public string email { get; set; }

		public string entryLatitude { get; set; }

		public string entryLongitude { get; set; }

		public int id { get; set; }

		public string image_icon { get; set; }

		public string mobile_no { get; set; }

		public string queue_no { get; set; }

		public string remarks { get; set; }

		public string reservation_date { get; set; }

		public string reservation_status { get; set; }

		public string reservation_time { get; set; }

		public string reserve_type { get; set; }

		public object servedBy { get; set; }

		public string status { get; set; }

		public string subtran { get; set; }

		public string tran_id_local { get; set; }

		public string tran_type_name { get; set; }
	}

	public class AddTFUserScheduleJSONResult
	{
		public string ResponseCode { get; set; }

		public string ResponseMessage { get; set; }

		public ScheduleDetails ScheduleDetails { get; set; }
	}

	public class TFScheduledAppointment
	{
		public AddTFUserScheduleJSONResult addTFUserScheduleJSONResult { get; set; }
	}

}

