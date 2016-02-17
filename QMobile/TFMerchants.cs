using System;

namespace QMobile
{
	public class TFQMobileLatestVersion
	{
		public string getLatestQMobileVersionResult { get; set; }
	}

	public class TFMerchants
	{
		public int id { get; set; }

		public int COMPANY_NO { get; set; }

		public int BRANCH_NO { get; set; }

		public string COMPANY_NAME { get; set; }

		public string BRANCH_NAME { get; set; }

		public string COUNTRY { get; set; }

		public string LICENSE_EXPIRY_DATE { get; set; }

		public bool complete { get; set; }

		public string businesstype_id { get; set; }

		public string address { get; set; }

		public string siteURL { get; set; }

		public string serviceURL { get; set; }

		public string longitude { get; set; }

		public string latitude { get; set; }

		public int timezone { get; set; }

		public int provider_code { get; set; }

		public string sms_code { get; set; }

		public string icon_image { get; set; }

		public string contact_no { get; set; }

		public string edition { get; set; }

		public int featured_flag { get; set; }

		public string featured_date { get; set; }

		public int video_limit { get; set; }

		public string STATE_PROVINCE { get; set; }

		public int schedReserve_flag { get; set; }

		public int schedReserve_interval { get; set; }

		public int schedReserve_slots { get; set; }

		public int schedReserveWeekend_flag { get; set; }

		public string schedReserve_startTime { get; set; }

		public string schedReserve_endTime { get; set; }

		public double distance { get; set; }

		public bool _unlisted { get; set; }

		public bool schedReserve_sameDay { get; set; }

		public int regReserve_flag { get; set; }

		public int regReserveWeekend_flag { get; set; }
	}
}

