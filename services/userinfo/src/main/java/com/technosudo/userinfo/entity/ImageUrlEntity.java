package com.technosudo.userinfo.entity;

import jakarta.persistence.Entity;
import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Column;
import org.springframework.data.relational.core.mapping.Table;

import java.util.UUID;

@Entity
@Table(name = "photo_urls")
public record ImageUrlEntity(

        @Id @jakarta.persistence.Id @Column("id")
        UUID uuid,

        @Column("photo_url")
        String imageUrl
) {
}
