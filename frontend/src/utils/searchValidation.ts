export interface SearchFormValues {
    city: string;
    bedrooms: string;
    maxPrice: string;
    hasGarage: boolean;
  }
  
  export type SearchFieldErrors = Partial<Record<keyof SearchFormValues, string>>;
  
  export function validateSearchForm(values: SearchFormValues): SearchFieldErrors {
    const errors: SearchFieldErrors = {};
  
    if (values.city.trim().length > 100) {
      errors.city = 'City must be at most 100 characters.';
    }
  
    if (values.bedrooms.trim() !== '') {
      const bedrooms = Number(values.bedrooms);
      if (Number.isNaN(bedrooms) || !Number.isInteger(bedrooms)) {
        errors.bedrooms = 'Bedrooms must be a whole number.';
      } else if (bedrooms < 0 || bedrooms > 20) {
        errors.bedrooms = 'Bedrooms must be between 0 and 20.';
      }
    }
  
    if (values.maxPrice.trim() !== '') {
      const maxPrice = Number(values.maxPrice);
      if (Number.isNaN(maxPrice) || maxPrice < 0) {
        errors.maxPrice = 'Max price must be a number >= 0.';
      }
    }
  
    return errors;
  }
  
  export function hasValidationErrors(errors: SearchFieldErrors): boolean {
    return Object.keys(errors).length > 0;
  }