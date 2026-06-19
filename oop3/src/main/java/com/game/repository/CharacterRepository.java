package com.game.repository;

import com.game.entity.Character;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface CharacterRepository extends CrudRepository<Character, Long> {
    List<Character> findByNameContainingIgnoreCaseAndGuildIsNull(String name);
}
