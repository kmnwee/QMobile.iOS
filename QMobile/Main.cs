using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace QMobile
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			try{
				UIApplication.Main (args, null, "AppDelegate");
			}
			catch(Exception e){
				Console.Out.WriteLine (e.Source);
				Console.Out.WriteLine (e.Message);
				Console.Out.WriteLine (e.StackTrace);
				new UIAlertView ("Application Prompt", e.Message, null, "OK", null).Show ();
				new UIAlertView ("Application Prompt", e.StackTrace, null, "OK", null).Show ();
			}
		}
	}
}
