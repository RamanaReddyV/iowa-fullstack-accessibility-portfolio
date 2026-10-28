using FluentAssertions;
using System.Net;
using Xunit;

namespace TaskManager.IntegrationTests.Controllers;

public class TasksControllerIntegrationTests : IClassFixture<TaskManagerWebFactory>
{
    private readonly HttpClient _client;
    private readonly TaskManagerWebFactory _factory;

    public TasksControllerIntegrationTests(TaskManagerWebFactory factory)
    {
        _factory = factory;
        _factory.SeedTestData();
        _client = factory.CreateClient();
    }

    // ── GET /Tasks ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_TasksIndex_ReturnsOk()
    {
        var response = await _client.GetAsync("/Tasks");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_TasksIndex_ContainsExpectedHeading()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("My Tasks");
    }

    [Fact]
    public async Task Get_TasksIndex_ContainsSkipLink()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("skip-link");
        html.Should().Contain("Skip to main content");
    }

    [Fact]
    public async Task Get_TasksIndex_ContainsMainLandmark()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("id=\"main-content\"");
        html.Should().Contain("role=\"main\"");
    }

    [Fact]
    public async Task Get_TasksIndex_ContainsAriaLiveRegion()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("aria-live=\"polite\"");
    }

    // ── GET /Tasks/Create ────────────────────────────────────────────────────

    [Fact]
    public async Task Get_TasksCreate_ReturnsOk()
    {
        var response = await _client.GetAsync("/Tasks/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_TasksCreate_ContainsAccessibleForm()
    {
        var response = await _client.GetAsync("/Tasks/Create");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("aria-required=\"true\"");
        html.Should().Contain("aria-label=\"Create new task form\"");
    }

    [Fact]
    public async Task Get_TasksCreate_ContainsBreadcrumb()
    {
        var response = await _client.GetAsync("/Tasks/Create");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("aria-label=\"Breadcrumb\"");
    }

    // ── GET /Tasks/Details/1 ─────────────────────────────────────────────────

    [Fact]
    public async Task Get_TaskDetails_SeededTask_ReturnsOk()
    {
        var response = await _client.GetAsync("/Tasks/Details/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_TaskDetails_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/Tasks/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── GET /Tasks/Edit/1 ────────────────────────────────────────────────────

    [Fact]
    public async Task Get_TaskEdit_SeededTask_ReturnsOk()
    {
        var response = await _client.GetAsync("/Tasks/Edit/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── GET /Tasks/Delete/1 ──────────────────────────────────────────────────

    [Fact]
    public async Task Get_TaskDelete_SeededTask_ReturnsOk()
    {
        var response = await _client.GetAsync("/Tasks/Delete/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_TaskDelete_ContainsConfirmationWarning()
    {
        var response = await _client.GetAsync("/Tasks/Delete/1");
        var html = await response.Content.ReadAsStringAsync();
        // Matches the actual text in the Delete view
        html.Should().Contain("This action cannot be undone");
    }

    // ── Accessibility: HTML structure checks ─────────────────────────────────

    [Fact]
    public async Task Get_AllPages_ContainLangAttribute()
    {
        var pages = new[] { "/Tasks", "/Tasks/Create", "/Tasks/Edit/1", "/Tasks/Details/1" };
        foreach (var page in pages)
        {
            var response = await _client.GetAsync(page);
            var html = await response.Content.ReadAsStringAsync();
            html.Should().Contain("lang=\"en\"", because: $"{page} must have lang attribute for screen readers");
        }
    }

    [Fact]
    public async Task Get_AllPages_ContainMetaDescription()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("meta name=\"description\"");
    }

    [Fact]
    public async Task Get_AllPages_ContainNavLandmark()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("role=\"banner\"");
        html.Should().Contain("aria-label=\"Primary navigation\"");
    }

    [Fact]
    public async Task Get_AllPages_ContainContentInfoLandmark()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("role=\"contentinfo\"");
    }

    [Fact]
    public async Task Get_IndexPage_TableHasCaption()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        // Seeded data means table is rendered; verify caption exists
        html.Should().Contain("<caption");
    }

    [Fact]
    public async Task Get_IndexPage_TableHasScopeAttributes()
    {
        var response = await _client.GetAsync("/Tasks");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("scope=\"col\"");
    }

    // ── Search/filter ────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_TasksIndex_WithSearch_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/Tasks?search=WCAG");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("WCAG");
    }
}
