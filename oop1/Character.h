#pragma once

#include <string>

class Character {
protected:
    std::string name;
    int level;
    int health;
    int maxHealth;
    int baseDamage;

    int regenAmount;
    float regenIntervalSec;
    float regenAccumulator;

public:
    Character(const std::string& name, int level);
    virtual ~Character() = default;

    virtual void attack(Character& target) = 0;
    virtual void useAbility() = 0;
    virtual void useSpecial();
    virtual void protectAlly(Character& ally, int incomingDamage);
    virtual void takeDamage(int damage);
    virtual void printInfo() const;

    void heal(int amount);
    void update(float deltaSeconds);

    std::string getName() const;
    int getHealth() const;
    int getLevel() const;
    int getMaxHealth() const;
    int getBaseDamage() const;
    void setHealth(int h);

protected:
    void setBaseDamage(int dmg);
    virtual void onUpdate(float deltaSeconds);
};

