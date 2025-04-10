use std::sync::{Arc, Mutex};

use dao::database_context::Database;

pub mod config;
pub mod dao;
pub mod startup;
pub mod controller;
pub mod model;

pub struct AppState<'a> {
    pub connections: Mutex<u32>,
    pub context: Arc<Database<'a>>,
}
