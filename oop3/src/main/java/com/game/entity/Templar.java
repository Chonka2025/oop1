package com.game.entity;

import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.PrimaryKeyJoinColumn;
import jakarta.persistence.Table;
import jakarta.persistence.Transient;

import lombok.Getter;
import lombok.Setter;

@Entity
@Table(name = "templar")
@PrimaryKeyJoinColumn(name = "id", referencedColumnName = "id")
@DiscriminatorValue("Templar")
@Getter
@Setter
public class Templar extends MeleeCharacter {

    @Column(nullable = false)
    private Integer faith;

    @Column(name = "block_chance", nullable = false)
    private Integer blockChance;

    @Column(nullable = false)
    private Integer armor;

    @Column(name = "holy_power", nullable = false)
    private Integer holyPower;

    @Column(name = "is_immobilized", nullable = false)
    private Boolean isImmobilized;

    @Column(name = "aegis_ticks_remaining", nullable = false)
    private Integer aegisTicksRemaining;

    @Column(name = "aegis_heal_amount", nullable = false)
    private Integer aegisHealAmount;

    @Transient
    private Integer currentAegisHeal = 0;

    public Templar() {
        this.faith = 60;
        this.blockChance = 20;
        this.armor = 15;
        this.holyPower = 3;
        this.isImmobilized = false;
        this.aegisTicksRemaining = 0;
        this.aegisHealAmount = 30;
    }

    public Templar(Long id, String name, Integer level, Integer health, Integer maxHealth,
      Integer baseDamage, Integer regenAmount, Float regenIntervalSec,
      Integer strength, Integer agility, Integer faith, Integer blockChance, Integer armor,
      Integer holyPower, Boolean isImmobilized, Integer aegisTicksRemaining,
      Integer aegisHealAmount) {
        super(id, name, level, health, maxHealth, baseDamage, regenAmount, regenIntervalSec,
          strength, agility);
        this.faith = faith;
        this.blockChance = blockChance;
        this.armor = level * 4;
        this.holyPower = holyPower;
        this.isImmobilized = isImmobilized;
        this.aegisTicksRemaining = aegisTicksRemaining;
        this.aegisHealAmount = aegisHealAmount;
    }

    public void activateDivineAegis() {
        if (faith >= 10 && !isImmobilized) {
            isImmobilized = true;
            aegisTicksRemaining = 10;
            currentAegisHeal = 0;
            faith -= 10;
            System.out.println(getName() + " использует неуязвимый купол!");
            System.out.println(" Полная защита от всего урона");
            System.out.println(" Неподвижен");
            System.out.println(" Лечение купола: " + (aegisHealAmount / 2) + " HP за тик");
            System.out.println(" Итого лечение за время купола: " + aegisHealAmount + " HP");
        } else if (isImmobilized) {
            System.out.println(getName() + " уже защищен Неуязвимым куполом!");
        } else {
            System.out.println(getName() + ": Недостаточно энергии!");
            System.out.println("  Требуется: 50 энергии, имеется: " + faith);
        }
    }

    public void onUpdate(float deltaSeconds) {
        if (!isImmobilized || aegisTicksRemaining <= 0) return;
        int healPerTick = aegisHealAmount / 2;
        heal(healPerTick);
        currentAegisHeal += healPerTick;
        aegisTicksRemaining--;
        System.out.println("Купол лечит " + getName() + " на +" + healPerTick + " HP");
        if (aegisTicksRemaining <= 0) deactivateDivineAegis();
    }

    public void deactivateDivineAegis() {
        if (!isImmobilized) return;

        isImmobilized = false;
        if (currentAegisHeal < aegisHealAmount) {
            int remainingHeal = aegisHealAmount - currentAegisHeal;
            heal(remainingHeal);
            System.out.println("Финальное лечение купола: +" + remainingHeal + " HP");
        }

        System.out.println("Неуязвимый купол " + getName() + " рассеивается.");
        System.out.println("  Итого получено лечения: " + currentAegisHeal + "/" + aegisHealAmount + " HP");
        System.out.println("  Подвижность восстановлена!");
        aegisTicksRemaining = 0;
        currentAegisHeal = 0;
    }

    public void attack(Character target) {
        if (isImmobilized) {
            System.out.println(getName() + " не может атаковать под Неуязвимым куполом!");
            return;
        }

        int damage = calculateDamage();
        if (holyPower >= 3) {
            damage += holyPower * 5;
            holyPower = 0;
            System.out.println(getName() + " наносит удар со Святой силой! ");
        } else {
            System.out.println(getName() + " наносит мощный удар! ");
        }

        System.out.println("Урон: " + damage);
        target.takeDamage(damage);
        faith += 5;
        holyPower++;
    }

    public void useAbility() { activateDivineAegis(); }

    public void useSpecial() {
        armor += 10;
        blockChance += 10;
        System.out.println(getName() + " усиливает броню и блок!");
        System.out.println("  Броня: " + armor + ", шанс блока: " + blockChance + "%");
    }

    public void takeDamage(int damage) {
        if (isImmobilized) {
            System.out.println(getName() + " неуязвим в куполе!");
            System.out.println("  Урон " + damage + " полностью поглощен.");
            return;
        }
        damage = Math.max(0, damage - armor / 2);
        super.takeDamage(damage);
    }

    public void takeHitForAlly(Character ally, int incomingDamage) {
        System.out.println(getName() + " принимает удар за союзника " + ally.getName() + "!");
        takeDamage(incomingDamage);
    }

    public void protectAlly(Character ally, int incomingDamage) {
        takeHitForAlly(ally, incomingDamage);
    }

    public void printInfo() {
        System.out.println(getName() + ": броня " + armor + ", шанс блока " + blockChance + "%");
    }
}
