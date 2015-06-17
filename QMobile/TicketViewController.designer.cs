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
	[Register ("TicketViewController")]
	partial class TicketViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView branchDetailsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel branchLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel companyLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel currentlyServingLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel currentlyServingValue { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton refreshButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ticketContainerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView ticketDetailsTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ticketNoLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ticketTitleLabel { get; set; }

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
			if (companyLabel != null) {
				companyLabel.Dispose ();
				companyLabel = null;
			}
			if (currentlyServingLabel != null) {
				currentlyServingLabel.Dispose ();
				currentlyServingLabel = null;
			}
			if (currentlyServingValue != null) {
				currentlyServingValue.Dispose ();
				currentlyServingValue = null;
			}
			if (refreshButton != null) {
				refreshButton.Dispose ();
				refreshButton = null;
			}
			if (ticketContainerView != null) {
				ticketContainerView.Dispose ();
				ticketContainerView = null;
			}
			if (ticketDetailsTable != null) {
				ticketDetailsTable.Dispose ();
				ticketDetailsTable = null;
			}
			if (ticketNoLabel != null) {
				ticketNoLabel.Dispose ();
				ticketNoLabel = null;
			}
			if (ticketTitleLabel != null) {
				ticketTitleLabel.Dispose ();
				ticketTitleLabel = null;
			}
		}
	}
}
