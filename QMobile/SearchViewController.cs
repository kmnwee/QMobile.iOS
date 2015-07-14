using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.iOS;
using CoreGraphics;
using MBProgressHUD;

namespace QMobile
{
	partial class SearchViewController : UIViewController
	{
		public List<TFMerchants> TFMerchants;
		public string initialText;
		LoadingOverlay _loadPop;
		MTMBProgressHUD hud;

		public SearchViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//this.searchBarMain.BarTintColor = TFColor.FromHexString("#00bcd4", 1.0f);
			var frame = this.View.Frame;
			frame.Width = 375;
			this.searchBarMain.Frame = frame;
			this.searchBarMain.Translucent = true;
			Console.WriteLine ("Search View Loaded");
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

			hud = new MTMBProgressHUD (View) {
				LabelText = "Preparing search...",
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.Fade,
				//DetailsLabelText = "loading profile details...",
				Mode = MBProgressHUDMode.Indeterminate,
				Color = UIColor.Gray,
				Opacity = 60,
				DimBackground = true
			};

			View.AddSubview (hud);
			hud.Show (animated: true);

			if (AppDelegate.tfAccount.loggedIn) {
				InvokeOnMainThread (async () => {
					TFMerchants = new List<QMobile.TFMerchants> ();
					try {
						TFMerchants = await AppDelegate.MobileService.GetTable<TFMerchants> ().Take (500).Where (TFMerchants => !TFMerchants._unlisted).ToListAsync ();

						resultsTable.Source = new SearchResultsTableSource (TFMerchants.ToArray (), this);
						resultsTable.ReloadData ();
					} catch (Exception e) {
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					}

					hud.Hide(true);
					//------LOADING Screen END----------------------
					//this._loadPop.Hide ();
					//------LOADING Screen END----------------------
				});

			} else {
				hud.Hide (true);
				//------LOADING Screen END----------------------
				//this._loadPop.Hide ();
				//------LOADING Screen END----------------------
				new UIAlertView ("Profile Needed", "To receive updates/notifications please login in the profile tab.", null, "OK", null).Show ();
			}

			searchBarMain.TextChanged += async (object sender, UISearchBarTextChangedEventArgs e) => {
				if (!e.SearchText.Equals ("")) {
					List<TFMerchants> TFMerchantsSearch = new List<QMobile.TFMerchants> ();
					try {
						TFMerchantsSearch = await AppDelegate.MobileService.GetTable<TFMerchants> ().Take (500)
							.Where (TFMerchants => !TFMerchants._unlisted && (TFMerchants.COMPANY_NAME.Contains (e.SearchText) || TFMerchants.BRANCH_NAME.Contains (e.SearchText))).ToListAsync ();
						resultsTable.Source = new SearchResultsTableSource (TFMerchantsSearch.ToArray (), this);
						InvokeOnMainThread (() => resultsTable.ReloadData ());
					} catch (Exception ex) {
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					}
				} else {
				}
			};

			searchBarMain.CancelButtonClicked += async (object sender, EventArgs e) => {
				List<TFMerchants> TFMerchantsDefault = new List<QMobile.TFMerchants> ();
				try {
					TFMerchantsDefault = await AppDelegate.MobileService.GetTable<TFMerchants> ().ToListAsync ();
					resultsTable.Source = new SearchResultsTableSource (TFMerchantsDefault.ToArray (), this);
					InvokeOnMainThread (() => resultsTable.ReloadData ());
				} catch (Exception ex) {
					new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
				}
			};
			//resultsTable.
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
