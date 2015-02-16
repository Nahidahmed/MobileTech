using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MobileTech.iOS
{
	public class AlertHelper
	{
		public static Task<bool> ShowYesNoAlert(string title, string message) {
			var tcs = new TaskCompletionSource<bool>();

			UIApplication.SharedApplication.InvokeOnMainThread(new NSAction(() =>
				{
//					UIAlertView alert = new UIAlertView(title, message, null, NSBundle.MainBundle.LocalizedString("No", "No"),
//						NSBundle.MainBundle.LocalizedString("Yes", "Yes"));
//					alert.Clicked += (sender, buttonArgs) => tcs.SetResult(buttonArgs.ButtonIndex != alert.CancelButtonIndex);

					UIAlertView alert = new UIAlertView(title, message, null, NSBundle.MainBundle.LocalizedString("No", "No"),
						NSBundle.MainBundle.LocalizedString("Yes", "Yes"));
					alert.Clicked += (sender, buttonArgs) => tcs.SetResult(buttonArgs.ButtonIndex != alert.CancelButtonIndex);

					alert.Show();
				}));

			return tcs.Task;
		}
	}
}

