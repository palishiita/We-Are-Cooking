package com.technosudo.userinfo.controller;

import com.technosudo.userinfo.service.KeycloakService;
import lombok.AllArgsConstructor;
import org.keycloak.representations.idm.UserRepresentation;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@RestController
@RequestMapping("/api/user")
@AllArgsConstructor
public class UserController {

    private KeycloakService keycloakService;

    @GetMapping("/{uuid}")
    public UserRepresentation getUserDetailsByUuid(@PathVariable UUID uuid) {
        return keycloakService.getUserByUuid(uuid);
    }
}
