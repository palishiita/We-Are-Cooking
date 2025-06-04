package com.technosudo.userinfo.entity.other;

import jakarta.persistence.Entity;
import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Column;
import org.springframework.data.relational.core.mapping.Table;

import java.util.UUID;

@Entity
@Table("recipes")
public record RecipeEntity(

        @Id @jakarta.persistence.Id
        UUID id,

        @Column("posting_user_id")
        UUID authorId,
        String name,
        String description
) {
}
