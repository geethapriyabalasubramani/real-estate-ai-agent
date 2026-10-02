import { type FormEvent, useState } from 'react';
import { agentChat } from '../api/aiSearchClient';
import type { AgentToolCallTrace } from '../types/aiSearch';

export function AiAgentChat() {
  const [message, setMessage] = useState(
    'Find me a 2-bedroom in San Jose under 900k that is walkable to transit and summarize the best option.'
  );
  const [reply, setReply] = useState<string | null>(null);
  const [toolCalls, setToolCalls] = useState<AgentToolCallTrace[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setReply(null);
    setToolCalls([]);

    try {
      const response = await agentChat({ message });
      setReply(response.reply);
      setToolCalls(response.toolCalls);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Agent chat failed');
    } finally {
      setLoading(false);
    }
  }

  return (
    <section
      style={{
        marginBottom: '2rem',
        padding: '1rem',
        border: '1px solid #2a5a8a',
        borderRadius: 8,
        background: '#f4f8fc',
      }}
    >
      <h2 style={{ marginTop: 0, fontSize: '1.15rem' }}>Agent chat (Phase 6)</h2>
      <p style={{ marginTop: 0, color: '#444' }}>
        The model chooses tools (hybrid search, filters, RAG, etc.) and returns a final answer. Tool
        steps are shown for transparency.
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '0.75rem' }}>
        <label>
          Message
          <textarea
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            rows={3}
            required
            minLength={1}
            maxLength={2000}
            style={{ width: '100%', marginTop: '0.25rem' }}
          />
        </label>
        <button type="submit" disabled={loading || message.trim().length < 1}>
          {loading ? 'Agent working…' : 'Send to agent'}
        </button>
      </form>

      {error && (
        <p style={{ color: 'crimson', marginTop: '1rem' }} role="alert">
          {error}
        </p>
      )}

      {reply && !error && (
        <div style={{ marginTop: '1.25rem' }}>
          <h3 style={{ margin: '0 0 0.5rem', fontSize: '1rem' }}>Reply</h3>
          <p style={{ margin: 0, lineHeight: 1.5, whiteSpace: 'pre-wrap' }}>{reply}</p>
        </div>
      )}

      {toolCalls.length > 0 && !error && (
        <div style={{ marginTop: '1.25rem' }}>
          <h3 style={{ margin: '0 0 0.5rem', fontSize: '1rem' }}>Tool trace</h3>
          <ol style={{ margin: 0, paddingLeft: '1.25rem' }}>
            {toolCalls.map((call, index) => (
              <li key={`${call.toolName}-${index}`} style={{ marginBottom: '0.5rem' }}>
                <details>
                  <summary style={{ cursor: 'pointer' }}>
                    <strong>{call.toolName}</strong>
                  </summary>
                  <pre
                    style={{
                      margin: '0.35rem 0',
                      fontSize: '0.8rem',
                      whiteSpace: 'pre-wrap',
                      background: '#fff',
                      padding: '0.5rem',
                      borderRadius: 4,
                      border: '1px solid #ddd',
                    }}
                  >
                    args: {call.argumentsJson}
                    {'\n'}
                    result: {call.resultSummary}
                  </pre>
                </details>
              </li>
            ))}
          </ol>
        </div>
      )}
    </section>
  );
}
