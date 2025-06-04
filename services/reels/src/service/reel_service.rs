use async_trait::async_trait;
use sqlx::types::chrono::{NaiveDateTime, Utc};
use tokio::fs;
use std::sync::Arc;
use uuid::Uuid;

use crate::{
    dao::database_context::Database, error::error::AppError, model::{PostReel, Reel, ReelWithVideos}
};

#[async_trait]
pub trait ReelRepository<'a>: Send + Sync {
    fn new(db: Arc<Database<'a>>) -> Self;
    async fn get_reel_by_id(&self, reel_id: Uuid) -> Result<Reel, AppError>;
    async fn get_reels_paginated(
        &self,
        page: u32,
        limit: u32,
    ) -> Result<Vec<Reel>, AppError>;
    async fn get_reels_by_user_id(
        &self,
        user_id: Uuid,
        page: u32,
        limit: u32,
    ) -> Result<Vec<Reel>, AppError>;
    async fn get_reels_with_videos_paginated(
        &self,
        page: u32,
        limit: u32,
    ) -> Result<ReelWithVideos, AppError>;
    async fn post_reel(&self, reel: PostReel, video_id: Option<Uuid>) -> Result<(), AppError>;
    // async fn post_reel_with_video(
    //     &self,
    //     reel: PostReel,
    //     video: PostVideo,
    //     file: BytesMut,
    //     file_name: String,
    // ) -> Result<(), AppError>;
    async fn put_reel(&self, reel: PostReel) -> Result<(), actix_web::Error>;
    async fn delete_reel_with_video(&self, reel_id: Uuid) -> Result<(), AppError>;
}

pub struct ReelService<'a> {
    pub db: Arc<Database<'a>>,
}

#[async_trait]
impl<'a> ReelRepository<'a> for ReelService<'a> {
    fn new(db: Arc<Database<'a>>) -> Self {
        ReelService { db }
    }

    async fn get_reel_by_id(&self, reel_id: Uuid) -> Result<Reel, AppError> {
        match self.db.reels.get_reel_by_id(reel_id).await {
            Ok(reels) => Ok(reels),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }

    async fn get_reels_paginated(
        &self,
        page: u32,
        limit: u32,
    ) -> Result<Vec<Reel>, AppError> {
        let offset = (page.saturating_sub(1) * limit) as i64;
        let limit = limit as i64;

        match self.db.reels.get_reels_paginated(offset, limit).await {
            Ok(reels) => Ok(reels),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }

    async fn get_reels_by_user_id(
        &self,
        user_id: Uuid,
        page: u32,
        limit: u32,
    ) -> Result<Vec<Reel>, AppError> {
        let offset = (page.saturating_sub(1) * limit) as i64;
        let limit = limit as i64;

        match self.db.reels.get_reels_by_user_id_paginated(user_id, offset, limit).await {
            Ok(reels) => Ok(reels),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }

    async fn get_reels_with_videos_paginated(
        &self,
        page: u32,
        limit: u32,
    ) -> Result<ReelWithVideos, AppError> {
        let offset = (page.saturating_sub(1) * limit) as i64;
        let limit = limit as i64;

        match self.db.reels.get_reels_with_videos_paginated(offset, limit).await {
            Ok(reels_with_videos) => Ok(reels_with_videos),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }

    async fn post_reel(&self, reel: PostReel, video_id: Option<Uuid>) -> Result<(), AppError> {
        let reel_id: Uuid = Uuid::new_v4();
        let timestamp: NaiveDateTime = Utc::now().naive_utc();

        let reel: Reel = Reel {
            id: reel_id,
            video_id: video_id.unwrap_or(Uuid::new_v4()),
            posting_user_id: reel.posting_user_id,
            title: reel.title,
            description: reel.description,
            creation_timestamp: timestamp,
        };

        let _ = self.db.reels.post_reel(&reel).await;

        Ok(())
    }

    // async fn post_reel_with_video(
    //     &self,
    //     reel: PostReel,
    //     video: PostVideo,
    //     file: BytesMut,
    //     file_name: String,
    // ) -> Result<(), AppError> {
    //     todo!()
    // }

    async fn put_reel(&self, reel: PostReel) -> Result<(), actix_web::Error> {
        todo!()
    }

    async fn delete_reel_with_video(&self, reel_id: Uuid) -> Result<(), AppError> {
        match self.db.reels.delete_reel(reel_id).await {
            Ok(Some(video_url)) => {
                if let Err(e) = fs::remove_file(&video_url).await {
                    return Err(AppError::InternalError(e.to_string()))
                }
                Ok(())
            }
            Ok(None) => Err(AppError::InternalError("Video not found".into())),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }
}
