// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace QMobile
{
	[Register ("CompanyViewController")]
	partial class CompanyViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView branchDetailsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel branchLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView branchListTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel companyLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MapKit.MKMapView mapView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (branchDetailsView != null) {
				branchDetailsView.Dispose ();
				branchDetailsView = null;
			}
			if (branchLabel != null) {
				branchLabel.Dispose ();
				branchLabel = null;
			}
			if (branchListTable != null) {
				branchListTable.Dispose ();
				branchListTable = null;
			}
			if (companyLabel != null) {
				companyLabel.Dispose ();
				companyLabel = null;
			}
			if (mapView != null) {
				mapView.Dispose ();
				mapView = null;
			}
		}
	}
}
