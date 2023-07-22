using Microsoft.Data.Sqlite;
using MyToDo.Entities;
using System.Diagnostics;

namespace MyToDo.Storage {
	internal class SqliteToDoItemManager : IToDoItemManager {

		private static string dbPath = "E:\\todos.db"; //a sane person would make a config file or smth, but I am not [anymore]
													   //in fact, I've made a fatal decision in using MAUI for this Windows specific personal use app

		public void SaveItem(ToDoItem item) {

			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"INSERT INTO TODOS VALUES ($id, $title, $deadline, $isrecurring, $acknowledged)";
			command.Parameters.AddWithValue("$id", item.Id);
			command.Parameters.AddWithValue("$title", item.Title);
			command.Parameters.AddWithValue("$deadline", item.Deadline.ToString());
			command.Parameters.AddWithValue("$isrecurring", item.IsRecurring == true ? 1 : 0);
			command.Parameters.AddWithValue("$acknowledged", item.Acknowledged);
			command.ExecuteNonQuery();

		}

		//avoids creating a list by itself
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
					Acknowledged = reader.GetBoolean(4),
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


		public void UpdateDeadline(ToDoItem item) {
			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"UPDATE TODOS SET DEADLINE = $deadline WHERE ID = $id";
			command.Parameters.AddWithValue("$deadline", item.Deadline.ToString());
			command.Parameters.AddWithValue("$id", item.Id);
			command.ExecuteNonQuery();
		}

		public void CreateDBFile() {

			using var connection = new SqliteConnection("Data Source=" + dbPath);
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = @"CREATE TABLE IF NOT EXISTS TODOS (ID STRING PRIMARY KEY, TITLE STRING, DEADLINE DATE, ISRECURRING BOOLEAN, ACKNOWLEDGED BOOLEAN)";
			command.ExecuteNonQuery();

			Debug.WriteLine(FileSystem.Current.AppDataDirectory);

		}
	}
}
