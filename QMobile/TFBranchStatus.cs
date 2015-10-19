using System;

namespace QMobile
{
	public class GetBranchStatusResult
	{
		public string ResponseCode { get; set; }
		public string ResponseMessage { get; set; }
	}

	public class GetBranchStatusResponse
	{
		public GetBranchStatusResult getBranchStatusResult { get; set; }
	}
}

