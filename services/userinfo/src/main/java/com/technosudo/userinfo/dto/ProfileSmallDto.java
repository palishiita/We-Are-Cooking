package com.technosudo.userinfo.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ProfileSmallDto {
    private UUID userUuid;
    private String username;
    private String imageUrl;
    private String imageSmallUrl;
    private Boolean isBanned;
}
