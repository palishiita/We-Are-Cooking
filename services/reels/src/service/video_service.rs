use actix_web::web;
use crate::{model::{Video, PostVideo}, AppState};

pub trait VideoRepository {
    fn get_reel(&self, reel: Video);
    fn get_reels_paginated(&self, reels: Vec<Video>);
    fn post_reel(&self, reel: PostVideo);
    fn put_reel(&self, reel: PostVideo);
    fn delete_reel(&self, reel_id: i64);
}

pub struct VideoService<'a> {
    pub app_state: web::Data<AppState<'a>>,
}

impl<'a> VideoRepository for VideoService<'a> {
    fn get_reel(&self, reel: Video) {
        todo!()
    }

    fn get_reels_paginated(&self, reels: Vec<Video>) {
        todo!()
    }

    fn post_reel(&self, reel: PostVideo) {
        todo!()
    }

    fn put_reel(&self, reel: PostVideo) {
        todo!()
    }

    fn delete_reel(&self, reel_id: i64) {
        todo!()
    }
}
