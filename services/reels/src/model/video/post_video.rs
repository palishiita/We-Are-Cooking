use actix_multipart::form::{MultipartForm, json::Json as MpJson, tempfile::TempFile};

use super::video::Video;

#[derive(Debug, MultipartForm)]
pub struct PostVideo {
    #[multipart(limit = "100MB")]
    pub file: TempFile,
    pub json: MpJson<Video>,
}
