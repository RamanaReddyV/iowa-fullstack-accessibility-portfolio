using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.Core.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository) => _repository = repository;

    public Task<IEnumerable<TaskItem>> GetAllTasksAsync()    => _repository.GetAllAsync();
    public Task<TaskItem?>             GetTaskByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<TaskItem>              CreateTaskAsync(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Task title is required.", nameof(task));
        task.CreatedAt = DateTime.UtcNow;
        return _repository.CreateAsync(task);
    }
    public Task<TaskItem> UpdateTaskAsync(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Task title is required.", nameof(task));
        return _repository.UpdateAsync(task);
    }
    public Task DeleteTaskAsync(int id)    => _repository.DeleteAsync(id);
    public Task<IEnumerable<TaskItem>> SearchTasksAsync(string? query, TaskPriority? priority, Entities.TaskStatus? status)
        => _repository.SearchAsync(query, priority, status);
}
