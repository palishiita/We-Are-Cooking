use uuid::Uuid;

use crate::model::{PostVideo, Video};

use super::database_context::Table;

impl<'c> Table<'c, Video> {
    pub async fn drop_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query("DROP TABLE IF EXISTS vidoes;")
            .execute(&*self.pool)
            .await
            .map(|_| ())
    }

    pub async fn create_table(&self) -> Result<(), sqlx::Error> {
        sqlx::query(
            r#"

            "#,
        )
        .execute(&*self.pool)
        .await
        .map(|_| ())
    }

    pub async fn get_video_by_id(&self, reel_id: &Uuid) -> Result<Video, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT *
                FROM 'videos'
                WHERE 'id' = ?
            "#,
        )
        .bind(reel_id)
        .fetch_one(&*self.pool)
        .await
    }

    pub async fn post_video(&self, video: &Video) -> Result<u64, sqlx::Error> {
        let _ = self.create_table().await;
        sqlx::query(
            r#"
                INSERT INTO videos (id, posting_user_id, title, description, video_length_seconds, video_url)
                VALUES($1, $2, $3, $4, $5, $6)
            "#
        )
            .bind(video.id)               
            .bind(video.posting_user_id)
            .bind(video.title.clone())
            .bind(video.description.clone())
            .bind(video.video_length_seconds)
            .bind(video.video_url.clone())
            .execute(&*self.pool) 
            .await
            .map(|x| x.rows_affected())
    }

    pub async fn put_video_by_id(&self, video: &Video) -> Result<u64, sqlx::Error> {
        let _ = self.create_table().await;
        sqlx::query(
            r#"
                INSERT INTO videos (id, posting_user_id, title, description, video_length_seconds, video_url)
                VALUES($1, $2, $3, $4, $5, $6)
            "#
        )   
            .bind(video.posting_user_id)
            .bind(&video.title)
            .bind(&video.description)
            .bind(video.video_length_seconds)
            .execute(&*self.pool) 
            .await
            .map(|x| x.rows_affected())
    }
}
