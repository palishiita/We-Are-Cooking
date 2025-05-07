package com.technosudo.userinfo.service;

import com.technosudo.userinfo.entity.UserProfile;
import com.technosudo.userinfo.repository.UserProfileRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Service
@AllArgsConstructor
public class UserService {

    private UserProfileRepository userProfileRepository;

    public UserProfile getUserProfile(UUID uuid) {
        return userProfileRepository.findById(uuid).orElseThrow();
    }

    public UserProfile getUserProfiles(UUID uuid) {
        return userProfileRepository.findById(uuid).orElseThrow();
    }
}
