using System;

namespace QMobile
{
	public class CurrentServing
	{
		public string counterNo { get; set; }
		public string queueNo { get; set; }
		public string tranType { get; set; }
	}

	public class GetCurrentServingByTranMobileJSONResult
	{
		public CurrentServing CurrentServing { get; set; }
		public string ResponseCode { get; set; }
		public string ResponseMessage { get; set; }
	}

	public class TFCurrentServingResponse
	{
		public GetCurrentServingByTranMobileJSONResult getCurrentServingByTranMobileJSONResult { get; set; }
	}
}

