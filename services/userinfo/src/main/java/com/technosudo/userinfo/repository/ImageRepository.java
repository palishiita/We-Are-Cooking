package com.technosudo.userinfo.repository;

import com.technosudo.userinfo.entity.ImageUrlEntity;
import org.springframework.data.repository.CrudRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
public interface ImageRepository extends CrudRepository<ImageUrlEntity, UUID> {
}
