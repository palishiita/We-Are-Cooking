package com.technosudo.userinfo.entity;

import jakarta.persistence.Entity;
import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Table;

import java.util.UUID;

@Entity
@Table(name = "user_profiles")
public record ProfileEntity(

        @Id @jakarta.persistence.Id
        UUID userUuid,

        UUID imageUrlUuid,
        UUID imageSmallUrlUuid,

        Boolean isPrivate,

        String description
) {
}
