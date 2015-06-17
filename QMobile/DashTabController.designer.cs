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
	[Register ("DashTabController")]
	partial class DashTabController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITabBar DashTabBar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (DashTabBar != null) {
				DashTabBar.Dispose ();
				DashTabBar = null;
			}
		}
	}
}
