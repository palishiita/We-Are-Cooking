use sqlx::postgres::PgRow;
use sqlx::{FromRow, Row};

#[derive(serde::Serialize, serde::Deserialize, Clone)]
pub struct Reel {
    pub id: String,
    pub path: String,
    pub created_at: String,
}

impl<'c> FromRow<'c, PgRow> for Reel {
    fn from_row(row: &PgRow) -> Result<Self, sqlx::Error> {
        Ok(Reel {
            id: row.get(0),
            path: row.get(1),
            created_at: row.get(2),
        })
    }
}
