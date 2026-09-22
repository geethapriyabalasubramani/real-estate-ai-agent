import { describe, expect, it } from 'vitest';
import {
  hasValidationErrors,
  validateSearchForm,
} from './searchValidation';

describe('validateSearchForm', () => {
  it('returns no errors for valid values', () => {
    const errors = validateSearchForm({
      city: 'San Jose',
      bedrooms: '2',
      maxPrice: '1200000',
      hasGarage: true,
    });

    expect(hasValidationErrors(errors)).toBe(false);
  });

  it('rejects bedrooms above 20', () => {
    const errors = validateSearchForm({
      city: 'San Jose',
      bedrooms: '99',
      maxPrice: '',
      hasGarage: false,
    });

    expect(errors.bedrooms).toMatch(/between 0 and 20/i);
  });

  it('rejects city longer than 100 characters', () => {
    const errors = validateSearchForm({
      city: 'x'.repeat(101),
      bedrooms: '',
      maxPrice: '',
      hasGarage: false,
    });

    expect(errors.city).toMatch(/100 characters/i);
  });

  it('rejects negative max price', () => {
    const errors = validateSearchForm({
      city: '',
      bedrooms: '',
      maxPrice: '-1',
      hasGarage: false,
    });

    expect(errors.maxPrice).toMatch(/>= 0/i);
  });
});