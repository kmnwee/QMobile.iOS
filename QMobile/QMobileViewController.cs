using System;
using System.Drawing;
using System.Collections.Generic;
using Xamarin.Auth;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;
using Foundation;
using UIKit;
using System.Net;
using System.Json;
using System.IO;
using System.Threading.Tasks;

namespace QMobile
{
	public partial class QMobileViewController : UIViewController
	{
		public QMobileViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			IEnumerable<Account> accounts = AccountStore.Create ().FindAccountsForService ("Facebook");

			int x = 0;
			foreach (Account a in accounts) 
			{
				x++;
				Console.WriteLine ("Accounts: " + x);
				Console.WriteLine ("Account username: " + a.Username);
				Console.WriteLine ("Account toString: " + a.ToString());
			}
			Account currentAcct = null;
			using (IEnumerator<Account> enumer = accounts.GetEnumerator ()) {
				if (enumer.MoveNext ()) {
					currentAcct = enumer.Current;
					Console.WriteLine ("Current Account toString: " + currentAcct.ToString());

					DashTabController dashTab = this.Storyboard.InstantiateViewController ("DashTabController") as DashTabController;
					//DashBoardController dashTab = this.Storyboard.InstantiateViewController ("DashBoardController") as DashBoardController;

					var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, currentAcct);
					request.GetResponseAsync().ContinueWith (t => {
						if (t.IsFaulted)
							Console.WriteLine ("Error: " + t.Exception.InnerException.Message);
						else {
							string json = t.Result.GetResponseText();
							Console.WriteLine (json);
							var parsed = JObject.Parse(json);
							Console.WriteLine(parsed["first_name"]);
							Console.WriteLine(parsed["last_name"]);
							Console.WriteLine(parsed["id"]);
							Console.WriteLine(Convert.ToString(parsed["first_name"]));
							InvokeOnMainThread (() => {
								dashTab.name = Convert.ToString(parsed["first_name"]);
								dashTab.lastname = Convert.ToString(parsed["last_name"]);
								if(!String.IsNullOrEmpty(Convert.ToString(parsed["email"]))) dashTab.email = Convert.ToString(parsed["email"]);
								else dashTab.email = Convert.ToString(parsed["id"]);
								dashTab.birthday = Convert.ToString(parsed["birthday"]);
								dashTab.Title += String.Format("");
								dashTab.acct = currentAcct;
								dashTab.id = Convert.ToString(parsed["id"]);
//								UIImageView logoNavBarView = new UIImageView ();
//								logoNavBarView.ContentMode = UIViewContentMode.ScaleAspectFit;
//								logoNavBarView.Image = UIImage.FromBundle ("LogoWithOutBackground.png");
//								NavigationItem.TitleView = logoNavBarView;
								NavigationItem.BackBarButtonItem = new UIBarButtonItem ("<", UIBarButtonItemStyle.Plain, null);
								//if(NavigationController == null)Console.WriteLine("Navigation Controller NULL");
								NavigationController.PushViewController (dashTab, true);

							});
						}
					});
				}
			}



			gpButton.TouchUpInside+= (object sender, EventArgs e) => {
				string name = "";
				string accessToken = "";

				try
				{
					var auth = new OAuth2Authenticator (
						clientId: "932884625219-rma65jituq0ni8t07qubjbnr2poc66po.apps.googleusercontent.com",
						clientSecret: "KLEZxQNXf140J3fibCcrrUjK",
						scope: "openid https://www.googleapis.com/auth/userinfo.email",
						authorizeUrl: new Uri ("https://accounts.google.com/o/oauth2/auth"),
						redirectUrl: new Uri ("http://tfinnovations.com/"),
						accessTokenUrl: new Uri ("https://accounts.google.com/o/oauth2/token"),
						getUsernameAsync: null
					);

					auth.Completed += (sender2, eventArgs) => {
						DismissViewController (true, null);

						//DashBoardController dashBoard = this.Storyboard.InstantiateViewController ("DashBoardController") as DashBoardController;
						DashTabController dashTab = this.Storyboard.InstantiateViewController ("DashTabController") as DashTabController;

						if (eventArgs.IsAuthenticated) {
							// Use eventArgs.Account to do wonderful things
							//name = eventArgs.Account.Username;
							AccountStore.Create ().Save (eventArgs.Account, "Google");
							eventArgs.Account.Properties.TryGetValue("access_token", out accessToken);
							Console.WriteLine("token: " + accessToken);

							string url = "https://www.googleapis.com/oauth2/v1/userinfo?access_token="+accessToken;
							HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
							request.ContentType = "application/json";
							request.Method = "GET";

							using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
							{
								if (response.StatusCode != HttpStatusCode.OK)
									Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
								using (StreamReader reader = new StreamReader(response.GetResponseStream()))
								{
									var content = reader.ReadToEnd();
									if(string.IsNullOrWhiteSpace(content)) {
										Console.Out.WriteLine("Response contained empty body...");
									}
									else {
										Console.Out.WriteLine("Response Body: \r\n {0}", content);
										var parsed = JObject.Parse(content);
										Console.WriteLine(parsed["name"]);
										Console.WriteLine(parsed["family_name"]);
										name = Convert.ToString(parsed["name"]);
										Console.WriteLine (name);
										InvokeOnMainThread (() => {
											dashTab.name = name;
											dashTab.email = Convert.ToString(parsed["email"]);
											dashTab.birthday = Convert.ToString(parsed["birthday"]);
											dashTab.acct = eventArgs.Account;
											NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Logout", UIBarButtonItemStyle.Plain, null);
											NavigationController.PushViewController (dashTab, true);
										});
									}
								}
							}
						}
						else{
							name = "Failed!";
						}

					};
					PresentViewController (auth.GetUI (), true, null);
				}
				catch (Exception ex)
				{
					new UIAlertView("Error!", "Internet connection lost. Please try again." , null, "OK", null).Show ();
				}

			};


			fbButton.TouchUpInside += (object sender, EventArgs e) =>{
				// Launches a new instance of CallHistoryController
				//CallHistoryController callHistory = this.Storyboard.InstantiateViewController ("CallHistoryController") as CallHistoryController;
				string name = "";

				try{
					var auth = new OAuth2Authenticator (
						clientId: "440108756136511",
						scope: "email",
						authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
						redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

					auth.Completed += (sender2, eventArgs) => {
						DismissViewController (true, null);
						//DashBoardController dashBoard = this.Storyboard.InstantiateViewController ("DashBoardController") as DashBoardController;
						DashTabController dashTab = this.Storyboard.InstantiateViewController ("DashTabController") as DashTabController;

						if (eventArgs.IsAuthenticated) {
							// Use eventArgs.Account to do wonderful things
							//name = eventArgs.Account.Username;
							AccountStore.Create ().Save (eventArgs.Account, "Facebook");

							var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, eventArgs.Account);
							request.GetResponseAsync().ContinueWith (t => {
								if (t.IsFaulted)
									Console.WriteLine ("Error: " + t.Exception.InnerException.Message);
								else {
									string json = t.Result.GetResponseText();
									Console.WriteLine (json);
									var parsed = JObject.Parse(json);
									Console.WriteLine(parsed["first_name"]);
									Console.WriteLine(parsed["last_name"]);
									name = Convert.ToString(parsed["first_name"]);
									Console.WriteLine (name);
									InvokeOnMainThread (() => {
										dashTab.name = Convert.ToString(parsed["first_name"]);
										if(!String.IsNullOrEmpty(Convert.ToString(parsed["email"]))) dashTab.email = Convert.ToString(parsed["email"]);
										else dashTab.email = Convert.ToString(parsed["id"]);
										dashTab.birthday = Convert.ToString(parsed["birthday"]);
										dashTab.Title += String.Format("");
										dashTab.acct = eventArgs.Account;
										dashTab.id = Convert.ToString(parsed["id"]);
										NavigationItem.BackBarButtonItem = new UIBarButtonItem ("<", UIBarButtonItemStyle.Plain, null);
										//if(NavigationController == null)Console.WriteLine("Navigation Controller NULL");
										NavigationController.PushViewController (dashTab, true);
									});
								}
							});
						}
						else{
							name = "Failed!";
							InvokeOnMainThread (() => {});
						}

					};
					PresentViewController (auth.GetUI (), true, null);
				}
				catch (Exception ex)
				{
					new UIAlertView("Error!", "Internet connection lost. Please try again." , null, "OK", null).Show ();
				}

			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}

