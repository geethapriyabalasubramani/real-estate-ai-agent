import type {
    NaturalLanguageSearchRequest,
    NaturalLanguageSearchResponse,
  } from '../types/aiSearch';
  
  const baseUrl = import.meta.env.VITE_API_BASE_URL;
  
  if (!baseUrl) {
    throw new Error('VITE_API_BASE_URL is not set');
  }
  
  async function readErrorMessage(response: Response): Promise<string> {
    const text = await response.text();
    if (!text) return `Request failed (${response.status})`;
  
    try {
      const body = JSON.parse(text) as {
        error?: string;
        title?: string;
        errors?: Record<string, string[]>;
      };
  
      if (body.errors) {
        const messages = Object.values(body.errors).flat();
        if (messages.length > 0) return messages.join(' ');
      }
  
      return body.error ?? body.title ?? text;
    } catch {
      return text;
    }
  }
  
  export async function searchWithNaturalLanguage(
    request: NaturalLanguageSearchRequest,
    signal?: AbortSignal
  ): Promise<NaturalLanguageSearchResponse> {
    const response = await fetch(`${baseUrl}/api/v1/ai/search`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ query: request.query.trim() }),
      signal,
    });
  
    if (!response.ok) {
      throw new Error(await readErrorMessage(response));
    }
  
    return response.json() as Promise<NaturalLanguageSearchResponse>;
  }