using System.Collections.Generic;

namespace CharacterBattle
{
  /// <summary>
  /// Интерфейс репозитория персонажей.
  /// Определяет контракт для CRUD-операций с персонажами в хранилище данных.
  /// Реализует паттерн Repository для разделения бизнес-логики и доступа к данным.
  /// </summary>
  public interface ICharacterRepository
  {
    /// <summary>Получить список всех персонажей из хранилища.</summary>
    List<Character> GetAll();

    /// <summary>Получить персонажа по уникальному идентификатору.</summary>
    /// <param name="id">Идентификатор персонажа</param>
    /// <returns>Персонаж или null, если не найден</returns>
    Character? GetById(int id);

    /// <summary>Добавить нового персонажа в хранилище.</summary>
    /// <param name="character">Объект персонажа для сохранения</param>
    void Add(Character character);

    /// <summary>Обновить данные существующего персонажа.</summary>
    /// <param name="character">Персонаж с обновлёнными данными</param>
    void Update(Character character);

    /// <summary>Удалить персонажа из хранилища по идентификатору.</summary>
    /// <param name="id">Идентификатор персонажа для удаления</param>
    void Delete(int id);

    /// <summary>Получить всех персонажей типа Seeker (Искатель).</summary>
    /// <returns>Список Искателей</returns>
    List<Seeker> GetAllSeekers();

    /// <summary>Получить всех персонажей типа Templar (Храмовник).</summary>
    /// <returns>Список Храмовников</returns>
    List<Templar> GetAllTemplars();
  }
}
