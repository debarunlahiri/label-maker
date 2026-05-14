// Author: Debarun Lahiri
// GitHub: https://github.com/debarunlahiri/

using ObjCRuntime;
using UIKit;

namespace LabelMaker;

public class Program
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
