using System.Collections.Generic;
// доступа к данным (CRUD и т д)
namespace CharacterBattle
{
  public interface ICharacterRepository
  {
    List<Character> GetAll();
    Character? GetById(int id);
    void Add(Character character);
    void Update(Character character);
    void Delete(int id);
    List<Seeker> GetAllSeekers();
    List<Templar> GetAllTemplars();
  }
}
