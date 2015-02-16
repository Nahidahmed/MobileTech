using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin;

namespace MobileTech.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			Insights.Initialize("eeaf7b819be894074f8d93d9b9131c557f91bbdd");

			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
