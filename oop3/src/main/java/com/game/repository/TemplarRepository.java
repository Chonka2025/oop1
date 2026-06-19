package com.game.repository;

import com.game.entity.Templar;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

// Репозиторий для Templar (только Templar'ов)
@Repository
public interface TemplarRepository extends CrudRepository<Templar, Long> {
}