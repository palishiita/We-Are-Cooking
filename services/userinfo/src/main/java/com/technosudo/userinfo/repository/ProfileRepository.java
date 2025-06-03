package com.technosudo.userinfo.repository;

import com.technosudo.userinfo.entity.ProfileEntity;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface ProfileRepository extends CrudRepository<ProfileEntity, UUID> {
    @Override
    List<ProfileEntity> findAllById(Iterable<UUID> ids);
}
