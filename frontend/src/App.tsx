import { PropertySearch } from './components/PropertySearch';

function App() {
  return (
    <main style={{ padding: '2rem', fontFamily: 'system-ui, sans-serif', maxWidth: 720 }}>
      <h1>Real Estate AI Agent</h1>
      <p>Search listings (Phase 2)</p>
      <PropertySearch />
    </main>
  );
}

export default App;