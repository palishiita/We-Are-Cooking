use serde::Deserialize;
use uuid::Uuid;

#[derive(Deserialize)]
pub struct PostVideo {
    pub posting_user_id: Uuid,
    pub title: String,
    pub description: String,
    pub video_length_seconds: i32,
}