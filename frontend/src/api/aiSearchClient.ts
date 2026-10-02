import type {
  HybridPropertySearchRequest,
  HybridPropertySearchResponse,
  NaturalLanguageSearchRequest,
  NaturalLanguageSearchResponse,
  AgentChatRequest,
  AgentChatResponse,
  PropertyAskRequest,
  PropertyAskResponse,
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
        detail?: string;
        title?: string;
        errors?: Record<string, string[]>;
      };
  
      if (body.errors) {
        const messages = Object.values(body.errors).flat();
        if (messages.length > 0) return messages.join(' ');
      }
  
      if (body.detail) {
        return body.detail;
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

export async function searchWithHybrid(
  request: HybridPropertySearchRequest,
  signal?: AbortSignal
): Promise<HybridPropertySearchResponse> {
  const response = await fetch(`${baseUrl}/api/v1/ai/hybrid-search`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      query: request.query.trim(),
      limit: request.limit ?? 10,
    }),
    signal,
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<HybridPropertySearchResponse>;
}

export async function askPropertyQuestion(
  request: PropertyAskRequest,
  signal?: AbortSignal
): Promise<PropertyAskResponse> {
  const response = await fetch(`${baseUrl}/api/v1/ai/ask`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      question: request.question.trim(),
      topK: request.topK ?? 5,
    }),
    signal,
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<PropertyAskResponse>;
}

export async function agentChat(
  request: AgentChatRequest,
  signal?: AbortSignal
): Promise<AgentChatResponse> {
  const response = await fetch(`${baseUrl}/api/v1/ai/agent/chat`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ message: request.message.trim() }),
    signal,
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<AgentChatResponse>;
}