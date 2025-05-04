use actix_web::web;
use crate::{model::{Reel, PostReel}, AppState};

pub trait ReelRepository {
    fn get_reel(&self, reel: Reel);
    fn get_reels_paginated(&self, reels: Vec<Reel>);
    fn post_reel(&self, reel: PostReel);
    fn put_reel(&self, reel: PostReel);
    fn delete_reel(&self, reel_id: i64);
}

pub struct ReelService<'a> {
    pub app_state: web::Data<AppState<'a>>,
}

impl<'a> ReelRepository for ReelService<'a> {
    fn get_reel(&self, reel: Reel) {
        todo!()
    }

    fn get_reels_paginated(&self, reels: Vec<Reel>) {
        todo!()
    }

    fn post_reel(&self, reel: PostReel) {
        todo!()
    }

    fn put_reel(&self, reel: PostReel) {
        todo!()
    }

    fn delete_reel(&self, reel_id: i64) {
        todo!()
    }
}
