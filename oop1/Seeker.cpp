#include "Seeker.h"

#include <cstdlib>
#include <iostream>

using std::cout;
using std::endl;

Seeker::Seeker(const std::string& name, int level, int startEnergy)
    : MeleeCharacter(name, level, 30 + level * 3, 50 + level * 4),
      energy(startEnergy),
      dodgeChance(20 + agility / 5),
      isStealthed(false),
      stealthSeconds(0.0f) {
    cout << "Создан Искатель " << getName() << " уровень: " << getLevel() << endl;
    cout << "  Способности: Теневой клинок, Скрытность" << endl;
}

Seeker::Seeker(const std::string& name, int level)
    : Seeker(name, level, 100) {}

Seeker::Seeker(const std::string& name)
    : Seeker(name, 1, 80) {}

void Seeker::attack(Character& target) {
    int damage = calculateDamage();
    if (isStealthed) {
        damage *= 2;
        isStealthed = false;
        stealthSeconds = 0.0f;
        cout << getName() << " атакует из скрытности! ";
    } else {
        cout << getName() << " наносит быстрый двойной удар! ";
    }

    cout << "Урон: " << damage << endl;
    target.takeDamage(damage);
    energy += 10;
}

void Seeker::useAbility() {
    if (energy >= 30) {
        cout << getName() << " использует 'Теневой клинок'!" << endl;
        dodgeChance += 15;
        energy -= 30;
        cout << "  Шанс уворота увеличен до " << dodgeChance << "%" << endl;
    } else {
        cout << getName() << ": Недостаточно энергии!" << endl;
    }
}

void Seeker::takeDamage(int damage) {
    if (tryDodge()) return;
    Character::takeDamage(damage);
}

void Seeker::useSpecial() {
    enterStealth();
}

void Seeker::enterStealth() {
    if (energy >= 20) {
        isStealthed = true;
        stealthSeconds = 10.0f;
        energy -= 20;
        dodgeChance += 25;
        cout << getName() << " скрывается в тенях..." << endl;
        cout << "  Шанс уворота: " << dodgeChance << "%" << endl;
    }
}

bool Seeker::tryDodge() const {
    int roll = rand() % 100;
    bool dodged = (roll < dodgeChance);
    if (dodged) {
        cout << getName() << " уворачивается от атаки!" << endl;
    }
    return dodged;
}

void Seeker::onUpdate(float deltaSeconds) {
    if (!isStealthed) return;

    stealthSeconds -= deltaSeconds;
    if (stealthSeconds <= 0.0f) {
        isStealthed = false;
        stealthSeconds = 0.0f;
        cout << getName() << " выходит из скрытности." << endl;
    }
}

void Seeker::showSeekerStats() const {
    showStats();
    cout << "Энергия: " << energy << "/100" << endl;
    cout << "Шанс уворота: " << dodgeChance << "%" << endl;
    cout << "Состояние: " << (isStealthed ? "В скрытности" : "Видим") << endl;
}

void Seeker::printInfo() const {
    cout << name << " ";
    Character::printInfo();
    cout << "  Энергия: " << energy
         << ", шанс уворота: " << dodgeChance << "%" << endl;
}

int Seeker::getEnergy() const { return energy; }
int Seeker::getDodgeChance() const { return dodgeChance; }
bool Seeker::getStealthState() const { return isStealthed; }

void Seeker::setEnergy(int e) {
    if (e >= 0 && e <= 100) {
        energy = e;
    }
}
