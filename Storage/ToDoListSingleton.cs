using MyToDo.Entities;

namespace MyToDo.Storage {
	public sealed class ToDoListSingleton
    {
		private ToDoListSingleton() { }
		private static List<ToDoItem> todos = null;
		public static List<ToDoItem> Instance {
			get {
				if(todos == null) {
					todos = new List<ToDoItem>();
				}
				return todos;
			}
		}
    }
}
