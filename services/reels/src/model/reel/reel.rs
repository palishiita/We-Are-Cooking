use sqlx::postgres::PgRow;
use sqlx::{FromRow, Row};
use uuid::Uuid;

#[derive(serde::Serialize, serde::Deserialize, Clone)]
pub struct Reel {
    #[serde(skip_deserializing)]
    pub id: Uuid,
    pub video_id: Uuid,
    pub posting_user_id: Uuid,
    pub title: String,
    pub description: String,
    pub creation_timestamp: String,
}

impl<'c> FromRow<'c, PgRow> for Reel {
    fn from_row(row: &PgRow) -> Result<Self, sqlx::Error> {
        Ok(Reel {
            id: row.get(0),
            video_id: row.get(1),
            posting_user_id: row.get(2),
            title: row.get(3),
            description: row.get(4),
            creation_timestamp: row.get(5),
        })
    }
}
