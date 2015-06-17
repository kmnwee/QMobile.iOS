using System;
using System.Collections.Generic;

namespace QMobile
{
	public class CurrentServingList
	{
		public int ServingID { get; set; }
		public int TFUserID { get; set; }
		public string counterNo { get; set; }
		public string customerName { get; set; }
		public string mobileNo { get; set; }
		public string queueNo { get; set; }
		public object subTranType { get; set; }
		public object subTranTypeId { get; set; }
		public string timeStamp { get; set; }
		public string tranType { get; set; }
		public object tranTypeId { get; set; }
	}

	public class GetCurrentServingListJSONResult
	{
		public List<CurrentServingList> CurrentServingList { get; set; }
		public string ResponseCode { get; set; }
		public string ResponseMessage { get; set; }
	}

	public class TFCurrentServingListResponse
	{
		public GetCurrentServingListJSONResult getCurrentServingListJSONResult { get; set; }
	}
}

