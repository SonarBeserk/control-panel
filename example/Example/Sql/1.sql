CREATE TABLE IF NOT EXISTS naming
(
    name_id  TEXT PRIMARY KEY NOT NULL,
    name     TEXT             NOT NULL,
    nickname TEXT             NOT NULL
);

CREATE TABLE IF NOT EXISTS stats
(
    stat_id TEXT PRIMARY KEY NOT NULL,
    name    TEXT             NOT NULL,
    sent_at TEXT             NOT NULL
);
