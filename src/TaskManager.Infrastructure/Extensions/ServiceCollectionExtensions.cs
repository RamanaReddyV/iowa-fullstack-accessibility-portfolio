using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Services;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string dbName = "TaskManagerInMemory")
    {
        services.AddDbContext<TaskDbContext>(opts =>
            opts.UseInMemoryDatabase(dbName));

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}
