/**
 * E2E Test Suite: Prescriptions Workflow
 * Tests prescription management and medication interactions
 */

describe('Prescriptions Workflow', () => {
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
  });

  describe('Prescription List', () => {
    it('should display prescriptions page', () => {
      cy.visit(`${apiBaseUrl}/prescriptions`);
      cy.contains('Prescriptions').should('be.visible');
    });

    it('should filter prescriptions by status', () => {
      cy.visit(`${apiBaseUrl}/prescriptions`);
      cy.get('select[name="status"]').select('active');
      cy.get('[role="table"]').should('contain', 'active');
    });
  });

  describe('Create Prescription', () => {
    it('should navigate to create prescription page', () => {
      cy.visit(`${apiBaseUrl}/prescriptions`);
      cy.contains('button', 'New Prescription').click();
      cy.url().should('include', '/prescriptions/new');
    });

    it('should create new prescription', () => {
      cy.visit(`${apiBaseUrl}/prescriptions/new`);

      cy.get('input[name="patientId"]').type('pat-1');
      cy.get('input[name="medicationName"]').type('Metformin');
      cy.get('input[name="dosage"]').type('500mg');
      cy.get('select[name="frequency"]').select('twice daily');
      cy.get('input[name="quantity"]').type('30');
      cy.get('input[name="refills"]').type('2');

      cy.contains('button', 'Create').click();
      cy.contains('Prescription created').should('be.visible');
    });

    it('should show validation errors', () => {
      cy.visit(`${apiBaseUrl}/prescriptions/new`);
      cy.contains('button', 'Create').click();
      cy.contains('required').should('be.visible');
    });
  });

  describe('Medication Interactions', () => {
    it('should check for interactions', () => {
      cy.visit(`${apiBaseUrl}/prescriptions/new`);

      // Add first medication
      cy.get('input[name="medicationName"]').type('Warfarin');
      cy.get('button').contains('Check Interactions').click();

      // Add second medication
      cy.get('input[name="medicationName"]').type('Aspirin');
      cy.get('button').contains('Check Interactions').click();

      // Should show interaction warning
      cy.contains('Severe interaction').should('be.visible');
      cy.contains('Increased bleeding risk').should('be.visible');
    });

    it('should warn about severe interactions', () => {
      cy.visit(`${apiBaseUrl}/prescriptions/new`);

      cy.get('input[name="medicationName"]').type('Warfarin');
      cy.get('button').contains('Check Interactions').click();

      cy.get('input[name="medicationName"]').type('Aspirin');
      cy.get('button').contains('Check Interactions').click();

      cy.get('[role="alert"]').should('have.class', 'alert-danger');
    });
  });

  describe('Prescription Management', () => {
    it('should refill prescription', () => {
      cy.visit(`${apiBaseUrl}/prescriptions`);
      cy.get('[role="table"] tr').first().within(() => {
        cy.contains('button', 'Refill').click();
      });

      cy.contains('Prescription refilled').should('be.visible');
    });

    it('should cancel prescription', () => {
      cy.visit(`${apiBaseUrl}/prescriptions`);
      cy.get('[role="table"] tr').first().within(() => {
        cy.contains('button', 'Cancel').click();
      });

      cy.get('textarea[name="reason"]').type('Patient discontinued');
      cy.contains('button', 'Confirm').click();

      cy.contains('Prescription cancelled').should('be.visible');
    });

    it('should send prescription to pharmacy', () => {
      cy.visit(`${apiBaseUrl}/prescriptions`);
      cy.get('[role="table"] tr').first().within(() => {
        cy.contains('button', 'Send').click();
      });

      cy.get('select[name="pharmacy"]').select('CVS Pharmacy');
      cy.contains('button', 'Send').click();

      cy.contains('Prescription sent').should('be.visible');
    });
  });
});
