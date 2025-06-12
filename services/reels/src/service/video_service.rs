use async_trait::async_trait;
use std::{path::PathBuf, sync::Arc};
use tokio::{fs::{self, File}, io::AsyncWriteExt};
use uuid::Uuid;
use bytes::BytesMut;

use crate::{
    dao::database_context::Database, error::error::AppError, model::{PostVideo, Video}
};

#[async_trait]
pub trait VideoRepository<'a> {
    fn new(db: Arc<Database<'a>>) -> Self;
    async fn get_video_by_id(&self, video_id: Uuid) -> Result<Video, AppError>;
    async fn get_video_by_reel_id(&self, reel_id: Uuid) -> Result<Video, AppError>;    async fn post_video(
        &self,
        video: PostVideo,
        posting_user_id: Uuid,
        temp_file: BytesMut,
        file_name: String,
    ) -> Result<Uuid, AppError>;
    async fn put_video(&self, video_id: Uuid, video: PostVideo, posting_user_id: Uuid) -> Result<(), AppError>;
    async fn delete_video(&self, video_id: Uuid) -> Result<(), AppError>;
}

pub struct VideoService<'a> {
    pub db: Arc<Database<'a>>,
}

#[async_trait]
impl<'a> VideoRepository<'a> for VideoService<'a> {
    fn new(db: Arc<Database<'a>>) -> Self {
        VideoService { db }
    }

    async fn get_video_by_id(&self, video_id: Uuid) -> Result<Video, AppError> {
        match self.db.videos.get_video_by_id(video_id).await {
            Ok(video) => Ok(video),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }

    async fn get_video_by_reel_id(&self, reel_id: Uuid) -> Result<Video, AppError> {
        match self.db.videos.get_video_by_reel_id(reel_id).await {
            Ok(video) => Ok(video),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }    async fn post_video(
        &self,
        video: PostVideo,
        posting_user_id: Uuid,
        file_bytes: BytesMut,
        file_name: String,
    ) -> Result<Uuid, AppError> {
        let extension = PathBuf::from(&file_name)
            .extension()
            .and_then(std::ffi::OsStr::to_str)
            .map_or(String::new(), |ext| format!(".{}", ext));

        let video_id = Uuid::new_v4();
        let unique_filename = format!("{}{}", video_id, extension);
        let dir = PathBuf::from("./upload");

        std::fs::create_dir_all(&dir).map_err(|e| AppError::InternalError(e.to_string()))?;

        let path = dir.join(&unique_filename);
        
        let mut file = File::create(&path).await.map_err(|e| AppError::InternalError(e.to_string()))?;
        file.write_all(&file_bytes).await.map_err(|e| AppError::InternalError(e.to_string()))?;

        let video_url = format!("/static/videos/{}", unique_filename);        let video: Video = Video {
            id: video_id,
            posting_user_id: posting_user_id,
            description: video.description,
            title: video.title,
            video_length_seconds: video.video_length_seconds,
            video_url: video_url,
        };

        match self.db.videos.post_video(&video).await {
            Ok(video_id) => Ok(video_id),
            Err(e) => Err(AppError::InternalError(e.to_string())),
        }
    }

    async fn put_video(&self, video_id: Uuid, video: PostVideo, posting_user_id: Uuid) -> Result<(), AppError> {
        todo!();
        // let video: Video = Video {
        //     id: video_id,
        //     posting_user_id: video.posting_user_id,
        //     description: video.description.clone(),
        //     title: video.description.clone(),
        //     video_length_seconds: video.video_length_seconds,
        //     video_url: "placeholder".to_string(),
        // };
        
        // match self.db.videos.put_video(video).await {
        //     Ok(_) => Ok(()),
        //     Err(e) => Err(AppError::InternalError(e.to_string())),
        // }
    }

    async fn delete_video(&self, video_id: Uuid) -> Result<(), AppError> {
        match self.db.videos.delete_video(video_id).await {
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
