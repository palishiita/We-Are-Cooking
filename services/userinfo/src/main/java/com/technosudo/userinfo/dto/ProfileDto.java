package com.technosudo.userinfo.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;
import java.util.UUID;

@AllArgsConstructor
@NoArgsConstructor
@Data
@Builder
public class ProfileDto {
    private UUID userUuid;
    private String userName;
    private String imageUrl;
    private String imageSmallUrl;

    private Boolean isPrivate;
    private Boolean isBanned;

    private String bio;
    private List<UUID> followers;
    // future list of reels, reviews, recipes
    private List<UUID> recipes;
    private List<UUID> reels;

}
