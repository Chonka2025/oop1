package com.game.entity;

import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.PrimaryKeyJoinColumn;
import jakarta.persistence.Table;

import lombok.Getter;
import lombok.Setter;

@Entity
@Table(name = "seeker")
@PrimaryKeyJoinColumn(name = "id", referencedColumnName = "id")
@DiscriminatorValue("Seeker")
@Getter
@Setter
public class Seeker extends MeleeCharacter {

    @Column(nullable = false)
    private Integer energy;

    @Column(name = "dodge_chance", nullable = false)
    private Integer dodgeChance;

    @Column(name = "is_stealthed", nullable = false)
    private Boolean isStealthed;

    @Column(name = "stealth_seconds", nullable = false)
    private Float stealthSeconds;

    public Seeker() {
        this.energy = 50;
        this.dodgeChance = 15;
        this.isStealthed = false;
        this.stealthSeconds = 3.0f;
    }

    public Seeker(Long id, String name, Integer level, Integer health,
      Integer maxHealth, Integer baseDamage, Integer regenAmount, Float regenIntervalSec,
      Integer strength, Integer agility, Integer energy, Integer dodgeChance,
      Boolean isStealthed, Float stealthSeconds) {
        super(id, name, level, health, maxHealth, baseDamage, regenAmount, regenIntervalSec,
          strength, agility);
        this.energy = energy;
        this.dodgeChance = dodgeChance;
        this.isStealthed = isStealthed;
        this.stealthSeconds = stealthSeconds;
    }

    public void attack(Character target) {
        int damage = calculateDamage();
        if (isStealthed) {
            damage *= 2;
            isStealthed = false;
            stealthSeconds = 0.0f;
            System.out.println(getName() + " атакует из скрытности! ");
        } else {
            System.out.println(getName() + " наносит быстрый двойной удар! ");
        }

        System.out.println("Урон: " + damage);
        target.takeDamage(damage);
        energy += 10;
    }

    public void useAbility() {
        if (energy >= 5) {
            System.out.println(getName() + " использует 'Теневой клинок'!");
            dodgeChance += 15;
            energy -= 5;
            System.out.println("  Шанс уворота увеличен до " + dodgeChance + "%");
        } else {
            System.out.println(getName() + ": Недостаточно энергии!");
        }
    }

    public void takeDamage(int damage) {
        if (tryDodge()) return;
        super.takeDamage(damage);
    }

    public void useSpecial() {
        enterStealth();
    }

    public void enterStealth() {
        if (energy >= 5) {
            isStealthed = true;
            stealthSeconds = 10.0f;
            energy -= 5;
            dodgeChance += 20;
            System.out.println(getName() + " скрывается в тенях...");
            System.out.println("  Шанс уворота: " + dodgeChance + "%");
        }
    }

    public boolean tryDodge() {
        int roll = (int) (Math.random() * 100);
        boolean dodged = roll < dodgeChance;
        if (dodged) {
            System.out.println(getName() + " уворачивается от атаки!");
        }
        return dodged;
    }

    public void onUpdate(float deltaSeconds) {
        if (!isStealthed) return;

        stealthSeconds -= deltaSeconds;
        if (stealthSeconds <= 0.0f) {
            isStealthed = false;
            stealthSeconds = 0.0f;
            System.out.println(getName() + " выходит из скрытности.");
        }
    }

    public void showSeekerStats() {
        showStats();
        System.out.println("Энергия: " + energy + "/100");
        System.out.println("Шанс уворота: " + dodgeChance + "%");
        System.out.println("Состояние: " + (isStealthed ? "В скрытности" : "Видим"));
    }

    public void printInfo() {
        System.out.println(getName() + ": энергия " + energy + ", шанс уворота " + dodgeChance + "%");
    }

    public void setEnergy(Integer energy) {
        if (energy == null) {
            this.energy = 50;
            return;
        }
        this.energy = Math.max(0, Math.min(energy, 1000));
    }
}
