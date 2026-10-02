import { type FormEvent, useState } from 'react';
import { searchWithNaturalLanguage } from '../api/aiSearchClient';
import type { PropertyListItem } from '../types/property';
import { formatAiCriteria } from '../utils/formatAiCriteria';
import { PropertyCard } from './PropertyCard';

export function AiPropertySearch() {
  const [query, setQuery] = useState(
    'Find me 2-bedroom properties in San Jose under 1.2 million with a garage.'
  );
  const [criteriaText, setCriteriaText] = useState<string | null>(null);
  const [items, setItems] = useState<PropertyListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await searchWithNaturalLanguage({ query });
      setCriteriaText(formatAiCriteria(response.interpretedCriteria));
      setItems(response.results.items);
      setTotalCount(response.results.totalCount);
    } catch (err) {
      setCriteriaText(null);
      setItems([]);
      setTotalCount(0);
      setError(err instanceof Error ? err.message : 'AI search failed');
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
      <h2 style={{ marginTop: 0, fontSize: '1.15rem' }}>AI search</h2>
      <p style={{ marginTop: 0, color: '#444' }}>
        Describe what you want in plain English. The API extracts filters and runs the same search as
        the form below.
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '0.75rem' }}>
        <label>
          Your question
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
          {loading ? 'Searching with AI…' : 'Search with AI'}
        </button>
      </form>

      {error && (
        <p style={{ color: 'crimson', marginTop: '1rem' }} role="alert">
          {error}
        </p>
      )}

      {criteriaText && !error && (
        <p style={{ marginTop: '1rem' }}>
          <strong>Interpreted as:</strong> {criteriaText}
        </p>
      )}

      <p style={{ marginTop: '0.75rem' }}>
        {loading
          ? 'Loading…'
          : `${totalCount} propert${totalCount === 1 ? 'y' : 'ies'} found`}
      </p>

      <div style={{ marginTop: '0.5rem' }}>
        {items.map((p) => (
          <PropertyCard key={p.id} property={p} />
        ))}
      </div>
    </section>
  );
}