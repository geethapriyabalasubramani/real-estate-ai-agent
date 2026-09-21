import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { getPropertyById } from '../api/propertiesClient';
import type { PropertyDetail } from '../types/property';

export function PropertyDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [property, setProperty] = useState<PropertyDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setError('Missing property id.');
      setLoading(false);
      return;
    }

    const controller = new AbortController();

    (async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await getPropertyById(id, controller.signal);
        setProperty(data);
      } catch (err) {
        if (controller.signal.aborted) return;
        setProperty(null);
        setError(err instanceof Error ? err.message : 'Failed to load property');
      } finally {
        if (!controller.signal.aborted) setLoading(false);
      }
    })();

    return () => controller.abort();
  }, [id]);

  const priceFormatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0,
  });

  if (loading) return <p>Loading property…</p>;
  if (error) {
    return (
      <div>
        <p style={{ color: 'crimson' }} role="alert">{error}</p>
        <Link to="/">← Back to search</Link>
      </div>
    );
  }
  if (!property) return null;

  const pricePerSqFt = (property.price / property.squareFeet).toFixed(2);

  return (
    <article>
      <p>
        <Link to="/">← Back to search</Link>
      </p>
      <h2 style={{ marginBottom: '0.25rem' }}>{property.addressLine1}</h2>
      <p>
        {property.city}, {property.state} {property.postalCode}
      </p>
      <p style={{ fontWeight: 600, fontSize: '1.25rem' }}>
        {priceFormatter.format(property.price)}
      </p>
      <p>
        {property.bedrooms} bd · {property.bathrooms} ba · {property.squareFeet} sqft ·{' '}
        {property.hasGarage ? 'Garage' : 'No garage'} · ${pricePerSqFt}/sqft
      </p>
      <h3>Description</h3>
      <p style={{ lineHeight: 1.5 }}>{property.description}</p>
    </article>
  );
}