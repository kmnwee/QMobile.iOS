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
	[Register ("ProfileViewController")]
	partial class ProfileViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView coverPhoto { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel emailLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView profileImageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel profileNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView profileOptionsTable { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (coverPhoto != null) {
				coverPhoto.Dispose ();
				coverPhoto = null;
			}
			if (emailLabel != null) {
				emailLabel.Dispose ();
				emailLabel = null;
			}
			if (profileImageView != null) {
				profileImageView.Dispose ();
				profileImageView = null;
			}
			if (profileNameLabel != null) {
				profileNameLabel.Dispose ();
				profileNameLabel = null;
			}
			if (profileOptionsTable != null) {
				profileOptionsTable.Dispose ();
				profileOptionsTable = null;
			}
		}
	}
}
