use uuid::Uuid;

use crate::model::Video;

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

    pub async fn get_video_by_id(&self, video_id: Uuid) -> Result<Video, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT *
                FROM videos
                WHERE id = $1
            "#,
        )
        .bind(video_id)
        .fetch_one(&*self.pool)
        .await
    }

    pub async fn get_video_by_reel_id(&self, reel_id: Uuid) -> Result<Video, sqlx::Error> {
        sqlx::query_as(
            r#"
                SELECT v.*
                FROM reels r
                JOIN videos v ON v.id = r.video_id
                WHERE r.id = $1
            "#,
        )
        .bind(reel_id)
        .fetch_one(&*self.pool)
        .await
    }

    pub async fn post_video(&self, video: &Video) -> Result<Uuid, sqlx::Error> {
        let _ = self.create_table().await;
    
        let row: (Uuid,) = sqlx::query_as(
            r#"
                INSERT INTO videos (id, posting_user_id, title, description, video_length_seconds, video_url)
                VALUES($1, $2, $3, $4, $5, $6)
                RETURNING id
            "#
        )
        .bind(video.id)
        .bind(video.posting_user_id)
        .bind(video.title.clone())
        .bind(video.description.clone())
        .bind(video.video_length_seconds)
        .bind(video.video_url.clone())
        .fetch_one(&*self.pool)
        .await?;
    
        Ok(row.0)
    }

    pub async fn put_video(&self, video: Video) -> Result<u64, sqlx::Error> {
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

    pub async fn delete_video(&self, video_id: Uuid) -> Result<Option<String>, sqlx::Error> {
        let mut tx = self.pool.begin().await?;

        let video_url: Option<(String,)> =
            sqlx::query_as::<_, (String,)>("SELECT video_url FROM videos WHERE id = $1")
                .bind(video_id)
                .fetch_optional(&mut *tx)
                .await?;

        if video_url.is_some() {
            sqlx::query("DELETE FROM videos WHERE id = $1")
                .bind(video_id)
                .execute(&mut *tx)
                .await?;
        }

        tx.commit().await?;
        Ok(video_url.map(|v| v.0))
        // let _ = self.create_table().await;
        // sqlx::query(
        //     r#"
        //         DELETE FROM videos
        //         WHERE id = $1
        //     "#
        // )
        //     .bind(video_id)
        //     .execute(&*self.pool)
        //     .await
        //     .map(|x| x.rows_affected())
    }
}
