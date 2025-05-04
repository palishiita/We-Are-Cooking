use actix_multipart::form::{MultipartForm, json::Json as MpJson, tempfile::TempFile};
use uuid::Uuid;

#[derive(Debug, MultipartForm)]
pub struct VideoForm {
    #[multipart(limit = "100MB")]
    pub file: TempFile,
    pub video: MpJson<PostVideo>,
}

#[derive(serde::Serialize, serde::Deserialize, Clone, Debug)]
pub struct PostVideo {
    pub posting_user_id: Uuid,
    pub title: String,
    pub description: String,
    pub video_length_seconds: i32,
}