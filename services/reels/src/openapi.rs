use utoipa::OpenApi;

use crate::controller;
use crate::model::{
    HealthResponse, PostReel, PostVideo, Reel, ReelWithVideos, ReelWithVideosForm, Video, VideoForm,
};

#[derive(OpenApi)]
#[openapi(
    paths(
        controller::health_controller::health_check,
        controller::reel_controller::get_reel_by_id,
        controller::reel_controller::get_reels_paginated,
        controller::reel_controller::get_reels_with_videos_paginated,
        controller::reel_controller::get_reels_by_user_id,
        controller::reel_controller::post_reel,
        controller::reel_controller::post_reel_with_video,
        controller::reel_controller::delete_reel_with_video,
        controller::video_controller::get_video_by_id,
        controller::video_controller::get_video_by_reel_id,
        controller::video_controller::post_video,
        controller::video_controller::put_video,
        controller::video_controller::delete_video,
    ),
    components(schemas(
        HealthResponse,
        Reel,
        PostReel,
        Video,
        PostVideo,
        VideoForm,
        ReelWithVideos,
        ReelWithVideosForm
    ))
)]
pub struct ApiDoc;
