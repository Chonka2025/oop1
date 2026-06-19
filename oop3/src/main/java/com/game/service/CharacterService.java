package com.game.service;

import com.game.dto.CharacterForm;
import com.game.entity.Character;
import com.game.entity.Guild;
import com.game.entity.Seeker;
import com.game.entity.Templar;
import com.game.repository.CharacterRepository;
import com.game.repository.GuildRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

// Основной сервис: CRUD всех персонажей, игровые действия (useSpecial, useAbility, tickOneSecond, takeDamage, heal)
// и конвертация между Character и CharacterForm. Работает через CharacterRepository.
@Service
@Transactional
public class CharacterService {

    private final CharacterRepository characterRepository;
    private final GuildRepository guildRepository;

    public CharacterService(CharacterRepository characterRepository, GuildRepository guildRepository) {
        this.characterRepository = characterRepository;
        this.guildRepository = guildRepository;
    }

    // Все персонажи, отсортированные по id
    public List<Character> findAll() {
        List<Character> characters = new ArrayList<>();
        characterRepository.findAll().forEach(characters::add);
        characters.sort(Comparator.comparing(Character::getId, Comparator.nullsLast(Long::compareTo)));
        return characters;
    }

    // Поиск по имени + сортировка по id (фильтрация через Stream)
    public List<Character> findAll(String search, String sortDir) {
        boolean asc = !"desc".equalsIgnoreCase(sortDir);
        List<Character> all = new ArrayList<>();
        characterRepository.findAll().forEach(all::add);

        return all.stream()
            .filter(c -> search == null || search.isBlank()
                || c.getName().toLowerCase().contains(search.toLowerCase()))
            .sorted(asc
                ? Comparator.comparing(Character::getId, Comparator.nullsLast(Long::compareTo))
                : Comparator.comparing(Character::getId, Comparator.nullsLast(Long::compareTo)).reversed())
            .toList();
    }

    // Поиск по id с исключением, если не найден
    public Character findById(Long id) {
        return characterRepository.findById(id)
                .orElseThrow(() -> new IllegalArgumentException("Character not found: " + id));
    }

    // Сохранение (создание или обновление) из данных формы
    public Character saveFromForm(CharacterForm form) {
        Character character = buildCharacter(form);
        return characterRepository.save(character);
    }

    public void deleteById(Long id) {
        characterRepository.deleteById(id);
    }

    // Выполнение игрового действия по строковому имени
    public Character runAction(Long id, String action) {
        Character character = findById(id);
        switch (action) {
            case "useSpecial" -> character.useSpecial();
            case "useAbility" -> runAbility(character);
            case "tickOneSecond" -> character.update(1.0f);
            case "takeDamage" -> runTakeDamage(character);
            case "heal" -> runHeal(character);
            default -> throw new IllegalArgumentException("Unsupported action: " + action);
        }
        return characterRepository.save(character);
    }

    // Нанести 20 ед. урона (через правильный метод подкласса)
    private void runTakeDamage(Character character) {
        int damage = 20;
        String className = character.getClass().getSimpleName();
        System.out.println("runTakeDamage called for: " + className + ", health before: " + character.getHealth());

        if ("Seeker".equals(className)) {
            ((Seeker) character).takeDamage(damage);
        } else if ("Templar".equals(className)) {
            ((Templar) character).takeDamage(damage);
        } else {
            character.takeDamage(damage);
        }
        System.out.println("health after: " + character.getHealth());
    }

    // Вылечить 15 ед. здоровья
    private void runHeal(Character character) {
        int amount = 15;
        String className = character.getClass().getSimpleName();
        System.out.println("runHeal called for: " + className + ", health before: " + character.getHealth());

        if ("Seeker".equals(className)) {
            ((Seeker) character).heal(amount);
        } else if ("Templar".equals(className)) {
            ((Templar) character).heal(amount);
        } else {
            character.heal(amount);
        }
        System.out.println("health after: " + character.getHealth());
    }
    // Конвертация Character -> CharacterForm (для заполнения формы редактирования)
    public CharacterForm toForm(Character character) {
        CharacterForm form = new CharacterForm();
        form.setId(character.getId());
        form.setType(character.getCharacterClass().toUpperCase());
        form.setName(character.getName());
        form.setLevel(character.getLevel());
        form.setHealth(character.getHealth());
        form.setMaxHealth(character.getMaxHealth());
        form.setBaseDamage(character.getBaseDamage());
        form.setRegenAmount(character.getRegenAmount());
        form.setRegenIntervalSec(character.getRegenIntervalSec());

        if (character instanceof Seeker seeker) {
            form.setStrength(seeker.getStrength());
            form.setAgility(seeker.getAgility());
            form.setEnergy(seeker.getEnergy());
            form.setDodgeChance(seeker.getDodgeChance());
            form.setIsStealthed(seeker.getIsStealthed());
            form.setStealthSeconds(seeker.getStealthSeconds());
        } else if (character instanceof Templar templar) {
            form.setStrength(templar.getStrength());
            form.setAgility(templar.getAgility());
            form.setFaith(templar.getFaith());
            form.setBlockChance(templar.getBlockChance());
            form.setArmor(templar.getArmor());
            form.setHolyPower(templar.getHolyPower());
            form.setIsImmobilized(templar.getIsImmobilized());
            form.setAegisTicksRemaining(templar.getAegisTicksRemaining());
            form.setAegisHealAmount(templar.getAegisHealAmount());
        }

        form.setGuildId(character.getGuild() != null ? character.getGuild().getId() : null);

        return form;
    }

    // Сборка/обновление сущности из данных формы
    private Character buildCharacter(CharacterForm form) {
        Character character;
        if (form.getId() != null) {
            // Редактирование существующего
            character = findById(form.getId());
            applySubtypeFields(character, form);
        } else {
            // Создание нового (выбор типа по form.type)
            character = switch (safeType(form.getType())) {
                case "SEEKER" -> mapSeeker(form);
                case "TEMPLAR" -> mapTemplar(form);
                default -> throw new IllegalArgumentException("Unsupported character type");
            };
        }

        character.setId(form.getId());
        character.setName(form.getName());
        character.setLevel(defaultInt(form.getLevel(), 1));
        character.setMaxHealth(defaultInt(form.getMaxHealth(), 100));
        character.setHealth(defaultInt(form.getHealth(), character.getMaxHealth()));
        character.setBaseDamage(defaultInt(form.getBaseDamage(), 10));
        character.setRegenAmount(defaultInt(form.getRegenAmount(), 1));
        character.setRegenIntervalSec(defaultFloat(form.getRegenIntervalSec(), 1.0f));

        if (form.getGuildId() != null) {
            Guild guild = guildRepository.findById(form.getGuildId())
                    .orElseThrow(() -> new IllegalArgumentException("Guild not found: " + form.getGuildId()));
            character.setGuild(guild);
        } else {
            character.setGuild(null);
        }

        return character;
    }

    // Маппинг полей формы в нового Seeker
    private Seeker mapSeeker(CharacterForm form) {
        Seeker seeker = new Seeker();
        applySubtypeFields(seeker, form);
        return seeker;
    }

    // Маппинг полей формы в нового Templar
    private Templar mapTemplar(CharacterForm form) {
        Templar templar = new Templar();
        applySubtypeFields(templar, form);
        return templar;
    }

    // Вызов абилки в зависимости от типа персонажа
    private void runAbility(Character character) {
        if (character instanceof Seeker seeker) {
            seeker.useAbility();
            return;
        }
        if (character instanceof Templar templar) {
            templar.useAbility();
        }
    }

    // Обновление специфичных полей подкласса при редактировании
    private void applySubtypeFields(Character character, CharacterForm form) {
        if (character instanceof Seeker seeker) {
            seeker.setStrength(defaultInt(form.getStrength(), 10));
            seeker.setAgility(defaultInt(form.getAgility(), 10));
            seeker.setEnergy(defaultInt(form.getEnergy(), 50));
            seeker.setDodgeChance(defaultInt(form.getDodgeChance(), 15));
            seeker.setIsStealthed(Boolean.TRUE.equals(form.getIsStealthed()));
            seeker.setStealthSeconds(defaultFloat(form.getStealthSeconds(), 0.0f));
        } else if (character instanceof Templar templar) {
            templar.setStrength(defaultInt(form.getStrength(), 12));
            templar.setAgility(defaultInt(form.getAgility(), 8));
            templar.setFaith(defaultInt(form.getFaith(), 60));
            templar.setBlockChance(defaultInt(form.getBlockChance(), 20));
            templar.setArmor(defaultInt(form.getArmor(), 15));
            templar.setHolyPower(defaultInt(form.getHolyPower(), 0));
            templar.setIsImmobilized(Boolean.TRUE.equals(form.getIsImmobilized()));
            templar.setAegisTicksRemaining(defaultInt(form.getAegisTicksRemaining(), 0));
            templar.setAegisHealAmount(defaultInt(form.getAegisHealAmount(), 30));
        }
    }

    // null-безопасное приведение типа к верхнему регистру
    private String safeType(String type) {
        return type == null ? "" : type.trim().toUpperCase();
    }

    // Замена null на значение по умолчанию (int)
    private int defaultInt(Integer value, int defaultValue) {
        return value == null ? defaultValue : value;
    }

    // Замена null на значение по умолчанию (float)
    private float defaultFloat(Float value, float defaultValue) {
        return value == null ? defaultValue : value;
    }
}
