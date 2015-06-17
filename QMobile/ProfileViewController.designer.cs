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
		UILabel appointmentCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel appointmentsLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel emailLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel favoritesCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel favoritesLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton logoutButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel notifCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel notifLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView profileImageView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel profileNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ticketCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ticketsLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (appointmentCount != null) {
				appointmentCount.Dispose ();
				appointmentCount = null;
			}
			if (appointmentsLabel != null) {
				appointmentsLabel.Dispose ();
				appointmentsLabel = null;
			}
			if (emailLabel != null) {
				emailLabel.Dispose ();
				emailLabel = null;
			}
			if (favoritesCount != null) {
				favoritesCount.Dispose ();
				favoritesCount = null;
			}
			if (favoritesLabel != null) {
				favoritesLabel.Dispose ();
				favoritesLabel = null;
			}
			if (logoutButton != null) {
				logoutButton.Dispose ();
				logoutButton = null;
			}
			if (notifCount != null) {
				notifCount.Dispose ();
				notifCount = null;
			}
			if (notifLabel != null) {
				notifLabel.Dispose ();
				notifLabel = null;
			}
			if (profileImageView != null) {
				profileImageView.Dispose ();
				profileImageView = null;
			}
			if (profileNameLabel != null) {
				profileNameLabel.Dispose ();
				profileNameLabel = null;
			}
			if (ticketCount != null) {
				ticketCount.Dispose ();
				ticketCount = null;
			}
			if (ticketsLabel != null) {
				ticketsLabel.Dispose ();
				ticketsLabel = null;
			}
		}
	}
}
