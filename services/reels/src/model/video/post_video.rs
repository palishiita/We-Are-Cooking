use actix_multipart::form::{MultipartForm, json::Json as MpJson, tempfile::TempFile};
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
    pub posting_user_id: Uuid,
    pub title: String,
    pub description: String,
    pub video_length_seconds: i32,
}
