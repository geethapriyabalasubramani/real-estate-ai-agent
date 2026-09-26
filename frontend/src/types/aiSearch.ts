import type { PagedPropertyResponse } from './property';

export interface AiPropertySearchCriteria {
  city?: string | null;
  bedrooms?: number | null;
  minPrice?: number | null;
  maxPrice?: number | null;
  hasGarage?: boolean | null;
}

export interface NaturalLanguageSearchRequest {
  query: string;
}

export interface NaturalLanguageSearchResponse {
  interpretedCriteria: AiPropertySearchCriteria;
  results: PagedPropertyResponse;
}