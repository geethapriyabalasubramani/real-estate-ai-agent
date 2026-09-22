import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import { PropertySearch } from './PropertySearch';

describe('PropertySearch', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn());
  });

  it('shows validation error and does not call fetch for invalid bedrooms', async () => {
    const user = userEvent.setup();

    render(<PropertySearch />);

    const bedroomsInput = screen.getByLabelText(/bedrooms/i);
    await user.clear(bedroomsInput);
    await user.type(bedroomsInput, '99');

    await user.click(screen.getByRole('button', { name: /search/i }));

    expect(await screen.findByText(/between 0 and 20/i)).toBeInTheDocument();
    expect(fetch).not.toHaveBeenCalled();
  });
});