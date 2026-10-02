import type { PagedPropertyResponse, PropertyListItem } from './property';

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

export interface HybridPropertySearchRequest {
  query: string;
  limit?: number;
}

export interface HybridPropertySearchResultItem extends PropertyListItem {
  hybridScore: number;
  matchedSemantic: boolean;
  matchedFilter: boolean;
}

export interface HybridPropertySearchResponse {
  query: string;
  interpretedCriteria: AiPropertySearchCriteria | null;
  filterSearchNote: string | null;
  results: HybridPropertySearchResultItem[];
}

export interface PropertyAskRequest {
  question: string;
  topK?: number;
}

export interface PropertyAskSourceItem {
  id: string;
  addressLine1: string;
  city: string;
  state: string;
  similarityScore: number;
}

export interface PropertyAskResponse {
  question: string;
  answer: string;
  sources: PropertyAskSourceItem[];
}

export interface AgentChatRequest {
  message: string;
}

export interface AgentToolCallTrace {
  toolName: string;
  argumentsJson: string;
  resultSummary: string;
}

export interface AgentChatResponse {
  reply: string;
  toolCalls: AgentToolCallTrace[];
}