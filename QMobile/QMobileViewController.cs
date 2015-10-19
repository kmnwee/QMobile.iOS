using System;
using System.Drawing;
using System.Collections.Generic;
using Xamarin.Auth;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;
using Foundation;
using UIKit;
using System.Net;
using System.Json;
using System.IO;
using System.Threading.Tasks;

namespace QMobile
{
	public partial class QMobileViewController : UIViewController
	{
		public TFMerchants[] tableItems;

		public QMobileViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine ("Here");
			Console.WriteLine (tableItems.Length);
			foreach (TFMerchants tf in tableItems) {
				Console.WriteLine (tf.BRANCH_NAME);
			}
			List<TFMerchants> tfml = new List<TFMerchants> ();
			tfml.AddRange (tableItems);

			InvokeOnMainThread (async () => {
				testTableView.Source = new QTableSource (tfml.ToArray(), this);
				testTableView.ReloadData ();
			});
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}

