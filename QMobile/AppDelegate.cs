using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using WindowsAzure.Messaging;
using Foundation;
using UIKit;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using System.Globalization;
using AudioToolbox;
using System.Web;

namespace QMobile
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public static UIStoryboard initialStoryboard = UIStoryboard.FromName ("MainStoryboard", null);
		public static UINavigationController initialViewController;

		// class-level declarations
		const string MapsApiKey = "AIzaSyDmD218pC9mr7Ns4RYpnBGVJtJt5mpqG0I";
		public static MobileServiceClient MobileService;
		public static TFAccount tfAccount;
		public static TFContentFetchFlags contentFlags;
		public static bool allowRunBGTasks;
		public static bool initialLoadFeatured;
		public static bool initialLoadTix;
		public static bool initialLoadFav;
		NSUrl url;
		SystemSound systemSound;
		LoadingOverlay _loadPop;

		private SBNotificationHub Hub { get; set; }

		public override UIWindow Window {
			get;
			set;
		}
		
		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			allowRunBGTasks = false;
			initialLoadFeatured = true;
			initialLoadTix = true;
			initialLoadFav = true;
			contentFlags = new TFContentFetchFlags ();
			contentFlags.listenForNewContent = false;
			contentFlags.newContentAvailable = false;
			contentFlags.company_id = 0;
			contentFlags.branch_id = 0;
			var platform = new Microsoft.WindowsAzure.MobileServices.CurrentPlatform ();
			System.Diagnostics.Debug.WriteLine (platform);
			var uID = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString ();
			Console.Out.WriteLine ("Device ID: " + uID);
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

			try {
				MobileService = new MobileServiceClient (
					"https://tfmobilegatewaysit.azure-mobile.net/",
					"brFnSUNYklwSVFQhCoOvnORrxhKWur79"
				);

				//MapServices.ProvideAPIKey (MapsApiKey);
			} catch (Exception e) {
				//new UIAlertView ("Azure Prompt", e.Message, null, "OK", null).Show ();
				//new UIAlertView ("Azure Prompt", e.StackTrace, null, "OK", null).Show ();
				Console.Out.WriteLine (e.StackTrace);
			}

			getStoredAccount ();
			if (!String.IsNullOrEmpty (tfAccount.email)) {
				Console.WriteLine ("Email not empty - proceed with Registration for notifications...");
				registerForRemoteNotifications ();
			}

			UIApplication.CheckForEventAndDelegateMismatches = false;
			return true;
		}

		public void getStoredAccount ()
		{
			try {
				IEnumerable<Account> accounts = AccountStore.Create ().FindAccountsForService ("Facebook");
				IEnumerable<Account> accounts2 = AccountStore.Create ().FindAccountsForService ("Anonymous");
				tfAccount = new TFAccount ();
				int x = 0, y = 0;

				foreach (Account a in accounts) {
					x++;
					Console.WriteLine ("Accounts: " + x);
					Console.WriteLine ("Account username: " + a.Username);
					Console.WriteLine ("Account toString: " + a.ToString ());
				}

				foreach (Account a in accounts2) {
					y++;
					Console.WriteLine ("Accounts: " + y);
					Console.WriteLine ("Account username: " + a.Username);
					Console.WriteLine ("Account toString: " + a.ToString ());
				}
				Account currentAcct = null;
				if (x > 0) {
					Console.WriteLine ("Appdelegate Pasok Account!");
					using (IEnumerator<Account> enumer = accounts.GetEnumerator ()) {
						if (enumer.MoveNext ()) {
							currentAcct = enumer.Current;
							Console.WriteLine ("Current Account toString: " + currentAcct.ToString ());
							var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, currentAcct);
							try {
								string json = request.GetResponseAsync ().Result.GetResponseText ();
								Console.WriteLine (json);
								var parsed = JObject.Parse (json);
								Console.WriteLine (parsed ["first_name"]);
								Console.WriteLine (parsed ["last_name"]);
								Console.WriteLine (parsed ["id"]);
								Console.WriteLine (Convert.ToString (parsed ["first_name"]));
								InvokeOnMainThread (() => {
									tfAccount.loggedIn = true;
									tfAccount.name = Convert.ToString (parsed ["first_name"]);
									tfAccount.lastname = Convert.ToString (parsed ["last_name"]);
									if (!String.IsNullOrEmpty (Convert.ToString (parsed ["email"])))
										tfAccount.email = Convert.ToString (parsed ["email"]);
									else
										tfAccount.email = Convert.ToString (parsed ["id"]);
									tfAccount.birthday = Convert.ToString (parsed ["birthday"]);
									tfAccount.account = currentAcct;
									tfAccount.id = Convert.ToString (parsed ["id"]);
									tfAccount.loginType = 1;
									string param1 = HttpUtility.ParseQueryString(currentAcct.ToString ()).Get("access_token");
									tfAccount.access_token = param1;
									Console.WriteLine("ACCESS TOKEN : " + tfAccount.access_token);
								});
							} catch (Exception ex) {
								tfAccount.loggedIn = false;
								Console.WriteLine("Login failed - FB " + ex.StackTrace);
								Console.WriteLine("Login failed - FB " + ex.Message);
							}

						}
					}
				} else if (y > 0) {
					//Anonymous Login

					Console.WriteLine ("Appdelegate Anonymous Pasok Account!");
					using (IEnumerator<Account> enumer = accounts2.GetEnumerator ()) {
						if (enumer.MoveNext ()) {
							currentAcct = enumer.Current;
							Console.WriteLine ("Current Account toString: " + currentAcct.ToString ());
							string uEmail = string.Empty;
							string uID = string.Empty;
							string uType = string.Empty;
							string uName = string.Empty;
							string uLastName = string.Empty;
							currentAcct.Properties.TryGetValue("uID", out uEmail);
							currentAcct.Properties.TryGetValue("uID", out uID);
							currentAcct.Properties.TryGetValue("uType", out uType);
							currentAcct.Properties.TryGetValue("uName", out uName);
							currentAcct.Properties.TryGetValue("uLastName", out uLastName);

							InvokeOnMainThread (() => {
								tfAccount.loggedIn = true;
								tfAccount.email = uEmail;
								tfAccount.account = currentAcct;
								tfAccount.id = uID;
								tfAccount.name = uName;
								tfAccount.lastname = uLastName;
								tfAccount.loginType = 2;
								Console.WriteLine ("email : " + tfAccount.email);
								Console.WriteLine ("id : " + tfAccount.id);
								Console.WriteLine ("name : " + tfAccount.name);
								Console.WriteLine ("lastname : " + tfAccount.lastname);
							});
						}
					}

				} else {
					if (tfAccount.loginType == 2 && String.IsNullOrEmpty (tfAccount.email)) {
						tfAccount.loggedIn = true;
						Console.WriteLine ("Appdelegate In Type 2");
					} else {
						tfAccount.loggedIn = false;
						Console.WriteLine ("Appdelegate Labas Account!");
					}
					
				}
			} catch (Exception fs) {
				new UIAlertView ("Account Prompt", fs.Message, null, "OK", null).Show ();
				new UIAlertView ("Account Prompt", fs.StackTrace, null, "OK", null).Show ();
			}
		}

		public static void registerForRemoteNotifications ()
		{
			try {
				var version8 = new Version (8, 0);
				Console.WriteLine ("Version " + UIDevice.CurrentDevice.SystemVersion.ToString ());
				if (new Version (UIDevice.CurrentDevice.SystemVersion) >= version8) {
					Console.WriteLine ("Version >> " + UIDevice.CurrentDevice.SystemVersion.ToString ());
					var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Sound |
					               UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);

					UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
					UIApplication.SharedApplication.RegisterForRemoteNotifications ();

					//UIApplication.SharedApplication.UnregisterForRemoteNotifications();

				} else {
					UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (UIRemoteNotificationType.Badge |
					UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);
				}
			} catch (Exception ef) {

				new UIAlertView ("Notif Prompt", ef.Message, null, "OK", null).Show ();
				new UIAlertView ("Notif Prompt", ef.StackTrace, null, "OK", null).Show ();
			}
		}

		public static void unregisterForRemoteNotifications ()
		{
			Console.WriteLine ("unregisterForRemoteNotifications was called");
			try {
				UIApplication.SharedApplication.UnregisterForRemoteNotifications ();
			} catch (Exception ef) {

				new UIAlertView ("Notif Prompt", ef.Message, null, "OK", null).Show ();
				new UIAlertView ("Notif Prompt", ef.StackTrace, null, "OK", null).Show ();
			}
		}

		public override void DidRegisterUserNotificationSettings (UIApplication application, UIUserNotificationSettings notificationSettings)
		{
			Console.WriteLine ("DidRegisterUserNotificationSettings was called");
		}


		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			Console.WriteLine ("FailedToRegisterForRemoteNotifications was called");
			Console.WriteLine ("Error1: " + error.LocalizedFailureReason);
			Console.WriteLine ("Error2: " + error.LocalizedDescription);
			Console.WriteLine ("Error3: " + error.LocalizedRecoverySuggestion);
		}

		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
		{
			// show an alert
			new UIAlertView (notification.AlertAction, notification.AlertBody, null, "OK", null).Show ();
			Console.WriteLine ("Notification Received!");

			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			Console.WriteLine ("RegisteredForRemoteNotifications was called");
			Hub = new SBNotificationHub (Constants.ConnectionString, Constants.NotificationHubPath);

			Console.WriteLine ("deviceToken : " + deviceToken.Description);

			Hub.UnregisterAllAsync (deviceToken, (error) => {
				if (error != null) {
					Console.WriteLine ("Error calling Unregister: {0}", error.ToString ());
					return;
				}

				url = NSUrl.FromFilename ("Sounds/3.mp3");

				systemSound = new SystemSound (url);

				Console.WriteLine ("Registering with tag : " + tfAccount.email);
				var tagString = new List<String>{ tfAccount.email };// do after login
				NSSet tags = new NSSet (tagString.ToArray ()); // create tags if you want

				Hub.RegisterNativeAsync (deviceToken, tags, (errorCallback) => {
					if (errorCallback != null)
						Console.WriteLine ("RegisterNativeAsync error: " + errorCallback.ToString ());
				});
			});
		}

		public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			Console.WriteLine ("DidReceiveRemoteNotification was called");
			ProcessNotification (userInfo, false);
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			Console.WriteLine ("ReceivedRemoteNotification was called --  after tap");
			ProcessNotification (userInfo, false);
		}

		async void ProcessNotification (NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey (new NSString ("aps"))) {
				Console.WriteLine ("options : " + options.ToString ());

				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey (new NSString ("aps")) as NSDictionary;
				if (aps.ContainsKey (new NSString ("content-available"))) {
					Console.WriteLine (options.ToString ());
					Console.WriteLine ("Silent Notification");
					string company_id = string.Empty;
					string branch_id = string.Empty;
					string type = string.Empty;

					if (aps.ContainsKey (new NSString ("company_id")))
						company_id = (aps [new NSString ("company_id")] as NSString).ToString ();
					if (aps.ContainsKey (new NSString ("branch_id")))
						branch_id = (aps [new NSString ("branch_id")] as NSString).ToString ();
					if (aps.ContainsKey (new NSString ("content_type")))
						type = (aps [new NSString ("content_type")] as NSString).ToString ();

					if (type.Equals ("CURRENTSERVING")) {
						Console.WriteLine ("Fetch Current Serving from " + company_id + " + " + branch_id);

						if (contentFlags.listenForNewContent) {
							if (Convert.ToString (contentFlags.company_id).Equals (company_id) && Convert.ToString (contentFlags.branch_id).Equals (branch_id))
								contentFlags.newContentAvailable = true;
						}
					}

				} else {
					string alert = string.Empty;
					string title = string.Empty;
					string email = string.Empty;
					string ticket_type = string.Empty;
					string company_id = string.Empty;
					string branch_id = string.Empty;
					string ticket_id = string.Empty;
					//Extract the alert text
					// NOTE: If you're using the simple alert by just specifying
					// "  aps:{alert:"alert msg here"}  " this will work fine.
					// But if you're using a complex alert with Localization keys, etc.,
					// your "alert" object from the aps dictionary will be another NSDictionary.
					// Basically the json gets dumped right into a NSDictionary,
					// so keep that in mind.
					NSDictionary alertDS = aps.ObjectForKey (new NSString ("alert")) as NSDictionary;
					alert = (alertDS [new NSString ("body")] as NSString).ToString ();

					if (aps.ContainsKey (new NSString ("email")))
						email = (aps [new NSString ("email")] as NSString).ToString ();
					if (aps.ContainsKey (new NSString ("ticket_type")))
						ticket_type = (aps [new NSString ("ticket_type")] as NSString).ToString ();
					if (aps.ContainsKey (new NSString ("company_id")))
						company_id = (aps [new NSString ("company_id")] as NSString).ToString ();
					if (aps.ContainsKey (new NSString ("branch_id")))
						branch_id = (aps [new NSString ("branch_id")] as NSString).ToString ();
					if (aps.ContainsKey (new NSString ("ticket_id")))
						ticket_id = (aps [new NSString ("ticket_id")] as NSString).ToString ();
					//If this came from the ReceivedRemoteNotification while the app was running,
					// we of course need to manually process things like the sound, badge, and alert.
					if (!fromFinishedLaunching) {
						//Manually show an alert
						if (!string.IsNullOrEmpty (alert)) {
							systemSound.PlayAlertSound ();
							UIAlertView avAlert = new UIAlertView (title, alert, null, "View", new string[] { "Close" });
							//var proceedAlertReserve = new UIAlertView ("Please Confirm Details:", confirmationString, null, "Proceed", new string[] { "Cancel" });
							avAlert.Clicked += (s, b) => {
								if (b.ButtonIndex.ToString ().Equals ("0")) {
//							avAlert.Dismissed += (object sender, UIButtonEventArgs e) => {
									Console.WriteLine ("After View was pressed");
									List<TFScheduledReservation> aptLst = new List<TFScheduledReservation> ();
									List<TFOLReservation> rsvLst = new List<TFOLReservation> ();
									TFTicket tix = new TFTicket ();
									List<TFMerchants> tfmerchants = new List<TFMerchants> ();
									InvokeOnMainThread (async () => {
										if (ticket_type.Equals ("APPOINTMENT")) {
											//get tix from azure
											aptLst = await AppDelegate.MobileService.GetTable<TFScheduledReservation> ()
										.Where (sr => sr.id == Convert.ToInt32 (ticket_id) && sr.company_id == Convert.ToInt32 (company_id) && sr.branch_id == Convert.ToInt32 (branch_id)).Take (1).ToListAsync ();
											tfmerchants = new List<TFMerchants> ();
											tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
										.Where (TFMerchants => TFMerchants.COMPANY_NO == Convert.ToInt32 (company_id) && TFMerchants.BRANCH_NO == Convert.ToInt32 (branch_id)).Take (1).ToListAsync ();
											if (aptLst.Any ()) {
												if (tfmerchants.Any ())
													tix.merchant = tfmerchants.ToArray () [0];
												tix.branch_id = aptLst.First ().branch_id;
												tix.company_id = aptLst.First ().company_id;
												tix.id = Convert.ToString (aptLst.First ().id);
												tix.cust_name = aptLst.First ().cust_name;
												tix.queue_no = aptLst.First ().queue_no;
												tix.status = aptLst.First ().status;
												tix.date = aptLst.First ().reservation_date;
												tix.time = aptLst.First ().reservation_time;
												tix.image_icon = aptLst.First ().image_icon;
												tix.tran_type_name = aptLst.First ().tran_type_name;
												tix.tran_id_local = aptLst.First ().tran_id_local;
												tix.type = "APPOINTMENT";
											}
										} else if (ticket_type.Equals ("RESERVATION")) {
											//get tix from azure
											rsvLst = await AppDelegate.MobileService.GetTable<TFOLReservation> ()
										.Where (sr => sr.user_refno == ticket_id && sr.company_id == Convert.ToInt32 (company_id) && sr.branch_id == Convert.ToInt32 (branch_id)).Take (1).ToListAsync ();
											tfmerchants = new List<TFMerchants> ();
											tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ()
										.Where (TFMerchants => TFMerchants.COMPANY_NO == Convert.ToInt32 (company_id) && TFMerchants.BRANCH_NO == Convert.ToInt32 (branch_id)).Take (1).ToListAsync ();
											if (rsvLst.Any ()) {
												if (tfmerchants.Any ())
													tix.merchant = tfmerchants.ToArray () [0];
												tix.branch_id = rsvLst.First ().branch_id;
												tix.company_id = rsvLst.First ().company_id;
												tix.id = Convert.ToString (rsvLst.First ().id);
												tix.cust_name = rsvLst.First ().cust_name;
												tix.queue_no = rsvLst.First ().queue_no;
												tix.status = rsvLst.First ().status;
												tix.date = DateTime.ParseExact (rsvLst.First ().date_in, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString ("yyyy-MM-dd");
												tix.image_icon = rsvLst.First ().image_icon;
												tix.tran_type_name = rsvLst.First ().tran_type;
												tix.tran_id_local = rsvLst.First ().tran_id_local;
												tix.type = "RESERVATION";
											}
										}

										if (!String.IsNullOrEmpty (tix.queue_no)) {
											var window = new UIWindow (UIScreen.MainScreen.Bounds);

											initialViewController = (UINavigationController)initialStoryboard.InstantiateInitialViewController ();

											Window.RootViewController = initialViewController;

											TicketViewController ticketView = initialViewController.Storyboard.InstantiateViewController ("TicketViewController") as TicketViewController;
											DashTabController dashTabView = initialViewController.Storyboard.InstantiateViewController ("DashTabController") as DashTabController;

											// reset our badge
											UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
//									ticketView.NavigationItem.SetRightBarButtonItem (new UIBarButtonItem ("Home", UIBarButtonItemStyle.Plain, (sender2, args) => {
//										Console.WriteLine ("Home Button was pressed!");
//										dashTabView.NavigationItem.SetHidesBackButton (true, false);
//										initialViewController.NavigationController.PushViewController (dashTabView, true);
//									}), true);

											ticketView.ticket = tix;
											initialViewController.PushViewController (ticketView, true);

										}
									});
								} else if (b.ButtonIndex.ToString ().Equals ("1")) {
									Console.WriteLine ("Cancel");
								}
							};

							avAlert.Show ();
						}
					}
				}
				//here
			}
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
			Console.WriteLine ("App entering background state.");
			allowRunBGTasks = false;
		}

		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
			allowRunBGTasks = true;
			Console.WriteLine ("App entering foreground state.");
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		//		public static MobileServiceClient MobileService = new MobileServiceClient(
		//			"https://tfmobilegatewaysit.azure-mobile.net/",
		//			"brFnSUNYklwSVFQhCoOvnORrxhKWur79"
		//		);

		//		void getCurrentServingList (int company_id, int branch_id)
		//		{
		//
		//			string url = "";
		//
		//			if (merchant.edition.Equals ("CLOUD-SAAS")) {
		//				url = String.Format (merchant.serviceURL	+ "/kioskJSON.svc/getCurrentServingListJSON/{0}/{1}/", merchant.COMPANY_NO, merchant.BRANCH_NO);
		//			} else {
		//				url = String.Format (merchant.serviceURL	+ "/kioskJSON.svc/getCurrentServingListJSON/");
		//			}
		//
		//
		//			try {
		//				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
		//				request.ContentType = "application/json";
		//				request.Method = "GET";
		//				using (HttpWebResponse response = request.GetResponse () as HttpWebResponse) {
		//					if (response.StatusCode != HttpStatusCode.OK) {
		//						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
		//						new UIAlertView ("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
		//					} else {
		//						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
		//							var content = reader.ReadToEnd ();
		//							if (string.IsNullOrWhiteSpace (content)) {
		//								Console.Out.WriteLine ("Response contained empty body...");
		//							} else {
		//								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
		//								servingListJSON = new TFCurrentServingListResponse ();
		//								servingListJSON = JsonConvert.DeserializeObject<TFCurrentServingListResponse> (content);
		//								Console.WriteLine (servingListJSON.getCurrentServingListJSONResult.ResponseCode);
		//								Console.WriteLine (servingListJSON.getCurrentServingListJSONResult.ResponseMessage);
		//
		//								if (servingListJSON.getCurrentServingListJSONResult.ResponseCode.Equals ("00")) {
		//									foreach (CurrentServingList cs in servingListJSON.getCurrentServingListJSONResult.CurrentServingList) {
		//										Console.WriteLine ("String Date : " + cs.timeStamp);
		//										Console.WriteLine ("Date : " + DateTime.Parse (cs.timeStamp).ToString ());
		//									}
		//
		//									lastTicketNoLabel.Text = servingListJSON.getCurrentServingListJSONResult.CurrentServingList
		//										.OrderByDescending (r => DateTime.Parse (r.timeStamp)).FirstOrDefault ().queueNo;
		//								} else {
		//									lastTicketNoLabel.Text = " -- ";
		//									new UIAlertView ("Counters are Closed", "Counters have not yet started serving tickets. Try refreshing after a few minutes.", null, "OK", null).Show ();
		//								}
		//
		//							}
		//						}
		//					}
		//				}
		//			} catch (Exception e) {
		//				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
		//				Console.WriteLine ("Problem loading Currently Serving List...");
		//				Console.WriteLine (e.Message);
		//				Console.WriteLine (e.StackTrace);
		//			}
		//			//---------------------------------------------------------
		//}
	}
}

