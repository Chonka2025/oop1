package com.game.controller;

import com.game.entity.Guild;
import com.game.service.GuildService;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;

@Controller
@RequestMapping("/guilds")
public class GuildController {

    private final GuildService guildService;

    public GuildController(GuildService guildService) {
        this.guildService = guildService;
    }

    // /guilds — список всех гильдий
    @GetMapping
    public String list(Model model) {
        model.addAttribute("guilds", guildService.findAll());
        return "guilds/list";
    }

    // /guilds/new — форма создания новой гильдии
    @GetMapping("/new")
    public String createForm() {
        return "guilds/form";
    }

    // /guilds — сохранить новую гильдию, затем редирект на её страницу
    @PostMapping
    public String save(@RequestParam String name, @RequestParam(required = false) String description) {
        Guild guild = guildService.save(name, description);
        return "redirect:/guilds/" + guild.getId();
    }

    // /guilds/{id} — карточка гильдии (участники + форма добавления по имени)
    @GetMapping("/{id}")
    public String details(@PathVariable Long id, Model model) {
        model.addAttribute("guild", guildService.findById(id));
        return "guilds/details";
    }

    // /guilds/{id}/edit — форма редактирования гильдии
    @GetMapping("/{id}/edit")
    public String editForm(@PathVariable Long id, Model model) {
        model.addAttribute("guild", guildService.findById(id));
        return "guilds/form";
    }

    // /guilds/{id}/edit — сохранить изменения гильдии
    @PostMapping("/{id}/edit")
    public String update(@PathVariable Long id, @RequestParam String name,
                          @RequestParam(required = false) String description) {
        guildService.update(id, name, description);
        return "redirect:/guilds/" + id;
    }

    // /guilds/{id}/delete — удалить гильдию и вернуться к списку
    @PostMapping("/{id}/delete")
    public String delete(@PathVariable Long id) {
        guildService.deleteById(id);
        return "redirect:/guilds";
    }

    // /guilds/{id}/addMember — добавить персонажа по имени
    @PostMapping("/{id}/addMember")
    public String addMember(@PathVariable Long id, @RequestParam String name, Model model) {
        String error = guildService.addMemberByName(id, name);
        if (error != null) {
            model.addAttribute("guild", guildService.findById(id));
            model.addAttribute("error", error);
            return "guilds/details";
        }
        return "redirect:/guilds/" + id;
    }

    // /guilds/{id}/removeMember — исключить персонажа из гильдии
    @PostMapping("/{id}/removeMember")
    public String removeMember(@PathVariable Long id, @RequestParam Long characterId) {
        guildService.removeMember(id, characterId);
        return "redirect:/guilds/" + id;
    }
}
