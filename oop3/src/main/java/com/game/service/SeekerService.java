package com.game.service;

import com.game.entity.Seeker;
import com.game.repository.SeekerRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

// Сервис для Seeker: CRUD через SeekerRepository + tickStealthTimers (раз в секунду уменьшает таймер скрытности)
@Service
@Transactional
public class SeekerService {

    private final SeekerRepository seekerRepository;

    public SeekerService(SeekerRepository seekerRepository) {
        this.seekerRepository = seekerRepository;
    }

    public Seeker save(Seeker seeker) {
        return seekerRepository.save(seeker);
    }

    public Optional<Seeker> findById(Long id) {
        return seekerRepository.findById(id);
    }

    public List<Seeker> findAll() {
        List<Seeker> seekers = new ArrayList<>();
        seekerRepository.findAll().forEach(seekers::add);
        return seekers;
    }

    public void deleteById(Long id) {
        seekerRepository.deleteById(id);
    }

    public void delete(Seeker seeker) {
        seekerRepository.delete(seeker);
    }

    // Тик таймеров скрытности для всех Seekers (вызывается раз в секунду из SeekerTimerScheduler)
    public void tickStealthTimers(float deltaSeconds) {
        List<Seeker> seekers = findAll();
        for (Seeker seeker : seekers) {
            if (Boolean.TRUE.equals(seeker.getIsStealthed())) {
                seeker.update(deltaSeconds);
                seekerRepository.save(seeker);
            }
        }
    }
}