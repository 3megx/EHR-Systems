/**
 * E2E Test Suite: Authentication Workflow
 * Tests critical user authentication flows
 */

describe('Authentication Workflow', () => {
  const apiBaseUrl = 'http://localhost:4200';
  const mockUser = {
    email: 'doctor@ehr.com',
    password: 'Test1234!@',
  };

  beforeEach(() => {
    // Clear browser storage
    localStorage.clear();
    sessionStorage.clear();
  });

  describe('Login Flow', () => {
    it('should display login page', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.contains('Sign In').should('be.visible');
      cy.get('input[type="email"]').should('exist');
      cy.get('input[type="password"]').should('exist');
    });

    it('should show validation errors for empty form', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.contains('button', 'Sign In').click();
      cy.contains('required').should('be.visible');
    });

    it('should show validation error for invalid email', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.get('input[type="email"]').type('invalid-email');
      cy.get('input[type="password"]').type(mockUser.password);
      cy.contains('button', 'Sign In').click();
      cy.contains('valid email').should('be.visible');
    });

    it('should login successfully with valid credentials', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.get('input[type="email"]').type(mockUser.email);
      cy.get('input[type="password"]').type(mockUser.password);
      cy.contains('button', 'Sign In').click();

      // Should redirect to dashboard
      cy.url().should('include', '/dashboard');
      cy.contains('Welcome back').should('be.visible');
    });

    it('should show error for incorrect password', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.get('input[type="email"]').type(mockUser.email);
      cy.get('input[type="password"]').type('wrongpassword');
      cy.contains('button', 'Sign In').click();
      cy.contains('Login failed').should('be.visible');
    });

    it('should remember user when checkbox selected', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.get('input[type="email"]').type(mockUser.email);
      cy.get('input[type="password"]').type(mockUser.password);
      cy.get('input[type="checkbox"]').check();
      cy.contains('button', 'Sign In').click();

      // Check localStorage for remember-me token
      cy.window().then((win) => {
        const rememberMe = win.localStorage.getItem('remember_me');
        expect(rememberMe).to.exist;
      });
    });
  });

  describe('Session Management', () => {
    it('should maintain session after login', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.get('input[type="email"]').type(mockUser.email);
      cy.get('input[type="password"]').type(mockUser.password);
      cy.contains('button', 'Sign In').click();

      // Verify token is stored
      cy.window().then((win) => {
        const token = win.localStorage.getItem('auth_token');
        expect(token).to.exist;
      });

      // Should stay on dashboard on page refresh
      cy.reload();
      cy.url().should('include', '/dashboard');
    });

    it('should redirect to login when accessing protected route without auth', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.url().should('include', '/auth/login');
    });

    it('should logout successfully', () => {
      // Login first
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.get('input[type="email"]').type(mockUser.email);
      cy.get('input[type="password"]').type(mockUser.password);
      cy.contains('button', 'Sign In').click();

      // Click logout button
      cy.contains('button', 'Logout').click();

      // Should redirect to login
      cy.url().should('include', '/auth/login');

      // Token should be cleared
      cy.window().then((win) => {
        const token = win.localStorage.getItem('auth_token');
        expect(token).to.be.null;
      });
    });
  });

  describe('Password Reset', () => {
    it('should navigate to forgot password page', () => {
      cy.visit(`${apiBaseUrl}/auth/login`);
      cy.contains('a', 'Forgot?').click();
      cy.url().should('include', '/auth/forgot-password');
    });

    it('should submit forgot password form', () => {
      cy.visit(`${apiBaseUrl}/auth/forgot-password`);
      cy.get('input[type="email"]').type(mockUser.email);
      cy.contains('button', 'Reset Password').click();
      cy.contains('Check your email').should('be.visible');
    });
  });
});
