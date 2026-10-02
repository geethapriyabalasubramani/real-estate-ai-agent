import type { AiPropertySearchCriteria } from '../types/aiSearch';

export function formatAiCriteria(criteria: AiPropertySearchCriteria): string {
  const parts: string[] = [];

  if (criteria.city) parts.push(`City: ${criteria.city}`);
  if (criteria.bedrooms != null) parts.push(`Bedrooms: ${criteria.bedrooms}`);
  if (criteria.minPrice != null) {
    parts.push(`Min price: $${criteria.minPrice.toLocaleString()}`);
  }
  if (criteria.maxPrice != null) {
    parts.push(`Max price: $${criteria.maxPrice.toLocaleString()}`);
  }
  if (criteria.hasGarage === true) parts.push('Garage: yes');
  if (criteria.hasGarage === false) parts.push('Garage: no');

  return parts.length > 0 ? parts.join(' · ') : 'No specific filters extracted';
}
