package com.technosudo.userinfo.dto.other;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class ReelDto {
    UUID id;
    UUID authorId;
    UUID videoId;
    String title;
    String description;
}
