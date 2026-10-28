using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers;

public class TasksController : Controller
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service) => _service = service;

    // GET /Tasks
    public async Task<IActionResult> Index(string? search, TaskPriority? priority, Core.Entities.TaskStatus? status)
    {
        var tasks = await _service.SearchTasksAsync(search, priority, status);
        var all   = await _service.GetAllTasksAsync();

        var vm = new TaskListViewModel
        {
            Tasks         = tasks.Select(MapToViewModel),
            SearchQuery   = search,
            FilterPriority = priority,
            FilterStatus  = status,
            TotalCount    = all.Count(),
            TodoCount     = all.Count(t => t.Status == Core.Entities.TaskStatus.Todo),
            InProgressCount = all.Count(t => t.Status == Core.Entities.TaskStatus.InProgress),
            DoneCount     = all.Count(t => t.Status == Core.Entities.TaskStatus.Done)
        };
        return View(vm);
    }

    // GET /Tasks/Create
    public IActionResult Create() => View(new TaskItemViewModel());

    // POST /Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItemViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _service.CreateTaskAsync(MapToEntity(vm));
        TempData["SuccessMessage"] = $"Task \"{vm.Title}\" created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Tasks/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var task = await _service.GetTaskByIdAsync(id);
        if (task is null) return NotFound();
        return View(MapToViewModel(task));
    }

    // POST /Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItemViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _service.UpdateTaskAsync(MapToEntity(vm));
        TempData["SuccessMessage"] = $"Task \"{vm.Title}\" updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Tasks/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var task = await _service.GetTaskByIdAsync(id);
        if (task is null) return NotFound();
        return View(MapToViewModel(task));
    }

    // GET /Tasks/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _service.GetTaskByIdAsync(id);
        if (task is null) return NotFound();
        return View(MapToViewModel(task));
    }

    // POST /Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _service.GetTaskByIdAsync(id);
        var title = task?.Title ?? "Task";
        await _service.DeleteTaskAsync(id);
        TempData["SuccessMessage"] = $"\"{title}\" deleted.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Tasks/ToggleStatus (AJAX)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id, string newStatus)
    {
        var task = await _service.GetTaskByIdAsync(id);
        if (task is null) return NotFound(new { message = "Task not found" });

        if (Enum.TryParse<Core.Entities.TaskStatus>(newStatus, out var parsedStatus))
        {
            task.Status = parsedStatus;
            await _service.UpdateTaskAsync(task);
            return Ok(new { success = true, status = task.Status.ToString() });
        }
        return BadRequest(new { message = "Invalid status" });
    }

    // ── Mapping helpers ──────────────────────────────────────────────────────
    private static TaskItemViewModel MapToViewModel(TaskItem t) => new()
    {
        Id          = t.Id,
        Title       = t.Title,
        Description = t.Description,
        Priority    = t.Priority,
        Status      = t.Status,
        DueDate     = t.DueDate,
        CreatedBy   = t.CreatedBy,
        CreatedAt   = t.CreatedAt
    };

    private static TaskItem MapToEntity(TaskItemViewModel vm) => new()
    {
        Id          = vm.Id,
        Title       = vm.Title,
        Description = vm.Description,
        Priority    = vm.Priority,
        Status      = vm.Status,
        DueDate     = vm.DueDate,
        CreatedBy   = vm.CreatedBy,
        CreatedAt   = vm.CreatedAt
    };
}
