package com.technosudo.userinfo.service;

import org.keycloak.OAuth2Constants;
import org.keycloak.admin.client.Keycloak;
import org.keycloak.admin.client.KeycloakBuilder;
import org.keycloak.representations.idm.UserRepresentation;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Service
public class KeycloakService {

    private final Keycloak keycloak;
    private final String realm = "Techstructure";
    // ak64diQm75VdTgDPshJJxKXcZ6UH0BuZ

    public KeycloakService() {
        this.keycloak = KeycloakBuilder.builder()
                .serverUrl("http://login.techstructure.com:8080") // Your Keycloak URL
                .realm("master") // Realm for admin login
                .clientId("admin-cli") // Use admin-cli for password grant
                .username("admin") // Admin user
                .password("admin") // Admin password
                .grantType(OAuth2Constants.PASSWORD)
                .build();
    }

    public UserRepresentation getUserByUuid(UUID uuid) {
        return keycloak.realm(realm).users().get(uuid.toString()).toRepresentation();
    }
}
