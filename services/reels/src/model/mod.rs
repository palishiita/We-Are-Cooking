mod reel;
mod video;

pub type Reel = reel::reel::Reel;
pub type PostReel = reel::post_reel::PostReel;

pub type Video = video::video::Video;
pub type PostVideo = video::post_video::PostVideo;
pub type VideoForm = video::post_video::VideoForm;

mod health;
pub type HealthResponse = health::health_response::HealthResponse;
