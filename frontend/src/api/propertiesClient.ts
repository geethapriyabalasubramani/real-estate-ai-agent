import type { PagedPropertyResponse } from '../types/property';

const baseUrl = import.meta.env.VITE_API_BASE_URL;

if (!baseUrl) {
  throw new Error('VITE_API_BASE_URL is not set');
}

export interface PropertySearchParams {
  city?: string;
  bedrooms?: number;
  maxPrice?: number;
  hasGarage?: boolean;
  page?: number;
  pageSize?: number;
}

export async function searchProperties(
  params: PropertySearchParams,
  signal?: AbortSignal
): Promise<PagedPropertyResponse> {
  const query = new URLSearchParams();

  if (params.city?.trim()) query.set('city', params.city.trim());
  if (params.bedrooms !== undefined) query.set('bedrooms', String(params.bedrooms));
  if (params.maxPrice !== undefined) query.set('maxPrice', String(params.maxPrice));
  if (params.hasGarage === true) query.set('hasGarage', 'true');
  if (params.page) query.set('page', String(params.page));
  if (params.pageSize) query.set('pageSize', String(params.pageSize));

  const url = `${baseUrl}/api/properties?${query.toString()}`;
  const response = await fetch(url, { signal });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Search failed (${response.status})`);
  }

  return response.json() as Promise<PagedPropertyResponse>;
}