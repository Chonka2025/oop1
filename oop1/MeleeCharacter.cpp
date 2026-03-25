#include "MeleeCharacter.h"

#include <iostream>

using std::cout;
using std::endl;

MeleeCharacter::MeleeCharacter(const std::string& name, int level, int str, int agi)
    : Character(name, level), strength(str), agility(agi) {
    setBaseDamage(10 + level * 2);
}

void MeleeCharacter::showStats() const {
    cout << "Статистика " << getName() << endl;
    cout << "Уровень: " << getLevel() << endl;
    cout << "Здоровье: " << getHealth() << "/" << getMaxHealth() << endl;
    cout << "Сила: " << strength << endl;
    cout << "Ловкость: " << agility << endl;
    cout << "Базовый урон: " << getBaseDamage() << endl;
}

int MeleeCharacter::calculateDamage() const {
    return getBaseDamage() + strength / 2;
}
