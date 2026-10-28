# TaskFlow Architecture Diagram

```mermaid
graph TB
    subgraph "Monorepo: iowa-fullstack-accessibility-portfolio"

        subgraph "src/ — Application Code"
            CORE["TaskManager.Core\n(Class Library, net10.0)\n\n• Entities: TaskItem, TaskPriority, TaskStatus\n• Interfaces: ITaskRepository, ITaskService\n• Services: TaskService\n(Pure domain logic — no infrastructure deps)"]

            INFRA["TaskManager.Infrastructure\n(Class Library, net10.0)\n\n• Data: TaskDbContext (EF Core)\n• Repositories: TaskRepository\n• Extensions: ServiceCollectionExtensions\n• InMemory DB (demo) / SQL Server (prod)"]

            WEB["TaskManager.Web\n(ASP.NET MVC, net10.0)\n\n• Controllers: TasksController\n• Views: Index, Create, Edit, Details, Delete\n• ViewModels: TaskItemViewModel, TaskListViewModel\n• Program.cs (top-level)\n• wwwroot: Bootstrap5, Material Icons,\n  SASS→CSS, Accessible JS"]
        end

        subgraph "tests/ — Test Projects"
            UNIT["TaskManager.UnitTests\n(xUnit, net10.0)\n\n• TaskServiceTests (18 tests)\n• TaskRepositoryTests (4 tests)\n• Moq + FluentAssertions\n• InMemory EF per test"]

            INTEG["TaskManager.IntegrationTests\n(xUnit + Mvc.Testing, net10.0)\n\n• TasksControllerIntegrationTests\n• Full HTTP pipeline tests\n• Accessibility HTML assertions\n• 20 tests"]

            E2E["TaskManager.E2E\n(Playwright + TypeScript)\n\n• taskflow.spec.ts\n• Full browser E2E tests\n• Keyboard navigation tests\n• Responsive viewport tests\n• Reports → accessibility-reports/"]

            A11Y["TaskManager.AccessibilityTests\n(axe-core + Playwright)\n\n• accessibility.spec.ts\n• WCAG 2.1 AA scanning\n• Section 508 compliance\n• All 5 pages scanned\n• Keyboard focus tests\n• Screen reader checks\n• Reports → accessibility-reports/"]
        end

        subgraph "pipeline/ — CI/CD"
            PIPE["azure-pipelines.yml\n(Azure DevOps)\n\nStage 1: Build\n  - Restore, build, publish\nStage 2: Unit & Integration Tests\n  - xUnit + code coverage\nStage 3: Accessibility & E2E\n  - axe-core WCAG scan\n  - Playwright E2E\n  - Fail on critical violations\nStage 4: Deploy Staging\n  - Azure App Service slot\nStage 5: Deploy Production\n  - Manual approval gate"]
        end

        subgraph "docs/ — Architecture"
            DOCS["docs/architecture/\n• ARCHITECTURE.md (this file)\n• architecture-diagram.md (Mermaid)\n• ACCESSIBILITY.md\n• JAWS-checklist.md"]
        end

        subgraph "accessibility-reports/ — Reports"
            REPORTS["accessibility-reports/\n• axe-tasks-index.json\n• axe-tasks-create.json\n• axe-tasks-edit.json\n• axe-tasks-details.json\n• axe-tasks-delete.json\n• axe-html-report/\n• e2e-results.json\n• e2e-html-report/"]
        end
    end

    %% Dependencies
    INFRA -->|references| CORE
    WEB   -->|references| CORE
    WEB   -->|references| INFRA
    UNIT  -->|tests| CORE
    UNIT  -->|tests| INFRA
    INTEG -->|tests via WebApplicationFactory| WEB
    E2E   -->|E2E via HTTP| WEB
    A11Y  -->|axe scan via HTTP| WEB

    %% Pipe orchestrates everything
    PIPE  -->|runs| UNIT
    PIPE  -->|runs| INTEG
    PIPE  -->|runs| E2E
    PIPE  -->|runs| A11Y
    PIPE  -->|deploys| WEB
    PIPE  -->|saves| REPORTS
```

## Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Runtime | .NET | 10.0 |
| Web Framework | ASP.NET Core MVC | 9.0.5 |
| ORM | Entity Framework Core | 9.0.5 |
| Database (demo) | EF Core InMemory | 9.0.5 |
| UI Framework | Bootstrap | 5.3 (bundled) |
| UI Icons | Material Icons | Google CDN |
| Fonts | Roboto (Google Fonts) | CDN |
| Styling | CSS (WCAG-compliant) | Custom |
| Unit Testing | xUnit | 2.9.3 |
| Assertions | FluentAssertions | 6.12.2 |
| Mocking | Moq | 4.20.72 |
| Integration Testing | Microsoft.Mvc.Testing | 9.0.5 |
| E2E Testing | Playwright | 1.49.0 |
| Accessibility Scanning | axe-core / @axe-core/playwright | 4.10.0 |
| CI/CD | Azure DevOps Pipelines | YAML v1 |

## Accessibility Standards Implemented

| Standard | Criteria | Implementation |
|----------|----------|----------------|
| WCAG 2.1 AA | 1.1.1 Non-text Content | `aria-hidden` on decorative icons, `alt` on images |
| WCAG 2.1 AA | 1.3.1 Info & Relationships | Semantic HTML, ARIA roles, `<table>` with `scope` |
| WCAG 2.1 AA | 1.4.3 Contrast (min 4.5:1) | All colors verified; custom CSS tokens |
| WCAG 2.1 AA | 1.4.4 Resize Text | Fluid layout, rem units |
| WCAG 2.1 AA | 1.4.10 Reflow | Responsive Bootstrap grid, no horizontal scroll |
| WCAG 2.1 AA | 1.4.11 Non-text Contrast | UI component borders at 3:1 |
| WCAG 2.1 AA | 2.1.1 Keyboard | All interactions keyboard-operable |
| WCAG 2.1 AA | 2.4.1 Bypass Blocks | Skip Navigation link |
| WCAG 2.1 AA | 2.4.2 Page Titled | Unique `<title>` per page |
| WCAG 2.1 AA | 2.4.4 Link Purpose | Descriptive `aria-label` on all links |
| WCAG 2.1 AA | 2.4.7 Focus Visible | 3px outline + box-shadow on `:focus-visible` |
| WCAG 2.1 AA | 3.1.1 Language of Page | `<html lang="en">` |
| WCAG 2.1 AA | 3.3.1 Error Identification | `aria-invalid`, `aria-describedby` on errors |
| WCAG 2.1 AA | 3.3.2 Labels or Instructions | All form inputs have visible `<label>` |
| WCAG 2.1 AA | 4.1.2 Name, Role, Value | ARIA attributes on all interactive elements |
| WCAG 2.1 AA | 4.1.3 Status Messages | `aria-live="polite"` live regions |
| Section 508 | 1194.22 (a) | Alt text on images |
| Section 508 | 1194.22 (l) | No auto-play, reduced motion support |
| Section 508 | 1194.22 (n) | Forms have accessible labels |
| Section 508 | 1194.22 (o) | Skip navigation provided |

## JAWS Screen Reader Checklist

- [x] Page title announced on load
- [x] Landmark regions announced (banner, main, contentinfo, navigation)
- [x] Skip link available and functional
- [x] All headings in logical hierarchy (H1 → H2)
- [x] Form labels correctly associated via `for`/`id`
- [x] Required fields announced as required (`aria-required`)
- [x] Error messages announced via `aria-live`
- [x] Table headers announced with scope
- [x] Buttons have descriptive labels
- [x] Links have descriptive text (no "click here")
- [x] Status badges use `aria-label` for context
- [x] Focus order matches visual order
- [x] Modal/confirmation pages follow focus management

## Keyboard Navigation Map

| Key | Action |
|-----|--------|
| `Tab` | Move to next interactive element |
| `Shift+Tab` | Move to previous interactive element |
| `Enter` / `Space` | Activate button or link |
| `Alt+N` | New Task shortcut |
| `Alt+H` | Go Home (navbar brand) |
| `Escape` | Close dropdowns/modals |
| Arrow keys | Navigate within select dropdowns |
