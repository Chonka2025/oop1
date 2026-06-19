package com.game.service;

import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

// Планировщик: @Scheduled(fixedRate=1000) — каждую секунду вызывает seekerService.tickStealthTimers(1.0f)
@Component
public class SeekerTimerScheduler {

    private final SeekerService seekerService;

    public SeekerTimerScheduler(SeekerService seekerService) {
        this.seekerService = seekerService;
    }

    @Scheduled(fixedRate = 1000)
    public void tickSeekers() {
        seekerService.tickStealthTimers(1.0f);
    }
}
