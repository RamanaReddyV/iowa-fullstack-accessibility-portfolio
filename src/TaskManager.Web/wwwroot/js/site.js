/**
 * TaskFlow — Accessible JavaScript
 * WCAG 2.1 AA / Section 508 compliant interactions
 *
 * Features:
 *  - Keyboard navigation enhancement
 *  - ARIA live region announcements
 *  - Focus management after dynamic updates
 *  - Accessible form validation feedback
 *  - Screen reader announcements
 */

'use strict';

(function () {

  // ── ARIA Live Region Announcer ───────────────────────────────────────────────
  const liveRegion = document.getElementById('live-region');

  function announce(message, priority) {
    if (!liveRegion) return;
    // Clear then re-set triggers screen reader announcement
    liveRegion.setAttribute('aria-live', priority === 'assertive' ? 'assertive' : 'polite');
    liveRegion.textContent = '';
    setTimeout(function () {
      liveRegion.textContent = message;
    }, 100);
  }

  // ── Auto-dismiss alerts with screen reader announcement ─────────────────────
  document.querySelectorAll('.alert[role="alert"]').forEach(function (alert) {
    var msgEl = alert.querySelector('div');
    if (msgEl) {
      announce(msgEl.textContent.trim(), 'polite');
    }
    // Auto-dismiss after 6 seconds (not for errors)
    if (!alert.classList.contains('alert-danger')) {
      setTimeout(function () {
        if (alert.parentNode) {
          var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
          if (bsAlert) bsAlert.close();
        }
      }, 6000);
    }
  });

  // ── Keyboard Navigation: Table row Enter/Space to activate first link ────────
  document.querySelectorAll('table tbody tr').forEach(function (row) {
    row.setAttribute('tabindex', '0');
    row.addEventListener('keydown', function (e) {
      if (e.key === 'Enter' || e.key === ' ') {
        var firstLink = row.querySelector('a');
        if (firstLink) {
          e.preventDefault();
          firstLink.click();
        }
      }
    });
    // Add hover/focus styling class
    row.addEventListener('focus', function () {
      row.classList.add('table-active');
    });
    row.addEventListener('blur', function () {
      row.classList.remove('table-active');
    });
  });

  // ── Form Validation Accessibility Enhancement ────────────────────────────────
  // Mark invalid fields with aria-invalid and move focus to first error
  document.querySelectorAll('form[novalidate]').forEach(function (form) {
    form.addEventListener('submit', function () {
      setTimeout(function () {
        var firstInvalid = form.querySelector('.input-validation-error, [aria-invalid="true"]');
        if (firstInvalid) {
          firstInvalid.focus();
          announce('Form has validation errors. Please review the highlighted fields.', 'assertive');
          // Mark all invalid fields
          form.querySelectorAll('.input-validation-error').forEach(function (el) {
            el.setAttribute('aria-invalid', 'true');
          });
        }
      }, 100);
    });
  });

  // ── Search form: announce result count ──────────────────────────────────────
  var searchForm = document.querySelector('form[role="search"]');
  if (searchForm) {
    // On page load if filters are active, announce result count
    var taskCount = document.querySelectorAll('table tbody tr, [role="listitem"]').length;
    var filterActive = document.querySelector('a[aria-label="Clear all filters"]');
    if (filterActive) {
      announce(taskCount + ' task' + (taskCount !== 1 ? 's' : '') + ' found.', 'polite');
    }
  }

  // ── Focus main content on skip link click ───────────────────────────────────
  var skipLink = document.querySelector('.skip-link');
  if (skipLink) {
    skipLink.addEventListener('click', function (e) {
      var target = document.getElementById('main-content');
      if (target) {
        e.preventDefault();
        target.focus();
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    });
  }

  // ── Navbar toggle: manage aria-expanded ─────────────────────────────────────
  var navToggler = document.querySelector('.navbar-toggler');
  var navCollapse = document.querySelector('.navbar-collapse');
  if (navToggler && navCollapse) {
    navCollapse.addEventListener('show.bs.collapse', function () {
      navToggler.setAttribute('aria-expanded', 'true');
      announce('Navigation menu expanded', 'polite');
    });
    navCollapse.addEventListener('hide.bs.collapse', function () {
      navToggler.setAttribute('aria-expanded', 'false');
      announce('Navigation menu collapsed', 'polite');
    });
  }

  // ── Confirm destructive actions (keyboard-safe) ──────────────────────────────
  // Already handled server-side via DELETE confirmation page — no JS confirm() needed

  // ── Add keyboard shortcut hints (non-intrusive) ──────────────────────────────
  // Alt+N = New Task, Alt+H = Home
  document.addEventListener('keydown', function (e) {
    if (e.altKey && !e.ctrlKey && !e.metaKey) {
      if (e.key === 'n' || e.key === 'N') {
        var newTaskLink = document.querySelector('[aria-label="Create a new task"]');
        if (newTaskLink) { e.preventDefault(); newTaskLink.click(); }
      }
      if (e.key === 'h' || e.key === 'H') {
        var homeLink = document.querySelector('.navbar-brand');
        if (homeLink) { e.preventDefault(); homeLink.click(); }
      }
    }
  });

})();
