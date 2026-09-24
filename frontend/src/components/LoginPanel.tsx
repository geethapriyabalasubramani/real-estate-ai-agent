import { FormEvent, useEffect, useState } from 'react';
import { getMe, login, register } from '../api/authClient';
import type { MeResponse } from '../types/auth';
import { clearAccessToken, setAccessToken } from '../utils/authStorage';

export function LoginPanel() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [me, setMe] = useState<MeResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [checkingSession, setCheckingSession] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    (async () => {
      try {
        const profile = await getMe(controller.signal);
        setMe(profile);
      } catch {
        clearAccessToken();
        setMe(null);
      } finally {
        setCheckingSession(false);
      }
    })();

    return () => controller.abort();
  }, []);

  async function handleLogin(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const auth = await login({ email: email.trim(), password });
      setAccessToken(auth.accessToken);
      const profile = await getMe();
      setMe(profile);
    } catch (err) {
      clearAccessToken();
      setMe(null);
      setError(err instanceof Error ? err.message : 'Login failed');
    } finally {
      setLoading(false);
    }
  }

  async function handleRegister() {
    setLoading(true);
    setError(null);
    try {
      const auth = await register({ email: email.trim(), password });
      setAccessToken(auth.accessToken);
      const profile = await getMe();
      setMe(profile);
    } catch (err) {
      clearAccessToken();
      setMe(null);
      setError(err instanceof Error ? err.message : 'Register failed');
    } finally {
      setLoading(false);
    }
  }

  function handleLogout() {
    clearAccessToken();
    setMe(null);
    setError(null);
  }

  if (checkingSession) {
    return <p>Checking session…</p>;
  }

  if (me) {
    return (
      <section
        style={{
          marginBottom: '1.5rem',
          padding: '1rem',
          border: '1px solid #ddd',
          borderRadius: 8,
        }}
      >
        <p style={{ margin: 0 }}>
          Logged in as <strong>{me.email}</strong> (id: {me.userId})
        </p>
        <button type="button" onClick={handleLogout} style={{ marginTop: '0.5rem' }}>
          Logout
        </button>
      </section>
    );
  }

  return (
    <section
      style={{
        marginBottom: '1.5rem',
        padding: '1rem',
        border: '1px solid #ddd',
        borderRadius: 8,
      }}
    >
      <h2 style={{ marginTop: 0, fontSize: '1.1rem' }}>Account</h2>
      <form onSubmit={handleLogin} style={{ display: 'grid', gap: '0.75rem', maxWidth: 360 }}>
        <label>
          Email
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="email"
            style={{ width: '100%' }}
          />
        </label>
        <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={8}
            autoComplete="current-password"
            style={{ width: '100%' }}
          />
        </label>
        <div style={{ display: 'flex', gap: '0.5rem' }}>
          <button type="submit" disabled={loading}>
            {loading ? '…' : 'Log in'}
          </button>
          <button type="button" disabled={loading} onClick={handleRegister}>
            Register
          </button>
        </div>
      </form>
      {error && (
        <p style={{ color: 'crimson', marginTop: '0.75rem' }} role="alert">
          {error}
        </p>
      )}
    </section>
  );
}