package com.technosudo.userinfo.service;

import com.technosudo.userinfo.dto.ProfileDto;
import com.technosudo.userinfo.dto.ProfileSmallDto;
import com.technosudo.userinfo.dto.other.RecipeDto;
import com.technosudo.userinfo.dto.other.ReelDto;
import com.technosudo.userinfo.entity.ProfileEntity;
import com.technosudo.userinfo.repository.ImageRepository;
import com.technosudo.userinfo.repository.ProfileRepository;
import com.technosudo.userinfo.repository.RecipeRepository;
import com.technosudo.userinfo.repository.ReelRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.Collections;
import java.util.List;
import java.util.UUID;

@Service
@AllArgsConstructor
public class ProfileService {

    private ProfileRepository profileRepository;
    private ImageRepository imageRepository;
    private KeycloakService keycloakService;

    private RecipeRepository recipeRepository;
    private ReelRepository reelRepository;

    public ProfileDto getProfile(UUID userUuid, UUID principalUuid) {

        var user = keycloakService.getUserByUuid(userUuid);
        ProfileEntity profile = profileRepository.findById(userUuid).orElseGet(() -> register(userUuid));

        var builder = ProfileDto.builder()
                .userUuid(profile.getUserUuid())
                .userName(user.getUsername())
                .isBanned(false);
        if (false) // is banned
            return builder.build();

        var url = imageRepository.findById(profile.getImageUrlUuid()).orElseThrow();
        var urlSmall = imageRepository.findById(profile.getImageSmallUrlUuid()).orElseThrow();

        builder = builder
                .imageUrl(url.imageUrl())
                .imageSmallUrl(urlSmall.imageUrl())
                .isPrivate(profile.getIsPrivate());

        if (userUuid.equals(principalUuid) || !profile.getIsPrivate()) {
            return builder.bio(profile.getDescription())
                    .followers(Collections.emptyList())
                    .recipes(Collections.emptyList())
                    .reels(Collections.emptyList()).build();
        }
        return builder.build();
    }

    public ProfileSmallDto getSmallProfile(UUID userUuid) {

        ProfileEntity profile = profileRepository.findById(userUuid).orElseThrow();
        var user = keycloakService.getUserByUuid(userUuid);

        var builder = ProfileSmallDto.builder()
                .userUuid(profile.getUserUuid())
                .username(user.getUsername())
                .isBanned(false);
        if (false) // is banned
            return builder.build();

        var url = imageRepository.findById(profile.getImageUrlUuid()).orElseThrow();
        var urlSmall = imageRepository.findById(profile.getImageSmallUrlUuid()).orElseThrow();

        return builder
                .imageUrl(url.imageUrl())
                .imageSmallUrl(urlSmall.imageUrl()).build();
    }
    public List<ProfileSmallDto> getSmallProfiles(List<UUID> userUuid) {
//        Iterable<ProfileEntity> profile = profileRepository.findAllById(userUuid);
//        var urls = imageRepository.findAllById(StreamSupport.stream(profile.spliterator(), true)
//                .map(ProfileEntity::imageUrlUuid).toList());
//        var urlsSmall = imageRepository.findAllById(StreamSupport.stream(profile.spliterator(), true)
//                .map(ProfileEntity::imageSmallUrlUuid).toList());
//        var user = keycloakService.getUserByUuid(userUuid);
//
//        return ProfileDto.builder()
//                .userUuid(profile.userUuid())
//                .userName(user.getUsername())
//                .imageUrl(url.imageUrl())
//                .imageSmallUrl(urlSmall.imageUrl())
//                .isPrivate(profile.isPrivate())
//                .isBanned(false).build();
        return Collections.emptyList();
    }

    ProfileEntity register(UUID userUuid) {
        return profileRepository.save(ProfileEntity.builder()
                .userUuid(userUuid)
                .imageUrlUuid(UUID.fromString("847cd648-bcf5-4b5d-8905-e579a285b5e6"))
                .imageSmallUrlUuid(UUID.fromString("847cd648-bcf5-4b5d-8905-e579a285b5e6"))
                .isPrivate(false)
                .description("New account")
                .isNew(true).build());
    }

    public List<RecipeDto> getUserRecipes(UUID userUuid) {
        var recipes = recipeRepository.getAllByAuthorId(userUuid);
        return recipes.stream()
                .map(entity -> RecipeDto.builder()
                        .id(entity.id())
                        .authorId(entity.authorId())
                        .name(entity.name())
                        .description(entity.description()).build())
                .toList();
    }
    public List<ReelDto> getUserReels(UUID userUuid) {
        var reels = reelRepository.getAllByAuthorId(userUuid);
        return reels.stream()
                .map(entity -> ReelDto.builder()
                        .id(entity.id())
                        .authorId(entity.authorId())
                        .videoId(entity.videoId())
                        .title(entity.title())
                        .description(entity.description()).build())
                .toList();
    }
}
