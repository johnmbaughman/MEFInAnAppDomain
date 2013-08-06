using System;
using AppDomainTestInterfaces;

namespace AppDomainTestLib {

	public class Import : MarshalByRefObject, IExport {

		public void InHere(AppDomainArgs args) {
			Console.WriteLine("In MEF Library1: AppDomain: {0}, args.StringArg: {1}",
				AppDomain.CurrentDomain.FriendlyName,
				args.StringArg);

			args.StringArg = "Leaving MEF Library1";
			Console.WriteLine("Library1 args set to {0}", args.StringArg);
		}
	}
}