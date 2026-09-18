\# Architecture (draft)



\## Overview



Single monorepo with a React SPA talking to an ASP.NET Core REST API,

backed by PostgreSQL (with pgvector for semantic search).



\## Planned data flow



1\. User asks a natural-language question in the React UI.

2\. Backend receives the request (later: via an AI agent layer).

3\. AI converts intent to structured search criteria.

4\. API queries PostgreSQL (structured + vector search).

5\. Results return to the UI with explanations.



\## Services (future phases)



\- \*\*frontend\*\* — Static SPA (Vite build)

\- \*\*backend\*\* — REST API + AI orchestration

\- \*\*postgres\*\* — Primary data store + embeddings

\- \*\*redis\*\* — Caching (Phase 8)

\- \*\*kafka\*\* — Events (Phase 8)



This document will evolve as we build.

