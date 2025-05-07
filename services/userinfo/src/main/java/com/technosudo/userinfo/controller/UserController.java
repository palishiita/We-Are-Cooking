package com.technosudo.userinfo.controller;

import com.technosudo.userinfo.entity.UserProfile;
import com.technosudo.userinfo.service.UserService;
import lombok.AllArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequestMapping("/api/profiles")
@AllArgsConstructor
public class UserController {

    private UserService userService;

    @GetMapping("{id}")
    public UserProfile getUserProfile(@PathVariable UUID id) {
        return userService.getUserProfile(id);
    }
}
