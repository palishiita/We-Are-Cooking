package com.technosudo.userinfo.entity;

import jakarta.persistence.Entity;
import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Table;

import java.util.UUID;

@Entity
@Table(name = "user_profiles")
public record UserProfile(

        @Id @jakarta.persistence.Id
        UUID userId,

        UUID photoUrlId,
        String description
) {
}
