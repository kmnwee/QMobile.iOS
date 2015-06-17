using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using CoreGraphics;
using ObjCRuntime;
using System.Linq;

namespace QMobile
{
	partial class FavoritesTableController : UITableViewController
	{
		public List<TFMemberFavorites> favorites { get; set; }

		public string name = "";
		public string email = "";
		public string birthday = "";
		UITableView favoritesTable;
		LoadingOverlay _loadPop;

		public FavoritesTableController (IntPtr handle) : base (handle)
		{

		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			DashTabController dashTab = this.ParentViewController as DashTabController;
			Console.WriteLine ("Fav View Loaded");
			Console.WriteLine (String.Format ("Fav Tab : {0}, {1}, {2}", AppDelegate.tfAccount.name, AppDelegate.tfAccount.email, AppDelegate.tfAccount.birthday));
			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += (object sender, EventArgs e) => {
				Console.WriteLine ("Refresh Initiated");	
				RefreshFavoritesTable (AppDelegate.tfAccount.email);
				InvokeOnMainThread (() => {
					RefreshControl.EndRefreshing ();
				});
			};
		}

		public void RefreshFavoritesTable (string email)
		{
			Console.WriteLine ("RefreshFavoritesTable email: " + email);
			List<TFMerchants> tfmerchants = new List<TFMerchants> ();
			List<TFMemberFavorites> tffavorites = new List<TFMemberFavorites> ();
			List<TFMemberFavoritesEx> tffavoritesCompl = new List<TFMemberFavoritesEx> ();
			List<TFMerchants> tfmerchants2 = new List<TFMerchants> ();
			TFMemberFavoritesEx favEx = new TFMemberFavoritesEx ();
			InvokeOnMainThread (async () => {
				try {
					tffavorites = await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().Where (TFMemberFavorites => TFMemberFavorites.email == email).ToListAsync ();

					foreach (TFMemberFavorites fav in tffavorites) {
						Console.WriteLine ("Fav : " + fav.email + " , " + fav.company_id + " , " + fav.branch_id + " , " + fav.__deleted);
						tfmerchants2 = new List<TFMerchants> ();
						tfmerchants2 = await AppDelegate.MobileService.GetTable<TFMerchants> ()
							.Where (TFMerchants => TFMerchants.COMPANY_NO == fav.company_id && TFMerchants.BRANCH_NO == fav.branch_id && !TFMerchants._unlisted).Take (1).ToListAsync ();
						if (tfmerchants2.Any () && fav.__deleted.ToUpper ().Equals ("FALSE")) {
							favEx = new TFMemberFavoritesEx ();
							favEx.branch_id = fav.branch_id;
							favEx.company_id = fav.company_id;
							favEx.date_added = fav.date_added;
							favEx.date_updated = fav.date_updated;
							favEx.email = fav.email;
							favEx.id = fav.id;
							favEx.__deleted = fav.__deleted;
							favEx.branch_name = tfmerchants2.ToArray () [0].BRANCH_NAME;
							favEx.company_name = tfmerchants2.ToArray () [0].COMPANY_NAME;
							favEx.icon_image = tfmerchants2.ToArray () [0].icon_image;
							favEx.merchant = tfmerchants2.ToArray () [0];
							tffavoritesCompl.Add (favEx);
						}
					}

					favoritesTable = new UITableView {
						//Frame = new CoreGraphics.CGRect(0, NavigationController.NavigationBar.Bounds.Height, 
						//	View.Bounds.Width, NavigationController.NavigationBar.Bounds.Height - searchBar.Bounds.Height - 50),
						SeparatorStyle = UITableViewCellSeparatorStyle.None,
						Source = new FavoritesTableSource (tffavoritesCompl.ToArray (), this)
					};

					if (!tffavoritesCompl.Any ()) {
						new UIAlertView ("No Favorites!", "You currently have no favorites.", null, "OK", null).Show ();
					}

					this.TableView = favoritesTable;
				} catch (Exception e) {
					new UIAlertView ("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
				}
				//------LOADING Screen END----------------------
				this._loadPop.Hide ();
				//------LOADING Screen END----------------------
			});
		}


		public override void ViewWillAppear (bool animated)
		{
			Console.WriteLine ("Fav ViewWillAppear");
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			Console.WriteLine ("Fav ViewDidAppear");
			Console.WriteLine ("Fav view: " + AppDelegate.tfAccount.email + "/ " + AppDelegate.tfAccount.loggedIn);
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

			//call Refresh Table
			if (AppDelegate.tfAccount.loggedIn) {
				RefreshFavoritesTable (AppDelegate.tfAccount.email);
			} else {
				//------LOADING Screen END----------------------
				this._loadPop.Hide ();
				//------LOADING Screen END----------------------
				RefreshFavoritesTable ("");
				InvokeOnMainThread (() => {
					favoritesTable = new UITableView ();
				});
				new UIAlertView ("Profile Needed", "Please login in profile tab.", null, "OK", null).Show ();

			}
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if (favoritesTable != null) {
//				var length = MonoTouch.ObjCRuntime.Messaging.float_objc_msgSend (
//					TopLayoutGuide.Handle,
//					(new Selector("length")).Handle);
				favoritesTable.ContentInset = new UIEdgeInsets (64, 0, 0, 0);
			}
			
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
