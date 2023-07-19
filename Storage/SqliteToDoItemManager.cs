using Microsoft.Data.Sqlite;
using MyToDo.Entities;

namespace MyToDo.Storage {
	internal class SqliteToDoItemManager : IToDoItemManager {

		private static string dbPath = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "todos.db");

		public void SaveItem(ToDoItem item) {

			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"INSERT INTO TODOS VALUES ($id, $title, $deadline, $isrecurring )";
			command.Parameters.AddWithValue("$id", item.Id);
			command.Parameters.AddWithValue("$title", item.Title);
			command.Parameters.AddWithValue("$deadline", item.Deadline.ToString());
			command.Parameters.AddWithValue("$isrecurring", item.IsRecurring == true ? 1 : 0);
			command.ExecuteNonQuery();

		}

		public IEnumerable<ToDoItem> GetAllItems() {
			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"SELECT * FROM TODOS";
			//List<ToDoItem> items = new();

			using var reader = command.ExecuteReader();
			while (reader.Read()) {
				//items.Add(
				yield return new ToDoItem() {
					Id = reader.GetGuid(0),
					Title = reader.GetString(1),
					Deadline = DateTime.ParseExact(reader.GetString(2), "dd/MM/yyyy HH:mm:ss", null),
					IsRecurring = reader.GetBoolean(3),
				};
			}

			//return items;
		}


		public void DeleteItem(ToDoItem item) {
			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"DELETE FROM TODOS WHERE ID = $id";
			command.Parameters.AddWithValue("$id", item.Id);
			command.ExecuteNonQuery();
		}



		public void UpdateItem(ToDoItem item) {
			throw new NotImplementedException();
		}

		public void CreateDBFile() {
			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"CREATE TABLE IF NOT EXISTS TODOS (ID STRING PRIMARY KEY, TITLE STRING, DEADLINE DATE, ISRECURRING BOOLEAN)";
			command.ExecuteNonQuery();
		}
	}
}
