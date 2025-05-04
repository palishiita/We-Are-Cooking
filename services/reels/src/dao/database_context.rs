use sqlx::postgres::PgRow;
use sqlx::{FromRow, PgPool};
use std::marker::PhantomData;
use std::sync::Arc;

use crate::model::{Reel, Video};

pub struct Database<'c> {
    pub reels: Arc<Table<'c, Reel>>,
    pub videos: Arc<Table<'c, Video>>,
}

impl<'a> Database<'a> {
    pub async fn new(pg_url: &str) -> Database<'a> {
        let conn = PgPool::connect(pg_url).await.unwrap();
        let pool = Arc::new(conn);

        Database {
            reels: Arc::from(Table::new(pool.clone())),
            videos: Arc::from(Table::new(pool.clone())),
        }
    }
}

pub struct Table<'c, T>
where
    T: FromRow<'c, PgRow>,
{
    pub pool: Arc<PgPool>,
    _from_row: fn(&'c PgRow) -> Result<T, sqlx::Error>,
    _marker: PhantomData<&'c T>,
}

impl<'c, T> Table<'c, T>
where
    T: FromRow<'c, PgRow>,
{
    fn new(pool: Arc<PgPool>) -> Self {
        Table {
            pool,
            _from_row: T::from_row,
            _marker: PhantomData,
        }
    }
}