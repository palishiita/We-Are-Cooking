package com.technosudo.userinfo.controller;

import com.technosudo.userinfo.dto.UserProfileDto;
import com.technosudo.userinfo.entity.UserProfileEntity;
import com.technosudo.userinfo.service.UserService;
import lombok.AllArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequestMapping("/api/user")
@AllArgsConstructor
public class UserController {

    private UserService userService;



    @GetMapping("{uuid}/email")
    public String getEmailByUuid() {
        return "TODO";
    }
    @GetMapping("{uuid}/username")
    public String getUsernameByUuid() {
        return "TODO";
    }
}
