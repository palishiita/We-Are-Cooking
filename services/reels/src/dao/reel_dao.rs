use sqlx::types::chrono::Utc;
use uuid::Uuid;

use crate::model::Reel;

use super::database_context::Table;

impl<'c> Table<'c, Reel> {
    pub async fn drop_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query("DROP TABLE IF EXISTS reels;")
            .execute(&*self.pool)
            .await
            .map(|_|())
    }

    pub async fn create_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query(
            r#"

            "#,
        )
            .execute(&*self.pool)
            .await
            .map(|_|())
    }

    pub async fn get_reel_by_id(&self, reel_id: &Uuid) -> Result<Reel, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT *
                FROM reels
                WHERE id = ?
            "#,
        )
        .bind(reel_id)
        .fetch_one(&*self.pool)
        .await
    }

    pub async fn post_reel(&self, reel: &Reel) -> Result<u64, sqlx::Error> {
        let _ = self.create_table().await;
        sqlx::query(
            r#"
                INSERT INTO reels (id, video_id, posting_user_id, title, description, creation_timestamp)
                VALUES($1, $2, $3, $4, $5, $6)
            "#
        )
            .bind(reel.id)               
            .bind(reel.video_id)         
            .bind(reel.posting_user_id)
            .bind(reel.title.clone())
            .bind(reel.description.clone())
            .bind(Utc::now().naive_utc())
            .execute(&*self.pool) 
            .await
            .map(|x| x.rows_affected())
    }
}
