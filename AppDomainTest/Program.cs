using System;
using System.IO;
using AppDomainTestRunner;

namespace AppDomainTest {

	internal class Program {
		private static AppDomain domain;

		[STAThread]
		private static void Main() {
			var cachePath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "ShadowCopyCache");
			var pluginPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Plugins");
			if (!Directory.Exists(cachePath)) {
				Directory.CreateDirectory(cachePath);
			}

			if (!Directory.Exists(pluginPath)) {
				Directory.CreateDirectory(pluginPath);
			}

			// This creates a ShadowCopy of the MEF DLL's (and any other DLL's in the ShadowCopyDirectories)
			var setup = new AppDomainSetup {
				CachePath = cachePath,
				ShadowCopyFiles = "true",
				ShadowCopyDirectories = pluginPath
			};

			// Create a new AppDomain then create an new instance of this application in the new AppDomain.
			// This bypasses the Main method as it's not executing it.
			domain = AppDomain.CreateDomain("Host_AppDomain", AppDomain.CurrentDomain.Evidence, setup);
			var runner = (Runner)domain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.FullName, typeof(Runner).FullName);

			Console.WriteLine("The main AppDomain is: {0}", AppDomain.CurrentDomain.FriendlyName);

			// We now have access to all the methods and properties of Program.
			var appDomainArgs = runner.AppDomainArgHandler();
			Console.WriteLine("appDomainArgs in {0}: StringArgs = {1}", AppDomain.CurrentDomain.FriendlyName, appDomainArgs.StringArg);
			runner.DoWorkInShadowCopiedDomain();
			runner.AppDomainArgHandler();
			runner.DoSomething();

			Console.WriteLine("\nHere you can remove a DLL from the Plugins folder.");
			Console.WriteLine("Press any key when ready...");
			Console.ReadKey();

			// After removing a DLL, we can now recompose the MEF parts and see that the removed DLL is no longer accessed.
			runner.Recompose();
			runner.DoSomething();
			runner.AppDomainArgHandler();

			Console.WriteLine("\nHere we will begin to replace Lib3 with an updated version. \nDelete the old one first DLL from the Plugins folder.");
			Console.WriteLine("Press any key when ready...");
			Console.ReadKey();

			Console.WriteLine("Now showing that Lib3 is deleted.");
			runner.Recompose();
			runner.DoSomething();
			
			Console.WriteLine("\nNext drop the new Lib3 in the Plugins folder.");
			Console.WriteLine("Press any key when ready...");
			Console.ReadKey();

			runner.Recompose();
			runner.DoSomething();
			runner.AppDomainArgHandler();

			Console.WriteLine("Press any key when ready...");
			Console.ReadKey();

			// Clean up.
			AppDomain.Unload(domain);
		}
	}
}