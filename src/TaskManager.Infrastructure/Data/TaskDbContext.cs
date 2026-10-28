using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data;

public sealed class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).HasMaxLength(2000);
            entity.Property(t => t.CreatedBy).HasMaxLength(100);
        });

        // Seed data for demo
        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem
            {
                Id = 1, Title = "Set up project architecture",
                Description = "Configure monorepo, DI, EF Core, and accessibility tooling.",
                Priority = TaskPriority.High, Status = Core.Entities.TaskStatus.Done,
                CreatedAt = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(-3),
                CreatedBy = "Architect"
            },
            new TaskItem
            {
                Id = 2, Title = "Implement WCAG 2.1 AA compliance",
                Description = "Ensure all pages pass axe-core rules, keyboard navigation, and screen reader support.",
                Priority = TaskPriority.High, Status = Core.Entities.TaskStatus.InProgress,
                CreatedAt = DateTime.UtcNow.AddDays(-3), DueDate = DateTime.UtcNow.AddDays(2),
                CreatedBy = "Developer"
            },
            new TaskItem
            {
                Id = 3, Title = "Write unit and integration tests",
                Description = "Cover all service methods, repository operations, and controller actions.",
                Priority = TaskPriority.Medium, Status = Core.Entities.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow.AddDays(-1), DueDate = DateTime.UtcNow.AddDays(5),
                CreatedBy = "QA Engineer"
            },
            new TaskItem
            {
                Id = 4, Title = "Setup ADO CI/CD pipeline",
                Description = "Build, test, accessibility scan, and deploy pipeline using Azure DevOps YAML.",
                Priority = TaskPriority.Medium, Status = Core.Entities.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(7),
                CreatedBy = "DevOps"
            },
            new TaskItem
            {
                Id = 5, Title = "Conduct JAWS screen reader audit",
                Description = "Verify all interactive elements are properly announced by JAWS screen reader.",
                Priority = TaskPriority.Low, Status = Core.Entities.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(10),
                CreatedBy = "Accessibility Tester"
            }
        );
    }
}
