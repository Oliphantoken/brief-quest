# Brief Quest: review tracker

Keep this file in the repo (for example `docs/review-tracker.md`). Fill in the score columns after each quiz.

## How it works

- Every quiz is a **module** with a name, topic tags and a question count.
- Each plan day adds a small module (about 5 questions). Each Friday adds a cumulative module mixing older topics.
- Questions you miss come back after about **1, 3 and 7 days** (spaced repetition).
- To extend it: add a row to the table, list the topic tags, and ask Claude for a module on those tags.

## Topic tags

| Tag | Topic |
|---|---|
| T1 | .NET setup, project structure, dotnet CLI |
| T2 | REST, HTTP verbs and status codes |
| T3 | Records, DTOs, entities, validation |
| T4 | Builder versus app, services, middleware, endpoints |
| T5 | Dependency injection, configuration, options, secrets |
| T6 | Async and HttpClient, retries, Gemini client |
| T7 | Validating AI output, structured output |
| T8 | EF Core, DbContext, relationships, migrations, seeding |
| T9 | SQL: SELECT, JOIN, GROUP BY, schema design |
| T10 | Docker, Compose, Dockerfile |
| T11 | Testing and CI (xUnit, WebApplicationFactory, GitHub Actions) |

## Modules

| Module | Covers | Tags | Questions | Date taken | Score | Missed | Retake due |
|---|---|---|---|---|---|---|---|
| Session 2 check | REST and records | T2, T3 | 3 | | | | |
| Session 3 check | EF Core | T8 | 8 | | | | |
| Session 4 check | DI, config, async, EF queries | T5, T6, T8 | 28 | | | | |
| Whole-tutorial review | Sessions 1-4 | T1-T8 | 40 | | 37/40 | 3 | |
| Week 1 review | Gemini client, validation, EF, testing | T3-T8, T11 | 30 | | 27/30 | 6, 11, 28 | done |
| Retake (Week 1) | Keys, 502, retry testing | T6, T11 | 3 | | | | |
| **Week 2 review** | SQL, relationships, Docker, CI | T8-T11 | 30 | | | | |
| Module A (cumulative) | Tutorial Sessions 1-2 | T1-T4 | 24 | | | | |
| Module B1 (cumulative) | Tutorial Session 3 | T8 | 19 | | | | |
| Module B2 (cumulative) | Tutorial Session 4 | T5, T6, T8 | 36 | | | | |
| Module C (cumulative) | Week 1 project | T5-T7, T11 | 33 | | | | |

## Rules for new modules

- Name them `Day N check` (5 questions) or `Week N review` (30 questions).
- Include at least one code-review question and one question from an earlier day.
- Record misses by topic tag, not only by number.
