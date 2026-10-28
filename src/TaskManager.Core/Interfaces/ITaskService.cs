using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?>             GetTaskByIdAsync(int id);
    Task<TaskItem>              CreateTaskAsync(TaskItem task);
    Task<TaskItem>              UpdateTaskAsync(TaskItem task);
    Task                        DeleteTaskAsync(int id);
    Task<IEnumerable<TaskItem>> SearchTasksAsync(string? query, TaskPriority? priority, Entities.TaskStatus? status);
}
