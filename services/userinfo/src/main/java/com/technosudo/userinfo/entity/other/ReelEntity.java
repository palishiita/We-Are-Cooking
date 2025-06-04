package com.technosudo.userinfo.entity.other;

import jakarta.persistence.Entity;
import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Column;
import org.springframework.data.relational.core.mapping.Table;

import java.util.UUID;

@Entity
@Table("reels")
public record ReelEntity(

        @Id @jakarta.persistence.Id
        UUID id,

        UUID videoId,
        @Column("posting_user_id")
        UUID authorId,
        String title,
        String description
) {
}
