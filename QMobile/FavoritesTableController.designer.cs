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
	[Register ("FavoritesTableController")]
	partial class FavoritesTableController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITabBarItem favoritesButtonTab { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView FavoritesTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (favoritesButtonTab != null) {
				favoritesButtonTab.Dispose ();
				favoritesButtonTab = null;
			}
			if (FavoritesTableView != null) {
				FavoritesTableView.Dispose ();
				FavoritesTableView = null;
			}
		}
	}
}
