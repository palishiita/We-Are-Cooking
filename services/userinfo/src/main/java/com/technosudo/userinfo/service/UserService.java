package com.technosudo.userinfo.service;

import com.technosudo.userinfo.dto.UserProfileDto;
import com.technosudo.userinfo.entity.UserProfileEntity;
import com.technosudo.userinfo.repository.UserProfileRepository;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.UUID;

@Service
@AllArgsConstructor
public class UserService {

    private UserProfileRepository userProfileRepository;

    public UserProfileDto getUserProfile(UUID uuid) {
        var entity =  userProfileRepository.findById(uuid).orElseThrow();
        return new UserProfileDto(entity.userId(), "Username", entity.photoUrlId().toString(), entity.description());
    }
}
