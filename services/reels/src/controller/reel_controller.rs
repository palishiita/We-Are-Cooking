use std::collections::HashMap;

use crate::{
    error::error::AppError, model::{PostReel, PostVideo, Reel, ReelWithVideosForm}, service::{reel_service::ReelRepository, video_service::VideoRepository}, util::read_bytes::read_bytes, AppState
};
use actix_multipart::{Field, Multipart};
use actix_web::{delete, get, post, put, web, HttpResponse, Responder, HttpRequest};
use bytes::BytesMut;
use serde_json::from_slice;
use uuid::Uuid;
use futures_util::StreamExt as _;

use super::log_request;

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(get_reel_by_id);
    cfg.service(get_reels_paginated);
    cfg.service(get_reels_with_videos_paginated);
    cfg.service(get_reels_by_user_id);
    cfg.service(post_reel);
    cfg.service(post_reel_with_video);
    cfg.service(put_reel);
    cfg.service(delete_reel_with_video);
}

#[utoipa::path(
    get,
    path = "/reel/{id}",
    params(
        ("id" = Uuid, Path, description = "Reel UUID")
    ),
    responses(
        (status = 200, description = "Reel found", body = Reel),
        (status = 404, description = "Reel not found")
    ),
    tag = "Reels"
)]
#[get("/reel/{id}")]
async fn get_reel_by_id(
    reel_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    let reel =  app_state
        .reels_service
        .get_reel_by_id(reel_id.into_inner())
        .await?;

    Ok(HttpResponse::Ok().json(reel))
}

#[utoipa::path(
    get,
    path = "/reel",
    params(
        ("page" = Option<u32>, Query, description = "Page number (default: 1)"),
        ("limit" = Option<u32>, Query, description = "Items per page (default: 10)")
    ),
    responses(
        (status = 200, description = "List of paginated reels", body = [Reel]),
        (status = 500, description = "Internal server error")
    ),
    tag = "Reels"
)]
#[get("/reel")]
async fn get_reels_paginated(
    web::Query(params): web::Query<HashMap<String, String>>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Get: /reel", &app_state.connections);

    let page = params
        .get("page")
        .and_then(|s| s.parse::<u32>().ok())
        .unwrap_or(1);
    let limit = params
        .get("limit")
        .and_then(|s| s.parse::<u32>().ok())
        .unwrap_or(10);

    let reels = app_state
        .reels_service
        .get_reels_paginated(page, limit)
        .await?;
    
    Ok(HttpResponse::Ok().json(reels))
}

#[utoipa::path(
    get,
    path = "/reel-videos",
    params(
        ("page" = Option<u32>, Query, description = "Page number (default: 1)"),
        ("limit" = Option<u32>, Query, description = "Items per page (default: 10)")
    ),
    responses(
        (status = 200, description = "List of paginated reels with videos"),
        (status = 500, description = "Internal server error"),
    ),
    tag = "Reels"
)]
#[get("/reel-videos")]
async fn get_reels_with_videos_paginated(
    web::Query(params): web::Query<HashMap<String, String>>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Get: /reel/videos", &app_state.connections);

    let page = params
        .get("page")
        .and_then(|s| s.parse::<u32>().ok())
        .unwrap_or(1);
    let limit = params
        .get("limit")
        .and_then(|s| s.parse::<u32>().ok())
        .unwrap_or(10);

    let reels_with_videos = app_state
        .reels_service
        .get_reels_with_videos_paginated(page, limit)
        .await?;
    
    Ok(HttpResponse::Ok().json(reels_with_videos))
}

#[utoipa::path(
    post,
    path = "/reel",
    request_body = PostReel,
    responses(
        (status = 201, description = "Reel created successfully"),
        (status = 500, description = "Internal server error")
    ),
    tag = "Reels"
)]
#[post("/reel")]
async fn post_reel(
    reel: web::Json<PostReel>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Post: /reel", &app_state.connections);

    match app_state.reels_service.post_reel(reel.into_inner(), None).await {
        Ok(_) => HttpResponse::Created().finish(),
        Err(_) => HttpResponse::InternalServerError().finish(),
    }
}

#[utoipa::path(
    post,
    path = "/reel-video",
    request_body(
        content = ReelWithVideosForm,
        content_type = "multipart/form-data"
    ),
    responses(
        (status = 200, description = "Video uploaded successfully"),
        (status = 400, description = "Invalid input")
    ),
    description = r#"
    "#,
    tag = "Reels"
)]
#[post("/reel-video")]
async fn post_reel_with_video(
    mut payload: Multipart,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Post: /reel-video", &app_state.connections);

    let mut video_metadata: Option<PostVideo> = None;
    let mut reel_metadata: Option<PostReel> = None;
    let mut video_data: Option<BytesMut> = None;
    let mut file_name: Option<String> = None;

    while let Some(item) = payload.next().await {
        let mut field: Field = item
            .map_err(|_| AppError::BadRequest("Invalid multipart file".into()))?;

        let disposition = field.content_disposition();
        let name = disposition.and_then(|d| d.get_name()).unwrap_or("");

        match name {
            "file" => {
                file_name = disposition.and_then(|d| d.get_filename().map(str::to_string));
                video_data = Some(read_bytes(&mut field).await?);
            }
            "video" => {
                let json_bytes = read_bytes(&mut field).await?;
                video_metadata = Some(
                    from_slice(&json_bytes)
                        .map_err(|_| AppError::BadRequest("Invalid JSON in 'video'".into()))?,
                );
            }
            "reel" => {
                let json_bytes = read_bytes(&mut field).await?;
                reel_metadata = Some(
                    from_slice(&json_bytes)
                        .map_err(|_| AppError::BadRequest("Invalid JSON in 'reel'".into()))?,
                );
            }
            _ => return Err(AppError::BadRequest("Invalid multipart file".into()))
        }
    }

    let video_metadata = video_metadata
        .ok_or_else(|| AppError::BadRequest("Missing video metadata".into()))?;
    let reel_metadata = reel_metadata
        .ok_or_else(|| AppError::BadRequest("Missing reel metadata".into()))?;
    let video_data = video_data
        .ok_or_else(|| AppError::BadRequest("Missing file field".into()))?;
    let file_name = file_name
        .ok_or_else(|| AppError::BadRequest("Missing file name".into()))?;

    let video_id = app_state
        .video_service
        .post_video(video_metadata, video_data, file_name)
        .await?;

    app_state
        .reels_service
        .post_reel(reel_metadata, Some(video_id))
        .await?;
    
    Ok(HttpResponse::Ok().finish())
}

#[put("/reel/{id}")]
async fn put_reel(reel_id: web::Path<Uuid>, app_state: web::Data<AppState<'_>>) -> impl Responder {
    log_request("Put: /reel/{id}", &app_state.connections);
    HttpResponse::NotImplemented()
}

#[utoipa::path(
    delete,
    path = "/reel/{id}",
    responses(
        (status = 200, description = "Reel and video successfully deleted", body = String),
        (status = 404, description = "Reel not found"),
        (status = 500, description = "Internal Server Error")
    ),
    params(
        ("id" = Uuid, Path, description = "The unique ID of the reel to be deleted")
    ),
    tag="Reels"
)]
#[delete("/reel/{id}")]
async fn delete_reel_with_video(
    reel_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Delete: /reel", &app_state.connections);

    let reel = app_state
        .reels_service
        .delete_reel_with_video(reel_id.into_inner())
        .await?;

    Ok(HttpResponse::Ok().json(reel))
}

#[utoipa::path(
    get,
    path = "/user/reels",
    params(
        ("page" = Option<u32>, Query, description = "Page number (default: 1)"),
        ("limit" = Option<u32>, Query, description = "Items per page (default: 10)")
    ),
    responses(
        (status = 200, description = "List of reels for the user", body = [Reel]),
        (status = 400, description = "Missing user ID in header"),
        (status = 500, description = "Internal server error")
    ),
    tag = "Reels"
)]
#[get("/user/reels")]
async fn get_reels_by_user_id(
    req: HttpRequest,
    web::Query(params): web::Query<HashMap<String, String>>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Get: /reel/user", &app_state.connections);

    // Pobierz UUID użytkownika z nagłówka X-Uuid
    let user_uuid = req
        .headers()
        .get("x-uuid")
        .and_then(|h| h.to_str().ok())
        .ok_or_else(|| AppError::BadRequest("Missing X-Uuid header".into()))?;

    let page = params
        .get("page")
        .and_then(|s| s.parse::<u32>().ok())
        .unwrap_or(1);
    let limit = params
        .get("limit")
        .and_then(|s| s.parse::<u32>().ok())
        .unwrap_or(10);

    let user_id = user_uuid.parse::<Uuid>()
        .map_err(|_| AppError::BadRequest("Invalid UUID format in X-Uuid header".into()))?;

    let reels = app_state
        .reels_service
        .get_reels_by_user_id(user_id, page, limit)
        .await?;
    
    Ok(HttpResponse::Ok().json(reels))
}