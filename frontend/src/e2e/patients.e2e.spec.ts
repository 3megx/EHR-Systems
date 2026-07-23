/**
 * E2E Test Suite: Patient Management Workflow
 * Tests critical patient data operations
 */

describe('Patient Management Workflow', () => {
  const apiBaseUrl = 'http://localhost:4200';
  const mockDoctor = {
    email: 'doctor@ehr.com',
    password: 'Test1234!@',
  };

  beforeEach(() => {
    // Login before each test
    cy.visit(`${apiBaseUrl}/auth/login`);
    cy.get('input[type="email"]').type(mockDoctor.email);
    cy.get('input[type="password"]').type(mockDoctor.password);
    cy.contains('button', 'Sign In').click();
    cy.url().should('include', '/dashboard');
  });

  describe('Patient List', () => {
    it('should display patient list page', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.contains('Patients').should('be.visible');
      cy.get('[role="table"]').should('exist');
    });

    it('should paginate through patients', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.get('button').contains('Next').should('exist');
      cy.get('button').contains('Previous').should('be.disabled');
    });

    it('should sort patients by column', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.get('th').contains('Name').click();
      cy.get('[aria-sort="ascending"]').should('exist');
    });
  });

  describe('Patient Search', () => {
    it('should navigate to search page', () => {
      cy.visit(`${apiBaseUrl}/patients/search`);
      cy.contains('Patient Search').should('be.visible');
    });

    it('should search patients by name', () => {
      cy.visit(`${apiBaseUrl}/patients/search`);
      cy.get('input[type="text"]').type('Robert');
      cy.get('button').contains('Search').click();
      cy.contains('Robert Wilson').should('be.visible');
    });

    it('should search patients by MRN', () => {
      cy.visit(`${apiBaseUrl}/patients/search`);
      cy.get('input[type="text"]').type('MRN001234');
      cy.get('button').contains('Search').click();
      cy.contains('MRN001234').should('be.visible');
    });

    it('should show no results message', () => {
      cy.visit(`${apiBaseUrl}/patients/search`);
      cy.get('input[type="text"]').type('NonexistentPatient12345');
      cy.get('button').contains('Search').click();
      cy.contains('No results').should('be.visible');
    });
  });

  describe('Patient Details', () => {
    it('should display patient details page', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.get('[role="table"] tr').first().click();
      cy.contains('Patient Details').should('be.visible');
    });

    it('should display patient allergies', () => {
      cy.visit(`${apiBaseUrl}/patients/pat-1`);
      cy.contains('Allergies').should('be.visible');
      cy.contains('Penicillin').should('be.visible');
    });

    it('should display chronic conditions', () => {
      cy.visit(`${apiBaseUrl}/patients/pat-1`);
      cy.contains('Chronic Conditions').should('be.visible');
      cy.contains('Type 2 Diabetes').should('be.visible');
    });

    it('should display patient timeline', () => {
      cy.visit(`${apiBaseUrl}/patients/pat-1/timeline`);
      cy.contains('Medical History').should('be.visible');
    });
  });

  describe('Patient CRUD Operations', () => {
    it('should create new patient', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.contains('button', 'Add Patient').click();

      // Fill form
      cy.get('input[name="firstName"]').type('Jane');
      cy.get('input[name="lastName"]').type('Smith');
      cy.get('input[name="mrn"]').type('MRN999999');
      cy.get('input[name="dateOfBirth"]').type('1985-05-15');
      cy.get('select[name="gender"]').select('female');

      cy.contains('button', 'Create').click();
      cy.contains('Patient created successfully').should('be.visible');
    });

    it('should update patient information', () => {
      cy.visit(`${apiBaseUrl}/patients/pat-1`);
      cy.contains('button', 'Edit').click();

      cy.get('input[name="phone"]').clear().type('555-9999');
      cy.contains('button', 'Save').click();

      cy.contains('Patient updated successfully').should('be.visible');
    });

    it('should show validation errors on invalid data', () => {
      cy.visit(`${apiBaseUrl}/patients`);
      cy.contains('button', 'Add Patient').click();

      cy.get('input[name="firstName"]').type('Jane');
      cy.contains('button', 'Create').click();

      cy.contains('Last name is required').should('be.visible');
    });
  });

  describe('Allergies Management', () => {
    it('should add allergy to patient', () => {
      cy.visit(`${apiBaseUrl}/patients/pat-1`);
      cy.contains('button', 'Add Allergy').click();

      cy.get('input[name="allergyName"]').type('Sulfa');
      cy.get('select[name="severity"]').select('moderate');
      cy.get('input[name="reaction"]').type('Rash');

      cy.contains('button', 'Add').click();
      cy.contains('Sulfa').should('be.visible');
    });

    it('should remove allergy from patient', () => {
      cy.visit(`${apiBaseUrl}/patients/pat-1`);
      cy.contains('Penicillin')
        .parent()
        .contains('button', 'Remove')
        .click();

      cy.contains('Penicillin').should('not.exist');
    });
  });
});
