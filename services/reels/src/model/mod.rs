mod reel;
mod video;

pub type Reel = reel::reel::Reel;
pub type Video = video::video::Video;
pub type PostVideo = video::post_video::PostVideo;

mod health;
pub type HealthResponse = health::health_response::HealthResponse;
