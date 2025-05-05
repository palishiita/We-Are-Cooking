use chrono::NaiveDateTime;
use sqlx::postgres::PgRow;
use sqlx::{FromRow, Row};
use utoipa::{ToSchema, schema};
use uuid::Uuid;

#[derive(serde::Serialize, serde::Deserialize, Clone, ToSchema)]
pub struct Reel {
    #[serde(skip_deserializing)]
    #[schema(example = "550e8400-e29b-41d4-a716-446655440000")]
    pub id: Uuid,

    #[schema(example = "111e8400-e29b-41d4-a716-446655440000")]
    pub video_id: Uuid,

    #[schema(example = "222e8400-e29b-41d4-a716-446655440000")]
    pub posting_user_id: Uuid,

    #[schema(example = "Funny Cat Compilation")]
    pub title: String,

    #[schema(example = "A compilation of the funniest cat videos.")]
    pub description: String,

    #[schema(example = "2024-05-04T12:34:56")]
    pub creation_timestamp: NaiveDateTime,
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
