using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using Google.Maps;
using CoreLocation;
using CoreGraphics;
using MapKit;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;

namespace QMobile
{
	partial class BranchViewController : UIViewController
	{
		public TFMerchants merchant;
		public CLLocationCoordinate2D coords;
		public CLLocationCoordinate2D coords2;

		TFMemberFavorites memberFavorite;
		List<TFBranchOption> options;
		CLLocationManager locationManager;
		bool firstLocationUpdate;
		bool isFavorite;
		MKMapView map;
		LoadingOverlay _loadPop;



		public BranchViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			companyLabel.Text = merchant.COMPANY_NAME;
			branchLabel.Text = merchant.BRANCH_NAME;

			InvokeOnMainThread (async () => {
				//check if branch if fav:
				long favCount = 0;
				try {
					favCount = (await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().Take (0).IncludeTotalCount ()
					.Where (TFMemberFavorites => TFMemberFavorites.email == AppDelegate.tfAccount.email
					&& TFMemberFavorites.company_id == merchant.COMPANY_NO
					&& TFMemberFavorites.branch_id == merchant.BRANCH_NO
					&& TFMemberFavorites.__deleted == "FALSE")
					.ToListAsync () as ITotalCountProvider).TotalCount;
					if (favCount > 0) {
						isFavorite = true;
						favButton.TintColor = UIColor.Yellow;
						Console.WriteLine ("This branch is fav!");
					} else {
						isFavorite = false;
						favButton.TintColor = UIColor.White;
						Console.WriteLine ("This branch is NOT fav!");
					}
				} catch (Exception e) {
					Console.WriteLine ("Problem loading Favorites...");
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
				}
			});

			favButton.TouchUpInside += async (object sender, EventArgs e) => {
				
				if (isFavorite) {
					var delFromFav = new UIAlertView ("Remove from Favorites?", "Are you sure you want to remove this branch from Favorites?", null, "Yes", new string[] { "Cancel" });
					delFromFav.Clicked += (s, b) => {
						if (b.ButtonIndex.ToString ().Equals ("0")) {
							Console.WriteLine ("del from fav");
							InvokeOnMainThread (async () => {
								Console.WriteLine ("Invoke del fav shit!");
								try {
									var favList = await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().Take (1)
									.Where (fav => fav.email == AppDelegate.tfAccount.email
									              && fav.company_id == merchant.COMPANY_NO
									              && fav.branch_id == merchant.BRANCH_NO
									              && fav.__deleted == "FALSE")
									.ToListAsync ();

									List<TFMemberFavorites> favoriteList = new List<TFMemberFavorites> ();
									favoriteList = favList;
									
									if (favoriteList.Any ()) { 
										favoriteList [0].__deleted = "TRUE";
										favoriteList [0].date_updated = DateTime.Now.ToString ();
										await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().UpdateAsync (favoriteList [0]);
										isFavorite = false;
										favButton.TintColor = UIColor.White;
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
				} else {
					var addToFav = new UIAlertView ("Add to Favorites?", "Are you sure you want to add this branch as a Favorite?", null, "Yes", new string[] { "Cancel" });
					addToFav.Clicked += (s, b) => {
						if (b.ButtonIndex.ToString ().Equals ("0")) {
							Console.WriteLine ("Add to fav");
							InvokeOnMainThread (async () => {
								Console.WriteLine ("Invoke add to fav shit!");
								try {
									memberFavorite = new TFMemberFavorites ();
									memberFavorite.branch_id = merchant.BRANCH_NO;
									memberFavorite.company_id = merchant.COMPANY_NO;
									memberFavorite.email = AppDelegate.tfAccount.email;
									memberFavorite.date_added = DateTime.Now.ToString ();
									memberFavorite.date_updated = DateTime.Now.ToString ();
									memberFavorite.__deleted = "FALSE";

									await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().InsertAsync (memberFavorite);
									favButton.TintColor = UIColor.Yellow;
									isFavorite = true;
								} catch (Exception eeee) {
									Console.WriteLine (eeee.Message + " - " + eeee.StackTrace);
								}
							});
						} else if (b.ButtonIndex.ToString ().Equals ("1")) {
							Console.WriteLine ("cancel add to fav");
						}
					};
					addToFav.Show ();
				}


			};

			coords = new CLLocationCoordinate2D ();
//			if(!String.IsNullOrEmpty(merchant.address))
//				infoLabel.Text = merchant.address;
//			else infoLabel.Text = merchant.contact_no;
			//merchantImage.Image = FromURL (merchant.icon_image);

			//Use Apple Map kit
			var version8 = new Version (8, 0);
			locationManager = new CLLocationManager ();
			if (new Version (UIDevice.CurrentDevice.SystemVersion) >= version8)
				locationManager.RequestWhenInUseAuthorization ();
			
			mapView.MapType = MKMapType.Standard;
			mapView.ShowsUserLocation = true;
			//mapView.SetCenterCoordinate

			var merchantLocation = new MKPointAnnotation ();
			merchantLocation.Title = merchant.BRANCH_NAME;
			merchantLocation.SetCoordinate (new CLLocationCoordinate2D (Convert.ToDouble (merchant.latitude), Convert.ToDouble (merchant.longitude)));
			mapView.AddAnnotation (merchantLocation);

			InvokeOnMainThread (() => {
//				this.NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
				//this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
				this.NavigationItem.TitleView = new UIImageView (FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("LogoWithOutBackground.png"), 50, 50));
			});

			//Set Options Table
			TFBranchOption option1 = new TFBranchOption ();
			TFBranchOption option2 = new TFBranchOption ();
			TFBranchOption option3 = new TFBranchOption ();
			options = new List<TFBranchOption> ();

			if (merchant.regReserve_flag == 1) {
				option1.title = "Get Ticket";
				option1.action = "RESERVATION";
				option1.info = "Reserve a ticket for today";
				option1.image = "";
				option1.merchant = merchant;
				options.Add (option1);
			}

			if (merchant.schedReserve_flag == 1) {
				option2.title = "Schedule Appointment";
				option2.action = "APPOINTMENT";
				option2.info = "Schedule an appointment for upcoming days";
				option2.image = "";
				option2.merchant = merchant;
				options.Add (option2);
			}

			option3.title = "View Store Status";
			option3.action = "CURRENTSERVING";
			option3.info = "See a list of tickets that are currently being served";
			option3.image = "";
			option3.merchant = merchant;
			options.Add (option3); 

			InvokeOnMainThread (() => {
				BranchViewOptionsTable.TableFooterView = new UIView (CGRect.Empty);
				BranchViewOptionsTable.Source = new BranchViewOptionsSource (options.ToArray (), this);
				BranchViewOptionsTable.ReloadData ();
			});
			//

			//mapView.DidUpdateUserLocation -= after
			mapView.DidUpdateUserLocation += (sender, e) => {
				if (mapView.UserLocation != null) {
					CLLocationCoordinate2D coordsCenter = new CLLocationCoordinate2D ((merchantLocation.Coordinate.Latitude + mapView.UserLocation.Coordinate.Latitude) / 2, 
						                                      (merchantLocation.Coordinate.Longitude + mapView.UserLocation.Coordinate.Longitude) / 2);
					coords = mapView.UserLocation.Coordinate;
					//MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(1), MilesToLongitudeDegrees(1, coordsCenter.Latitude));
					MKCoordinateSpan span = new MKCoordinateSpan (Math.Abs (coordsCenter.Latitude - merchantLocation.Coordinate.Latitude) * 3.5, Math.Abs (coordsCenter.Longitude - merchantLocation.Coordinate.Longitude) * 3.5);
					MKCoordinateSpan spanMax = new MKCoordinateSpan (180.0, 180.0);

					CLLocation coordsCenterLoc = new CLLocation ((merchantLocation.Coordinate.Latitude + mapView.UserLocation.Coordinate.Latitude) / 2, 
						                             (merchantLocation.Coordinate.Longitude + mapView.UserLocation.Coordinate.Longitude) / 2);
					CLLocation coordsMerchantLoc = new CLLocation (merchantLocation.Coordinate.Latitude, merchantLocation.Coordinate.Longitude);
					CLLocation coordsUserLoc = new CLLocation (mapView.UserLocation.Coordinate.Latitude, mapView.UserLocation.Coordinate.Longitude);

					double distance = coordsCenterLoc.DistanceFrom (coordsMerchantLoc);
					double distanceDisplay = coordsUserLoc.DistanceFrom (coordsMerchantLoc);
					distanceView.Text = (Math.Ceiling ((distanceDisplay / 1000) * 100) / 100.0) + " kilometers";
					//MKCoordinateRegion reg = MKCoordinateRegion.FromDistance(coordsCenter, distance*2, distance*2);
					try {
						Console.WriteLine ("OK, Distance = " + distanceDisplay / 1000 + " kilometers.");
						//mapView.Region = MKCoordinateRegion.FromDistance(coordsCenter, distance*2, distance*2);
						mapView.SetRegion (MKCoordinateRegion.FromDistance (coordsCenter, distance * 2, distance * 2), true);
					} catch (Exception exx) {
						Console.WriteLine ("Too Far, Distance = " + distanceDisplay / 1000 + " kilometers.");
						//mapView.Region = MKCoordinateRegion.FromDistance(merchantLocation.Coordinate, 1, 1);
						mapView.SetRegion (MKCoordinateRegion.FromDistance (merchantLocation.Coordinate, 1, 1), true);
					}
					//mapView.Region = new MKCoordinateRegion(coordsCenter, spanMax);


					//Create Origin and Dest Place Marks and Map Items to use for directions
					//Start at User Location
					var addressDict = new NSDictionary ();
					addressDict = null;
					var orignPlaceMark = new MKPlacemark (coords, addressDict);
					var sourceItem = new MKMapItem (orignPlaceMark);

					//End at Xamarin Cambridge Office
					var destPlaceMark = new MKPlacemark (merchantLocation.Coordinate, addressDict);
					var destItem = new MKMapItem (destPlaceMark);

					var request = new MKDirectionsRequest {
						Source = sourceItem,
						Destination = destItem,
						RequestsAlternateRoutes = true
					};

					var directions = new MKDirections (request);
					directions.CalculateDirections ((response, error) => {
						if (error != null) {
							Console.WriteLine (error.LocalizedDescription);
						} else {
							//Add each Polyline from route to map as overlay
							Console.WriteLine ("routes : " + response.Routes.Length);
							foreach (var route in response.Routes) {
								Console.WriteLine ("Route Distance : " + route.Distance);
								mapView.AddOverlay (route.Polyline);
							}
						}
					});
				}
			};

			mapView.OverlayRenderer += ((mapView, overlay) => {
				Console.WriteLine ("Polyline Renderer was called");
				if (overlay is MKPolyline) {
					var route = (MKPolyline)overlay;
					var renderer = new MKPolylineRenderer (route) { 
						StrokeColor = UIColor.Orange, 
						LineWidth = 5
					};
					return renderer;
				}
				return null;
			});
		}



		public double MilesToLatitudeDegrees (double miles)
		{
			double earthRadius = 3960.0; // in miles
			double radiansToDegrees = 180.0 / Math.PI;
			return (miles / earthRadius) * radiansToDegrees;
		}

		public double MilesToLongitudeDegrees (double miles, double atLatitude)
		{
			double earthRadius = 3960.0; // in miles
			double degreesToRadians = Math.PI / 180.0;
			double radiansToDegrees = 180.0 / Math.PI;
			// derive the earth's radius at that point in latitude
			double radiusAtLatitude = earthRadius * Math.Cos (atLatitude * degreesToRadians);
			return (miles / radiusAtLatitude) * radiansToDegrees;
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
			//this.mapMainView.RemoveObserver (this, new NSString ("myLocation"));
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			//myLoc.StopUpdatingLocation ();
			base.ViewDidDisappear (animated);
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		#endregion
	}



}
	