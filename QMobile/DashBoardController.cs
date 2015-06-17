using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;

namespace QMobile
{
	partial class DashBoardController : UIViewController
	{
		public string name = "";
		public string email = "";
		public string birthday = "";
		//UISearchController searchController;

		public DashBoardController (IntPtr handle) : base (handle)
		{

		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//searchBar.set
			NameLabel.Text = "Hi, " + name;
			List<TFMerchants> tfmerchants = new List<TFMerchants> ();

			favoritesButton.TouchUpInside += async (object sender, EventArgs e) => {
				//NameLabel.Text = "Hi, " + name + " fav clicked";
				//CurrentPlatform.Init();

				UITableView favoritesTable;
				List<TFMemberFavorites> tffavorites = new List<TFMemberFavorites> ();
				List<TFMemberFavoritesEx> tffavoritesCompl = new List<TFMemberFavoritesEx> ();
				List<TFMerchants> tfmerchants2 = new List<TFMerchants> ();
				TFMemberFavoritesEx favEx = new TFMemberFavoritesEx ();

				try {
					tffavorites = await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().Where (TFMemberFavorites => TFMemberFavorites.email == email).ToListAsync ();

					foreach (TFMemberFavorites fav in tffavorites) {
						tfmerchants2 = await AppDelegate.MobileService.GetTable<TFMerchants> ()
						.Where (TFMerchants => TFMerchants.COMPANY_NO == fav.company_id && TFMerchants.BRANCH_NO == fav.branch_id).Take (1).ToListAsync ();
						//Console.WriteLine("Member Fav: " + fav.company_name + "; " + fav.branch_name + "; " + fav.__deleted);
						if (tfmerchants2.Count > 0 && fav.__deleted.ToUpper ().Equals ("FALSE")) {
							favEx = new TFMemberFavoritesEx ();
							favEx.branch_id = fav.branch_id;
							favEx.company_id = fav.company_id;
							favEx.date_added = fav.date_added;
							favEx.date_updated = fav.date_updated;
							favEx.email = fav.email;
							favEx.id = fav.id;
							favEx.__deleted = fav.__deleted;
							favEx.company_name = tfmerchants2.ToArray () [0].COMPANY_NAME;
							favEx.branch_name = tfmerchants2.ToArray () [0].BRANCH_NAME;
							favEx.icon_image = tfmerchants2.ToArray () [0].icon_image;
							favEx.merchant = tfmerchants2.ToArray () [0];
							tffavoritesCompl.Add (favEx);
							Console.WriteLine ("Member Fav Added: " + favEx.company_name + "; " + favEx.branch_name);
						}
					}
				} catch (Exception ef) {
					Console.WriteLine ("Problem loading Favorites...");
					Console.WriteLine (ef.Message);
					Console.WriteLine (ef.StackTrace);
				}

				favoritesTable = new UITableView {
					//Frame = new CoreGraphics.CGRect(0, NavigationController.NavigationBar.Bounds.Height, 
					//	View.Bounds.Width, NavigationController.NavigationBar.Bounds.Height - searchBar.Bounds.Height - 50),
					Source = new FavoritesTableSource (tffavoritesCompl.ToArray (), this)
				};

				FavoritesTableController favorites = this.Storyboard.InstantiateViewController ("FavoritesTableController") as FavoritesTableController;
				favorites.TableView = favoritesTable;
				InvokeOnMainThread (() => NavigationController.PushViewController (favorites, true));

			};

			myTicketsButton.TouchUpInside += (object sender, EventArgs e) => {
				NameLabel.Text = "Hi, " + name + " tix clicked";
			};



			searchButton.TouchUpInside += /*async*/ (object sender, EventArgs e) => {
				//NameLabel.Text = "Hi, " + name + " search text: " + e.SearchText;

				//tfmerchants = await AppDelegate.MobileService.GetTable<TFMerchants>().ToListAsync();

//				foreach(TFMerchants tfm in tfmerchants)
//				{
//					Console.Out.WriteLine(tfm.COMPANY_NAME + " | " + tfm.BRANCH_NAME);
//				}

				SearchViewController searchViewController = this.Storyboard.InstantiateViewController ("SearchViewController") as SearchViewController;
				//searchResultsController.searchResultsTab = searchResultsTable;
				//searchViewController.TFMerchants = tfmerchants;
				InvokeOnMainThread (() => NavigationController.PushViewController (searchViewController, true));
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


