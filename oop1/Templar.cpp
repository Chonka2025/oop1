#include "Templar.h"

#include <algorithm>
#include <iostream>

using std::cout;
using std::endl;

Templar::Templar(const std::string& name, int level, int startFaith)
    : MeleeCharacter(name, level, 50 + level * 4, 30 + level * 2),
      faith(startFaith),
      blockChance(25 + strength / 4),
      armor(20 + level * 3),
      holyPower(0),
      isImmobilized(false),
      aegisTicksRemaining(0),
      aegisHealAmount(50 + level * 10),
      currentAegisHeal(0) {
    cout << "Создан Храмовник " << getName()
         << " (уровень " << getLevel() << ")" << endl;
    cout << "  Способности: Священный удар, Неуязвимый купол" << endl;
}

Templar::Templar(const std::string& name, int level)
    : Templar(name, level, 100) {}

Templar::Templar(const std::string& name)
    : Templar(name, 1, 80) {}

void Templar::activateDivineAegis() {
    if (faith >= 50 && !isImmobilized) {
        isImmobilized = true;
        aegisTicksRemaining = 1;
        currentAegisHeal = 0;
        faith -= 50;
        cout << getName() << " использует неуязвимый купол!" << endl;
        cout << " Полная защита от всего урона" << endl;
        cout << " Неподвижен" << endl;
        cout << " Лечение купола: " << (aegisHealAmount / 2) << " HP за тик" << endl;
        cout << " Итого лечение за время купола: " << aegisHealAmount << " HP" << endl;
    } else if (isImmobilized) {
        cout << getName() << " уже защищен Неуязвимым куполом!" << endl;
    } else {
        cout << getName() << ": Недостаточно энергии!" << endl;
        cout << "  Требуется: 50 энергии, имеется: " << faith << endl;
    }
}

void Templar::onUpdate(float) {
  if (!isImmobilized || aegisTicksRemaining <= 0) {
    return;
  }

  int healPerTick = aegisHealAmount / 2;
  heal(healPerTick);
  currentAegisHeal += healPerTick;
  --aegisTicksRemaining;
  cout << "Купол лечит " << getName() << " на +" << healPerTick << " HP"
       << endl;

  if (aegisTicksRemaining <= 0) {
    deactivateDivineAegis();
  }
}

void Templar::deactivateDivineAegis() {
    if (!isImmobilized) return;

    isImmobilized = false;
    if (currentAegisHeal < aegisHealAmount) {
        int remainingHeal = aegisHealAmount - currentAegisHeal;
        heal(remainingHeal);
        cout << "Финальное лечение купола: +" << remainingHeal << " HP" << endl;
    }

    cout << "Неуязвимый купол " << getName() << " рассеивается." << endl;
    cout << "  Итого получено лечения: " << currentAegisHeal
         << "/" << aegisHealAmount << " HP" << endl;
    cout << "  Подвижность восстановлена!" << endl;
    aegisTicksRemaining = 0;
    currentAegisHeal = 0;
}

void Templar::attack(Character& target) {
    if (isImmobilized) {
        cout << getName() << " не может атаковать под Неуязвимым куполом!" << endl;
        return;
    }

    int damage = calculateDamage();
    if (holyPower >= 3) {
        damage += holyPower * 5;
        holyPower = 0;
        cout << getName() << " наносит удар со Святой силой! ";
    } else {
        cout << getName() << " наносит мощный удар! ";
    }

    cout << "Урон: " << damage << endl;
    target.takeDamage(damage);
    faith += 5;
    holyPower++;
}

void Templar::useAbility() {
    activateDivineAegis();
}

void Templar::useSpecial() {
    armor += 10;
    blockChance += 10;
    cout << getName() << " усиливает броню и блок!" << endl;
    cout << "  Броня: " << armor << ", шанс блока: " << blockChance << "%" << endl;
}

void Templar::takeDamage(int damage) {
    if (isImmobilized) {
        cout << getName() << " неуязвим в куполе!" << endl;
        cout << "  Урон " << damage << " полностью поглощен." << endl;
        return;
    }

    damage = std::max(0, damage - armor / 2);
    Character::takeDamage(damage);
}

void Templar::takeHitForAlly(Character& ally, int incomingDamage) {
    cout << getName() << " принимает удар за союзника "
         << ally.getName() << "!" << endl;
    takeDamage(incomingDamage);
}

void Templar::protectAlly(Character& ally, int incomingDamage) {
    takeHitForAlly(ally, incomingDamage);
}

void Templar::printInfo() const {
    cout << name;
    Character::printInfo();
    cout << "  Броня: " << armor
         << ", шанс блока: " << blockChance << "%" << endl;
}
