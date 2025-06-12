package com.technosudo.userinfo.repository;

import com.technosudo.userinfo.entity.other.RecipeEntity;
import org.springframework.data.repository.PagingAndSortingRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.UUID;

@Repository
public interface RecipeRepository extends PagingAndSortingRepository<RecipeEntity, UUID> {
    List<RecipeEntity> getAllByAuthorId(UUID authorId);
}
