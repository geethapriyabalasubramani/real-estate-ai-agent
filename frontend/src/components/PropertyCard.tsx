import type { PropertyListItem } from '../types/property';

interface PropertyCardProps {
  property: PropertyListItem;
}

export function PropertyCard({ property }: PropertyCardProps) {
  const price = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 0,
  }).format(property.price);

  return (
    <article
      style={{
        border: '1px solid #ddd',
        borderRadius: 8,
        padding: '1rem',
        marginBottom: '0.75rem',
      }}
    >
      <h3 style={{ margin: '0 0 0.5rem' }}>{property.addressLine1}</h3>
      <p style={{ margin: 0 }}>
        {property.city}, {property.state} {property.postalCode}
      </p>
      <p style={{ margin: '0.5rem 0 0' }}>
        {property.bedrooms} bd · {property.bathrooms} ba · {property.squareFeet}{' '}
        sqft · {property.hasGarage ? 'Garage' : 'No garage'}
      </p>
      <p style={{ margin: '0.5rem 0 0', fontWeight: 600 }}>{price}</p>
    </article>
  );
}
