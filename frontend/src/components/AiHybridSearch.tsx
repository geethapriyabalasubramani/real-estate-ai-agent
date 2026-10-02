import { type FormEvent, useState } from 'react';
import { searchWithHybrid } from '../api/aiSearchClient';
import type { HybridPropertySearchResultItem } from '../types/aiSearch';
import type { PropertyListItem } from '../types/property';
import { formatAiCriteria } from '../utils/formatAiCriteria';
import { PropertyCard } from './PropertyCard';

function toListItem(item: HybridPropertySearchResultItem): PropertyListItem {
  return {
    id: item.id,
    addressLine1: item.addressLine1,
    city: item.city,
    state: item.state,
    postalCode: item.postalCode,
    bedrooms: item.bedrooms,
    bathrooms: item.bathrooms,
    price: item.price,
    squareFeet: item.squareFeet,
    hasGarage: item.hasGarage,
  };
}

function formatMatchTags(item: HybridPropertySearchResultItem): string {
  const tags: string[] = [];
  if (item.matchedSemantic) tags.push('semantic');
  if (item.matchedFilter) tags.push('filters');
  return tags.length > 0 ? tags.join(' + ') : 'ranked';
}

export function AiHybridSearch() {
  const [query, setQuery] = useState(
    'San Jose 2 bed walkable to transit under 900k'
  );
  const [criteriaText, setCriteriaText] = useState<string | null>(null);
  const [filterNote, setFilterNote] = useState<string | null>(null);
  const [items, setItems] = useState<HybridPropertySearchResultItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await searchWithHybrid({ query, limit: 10 });
      setCriteriaText(
        response.interpretedCriteria
          ? formatAiCriteria(response.interpretedCriteria)
          : null
      );
      setFilterNote(response.filterSearchNote);
      setItems(response.results);
    } catch (err) {
      setCriteriaText(null);
      setFilterNote(null);
      setItems([]);
      setError(err instanceof Error ? err.message : 'Hybrid search failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <section
      style={{
        marginBottom: '2rem',
        padding: '1rem',
        border: '1px solid #ccc',
        borderRadius: 8,
      }}
    >
      <h2 style={{ marginTop: 0, fontSize: '1.15rem' }}>Hybrid search</h2>
      <p style={{ marginTop: 0, color: '#444' }}>
        Combines AI filter extraction with semantic (vector) search, then merges results by
        relevance.
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '0.75rem' }}>
        <label>
          Search query
          <textarea
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            rows={3}
            required
            minLength={3}
            maxLength={500}
            style={{ width: '100%', marginTop: '0.25rem' }}
          />
        </label>
        <button type="submit" disabled={loading || query.trim().length < 3}>
          {loading ? 'Searching…' : 'Hybrid search'}
        </button>
      </form>

      {error && (
        <p style={{ color: 'crimson', marginTop: '1rem' }} role="alert">
          {error}
        </p>
      )}

      {criteriaText && !error && (
        <p style={{ marginTop: '1rem' }}>
          <strong>Filters interpreted as:</strong> {criteriaText}
        </p>
      )}

      {filterNote && !error && (
        <p style={{ marginTop: '0.5rem', color: '#555', fontSize: '0.9rem' }}>{filterNote}</p>
      )}

      <p style={{ marginTop: '0.75rem' }}>
        {loading
          ? 'Loading…'
          : `${items.length} result${items.length === 1 ? '' : 's'} (merged ranking)`}
      </p>

      <div style={{ marginTop: '0.5rem' }}>
        {items.map((item) => (
          <div key={item.id}>
            <PropertyCard property={toListItem(item)} />
            <p
              style={{
                margin: '-0.35rem 0 0.75rem 1rem',
                fontSize: '0.85rem',
                color: '#555',
              }}
            >
              Hybrid score: {item.hybridScore.toFixed(4)} · matched: {formatMatchTags(item)}
            </p>
          </div>
        ))}
      </div>
    </section>
  );
}
