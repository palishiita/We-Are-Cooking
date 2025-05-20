package com.technosudo.userinfo.dto;

import lombok.AllArgsConstructor;
import lombok.NoArgsConstructor;

import java.util.UUID;

@AllArgsConstructor
@NoArgsConstructor
public class UserProfileDto {
    private UUID uuid;
    private String username;
    private String photoUrl;
    private String bio;
}
