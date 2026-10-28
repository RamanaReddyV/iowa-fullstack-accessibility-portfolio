import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  retries: 1,
  reporter: [
    ['html', { outputFolder: '../../accessibility-reports/axe-html-report', open: 'never' }],
    ['json', { outputFile: '../../accessibility-reports/axe-results.json' }],
    ['list']
  ],
  use: {
    baseURL: 'http://localhost:5001',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
