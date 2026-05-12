using System;
using System.Threading.Tasks;

namespace CharacterBattle
{
  public class Seeker : MeleeCharacter
  {
    // Поля
    private int _energy;
    private int _dodgeChance;
    private bool _isStealthed;
    private float _stealthSeconds;
    private ICharacterRepository? _repository;

    // Свойства
    public int Energy
    {
      get => _energy;
      set
      {
        if (value >= 0 && value <= 1000)
          _energy = value;
      }
    }

    public int DodgeChance
    {
      get => _dodgeChance;
      set => _dodgeChance = value;
    }

    public bool IsStealthed
    {
      get => _isStealthed;
      set => _isStealthed = value;
    }

    public float StealthSeconds
    {
      get => _stealthSeconds;
      set => _stealthSeconds = value;
    }

    // Конструкторы
    public Seeker(string name, int level, int startEnergy, ICharacterRepository? repository = null)
        : base(name, level, 30 + level * 3, 50 + level * 4)
    {
      _repository = repository;
      Energy = startEnergy;
      DodgeChance = 20 + Agility / 5;
      IsStealthed = false;
      StealthSeconds = 0.0f;
    }

    public Seeker(string name, int level, ICharacterRepository? repository = null) : this(name, level, 100, repository) { }

    public Seeker(string name, ICharacterRepository? repository = null) : this(name, 1, 80, repository) { }

    // Метод для установки репозитория после создания объекта
    public void SetRepository(ICharacterRepository? repository)
    {
      _repository = repository;
    }

    // Переопределённые методы
    public override void Attack(Character target)
    {
      int damage = CalculateDamage();
      string strike = IsStealthed
          ? $"скрытный удар (×2 от скрытности)"
          : "базовая атака ближнего боя";
      BattleNarrator.Line($"  • {Name} [{strike}]: расчёт урона {damage} (сила+база)");
      if (IsStealthed)
      {
        damage *= 2;
        IsStealthed = false;
        StealthSeconds = 0.0f;
        BattleNarrator.Line($"    после удвоения: {damage}");
      }
      target.TakeDamage(damage);
      int before = Energy;
      Energy += 5;
      BattleNarrator.Line($"    энергия: {before} → {Energy} (+5 за атаку)");
    }

    public override void UseAbility()
    {
      if (Energy >= 30)
      {
        DodgeChance += 15;
        Energy -= 30;
      }
    }

    public override void TakeDamage(int damage)
    {
      if (TryDodge(out int roll, out int needRoll))
      {
        BattleNarrator.Line(
            $"  ← {Name}: УКЛОНЕНИЕ");
        return;
      }

      BattleNarrator.Line($"  ← {Name}: входящий урон {damage}");
      base.TakeDamage(damage);
    }

    public override void UseSpecial()
    {
      EnterStealth();
      Timer();
    }

    private async void Timer()
    {
      await Task.Delay(5000);
      if (IsStealthed && _repository != null)
      {
        IsStealthed = false;
        StealthSeconds = 0.0f;
        _repository.Update(this);
      }
    }

    public override string PrintInfo()
    {
      return $"[Искатель] {base.PrintInfo()}, Энергия: {Energy}, Уворот: {DodgeChance}%";
    }

    public override string GetCharacterType()
    {
      return "Seeker";
    }

    // Специфичные методы
    public void EnterStealth()
    {
      if (Energy >= 5)
      {
        IsStealthed = true;
        StealthSeconds = 5.0f;
        Energy -= 1;
        DodgeChance += 5;

      }

    }

    /// <summary>Максимальный эффективный шанс уклонения).</summary>
    public const int MaxEffectiveDodgePercent = 100;

    public bool TryDodge(out int roll, out int effectiveThreshold)
    {
      roll = Random.Shared.Next(50);
      effectiveThreshold = Math.Min(DodgeChance, MaxEffectiveDodgePercent);
      return roll < effectiveThreshold;
    }

    public string GetSeekerStats()
    {
      return $"{PrintInfo()}\nСостояние: {(IsStealthed ? "В скрытности" : "Видим")}";
    }

    /// <summary>Сила, ловкость, базовый урон и уклонение по формулам конструктора для текущего уровня.</summary>
    public void DefaultStats()
    {
      Strength = 30 + Level * 3;
      Agility = 10 + Level * 4;
      BaseDamage = 30 + Level * 2;
      DodgeChance = 10 + Agility / 5;
    }

    protected override void OnUpdate(float deltaSeconds)
    {
      if (!IsStealthed) return;
      StealthSeconds -= deltaSeconds;
      if (StealthSeconds <= 0.0f)
      {
        IsStealthed = false;
        StealthSeconds = 0.0f;
      }
    }
  }
}
