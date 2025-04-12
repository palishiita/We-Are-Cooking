use crate::model::Video;

use super::database_context::Table;



impl<'c> Table<'c, Video> {
    pub async fn drop_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query("DROP TABLE IF EXISTS vidoes;")
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

    pub async fn get_reel_by_id(&self, reel_id: &str) -> Result<Video, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT *
                FROM 'reels'
                WHERE 'id' = ?
            "#,
        )
        .bind(reel_id)
        .fetch_one(&*self.pool)
        .await
    }
}
