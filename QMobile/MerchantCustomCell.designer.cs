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
	[Register ("MerchantCustomCell")]
	partial class MerchantCustomCell
	{
		[Outlet]
		UIKit.UILabel branchName { get; set; }

		[Outlet]
		UIKit.UILabel companyName { get; set; }

		[Outlet]
		UIKit.UIImageView imageHolder { get; set; }

		[Outlet]
		UIKit.UILabel info { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (branchName != null) {
				branchName.Dispose ();
				branchName = null;
			}
			if (companyName != null) {
				companyName.Dispose ();
				companyName = null;
			}
			if (imageHolder != null) {
				imageHolder.Dispose ();
				imageHolder = null;
			}
			if (info != null) {
				info.Dispose ();
				info = null;
			}
		}
	}
}
