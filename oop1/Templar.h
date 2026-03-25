#pragma once

#include "MeleeCharacter.h"

class Templar : public MeleeCharacter {
private:
    int faith;
    int blockChance;
    int armor;
    int holyPower;
    bool isImmobilized;
    int aegisTicksRemaining;
    int aegisHealAmount;
    int currentAegisHeal;

public:
    Templar(const std::string& name, int level, int startFaith);
    Templar(const std::string& name, int level);
    explicit Templar(const std::string& name);

    void activateDivineAegis();
    void deactivateDivineAegis();
    void takeHitForAlly(Character& ally, int incomingDamage);
    int getfaith() const { return faith; }
    void attack(Character& target) override;
    void useAbility() override;
    void useSpecial() override;
    void takeDamage(int damage) override;
    void protectAlly(Character& ally, int incomingDamage) override;
    void printInfo() const override;

protected:
    void onUpdate(float deltaSeconds) override;
};

