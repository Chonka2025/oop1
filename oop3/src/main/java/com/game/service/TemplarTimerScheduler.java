package com.game.service;

import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

// Планировщик: @Scheduled(fixedRate=1000) — каждую секунду вызывает templarService.tickAegisTimers(1.0f)
@Component
public class TemplarTimerScheduler {

    private final TemplarService templarService;

    public TemplarTimerScheduler(TemplarService templarService) {
        this.templarService = templarService;
    }

    @Scheduled(fixedRate = 1000)
    public void tickTemplars() {
        templarService.tickAegisTimers(1.0f);
    }
}