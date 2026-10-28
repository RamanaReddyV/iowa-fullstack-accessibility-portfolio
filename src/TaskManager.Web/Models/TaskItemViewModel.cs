using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Entities;

namespace TaskManager.Web.Models;

public class TaskItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    [Display(Name = "Task Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Priority")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Display(Name = "Status")]
    public Core.Entities.TaskStatus Status { get; set; } = Core.Entities.TaskStatus.Todo;

    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [Display(Name = "Created By")]
    [StringLength(100)]
    public string CreatedBy { get; set; } = "System";

    public DateTime CreatedAt { get; set; }
}

public class TaskListViewModel
{
    public IEnumerable<TaskItemViewModel> Tasks { get; set; } = [];
    public string? SearchQuery { get; set; }
    public TaskPriority? FilterPriority { get; set; }
    public Core.Entities.TaskStatus? FilterStatus { get; set; }
    public int TotalCount { get; set; }
    public int TodoCount { get; set; }
    public int InProgressCount { get; set; }
    public int DoneCount { get; set; }
}
