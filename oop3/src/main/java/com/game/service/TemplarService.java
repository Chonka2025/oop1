package com.game.service;

import com.game.entity.Templar;
import com.game.repository.TemplarRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

// Сервис для Templar: CRUD через TemplarRepository + tickAegisTimers (раз в секунду тикает лечение Divine Aegis)
@Service
@Transactional
public class TemplarService {

    private final TemplarRepository templarRepository;

    public TemplarService(TemplarRepository templarRepository) {
        this.templarRepository = templarRepository;
    }

    public Templar save(Templar templar) {
        return templarRepository.save(templar);
    }

    public Optional<Templar> findById(Long id) {
        return templarRepository.findById(id);
    }

    public List<Templar> findAll() {
        List<Templar> templars = new ArrayList<>();
        templarRepository.findAll().forEach(templars::add);
        return templars;
    }

    public void deleteById(Long id) {
        templarRepository.deleteById(id);
    }

    public void delete(Templar templar) {
        templarRepository.delete(templar);
    }

    // Тик купола для всех Templar (вызывается раз в секунду из TemplarTimerScheduler)
    public void tickAegisTimers(float deltaSeconds) {
        List<Templar> templars = findAll();
        for (Templar templar : templars) {
            if (Boolean.TRUE.equals(templar.getIsImmobilized()) && templar.getAegisTicksRemaining() > 0) {
                templar.update(deltaSeconds);
                templarRepository.save(templar);
            }
        }
    }
}