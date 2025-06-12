package com.technosudo.userinfo.entity;

import jakarta.persistence.Entity;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.annotation.Id;
import org.springframework.data.annotation.Transient;
import org.springframework.data.domain.Persistable;
import org.springframework.data.relational.core.mapping.Table;

import java.util.UUID;

@Entity
@Table(name = "user_profiles")
@AllArgsConstructor
@NoArgsConstructor
@Data
@Builder
public class ProfileEntity implements Persistable<UUID> {

        @Id @jakarta.persistence.Id
        private UUID userUuid;

        private UUID imageUrlUuid;
        private UUID imageSmallUrlUuid;

        private Boolean isPrivate;

        private String description;

        @Transient
        private boolean isNew;

        @Override
        public UUID getId() {
                return this.userUuid;
        }

        @Override
        public boolean isNew() {
                return this.isNew;
        }
}
