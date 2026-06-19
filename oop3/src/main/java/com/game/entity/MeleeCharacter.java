package com.game.entity;

import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorValue;
import jakarta.persistence.Entity;
import jakarta.persistence.PrimaryKeyJoinColumn;
import jakarta.persistence.Table;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Entity
@Table(name = "melee_character")
@PrimaryKeyJoinColumn(name = "id", referencedColumnName = "id")
@DiscriminatorValue("Melee")
@Getter
@Setter
@NoArgsConstructor
public class MeleeCharacter extends Character {

    @Column(nullable = false)
    private Integer strength;

    @Column(nullable = false)
    private Integer agility;

    public MeleeCharacter(Long id, String name, Integer level, Integer health, Integer maxHealth,
      Integer baseDamage, Integer regenAmount, Float regenIntervalSec,
      Integer strength, Integer agility) {
        super(id, name, level, health, maxHealth, baseDamage, regenAmount, regenIntervalSec);
        this.strength = strength;
        this.agility = 15;
    }

    public void showStats() {
        System.out.println("Статистика " + getName());
        System.out.println("Уровень: " + getLevel());
        System.out.println("Здоровье: " + getHealth() + "/" + getMaxHealth());
        System.out.println("Сила: " + strength);
        System.out.println("Ловкость: " + agility);
        System.out.println("Базовый урон: " + getBaseDamage());
    }

    public int calculateDamage() {
        return getBaseDamage() + strength / 2;
    }
}
