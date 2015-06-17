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
	[Register ("SearchViewController")]
	partial class SearchViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView resultsTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISearchBar searchBarMain { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITabBarItem searchButtonTab { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (resultsTable != null) {
				resultsTable.Dispose ();
				resultsTable = null;
			}
			if (searchBarMain != null) {
				searchBarMain.Dispose ();
				searchBarMain = null;
			}
			if (searchButtonTab != null) {
				searchButtonTab.Dispose ();
				searchButtonTab = null;
			}
		}
	}
}
