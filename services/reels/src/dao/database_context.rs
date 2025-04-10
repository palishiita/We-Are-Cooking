use sqlx::postgres::PgRow;
use sqlx::{FromRow, PgPool};
use std::sync::Arc;
use std::marker::PhantomData;

use crate::model::Reel;

pub struct Database<'c> {
    pub reels: Arc<Table<'c, Reel>>,
}

impl<'a> Database<'a> {
    pub async fn new(pg_url: &String) -> Database<'a> {
        let conn = PgPool::connect(&pg_url).await.unwrap();
        let pool = Arc::new(conn);

        Database {
            reels: Arc::from(Table::new(pool.clone())),
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
