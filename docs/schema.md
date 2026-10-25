# Brief Quest: database schema (v1)

Status: agreed on Day 6. Changes are recorded at the bottom.

## Decisions

1. **Outcome codes are text.** Briefs use codes like "LO1" and plain numbers, so codes are stored as text.
2. **Tier is stored as a word** (Pass, Merit or Distinction), with a check that rejects anything else.
3. **Extracted requirements become quests automatically.**
4. **Deleting a brief archives it.** The brief gets an `archived_at` date and is hidden from normal lists. A separate action permanently deletes the brief and all its outcomes, criteria and quests.
5. **Extraction runs once per brief.** A second attempt is blocked (409 Conflict), with a confirm-to-replace option as a possible later feature. `extracted_at` records that extraction has happened.
6. **Deleting an account removes everything the user owns**, including archived briefs, cached AI answers and usage counters.
7. **The AI cache is per user**, so one student's cached text is never reused for another.

## Tables

| Table | Columns | Keys and rules |
|---|---|---|
| `briefs` | id, owner_id, title, text, created_at, archived_at (nullable), extracted_at (nullable) | PK id. owner_id references the user, cascade on delete |
| `outcomes` | id, brief_id, code, text | PK id. brief_id references briefs, cascade. Unique (brief_id, code) |
| `criteria` | id, outcome_id, tier, text, source_sentence | PK id. outcome_id references outcomes, cascade. Tier limited to the three words |
| `quests` | id, brief_id, criterion_id (nullable), text, source_sentence, tier (nullable), status, first_step (nullable), created_at | PK id. brief_id cascade. criterion_id set to empty if the criterion is deleted. Status limited to Todo, Doing, Done |
| `ai_cache` | owner_id, brief_hash, response_json, created_at | PK (owner_id, brief_hash). owner_id cascade |
| `usage_counters` | owner_id, day, count | PK (owner_id, day). owner_id cascade |
| `command_words` | word, plain_meaning, bloom_level, example | PK word |

The `users` table comes from ASP.NET Core Identity (Day 11). `owner_id` points at it.

## SQL (reference)

The real tables are created by EF Core migrations. This SQL is for reading and for practising queries.

```sql
CREATE TABLE briefs (
  id           integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  owner_id     text         NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  title        varchar(200) NOT NULL,
  text         text         NOT NULL,
  created_at   timestamptz  NOT NULL DEFAULT now(),
  archived_at  timestamptz,
  extracted_at timestamptz
);

CREATE TABLE outcomes (
  id       integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  brief_id integer NOT NULL REFERENCES briefs(id) ON DELETE CASCADE,
  code     text    NOT NULL,
  text     text    NOT NULL,
  UNIQUE (brief_id, code)
);

CREATE TABLE criteria (
  id              integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  outcome_id      integer NOT NULL REFERENCES outcomes(id) ON DELETE CASCADE,
  tier            text    NOT NULL CHECK (tier IN ('Pass','Merit','Distinction')),
  text            text    NOT NULL,
  source_sentence text    NOT NULL
);

CREATE TABLE quests (
  id              integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  brief_id        integer NOT NULL REFERENCES briefs(id) ON DELETE CASCADE,
  criterion_id    integer REFERENCES criteria(id) ON DELETE SET NULL,
  text            text    NOT NULL,
  source_sentence text    NOT NULL,
  tier            text    CHECK (tier IN ('Pass','Merit','Distinction')),
  status          text    NOT NULL DEFAULT 'Todo' CHECK (status IN ('Todo','Doing','Done')),
  first_step      text,
  created_at      timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE ai_cache (
  owner_id      text        NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  brief_hash    text        NOT NULL,
  response_json jsonb       NOT NULL,
  created_at    timestamptz NOT NULL DEFAULT now(),
  PRIMARY KEY (owner_id, brief_hash)
);

CREATE TABLE usage_counters (
  owner_id text    NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  day      date    NOT NULL,
  count    integer NOT NULL DEFAULT 0,
  PRIMARY KEY (owner_id, day)
);

CREATE TABLE command_words (
  word          text PRIMARY KEY,
  plain_meaning text NOT NULL,
  bloom_level   text NOT NULL,
  example       text
);

CREATE INDEX ix_outcomes_brief_id  ON outcomes(brief_id);
CREATE INDEX ix_criteria_outcome_id ON criteria(outcome_id);
CREATE INDEX ix_quests_brief_id    ON quests(brief_id);
```

## What this means for the API

- `DELETE /briefs/{id}` archives the brief (204).
- `GET /briefs` hides archived briefs by default.
- `POST /briefs/{id}/restore` clears `archived_at`.
- `DELETE /briefs/{id}/permanent` removes the brief and everything under it. It is only allowed for an already archived brief.
- `POST /briefs/{id}/extract` returns 409 if `extracted_at` is already set.
- Every endpoint checks that the brief belongs to the signed-in user.

## Open questions for later

- Should cached AI answers expire after some days? (Privacy: they contain text derived from student work.)
- Do we show a "replace quests" option after the first extraction?
- Integer IDs are guessable, so authorisation checks are essential (Day 12).

## Change log

- v1: first agreed version (Day 6).
