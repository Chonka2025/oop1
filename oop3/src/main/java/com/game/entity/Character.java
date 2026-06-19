package com.game.entity;

import jakarta.persistence.Column;
import jakarta.persistence.DiscriminatorColumn;
import jakarta.persistence.DiscriminatorType;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Inheritance;
import jakarta.persistence.InheritanceType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

import lombok.Getter;
import lombok.Setter;

@Entity
@Table(name = "characters")
@Inheritance(strategy = InheritanceType.JOINED)
@DiscriminatorColumn(name = "character_type", discriminatorType = DiscriminatorType.STRING)
@Getter
@Setter
public abstract class Character {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String name;

    @Column(nullable = false)
    private Integer level;

    @Column(nullable = false)
    private Integer health;

    @Column(name = "max_health", nullable = false)
    private Integer maxHealth;

    @Column(name = "base_damage", nullable = false)
    private Integer baseDamage;

    @Column(name = "regen_amount", nullable = false)
    private Integer regenAmount;

    @Column(name = "regen_interval_sec", nullable = false)
    private Float regenIntervalSec;

    @ManyToOne
    @JoinColumn(name = "guild_id")
    private Guild guild;

    protected Character() {
    }

    protected Character(Long id, String name, Integer level, Integer health, Integer maxHealth,
      Integer baseDamage, Integer regenAmount, Float regenIntervalSec) {
        this.id = id;
        this.name = name;
        this.level = level;
        this.health = health;
        this.maxHealth = maxHealth;
        this.baseDamage = baseDamage;
        this.regenAmount = regenAmount;
        this.regenIntervalSec = regenIntervalSec;
    }

    public void setHealth(Integer health) {
        this.health = Math.max(0, Math.min(health, maxHealth == null ? health : maxHealth));
    }

    public void setBaseDamage(Integer baseDamage) {
        if (baseDamage != null && baseDamage > 0) {
            this.baseDamage = baseDamage;
        }
    }

    public void useSpecial() {
        System.out.println(name + " не имеет специального умения.");
    }

    public void protectAlly(Character ally, int incomingDamage) {
        System.out.println(name + " не может защитить союзника. Урон получает " + ally.getName() + ".");
        ally.takeDamage(incomingDamage);
    }

    public void takeDamage(int damage) {
        health = Math.max(0, health - damage);
    }

    public void heal(int amount) {
        health = Math.min(maxHealth, health + amount);
    }

    protected void onUpdate(float deltaSeconds) {
    }

    public void update(float deltaSeconds) {
        if (health <= 0) return;
        onUpdate(deltaSeconds);
    }

    public String getCharacterClass() {
        return getClass().getSimpleName();
    }

    @Override
    public String toString() {
        return "%s{id=%d, name='%s', level=%d}".formatted(getCharacterClass(), id, name, level);
    }
}
