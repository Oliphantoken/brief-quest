# Brief Quest

An AI-powered assignment brief analyser built for students who struggle with keeping track of their progress when completing requirements and fulfilling assessment criteria. Paste or upload an assignment brief and Brief Quest extracts every requirement into a checklist, organised by Pass, Merit and Distinction, with each item linked back to the exact sentence it came from.

**Live demo:** https://brief-quest-gamma.vercel.app/auth
**API docs:** Coming soon
**CI status:** ![CI](https://github.com/oliphantoken/brief-quest/actions/workflows/ci.yml/badge.svg)

---

## What it does

1. Register or sign in with your email and password.
2. Paste a brief or upload a PDF or Word file.
3. Click **Extract quests with AI**. The app sends the brief to the Gemini API and returns a checklist of requirements, each linked to the exact sentence in the brief.
4. Tick off quests as you work through them. A progress bar shows how much of each tier is done.
5. Look up any command word (such as "evaluate" or "analyse") to see its plain-language meaning and Bloom's taxonomy level.


## Privacy and AI

BriefQuest sends the text of assignment briefs to the Gemini API (Google). On the free tier, Google may use submitted prompts to improve its models. Do not submit sensitive or personal information through this app.
Responses are cached per user in the database to reduce API calls.
---

## Architecture

```
┌─────────────────────────────────────────┐
│  Browser (Vercel)                        │
│  Next.js 15 · React · TypeScript        │
│  Tailwind CSS                            │
└──────────────┬──────────────────────────┘
               │ HTTPS + JSON
               │ Bearer token (Authorization header)
┌──────────────▼──────────────────────────┐
│  API (Google Cloud Run)                  │
│  ASP.NET Core 10 · C# · Minimal APIs   │
│  ASP.NET Core Identity                  │
│  Entity Framework Core 10 · Npgsql      │
└──────┬───────────────────────┬──────────┘
       │ EF Core               │ HttpClient
┌──────▼──────────┐   ┌────────▼──────────┐
│  PostgreSQL      │   │  Gemini API        │
│  (Neon)          │   │  (Google AI        │
│                  │   │   Studio)          │
└─────────────────┘   └───────────────────┘
```

**Data flow for extraction:**
1. The front end posts the brief text or file to `POST /briefs/{id}/extract`.
2. The API checks the per-user daily AI cap (stored in the database).
3. The API checks its per-user cache. If a matching entry exists, it uses the cached reply.
4. If not cached, it sends the brief to Gemini and receives structured JSON.
5. Every quoted source sentence is verified against the original brief. Any quote that cannot be found is dropped.
6. Outcomes, criteria and quests are saved and returned.

---

## Tech stack

| Layer | Technology |
|---|---|
| Front end | Next.js 15, React, TypeScript, Tailwind CSS |
| API | ASP.NET Core 10, C#, Minimal APIs |
| Auth | ASP.NET Core Identity with bearer tokens |
| Database | PostgreSQL 16 via Entity Framework Core 10 and Npgsql |
| AI | Google Gemini API (Flash model, structured output) |
| Testing | xUnit, WebApplicationFactory, GitHub Actions |
| Containers | Docker, docker compose |
| Hosting | Google Cloud Run (API), Neon (database), Vercel (front end) |

---

## Running locally

### Prerequisites

- .NET 10 SDK
- Node 24 and npm
- Docker Desktop

### Steps

```bash
# 1. Clone the repo
git clone https://github.com/YOUR_USER/brief-quest
cd brief-quest

# 2. Start PostgreSQL
docker compose up -d

# 3. Apply migrations
dotnet ef database update --project api --startup-project api

# 4. Add secrets (run once per machine)
dotnet user-secrets set "ConnectionStrings:BriefQuest" \
  "Host=localhost;Port=5432;Database=briefquest;Username=briefquest;Password=dev-only-password" \
  --project api
dotnet user-secrets set "Gemini:ApiKey" "YOUR_KEY" --project api
dotnet user-secrets set "Gemini:Model" "gemini-3.5-flash" --project api

# 5. Start the API
dotnet run --project api

# 6. Start the web page (separate terminal)
cd web
npm install
echo "NEXT_PUBLIC_API_URL=http://localhost:5000" > .env.local
npm run dev
```

The API runs at `http://localhost:5000` and the web page at `http://localhost:3000`.

### Running tests

Docker must be running. Tests use a separate `briefquest_test` database.

```bash
dotnet test
```

---

## Key decisions

### Why minimal APIs over controllers

Minimal APIs keep the routing code close to the endpoint logic and require less ceremony. For a project this size, controllers would add boilerplate without benefit.

### Why built-in Identity bearer tokens over JWT

ASP.NET Core Identity's built-in token endpoints give a working login with no extra packages. While the tokens are not JWTs, which means they can't be verified independently of the server, they are adequate and working for the scope of this web application prototype.

### Why source sentence validation

Gemini can invent or paraphrase requirements. Every quoted source sentence is checked against the original brief character by character (with whitespace normalised). Any requirement whose sentence cannot be found is dropped silently and counted as rejected. This catches hallucinated content before it reaches the student.

### Why a per-user AI cache

The same brief text submitted twice by the same user should not cost two AI calls. Responses are cached by a SHA-256 hash of the brief text, keyed per user so one student's data is never served to another.

### Why archive instead of delete

Deleting a brief would immediately remove all its quests and the student's progress. Archiving stamps the brief with a date and hides it from normal lists, while keeping the data intact. A separate permanent-delete action removes everything through the cascade rules.

### Why PostgreSQL over SQLite

The project's SQL, relationships, migrations and cascade rules are the same regardless of the database engine. PostgreSQL is what most job listings expect, so it was chosen for its learning value. SQLite would have been simpler to run locally.

---

## Limitations

### AI quota

The Gemini free tier allows a limited number of requests per minute and per day. If the limit is reached, extraction returns a `429` error and the user is asked to try again later or tomorrow. The limit resets daily.

### Cold starts

The API runs on Google Cloud Run's free tier, which scales to zero after a period of inactivity. The first request after a quiet period may take 10 to 30 seconds to respond. This is noted in the API docs page. The Neon database also scales to zero after 5 minutes of inactivity, so the first query after an idle period has a short warm-up delay.

### Token expiry and logout

The bearer token stays valid until it expires (60 minutes) even after the user signs out or deletes their account. After deletion, the user's data is gone, so a valid token returns empty results until it expires. Fixing this fully requires either a token blocklist or a much shorter expiry.

### File upload

PDF extraction sends the file directly to Gemini as base64. Source sentence validation is skipped for PDFs because the brief text is not extracted separately. Gemini processes the file natively and is less likely to invent sentences, but this is a weaker guarantee than the text-extraction path.

### No real-time updates

Ticking a quest updates the UI immediately through optimistic state, but there is no websocket or push notification. If two browser tabs are open, the second tab won't reflect changes made in the first until it is refreshed.

### Privacy

Brief text is sent to the Google Gemini API. On the free tier, prompts may be used by Google to improve its models. Do not submit personal data or sensitive information through this app. Responses are cached per user to reduce the number of calls made.

### Storage limits

Neon's free tier gives 1 GB of PostgreSQL storage per project, with no expiry date. For a low-traffic demo this is sufficient.

---

## Project structure

```
brief-quest/
├── api/                        C# ASP.NET Core API
│   ├── Data/                   DbContext and migrations
│   ├── Endpoints/              One file per feature group
│   ├── Entities/               EF Core entity classes
│   ├── DTOs/                   Request and response records
│   ├── Services/               Gemini client, extraction, cache, cap
│   ├── Options/                Typed settings classes
│   ├── Exceptions/             Custom exceptions and global handler
│   └── Program.cs              App entry point
├── api.tests/                  xUnit tests
│   ├── ApiFactory.cs           WebApplicationFactory with test database
│   └── *Tests.cs               Integration and unit tests
├── web/                        Next.js front end
│   └── app/
│       ├── auth/               Sign in and register
│       ├── briefs/             List, detail and new brief pages
│       └── lib/                API client and token helpers
├── docs/
│   ├── schema.md               Agreed database schema and decisions
│   └── review-tracker.md       Quiz module tracker
├── docker-compose.yml          Local PostgreSQL
└── .github/workflows/          CI pipeline
    └── ci.yml
```

---

## Environment variables (production)

These are set on Cloud Run. Never commit them.

| Variable | Description |
|---|---|
| `ConnectionStrings__BriefQuest` | Neon PostgreSQL connection string |
| `Gemini__ApiKey` | Google AI Studio API key |
| `Gemini__Model` | Gemini model name, e.g. `gemini-3.5-flash` |
| `AllowedOrigins` | Comma-separated list of allowed CORS origins |

---

## Roadmap

- Google sign-in (OAuth)
- JWT authentication
- Command-word decoder endpoint on the front end
- First-step generation in the brief detail view
- Tutor-side brief clarity checker
- Bloom's taxonomy ladder visualisation
