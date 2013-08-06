using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Linq;
using AppDomainTestInterfaces;

namespace AppDomainTestRunner {

	public class Runner : MarshalByRefObject {
		private CompositionContainer container;
		private DirectoryCatalog directoryCatalog;
		private IEnumerable<IExport> exports;
		private AppDomainArgs appDomainArgs;
		private static readonly string pluginPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Plugins");

		public void DoWorkInShadowCopiedDomain() {
			// Use RegistrationBuilder to set up our MEF parts.
			var regBuilder = new RegistrationBuilder();
			regBuilder.ForTypesDerivedFrom<IExport>().Export<IExport>();

			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new AssemblyCatalog(typeof(Runner).Assembly, regBuilder));
			directoryCatalog = new DirectoryCatalog(pluginPath, regBuilder);
			catalog.Catalogs.Add(directoryCatalog);

			container = new CompositionContainer(catalog);
			container.ComposeExportedValue(container);

			// Get our exports available to the rest of Program.
			exports = container.GetExportedValues<IExport>();
			Console.WriteLine("{0} exports in AppDomain {1}", exports.Count(), AppDomain.CurrentDomain.FriendlyName);
		}

		public void Recompose() {
			// Gimme 3 steps...
			directoryCatalog.Refresh();
			container.ComposeParts(directoryCatalog.Parts);
			exports = container.GetExportedValues<IExport>();
		}

		public void DoSomething() {
			// Tell our MEF parts to do something.
			exports.ToList().ForEach(e => {
				e.InHere(appDomainArgs);
				Console.WriteLine("DoSomething args set to {0}", appDomainArgs.StringArg);
			});
		}

		public AppDomainArgs AppDomainArgHandler() {
			// Initialize our AppDomainArgs. 
			if (AppDomain.CurrentDomain.FriendlyName != "Host_AppDomain") return null;
			if (appDomainArgs == null) {
				Console.WriteLine("Initializing instance of appDomainArgs in in AppDomain {0}", AppDomain.CurrentDomain.FriendlyName);
				appDomainArgs = new AppDomainArgs { StringArg = "Args initialized" };
				return appDomainArgs;
			}
			appDomainArgs.StringArg = string.Format("Run through AppDomainArgHandler in AppDomain {0}", AppDomain.CurrentDomain.FriendlyName);
			Console.WriteLine(appDomainArgs.StringArg);
			return appDomainArgs;
		}
	}
}