using System;
using Xamarin.Auth;

namespace QMobile
{
	public class TFAccount
	{
		public Account account { get; set;}
		public string name { get; set;}
		public string email { get; set;}
		public string birthday { get; set;}
		public string id { get; set;}
		public string lastname { get; set;}
		public bool loggedIn { get; set;}
		public string gender {get;set;}
		public string timezone {get;set;}
		public int loginType { get; set;}
		public string access_token {get;set;}

	}
}

