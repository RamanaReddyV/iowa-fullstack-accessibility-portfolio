using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context) => _context = context;

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
        => await _context.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();

    public async Task<TaskItem?> GetByIdAsync(int id)
        => await _context.Tasks.FindAsync(id);

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is not null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TaskItem>> SearchAsync(
        string? query,
        TaskPriority? priority,
        Core.Entities.TaskStatus? status)
    {
        var q = _context.Tasks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(t =>
                t.Title.Contains(query) ||
                t.Description.Contains(query));

        if (priority.HasValue)
            q = q.Where(t => t.Priority == priority.Value);

        if (status.HasValue)
            q = q.Where(t => t.Status == status.Value);

        return await q.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }
}
