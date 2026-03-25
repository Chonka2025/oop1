#pragma once

#include "MeleeCharacter.h"

class Seeker : public MeleeCharacter {
private:
    int energy;
    int dodgeChance;
    bool isStealthed;
    float stealthSeconds;

public:
    Seeker(const std::string& name, int level, int startEnergy);
    Seeker(const std::string& name, int level);
    explicit Seeker(const std::string& name);

    void attack(Character& target) override;
    void useAbility() override;
    void takeDamage(int damage) override;
    void useSpecial() override;
    void printInfo() const override;

    void enterStealth();
    bool tryDodge() const;
    void showSeekerStats() const;

    int getEnergy() const;
    int getDodgeChance() const;
    bool getStealthState() const;
    void setEnergy(int e);

protected:
    void onUpdate(float deltaSeconds) override;
};