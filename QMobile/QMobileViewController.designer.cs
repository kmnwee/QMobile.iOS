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
	[Register ("QMobileViewController")]
	partial class QMobileViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton fbButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton gpButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView testTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (fbButton != null) {
				fbButton.Dispose ();
				fbButton = null;
			}
			if (gpButton != null) {
				gpButton.Dispose ();
				gpButton = null;
			}
			if (testTableView != null) {
				testTableView.Dispose ();
				testTableView = null;
			}
		}
	}
}
