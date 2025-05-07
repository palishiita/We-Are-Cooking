mod reel;
mod reel_with_videos;
mod video;

pub type Reel = reel::reel::Reel;
pub type PostReel = reel::post_reel::PostReel;

pub type Video = video::video::Video;
pub type PostVideo = video::post_video::PostVideo;
pub type VideoForm = video::post_video::VideoForm;

pub type ReelWithVideosForm = reel_with_videos::reel_with_videos::ReelWithVideosForm;
pub type ReelWithVideos = reel_with_videos::reel_with_videos::ReelWithVideos;

mod health;
pub type HealthResponse = health::health_response::HealthResponse;
