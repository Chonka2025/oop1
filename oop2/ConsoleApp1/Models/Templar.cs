using System;

namespace CharacterBattle
{
  public class Templar : MeleeCharacter
  {
    // Поля
    private int _faith;
    private int _blockChance;
    private int _armor;
    private int _holyPower;
    private bool _isImmobilized;
    private int _aegisTicksRemaining;
    private int _aegisHealAmount;
    private int _currentAegisHeal;

    // Свойства
    public int Faith
    {
      get => _faith;
      set => _faith = value;
    }

    public int BlockChance
    {
      get => _blockChance;
      set => _blockChance = value;
    }

    public int Armor
    {
      get => _armor;
      set => _armor = value;
    }

    public int HolyPower
    {
      get => _holyPower;
      set => _holyPower = value;
    }

    public bool IsImmobilized
    {
      get => _isImmobilized;
      set => _isImmobilized = value;
    }

    public int AegisTicksRemaining => _aegisTicksRemaining;

    public int AegisHealAmount => _aegisHealAmount;

    public void SetAegisState(int ticksRemaining, int healAmount)
    {
      _aegisTicksRemaining = ticksRemaining;
      _aegisHealAmount = healAmount;
    }

    // Конструкторы
    public Templar(string name, int level, int startFaith)
        : base(name, level, 50 + level * 4, 30 + level * 2)
    {
      Faith = startFaith;
      BlockChance = 25 + Strength / 4;
      Armor = 20 + level * 3;
      HolyPower = 0;
      IsImmobilized = false;
      _aegisTicksRemaining = 0;
      _aegisHealAmount = 50 + level * 10;
      _currentAegisHeal = 0;
    }

    public Templar(string name, int level) : this(name, level, 100) { }

    public Templar(string name) : this(name, 1, 80) { }

    // Переопределённые методы
    public override void Attack(Character target)
    {
      if (IsImmobilized)
      {
        return;
      }

      int damage = CalculateDamage();
      if (HolyPower >= 3)
      {
        int bonus = HolyPower * 3;
        damage += bonus;
        HolyPower = 0;
      }
      else
      {
        HolyPower++;
      }

      target.TakeDamage(damage);
      int faithBefore = Faith;
      Faith += 5;
    }

    public override void UseAbility()
    {
      if (_aegisTicksRemaining <= 0) ActivateDivineAegis();
      else DeactivateDivineAegis();
    }

    public override void UseSpecial()
    {
      Armor += 10;
      BlockChance += 10;
    }

    public override void TakeDamage(int damage)
    {
      if (IsImmobilized && _aegisTicksRemaining <= 1)
      {
        return;
      }

      int raw = damage;
      int mitigated = Armor / 2;
      int reduced = raw - mitigated;
      // Чтобы бой не застревал при огромной броне: прошедший удар наносит хотя бы 1 урона.
      damage = raw > 0 ? Math.Max(1, reduced) : 0;
      base.TakeDamage(damage);
    }

    public override void ProtectAlly(Character ally, int incomingDamage)
    {
      TakeHitForAlly(ally, incomingDamage);
    }

    public override string PrintInfo()
    {
      return $"[Храмовник] {base.PrintInfo()}, Броня: {Armor}, Блок: {BlockChance}%";
    }

    public override string GetCharacterType()
    {
      return "Templar";
    }

    // Специфичные методы
    public void ActivateDivineAegis()
    {
      if (Faith >= 50 && !IsImmobilized)
      {
        IsImmobilized = true;
        _aegisTicksRemaining = 1;
        _currentAegisHeal = 15;
        Faith -= 50;
      }
      if (_aegisTicksRemaining < 0) {
        DeactivateDivineAegis();
      }
    }

    public void DeactivateDivineAegis()
    {
      if (IsImmobilized)
      {
        IsImmobilized = false;
        if (_currentAegisHeal < _aegisHealAmount)
        {
          int remainingHeal = _aegisHealAmount - _currentAegisHeal;
          Heal(remainingHeal);
        }
        _aegisTicksRemaining = 0;
        _currentAegisHeal = 0;
      }
    }

    public void TakeHitForAlly(Character ally, int incomingDamage)
    {
      TakeDamage(incomingDamage);
    }

    public string GetTemplarStats()
    {
      return $"{PrintInfo()}\nСвятая сила: {HolyPower}";
    }

    /// <summary>Сила, ловкость, урон, броня и блок по формулам конструктора для текущего уровня.</summary>
    public void DefaultStats()
    {
      Strength = 50 + Level * 4;
      Agility = 30 + Level * 2;
      BaseDamage = 10 + Level * 2;
      BlockChance = 25 + Strength / 4;
      Armor = 20 + Level * 3;
    }

    protected override void OnUpdate(float deltaSeconds)
    {
      if (!IsImmobilized) return;
      if (_aegisTicksRemaining <= 0)
      {
        // Защита от "залипания" после загрузки из БД:
        // если обездвижен, но тиков эгиды уже нет, снимаем состояние.
        DeactivateDivineAegis();
        return;
      }

      int healPerTick = _aegisHealAmount / 2;
      Heal(healPerTick);
      _currentAegisHeal += healPerTick;
      _aegisTicksRemaining--;

      if (_aegisTicksRemaining <= 0)
      {
        DeactivateDivineAegis();
      }
    }
  }
}
