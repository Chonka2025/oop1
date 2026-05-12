namespace CharacterBattle
{
  public abstract class MeleeCharacter : Character
  {
    // Поля
    private int _strength;
    private int _agility;

    // Свойства
    public int Strength
    {
      get => _strength;
      set => _strength = value;
    }

    public int Agility
    {
      get => _agility;
      set => _agility = value;
    }

    // Конструктор
    protected MeleeCharacter(string name, int level, int strength, int agility)
        : base(name, level)
    {
      _strength = strength;
      _agility = agility;
      BaseDamage = 10 + level * 2;
    }

    // Методы
    public int CalculateDamage()
    {
      return BaseDamage + Strength / 5;
    }

    public string ShowStats()
    {
      return $"Сила: {Strength}, Ловкость: {Agility}";
    }
  }
}
