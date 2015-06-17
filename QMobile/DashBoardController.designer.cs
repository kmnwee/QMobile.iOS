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
	[Register ("DashBoardController")]
	partial class DashBoardController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton favoritesButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton myTicketsButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel NameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton searchButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISearchDisplayController searchDisplayController { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (favoritesButton != null) {
				favoritesButton.Dispose ();
				favoritesButton = null;
			}
			if (myTicketsButton != null) {
				myTicketsButton.Dispose ();
				myTicketsButton = null;
			}
			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}
			if (searchButton != null) {
				searchButton.Dispose ();
				searchButton = null;
			}
			if (searchDisplayController != null) {
				searchDisplayController.Dispose ();
				searchDisplayController = null;
			}
		}
	}
}
