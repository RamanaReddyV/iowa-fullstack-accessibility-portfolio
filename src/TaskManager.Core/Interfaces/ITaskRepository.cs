using TaskManager.Core.Entities;

namespace TaskManager.Core.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?>             GetByIdAsync(int id);
    Task<TaskItem>              CreateAsync(TaskItem task);
    Task<TaskItem>              UpdateAsync(TaskItem task);
    Task                        DeleteAsync(int id);
    Task<IEnumerable<TaskItem>> SearchAsync(string? query, TaskPriority? priority, Entities.TaskStatus? status);
}
