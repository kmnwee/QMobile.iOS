// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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
		UIKit.UILabel info { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (companyName != null) {
				companyName.Dispose ();
				companyName = null;
			}

			if (branchName != null) {
				branchName.Dispose ();
				branchName = null;
			}

			if (info != null) {
				info.Dispose ();
				info = null;
			}
		}
	}
}
