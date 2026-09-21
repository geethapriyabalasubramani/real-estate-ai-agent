import { type FormEvent, useState } from 'react';
import { searchProperties } from '../api/propertiesClient';
import type { PropertyListItem } from '../types/property';
import { PropertyCard } from './PropertyCard';

export function PropertySearch() {
  const [city, setCity] = useState('San Jose');
  const [bedrooms, setBedrooms] = useState('2');
  const [maxPrice, setMaxPrice] = useState('1200000');
  const [hasGarage, setHasGarage] = useState(true);

  const [items, setItems] = useState<PropertyListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runSearch(e?: FormEvent) {
    e?.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const result = await searchProperties({
        city,
        bedrooms: bedrooms ? Number(bedrooms) : undefined,
        maxPrice: maxPrice ? Number(maxPrice) : undefined,
        hasGarage: hasGarage ? true : undefined,
        page: 1,
        pageSize: 10,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      setItems([]);
      setTotalCount(0);
      setError(err instanceof Error ? err.message : 'Search failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <section>
      <form onSubmit={runSearch} style={{ display: 'grid', gap: '0.75rem', maxWidth: 480 }}>
        <label>
          City
          <input value={city} onChange={(e) => setCity(e.target.value)} style={{ width: '100%' }} />
        </label>
        <label>
          Bedrooms
          <input
            type="number"
            min={0}
            value={bedrooms}
            onChange={(e) => setBedrooms(e.target.value)}
            style={{ width: '100%' }}
          />
        </label>
        <label>
          Max price
          <input
            type="number"
            min={0}
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
            style={{ width: '100%' }}
          />
        </label>
        <label style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
          <input
            type="checkbox"
            checked={hasGarage}
            onChange={(e) => setHasGarage(e.target.checked)}
          />
          Has garage
        </label>
        <button type="submit" disabled={loading}>
          {loading ? 'Searching…' : 'Search'}
        </button>
      </form>

      {error && (
        <p style={{ color: 'crimson', marginTop: '1rem' }} role="alert">
          {error}
        </p>
      )}

      <p style={{ marginTop: '1rem' }}>
        {loading ? 'Loading…' : `${totalCount} propert${totalCount === 1 ? 'y' : 'ies'} found`}
      </p>

      <div style={{ marginTop: '0.5rem' }}>
        {items.map((p) => (
          <PropertyCard key={p.id} property={p} />
        ))}
      </div>
    </section>
  );
}
