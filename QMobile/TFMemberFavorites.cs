using System;

namespace QMobile
{
	public class TFMemberFavorites
	{
		public int id { get; set;}
		public string email { get; set;}
		public int company_id { get; set;}
		public int branch_id { get; set;}
		public string date_added { get; set;}
		public string date_updated { get; set;}
		public string __deleted { get; set;}
	}

	public class TFMemberFavoritesEx
	{
		public int id { get; set;}
		public string email { get; set;}
		public int company_id { get; set;}
		public int branch_id { get; set;}
		public string date_added { get; set;}
		public string date_updated { get; set;}
		public string __deleted { get; set;}
		public string company_name {get;set;}
		public string branch_name {get;set;}
		public string icon_image { get; set;}
		public TFMerchants merchant  { get; set;}
	}


}

