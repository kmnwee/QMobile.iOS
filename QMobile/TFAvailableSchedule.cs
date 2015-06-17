using System;
using System.Collections.Generic;

namespace QMobile
{
	public class AvailableSched
	{
		public int index { get; set; }
		public string schedKey { get; set; }
		public string schedString { get; set; }
		public int slotsTaken { get; set; }
	}

	public class GetMerchantAvailableScheduleResult
	{
		public string ResponseCode { get; set; }
		public string ResponseMessage { get; set; }
		public List<AvailableSched> availableSched { get; set; }
	}

	public class TFAvailableSchedule
	{
		public GetMerchantAvailableScheduleResult getMerchantAvailableScheduleResult { get; set; }
	}
}

