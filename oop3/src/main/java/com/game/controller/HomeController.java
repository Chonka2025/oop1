package com.game.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class HomeController {

    // Перенаправление с корня "/" на список персонажей
    @GetMapping("/")
    public String home() {
        return "redirect:/characters";
    }
}
