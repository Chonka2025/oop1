-- Схема oop: таблицы не в public, чтобы совпадало с pgAdmin (схема oop).
-- Выполните в нужной базе (например labb).

-- CREATE SCHEMA IF NOT EXISTS oop;

-- CREATE TABLE IF NOT EXISTS oop.characters (
--   id SERIAL PRIMARY KEY,
--   character_type VARCHAR(20) NOT NULL CHECK (character_type IN ('Seeker', 'Templar')),
--   name VARCHAR(200) NOT NULL,
--   level INT NOT NULL,
--   health INT NOT NULL,
--   max_health INT NOT NULL,
--   base_damage INT NOT NULL,
--   regen_amount INT NOT NULL,
--   regen_interval_sec DOUBLE PRECISION NOT NULL,
-- );

-- CREATE TABLE IF NOT EXISTS oop.melee_character(
--     agility int NOT NULL,
--     strength INT NOT NULL,
--     id INT PRIMARY KEY REFERENCES oop.characters (id) ON DELETE CASCADE,
-- )

-- CREATE TABLE IF NOT EXISTS oop.seeker (
--   id INT PRIMARY KEY REFERENCES oop.characters (id) ON DELETE CASCADE,
--   energy INT NOT NULL,
--   dodge_chance INT NOT NULL,
--   is_stealthed BOOLEAN NOT NULL,
--   stealth_seconds DOUBLE PRECISION NOT NULL
-- );

-- CREATE TABLE IF NOT EXISTS oop.templar (
--   id INT PRIMARY KEY REFERENCES oop.characters (id) ON DELETE CASCADE,
--   faith INT NOT NULL,
--   block_chance INT NOT NULL,
--   armor INT NOT NULL,
--   holy_power INT NOT NULL,
--   is_immobilized BOOLEAN NOT NULL,
--   aegis_ticks_remaining INT NOT NULL DEFAULT 0,
--   aegis_heal_amount INT NOT NULL DEFAULT 0
-- );

-- CREATE INDEX IF NOT EXISTS idx_oop_characters_type ON oop.characters (character_type);
