using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using AudioToolbox;

namespace QMobile
{
	partial class CurrentServingViewController : UIViewController
	{
		public TFCurrentServingListResponse servingListJSON;
		public TFMerchants merchant;
		NSUrl url;
		SystemSound systemSound;

		public CurrentServingViewController (IntPtr handle) : base (handle)
		{
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			url = NSUrl.FromFilename ("Sounds/blop.aiff");
			systemSound = new SystemSound (url);
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			//-------Details List-----------------------
			servingListJSON = new TFCurrentServingListResponse ();
			getCurrentServingList ();
			AppDelegate.contentFlags.listenForNewContent = true;
			AppDelegate.contentFlags.company_id = merchant.COMPANY_NO;
			AppDelegate.contentFlags.branch_id = merchant.BRANCH_NO;
			Console.WriteLine ("listen for new content : " + AppDelegate.contentFlags.listenForNewContent);
			runGetCurrentServingByTranTask ();
			//---------------------------------------------------------

			InvokeOnMainThread (() => {
				//this.NavigationItem.TitleView = new UIImageView (UIImage.FromBundle ("iconx30.png"));
				this.NavigationItem.TitleView = new UIImageView (FeaturedTableSource.MaxResizeImage(UIImage.FromBundle ("LogoWithOutBackground.png"), 50, 50));

				//ticketIconView.Image = UIImage.FromBundle(ticketColor);
				branchNameLabel.Text = merchant.BRANCH_NAME;
				companyLabel.Text = merchant.COMPANY_NAME;
//				ticketNoLabel.Text = ticket.queue_no;
//				currentlyServingValue.Text = "";
//				currentlyServingLabel.Text = "";
				refreshButton.TintColor = UIColor.White;
				refreshButton.Hidden = false;
				//if (servingListJSON.getCurrentServingListMobileJSONResult.CurrentServingList.Any ()) {
//				if (servingListJSON.getCurrentServingListJSONResult.ResponseCode.Equals ("00")) {
//					currentServingTable.TableFooterView = new UIView (CGRect.Empty);
//					currentServingTable.Source = new CurrentServingTableSource (servingListJSON.getCurrentServingListJSONResult.CurrentServingList.ToArray (), this);
//					currentServingTable.ReloadData ();
//				}

			});

			refreshButton.TouchUpInside += async (object sender, EventArgs e) => {
				Console.WriteLine ("refresh button was pressed");
				getCurrentServingList ();
				//if (servingListJSON.getCurrentServingListMobileJSONResult.CurrentServingList.Any ()) {
//				if (servingListJSON.getCurrentServingListJSONResult.ResponseCode.Equals ("00")) {
//					currentServingTable.TableFooterView = new UIView (CGRect.Empty);
//					currentServingTable.Source = new CurrentServingTableSource (servingListJSON.getCurrentServingListJSONResult.CurrentServingList.ToArray (), this);
//					currentServingTable.ReloadData ();
				refreshButton.Hidden = true;
				await Task.Delay (20000);
				refreshButton.Hidden = false;
//				}
				//getCurrentServingByTran ();

			};

		}

		async void runGetCurrentServingByTranTask ()
		{
			while (AppDelegate.contentFlags.listenForNewContent) {
				Console.WriteLine ("Listening for new content");
				await Task.Delay (10000); //change to 20-30 secs
				if (AppDelegate.contentFlags.newContentAvailable) {
					Console.WriteLine ("New content available");
					getCurrentServingList ();
				} else {
					Console.WriteLine ("No New content");
				}
			}
		}

		async void blinkTicketBG ()
		{
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f); //TFColor.FromHexString ("#00bcd4", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			await Task.Delay (500);
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			await Task.Delay (500);
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			await Task.Delay (500);
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			await Task.Delay (500);
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			await Task.Delay (500);
			lastTicketCalledContainer.BackgroundColor = TFColor.FromHexString ("#00bcd4", 1.0f);
			branchDetailsView.BackgroundColor = TFColor.FromHexString ("#0097a9", 1.0f);
		}

		public async void getCurrentServingList ()
		{

			string url = "";

			if (merchant.edition.Equals ("CLOUD-SAAS")) {
				url = String.Format (merchant.serviceURL	+ "/kioskJSON.svc/getCurrentServingListJSON/{0}/{1}/", merchant.COMPANY_NO, merchant.BRANCH_NO);
			} else {
				url = String.Format (merchant.serviceURL	+ "/kioskJSON.svc/getCurrentServingListJSON/");
			}


			try {
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						new UIAlertView ("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								servingListJSON = new TFCurrentServingListResponse ();
								servingListJSON = JsonConvert.DeserializeObject<TFCurrentServingListResponse> (content);
								Console.WriteLine (servingListJSON.getCurrentServingListJSONResult.ResponseCode);
								Console.WriteLine (servingListJSON.getCurrentServingListJSONResult.ResponseMessage);

								if (servingListJSON.getCurrentServingListJSONResult.ResponseCode.Equals ("00")) {
									foreach (CurrentServingList cs in servingListJSON.getCurrentServingListJSONResult.CurrentServingList) {
										Console.WriteLine ("String Date : " + cs.timeStamp);
										Console.WriteLine ("Date : " + DateTime.Parse (cs.timeStamp).ToString ());
									}

									lastTicketNoLabel.Text = servingListJSON.getCurrentServingListJSONResult.CurrentServingList
																.OrderByDescending (r => DateTime.Parse (r.timeStamp)).FirstOrDefault ().queueNo;
									//call blink
									blinkTicketBG ();
									InvokeOnMainThread (() => {
										currentServingTable.TableFooterView = new UIView (CGRect.Empty);
										currentServingTable.Source = new CurrentServingTableSource (servingListJSON.getCurrentServingListJSONResult.CurrentServingList.ToArray (), this);
										currentServingTable.ReloadData ();
										systemSound.PlayAlertSound ();
									});

								} else {
									lastTicketNoLabel.Text = " -- ";
									if(merchant.COMPANY_NO != 7)
										new UIAlertView ("Counters are Closed", "Counters have not yet started serving tickets. Try refreshing after a few minutes.", null, "OK", null).Show ();
									else
										new UIAlertView ("Problem getting store status", "Try refreshing after a few minutes.", null, "OK", null).Show ();
								}
								AppDelegate.contentFlags.newContentAvailable = false;
								Console.WriteLine ("newContentAvailable : " + AppDelegate.contentFlags.newContentAvailable);

							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
				Console.WriteLine ("Problem loading Currently Serving List...");
				Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);
			}
			//---------------------------------------------------------
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
//			if (ticket.type.Equals ("RESERVATION")) {
//				AppDelegate.allowRunBGTasks = true;
//			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			//this.mapMainView.RemoveObserver (this, new NSString ("myLocation"));
//			AppDelegate.allowRunBGTasks = false;
//			Console.WriteLine ("STOPPED >> GetCurrentServing Tran : " + ticket.tran_id_local + " - " + ticket.tran_type_name);
			AppDelegate.contentFlags.listenForNewContent = false;
			AppDelegate.contentFlags.company_id = 0;
			AppDelegate.contentFlags.branch_id = 0;
			Console.WriteLine ("listen for new content : " + AppDelegate.contentFlags.listenForNewContent);
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			//myLoc.StopUpdatingLocation ();
			base.ViewDidDisappear (animated);
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		#endregion
	}
}
