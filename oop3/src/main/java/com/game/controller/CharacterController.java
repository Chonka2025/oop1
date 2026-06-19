package com.game.controller;

import com.game.dto.CharacterForm;
import com.game.entity.Character;
import com.game.service.CharacterService;
import com.game.service.GuildService;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;

@Controller
@RequestMapping("/characters")
public class CharacterController {

    private final CharacterService characterService;
    private final GuildService guildService;

    public CharacterController(CharacterService characterService, GuildService guildService) {
        this.characterService = characterService;
        this.guildService = guildService;
    }

    // Список всех персонажей (с поиском и сортировкой)
    @GetMapping
    public String list(
        @org.springframework.web.bind.annotation.RequestParam(required = false) String search,
        @org.springframework.web.bind.annotation.RequestParam(required = false, defaultValue = "asc") String sort,
        Model model
    ) {
        model.addAttribute("characters", characterService.findAll(search, sort));
        model.addAttribute("search", search);
        model.addAttribute("sort", sort);
        return "characters/list";
    }

    // Форма создания нового персонажа (с дефолтными значениями)
    @GetMapping("/new")
    public String createForm(Model model) {
        CharacterForm form = new CharacterForm();
        form.setType("SEEKER");
        form.setLevel(1);
        form.setHealth(100);
        form.setMaxHealth(100);
        form.setBaseDamage(10);
        form.setRegenAmount(2);
        form.setRegenIntervalSec(1.0f);
        form.setStrength(10);
        form.setAgility(10);
        form.setEnergy(50);
        form.setDodgeChance(15);
        model.addAttribute("form", form);
        model.addAttribute("editMode", false);
        form.setArmor(5);
        form.setBlockChance(1);
        form.setFaith(10);
        form.setStealthSeconds(0.0f);
        form.setHolyPower(1);
        form.setAegisHealAmount(10);
        form.setAegisTicksRemaining(5);
        model.addAttribute("allGuilds", guildService.findAll());
        return "characters/form";
    }

    // Детальная страница персонажа
    @GetMapping("/{id}")
    public String details(@PathVariable Long id, Model model) {
        Character character = characterService.findById(id);
        model.addAttribute("character", character);
        return "characters/details";
    }

    // Форма редактирования существующего персонажа
    @GetMapping("/{id}/edit")
    public String editForm(@PathVariable Long id, Model model) {
        Character character = characterService.findById(id);
        model.addAttribute("form", characterService.toForm(character));
        model.addAttribute("editMode", true);
        model.addAttribute("allGuilds", guildService.findAll());
        return "characters/form";
    }

    // Сохранение (создание или обновление) персонажа из формы
    @PostMapping // Запрос на изменение
    public String save(@ModelAttribute("form") CharacterForm form) {
        Character character = characterService.saveFromForm(form);
        return "redirect:/characters/" + character.getId();
    }

    // Удаление персонажа
    @PostMapping("/{id}/delete")
    public String delete(@PathVariable Long id) {
        characterService.deleteById(id);
        return "redirect:/characters";
    }

    // Выполнение игрового действия: useSpecial, useAbility, tickOneSecond, takeDamage, heal
    @PostMapping("/{id}/actions/{action}")
    public String runAction(@PathVariable Long id, @PathVariable String action) {
        characterService.runAction(id, action);
        return "redirect:/characters/" + id;
    }
}
