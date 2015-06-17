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
	[Register ("BranchViewController")]
	partial class BranchViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView branchDetailsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel branchLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView BranchViewOptionsTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel companyLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel distanceView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton favButton { get; set; }

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
			if (BranchViewOptionsTable != null) {
				BranchViewOptionsTable.Dispose ();
				BranchViewOptionsTable = null;
			}
			if (companyLabel != null) {
				companyLabel.Dispose ();
				companyLabel = null;
			}
			if (distanceView != null) {
				distanceView.Dispose ();
				distanceView = null;
			}
			if (favButton != null) {
				favButton.Dispose ();
				favButton = null;
			}
			if (mapView != null) {
				mapView.Dispose ();
				mapView = null;
			}
		}
	}
}
