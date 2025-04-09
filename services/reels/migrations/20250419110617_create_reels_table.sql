-- Add migration script here

CREATE TABLE reels(
  id uuid NOT NULL,
  PRIMARY KEY (id),
  path TEXT NOT NULL UNIQUE,
  created_at timestamptz NOT NULL
);
