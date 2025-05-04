use actix_multipart::Multipart;
use actix_web::{get, post, put, web, HttpResponse, Responder};
use uuid::Uuid;

use crate::{model::Video, AppState};

use super::log_request;


pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(get_video);
    cfg.service(update_video);
}

#[get("/video/{id}")]
async fn get_video(
    video_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);
    let video = app_state.context.videos.get_video_by_id(&video_id).await;

    match video {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(video) => HttpResponse::Ok().json(video)
    }
}

#[post("/video")]
async fn post_video(
    mut payload: Multipart,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);
    let video = app_state.context.videos.post_video(&video_id).await;

    match video {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(video) => HttpResponse::Ok().json(video)
    }
}

#[put("/video/{id}")]
async fn put_video(
    video_id: web::Path<Video>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);
    let video = app_state.context.videos.put_video_by_id(&video_id).await;

    match video {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(video) => HttpResponse::Ok().json(video)
    }
}