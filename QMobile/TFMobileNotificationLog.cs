using System;

namespace QMobile
{
	public class TFMobileNotificationLog
	{
		public int id { get; set; }
		public int company_id { get; set; }
		public int branch_id { get; set; }
		public string type { get; set; }
		public string email { get; set; }
		public string message { get; set; }
		public string title { get; set; }
		public string date_sent { get; set; }
		public string action { get; set; }
		public string ticket_id { get; set; }
		public string ticket_type { get; set; }
	}
}

