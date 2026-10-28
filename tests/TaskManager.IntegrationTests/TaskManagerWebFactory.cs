using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory that replaces EF with a fresh InMemory DB per test run
/// and seeds deterministic test data so seeded IDs (1-5) are always available.
/// </summary>
public class TaskManagerWebFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TaskDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Replace with an isolated InMemory DB (shared name so seed runs once)
            services.AddDbContext<TaskDbContext>(opts =>
                opts.UseInMemoryDatabase("IntegrationTest_Shared"));
        });

        builder.UseEnvironment("Test");
    }

    /// <summary>Seed deterministic tasks into the test database.</summary>
    public void SeedTestData()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
        db.Database.EnsureCreated();

        if (!db.Tasks.Any())
        {
            db.Tasks.AddRange(
                new TaskItem
                {
                    Id = 1, Title = "Set up project architecture",
                    Description = "Configure monorepo, DI, EF Core.",
                    Priority = TaskPriority.High,
                    Status = Core.Entities.TaskStatus.Done,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedBy = "Architect"
                },
                new TaskItem
                {
                    Id = 2, Title = "Implement WCAG 2.1 AA compliance",
                    Description = "Ensure all pages pass axe-core rules.",
                    Priority = TaskPriority.High,
                    Status = Core.Entities.TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    CreatedBy = "Developer"
                },
                new TaskItem
                {
                    Id = 3, Title = "Write unit and integration tests",
                    Description = "Cover all service methods.",
                    Priority = TaskPriority.Medium,
                    Status = Core.Entities.TaskStatus.Todo,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = "QA Engineer"
                }
            );
            db.SaveChanges();
        }
    }
}
