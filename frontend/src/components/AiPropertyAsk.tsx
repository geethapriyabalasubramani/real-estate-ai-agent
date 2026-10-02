import { type FormEvent, useState } from 'react';
import { Link } from 'react-router-dom';
import { askPropertyQuestion } from '../api/aiSearchClient';
import type { PropertyAskSourceItem } from '../types/aiSearch';

export function AiPropertyAsk() {
  const [question, setQuestion] = useState(
    'Which homes are walkable to transit in downtown San Jose?'
  );
  const [answer, setAnswer] = useState<string | null>(null);
  const [sources, setSources] = useState<PropertyAskSourceItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setAnswer(null);
    setSources([]);

    try {
      const response = await askPropertyQuestion({ question, topK: 5 });
      setAnswer(response.answer);
      setSources(response.sources);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ask failed');
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
        background: '#fafafa',
      }}
    >
      <h2 style={{ marginTop: 0, fontSize: '1.15rem' }}>Ask the catalog (RAG)</h2>
      <p style={{ marginTop: 0, color: '#444' }}>
        Answers are generated from the most similar listings in the database (retrieval + LLM).
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '0.75rem' }}>
        <label>
          Your question
          <textarea
            value={question}
            onChange={(e) => setQuestion(e.target.value)}
            rows={3}
            required
            minLength={3}
            maxLength={500}
            style={{ width: '100%', marginTop: '0.25rem' }}
          />
        </label>
        <button type="submit" disabled={loading || question.trim().length < 3}>
          {loading ? 'Thinking…' : 'Ask'}
        </button>
      </form>

      {error && (
        <p style={{ color: 'crimson', marginTop: '1rem' }} role="alert">
          {error}
        </p>
      )}

      {answer && !error && (
        <div style={{ marginTop: '1.25rem' }}>
          <h3 style={{ margin: '0 0 0.5rem', fontSize: '1rem' }}>Answer</h3>
          <p style={{ margin: 0, lineHeight: 1.5, whiteSpace: 'pre-wrap' }}>{answer}</p>
        </div>
      )}

      {sources.length > 0 && !error && (
        <div style={{ marginTop: '1.25rem' }}>
          <h3 style={{ margin: '0 0 0.5rem', fontSize: '1rem' }}>Sources</h3>
          <p style={{ margin: '0 0 0.5rem', fontSize: '0.9rem', color: '#555' }}>
            Listings retrieved for context (similarity is relative, not a percentage):
          </p>
          <ul style={{ margin: 0, paddingLeft: '1.25rem' }}>
            {sources.map((source) => (
              <li key={source.id} style={{ marginBottom: '0.35rem' }}>
                <Link to={`/properties/${source.id}`}>
                  {source.addressLine1}, {source.city}, {source.state}
                </Link>
                <span style={{ color: '#666', fontSize: '0.85rem' }}>
                  {' '}
                  · score {source.similarityScore.toFixed(3)}
                </span>
              </li>
            ))}
          </ul>
        </div>
      )}
    </section>
  );
}
