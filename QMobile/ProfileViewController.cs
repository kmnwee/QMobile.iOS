using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Linq;
using SDWebImage;
using MBProgressHUD;
using System.Net;
using System.IO;
using System.Web;

namespace QMobile
{
	partial class ProfileViewController : UIViewController
	{
		LoadingOverlay _loadPop;
		bool loggedIn;
		TFMemberRegistry memberRegistry;
		MTMBProgressHUD hud;
		List<TFProfileOption> options;
		DashTabController dashTab;

		public ProfileViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			dashTab = this.ParentViewController as DashTabController;
			Console.WriteLine ("Profile View Loaded");
			Console.WriteLine (String.Format ("Tix Tab : {0}, {1}, {2}, {3}", AppDelegate.tfAccount.name, AppDelegate.tfAccount.email, AppDelegate.tfAccount.birthday, AppDelegate.tfAccount.id));

////			//------LOADING Screen--------------------------
//			// Determine the correct size to start the overlay (depending on device orientation)
//			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
//			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
//				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
//			}
//			// show the loading overlay on the UI thread using the correct orientation sizing
//			this._loadPop = new LoadingOverlay (bounds);
////			this.View.Add (this._loadPop);
////			//------LOADING Screen--------------------------
			hud = new MTMBProgressHUD (View) {
				LabelText = "Loading profile details...",
				RemoveFromSuperViewOnHide = true,
				AnimationType = MBProgressHUDAnimation.Fade,
				//DetailsLabelText = "loading profile details...",
				Mode = MBProgressHUDMode.Indeterminate,
				Color = UIColor.Gray,
				Opacity = 60,
				DimBackground = true
			};

			View.AddSubview (hud);
			hud.Show (animated: true);

			initiateProfileView ();

			hud.Hide (animated: true, delay: 1);

			//------LOADING Screen END----------------------
			//this._loadPop.Hide ();
			//------LOADING Screen END----------------------



//			loginAnonymousButton.TouchUpInside += (object sender, EventArgs e) => {
//				
//			};
//
//			logoutButton.TouchUpInside += (object sender, EventArgs e) => {
//				Console.WriteLine ("Log button was pressed");
//				Console.WriteLine ("App Delegate " + AppDelegate.tfAccount.loggedIn);
//				//Console.WriteLine ("App Delegate2 " + AppDelegate.tfAccount.loggedIn);
//				//logout account
//
//				if (AppDelegate.tfAccount.loggedIn) {
//					signOut();
//				} else {
//					//new UIAlertView("Alert", "Login!", null, "Got It!", null).Show ();
//				}
//			};
		}

		public void signInGuest()
		{
			Console.Out.WriteLine ("Login Anonymously pressed");

			var proceedAlert = new UIAlertView ("Sign in as Guest?", "Notifications and updates will be sent to this device only.", null, "Continue", new string[] { "Cancel" });
			proceedAlert.Clicked += (s, b) => {
				if (b.ButtonIndex.ToString ().Equals ("0")) {
					var uID = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString ();
					Console.Out.WriteLine ("Device ID: " + uID);

					AppDelegate.tfAccount.name = "Guest";
					Console.WriteLine (AppDelegate.tfAccount.name);
					Account anoAcc = new Account ();
					anoAcc.Properties.Add ("uID", uID);
					anoAcc.Properties.Add ("uType", "2");
					anoAcc.Properties.Add ("uName", "Guest");
					anoAcc.Properties.Add ("uLastName", "Visitor");
					anoAcc.Properties.Add ("uEmail", uID);

					InvokeOnMainThread (async () => {
						AppDelegate.tfAccount.loginType = 2;
						AppDelegate.tfAccount.name = "Guest";
						AppDelegate.tfAccount.lastname = "Visitor";
						AppDelegate.tfAccount.email = uID;
						AppDelegate.tfAccount.birthday = "01/01/1900";
						AppDelegate.tfAccount.account = anoAcc;
						AppDelegate.tfAccount.id = uID;
						AppDelegate.tfAccount.loggedIn = true;
						AppDelegate.tfAccount.gender = "Undefined";
						AppDelegate.tfAccount.timezone = "8";
						AppDelegate.registerForRemoteNotifications ();
						AccountStore.Create ().Save (anoAcc, "Anonymous");
						long memberCheck = 0;

						try {
							memberCheck = (await AppDelegate.MobileService.GetTable<TFMemberRegistry> ().Take (0).IncludeTotalCount ()
								.Where (m => m.email == AppDelegate.tfAccount.email && m.device_type == "iOS")
								.ToListAsync () as ITotalCountProvider).TotalCount;
							if (memberCheck < 1) {
								Console.WriteLine ("Member does not exist!");
								memberRegistry = new TFMemberRegistry ();
								memberRegistry.app_id = AppDelegate.tfAccount.id;
								memberRegistry.birthday = AppDelegate.tfAccount.birthday;
								memberRegistry.device_type = "iOS";
								memberRegistry.email = AppDelegate.tfAccount.email;
								memberRegistry.first_name = AppDelegate.tfAccount.name;
								memberRegistry.full_name = AppDelegate.tfAccount.name + " " + AppDelegate.tfAccount.lastname;
								memberRegistry.gender = AppDelegate.tfAccount.gender;
								memberRegistry.last_name = AppDelegate.tfAccount.lastname;
								memberRegistry.reg_date = DateTime.Now.ToString ();
								memberRegistry.timezone = AppDelegate.tfAccount.timezone;
								await AppDelegate.MobileService.GetTable<TFMemberRegistry> ().InsertAsync (memberRegistry);

							} else {
								Console.WriteLine ("Member does exists!");
							}


							View.AddSubview (hud);
							hud.Show (animated: true);

							initiateProfileView ();

							hud.Hide (animated: true, delay: 1);
							//------LOADING Screen END----------------------
							//								this._loadPop.Hide ();
							//------LOADING Screen END----------------------

							dashTab.SelectedIndex = 4;
						} catch (Exception ess) {
							Console.WriteLine ("Problem loading Member...");
							Console.WriteLine (ess.Message);
							Console.WriteLine (ess.StackTrace);
						}
					});

				} else if (b.ButtonIndex.ToString ().Equals ("1")) {
					Console.WriteLine ("Cancel");
				}
			};
			proceedAlert.Show ();


		}

		public void signIn()
		{
			try {
				var auth = new OAuth2Authenticator (
					clientId: "440108756136511",
					scope: "email",
					authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
					redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

				FeaturedViewController featuredView = this.Storyboard.InstantiateViewController ("FeaturedViewController") as FeaturedViewController;

				auth.AllowCancel = true;
				auth.Completed += (sender2, eventArgs) => {
					DismissViewController (true, null);

					if (eventArgs.IsAuthenticated) {
						// Use eventArgs.Account to do wonderful things
						//name = eventArgs.Account.Username;
						AccountStore.Create ().Save (eventArgs.Account, "Facebook");

						var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, eventArgs.Account);
						request.GetResponseAsync ().ContinueWith (t => {
							if (t.IsFaulted) {
								AppDelegate.tfAccount.loggedIn = false;	
								new UIAlertView ("Oops!", "Problem connecting to facebook. Please try again.", null, "OK", null).Show ();
							} else {
								string json = t.Result.GetResponseText ();
								Console.WriteLine (json);
								var parsed = JObject.Parse (json);
								Console.WriteLine (parsed ["first_name"]);
								Console.WriteLine (parsed ["last_name"]);
								AppDelegate.tfAccount.name = Convert.ToString (parsed ["first_name"]);
								Console.WriteLine (AppDelegate.tfAccount.name);
								InvokeOnMainThread (async () => {
									AppDelegate.tfAccount.loginType = 1;
									AppDelegate.tfAccount.name = Convert.ToString (parsed ["first_name"]);
									AppDelegate.tfAccount.lastname = Convert.ToString (parsed ["last_name"]);
									if (!String.IsNullOrEmpty (Convert.ToString (parsed ["email"])))
										AppDelegate.tfAccount.email = Convert.ToString (parsed ["email"]);
									else
										AppDelegate.tfAccount.email = Convert.ToString (parsed ["id"]);
									AppDelegate.tfAccount.birthday = Convert.ToString (parsed ["birthday"]);
									AppDelegate.tfAccount.account = eventArgs.Account;
									AppDelegate.tfAccount.id = Convert.ToString (parsed ["id"]);
									AppDelegate.tfAccount.loggedIn = true;
									AppDelegate.tfAccount.gender = Convert.ToString (parsed ["gender"]);
									AppDelegate.tfAccount.timezone = Convert.ToString (parsed ["timezone"]);
									string param1 = HttpUtility.ParseQueryString(eventArgs.Account.ToString ()).Get("access_token");
									AppDelegate.tfAccount.access_token = param1;
									AppDelegate.registerForRemoteNotifications ();

									long memberCheck = 0;

									try {
										memberCheck = (await AppDelegate.MobileService.GetTable<TFMemberRegistry> ().Take (0).IncludeTotalCount ()
											.Where (m => m.email == AppDelegate.tfAccount.email && m.device_type == "iOS")
											.ToListAsync () as ITotalCountProvider).TotalCount;
										if (memberCheck < 1) {
											Console.WriteLine ("Member does not exist!");
											memberRegistry = new TFMemberRegistry ();
											memberRegistry.app_id = AppDelegate.tfAccount.id;
											memberRegistry.birthday = AppDelegate.tfAccount.birthday;
											memberRegistry.device_type = "iOS";
											memberRegistry.email = AppDelegate.tfAccount.email;
											memberRegistry.first_name = AppDelegate.tfAccount.name;
											memberRegistry.full_name = AppDelegate.tfAccount.name + " " + AppDelegate.tfAccount.lastname;
											memberRegistry.gender = AppDelegate.tfAccount.gender;
											memberRegistry.last_name = AppDelegate.tfAccount.lastname;
											memberRegistry.reg_date = DateTime.Now.ToString ();
											memberRegistry.timezone = AppDelegate.tfAccount.timezone;
											await AppDelegate.MobileService.GetTable<TFMemberRegistry> ().InsertAsync (memberRegistry);
										} else {
											Console.WriteLine ("Member does exists!");
										}

										View.AddSubview (hud);
										hud.Show (animated: true);

										initiateProfileView ();

										hud.Hide (animated: true);
										//------LOADING Screen END----------------------
										//this._loadPop.Hide ();
										//------LOADING Screen END----------------------
										dashTab.SelectedIndex = 4;
									} catch (Exception ess) {
										Console.WriteLine ("Problem loading Member...");
										Console.WriteLine (ess.Message);
										Console.WriteLine (ess.StackTrace);
									}
									//ViewDidLoad();
								});
							}
						});
					} else {
						AppDelegate.tfAccount.loggedIn = false;
					}

				};
				PresentViewController (auth.GetUI (), true, null);
			} catch (Exception ex) {
				new UIAlertView ("Oops!", "Internet connection lost. Please try again.", null, "OK", null).Show ();
			}
		}

		public void setTabIndex(int index)
		{
			dashTab.SelectedIndex = index;
		}

		public void signOut()
		{
			var logoutAlert = new UIAlertView ("Sign Out", "Are you sure you want to sign out?", null, "Yes", new string[] { "Cancel" });
			logoutAlert.Clicked += (s, b) => {
				if (b.ButtonIndex.ToString ().Equals ("0")) {
					Console.WriteLine ("Proceed with logout");
					InvokeOnMainThread (() => {
						profileImageView.Image = UIImage.FromBundle ("Icons3/user-icon.png");
						coverPhoto.Image = null;
						profileNameLabel.Text = "Profile";
						emailLabel.Text = "";
//						logoutButton.SetTitleColor (UIColor.Black, UIControlState.Normal);
//						logoutButton.SetTitle ("Sign in using Facebook", UIControlState.Normal);
//						loginAnonymousButton.Hidden = false;
//						loginAnonymousButton.SetTitle ("Sign in as Guest", UIControlState.Normal);
					});

					try {
						if (AppDelegate.tfAccount.loginType == 1)
							AccountStore.Create ().Delete (AppDelegate.tfAccount.account, "Facebook");
						else if (AppDelegate.tfAccount.loginType == 2)
							AccountStore.Create ().Delete (AppDelegate.tfAccount.account, "Anonymous");
						AppDelegate.tfAccount = new TFAccount ();
						AppDelegate.tfAccount.loggedIn = false;
						AppDelegate.unregisterForRemoteNotifications ();
					} catch (Exception ex) {
						Console.Out.WriteLine (ex.StackTrace);
						Console.Out.WriteLine (ex.Message);
					}
					//ViewDidLoad();
					initiateProfileView();
				} else if (b.ButtonIndex.ToString ().Equals ("1")) {
					Console.WriteLine ("cancel logout");
				}
			};
			logoutAlert.Show ();
		}
			
		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		public void initiateProfileView ()
		{
			this.View.BackgroundColor = UIColor.White;//TFColor.FromHexString ("#0097a9", 1.0f);
			this.coverPhoto.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			this.profileImageView.Hidden = false;
			this.profileNameLabel.Hidden = false;
			this.emailLabel.Hidden = false;
			this.coverPhoto.Hidden = false;

			this.emailLabel.TextColor = UIColor.White;
			this.profileNameLabel.TextColor = UIColor.White;
			
			if (AppDelegate.tfAccount.loggedIn) {
				List<TFMemberFavorites> tffavorites = new List<TFMemberFavorites> ();
				List<TFMerchants> tfmerchants2 = new List<TFMerchants> ();
				int favCount = 0;
				InvokeOnMainThread (async () => {
					Console.WriteLine ("Profile True");
					//profileNameLabel.Text = AppDelegate.tfAccount.name + " " + AppDelegate.tfAccount.lastname.Substring (0, 1) + ".";
					profileNameLabel.Text = AppDelegate.tfAccount.name + " " + AppDelegate.tfAccount.lastname;
					if (AppDelegate.tfAccount.loginType == 1)
						emailLabel.Text = AppDelegate.tfAccount.email;
					else
						emailLabel.Hidden = true;

					//Set Options Table
					TFProfileOption option1 = new TFProfileOption ();
					TFProfileOption option2 = new TFProfileOption ();
					TFProfileOption option3 = new TFProfileOption ();
					TFProfileOption option4 = new TFProfileOption ();
					TFProfileOption option5 = new TFProfileOption ();
					options = new List<TFProfileOption> ();

					option1.title = "Scan QR Code";
					option1.action = "QR";
					option1.info = "Scan QR Code from ticket to be notified when your turn is near";
					option1.image = "icons_f/QR.png";
					options.Add (option1);

					option2.title = "Get a ticket";
					option2.action = "Search";
					option2.info = "Search for the merchant you intend to transact with";
					option2.image = "icons_f/search.png";
					options.Add (option2);

					option3.title = "View Favorites";
					option3.action = "Favorites";
					option3.info = "See your bookmarked merchants";
					option3.image = "icons_f/favorites.png";
					options.Add (option3);

					option4.title = "My Tickets";
					option4.action = "Tickets";
					option4.info = "See your active tickets";
					option4.image = "icons_f/tickets.png";
					options.Add (option4);

					option5.title = "Sign Out";
					option5.action = "SignOut";
					option5.info = "Sign out of QMobile";
					option5.image = "icons_f/signout.png";
					options.Add (option5);

					InvokeOnMainThread (() => {
						profileOptionsTable.Hidden = false;
						profileOptionsTable.TableFooterView = new UIView (CGRect.Empty);
						profileOptionsTable.Source = new ProfileViewOptionsSource (options.ToArray (), this);
						profileOptionsTable.ReloadData ();
					});

					try {
						if (AppDelegate.tfAccount.loginType == 1) {
							//profileImageView.Image = FromURL ("https://graph.facebook.com/" + AppDelegate.tfAccount.id + "/picture?type=large");
							profileImageView.SetImage (
								url: new NSUrl ("https://graph.facebook.com/" + AppDelegate.tfAccount.id + "/picture?type=large"),
								placeholder: UIImage.FromBundle ("Icons3/user-icon.png"),
								options: SDWebImageOptions.RefreshCached
							);

							//string coverPhoto = Json String.Format("https://graph.facebook.com/{0}?fields=cover&access_token={1}", AppDelegate.tfAccount.id, AppDelegate.tfAccount.access_token);
							string url = String.Format ("https://graph.facebook.com/{0}?fields=cover&access_token={1}", AppDelegate.tfAccount.id, AppDelegate.tfAccount.access_token);
							HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
							request.ContentType = "application/json";
							request.Method = "GET";
							using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
								if (response.StatusCode != HttpStatusCode.OK) {
									Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
									//stock cover photo
									//new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
								} else {
									using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
										var content = reader.ReadToEnd ();
										if (string.IsNullOrWhiteSpace (content)) {
											//stock cover photo
											Console.Out.WriteLine ("Response contained empty body...");
										} else {
											Console.Out.WriteLine ("Cover Photo : " + JObject.Parse (content) ["cover"]["source"]);
											string coverURL = JObject.Parse (content) ["cover"]["source"].ToString();
											coverPhoto.SetImage(
												url: new NSUrl (coverURL),
												placeholder: UIImage.FromBundle ("Icons3/user-icon.png"),
												options: SDWebImageOptions.RefreshCached
											);
										}
									}
								}
							}
						} else if (AppDelegate.tfAccount.loginType == 2)
							profileImageView.Image = UIImage.FromBundle ("Icons3/anonymous.png");
						profileImageView.TintColor = null;
					} catch (Exception e) {
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
						//Console.WriteLine ("Problem loading Tickets Count...");
						Console.WriteLine (e.Message);
						Console.WriteLine (e.StackTrace);
					}
//					favoritesCount.Text = Convert.ToString (favCount);
					//Convert.ToString ((await AppDelegate.MobileService.GetTable<TFMemberFavorites> ().Take (0).IncludeTotalCount ()
//						.Where (TFMemberFavorites => TFMemberFavorites.email == AppDelegate.tfAccount.email && TFMemberFavorites.__deleted == "FALSE")
//						.ToListAsync () as ITotalCountProvider).TotalCount);
				});
			} else {
				InvokeOnMainThread (() => {
					Console.WriteLine ("Profile False");
					//profileImageView.Hidden = true;
					//profileNameLabel.Hidden = true;
					emailLabel.Hidden = true;
					//coverPhoto.Hidden = true;
					//profileOptionsTable.Hidden = false;

					//Set Options Table
					TFProfileOption option1 = new TFProfileOption ();
					TFProfileOption option2 = new TFProfileOption ();
					options = new List<TFProfileOption> ();

					option1.title = "Sign in with Facebook";
					option1.action = "SIFB";
					option1.info = "Sign in to QMobile using your Facebook account";
					option1.image = "icons_f/facebook.png";
					options.Add (option1);

					option2.title = "Sign in as Guest";
					option2.action = "SIG";
					option2.info = "Sign in to QMobile as Guest";
					option2.image = "icons_f/guest.png";
					options.Add (option2);

					InvokeOnMainThread (() => {
						profileOptionsTable.Hidden = false;
						profileOptionsTable.TableFooterView = new UIView (CGRect.Empty);
						profileOptionsTable.Source = new ProfileViewOptionsSource (options.ToArray (), this);
						profileOptionsTable.ReloadData ();
					});

//					profileImageView.Image = UIImage.FromBundle ("Icons3/user-icon.png");
//					profileNameLabel.Text = "Profile";
//					emailLabel.Text = "";
					//this.View.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);

//					ticketCount.Text = "0";
//					appointmentCount.Text = "0";
//					notifCount.Text = "0";
//					favoritesCount.Text = "0";

				});
			}

			CALayer profileImageCircle = profileImageView.Layer;
			//profileImageCircle.CornerRadius = profileImageView.Frame.Size.Width / 2;//60;
			profileImageCircle.MasksToBounds = true;
			profileImageCircle.BorderColor = UIColor.White.CGColor;
			profileImageCircle.BorderWidth = 1;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
//			if (profileOptionsTable != null)
//				profileOptionsTable.ContentInset = new UIEdgeInsets (0, 0, 64, 0);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
//			InvokeOnMainThread (() => {
//				Console.WriteLine ("Tickets Table!");
//				this.ParentViewController.NavigationItem.SetRightBarButtonItem 
//				(new UIBarButtonItem ("Sign Out", UIBarButtonItemStyle.Plain, (sender, args) => {
//						signOut();
//					}), true);
//			});

		}

		public override void ViewDidAppear (bool animated)
		{
			initiateProfileView ();
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
