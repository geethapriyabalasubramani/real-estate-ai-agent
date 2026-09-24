import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { LoginPanel } from './components/LoginPanel';
import { PropertySearch } from './components/PropertySearch';
import { PropertyDetailPage } from './pages/PropertyDetailPage';

function App() {
  return (
    <BrowserRouter>
      <main style={{ padding: '2rem', fontFamily: 'system-ui, sans-serif', maxWidth: 720 }}>
        <h1>Real Estate AI Agent</h1>
        <LoginPanel />
        <Routes>
          <Route
            path="/"
            element={
              <>
                <p>Search listings (Phase 2)</p>
                <PropertySearch />
              </>
            }
          />
          <Route path="/properties/:id" element={<PropertyDetailPage />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}

export default App;