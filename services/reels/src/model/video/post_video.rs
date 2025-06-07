use actix_multipart::form::{json::Json as MpJson, tempfile::TempFile, MultipartForm};
use utoipa::*;
use uuid::Uuid;

#[derive(Debug, MultipartForm, ToSchema)]
pub struct VideoForm {
    #[schema(value_type = String, format = Binary)]
    #[multipart(limit = "100MB")]
    pub file: TempFile,
    #[schema(value_type = PostVideo)]
    pub video: MpJson<PostVideo>,
}

#[derive(serde::Serialize, serde::Deserialize, Clone, Debug, ToSchema)]
pub struct PostVideo {
    pub title: String,
    pub description: String,
    pub video_length_seconds: i32,
}
