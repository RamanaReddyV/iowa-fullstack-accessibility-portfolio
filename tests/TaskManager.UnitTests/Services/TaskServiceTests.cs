using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Services;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;
using Xunit;

namespace TaskManager.UnitTests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly TaskDbContext  _context;
    private readonly TaskRepository _repository;
    private readonly TaskService    _service;

    public TaskServiceTests()
    {
        var opts = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context    = new TaskDbContext(opts);
        _repository = new TaskRepository(_context);
        _service    = new TaskService(_repository);
    }

    // ── CreateTaskAsync ──────────────────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_WithValidTitle_ReturnsPersistedTask()
    {
        // Arrange
        var task = new TaskItem { Title = "Write unit tests", Description = "Cover all services", CreatedBy = "Dev" };

        // Act
        var result = await _service.CreateTaskAsync(task);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Write unit tests");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var task = new TaskItem { Title = "" };

        // Act
        Func<System.Threading.Tasks.Task> act = () => _service.CreateTaskAsync(task);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*title*");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_WithWhitespaceTitle_ThrowsArgumentException()
    {
        var task = new TaskItem { Title = "   " };
        Func<System.Threading.Tasks.Task> act = () => _service.CreateTaskAsync(task);
        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ── GetAllTasksAsync ─────────────────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_EmptyDb_ReturnsEmptyCollection()
    {
        var result = await _service.GetAllTasksAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasks_WithSeededData_ReturnsAllTasks()
    {
        await _service.CreateTaskAsync(new TaskItem { Title = "Task A" });
        await _service.CreateTaskAsync(new TaskItem { Title = "Task B" });

        var result = await _service.GetAllTasksAsync();

        result.Should().HaveCount(2);
    }

    // ── GetTaskByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_ExistingId_ReturnsTask()
    {
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Find me" });

        var result = await _service.GetTaskByIdAsync(created.Id);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Find me");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskById_NonExistingId_ReturnsNull()
    {
        var result = await _service.GetTaskByIdAsync(9999);
        result.Should().BeNull();
    }

    // ── UpdateTaskAsync ──────────────────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task UpdateTask_ChangesTitle_PersistsChange()
    {
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Original" });
        created.Title = "Updated Title";

        var result = await _service.UpdateTaskAsync(created);

        result.Title.Should().Be("Updated Title");
        var fetched = await _service.GetTaskByIdAsync(created.Id);
        fetched!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTask_WithEmptyTitle_ThrowsArgumentException()
    {
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "Valid" });
        created.Title = "";

        Func<System.Threading.Tasks.Task> act = () => _service.UpdateTaskAsync(created);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ── DeleteTaskAsync ──────────────────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_ExistingId_RemovesFromDb()
    {
        var created = await _service.CreateTaskAsync(new TaskItem { Title = "To Delete" });

        await _service.DeleteTaskAsync(created.Id);

        var result = await _service.GetTaskByIdAsync(created.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTask_NonExistingId_DoesNotThrow()
    {
        Func<System.Threading.Tasks.Task> act = () => _service.DeleteTaskAsync(9999);
        await act.Should().NotThrowAsync();
    }

    // ── SearchTasksAsync ─────────────────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task SearchTasks_ByTitle_ReturnsMatchingTasks()
    {
        await _service.CreateTaskAsync(new TaskItem { Title = "Accessibility audit" });
        await _service.CreateTaskAsync(new TaskItem { Title = "Performance test" });

        // InMemory EF is case-sensitive; search with matching case
        var result = await _service.SearchTasksAsync("Accessibility", null, null);

        result.Should().HaveCount(1);
        result.First().Title.Should().Contain("Accessibility");
    }

    [Fact]
    public async System.Threading.Tasks.Task SearchTasks_ByPriority_ReturnsMatchingTasks()
    {
        await _service.CreateTaskAsync(new TaskItem { Title = "High task", Priority = TaskPriority.High });
        await _service.CreateTaskAsync(new TaskItem { Title = "Low task",  Priority = TaskPriority.Low  });

        var result = await _service.SearchTasksAsync(null, TaskPriority.High, null);

        result.Should().HaveCount(1);
        result.First().Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    public async System.Threading.Tasks.Task SearchTasks_ByStatus_ReturnsMatchingTasks()
    {
        await _service.CreateTaskAsync(new TaskItem { Title = "Done task", Status = Core.Entities.TaskStatus.Done });
        await _service.CreateTaskAsync(new TaskItem { Title = "Todo task", Status = Core.Entities.TaskStatus.Todo });

        var result = await _service.SearchTasksAsync(null, null, Core.Entities.TaskStatus.Done);

        result.Should().HaveCount(1);
        result.First().Status.Should().Be(Core.Entities.TaskStatus.Done);
    }

    [Fact]
    public async System.Threading.Tasks.Task SearchTasks_NoCriteria_ReturnsAllTasks()
    {
        await _service.CreateTaskAsync(new TaskItem { Title = "A" });
        await _service.CreateTaskAsync(new TaskItem { Title = "B" });
        await _service.CreateTaskAsync(new TaskItem { Title = "C" });

        var result = await _service.SearchTasksAsync(null, null, null);

        result.Should().HaveCount(3);
    }

    // ── Priority & Status defaults ───────────────────────────────────────────

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_DefaultPriority_IsMedium()
    {
        var task = await _service.CreateTaskAsync(new TaskItem { Title = "Default priority task" });
        task.Priority.Should().Be(TaskPriority.Medium);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTask_DefaultStatus_IsTodo()
    {
        var task = await _service.CreateTaskAsync(new TaskItem { Title = "Default status task" });
        task.Status.Should().Be(Core.Entities.TaskStatus.Todo);
    }

    public void Dispose() => _context.Dispose();
}
