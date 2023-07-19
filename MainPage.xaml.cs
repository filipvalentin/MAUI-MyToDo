using MyToDo.Entities;
using MyToDo.Storage;

namespace MyToDo;

public partial class MainPage : ContentPage {

	private static readonly Color lightGrayColor = Color.FromArgb("#8b8b8c");
	private Dictionary<Guid, ToDoItem> idMap;
	private Dictionary<Guid, Frame> idMap_CheckBoxToFrame;

	IToDoItemManager todoManager;

	public MainPage() {
		InitializeComponent();
		idMap = new Dictionary<Guid, ToDoItem>();
		idMap_CheckBoxToFrame = new Dictionary<Guid, Frame>();
		todoManager = new SqliteToDoItemManager();
		todoManager.CreateDBFile();
		MyToDoMainPage.Loaded += LoadToDos;
	}

	private void LoadToDos(object sender, EventArgs e) {

		foreach (ToDoItem item in todoManager.GetAllItems()) {
			AddItemToPanel(item);
		}

	}


	private void AddItemToPanel(ToDoItem item) {
		Frame frame = new() {
			CornerRadius = 10,
			Padding = 0,
			HasShadow = true,
			BorderColor = lightGrayColor,
			BackgroundColor = lightGrayColor
		};

		Grid grid = new() { Padding = 10 };
		grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

		StackLayout stackLayout = new() { Spacing = 10 };
		stackLayout.Add(new Label() { Text = item.Title, FontSize = 16 });

		HorizontalStackLayout hsl = new() { Spacing = 40 };
		HorizontalStackLayout deadlineHSL = new() { Spacing = 5 };
		deadlineHSL.Add(new Label() { Text = "Deadline" });
		deadlineHSL.Add(new Label() { Text = item.Deadline.ToString() });
		hsl.Add(deadlineHSL);
		if (item.IsRecurring)
			hsl.Add(new Label() { Text = "Recurring" });
		stackLayout.Add(hsl);
		grid.SetColumn(stackLayout, 0);
		grid.Add(stackLayout);

		StackLayout checkBoxSL = new() { HorizontalOptions = LayoutOptions.End };
		CheckBox cb = new();
		cb.CheckedChanged += new EventHandler<CheckedChangedEventArgs>(CheckBox_CheckChanged);
		idMap.Add(cb.Id, item);
		idMap_CheckBoxToFrame.Add(cb.Id, frame);
		checkBoxSL.Add(cb);
		grid.SetColumn(checkBoxSL, 1);
		grid.Add(checkBoxSL);

		frame.Content = grid;

		ToDoList.Add(frame);
	}

	private void CheckBox_CheckChanged(object sender, CheckedChangedEventArgs e) {
		Guid checkBoxId = ((CheckBox)sender).Id;

		Thread.Sleep(200);
		todoManager.DeleteItem(idMap[checkBoxId]);
		ToDoList.Remove(idMap_CheckBoxToFrame[checkBoxId]);
	}


	private async void AddNewToDoButton_Clicked(object sender, EventArgs e) {

		if (NewToDoTitle.Text == null) {
			await DisplayAlert("Invalid title!", "To do title shouldn't be empty!", "I understand");
			return;
		}

		ToDoItem newItem = new() {
			Id = Guid.NewGuid(),
			Title = NewToDoTitle.Text,
			Deadline = new DateTime(NewToDoDate.Date.Year,
				NewToDoDate.Date.Month,
				NewToDoDate.Date.Day,
				NewToDoTime.Time.Hours,
				NewToDoTime.Time.Minutes,
				NewToDoTime.Time.Seconds
				),
			IsRecurring = NewToDoIsRecurring.IsChecked
		};

		DiscardToDoButton_Clicked(this, EventArgs.Empty);

		//ClearNewToDo();

		AddItemToPanel(newItem);

		todoManager.SaveItem(newItem);

	}

	private void InitializeNewToDoButton_Clicked(object sender, EventArgs e) {
		InitializeNewToDoButton.IsVisible = false;
		NewToDoFrame.IsVisible = true;
	}

	private void DiscardToDoButton_Clicked(object sender, EventArgs e) {
		InitializeNewToDoButton.IsVisible = true;
		NewToDoFrame.IsVisible = false;
		NewToDoTitle.Text = null;
		NewToDoTime.Time = new TimeSpan();
		NewToDoDate.Date = DateTime.Now;
		NewToDoIsRecurring.IsChecked = false;
		//ClearNewToDo();
	}


	//private void ClearNewToDo() {

	//}
}

