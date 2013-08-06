using System;
using AppDomainTestInterfaces;

namespace AppDomainTestLib2 {

	public class Import : MarshalByRefObject, IExport {

		public void InHere(AppDomainArgs args) {
			Console.WriteLine("In MEF Library2: AppDomain: {0}, args.StringArg: {1}",
				AppDomain.CurrentDomain.FriendlyName,
				args.StringArg);

			args.StringArg = "Leaving MEF Library2";
			Console.WriteLine("Library2 args set to {0}", args.StringArg);
		}
	}
}