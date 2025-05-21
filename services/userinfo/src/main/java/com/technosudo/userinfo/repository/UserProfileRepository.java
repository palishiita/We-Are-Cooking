package com.technosudo.userinfo.repository;

import com.technosudo.userinfo.entity.UserProfileEntity;
import org.springframework.data.repository.CrudRepository;

import java.util.UUID;

public interface UserProfileRepository extends CrudRepository<UserProfileEntity, UUID> {

}
