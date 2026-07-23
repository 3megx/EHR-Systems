/**
 * Medical Utilities
 * Medical calculations and conversions
 */

/**
 * Calculate BMI (Body Mass Index)
 * @param weight in kg
 * @param height in cm
 */
export function calculateBMI(weight: number, height: number): number {
  const heightInMeters = height / 100;
  return Math.round((weight / (heightInMeters * heightInMeters)) * 10) / 10;
}

/**
 * Get BMI category
 */
export function getBMICategory(bmi: number): string {
  if (bmi < 18.5) return 'Underweight';
  if (bmi < 25) return 'Normal weight';
  if (bmi < 30) return 'Overweight';
  if (bmi < 35) return 'Obese Class I';
  if (bmi < 40) return 'Obese Class II';
  return 'Obese Class III';
}

/**
 * Calculate ideal body weight (Devine formula)
 * @param height in cm
 * @param gender 'M' or 'F'
 */
export function calculateIdealBodyWeight(height: number, gender: 'M' | 'F'): number {
  const heightInInches = height / 2.54;
  const basePounds = gender === 'M' ? 50 : 45.5;
  const additionalPerInch = gender === 'M' ? 2.3 : 2.3;
  const idealWeightInPounds = basePounds + (heightInInches - 60) * additionalPerInch;
  return Math.round((idealWeightInPounds / 2.205) * 10) / 10; // Convert to kg
}

/**
 * Convert temperature from Celsius to Fahrenheit
 */
export function celsiusToFahrenheit(celsius: number): number {
  return Math.round(((celsius * 9) / 5 + 32) * 10) / 10;
}

/**
 * Convert temperature from Fahrenheit to Celsius
 */
export function fahrenheitToCelsius(fahrenheit: number): number {
  return Math.round((((fahrenheit - 32) * 5) / 9) * 10) / 10;
}

/**
 * Interpret blood pressure reading
 */
export function interpretBloodPressure(systolic: number, diastolic: number): string {
  if (systolic < 120 && diastolic < 80) return 'Normal';
  if (systolic >= 120 && systolic <= 129 && diastolic < 80) return 'Elevated';
  if (systolic >= 130 && systolic <= 139 && diastolic >= 80 && diastolic <= 89) return 'Stage 1 Hypertension';
  if (systolic >= 140 || diastolic >= 90) return 'Stage 2 Hypertension';
  if (systolic > 180 || diastolic > 120) return 'Hypertensive Crisis';
  return 'Unknown';
}

/**
 * Calculate eGFR (Estimated Glomerular Filtration Rate)
 * Using MDRD equation
 */
export function calculateEGFR(creatinine: number, age: number, gender: 'M' | 'F', race: 'Black' | 'Other' = 'Other'): number {
  const raceCoeff = race === 'Black' ? 1.212 : 1;
  const genderCoeff = gender === 'F' ? 0.742 : 1;

  const eGFR = 175 * Math.pow(creatinine, -1.154) * Math.pow(age, -0.203) * genderCoeff * raceCoeff;

  return Math.round(eGFR * 10) / 10;
}

/**
 * Interpret eGFR value
 */
export function interpretEGFR(eGFR: number): string {
  if (eGFR >= 90) return 'Normal kidney function';
  if (eGFR >= 60) return 'Mild decrease in kidney function';
  if (eGFR >= 30) return 'Moderate decrease in kidney function';
  if (eGFR >= 15) return 'Severe decrease in kidney function';
  return 'Kidney failure';
}

/**
 * Calculate heart rate zone (max heart rate method)
 */
export function calculateHeartRateZone(age: number, intensity: 'light' | 'moderate' | 'vigorous'): { min: number; max: number } {
  const maxHeartRate = 220 - age;

  switch (intensity) {
    case 'light':
      return { min: Math.round(maxHeartRate * 0.5), max: Math.round(maxHeartRate * 0.7) };
    case 'moderate':
      return { min: Math.round(maxHeartRate * 0.7), max: Math.round(maxHeartRate * 0.85) };
    case 'vigorous':
      return { min: Math.round(maxHeartRate * 0.85), max: Math.round(maxHeartRate * 1.0) };
  }
}

/**
 * Calculate APGAR score interpretation
 */
export function getAPGARInterpretation(score: number): string {
  if (score >= 9) return 'Normal - Reassuring';
  if (score >= 7 && score <= 8) return 'Normal - Low risk';
  if (score >= 4 && score <= 6) return 'Moderately abnormal - Requires intervention';
  return 'Severely abnormal - Requires resuscitation';
}

/**
 * Get medication dose interval label
 */
export function getDoseIntervalLabel(frequency: number): string {
  const intervals: Record<number, string> = {
    1: 'Once daily',
    2: 'Twice daily',
    3: 'Three times daily',
    4: 'Four times daily',
  };
  return intervals[frequency] || `${frequency} times daily`;
}
