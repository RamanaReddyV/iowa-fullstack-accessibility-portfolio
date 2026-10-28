# TaskFlow — Accessible Task Management Application

[![WCAG 2.1 AA](https://img.shields.io/badge/WCAG_2.1-AA_Compliant-green)](accessibility-reports/ACCESSIBILITY-REPORT.md)
[![Section 508](https://img.shields.io/badge/Section_508-Compliant-blue)](accessibility-reports/ACCESSIBILITY-REPORT.md)
[![Unit Tests](https://img.shields.io/badge/Unit_Tests-21%2F21-brightgreen)]()
[![Integration Tests](https://img.shields.io/badge/Integration_Tests-20%2F20-brightgreen)]()
[![Axe Tests](https://img.shields.io/badge/axe--core_Tests-13%2F13-brightgreen)]()

> **Monorepo** · **ASP.NET MVC** · **EF Core InMemory** · **WCAG 2.1 AA** · **Section 508** · **JAWS Compatible** · **Playwright E2E** · **axe-core** · **Azure DevOps CI/CD**

---

## Overview

**TaskFlow** is a full-stack accessible task management web application built as a demonstration of:

- ✅ **WCAG 2.1 AA** and **Section 508** accessibility compliance
- ✅ **ASP.NET MVC** with clean architecture (Core / Infrastructure / Web)
- ✅ **EF Core InMemory** database for demo purposes
- ✅ **Responsive UI** (Bootstrap 5 + Material Icons + custom WCAG-compliant CSS)
- ✅ **Screen reader support** (JAWS + NVDA compatible)
- ✅ **Full keyboard navigation** with focus management
- ✅ **Automated accessibility testing** with axe-core (all pages pass)
- ✅ **Unit tests** (xUnit + FluentAssertions + Moq)
- ✅ **Integration tests** (Microsoft.Mvc.Testing)
- ✅ **E2E tests** (Playwright + TypeScript)
- ✅ **Azure DevOps CI/CD pipeline** (5-stage YAML)

---

## Architecture

```
iowa-fullstack-accessibility-portfolio/
│
├── src/
│   ├── TaskManager.Core/              # Domain layer (entities, interfaces, services)
│   ├── TaskManager.Infrastructure/    # EF Core InMemory, repositories, DI extensions
│   └── TaskManager.Web/               # ASP.NET MVC application
│       ├── Controllers/TasksController.cs
│       ├── Models/ (ViewModels)
│       ├── Views/Tasks/ (CRUD views)
│       └── wwwroot/ (Bootstrap 5, Material Icons, WCAG CSS, accessible JS)
│
├── tests/
│   ├── TaskManager.UnitTests/         # xUnit: 21 service + repository tests
│   ├── TaskManager.IntegrationTests/  # xUnit + Mvc.Testing: 20 HTTP pipeline tests
│   ├── TaskManager.E2E/               # Playwright TypeScript E2E tests
│   └── TaskManager.AccessibilityTests/ # axe-core WCAG 2.1 AA automated scans
│
├── pipeline/
│   └── azure-pipelines.yml            # 5-stage ADO CI/CD pipeline
│
├── docs/
│   └── architecture/ARCHITECTURE.md   # Full architecture + WCAG checklist
│
└── accessibility-reports/
    ├── ACCESSIBILITY-REPORT.md        # Test summary and findings
    └── axe-*.json                     # Raw axe-core JSON per page
```

See the full architecture diagram in [docs/architecture/ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md).

---

## Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (for E2E / accessibility tests)

### Run the Application

```bash
dotnet run --project src/TaskManager.Web/TaskManager.Web.csproj
# App starts at http://localhost:5000
```

### Run All .NET Tests

```bash
dotnet test --configuration Release
# 21 unit + 20 integration tests
```

### Run Accessibility Tests (axe-core)

```bash
# 1. Start the app (in another terminal)
dotnet run --project src/TaskManager.Web/ --urls http://localhost:5001

# 2. Install Playwright browsers (first time only)
cd tests/TaskManager.AccessibilityTests
npm install
npx playwright install chromium

# 3. Run axe-core tests
npx playwright test
# Reports saved to accessibility-reports/
```

### Run E2E Tests (Playwright)

```bash
# 1. Start the app
dotnet run --project src/TaskManager.Web/ --urls http://localhost:5001

# 2. Run E2E tests
cd tests/TaskManager.E2E
npm install
npx playwright install chromium
npx playwright test
```

---

## Test Results

| Test Suite | Tests | Result |
|-----------|-------|--------|
| Unit Tests (xUnit) | 21 | ✅ All Pass |
| Integration Tests (xUnit + Mvc.Testing) | 20 | ✅ All Pass |
| Accessibility Tests (axe-core) | 13 | ✅ All Pass (0 violations) |
| E2E Tests (Playwright) | 14+ | ✅ All Pass |

---

## Accessibility Features

### WCAG 2.1 AA Compliance

| Category | Implementation |
|----------|---------------|
| **Skip Navigation** | `<a href="#main-content" class="skip-link">` (WCAG 2.4.1) |
| **Keyboard Navigation** | All interactions operable via Tab/Enter/Space |
| **Focus Indicators** | 3px outline + box-shadow on `:focus-visible` (WCAG 2.4.7) |
| **ARIA Landmarks** | `banner`, `main`, `nav`, `contentinfo` roles |
| **ARIA Live Regions** | `aria-live="polite"` for status messages (WCAG 4.1.3) |
| **Color Contrast** | All text ≥4.5:1 ratio (verified by axe-core) |
| **Form Labels** | All inputs have associated `<label for>` (WCAG 1.3.1) |
| **Error Identification** | `aria-invalid`, `aria-describedby` on validation (WCAG 3.3.1) |
| **Table Headers** | `scope="col"` and `<caption>` on data tables (WCAG 1.3.1) |
| **Language** | `<html lang="en">` (WCAG 3.1.1) |
| **Unique Page Titles** | Dynamic `<title>` per page (WCAG 2.4.2) |
| **Responsive** | Bootstrap 5 grid, mobile card view, no horizontal scroll |
| **Reduced Motion** | `@media (prefers-reduced-motion: reduce)` CSS |
| **High Contrast** | `@media (forced-colors: active)` CSS support |

### Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Tab` | Navigate to next interactive element |
| `Shift+Tab` | Navigate to previous interactive element |
| `Enter` / `Space` | Activate focused element |
| `Alt+N` | Create new task |
| `Alt+H` | Go to home (task list) |

### Screen Reader Tested Features

- Page title announced on load
- All landmark regions labeled
- Form required fields announced
- Validation errors announced via live region
- Table column headers announced with scope
- Decorative icons hidden with `aria-hidden="true"`
- Status badges provide contextual `aria-label`

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 10.0 |
| Framework | ASP.NET Core MVC 9.x |
| ORM | Entity Framework Core 9.0.5 |
| Database | EF Core InMemory (demo) |
| UI | Bootstrap 5.3, Material Icons, Custom WCAG CSS |
| Language (C#) | C# 12 (net10.0) |
| Unit Testing | xUnit 2.9.3 + FluentAssertions 6.12.2 + Moq 4.20 |
| Integration Testing | Microsoft.AspNetCore.Mvc.Testing 9.0.5 |
| E2E Testing | Playwright 1.49.0 (TypeScript) |
| Accessibility Testing | axe-core 4.10 via @axe-core/playwright |
| CI/CD | Azure DevOps Pipelines (YAML) |

---

## Accessibility Reports

All reports are in [`accessibility-reports/`](accessibility-reports/):

- **[ACCESSIBILITY-REPORT.md](accessibility-reports/ACCESSIBILITY-REPORT.md)** — Full summary
- `axe-tasks-index.json` — Desktop task list (0 violations)
- `axe-tasks-index-mobile.json` — Mobile task list (0 violations)
- `axe-tasks-create.json` — Create form (0 violations)
- `axe-tasks-create-validation.json` — Create form with errors (0 violations)
- `axe-tasks-edit.json` — Edit form (0 violations)
- `axe-tasks-details.json` — Task details (0 violations)
- `axe-tasks-delete.json` — Delete confirmation (0 violations)

---

## CI/CD Pipeline

The [`pipeline/azure-pipelines.yml`](pipeline/azure-pipelines.yml) defines a 5-stage pipeline:

1. **Build** — Restore, build, publish web artifact
2. **Unit & Integration Tests** — xUnit with code coverage
3. **Accessibility & E2E Tests** — axe-core WCAG scan + Playwright E2E (fails on critical violations)
4. **Deploy to Staging** — Azure App Service staging slot
5. **Deploy to Production** — Manual approval gate + slot swap

---

## Architecture Documentation

- [docs/architecture/ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md) — Full Mermaid diagram, tech stack table, WCAG criteria table, JAWS checklist, keyboard map

---

## License

MIT — See [LICENSE](LICENSE) for details.
