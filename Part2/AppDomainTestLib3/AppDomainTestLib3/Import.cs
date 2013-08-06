using System;
using AppDomainTestInterfaces;

namespace AppDomainTestLib3 {

	public class Import : MarshalByRefObject, IExport {

		public void InHere(AppDomainArgs args) {
			Console.WriteLine("In MEF Library3: AppDomain: {0}, args.StringArg: {1}",
				AppDomain.CurrentDomain.FriendlyName,
				args.StringArg);

			args.StringArg = "Leaving MEF Library3";
			Console.WriteLine("Library3 args set to {0}", args.StringArg);
		}
	}
}
