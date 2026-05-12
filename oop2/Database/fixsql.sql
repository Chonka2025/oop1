-- Если таблицы уже созданы в схеме oop, но FK указывали на public.characters,
-- вставка в oop.templars даёт 23503. Выполните этот скрипт один раз.

-- ALTER TABLE oop.seekers
--   DROP CONSTRAINT IF EXISTS seekers_character_id_fkey;

-- ALTER TABLE oop.seekers
--   ADD CONSTRAINT seekers_character_id_fkey
--   FOREIGN KEY (character_id) REFERENCES oop.characters (id) ON DELETE CASCADE;

-- ALTER TABLE oop.templars
--   DROP CONSTRAINT IF EXISTS templars_character_id_fkey;

-- ALTER TABLE oop.templars
--   ADD CONSTRAINT templars_character_id_fkey
--   FOREIGN KEY (character_id) REFERENCES oop.characters (id) ON DELETE CASCADE;
