using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using System.Globalization;
using MBProgressHUD;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace QMobile
{
	partial class FeaturedViewController : UIViewController
	{
		public List<TFMerchants> TFMerchantList;
		public List<TFMerchants> TFFeaturedMerchants;
		public UIEdgeInsets featuredInsets;
		LoadingOverlay _loadPop;
		MTMBProgressHUD hud;

		public FeaturedViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine ("Featured Tab Loaded");
			hud = new MTMBProgressHUD (View) {
				LabelText = "Fetching Stores...",
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.Fade,
				//DetailsLabelText = "loading profile details...",
				Mode = MBProgressHUDMode.Indeterminate,
				Color = UIColor.Gray,
				Opacity = 60,
				DimBackground = true
			};

			getLatesQMobileVersion (); //check version

//			//------LOADING Screen--------------------------
//			// Determine the correct size to start the overlay (depending on device orientation)
//			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
//			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
//				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
//			}
//			// show the loading overlay on the UI thread using the correct orientation sizing
//			this._loadPop = new LoadingOverlay (bounds);

//			//------LOADING Screen--------------------------
//			// Determine the correct size to start the overlay (depending on device orientation)
//			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
//			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
//				bounds.Size = new CGSize(bounds.Size.Height, bounds.Size.Width);
//			}
//			// show the loading overlay on the UI thread using the correct orientation sizing
//			this._loadPop = new LoadingOverlay (bounds);
//			this.View.Add ( this._loadPop );
//			//------LOADING Screen--------------------------
//
//			InvokeOnMainThread ( async () => {
//				TFMerchantList = new List<QMobile.TFMerchants> ();
//				TFFeaturedMerchants = new List<TFMerchants> ();
//
//				//featuredTable.ContentInset = new UIEdgeInsets(64,0,0,0);
//				try{
//					TFMerchantList = await AppDelegate.MobileService.GetTable<TFMerchants>().Take(500)
//						.Where(TFMerchants => TFMerchants.featured_flag > 0).ToListAsync();
//					TFFeaturedMerchants = TFMerchantList.GroupBy(c => c.COMPANY_NO).Select(grp => grp.FirstOrDefault()).OrderBy(f => f.featured_flag).ThenBy(b => b.BRANCH_NO).ToList();
//					featuredTable.Source = new FeaturedTableSource (TFFeaturedMerchants.ToArray (), this);
//					featuredTable.ReloadData ();
//				}catch(Exception e){
//					new UIAlertView("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
//				}
//				//------LOADING Screen END----------------------
//				this._loadPop.Hide ();
//				//------LOADING Screen END----------------------
//			});
		}

		public async void getLatesQMobileVersion ()
		{
			//check latest version of QMobile
			string ver = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
			Console.WriteLine("QMobile Version " + NSBundle.MainBundle.InfoDictionary["CFBundleVersion"]);
			Console.WriteLine("QMobile Version " + NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"]);
			//call version check
			string url = "https://tfsmsgatesit.azurewebsites.net/TFGatewayJSON.svc/getLatestQMobileVersion/iOS/";
			TFQMobileLatestVersion latestVer = new TFQMobileLatestVersion ();
			try {
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						//new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								latestVer = JsonConvert.DeserializeObject<TFQMobileLatestVersion> (content);
								Console.Out.WriteLine("QMobile Latest " + latestVer.getLatestQMobileVersionResult);

								if(!ver.Equals(latestVer.getLatestQMobileVersionResult)){
									Console.WriteLine("Must Update!");
									var proceedAlert = new UIAlertView ("Update Available", "A new version of QMobile is available in the App Store!", null, "Update Now", new string[] { "Later" });
									proceedAlert.Clicked += (s, b) => {
										if (b.ButtonIndex.ToString ().Equals ("0")) {
											//Rivets.AppLinks.Navigator.Navigate("itms://itunes.apple.com/app/id986245271?mt=8");
											UIApplication.SharedApplication.OpenUrl(new NSUrl("itms://itunes.apple.com/app/id986245271?mt=8"));
											Console.WriteLine ("Now");
										} else if (b.ButtonIndex.ToString ().Equals ("1")) {
											Console.WriteLine ("Later");
										}
									};
									proceedAlert.Show ();
								}
							}
						}
					}
				}
			}catch(Exception e){
					
			}

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			if (AppDelegate.initialLoadFeatured) {

				//this.View.Add (this._loadPop);
				View.AddSubview (hud);
				hud.Show (animated: true);
			}
			//------LOADING Screen--------------------------
			InvokeOnMainThread (async () => {
				if (AppDelegate.tfAccount.loggedIn) {
					TFMerchantList = new List<QMobile.TFMerchants> ();
					TFFeaturedMerchants = new List<TFMerchants> ();

					//featuredTable.ContentInset = featuredInsets;
					try {
						TFMerchantList = await AppDelegate.MobileService.GetTable<TFMerchants> ().Take (500)
						.Where (TFMerchants => TFMerchants.featured_flag > 0 && !TFMerchants._unlisted).ToListAsync ();
						TFFeaturedMerchants = TFMerchantList.GroupBy (c => c.COMPANY_NO).Select (grp => grp.FirstOrDefault ()).OrderBy (f => f.featured_flag).ToList ();
						featuredTable.Source = new FeaturedTableSource (TFFeaturedMerchants.ToArray (), this);
						//featuredTable.Source = new QTableSource (TFFeaturedMerchants.ToArray (), this);
						featuredTable.ReloadData ();

						hud.Hide(true);
						//------LOADING Screen END----------------------
						//this._loadPop.Hide ();
						//------LOADING Screen END----------------------
						AppDelegate.initialLoadFeatured = false;
					} catch (Exception e) {
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
						Console.Out.WriteLine(e.Message);
						Console.Out.WriteLine(e.StackTrace);
					}
				} else {
					//------LOADING Screen END----------------------
					if (AppDelegate.initialLoadFeatured) hud.Hide(true);//this._loadPop.Hide ();
					//------LOADING Screen END----------------------
					new UIAlertView ("Profile Needed", "To receive updates/notifications please login in the profile tab.", null, "OK", null).Show ();
				}

			});
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			if (featuredTable != null)
				featuredTable.ContentInset = new UIEdgeInsets (0, 0, 8, 0);
			
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
