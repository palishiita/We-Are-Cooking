package com.technosudo.userinfo.controller;

import com.technosudo.userinfo.dto.UserProfileDto;
import com.technosudo.userinfo.service.UserService;
import lombok.AllArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
@AllArgsConstructor
@RequestMapping("/api/profile")
public class ProfileController {

    private UserService userService;

    @GetMapping("{uuid}")
    public UserProfileDto getUserDetailsBy(@PathVariable UUID uuid) {
        return userService.getUserProfile(uuid);
    }
}
