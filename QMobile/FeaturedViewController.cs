using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using System.Globalization;

namespace QMobile
{
	partial class FeaturedViewController : UIViewController
	{
		public List<TFMerchants> TFMerchantList;
		public List<TFMerchants> TFFeaturedMerchants;
		public UIEdgeInsets featuredInsets;
		LoadingOverlay _loadPop;

		public FeaturedViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine ("Featured Tab Loaded");
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



		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			//------LOADING Screen--------------------------
			// Determine the correct size to start the overlay (depending on device orientation)
			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
			}
			// show the loading overlay on the UI thread using the correct orientation sizing
			this._loadPop = new LoadingOverlay (bounds);
			this.View.Add (this._loadPop);
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
						featuredTable.ReloadData ();
						//------LOADING Screen END----------------------
						this._loadPop.Hide ();
						//------LOADING Screen END----------------------
					} catch (Exception e) {
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					}
				} else {
					//------LOADING Screen END----------------------
					this._loadPop.Hide ();
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
				featuredTable.ContentInset = new UIEdgeInsets (64, 0, 0, 0);
			
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
