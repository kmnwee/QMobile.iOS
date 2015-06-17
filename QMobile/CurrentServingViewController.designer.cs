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
	[Register ("CurrentServingViewController")]
	partial class CurrentServingViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView branchDetailsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel branchNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel companyLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView currentServingTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView lastTicketCalledContainer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lastTicketNoLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton refreshButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ticketTitleLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (branchDetailsView != null) {
				branchDetailsView.Dispose ();
				branchDetailsView = null;
			}
			if (branchNameLabel != null) {
				branchNameLabel.Dispose ();
				branchNameLabel = null;
			}
			if (companyLabel != null) {
				companyLabel.Dispose ();
				companyLabel = null;
			}
			if (currentServingTable != null) {
				currentServingTable.Dispose ();
				currentServingTable = null;
			}
			if (lastTicketCalledContainer != null) {
				lastTicketCalledContainer.Dispose ();
				lastTicketCalledContainer = null;
			}
			if (lastTicketNoLabel != null) {
				lastTicketNoLabel.Dispose ();
				lastTicketNoLabel = null;
			}
			if (refreshButton != null) {
				refreshButton.Dispose ();
				refreshButton = null;
			}
			if (ticketTitleLabel != null) {
				ticketTitleLabel.Dispose ();
				ticketTitleLabel = null;
			}
		}
	}
}
