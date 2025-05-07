use actix_multipart::form::{MultipartForm, json::Json as MpJson, tempfile::TempFile};
use utoipa::*;

use crate::model::{PostReel, PostVideo, Reel, Video};

#[derive(Debug, MultipartForm, ToSchema)]
pub struct ReelWithVideosForm {
    #[schema(value_type = String, format = Binary)]
    #[multipart(limit = "100MB")]
    pub file: TempFile,
    #[schema(value_type = PostReel)]
    pub reel: MpJson<PostReel>,
    #[schema(value_type = PostVideo)]
    pub video: MpJson<PostVideo>,
}

#[derive(serde::Deserialize, serde::Serialize, ToSchema)]
pub struct ReelWithVideos {
    pub reels: Vec<Reel>,
    pub videos: Vec<Video>,
}
