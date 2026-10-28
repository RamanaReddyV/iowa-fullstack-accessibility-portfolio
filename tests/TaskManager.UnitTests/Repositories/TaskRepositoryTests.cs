using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;
using Xunit;

namespace TaskManager.UnitTests.Repositories;

public class TaskRepositoryTests : IDisposable
{
    private readonly TaskDbContext  _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var opts = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context    = new TaskDbContext(opts);
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_ReturnsOrderedByCreatedAtDescending()
    {
        var older = new TaskItem { Title = "Older", CreatedAt = DateTime.UtcNow.AddHours(-2) };
        var newer = new TaskItem { Title = "Newer", CreatedAt = DateTime.UtcNow };
        _context.Tasks.AddRange(older, newer);
        await _context.SaveChangesAsync();

        var result = (await _repository.GetAllAsync()).ToList();

        result[0].Title.Should().Be("Newer");
        result[1].Title.Should().Be("Older");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_AssignsId()
    {
        var task = new TaskItem { Title = "New Task" };

        var result = await _repository.CreateAsync(task);

        result.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ExistingTask_RemovesIt()
    {
        var task = new TaskItem { Title = "Delete Me" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(task.Id);

        var found = await _repository.GetByIdAsync(task.Id);
        found.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task SearchAsync_CombinedFilters_ReturnsCorrectSubset()
    {
        _context.Tasks.AddRange(
            new TaskItem { Title = "High Pending",   Priority = TaskPriority.High, Status = Core.Entities.TaskStatus.Todo },
            new TaskItem { Title = "High Done",      Priority = TaskPriority.High, Status = Core.Entities.TaskStatus.Done },
            new TaskItem { Title = "Low Pending",    Priority = TaskPriority.Low,  Status = Core.Entities.TaskStatus.Todo }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync(null, TaskPriority.High, Core.Entities.TaskStatus.Todo);

        result.Should().HaveCount(1);
        result.First().Title.Should().Be("High Pending");
    }

    public void Dispose() => _context.Dispose();
}
