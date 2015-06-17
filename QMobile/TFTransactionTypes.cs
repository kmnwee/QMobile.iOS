using System;
using System.Collections.Generic;

namespace QMobile
{
	public class TransactionType
	{
		public int index { get; set;}
		public string tranType { get; set; }
		public string tran_id_local { get; set; }
		public int waitingCount { get; set; }
	}

	public class GetAllTranTypesMobileJSONResult
	{
		public string ResponseCode { get; set; }
		public string ResponseMessage { get; set; }
		public List<TransactionType> TransactionTypes { get; set; }
	}

	public class TFTransactionTypes
	{
		public GetAllTranTypesMobileJSONResult getAllTranTypesMobileJSONResult { get; set; }
	}
}

