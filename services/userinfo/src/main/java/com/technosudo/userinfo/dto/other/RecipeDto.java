package com.technosudo.userinfo.dto.other;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.relational.core.mapping.Column;

import java.util.UUID;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class RecipeDto {
    private UUID id;
    private UUID authorId;
    private String name;
    private String description;
}
