package com.technosudo.userinfo.controller;

import com.technosudo.userinfo.dto.ProfileDto;
import com.technosudo.userinfo.dto.ProfileSmallDto;
import com.technosudo.userinfo.dto.other.RecipeDto;
import com.technosudo.userinfo.dto.other.ReelDto;
import com.technosudo.userinfo.service.ProfileService;
import lombok.AllArgsConstructor;
import org.springframework.http.HttpHeaders;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Objects;
import java.util.UUID;

@RestController
@AllArgsConstructor
public class ProfileController {

    private ProfileService profileService;

    @GetMapping("/me")
    public ProfileDto getOwnProfile(@RequestHeader HttpHeaders headers) {
        var principalUuid = UUID.fromString(Objects.requireNonNull(headers.getFirst("X-Uuid")));
        return profileService.getProfile(principalUuid, principalUuid);
    }

    @GetMapping("/{uuid}")
    public ProfileDto getProfile(@RequestHeader HttpHeaders headers, @PathVariable UUID uuid) {
        var principalUuid = UUID.fromString(Objects.requireNonNull(headers.getFirst("X-Uuid")));
        return profileService.getProfile(uuid, principalUuid);
    }

    //    @GetMapping("/{uuid}/followers")
//    public ProfileDto getUserProfile(@PathVariable UUID uuid) {
//        return userService.getUserProfile(uuid);
//    }
//    @GetMapping("/{uuid}/following")
//    public ProfileDto getUserProfile(@PathVariable UUID uuid) {
//        return userService.getUserProfile(uuid);
//    }
    @GetMapping("/{uuid}/reels")
    public List<ReelDto> getUserReels(@PathVariable UUID uuid) {
        return profileService.getUserReels(uuid);
    }
    @GetMapping("/{uuid}/recipes")
    public List<RecipeDto> getUserRecipes(@PathVariable UUID uuid) {
        return profileService.getUserRecipes(uuid);
    }
//    @GetMapping("/{uuid}/reviews")
//    public ProfileDto getUserProfile(@PathVariable UUID uuid) {
//        return userService.getUserProfile(uuid);
//    }

    @GetMapping("/small")
    public List<ProfileSmallDto> getSmallProfiles(@RequestBody List<UUID> uuids) {
        return profileService.getSmallProfiles(uuids);
    }
    @GetMapping("/small/{uuid}")
    public ProfileSmallDto getSmallProfile(@PathVariable UUID uuid) {
        return profileService.getSmallProfile(uuid);
    }
}
