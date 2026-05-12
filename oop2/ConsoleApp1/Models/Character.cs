using System;

namespace CharacterBattle
{
  public abstract class Character
  {
    // Поля (private)
    private int _id;
    private string _name;
    private int _level;
    private int _health;
    private int _maxHealth;
    private int _baseDamage;
    private int _regenAmount;
    private float _regenIntervalSec;
    private float _regenAccumulator;

    // Свойства (геттеры/сеттеры)
    public int Id
    {
      get => _id;
      set => _id = value;
    }

    public string Name
    {
      get => _name;
      set => _name = value;
    }

    public int Level
    {
      get => _level;
      set
      {
        _level = value;
        _maxHealth = 100 * value;
        _baseDamage = 40 * value;
        if (_health > _maxHealth) _health = _maxHealth;
      }
    }

    public int Health
    {
      get => _health;
      set => _health = Math.Clamp(value, 0, _maxHealth);
    }

    public int MaxHealth
    {
      get => _maxHealth;
      set => _maxHealth = value;
    }

    public int BaseDamage
    {
      get => _baseDamage;
      protected internal set
      {
        if (value > 0) _baseDamage = value;
      }
    }

    public int RegenAmount
    {
      get => _regenAmount;
      set => _regenAmount = value;
    }

    public float RegenIntervalSec
    {
      get => _regenIntervalSec;
      set => _regenIntervalSec = value;
    }

    public float RegenAccumulator
    {
      get => _regenAccumulator;
      set => _regenAccumulator = value;
    }

    // Конструктор
    protected Character(string name, int level)
    {
      _name = name;
      Level = level;
      _health = _maxHealth;
      _regenAmount = 5;
      _regenIntervalSec = 5.0f;
      _regenAccumulator = 0.0f;
    }

    // Абстрактные методы
    public abstract void Attack(Character target);
    public abstract void UseAbility();

    // Виртуальные методы
    public virtual void UseSpecial()
    {
      // По умолчанию ничего не делает
    }

    public virtual void ProtectAlly(Character ally, int incomingDamage)
    {
      ally.TakeDamage(incomingDamage);
    }

    public virtual void TakeDamage(int damage)
    {
      Health = Math.Max(0, Health - Math.Max(0, damage));
    }

    public virtual string PrintInfo()
    {
      return $"{Name} (ур. {Level}) - HP: {Health}/{MaxHealth}, Урон: {BaseDamage}";
    }

    // Обычные методы
    public void Heal(int amount)
    {
      if (amount <= 0) return;
      Health += amount;
    }

    public void Update(float deltaSeconds)
    {
      if (Health <= 0) return;
      OnUpdate(deltaSeconds);
      RegenAccumulator += deltaSeconds;
      while (RegenAccumulator >= RegenIntervalSec)
      {
        RegenAccumulator -= RegenIntervalSec;
        Heal(RegenAmount);
      }
    }

    protected virtual void OnUpdate(float deltaSeconds) { }

    public abstract string GetCharacterType();
  }
}
