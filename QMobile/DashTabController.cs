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

			//this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
			this.NavigationItem.TitleView = new UIImageView (FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("LogoWithOutBackground.png"), 50, 50));
			this.NavigationController.NavigationBar.TintColor = UIColor.White;
			this.NavigationController.NavigationBar.BarTintColor = bgColor;
			//this.NavigationController.NavigationBar.ShadowImage = new UIImage ();
			//this.NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			this.NavigationController.NavigationBar.TitleTextAttributes = titleAttributes;
			this.TabBar.Translucent = false;
			this.TabBar.SelectedImageTintColor = TFColor.FromHexString("#0097a7", 1.0f);
			this.TabBar.Items[0].Image = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/featured.png"), 32, 32);
			this.TabBar.Items[0].SelectedImage = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/featured.png"), 32, 32);
			this.TabBar.Items[1].Image = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/search.png"), 32, 32);
			this.TabBar.Items[1].SelectedImage = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/search.png"), 32, 32);
			this.TabBar.Items[2].Image = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/favorites.png"), 32, 32);
			this.TabBar.Items[2].SelectedImage = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/favorites.png"), 32, 32);
			this.TabBar.Items[3].Image = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/tickets.png"), 32, 32);
			this.TabBar.Items[3].SelectedImage = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/tickets.png"), 32, 32);
			this.TabBar.Items[4].Image = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/profile.png"), 32, 32);
			this.TabBar.Items[4].SelectedImage = FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("icons_f/profile.png"), 32, 32);
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
