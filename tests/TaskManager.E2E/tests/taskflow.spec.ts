import { test, expect } from '@playwright/test';

/**
 * TaskFlow E2E Tests
 * Tests full user journeys including keyboard navigation (WCAG 2.1)
 */

const BASE = 'http://localhost:5001';

test.describe('Task List Page', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE}/Tasks`);
  });

  test('should load and display task list heading', async ({ page }) => {
    await expect(page).toHaveTitle(/My Tasks/);
    await expect(page.getByRole('heading', { name: 'My Tasks' })).toBeVisible();
  });

  test('should have skip navigation link', async ({ page }) => {
    const skipLink = page.locator('.skip-link');
    await expect(skipLink).toBeAttached();
    // Tab to focus it
    await page.keyboard.press('Tab');
    await expect(skipLink).toBeFocused();
  });

  test('should navigate to main content via skip link (keyboard)', async ({ page }) => {
    await page.keyboard.press('Tab');
    await page.keyboard.press('Enter');
    const main = page.locator('#main-content');
    await expect(main).toBeFocused();
  });

  test('should show KPI summary cards', async ({ page }) => {
    await expect(page.getByRole('region', { name: /Total tasks/i })).toBeVisible();
    await expect(page.getByRole('region', { name: /To-do tasks/i })).toBeVisible();
  });

  test('should filter tasks by search query using keyboard only', async ({ page }) => {
    const searchInput = page.locator('#searchInput');
    await searchInput.focus();
    await searchInput.fill('WCAG');
    await page.keyboard.press('Enter');
    await expect(page.url()).toContain('search=WCAG');
  });

  test('should have accessible table with headers', async ({ page }) => {
    // Table should have column headers with scope
    const headers = page.locator('table th[scope="col"]');
    const count = await headers.count();
    expect(count).toBeGreaterThan(0);
  });

  test('should navigate to create task via keyboard shortcut Alt+N', async ({ page }) => {
    await page.keyboard.press('Alt+n');
    await expect(page).toHaveURL(/.*Tasks\/Create.*/);
  });
});

test.describe('Create Task Page', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Create`);
  });

  test('should focus title field on load (autofocus)', async ({ page }) => {
    const titleInput = page.locator('#Title');
    await expect(titleInput).toBeFocused();
  });

  test('should show validation error when submitting empty title', async ({ page }) => {
    await page.click('button[type="submit"]');
    const errorMsg = page.locator('#titleError');
    await expect(errorMsg).toBeVisible();
    await expect(errorMsg).toContainText('required');
  });

  test('should complete create task form using keyboard only', async ({ page }) => {
    // Tab through form fields
    await page.keyboard.press('Tab'); // Focus on title (already focused)
    await page.locator('#Title').fill('Keyboard-navigated Task');
    await page.keyboard.press('Tab');
    await page.locator('#Description').fill('Created via keyboard only');
    await page.keyboard.press('Tab');
    // Priority select
    await page.keyboard.press('Tab');
    // Status select
    await page.keyboard.press('Tab');
    // Due date
    await page.keyboard.press('Tab');
    // Created by
    await page.locator('#CreatedBy').fill('Test Runner');
    await page.keyboard.press('Tab');
    // Submit via Enter on button
    await page.keyboard.press('Enter');
    await expect(page).toHaveURL(`${BASE}/Tasks`);
  });

  test('should have proper aria labels on all form inputs', async ({ page }) => {
    const titleInput = page.locator('#Title');
    await expect(titleInput).toHaveAttribute('aria-required', 'true');
    await expect(titleInput).toHaveAttribute('aria-describedby', /titleError/);
  });
});

test.describe('Task Details Page', () => {
  test('should display task details with proper landmarks', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Details/1`);
    await expect(page.getByRole('article')).toBeVisible();
    await expect(page.locator('nav[aria-label="Breadcrumb"]')).toBeVisible();
  });
});

test.describe('Task Delete Confirmation', () => {
  test('should show confirmation page with accessible warning', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Delete/1`);
    await expect(page.getByRole('heading', { name: 'Confirm Delete' })).toBeVisible();
    await expect(page.getByRole('alert')).toBeVisible();
  });

  test('should have cancel button accessible by keyboard', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Delete/1`);
    const cancelBtn = page.getByRole('link', { name: /Cancel/i });
    await expect(cancelBtn).toBeVisible();
    await cancelBtn.focus();
    await expect(cancelBtn).toBeFocused();
  });
});

test.describe('Responsive Layout', () => {
  test('should display mobile card view on small screens', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 });
    await page.goto(`${BASE}/Tasks`);
    // Mobile card view is shown (d-md-none)
    const mobileList = page.locator('[aria-label="Task list"]').first();
    await expect(mobileList).toBeVisible();
  });

  test('should display desktop table on large screens', async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 800 });
    await page.goto(`${BASE}/Tasks`);
    const table = page.locator('table[aria-label="Task list"]');
    await expect(table).toBeVisible();
  });
});
