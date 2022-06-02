namespace ERNI.PBA.Client.Pages
{
    public class TodoState
    {
        public List<TodoItem> Todos = new();

        public string NewTodo { get; set; } = string.Empty;

        public DateTime? DueDate = null;

        public void Save()
        {
            if (string.IsNullOrEmpty(NewTodo))
                return;

            Todos.Add(new TodoItem
            {
                Todo = NewTodo
            });

            NewTodo = string.Empty;

        }

    }
}
