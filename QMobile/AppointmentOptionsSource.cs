using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using CoreGraphics;

namespace QMobile
{
	public class AppointmentOptionsSource : UITableViewSource
	{
		TFProcessOption[] tableItems;
		TFMerchants merchant;
		string cellId = "TableCell";
		public TFAvailableSchedule availableSchedResultJSON;
		public TFScheduledAppointment schedAppointmentJSON;
		private UIViewController viewControllerLocal;
		public TFOLReservation reservation;
		public TFGetTicketResponse ticketResponse;
		LoadingOverlay _loadPop;

		public List<DateSelection> dateSelection;

		public class DateSelection
		{
			public int index { get; set; }

			public string dateDisplay { get; set; }

			public string dateValue { get; set; }
		}

		public AppointmentOptionsSource (TFProcessOption[] items, TFMerchants viewMerchant, UIViewController viewController)
		{
			tableItems = items;
			merchant = viewMerchant;
			viewControllerLocal = viewController;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Length;
		}

		//		public override string TitleForHeader (UITableView tableView, nint section)
		//		{
		//			return "Schedule an Appointment";
		//		}
		//
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{			
			tableView.DeselectRow (indexPath, true);

			//AppointmentViewController appointmentView = viewControllerLocal.Storyboard.InstantiateViewController ("AppointmentViewController") as AppointmentViewController;
			//------LOADING Screen--------------------------
			// Determine the correct size to start the overlay (depending on device orientation)
			var bounds = UIScreen.MainScreen.Bounds; // portrait bounds
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
				bounds.Size = new CGSize (bounds.Size.Height, bounds.Size.Width);
			}
			// show the loading overlay on the UI thread using the correct orientation sizing
			this._loadPop = new LoadingOverlay (bounds);
			//------LOADING Screen--------------------------

			InvokeOnMainThread (() => {
				switch (tableItems [indexPath.Row].type) {
				//DATE BUTTON WAS PRESSED -------------------------------------------------------------------------------------------------------
				case "Date":
					var dateActionSheet = new UIActionSheet ("Select Appointment Date");
					DateSelection ds = new DateSelection ();
					dateSelection = new List<DateSelection> ();
					var dsDate = new DateTime ();
					if(merchant.schedReserve_sameDay)
						dsDate = DateTime.Now;
					else
						dsDate = DateTime.Now.AddDays(1);
					
					int daysCount = 5;
					switch (merchant.schedReserveWeekend_flag) {
					case 0:
						for (int x = 0; x < daysCount; x++) {
							if (dsDate.DayOfWeek == DayOfWeek.Saturday)
								dsDate = dsDate.AddDays (2);
							else if (dsDate.DayOfWeek == DayOfWeek.Sunday)
								dsDate = dsDate.AddDays (1);

							ds = new DateSelection ();
							ds.index = Convert.ToInt32 (dateActionSheet.AddButton (dsDate.ToString ("ddd, MMMM dd")));
							Console.WriteLine ("Button #" + ds.index);
							ds.dateDisplay = dsDate.ToString ("ddd, MMMM dd");
							ds.dateValue = dsDate.ToString ("yyyy-MM-dd");
							dateSelection.Add (ds);

							dsDate = dsDate.AddDays (1);
						}
						break;
					case 1:
						for (int x = 0; x < daysCount; x++) {
							if (dsDate.DayOfWeek == DayOfWeek.Sunday)
								dsDate = dsDate.AddDays (1);

							ds = new DateSelection ();
							ds.index = Convert.ToInt32 (dateActionSheet.AddButton (dsDate.ToString ("ddd, MMMM dd")));
							Console.WriteLine ("Button #" + ds.index);
							ds.dateDisplay = dsDate.ToString ("ddd, MMMM dd");
							ds.dateValue = dsDate.ToString ("yyyy-MM-dd");
							dateSelection.Add (ds);

							dsDate = dsDate.AddDays (1);
						}
						break;
					case 2:
						for (int x = 0; x < daysCount; x++) {
							if (dsDate.DayOfWeek == DayOfWeek.Saturday)
								dsDate = dsDate.AddDays (1);

							ds = new DateSelection ();
							ds.index = Convert.ToInt32 (dateActionSheet.AddButton (dsDate.ToString ("ddd, MMMM dd")));
							Console.WriteLine ("Button #" + ds.index);
							ds.dateDisplay = dsDate.ToString ("ddd, MMMM dd");
							ds.dateValue = dsDate.ToString ("yyyy-MM-dd");
							dateSelection.Add (ds);

							dsDate = dsDate.AddDays (1);
						}
						break;
					case 3:
						for (int x = 0; x < daysCount; x++) {
							ds = new DateSelection ();
							ds.index = Convert.ToInt32 (dateActionSheet.AddButton (dsDate.ToString ("ddd, MMMM dd")));
							Console.WriteLine ("Button #" + ds.index);
							ds.dateDisplay = dsDate.ToString ("ddd, MMMM dd");
							ds.dateValue = dsDate.ToString ("yyyy-MM-dd");
							dateSelection.Add (ds);

							dsDate = dsDate.AddDays (1);
						}
						break;
					default:
						break;
					}
					Console.WriteLine ("Button #" + dateActionSheet.AddButton ("Cancel"));
					dateActionSheet.CancelButtonIndex = 5;
					dateActionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
						if (b.ButtonIndex != dateActionSheet.CancelButtonIndex) {
							string dateTouched = dateSelection.Where (d => d.index == b.ButtonIndex).First ().dateValue;
							Console.WriteLine ("Button " + dateTouched + " clicked");
							(viewControllerLocal as AppointmentViewController).schedDate = dateTouched;
							//update row
							tableItems [indexPath.Row].valueDisplay = DateTime.ParseExact ((viewControllerLocal as AppointmentViewController).schedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString ("ddd, MMM dd");
							tableItems [indexPath.Row].value = DateTime.ParseExact ((viewControllerLocal as AppointmentViewController).schedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString ("yyyy-mm-dd");
							//update Available Slots
							updateAvailableSched ();
							tableItems [1].valueDisplay = (viewControllerLocal as AppointmentViewController).schedTimeString;
							tableItems [1].value = (viewControllerLocal as AppointmentViewController).schedTimeKey;
							tableView.ReloadData ();
						} else {
							Console.WriteLine ("Cancelled");
						}
					};
					dateActionSheet.ShowInView (viewControllerLocal.View);
					break;
				//DATE BUTTON WAS PRESSED <END>-------------------------------------------------------------------------------------------------------

				//TIME SLOT BUTTON WAS PRESSED -------------------------------------------------------------------------------------------------------
				case "Time":
					//new UIAlertView ("Alert", tableItems [indexPath.Row].title, null, "Got It!", null).Show ();
					var timeActionSheet = new UIActionSheet ("Select Time Slot");
					AvailableSched availSched = new AvailableSched ();
					List<AvailableSched> availableSchedList = new List<AvailableSched> ();
					//add reloading of schedule from JSON call here to make sure updated one is shown
					//if( (viewControllerLocal as AppointmentViewController).availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched) == null)
					foreach (AvailableSched asched in (viewControllerLocal as AppointmentViewController).availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched) {
						asched.index = Convert.ToInt32 (timeActionSheet.AddButton (asched.schedString));
						availableSchedList.Add (asched);
					}
					(viewControllerLocal as AppointmentViewController).availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched = availableSchedList;
					timeActionSheet.CancelButtonIndex = timeActionSheet.AddButton ("Cancel");
					timeActionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
						if (b.ButtonIndex != timeActionSheet.CancelButtonIndex) {
							string timeTouched = availableSchedList.Where (d => d.index == b.ButtonIndex).First ().schedString;
							string timeKeyTouched = availableSchedList.Where (d => d.index == b.ButtonIndex).First ().schedKey;
							Console.WriteLine ("Button " + timeKeyTouched + " clicked");
							(viewControllerLocal as AppointmentViewController).schedTimeString = timeTouched;
							(viewControllerLocal as AppointmentViewController).schedTimeKey = timeKeyTouched;
							//update row
							tableItems [indexPath.Row].valueDisplay = (viewControllerLocal as AppointmentViewController).schedTimeString;
							tableItems [indexPath.Row].value = (viewControllerLocal as AppointmentViewController).schedTimeKey;
							tableView.ReloadData ();
						} else {
							Console.WriteLine ("Cancelled");
						}
					};
					timeActionSheet.ShowInView (viewControllerLocal.View);
					break;
				//TIME SLOT BUTTON WAS PRESSED <END>-------------------------------------------------------------------------------------------------------

				//TRANSACTION BUTTON WAS PRESSED -------------------------------------------------------------------------------------------------------
				case "Transaction":
					//new UIAlertView ("Alert", tableItems [indexPath.Row].title, null, "Got It!", null).Show ();
					var tranActionSheet = new UIActionSheet ("Select Service Type");
					TransactionType tranType = new TransactionType ();
					List<TransactionType> tranTypesNew = new List<TransactionType> ();
					//add reloading of transaction types from JSON call here to make sure updated one is shown
					foreach (TransactionType tt in (viewControllerLocal as AppointmentViewController).transactionTypesJSON.getAllTranTypesMobileJSONResult.TransactionTypes) {
						tt.index = Convert.ToInt32 (tranActionSheet.AddButton (tt.tranType));
						tranTypesNew.Add (tt);
					}
					(viewControllerLocal as AppointmentViewController).transactionTypesJSON.getAllTranTypesMobileJSONResult.TransactionTypes = tranTypesNew;
					tranActionSheet.CancelButtonIndex = tranActionSheet.AddButton ("Cancel");
					tranActionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
						if (b.ButtonIndex != tranActionSheet.CancelButtonIndex) {
							string tranTypeTouched = tranTypesNew.Where (d => d.index == b.ButtonIndex).First ().tranType;
							string tranTypeIdTouched = tranTypesNew.Where (d => d.index == b.ButtonIndex).First ().tran_id_local;
							Console.WriteLine ("Button " + tranTypeTouched + " clicked");
							(viewControllerLocal as AppointmentViewController).schedTranType = tranTypeTouched;
							(viewControllerLocal as AppointmentViewController).schedTranTypeId = tranTypeIdTouched;
							//update row
							tableItems [indexPath.Row].valueDisplay = (viewControllerLocal as AppointmentViewController).schedTranType;
							tableItems [indexPath.Row].value = (viewControllerLocal as AppointmentViewController).schedTranType;
							tableItems [indexPath.Row].valueId = (viewControllerLocal as AppointmentViewController).schedTranTypeId;
							//update Available Slots if Appointment
							if ((viewControllerLocal as AppointmentViewController).action.Equals ("APPOINTMENT")) {
								updateAvailableSched ();
								tableItems [1].valueDisplay = (viewControllerLocal as AppointmentViewController).schedTimeString;
								tableItems [1].value = (viewControllerLocal as AppointmentViewController).schedTimeKey;
							}
							tableView.ReloadData ();
						} else {
							Console.WriteLine ("Cancelled");
						}
					};
					tranActionSheet.ShowInView (viewControllerLocal.View);
					break;
				//TRANSACTION BUTTON WAS PRESSED <END>-------------------------------------------------------------------------------------------------------

				//MOBILE BUTTON WAS PRESSED -------------------------------------------------------------------------------------------------------
				case "Mobile":
					//Create Alert
					var textInputAlertController = UIAlertController.Create ("Enter Mobile No", "Entering your mobile number will allow you to receive In-app notifications as well as SMS alerts regarding your place in the queue.", 
						                               UIAlertControllerStyle.Alert);

					//Add Text Input
					textInputAlertController.AddTextField (textField => {
					});
					textInputAlertController.TextFields [0].KeyboardType = UIKeyboardType.PhonePad;

					//Add Actions
					var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => Console.WriteLine ("Cancel was Pressed"));
					var okayAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Default, alertAction => {
						Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields [0].Text);
						(viewControllerLocal as AppointmentViewController).reservMobileNo = textInputAlertController.TextFields [0].Text;
						Console.WriteLine ("The user entered '{0}'", (viewControllerLocal as AppointmentViewController).reservMobileNo);
						if ((viewControllerLocal as AppointmentViewController).action.Equals ("RESERVATION")) {
							tableItems [indexPath.Row].valueDisplay = (viewControllerLocal as AppointmentViewController).reservMobileNo;
							tableItems [indexPath.Row].value = (viewControllerLocal as AppointmentViewController).reservMobileNo;
						}
						tableView.ReloadData ();
					});

					textInputAlertController.AddAction (cancelAction);
					textInputAlertController.AddAction (okayAction);

					//Present Alert
					viewControllerLocal.PresentViewController (textInputAlertController, true, null);
					break;
				//MOBILE BUTTON WAS PRESSED <END>-------------------------------------------------------------------------------------------------------

				//PROCEED BUTTON WAS PRESSED -------------------------------------------------------------------------------------------------------
				case "Proceed":
					if ((viewControllerLocal as AppointmentViewController).action.Equals ("APPOINTMENT")) {
						Console.WriteLine ("Confirm Date: " + (viewControllerLocal as AppointmentViewController).schedDate);
						Console.WriteLine ("Confirm Time: " + (viewControllerLocal as AppointmentViewController).schedTimeKey);
						Console.WriteLine ("Confirm Tran: " + (viewControllerLocal as AppointmentViewController).schedTranType);

						if (!String.IsNullOrEmpty ((viewControllerLocal as AppointmentViewController).schedDate) &&
						    !String.IsNullOrEmpty ((viewControllerLocal as AppointmentViewController).schedTimeKey) &&
						    !String.IsNullOrEmpty ((viewControllerLocal as AppointmentViewController).schedTranType)) {
							string confirmationString = String.Format ("Date : {0}\nTime : {1}\nService : {2}", 
								                            DateTime.ParseExact ((viewControllerLocal as AppointmentViewController).schedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString ("ddd, MMMM dd"),
								                            (viewControllerLocal as AppointmentViewController).schedTimeString, 
								                            (viewControllerLocal as AppointmentViewController).schedTranType);
							var proceedAlert = new UIAlertView ("Please Confirm Details:", confirmationString, null, "Proceed", new string[] { "Cancel" });
							proceedAlert.Clicked += (s, b) => {
								if (b.ButtonIndex.ToString ().Equals ("0")) {
									//Call addTFScheduledReservation Service
									addTFScheduledAppointment ();

//									Console.WriteLine ("Proceed with Transaction");
//									var notification = new UILocalNotification ();
//									// set the fire date (the date time in which it will fire)
//									notification.FireDate = NSDate.Now.AddSeconds (30);
//									// configure the alert stuff
//									notification.AlertAction = "QMobile Alert!";
//									notification.AlertBody = "Your Scheduled appointment is near!";
//									// modify the badge
//									notification.ApplicationIconBadgeNumber = 1;
//									// set the sound to be the default sound
//									notification.SoundName = UILocalNotification.DefaultSoundName;
//									// schedule it
//									UIApplication.SharedApplication.ScheduleLocalNotification (notification);
								} else if (b.ButtonIndex.ToString ().Equals ("1")) {
									Console.WriteLine ("Cancel");
								}
							};
							proceedAlert.Show ();
						} else {
							new UIAlertView ("Please review details", "One of the options cannot be empty.", null, "OK", null).Show ();
						}
					} else if ((viewControllerLocal as AppointmentViewController).action.Equals ("RESERVATION")) {
						string confirmationString = String.Format ("Date : {0}\nMobile No: {1}\nService : {2}", 
							                            DateTime.ParseExact ((viewControllerLocal as AppointmentViewController).schedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString ("ddd, MMMM dd"),
							                            (viewControllerLocal as AppointmentViewController).reservMobileNo, 
							                            (viewControllerLocal as AppointmentViewController).schedTranType);
						var proceedAlertReserve = new UIAlertView ("Please Confirm Details:", confirmationString, null, "Proceed", new string[] { "Cancel" });
						proceedAlertReserve.Clicked += (s, b) => {
							if (b.ButtonIndex.ToString ().Equals ("0")) {
								//Call addTFUserJSON Service
								Console.WriteLine ("Proceed with Reservation");
								addTFOLReservation ();
//								var notification = new UILocalNotification ();
//								// set the fire date (the date time in which it will fire)
//								notification.FireDate = NSDate.Now.AddSeconds (30);
//								// configure the alert stuff
//								notification.AlertAction = "QMobile Alert!";
//								notification.AlertBody = "Your ticket is near!";
//								// modify the badge
//								notification.ApplicationIconBadgeNumber = 1;
//								// set the sound to be the default sound
//								notification.SoundName = UILocalNotification.DefaultSoundName;
//								// schedule it
//								UIApplication.SharedApplication.ScheduleLocalNotification (notification);
							} else if (b.ButtonIndex.ToString ().Equals ("1")) {
								Console.WriteLine ("Cancel");
							}
						};
						proceedAlertReserve.Show ();
					}

					break;
				//PROCESS BUTTON WAS PRESSED <END>-------------------------------------------------------------------------------------------------------
				default:
					new UIAlertView ("Button Issue", "The button you tapped had no corresponding action.", null, "OK", null).Show ();
					break;
				}
			});

		}



		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellId);
			if (tableItems [indexPath.Row].type.Equals ("Proceed")) {
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellId);
				}
				cell.TextLabel.Text = tableItems [indexPath.Row].title;// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
				cell.TextLabel.TextAlignment = UITextAlignment.Center;
				cell.TextLabel.TextColor = UIColor.White;
				cell.BackgroundColor = UIColor.Orange;
				cell.Accessory = UITableViewCellAccessory.None;
			} else {
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Value1, cellId);
				}

				cell.TextLabel.Text = tableItems [indexPath.Row].title;// + " | " + tableItems[indexPath.Row].BRANCH_NAME;
				cell.DetailTextLabel.Text = tableItems [indexPath.Row].valueDisplay;

				if ((viewControllerLocal as AppointmentViewController).action.Equals ("RESERVATION") && tableItems [indexPath.Row].type.Equals ("Date")) {
					cell.Accessory = UITableViewCellAccessory.None;
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					cell.UserInteractionEnabled = false;
				} else {
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}


			}


			//Console.Out.WriteLine (cell.TextLabel.Text);
			//cell.ImageView.Image = FromURL(tableItems [indexPath.Row].icon_image);// fix size. use small thumbnail only
			return cell;
		}

		static UIImage FromURL (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				return UIImage.LoadFromData (data);
		}

		public async void updateAvailableSched ()
		{
			//Time Slot -- add error checking for this
			try {
				string url = String.Format ("http://tfsmsgatesit.azurewebsites.net/TFGatewayJSON.svc/getTFMerchantAvailableSchedule/{0}/{1}/{2}/{3}/{4}/", 
					             merchant.COMPANY_NO, merchant.BRANCH_NO, (viewControllerLocal as AppointmentViewController).schedDate, "ID", 
					             String.IsNullOrEmpty ((viewControllerLocal as AppointmentViewController).schedTranTypeId) ? "0" : (viewControllerLocal as AppointmentViewController).schedTranTypeId);
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = request.GetResponse () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								availableSchedResultJSON = new TFAvailableSchedule ();
								availableSchedResultJSON = JsonConvert.DeserializeObject<TFAvailableSchedule> (content);
								Console.WriteLine (availableSchedResultJSON.getMerchantAvailableScheduleResult.ResponseCode);
								Console.WriteLine (availableSchedResultJSON.getMerchantAvailableScheduleResult.ResponseMessage);
								(viewControllerLocal as AppointmentViewController).availableSchedResultJSON = availableSchedResultJSON;
								if (availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched.Any ()) {
									(viewControllerLocal as AppointmentViewController).schedTimeKey = availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched.First ().schedKey;
									(viewControllerLocal as AppointmentViewController).schedTimeString = availableSchedResultJSON.getMerchantAvailableScheduleResult.availableSched.First ().schedString;
								} else {
									(viewControllerLocal as AppointmentViewController).schedTimeKey = "";
									(viewControllerLocal as AppointmentViewController).schedTimeString = "";
								}
							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("No Internet", "We can't seem to connect to the internet.", null, "OK", null).Show ();
			}
			//---------------------------------------------------------
		}

		public async void addTFScheduledAppointment ()
		{
			//Time Slot -- add error checking for this
			this.viewControllerLocal.Add (this._loadPop);
			try {
				string url = String.Format ("http://tfsmsgatesit.azurewebsites.net/TFGatewayJSON.svc/addTFUserScheduleJSON/{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}/{10}/{11}/{12}/", 
					             merchant.COMPANY_NO, 
					             merchant.BRANCH_NO,
					             "-",
					             AppDelegate.tfAccount.email, 
					             AppDelegate.tfAccount.name,
					             (viewControllerLocal as AppointmentViewController).schedTranType,
					             (viewControllerLocal as AppointmentViewController).schedTranTypeId,
					             "-", 
					             (viewControllerLocal as AppointmentViewController).schedDate, 
					             (viewControllerLocal as AppointmentViewController).schedTimeKey, 
					             (viewControllerLocal as AppointmentViewController).userLocation.Latitude, 
					             (viewControllerLocal as AppointmentViewController).userLocation.Longitude,
					             merchant.icon_image.Replace ("/", ";").Replace (":", "~"));
								
				Console.WriteLine (url);
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								schedAppointmentJSON = new TFScheduledAppointment ();
								schedAppointmentJSON = JsonConvert.DeserializeObject<TFScheduledAppointment> (content);
								Console.WriteLine (schedAppointmentJSON.addTFUserScheduleJSONResult.ResponseCode);
								Console.WriteLine (schedAppointmentJSON.addTFUserScheduleJSONResult.ResponseMessage);

								TicketViewController ticketView = viewControllerLocal.Storyboard.InstantiateViewController ("TicketViewController") as TicketViewController;
								DashTabController dashTabView = viewControllerLocal.Storyboard.InstantiateViewController ("DashTabController") as DashTabController;

								if (schedAppointmentJSON.addTFUserScheduleJSONResult.ResponseCode.Equals ("00")) {
									//add local notification here (2 hrs, 30 mins, 15 mins)							
									InvokeOnMainThread (() => {
										ticketView.appointment = schedAppointmentJSON;
										TFTicket tix = new TFTicket ();
										tix.branch_id = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.branch_id;
										tix.company_id = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.company_id;
										tix.cust_name = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.cust_name;
										tix.date = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.reservation_date;
										tix.id = Convert.ToString (schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.id);
										tix.image_icon = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.image_icon;
										tix.merchant = merchant;
										tix.queue_no = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.queue_no;
										tix.time = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.reservation_time;
										tix.tran_id_local = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.tran_id_local;
										tix.tran_type_name = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.tran_type_name;
										tix.status = schedAppointmentJSON.addTFUserScheduleJSONResult.ScheduleDetails.status;
										tix.type = "APPOINTMENT";
										ticketView.ticket = tix;
										ticketView.NavigationItem.SetRightBarButtonItem (new UIBarButtonItem ("Home", UIBarButtonItemStyle.Plain, (sender, args) => {
											Console.WriteLine ("Home Button was pressed!");
											dashTabView.NavigationItem.SetHidesBackButton (true, false);
											viewControllerLocal.NavigationController.PushViewController (dashTabView, true);
										}), true);
										addLocalNotification (tix);
										viewControllerLocal.NavigationController.PushViewController (ticketView, true);	
									});
								} else {
									new UIAlertView ("Oops!", "Can't seem to schedule an appointment now.\nMessage: "
									+ schedAppointmentJSON.addTFUserScheduleJSONResult.ResponseMessage, null, "OK", null).Show ();
								}
							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
			}
			this._loadPop.Hide ();
			//---------------------------------------------------------
		}

		public void addLocalNotification (TFTicket ticket)
		{
			DateTime firedate = new DateTime ();
			firedate = DateTime.ParseExact (ticket.date + " " + ticket.time, "yyyy-MM-dd HHmm", CultureInfo.InvariantCulture);
			var version8 = new Version (8, 2);

			var notification1 = new UILocalNotification ();
			notification1.FireDate = DateTimeToNSDate (firedate.AddHours (-2));
			if (new Version (UIDevice.CurrentDevice.SystemVersion) >= version8)
				notification1.AlertTitle = "QMobile Reminder!";
			//notification1.AlertAction = "OK";
			notification1.AlertBody = String.Format ("Ticket No. {2}\nYou have an appointment at {0} - {1} 2 Hours from now.", ticket.merchant.COMPANY_NAME, ticket.merchant.BRANCH_NAME, ticket.queue_no);
			notification1.ApplicationIconBadgeNumber = 1;
			notification1.SoundName = UILocalNotification.DefaultSoundName;
			if (firedate.Subtract (DateTime.Now).Hours >= 2)
				UIApplication.SharedApplication.ScheduleLocalNotification (notification1);

			var notification2 = new UILocalNotification ();
			notification2.FireDate = DateTimeToNSDate (firedate.AddMinutes (-30));
			if (new Version (UIDevice.CurrentDevice.SystemVersion) >= version8)
				notification2.AlertTitle = "QMobile Reminder!";
			//notification2.AlertAction = "OK";
			notification2.AlertBody = String.Format ("Ticket No. {2}\nYou have an appointment at {0} - {1} 30 Minutes from now.", ticket.merchant.COMPANY_NAME, ticket.merchant.BRANCH_NAME, ticket.queue_no);
			notification2.ApplicationIconBadgeNumber = 1;
			notification2.SoundName = UILocalNotification.DefaultSoundName;
			if (firedate.Subtract (DateTime.Now).Minutes >= 30)
				UIApplication.SharedApplication.ScheduleLocalNotification (notification2);

			var notification3 = new UILocalNotification ();
			notification3.FireDate = DateTimeToNSDate (firedate.AddMinutes (-15));
			if (new Version (UIDevice.CurrentDevice.SystemVersion) >= version8)
				notification3.AlertTitle = "QMobile Reminder!";
			//notification3.AlertAction = "OK";
			notification3.AlertBody = String.Format ("Ticket No. {2}\nYou have an appointment at {0} - {1} 15 Minutes from now.", ticket.merchant.COMPANY_NAME, ticket.merchant.BRANCH_NAME, ticket.queue_no);
			notification3.ApplicationIconBadgeNumber = 1;
			notification3.SoundName = UILocalNotification.DefaultSoundName;
			if (firedate.Subtract (DateTime.Now).Minutes >= 15)
				UIApplication.SharedApplication.ScheduleLocalNotification (notification3);

			var notification4 = new UILocalNotification ();
			notification4.FireDate = DateTimeToNSDate (firedate.AddMinutes (-2));
			if (new Version (UIDevice.CurrentDevice.SystemVersion) >= version8)
				notification4.AlertTitle = "QMobile Reminder!";
			//notification4.AlertAction = "OK";
			notification4.AlertBody = String.Format ("Ticket No. {2}\nYou have an appointment at {0} - {1} 2 Minutes from now.", ticket.merchant.COMPANY_NAME, ticket.merchant.BRANCH_NAME, ticket.queue_no);
			notification4.ApplicationIconBadgeNumber = 1;
			notification4.SoundName = UILocalNotification.DefaultSoundName;
			if (firedate.Subtract (DateTime.Now).Minutes >= 2)
				UIApplication.SharedApplication.ScheduleLocalNotification (notification4);

			Console.WriteLine ("1 : " + notification1.FireDate.ToString ());
			Console.WriteLine ("2 : " + notification2.FireDate.ToString ());
			Console.WriteLine ("3 : " + notification3.FireDate.ToString ());
			Console.WriteLine ("4 : " + notification4.FireDate.ToString ());
			
		}

		public static NSDate DateTimeToNSDate (DateTime date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (
				                     new DateTime (2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate (
				(date - reference).TotalSeconds);
		}

		public async void addTFOLReservation ()
		{
			//http://tfqmsservicesc7b32.azurewebsites.net/kioskJSON.svc/addTFUserJSON/Sales/09088152132/Ken%20Marvin/M;R;7;32;weeken88@mail.com/217/14.005/121.32
			this.viewControllerLocal.Add (this._loadPop);

			string url = "";
			if (merchant.edition.Equals ("CLOUD-SAAS")) {
				url = String.Format (merchant.serviceURL	+ "/kioskJSON.svc/addTFUserJSON/{0}/{1}/{2}/{3}/{4};{5};{6};{7};{8}/{9}/{10}/{11}/{12}/", 
					(viewControllerLocal as AppointmentViewController).schedTranTypeId, 
					"0", 
					String.IsNullOrEmpty ((viewControllerLocal as AppointmentViewController).reservMobileNo) ? "-" : (viewControllerLocal as AppointmentViewController).reservMobileNo,
					String.IsNullOrEmpty (AppDelegate.tfAccount.name) ? "-" : AppDelegate.tfAccount.name,
					"M", "R", merchant.COMPANY_NO, merchant.BRANCH_NO, String.IsNullOrEmpty (AppDelegate.tfAccount.email) ? "-" : AppDelegate.tfAccount.email,
					merchant.COMPANY_NO,
					merchant.BRANCH_NO,
					(viewControllerLocal as AppointmentViewController).userLocation.Latitude,
					(viewControllerLocal as AppointmentViewController).userLocation.Longitude);
			} else {
				url = String.Format (merchant.serviceURL	+ "/kioskJSON.svc/addTFUserJSON/{0}/{1}/{2}/{3};{4};{5};{6};{7}/{8}/{9}/{10}", 
					(viewControllerLocal as AppointmentViewController).schedTranType, 
					String.IsNullOrEmpty ((viewControllerLocal as AppointmentViewController).reservMobileNo) ? "-" : (viewControllerLocal as AppointmentViewController).reservMobileNo,
					String.IsNullOrEmpty (AppDelegate.tfAccount.name) ? "-" : AppDelegate.tfAccount.name,
					"M", "R",
					merchant.COMPANY_NO,
					merchant.BRANCH_NO,
					String.IsNullOrEmpty (AppDelegate.tfAccount.email) ? "-" : AppDelegate.tfAccount.email,
					(viewControllerLocal as AppointmentViewController).schedTranTypeId,
					(viewControllerLocal as AppointmentViewController).userLocation.Latitude,
					(viewControllerLocal as AppointmentViewController).userLocation.Longitude);
			}

			Console.WriteLine (url);

			try {
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (url));
				request.ContentType = "application/json";
				request.Method = "GET";
				using (HttpWebResponse response = await request.GetResponseAsync () as HttpWebResponse) {
					if (response.StatusCode != HttpStatusCode.OK) {
						Console.Out.WriteLine ("Error fetching data. Server returned status code: {0}", response.StatusCode);
						new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
					} else {
						using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
							var content = reader.ReadToEnd ();
							if (string.IsNullOrWhiteSpace (content)) {
								Console.Out.WriteLine ("Response contained empty body...");
							} else {
								Console.Out.WriteLine ("Response Body: \r\n {0}", content);
								ticketResponse = new TFGetTicketResponse ();
								ticketResponse = JsonConvert.DeserializeObject<TFGetTicketResponse> (content);
								Console.WriteLine (ticketResponse.addTFUserJSONResult.ResponseCode);
								Console.WriteLine (ticketResponse.addTFUserJSONResult.ResponseMessage);

								TicketViewController ticketView = viewControllerLocal.Storyboard.InstantiateViewController ("TicketViewController") as TicketViewController;
								DashTabController dashTabView = viewControllerLocal.Storyboard.InstantiateViewController ("DashTabController") as DashTabController;

								if (ticketResponse.addTFUserJSONResult.ResponseCode.Equals ("00")) {
									InvokeOnMainThread (() => {
										ticketView.olReservation = ticketResponse;
										TFTicket tix = new TFTicket ();
										tix.branch_id = merchant.BRANCH_NO;
										tix.company_id = merchant.COMPANY_NO;
										tix.cust_name = AppDelegate.tfAccount.name;
										tix.date = DateTime.ParseExact (ticketResponse.addTFUserJSONResult.UserDetails.dateIn, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToString ("yyyy-MM-dd");
										tix.id = "";
										tix.image_icon = merchant.icon_image;
										tix.merchant = merchant;
										tix.queue_no = ticketResponse.addTFUserJSONResult.UserDetails.queueNo;
										tix.time = "";
										tix.tran_id_local = ticketResponse.addTFUserJSONResult.UserDetails.tran_id_local;
										tix.tran_type_name = ticketResponse.addTFUserJSONResult.UserDetails.tranType;
										tix.status = ticketResponse.addTFUserJSONResult.UserDetails.status;
										tix.type = "RESERVATION";
										ticketView.ticket = tix;

										InvokeInBackground (async () => {
											reservation = new TFOLReservation ();
											reservation.user_refno = ticketResponse.addTFUserJSONResult.UserDetails.userReferenceNo;
											reservation.mobile_no = (viewControllerLocal as AppointmentViewController).reservMobileNo;
											reservation.cust_name = AppDelegate.tfAccount.name;
											reservation.queue_no = ticketResponse.addTFUserJSONResult.UserDetails.queueNo;
											reservation.date_in = ticketResponse.addTFUserJSONResult.UserDetails.dateIn;
											reservation.remarks = ticketResponse.addTFUserJSONResult.UserDetails.remarks;
											reservation.awt = ticketResponse.addTFUserJSONResult.UserDetails.awt;
											reservation.smsnotif_status = "";
											reservation.reserve_type = "R";
											reservation.company_id = merchant.COMPANY_NO;
											reservation.branch_id = merchant.BRANCH_NO;
											reservation.reservation_status = "";
											reservation.confirmation_no = "";
											reservation.mobile_userid = AppDelegate.tfAccount.email;
											reservation.tran_type = ticketResponse.addTFUserJSONResult.UserDetails.tranType;
											reservation.tran_id_local = ticketResponse.addTFUserJSONResult.UserDetails.tran_id_local;
											reservation.status = "PENDING";
											reservation.image_icon = merchant.icon_image;
											reservation.entryLatitude = Convert.ToString ((viewControllerLocal as AppointmentViewController).userLocation.Latitude);
											reservation.entryLongitude = Convert.ToString ((viewControllerLocal as AppointmentViewController).userLocation.Longitude);
											await AppDelegate.MobileService.GetTable<TFOLReservation> ().InsertAsync (reservation); //see how we can check if successful...
											Console.WriteLine ("Done Insert in TFOlReservation...");
										});

										ticketView.NavigationItem.SetRightBarButtonItem (new UIBarButtonItem ("Home", UIBarButtonItemStyle.Plain, (sender, args) => {
											Console.WriteLine ("Home Button was pressed!");
											dashTabView.NavigationItem.SetHidesBackButton (true, false);
											viewControllerLocal.NavigationController.PushViewController (dashTabView, true);
										}), true);

										viewControllerLocal.NavigationController.PushViewController (ticketView, true);	
									});
								} else {
									new UIAlertView ("Oops!", "Can't seem to get a ticket now.\nMessage: "
									+ ticketResponse.addTFUserJSONResult.ResponseMessage, null, "OK", null).Show ();
								}
							}
						}
					}
				}
			} catch (Exception e) {
				new UIAlertView ("Problem Connecting", "We can't seem to connect to the internet.", null, "OK", null).Show ();
			}

			this._loadPop.Hide ();
		}

	}
}

