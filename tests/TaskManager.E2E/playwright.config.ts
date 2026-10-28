import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  retries: 1,
  reporter: [
    ['html', { outputFolder: '../../accessibility-reports/e2e-html-report', open: 'never' }],
    ['json', { outputFile: '../../accessibility-reports/e2e-results.json' }],
    ['list']
  ],
  use: {
    baseURL: 'http://localhost:5001',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'off',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
