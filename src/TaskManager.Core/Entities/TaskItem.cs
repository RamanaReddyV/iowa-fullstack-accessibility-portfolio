namespace TaskManager.Core.Entities;

public enum TaskPriority { Low, Medium, High }
public enum TaskStatus  { Todo, InProgress, Done }

public sealed class TaskItem
{
    public int    Id          { get; set; }
    public string Title       { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority   { get; set; } = TaskPriority.Medium;
    public TaskStatus   Status     { get; set; } = TaskStatus.Todo;
    public DateTime     CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime?    DueDate    { get; set; }
    public string       CreatedBy  { get; set; } = "System";
}
