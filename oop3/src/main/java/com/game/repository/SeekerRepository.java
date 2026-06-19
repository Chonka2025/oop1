package com.game.repository;

import com.game.entity.Seeker;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

// Репозиторий для Seeker (только Seekers)
@Repository
public interface SeekerRepository extends CrudRepository<Seeker, Long> {
}