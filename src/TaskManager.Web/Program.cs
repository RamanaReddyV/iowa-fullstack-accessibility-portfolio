using TaskManager.Infrastructure.Extensions;
using TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Views
builder.Services.AddControllersWithViews();

// Infrastructure (EF Core InMemory + Services)
builder.Services.AddInfrastructure("TaskManagerDemo");

// Antiforgery
builder.Services.AddAntiforgery(opts => opts.HeaderName = "X-CSRF-Token");

var app = builder.Build();

// Seed InMemory DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.Run();

// Expose Program for integration testing
public partial class Program { }
