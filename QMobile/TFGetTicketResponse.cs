using System;

namespace QMobile
{
	public class UserDetails
	{
		public string awt { get; set; }
		public string customerName { get; set; }
		public string dateCalled { get; set; }
		public string dateIn { get; set; }
		public string dateOut { get; set; }
		public string dateServed { get; set; }
		public string entryLatitude { get; set; }
		public string entryLongitude { get; set; }
		public string mobileNo { get; set; }
		public string queueNo { get; set; }
		public string remarks { get; set; }
		public object smsTime { get; set; }
		public string status { get; set; }
		public string tranType { get; set; }
		public string tran_id_local { get; set; }
		public string userReferenceNo { get; set; }
	}

	public class AddTFUserJSONResult
	{
		public string ResponseCode { get; set; }
		public string ResponseMessage { get; set; }
		public UserDetails UserDetails { get; set; }
	}

	public class TFGetTicketResponse
	{
		public AddTFUserJSONResult addTFUserJSONResult { get; set; }
	}
}

