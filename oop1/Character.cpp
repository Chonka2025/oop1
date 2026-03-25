#include "Character.h"

#include <iostream>

using std::cout;
using std::endl;

Character::Character(const std::string& name, int level)
    : name(name),
      level(level),
      health(100 * level),
      maxHealth(100 * level),
      baseDamage(20 * (level * 2)),
      regenAmount(5),
      regenIntervalSec(5.0f),
      regenAccumulator(0.0f) {}

void Character::useSpecial() {
    cout << name << " не имеет специального умения." << endl;
}

void Character::protectAlly(Character& ally, int incomingDamage) {
    cout << name << " не может защитить союзника. Урон получает "
         << ally.getName() << "." << endl;
    ally.takeDamage(incomingDamage);
}

void Character::takeDamage(int damage) {
    health -= damage;
    if (health < 0) health = 0;
    cout << name << " получает " << damage << " урона. Осталось: "
         << health << "/" << maxHealth << endl;
}

void Character::heal(int amount) {
    health += amount;
    if (health > maxHealth) health = maxHealth;
    cout << name << " восстанавливает " << amount << " здоровья." << endl;
}

void Character::update(float deltaSeconds) {
    if (health <= 0) return;
    onUpdate(deltaSeconds);
    regenAccumulator += deltaSeconds;
    while (regenAccumulator >= regenIntervalSec) {
        regenAccumulator -= regenIntervalSec;
        heal(regenAmount);
    }
}

void Character::printInfo() const {
    cout << "Персонаж: " << name << ", уровень: " << level
         << ", здоровье: " << health << "/" << maxHealth
         << ", базовый урон: " << baseDamage << endl;
}

std::string Character::getName() const { return name; }
int Character::getHealth() const { return health; }
int Character::getLevel() const { return level; }
int Character::getMaxHealth() const { return maxHealth; }
int Character::getBaseDamage() const { return baseDamage; }

void Character::setHealth(int h) {
    if (h >= 0 && h <= maxHealth) {
        health = h;
    }
}

void Character::setBaseDamage(int dmg) {
    if (dmg > 0) {
        baseDamage = dmg;
    }
}

void Character::onUpdate(float) {}

