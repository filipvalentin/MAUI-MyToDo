namespace MyToDo.Entities {
	class ToDoItem {

		public Guid Id { get; set; }
		public String Title { get; set; }
		public DateTime Deadline { get; set; }
		public bool IsRecurring { get; set; }

	}
}
