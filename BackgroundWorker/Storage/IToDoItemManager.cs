using MyToDo.Entities;

namespace MyToDo.Storage {

	interface IToDoItemManager {
		void SaveItem(ToDoItem item);
		IEnumerable<ToDoItem> GetAllItems();
		void DeleteItem(ToDoItem item);
		void UpdateItem(ToDoItem item);
	}
}
