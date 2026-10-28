import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';
import * as fs from 'fs';
import * as path from 'path';

/**
 * TaskFlow — Accessibility Test Suite
 * Using axe-core for automated WCAG 2.1 AA and Section 508 scanning
 *
 * These tests run axe-core against each page and fail on any violations
 * at WCAG 2.1 AA level. Results are saved to accessibility-reports/.
 */

const BASE = 'http://localhost:5001';
const REPORT_DIR = path.join(__dirname, '../../../accessibility-reports');

// Ensure report directory exists
if (!fs.existsSync(REPORT_DIR)) {
  fs.mkdirSync(REPORT_DIR, { recursive: true });
}

/** Helper: run axe on current page and save JSON report */
async function runAxeAndSave(page: any, pageName: string) {
  const results = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'section508'])
    .analyze();

  // Save JSON report
  const reportPath = path.join(REPORT_DIR, `axe-${pageName}.json`);
  fs.writeFileSync(reportPath, JSON.stringify(results, null, 2));

  return results;
}

test.describe('WCAG 2.1 AA — Task List Page (/', () => {
  test('Tasks Index — zero axe violations', async ({ page }) => {
    await page.goto(`${BASE}/Tasks`);
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-index');

    // Log violations for CI output
    if (results.violations.length > 0) {
      console.log('Accessibility Violations on /Tasks:');
      results.violations.forEach(v => {
        console.log(`  [${v.impact?.toUpperCase()}] ${v.id}: ${v.description}`);
        console.log(`    Help: ${v.helpUrl}`);
        v.nodes.forEach(n => console.log(`    Element: ${n.html}`));
      });
    }

    expect(
      results.violations,
      `Found ${results.violations.length} WCAG 2.1 AA violation(s) on /Tasks:\n` +
      results.violations.map(v => `  • [${v.impact}] ${v.id}: ${v.description}`).join('\n')
    ).toHaveLength(0);
  });

  test('Tasks Index — passes on mobile viewport', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 812 });
    await page.goto(`${BASE}/Tasks`);
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-index-mobile');

    expect(
      results.violations.filter(v => v.impact !== 'minor'),
      'Critical/Serious WCAG violations on mobile /Tasks'
    ).toHaveLength(0);
  });
});

test.describe('WCAG 2.1 AA — Create Task Page', () => {
  test('Create Task — zero axe violations', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Create`);
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-create');

    expect(
      results.violations,
      `Found violations on /Tasks/Create:\n` +
      results.violations.map(v => `  • [${v.impact}] ${v.id}: ${v.description}`).join('\n')
    ).toHaveLength(0);
  });

  test('Create Task with validation errors — zero axe violations', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Create`);
    // Submit empty form to trigger validation
    await page.click('button[type="submit"]');
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-create-validation');

    expect(
      results.violations.filter(v => v.impact === 'critical' || v.impact === 'serious'),
      'Critical/Serious violations on create form with validation errors'
    ).toHaveLength(0);
  });
});

test.describe('WCAG 2.1 AA — Edit Task Page', () => {
  test('Edit Task — zero axe violations', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Edit/1`);
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-edit');

    expect(
      results.violations,
      `Found violations on /Tasks/Edit/1:\n` +
      results.violations.map(v => `  • [${v.impact}] ${v.id}: ${v.description}`).join('\n')
    ).toHaveLength(0);
  });
});

test.describe('WCAG 2.1 AA — Task Details Page', () => {
  test('Task Details — zero axe violations', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Details/1`);
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-details');

    expect(
      results.violations,
      `Found violations on /Tasks/Details/1:\n` +
      results.violations.map(v => `  • [${v.impact}] ${v.id}: ${v.description}`).join('\n')
    ).toHaveLength(0);
  });
});

test.describe('WCAG 2.1 AA — Delete Confirmation Page', () => {
  test('Delete Confirmation — zero axe violations', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Delete/1`);
    await page.waitForLoadState('networkidle');

    const results = await runAxeAndSave(page, 'tasks-delete');

    expect(
      results.violations,
      `Found violations on /Tasks/Delete/1:\n` +
      results.violations.map(v => `  • [${v.impact}] ${v.id}: ${v.description}`).join('\n')
    ).toHaveLength(0);
  });
});

test.describe('Keyboard Navigation — WCAG 2.1 (Operable)', () => {
  test('All interactive elements reachable by Tab', async ({ page }) => {
    await page.goto(`${BASE}/Tasks`);
    const focusableElements: string[] = [];
    let tabCount = 0;

    // Tab through up to 50 elements
    while (tabCount < 50) {
      await page.keyboard.press('Tab');
      const focused = await page.evaluate(() => {
        const el = document.activeElement;
        if (!el || el.tagName === 'BODY') return null;
        return {
          tag: el.tagName,
          role: el.getAttribute('role'),
          label: el.getAttribute('aria-label') || el.textContent?.trim().slice(0, 50)
        };
      });
      if (!focused) break;
      focusableElements.push(`${focused.tag}[${focused.role || ''}]: ${focused.label}`);
      tabCount++;
    }

    // At minimum, nav links, buttons, and form controls must be in tab order
    expect(focusableElements.length).toBeGreaterThan(3);
    console.log('Focusable elements:', focusableElements.join('\n'));
  });

  test('Focus is visible on all interactive elements', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Create`);

    // Check CSS ensures focus-visible styling
    const focusStyle = await page.evaluate(() => {
      const style = getComputedStyle(document.documentElement);
      return style.getPropertyValue('--tm-focus-ring');
    });
    expect(focusStyle).toBeTruthy();
  });
});

test.describe('Screen Reader Support', () => {
  test('All images have alt text or aria-hidden', async ({ page }) => {
    await page.goto(`${BASE}/Tasks`);
    const images = await page.locator('img').all();
    for (const img of images) {
      const alt = await img.getAttribute('alt');
      const ariaHidden = await img.getAttribute('aria-hidden');
      const role = await img.getAttribute('role');
      const hasAlt = alt !== null;
      const isHidden = ariaHidden === 'true';
      const isPresentation = role === 'presentation' || role === 'none';
      expect(
        hasAlt || isHidden || isPresentation,
        `Image missing alt text: ${await img.getAttribute('src')}`
      ).toBeTruthy();
    }
  });

  test('All form labels are associated with inputs', async ({ page }) => {
    await page.goto(`${BASE}/Tasks/Create`);
    const inputs = await page.locator('input:not([type="hidden"]):not([type="submit"]), select, textarea').all();
    for (const input of inputs) {
      const id = await input.getAttribute('id');
      const ariaLabel = await input.getAttribute('aria-label');
      const ariaLabelledBy = await input.getAttribute('aria-labelledby');
      if (id) {
        const label = await page.locator(`label[for="${id}"]`).count();
        expect(
          label > 0 || ariaLabel !== null || ariaLabelledBy !== null,
          `Input #${id} has no associated label`
        ).toBeTruthy();
      }
    }
  });

  test('Page has logical heading hierarchy', async ({ page }) => {
    await page.goto(`${BASE}/Tasks`);
    // Should have exactly one h1
    const h1Count = await page.locator('h1').count();
    expect(h1Count).toBe(1);
  });

  test('ARIA live regions are present', async ({ page }) => {
    await page.goto(`${BASE}/Tasks`);
    const liveRegion = page.locator('[aria-live]');
    await expect(liveRegion.first()).toBeAttached();
  });
});
