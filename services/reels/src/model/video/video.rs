use sqlx::postgres::PgRow;
use sqlx::{FromRow, Row};
use utoipa::ToSchema;
use uuid::Uuid;

#[derive(serde::Serialize, serde::Deserialize, Clone, Debug, ToSchema)]
pub struct Video {
    pub id: Uuid,
    pub posting_user_id: Uuid,
    pub title: String,
    pub description: String,
    pub video_length_seconds: i32,
    pub video_url: String,
}

impl<'c> FromRow<'c, PgRow> for Video {
    fn from_row(row: &PgRow) -> Result<Self, sqlx::Error> {
        Ok(Video {
            id: row.get(0),
            posting_user_id: row.get(1),
            title: row.get(2),
            description: row.get(3),
            video_length_seconds: row.get(4),
            video_url: row.get(5),
        })
    }
}
