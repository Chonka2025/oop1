package com.game.service;

import com.game.entity.Character;
import com.game.entity.Guild;
import com.game.repository.CharacterRepository;
import com.game.repository.GuildRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

@Service
@Transactional
// Сервис для управления гильдиями: CRUD + добавление/исключение участников.
// Работает с GuildRepository и CharacterRepository
public class GuildService {

    private final GuildRepository guildRepository;
    private final CharacterRepository characterRepository;

    public GuildService(GuildRepository guildRepository, CharacterRepository characterRepository) {
        this.guildRepository = guildRepository;
        this.characterRepository = characterRepository;
    }

    // отсортированные по id
    public List<Guild> findAll() {
        List<Guild> guilds = new ArrayList<>();
        guildRepository.findAll().forEach(guilds::add);
        guilds.sort(Comparator.comparing(Guild::getId));
        return guilds;
    }

    // Поиск по id, иначе исключение
    public Guild findById(Long id) {
        return guildRepository.findById(id)
                .orElseThrow(() -> new IllegalArgumentException("Guild not found: " + id));
    }

    // Создать новую гильдию и сохранить в БД
    public Guild save(String name, String description) {
        Guild guild = new Guild(name, description);
        return guildRepository.save(guild);
    }

    // Обновить название и описание существующей гильдии
    public Guild update(Long id, String name, String description) {
        Guild guild = findById(id);
        guild.setName(name);
        guild.setDescription(description);
        return guildRepository.save(guild);
    }

    // Удалить гильдию. Перед удалением обнулить guild_id у всех участников.
    public void deleteById(Long id) {
        Guild guild = findById(id);
        guild.getMembers().forEach(m -> m.setGuild(null));
        guildRepository.deleteById(id);
    }

    // Добавить персонажа в гильдию (просто установить guild_id).
    // public void addMember(Long guildId, Long characterId) {
    //     Guild guild = findById(guildId);
    //     Character character = characterRepository.findById(characterId)
    //             .orElseThrow(() -> new IllegalArgumentException("Character not found: " + characterId));
    //     character.setGuild(guild);
    //     characterRepository.save(character);
    // }

    // Добавить персонажа в гильдию по имени.
    public String addMemberByName(Long guildId, String name) {
        Guild guild = findById(guildId);
        List<Character> matches = characterRepository
                .findByNameContainingIgnoreCaseAndGuildIsNull(name.trim());
        if (matches.isEmpty()) {
            return "Персонаж с именем \"" + name + "\" не найден или уже в гильдии.";
        }
        Character character = matches.get(0);
        character.setGuild(guild);
        characterRepository.save(character);
        return null; // null = успех
    }

    // Исключить персонажа из гильдии.
    public void removeMember(Long guildId, Long characterId) {
        Character character = characterRepository.findById(characterId)
                .orElseThrow(() -> new IllegalArgumentException("Character not found: " + characterId));
        character.setGuild(null);
        characterRepository.save(character);
    }
}
