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
	[Register ("AppointmentViewController")]
	partial class AppointmentViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView appointmentOptionsTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView branchDetailsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel branchLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel companyLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (appointmentOptionsTable != null) {
				appointmentOptionsTable.Dispose ();
				appointmentOptionsTable = null;
			}
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
		}
	}
}
