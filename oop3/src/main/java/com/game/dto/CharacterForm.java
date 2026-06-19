package com.game.dto;

import lombok.Data;

@Data
public class CharacterForm {
    private Long id;
    private String type;
    private String name;
    private Integer level;
    private Integer health;
    private Integer maxHealth;
    private Integer baseDamage;
    private Integer regenAmount;
    private Float regenIntervalSec;
    private Integer strength;
    private Integer agility;
    private Integer energy;
    private Integer dodgeChance;
    private Boolean isStealthed;
    private Float stealthSeconds;
    private Integer faith;
    private Integer blockChance;
    private Integer armor;
    private Integer holyPower;
    private Boolean isImmobilized;
    private Integer aegisTicksRemaining;
    private Integer aegisHealAmount;
    private Long guildId;
}
