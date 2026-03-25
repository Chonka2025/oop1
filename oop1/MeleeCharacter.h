#pragma once

#include "Character.h"

class MeleeCharacter : public Character {
protected:
    int strength;
    int agility;

public:
    MeleeCharacter(const std::string& name, int level, int str, int agi);

    void showStats() const;
    int calculateDamage() const;
};

