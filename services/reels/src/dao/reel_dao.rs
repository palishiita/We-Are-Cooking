use sqlx::types::chrono::Utc;
use uuid::Uuid;

use crate::model::{Reel, ReelWithVideos, Video};

use super::database_context::Table;

impl<'c> Table<'c, Reel> {
    pub async fn drop_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query("DROP TABLE IF EXISTS reels;")
            .execute(&*self.pool)
            .await
            .map(|_| ())
    }

    pub async fn create_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query(
            r#"

            "#,
        )
        .execute(&*self.pool)
        .await
        .map(|_| ())
    }

    pub async fn get_reel_by_id(&self, reel_id: Uuid) -> Result<Reel, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT *
                FROM reels
                WHERE id = $1
            "#,
        )
        .bind(reel_id)
        .fetch_one(&*self.pool)
        .await
    }

    pub async fn get_reels_paginated(
        &self,
        offset: i64,
        limit: i64,
    ) -> Result<Vec<Reel>, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT * 
                FROM reels
                ORDER BY creation_timestamp DESC
                LIMIT $1 OFFSET $2
            "#,
        )
        .bind(limit)
        .bind(offset)
        .fetch_all(&*self.pool)
        .await
    }

    pub async fn get_reels_by_user_id_paginated(
        &self,
        user_id: Uuid,
        offset: i64,
        limit: i64,
    ) -> Result<Vec<Reel>, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT * 
                FROM reels
                WHERE posting_user_id = $1
                ORDER BY creation_timestamp DESC
                LIMIT $2 OFFSET $3
            "#,
        )
        .bind(user_id)
        .bind(limit)
        .bind(offset)
        .fetch_all(&*self.pool)
        .await
    }

    pub async fn get_reels_with_videos_paginated(
        &self,
        offset: i64,
        limit: i64,
    ) -> Result<ReelWithVideos, sqlx::Error> {
        let reels: Vec<Reel> = sqlx::query_as::<_, Reel>(
            r#"
                SELECT * 
                FROM reels
                ORDER BY creation_timestamp DESC
                LIMIT $1 OFFSET $2
            "#,
        )
        .bind(limit)
        .bind(offset)
        .fetch_all(&*self.pool)
        .await?;

        let video_ids: Vec<Uuid> = reels.iter()
            .map(|r| r.video_id)
            .collect();
        println!("video id {:?}",video_ids);
        let videos: Vec<Video> = sqlx::query_as(
            r#"
                SELECT * 
                FROM videos
                WHERE id = ANY($1)
            "#,
        )
        .bind(video_ids)
        .fetch_all(&*self.pool)
        .await?;
        
        Ok(ReelWithVideos {
            reels: reels,
            videos: videos,
        })
    }

    pub async fn post_reel(&self, reel: &Reel) -> Result<u64, sqlx::Error> {
        let _ = self.create_table().await;
        sqlx::query(
            r#"
                INSERT INTO reels (id, video_id, posting_user_id, title, description, creation_timestamp)
                VALUES($1, $2, $3, $4, $5, $6)
            "#
        )
            .bind(reel.id)               
            .bind(reel.video_id)         
            .bind(reel.posting_user_id)
            .bind(reel.title.clone())
            .bind(reel.description.clone())
            .bind(Utc::now().naive_utc())
            .execute(&*self.pool) 
            .await
            .map(|x| x.rows_affected())
    }

    pub async fn delete_reel(&self, reel_id: Uuid) -> Result<Option<String>, sqlx::Error> {
        let mut tx = self.pool.begin().await?;

        let video_id: Option<(Uuid,)> = sqlx::query_as::<_, (Uuid,)>(
            "SELECT video_id FROM reels WHERE id = $1"
        )
        .bind(reel_id)
        .fetch_optional(&mut *tx)
        .await?;
    
        if let Some((video_id,)) = video_id {
            let video_url: Option<(String,)> = sqlx::query_as::<_, (String,)>(
                "SELECT video_url FROM videos WHERE id = $1"
            )
            .bind(video_id)
            .fetch_optional(&mut *tx)
            .await?;
    
            sqlx::query("DELETE FROM reels WHERE id = $1")
                .bind(reel_id)
                .execute(&mut *tx)
                .await?;
    
            sqlx::query("DELETE FROM videos WHERE id = $1")
                .bind(video_id)
                .execute(&mut *tx)
                .await?;
    
            tx.commit().await?;
            Ok(video_url.map(|v| v.0))
        } else {
            tx.commit().await?;
            Ok(None)
        }
    }
}
