using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using NpgsqlTypes;

namespace CharacterBattle
{
  /// Репозиторий для работы с персонажами в PostgreSQL
  /// Реализует паттерн Repository: инкапсулирует доступ к данным
  public class CharacterRepository : ICharacterRepository
  {
    private readonly string _connectionString; // Строка подключения к БД (хост, порт, БД, логин, пароль)

    /// Конструктор репозитория
    /// connectionString - строка подключения к PostgreSQL
    public CharacterRepository(string connectionString)
    {
      _connectionString = connectionString;
    }

    /// Получить всех персонажей из БД
    /// Выполняет LEFT JOIN всех таблиц (characters, melee_character, seeker, templar)
    /// Возвращает список всех персонажей с их атрибутами
    public List<Character> GetAll()
    {
      var characters = new List<Character>();

      using var conn = new NpgsqlConnection(_connectionString);
      conn.Open();
      const string sql = @"
SELECT
  c.id, c.character_type, c.name, c.level, c.health, c.max_health, c.base_damage,
  c.regen_amount, c.regen_interval_sec,
  m.agility, m.strength,
  s.energy, s.dodge_chance, s.is_stealthed, s.stealth_seconds,
  t.faith, t.block_chance, t.armor, t.holy_power, t.is_immobilized,
  t.aegis_ticks_remaining, t.aegis_heal_amount
FROM oop.characters c
LEFT JOIN oop.melee_character m ON m.id = c.id
LEFT JOIN oop.seeker s ON s.id = c.id
LEFT JOIN oop.templar t ON t.id = c.id
ORDER BY c.id";

      using var cmd = new NpgsqlCommand(sql, conn);
      using var reader = cmd.ExecuteReader();
      while (reader.Read())
      {
        string type = reader.GetString(reader.GetOrdinal("character_type"));
        int id = reader.GetInt32(reader.GetOrdinal("id"));
        if (type == "Seeker")
          characters.Add(MapSeeker(reader, id));
        else if (type == "Templar")
          characters.Add(MapTemplar(reader, id));
      }

      return characters;
    }

    /// Получить персонажа по ID
    /// id - идентификатор персонажа в БД
    /// Возвращает персонажа или null если не найден
    public Character? GetById(int id)
    {
      using var conn = new NpgsqlConnection(_connectionString);
      conn.Open();
      const string sql = @"
SELECT
  c.id, c.character_type, c.name, c.level, c.health, c.max_health, c.base_damage,
  c.regen_amount, c.regen_interval_sec,
  m.agility, m.strength,
  s.energy, s.dodge_chance, s.is_stealthed, s.stealth_seconds,
  t.faith, t.block_chance, t.armor, t.holy_power, t.is_immobilized,
  t.aegis_ticks_remaining, t.aegis_heal_amount
FROM oop.characters c
LEFT JOIN oop.melee_character m ON m.id = c.id
LEFT JOIN oop.seeker s ON s.id = c.id
LEFT JOIN oop.templar t ON t.id = c.id
WHERE c.id = @id";

      using var cmd = new NpgsqlCommand(sql, conn);
      cmd.Parameters.AddWithValue("id", id);
      using var reader = cmd.ExecuteReader();
      if (!reader.Read())
        return null;

      string type = reader.GetString(reader.GetOrdinal("character_type"));
      if (type == "Seeker")
        return MapSeeker(reader, id);
      if (type == "Templar")
        return MapTemplar(reader, id);
      return null;
    }

    /// Добавить нового персонажа в БД
    /// character - объект персонажа для сохранения
    /// Использует транзакцию: сначала пишет в characters, затем в melee_character и seeker/templar
    public void Add(Character character)
    {
      using var conn = new NpgsqlConnection(_connectionString);
      conn.Open();
      using var transaction = conn.BeginTransaction();

      try
      {
        const string charQuery = @"
INSERT INTO oop.characters (character_type, name, level, health, max_health, base_damage,
  regen_amount, regen_interval_sec)
VALUES (@type, @name, @level, @health, @max_health, @base_damage,
  @regen_amount, @regen_interval_sec)
RETURNING id";

        using (var charCmd = new NpgsqlCommand(charQuery, conn, transaction))
        {
          charCmd.Parameters.AddWithValue("type", character.GetCharacterType());
          charCmd.Parameters.AddWithValue("name", character.Name);
          charCmd.Parameters.AddWithValue("level", character.Level);
          charCmd.Parameters.AddWithValue("health", character.Health);
          charCmd.Parameters.AddWithValue("max_health", character.MaxHealth);
          charCmd.Parameters.AddWithValue("base_damage", character.BaseDamage);
          charCmd.Parameters.AddWithValue("regen_amount", character.RegenAmount);
          charCmd.Parameters.Add("regen_interval_sec", NpgsqlDbType.Double).Value =
              (double)character.RegenIntervalSec;

          var scalar = charCmd.ExecuteScalar();
          int charId = Convert.ToInt32(scalar);
          character.Id = charId;

          if (character is MeleeCharacter melee)
          {
            const string meleeQuery = @"
INSERT INTO oop.melee_character (id, agility, strength)
VALUES (@id, @agility, @strength)";
            using var meleeCmd = new NpgsqlCommand(meleeQuery, conn, transaction);
            meleeCmd.Parameters.AddWithValue("id", charId);
            meleeCmd.Parameters.AddWithValue("agility", melee.Agility);
            meleeCmd.Parameters.AddWithValue("strength", melee.Strength);
            meleeCmd.ExecuteNonQuery();
          }

          if (character is Seeker seeker)
          {
            const string seekerQuery = @"
INSERT INTO oop.seeker (id, energy, dodge_chance, is_stealthed, stealth_seconds)
VALUES (@id, @energy, @dodge_chance, @is_stealthed, @stealth_seconds)";
            using var seekerCmd = new NpgsqlCommand(seekerQuery, conn, transaction);
            seekerCmd.Parameters.AddWithValue("id", charId);
            seekerCmd.Parameters.AddWithValue("energy", seeker.Energy);
            seekerCmd.Parameters.AddWithValue("dodge_chance", seeker.DodgeChance);
            seekerCmd.Parameters.AddWithValue("is_stealthed", seeker.IsStealthed);
            seekerCmd.Parameters.Add("stealth_seconds", NpgsqlDbType.Double).Value =
                (double)seeker.StealthSeconds;
            seekerCmd.ExecuteNonQuery();

            seeker.SetRepository(this);
          }
          else if (character is Templar templar)
          {
            const string templarQuery = @"
INSERT INTO oop.templar (id, faith, block_chance, armor, holy_power,
  is_immobilized, aegis_ticks_remaining, aegis_heal_amount)
VALUES (@id, @faith, @block_chance, @armor, @holy_power,
  @is_immobilized, @aegis_ticks_remaining, @aegis_heal_amount)";
            using var templarCmd = new NpgsqlCommand(templarQuery, conn, transaction);
            templarCmd.Parameters.AddWithValue("id", charId);
            templarCmd.Parameters.AddWithValue("faith", templar.Faith);
            templarCmd.Parameters.AddWithValue("block_chance", templar.BlockChance);
            templarCmd.Parameters.AddWithValue("armor", templar.Armor);
            templarCmd.Parameters.AddWithValue("holy_power", templar.HolyPower);
            templarCmd.Parameters.AddWithValue("is_immobilized", templar.IsImmobilized);
            templarCmd.Parameters.AddWithValue("aegis_ticks_remaining", templar.AegisTicksRemaining);
            templarCmd.Parameters.AddWithValue("aegis_heal_amount", templar.AegisHealAmount);
            templarCmd.ExecuteNonQuery();
          }
        }

        transaction.Commit();
      }
      catch
      {
        transaction.Rollback();
        throw;
      }
    }

    /// Обновить данные персонажа в БД
    /// character - персонаж с обновлёнными данными
    /// Использует INSERT ... ON CONFLICT (upsert) для обновления связанных таблиц
    public void Update(Character character)
    {
      using var conn = new NpgsqlConnection(_connectionString);
      conn.Open();
      using var transaction = conn.BeginTransaction();

      try
      {
        const string charQuery = @"
UPDATE oop.characters SET
  name = @name, level = @level, health = @health,
  max_health = @max_health, base_damage = @base_damage,
  regen_amount = @regen_amount, regen_interval_sec = @regen_interval_sec

WHERE id = @id";

        using (var charCmd = new NpgsqlCommand(charQuery, conn, transaction))
        {
          charCmd.Parameters.AddWithValue("id", character.Id);
          charCmd.Parameters.AddWithValue("name", character.Name);
          charCmd.Parameters.AddWithValue("level", character.Level);
          charCmd.Parameters.AddWithValue("health", character.Health);
          charCmd.Parameters.AddWithValue("max_health", character.MaxHealth);
          charCmd.Parameters.AddWithValue("base_damage", character.BaseDamage);
          charCmd.Parameters.AddWithValue("regen_amount", character.RegenAmount);
          charCmd.Parameters.Add("regen_interval_sec", NpgsqlDbType.Double).Value =
              (double)character.RegenIntervalSec;
          charCmd.ExecuteNonQuery();

          if (character is MeleeCharacter melee)
          {
            const string meleeQuery = @"
INSERT INTO oop.melee_character (id, agility, strength)
VALUES (@id, @agility, @strength)
ON CONFLICT (id) DO UPDATE SET
  agility = EXCLUDED.agility,
  strength = EXCLUDED.strength";
            using var meleeCmd = new NpgsqlCommand(meleeQuery, conn, transaction);
            meleeCmd.Parameters.AddWithValue("id", character.Id);
            meleeCmd.Parameters.AddWithValue("agility", melee.Agility);
            meleeCmd.Parameters.AddWithValue("strength", melee.Strength);
            meleeCmd.ExecuteNonQuery();
          }

          if (character is Seeker seeker)
          {
            const string seekerQuery = @"
INSERT INTO oop.seeker (id, energy, dodge_chance, is_stealthed, stealth_seconds)
VALUES (@id, @energy, @dodge_chance, @is_stealthed, @stealth_seconds)
ON CONFLICT (id) DO UPDATE SET
  energy = EXCLUDED.energy,
  dodge_chance = EXCLUDED.dodge_chance,
  is_stealthed = EXCLUDED.is_stealthed,
  stealth_seconds = EXCLUDED.stealth_seconds";
            using var seekerCmd = new NpgsqlCommand(seekerQuery, conn, transaction);
            seekerCmd.Parameters.AddWithValue("id", character.Id);
            seekerCmd.Parameters.AddWithValue("energy", seeker.Energy);
            seekerCmd.Parameters.AddWithValue("dodge_chance", seeker.DodgeChance);
            seekerCmd.Parameters.AddWithValue("is_stealthed", seeker.IsStealthed);
            seekerCmd.Parameters.Add("stealth_seconds", NpgsqlDbType.Double).Value =
                (double)seeker.StealthSeconds;
            seekerCmd.ExecuteNonQuery();
          }
          else if (character is Templar templar)
          {
            const string templarQuery = @"
INSERT INTO oop.templar (id, faith, block_chance, armor, holy_power, is_immobilized,
  aegis_ticks_remaining, aegis_heal_amount)
VALUES (@id, @faith, @block_chance, @armor, @holy_power, @is_immobilized,
  @aegis_ticks_remaining, @aegis_heal_amount)
ON CONFLICT (id) DO UPDATE SET
  faith = EXCLUDED.faith,
  block_chance = EXCLUDED.block_chance,
  armor = EXCLUDED.armor,
  holy_power = EXCLUDED.holy_power,
  is_immobilized = EXCLUDED.is_immobilized,
  aegis_ticks_remaining = EXCLUDED.aegis_ticks_remaining,
  aegis_heal_amount = EXCLUDED.aegis_heal_amount";
            using var templarCmd = new NpgsqlCommand(templarQuery, conn, transaction);
            templarCmd.Parameters.AddWithValue("id", character.Id);
            templarCmd.Parameters.AddWithValue("faith", templar.Faith);
            templarCmd.Parameters.AddWithValue("block_chance", templar.BlockChance);
            templarCmd.Parameters.AddWithValue("armor", templar.Armor);
            templarCmd.Parameters.AddWithValue("holy_power", templar.HolyPower);
            templarCmd.Parameters.AddWithValue("is_immobilized", templar.IsImmobilized);
            templarCmd.Parameters.AddWithValue("aegis_ticks_remaining", templar.AegisTicksRemaining);
            templarCmd.Parameters.AddWithValue("aegis_heal_amount", templar.AegisHealAmount);
            templarCmd.ExecuteNonQuery();
          }
        }

        transaction.Commit();
      }
      catch
      {
        transaction.Rollback();
        throw;
      }
    }

    /// Удалить персонажа из БД
    /// id - идентификатор персонажа для удаления
    /// Удаление каскадируется на связанные таблицы благодаря FK с ON DELETE CASCADE
    public void Delete(int id)
    {
      using var conn = new NpgsqlConnection(_connectionString);
      conn.Open();
      using var cmd = new NpgsqlCommand("DELETE FROM oop.characters WHERE id = @id", conn);
      cmd.Parameters.AddWithValue("id", id);
      cmd.ExecuteNonQuery();
    }

    /// Получить список всех Искателей (Seeker)
    /// Использует LINQ для фильтрации из GetAll()
    public List<Seeker> GetAllSeekers()
    {
      return GetAll().OfType<Seeker>().ToList();
    }

    /// Получить список всех Храмовников (Templar)
    /// Использует LINQ для фильтрации из GetAll()
    public List<Templar> GetAllTemplars()
    {
      return GetAll().OfType<Templar>().ToList();
    }

    /// Маппит строку результата в объект Seeker
    /// reader - NpgsqlDataReader с данными из БД
    /// id - ID персонажа
    /// Возвращает готовый объект Seeker
    private Seeker MapSeeker(NpgsqlDataReader reader, int id)
    {
      var seeker = new Seeker(
          reader.GetString(reader.GetOrdinal("name")),
          reader.GetInt32(reader.GetOrdinal("level")),
          reader.GetInt32(reader.GetOrdinal("health")));

      seeker.Id = id;
      seeker.Health = reader.GetInt32(reader.GetOrdinal("health"));
      seeker.MaxHealth = reader.GetInt32(reader.GetOrdinal("max_health"));
      seeker.BaseDamage = reader.GetInt32(reader.GetOrdinal("base_damage"));
      seeker.RegenAmount = reader.GetInt32(reader.GetOrdinal("regen_amount"));
      seeker.RegenIntervalSec = (float)reader.GetDouble(reader.GetOrdinal("regen_interval_sec"));

      seeker.Agility = reader.GetInt32(reader.GetOrdinal("agility"));
      seeker.Strength = reader.GetInt32(reader.GetOrdinal("strength"));

      seeker.Energy = reader.GetInt32(reader.GetOrdinal("energy"));
      seeker.DodgeChance = reader.GetInt32(reader.GetOrdinal("dodge_chance"));
      seeker.IsStealthed = reader.GetBoolean(reader.GetOrdinal("is_stealthed"));
      seeker.StealthSeconds = (float)reader.GetDouble(reader.GetOrdinal("stealth_seconds"));

      seeker.SetRepository(this);

      return seeker;
    }

    /// Маппит строку результата в объект Templar
    /// reader - NpgsqlDataReader с данными из БД
    /// id - ID персонажа
    /// Возвращает готовый объект Templar
    private Templar MapTemplar(NpgsqlDataReader reader, int id)
    {
      var templar = new Templar(
          reader.GetString(reader.GetOrdinal("name")),
          reader.GetInt32(reader.GetOrdinal("level")));

      templar.Id = id;
      templar.Health = reader.GetInt32(reader.GetOrdinal("health"));
      templar.MaxHealth = reader.GetInt32(reader.GetOrdinal("max_health"));
      templar.BaseDamage = reader.GetInt32(reader.GetOrdinal("base_damage"));
      templar.RegenAmount = reader.GetInt32(reader.GetOrdinal("regen_amount"));
      templar.RegenIntervalSec = (float)reader.GetDouble(reader.GetOrdinal("regen_interval_sec"));

      templar.Agility = reader.GetInt32(reader.GetOrdinal("agility"));
      templar.Strength = reader.GetInt32(reader.GetOrdinal("strength"));

      templar.Faith = reader.GetInt32(reader.GetOrdinal("faith"));
      templar.BlockChance = reader.GetInt32(reader.GetOrdinal("block_chance"));
      templar.Armor = reader.GetInt32(reader.GetOrdinal("armor"));
      templar.HolyPower = reader.GetInt32(reader.GetOrdinal("holy_power"));
      templar.IsImmobilized = reader.GetBoolean(reader.GetOrdinal("is_immobilized"));
      templar.SetAegisState(
          reader.GetInt32(reader.GetOrdinal("aegis_ticks_remaining")),
          reader.GetInt32(reader.GetOrdinal("aegis_heal_amount")));

      return templar;
    }
  }
}
