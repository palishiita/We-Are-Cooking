package com.technosudo.userinfo.repository;

import com.technosudo.userinfo.entity.other.ReelEntity;
import org.springframework.data.repository.PagingAndSortingRepository;

import java.util.List;
import java.util.UUID;

public interface ReelRepository extends PagingAndSortingRepository<ReelEntity, UUID> {
    List<ReelEntity> getAllByAuthorId(UUID authorId);
}
