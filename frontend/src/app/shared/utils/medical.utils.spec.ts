import {
  calculateBMI,
  getBMICategory,
  celsiusToFahrenheit,
  fahrenheitToCelsius,
  interpretBloodPressure,
  calculateEGFR,
  interpretEGFR,
  calculateHeartRateZone,
} from './medical.utils';

describe('Medical Utilities', () => {
  describe('calculateBMI', () => {
    it('should calculate BMI correctly', () => {
      const bmi = calculateBMI(70, 180);
      expect(bmi).toBeDefined();
      expect(typeof bmi).toBe('number');
    });

    it('should return valid BMI for standard values', () => {
      const bmi = calculateBMI(70, 170); // 24.2
      expect(bmi).toBeGreaterThan(0);
      expect(bmi).toBeLessThan(50);
    });
  });

  describe('getBMICategory', () => {
    it('should return Underweight for low BMI', () => {
      expect(getBMICategory(18)).toBe('Underweight');
    });

    it('should return Normal weight for healthy BMI', () => {
      expect(getBMICategory(22)).toBe('Normal weight');
    });

    it('should return Overweight for high BMI', () => {
      expect(getBMICategory(27)).toBe('Overweight');
    });

    it('should return Obese Class I for BMI >= 30', () => {
      expect(getBMICategory(32)).toBe('Obese Class I');
    });
  });

  describe('Temperature conversion', () => {
    it('should convert Celsius to Fahrenheit', () => {
      const fahrenheit = celsiusToFahrenheit(0);
      expect(fahrenheit).toBe(32);
    });

    it('should convert Fahrenheit to Celsius', () => {
      const celsius = fahrenheitToCelsius(32);
      expect(celsius).toBe(0);
    });

    it('should handle body temperature', () => {
      const celsius = 37;
      const fahrenheit = celsiusToFahrenheit(celsius);
      expect(fahrenheit).toBeCloseTo(98.6, 1);
    });
  });

  describe('interpretBloodPressure', () => {
    it('should interpret normal blood pressure', () => {
      const result = interpretBloodPressure(118, 75);
      expect(result).toBe('Normal');
    });

    it('should interpret elevated blood pressure', () => {
      const result = interpretBloodPressure(125, 75);
      expect(result).toBe('Elevated');
    });

    it('should interpret Stage 1 Hypertension', () => {
      const result = interpretBloodPressure(135, 85);
      expect(result).toBe('Stage 1 Hypertension');
    });

    it('should interpret Stage 2 Hypertension', () => {
      const result = interpretBloodPressure(145, 95);
      expect(result).toBe('Stage 2 Hypertension');
    });

    it('should interpret Hypertensive Crisis', () => {
      const result = interpretBloodPressure(185, 125);
      expect(result).toBe('Hypertensive Crisis');
    });
  });

  describe('calculateEGFR', () => {
    it('should calculate eGFR for normal kidney function', () => {
      const eGFR = calculateEGFR(0.8, 45, 'M', 'Other');
      expect(eGFR).toBeGreaterThan(60);
    });

    it('should calculate eGFR with different parameters', () => {
      const eGFR = calculateEGFR(1.0, 50, 'F', 'Other');
      expect(eGFR).toBeDefined();
      expect(typeof eGFR).toBe('number');
    });
  });

  describe('interpretEGFR', () => {
    it('should interpret normal eGFR', () => {
      expect(interpretEGFR(95)).toBe('Normal kidney function');
    });

    it('should interpret mild decrease', () => {
      expect(interpretEGFR(75)).toBe('Mild decrease in kidney function');
    });

    it('should interpret moderate decrease', () => {
      expect(interpretEGFR(45)).toBe('Moderate decrease in kidney function');
    });

    it('should interpret severe decrease', () => {
      expect(interpretEGFR(20)).toBe('Severe decrease in kidney function');
    });

    it('should interpret kidney failure', () => {
      expect(interpretEGFR(10)).toBe('Kidney failure');
    });
  });

  describe('calculateHeartRateZone', () => {
    it('should calculate light intensity zone', () => {
      const zone = calculateHeartRateZone(40, 'light');
      expect(zone.min).toBeLessThan(zone.max);
      expect(zone.min).toBeGreaterThan(0);
    });

    it('should calculate moderate intensity zone', () => {
      const zone = calculateHeartRateZone(40, 'moderate');
      expect(zone.min).toBeGreaterThan(90);
      expect(zone.max).toBeLessThan(175);
    });

    it('should calculate vigorous intensity zone', () => {
      const zone = calculateHeartRateZone(40, 'vigorous');
      expect(zone.min).toBeGreaterThan(100);
    });
  });
});
