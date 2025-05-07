use std::sync::Mutex;

use service::{reel_service::ReelService, video_service::VideoService};

pub mod config;
pub mod controller;
pub mod dao;
pub mod model;
pub mod openapi;
pub mod service;
pub mod util;
pub mod error;

pub struct AppState<'a> {
    pub connections: Mutex<u32>,
    pub reels_service: ReelService<'a>,
    pub video_service: VideoService<'a>,
}
