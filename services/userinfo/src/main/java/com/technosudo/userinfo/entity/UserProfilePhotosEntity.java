package com.technosudo.userinfo.entity;

import jakarta.persistence.Entity;
import org.springframework.data.relational.core.mapping.Table;

@Entity
@Table("photo_urls")
public record UserProfilePhotosEntity() {
}
