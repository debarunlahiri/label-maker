// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using Foundation;
using UIKit;

namespace LabelMaker;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		return base.FinishedLaunching(application, launchOptions);
	}

	public override void OnActivated(UIApplication application)
	{
		base.OnActivated(application);
		ApplyLightWindowChrome(application);
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	private static void ApplyLightWindowChrome(UIApplication application)
	{
		foreach (var connectedScene in application.ConnectedScenes)
		{
			if (connectedScene is not UIWindowScene windowScene)
				continue;

			windowScene.Titlebar?.SetValueForKey(NSNumber.FromInt32((int)UITitlebarTitleVisibility.Visible), new NSString("titleVisibility"));

			foreach (var window in windowScene.Windows)
			{
				window.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
			}
		}
	}
}
