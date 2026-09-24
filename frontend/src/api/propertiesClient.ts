import type { PagedPropertyResponse, PropertyDetail } from '../types/property';

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

  const url = `${baseUrl}/api/v1/properties?${query.toString()}`;
  const response = await fetch(url, { signal });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<PagedPropertyResponse>;
}


export async function getPropertyById(id: string, signal?: AbortSignal): Promise<PropertyDetail> {
  const url = `${baseUrl}/api/v1/properties/${id}`;
  const response = await fetch(url, { signal });

  if (response.status === 404) {
    throw new Error('Property not found.');
  }

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Request failed (${response.status})`);
  }

  return response.json() as Promise<PropertyDetail>;
}
async function readErrorMessage(response: Response): Promise<string> {
    const text = await response.text();
    if (!text) return `Request failed (${response.status})`;
  
    try {
      const body = JSON.parse(text) as {
        title?: string;
        errors?: Record<string, string[]>;
      };
  
      if (body.errors) {
        const messages = Object.values(body.errors).flat();
        if (messages.length > 0) return messages.join(' ');
      }
  
      if (body.title) return body.title;
    } catch {
      // not JSON
    }
  
    return text;
  }