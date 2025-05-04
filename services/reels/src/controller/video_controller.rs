use std::path::PathBuf;
use actix_multipart::form::{MultipartForm, json::Json as MpJson, tempfile::TempFile};
use actix_web::{HttpResponse, Responder, get, post, put, web};
use uuid::Uuid;

use crate::{
    AppState,
    model::{PostVideo, Video},
};

use super::log_request;

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(get_video);
    cfg.service(post_video);
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
        Ok(video) => HttpResponse::Ok().json(video),
    }
}

#[post("/video")]
async fn post_video(
    MultipartForm(form): MultipartForm<PostVideo>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);

    let mut video: Video = form.json.0;
    let temp_file: TempFile = form.file;

    let filename = temp_file
        .file_name
        .clone()
        .unwrap_or_else(|| "untitled".to_string());
    let extension = PathBuf::from(&filename)
        .extension()
        .and_then(std::ffi::OsStr::to_str)
        .map_or(String::new(), |ext| format!(".{}", ext));

    let unique_filename = format!("{}{}", video.id, extension);
    let dir = PathBuf::from("./upload");
    let path = dir.join(&unique_filename);

    let relative_path_str = path.to_string_lossy().to_string();
    video.video_url = relative_path_str.clone();

    let video = app_state.context.videos.post_video(&video).await;

    match video {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(video) => HttpResponse::Ok().json(video),
    }
}

#[put("/video/{id}")]
async fn put_video(
    video_uuid: web::Path<Uuid>,
    video: web::Json<Video>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);
    let video = app_state.context.videos.put_video_by_id(&video).await;

    match video {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(video) => HttpResponse::Ok().json(video),
    }
}
