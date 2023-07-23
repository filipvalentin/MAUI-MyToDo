using MyToDo.Entities;
using MyToDo.Storage;
using System.Diagnostics;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MyToDoBackgroundWorker {
	public class BackgroundWorker {

		private static HashSet<Guid> scheduledToDos = new();
		private static Dictionary<Timer, ToDoItem> scheduledTimers = new();
		private static IToDoItemManager itemManager = new SqliteToDoItemManager();
		public BackgroundWorker() { }

		public void Run() {

			List<ToDoItem> scheduledItems = new();

			while (true) {

				foreach (var item in itemManager.GetAllItems()) {

					if (item.Deadline < DateTime.Now.AddDays(1)) {
						if (!scheduledToDos.Contains(item.Id)) {
							scheduledToDos.Add(item.Id);
							var deadline = item.Deadline.Subtract(DateTime.Now);
							if (deadline.TotalMilliseconds > 0) {
								Timer timer = new(deadline);
								timer.Start();
								timer.Elapsed += OnTimedEvent;
								scheduledTimers.Add(timer, item);
								Debug.WriteLine("aa");
							}
							else {
								if (!item.Acknowledged)
									scheduledItems.Add(item);
							}
						}
					}

				}

				foreach (var item in scheduledItems) {
					HandleItemAcknowledged(item);
				}
				scheduledItems.Clear();

				Thread.Sleep(300000);
			}
		}


		private static void OnTimedEvent(Object source, ElapsedEventArgs e) {
			Timer sourceTimer = (Timer)source;
			ToDoItem item = scheduledTimers[sourceTimer];

			sourceTimer.Stop();

			HandleItemAcknowledged(item);
		}

		private static void HandleItemAcknowledged(ToDoItem item) {
			DisplayNotification(item);
			item.Acknowledged = true;
			Debug.WriteLine($"{item.Id}");
			itemManager.UpdateItem(item);

		}



		static void DisplayNotification(ToDoItem item) {

			StringBuilder message = new();

			message.Append(item.Title).Append(" is happening now!");

			System.Media.SystemSounds.Beep.Play();

			MessageBox.Show(message.ToString(), "MyToDo", MessageBoxButtons.OK);
		}
	}
}
