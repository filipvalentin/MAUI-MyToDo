using System.Diagnostics;


namespace MyToDoBackgroundWorker {

	class Program {

		static void Main(string[] args) {

			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

			BackgroundWorker worker = new();
			worker.Run();
		}
	}
}