using System;
using UIKit;
using Foundation;
using Xamarin.Forms;
using ImageCircle.Forms.Plugin.iOS;
using System.Drawing;
using Microsoft.WindowsAzure.MobileServices;
using SWTableViewCell;
using System.Collections.Generic;
using CoreLocation;
using System.Runtime.InteropServices;
using System.Linq;
using ToastIOS;

namespace QMobile
{
	public class FavoritesTableSource : UITableViewSource
	{
		static TFMemberFavoritesEx[] tableItems;
		string cellId = "TableCell";
		private static UIViewController viewControllerLocal;
		static UITableView tableviewlocal;
		//static AppointmentViewController appointmentView;


		public FavoritesTableSource (TFMemberFavoritesEx[] items, UIViewController viewController)
		{
			tableItems = items;
			viewControllerLocal = viewController;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		//		public override string TitleForHeader(UITableView tableView, nint section)
		//		{
		//			return "Bookmarked Stores";
		//		}

		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//new UIAlertView("Alert", tableItems[indexPath.Row].company_id + " | " + tableItems[indexPath.Row].branch_id, null, "Got It!", null).Show ();
			tableView.DeselectRow (indexPath, true);

			BranchViewController branchView = viewControllerLocal.Storyboard.InstantiateViewController ("BranchViewController") as BranchViewController;
			branchView.merchant = tableItems [indexPath.Row].merchant;

			InvokeOnMainThread (() => {
				viewControllerLocal.NavigationController.PushViewController (branchView, true);
			});
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			List<UIButton> leftButtons;
			List<UIButton> rightButtons;
			tableviewlocal = tableView;
			SWTableViewCell.SWTableViewCell cell = tableView.DequeueReusableCell (cellId) as SWTableViewCell.SWTableViewCell;
			if (cell == null) {
				//cell = new SWTableViewCell.SWTableViewCell (UITableViewCellStyle.Subtitle, cellId);
				cell = new SWTableViewCell.SWTableViewCell (UITableViewCellStyle.Subtitle, cellId);
				try {
					Console.WriteLine ("Out");
					UIButton quickReserve = new UIButton ();
					//quickReserve.SetTitle ("Res", UIControlState.Normal);
					//quickReserve.SizeThatFits(new CoreGraphics.CGSize(50f,50f));
					quickReserve.SetImage (FeaturedTableSource.MaxResizeImage (UIImage.FromBundle ("action/ic_local_offer_white_24dp.png"), 30, 30), UIControlState.Normal);
					//quickReserve.Frame = new RectangleF(0, 0, 60, 60);
					quickReserve.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					quickReserve.BackgroundColor = UIColor.Orange;
					//
					UIButton removeFromFav = new UIButton ();
					//removeFromFav.SetTitle ("Rem", UIControlState.Normal);
					//removeFromFav.SizeThatFits(new CoreGraphics.CGSize(50f,50f));
					removeFromFav.SetImage (FeaturedTableSource.MaxResizeImage (UIImage.FromBundle ("action/ic_clear_white_24dp.png"), 30, 30), UIControlState.Normal);
					//removeFromFav.Frame = new RectangleF(0, 0, 60, 60);
					//removeFromFav.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
					removeFromFav.BackgroundColor = UIColor.Red;

					leftButtons = new List<UIButton> ();
					rightButtons = new List<UIButton> ();
					//leftButtons.Add (quickReserve);
					leftButtons.Add (removeFromFav);

					//
					cell.LeftUtilityButtons = leftButtons.ToArray ();
					//cell.RightUtilityButtons = rightButtons.ToArray();
					SwipeCellDelegate swdel = new SwipeCellDelegate ();
					cell.Delegate = swdel;

				} catch (Exception e) {
					Console.WriteLine (e.Message);
				}
			}

//			if (cell == null)
//				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellId);

						
			cell.TextLabel.Text = Convert.ToString (tableItems [indexPath.Row].branch_name);
			cell.DetailTextLabel.Text = Convert.ToString (tableItems [indexPath.Row].company_name);
			InvokeOnMainThread (async () => {
				long ticketsRes = 0;
				long ticketsAppt = 0;
				try {
					ticketsRes = (await AppDelegate.MobileService.GetTable<TFOLReservation> ().Take (0).IncludeTotalCount ()
					.Where (TFOLReservation => TFOLReservation.mobile_userid == tableItems [indexPath.Row].email
					&& TFOLReservation.company_id == tableItems [indexPath.Row].company_id
					&& TFOLReservation.branch_id == tableItems [indexPath.Row].branch_id)
					.ToListAsync () as ITotalCountProvider).TotalCount;
					ticketsAppt = (await AppDelegate.MobileService.GetTable<TFScheduledReservation> ().Take (0).IncludeTotalCount ()
					.Where (TFScheduledReservation => TFScheduledReservation.email == tableItems [indexPath.Row].email
					&& TFScheduledReservation.company_id == tableItems [indexPath.Row].company_id
					&& TFScheduledReservation.branch_id == tableItems [indexPath.Row].branch_id)
					.ToListAsync () as ITotalCountProvider).TotalCount;
					cell.DetailTextLabel.Text += String.Format (" | You had {0} ticket/s here", ticketsRes + ticketsAppt);
				} catch (Exception e) {
					Console.WriteLine ("Problem loading Tickets Count...");
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			});
			return cell;
		}


		public override nfloat GetHeightForRow (UITableView tableview, Foundation.NSIndexPath indexPath)
		{
			return 60;
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		static UIImage RounderCorners (UIImage image, float width, float radius)
		{
			UIGraphics.BeginImageContext (new SizeF (width, width));
			var c = UIGraphics.GetCurrentContext ();

			c.BeginPath ();
			c.MoveTo (width, width / 2);
			c.AddArcToPoint (width, width, width / 2, width, radius);
			c.AddArcToPoint (0, width, 0, width / 2, radius);
			c.AddArcToPoint (0, 0, width / 2, 0, radius);
			c.AddArcToPoint (width, 0, width, width / 2, radius);
			c.ClosePath ();
			c.Clip ();

			image.Draw (new PointF (0, 0));
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public static void quickReserve (NSIndexPath cellIndexPath)
		{
			Console.WriteLine ("Quick Reserve was called");
//			AppointmentViewController appointmentView = new AppointmentViewController (Marshal.AllocHGlobal (0));
//				appointmentView = viewControllerLocal.Storyboard.InstantiateViewController ("AppointmentViewController") as AppointmentViewController;
//
//
//			if (!tableItems [cellIndexPath.Row].merchant.edition.Equals ("DEMO") && !tableItems [cellIndexPath.Row].merchant.edition.Equals ("DESKTOP")) {
//				appointmentView.merchant = tableItems [cellIndexPath.Row].merchant;
//				appointmentView.action = "RESERVATION";//tableItems [cellIndexPath.Row].action;
//				appointmentView.userLocation = new CLLocationCoordinate2D (0, 0);
//				InvokeOnMainThread (() => {
//					viewControllerLocal.NavigationController.PushViewController (appointmentView, true);
//				});
//
//			} else {
//				new UIAlertView ("Alert", "This site is running QApps " + tableItems [cellIndexPath.Row].merchant.edition + " Edition! Mobile Transactions are not yet supported.", null, "Got It!", null).Show ();
//
//			}
//
			
		}


		public class SwipeCellDelegate : SWTableViewCellDelegate
		{
			public override void DidTriggerRightUtilityButton (SWTableViewCell.SWTableViewCell cell, nint index)
			{
				Console.WriteLine ("Right");

				NSIndexPath cellIndexPath = tableviewlocal.IndexPathForCell (cell);

				switch (index) {
				case 0:
					Console.WriteLine ("Reserve");
					new UIAlertView ("Quick Reserve", "You are about reserve " + tableItems [cellIndexPath.Row].company_name + " | " + tableItems [cellIndexPath.Row].branch_name, null, "Got It!", null).Show ();
					quickReserve (cellIndexPath);
					break;
				default:
					Console.WriteLine ("None");
					break;
				}
			}

			public override void DidTriggerLeftUtilityButton (SWTableViewCell.SWTableViewCell cell, nint index)
			{
				//base.DidTriggerLeftUtilityButton (cell, index);
				NSIndexPath cellIndexPath = tableviewlocal.IndexPathForCell (cell);
				Console.WriteLine ("Left");
				switch (index) {
//				case 0:
//					Console.WriteLine ("Reserve");
//					new UIAlertView ("Quick Reserve", "You are about reserve " + tableItems [cellIndexPath.Row].company_name + " | " + tableItems [cellIndexPath.Row].branch_name, null, "Got It!", null).Show ();
//					break;
				case 0:
					Console.WriteLine ("Remove");
					//new UIAlertView ("Remove from Favorites?", "You are about remove " + tableItems [cellIndexPath.Row].company_name + " | " + tableItems [cellIndexPath.Row].branch_name + " from favorites.", null, "Got It!", null).Show ();

					var delFromFav = new UIAlertView ("Remove from Favorites?", "You are about to remove this branch from your favorites", null, "Continue?", new string[] { "Cancel" });
					delFromFav.Clicked += (s, b) => {
						if (b.ButtonIndex.ToString ().Equals ("0")) {
							Console.WriteLine ("del from fav");
							InvokeOnMainThread (async () => {
								Console.WriteLine ("Invoke del fav shit! " + tableItems [cellIndexPath.Row].merchant.COMPANY_NO + ", " + tableItems [cellIndexPath.Row].merchant.BRANCH_NO);
								try {
									var favList = await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().Take (1)
										.Where (fav => fav.email == AppDelegate.tfAccount.email
									              && fav.company_id == tableItems [cellIndexPath.Row].merchant.COMPANY_NO
									              && fav.branch_id == tableItems [cellIndexPath.Row].merchant.BRANCH_NO
									              && fav.__deleted == "FALSE")
										.ToListAsync ();

									List<TFMemberFavorites> favoriteList = new List<TFMemberFavorites> ();
									favoriteList = favList;

									if (favoriteList.Any ()) { 
										favoriteList [0].__deleted = "TRUE";
										favoriteList [0].date_updated = DateTime.Now.ToString ();
										await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().UpdateAsync (favoriteList [0]);
//										isFavorite = false;
//										favButton.TintColor = UIColor.White;
										(viewControllerLocal as FavoritesTableController).tffavoritesCompl.RemoveAt (cellIndexPath.Row);

										Toast.MakeText(tableItems[cellIndexPath.Row].company_name + " - " + tableItems[cellIndexPath.Row].branch_name + " was removed from favorites!")
											.SetDuration(3000)
											.Show();
										Console.WriteLine ("Initial Length: " + tableItems.Length);
										foreach (TFMemberFavoritesEx f in tableItems) {
											Console.WriteLine (f.branch_name + " - " + f.company_name + " - " + f.email);
											Console.WriteLine (f.branch_id + " - " + f.company_id);
										}
										tableItems = (viewControllerLocal as FavoritesTableController).tffavoritesCompl.ToArray ();//tableItems.Where (ti => ti.branch_id != tableItems[cellIndexPath.Row].branch_id && ti.company_id != tableItems[cellIndexPath.Row].company_id).ToArray ();
										Console.WriteLine ("Post Length: " + tableItems.Length);
										foreach (TFMemberFavoritesEx f in tableItems) {
											Console.WriteLine (f.branch_name + " - " + f.company_name + " - " + f.email);
										}

										tableviewlocal.DeleteRows (new NSIndexPath[] { cellIndexPath }, UITableViewRowAnimation.Right);
									} else {		
										Console.WriteLine ("Branch is not really a favorite");
									}
								} catch (Exception eeee) {
									Console.WriteLine (eeee.Message + " - " + eeee.StackTrace);
								}

							});
						} else if (b.ButtonIndex.ToString ().Equals ("1")) {
							Console.WriteLine ("cancel del from fav");
						}
					};
					delFromFav.Show ();

					break;
				default:
					Console.WriteLine ("None");
					break;
				}

			}
		}
	}
}

