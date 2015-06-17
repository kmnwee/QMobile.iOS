using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using CoreLocation;
using MapKit;
using CoreGraphics;
using System.Linq;

namespace QMobile
{
	partial class CompanyViewController : UIViewController
	{
		public List<TFMerchants> branchList;
		public string companyName;
		public int companyId;
		CLLocationManager locationManager;
		LoadingOverlay _loadPop;
		bool userLocationFirstUpdate = true;

		public CompanyViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			companyLabel.Text = "Branches";
			;
			branchLabel.Text = companyName;

			//Use Apple Map kit
			locationManager = new CLLocationManager ();
			locationManager.RequestWhenInUseAuthorization ();
			mapView.MapType = MKMapType.Standard;
			mapView.ShowsUserLocation = true;

			InvokeOnMainThread (() => {
				this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
			});

			//GET ALL BRANCHES
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
				branchList = new List<TFMerchants> ();
				try {
					branchList = await AppDelegate.MobileService.GetTable<TFMerchants> ().Take (500)
						.Where (TFMerchants => TFMerchants.COMPANY_NO == companyId && !TFMerchants._unlisted).OrderBy (TFMerchants => TFMerchants.BRANCH_NAME).ToListAsync ();
					branchListTable.Source = new CompanyTableSource (branchList.ToArray (), this);
					branchListTable.ReloadData ();
				} catch (Exception e) {
					new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
				}
				//------LOADING Screen END----------------------
				this._loadPop.Hide ();
				//------LOADING Screen END----------------------
			});

			//mapView.DidUpdateUserLocation = null;
			mapView.DidUpdateUserLocation += (sender, e) => {
				if (mapView.UserLocation != null) {
//					CLLocationCoordinate2D coordsCenter = new CLLocationCoordinate2D ((merchantLocation.Coordinate.Latitude + mapView.UserLocation.Coordinate.Latitude) / 2, 
//						                                      (merchantLocation.Coordinate.Longitude + mapView.UserLocation.Coordinate.Longitude) / 2);
//					CLLocationCoordinate2D coords = mapView.UserLocation.Coordinate;
//					//MKCoordinateSpan span = new MKCoordinateSpan(MilesToLatitudeDegrees(1), MilesToLongitudeDegrees(1, coordsCenter.Latitude));
//					MKCoordinateSpan span = new MKCoordinateSpan (Math.Abs (coordsCenter.Latitude - merchantLocation.Coordinate.Latitude) * 3.5, Math.Abs (coordsCenter.Longitude - merchantLocation.Coordinate.Longitude) * 3.5);
//					MKCoordinateSpan spanMax = new MKCoordinateSpan (180.0, 180.0);
//
//					CLLocation coordsCenterLoc = new CLLocation ((merchantLocation.Coordinate.Latitude + mapView.UserLocation.Coordinate.Latitude) / 2, 
//						                             (merchantLocation.Coordinate.Longitude + mapView.UserLocation.Coordinate.Longitude) / 2);
//					CLLocation coordsMerchantLoc = new CLLocation (merchantLocation.Coordinate.Latitude, merchantLocation.Coordinate.Longitude);
					CLLocation coordsUserLoc = new CLLocation (mapView.UserLocation.Coordinate.Latitude, mapView.UserLocation.Coordinate.Longitude);
//
//					double distance = coordsCenterLoc.DistanceFrom (coordsMerchantLoc);
//					double distanceDisplay = coordsUserLoc.DistanceFrom (coordsMerchantLoc);
					//distanceView.Text =  (Math.Ceiling((distanceDisplay/1000)*100) / 100.0) + " kilometers";
					//MKCoordinateRegion reg = MKCoordinateRegion.FromDistance(coordsCenter, distance*2, distance*2);
//					try {
//						Console.WriteLine ("OK, Distance = " + distanceDisplay / 1000 + " kilometers.");
//						mapView.Region = MKCoordinateRegion.FromDistance (coordsCenter, distance * 2, distance * 2);
//					} catch (Exception exx) {
//						Console.WriteLine ("Too Far, Distance = " + distanceDisplay / 1000 + " kilometers.");
//						mapView.Region = MKCoordinateRegion.FromDistance (merchantLocation.Coordinate, 1, 1);
//					}
					//mapView.Region = new MKCoordinateRegion(coordsCenter, spanMax);

					if (userLocationFirstUpdate) {
						InvokeOnMainThread (async () => {
							branchList = new List<TFMerchants> ();
							TFMerchants newMerchant = new TFMerchants ();
							MKPointAnnotation merchantLocation = new MKPointAnnotation ();
							List<TFMerchants> newBranchList = new List<TFMerchants> ();
							try {
								branchList = await AppDelegate.MobileService.GetTable<TFMerchants> ().Take (500)
									.Where (TFMerchants => TFMerchants.COMPANY_NO == companyId && !TFMerchants._unlisted).OrderBy (TFMerchants => TFMerchants.BRANCH_NAME).ToListAsync ();

								foreach (TFMerchants m in branchList) {
									newMerchant = new TFMerchants ();
									newMerchant = m;
									newMerchant.distance = coordsUserLoc.DistanceFrom (new CLLocation (Convert.ToDouble (m.latitude), Convert.ToDouble (m.longitude)));
									newBranchList.Add (newMerchant);

									merchantLocation = new MKPointAnnotation ();
									merchantLocation.Title = m.BRANCH_NAME;
									merchantLocation.SetCoordinate (new CLLocationCoordinate2D (Convert.ToDouble (m.latitude), Convert.ToDouble (m.longitude)));

									mapView.AddAnnotation (merchantLocation);
								}

								newBranchList = newBranchList.OrderBy (m => m.distance).ToList ();
								branchListTable.Source = new CompanyTableSource (newBranchList.ToArray (), this);
								branchListTable.ReloadData ();

								CLLocation coordsCenterLoc = new CLLocation ();
								CLLocation coordsMerchantLoc = new CLLocation ();
								CLLocationCoordinate2D coordsCenter = new CLLocationCoordinate2D();
								double distance = 0;
								double latitude = 0;
								double longitude = 0;
								if (newBranchList.Count < 4) {
									Console.WriteLine (" < 4");
									latitude = Convert.ToDouble(newBranchList.Last().latitude);
									longitude = Convert.ToDouble(newBranchList.Last().longitude);
								} else {
									Console.WriteLine (" >= 4");
									latitude = Convert.ToDouble(newBranchList.ToArray()[3].latitude);
									longitude = Convert.ToDouble(newBranchList.ToArray()[3].longitude);
								}

								coordsCenter = new CLLocationCoordinate2D ((latitude + mapView.UserLocation.Coordinate.Latitude) /2, (longitude + mapView.UserLocation.Coordinate.Longitude)/2);
								coordsCenterLoc = new CLLocation ((latitude + mapView.UserLocation.Coordinate.Latitude)/2, (longitude + mapView.UserLocation.Coordinate.Longitude)/2);
								coordsMerchantLoc = new CLLocation (latitude, longitude);
								distance = coordsCenterLoc.DistanceFrom (coordsMerchantLoc);
								double distanceDisplay = coordsUserLoc.DistanceFrom (coordsMerchantLoc);
								try {
									Console.WriteLine ("OK, Distance = " + distanceDisplay / 1000 + " kilometers.");
									//mapView.Region = MKCoordinateRegion.FromDistance (mapView.UserLocation.Coordinate, distance*3, distance*3);
									mapView.SetRegion(MKCoordinateRegion.FromDistance (mapView.UserLocation.Coordinate, distance*3, distance*3), true);
								} catch (Exception exx) {
									Console.WriteLine ("Too Far, Distance = " + distanceDisplay / 1000 + " kilometers.");
									//mapView.Region = MKCoordinateRegion.FromDistance (mapView.UserLocation.Coordinate, 1, 1);
									mapView.SetRegion(MKCoordinateRegion.FromDistance (mapView.UserLocation.Coordinate, 1, 1),true);
								}
							} catch (Exception exx) {
								new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
								Console.WriteLine(exx.Message);
							}
						});
						userLocationFirstUpdate = false;
					}


				}
			};
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
