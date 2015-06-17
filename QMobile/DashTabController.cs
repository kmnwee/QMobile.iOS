using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;

namespace QMobile
{
	partial class DashTabController : UITabBarController
	{
		public string name = "";
		public string email = "";
		public string birthday = "";
		public string id = "";
		public string lastname = "";
		public bool loggedIn = false;
		public Account acct = null;


		public DashTabController (IntPtr handle) : base (handle)
		{

		}
		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Console.WriteLine(String.Format("Tab Items: {0}, {1}, {2}", AppDelegate.tfAccount.name, AppDelegate.tfAccount.email, AppDelegate.tfAccount.birthday ));

			UIStringAttributes titleAttributes = new UIStringAttributes ();
			titleAttributes.ForegroundColor = UIColor.White;
			UIColor bgColor = TFColor.FromHexString("#00bcd4", 1.0f);

			this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
			this.NavigationController.NavigationBar.TintColor = UIColor.White;
			this.NavigationController.NavigationBar.BarTintColor = bgColor;
			//this.NavigationController.NavigationBar.ShadowImage = new UIImage ();
			//this.NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			this.NavigationController.NavigationBar.TitleTextAttributes = titleAttributes;
			this.TabBar.Translucent = false;
			this.TabBar.SelectedImageTintColor = TFColor.FromHexString("#0097a7", 1.0f);

			//try select default tab
			//this.SelectedIndex = 2;

			InvokeOnMainThread (() => {
				this.NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
			});

			//Check if Account is Present:
			if (AppDelegate.tfAccount.loggedIn) {
				Console.WriteLine ("Logged IN!");
				this.SelectedIndex = 0;
			} else {
				Console.WriteLine ("No Account");
				this.SelectedIndex = 4;
			}



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
			//Console.WriteLine ("Back button was pressed! 1");
			//logout account
			//AccountStore.Create ().Delete(acct, "Facebook");
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			//Console.WriteLine ("Back button was pressed! 2");
			base.ViewDidDisappear (animated);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			Console.WriteLine ("Override!");
			return UIStatusBarStyle.LightContent;
		}

		#endregion
	}
}
