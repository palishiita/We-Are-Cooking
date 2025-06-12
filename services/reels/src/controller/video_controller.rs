use actix_multipart::{Field, Multipart};
use actix_web::{delete, get, post, put, web::{self, BytesMut}, HttpResponse, Responder, HttpRequest};
use serde_json::from_slice;
use uuid::Uuid;
use futures_util::StreamExt as _;

use crate::{
    error::error::AppError, model::{PostVideo, Video, VideoForm}, service::video_service::VideoRepository, util::read_bytes::read_bytes, AppState
};

use super::log_request;

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(get_video_by_id);
    cfg.service(get_video_by_reel_id);
    cfg.service(post_video);
    cfg.service(put_video);
    cfg.service(delete_video);
}

#[utoipa::path(
    get,
    path = "/video/{id}",
    params(
        ("id" = Uuid, Path, description = "ID of the video to fetch")
    ),
    responses(
        (status = 200, description = "Video fetched successfully", body = Video),
        (status = 404, description = "Video not found")
    ),
    tag = "Video"
)]
#[get("/video/{id}")]
async fn get_video_by_id(
    video_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Get: /video/{id}", &app_state.connections);

    let video = app_state
        .video_service
        .get_video_by_id(video_id.into_inner())
        .await?;

    
    Ok(HttpResponse::Ok().json(video))
}

#[utoipa::path(
    get,
    path = "/video/reel/{id}",
    params(
        ("id" = Uuid, Path, description = "ID of the reel to fetch associated video")
    ),
    responses(
        (status = 200, description = "Video fetched successfully", body = Video),
        (status = 404, description = "Reel or Video not found")
    ),
    tag = "Video"
)]
#[get("/video/reel/{id}")]
async fn get_video_by_reel_id(
    reel_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Get: /video/reel/{id}", &app_state.connections);

    let video = app_state
        .video_service
        .get_video_by_reel_id(reel_id.into_inner())
        .await?;

    Ok(HttpResponse::Ok().json(video))
}

#[utoipa::path(
    post,
    path = "/video",
    request_body(
        content = VideoForm,
        content_type = "multipart/form-data"
    ),
    responses(
        (status = 200, description = "Video uploaded successfully"),
        (status = 400, description = "Invalid input")
    ),    description = r#"
        KNOWN utoipa ERROR, CURL WON'T GENERATE PROPERLY
        Example cURL for uploading a video:

        curl -X 'POST' \
        'http://127.0.0.1:8000/video' \
        -H 'accept: */*' \
        -H 'Content-Type: multipart/form-data' \
        -H 'x-uuid: 3fa85f64-5717-4562-b3fc-2c963f66afa6' \
        -F 'file=@epico.mp4;type=video/mp4' \
        -F 'video={"description":"string","title":"string","video_length_seconds":1073741824};type=application/json'

    "#,
    tag = "Video"
)]
#[post("/video")]
async fn post_video(
    req: HttpRequest,
    mut payload: Multipart,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Post: /video", &app_state.connections);

    let posting_user_id = req
        .headers()
        .get("x-uuid")
        .and_then(|h| h.to_str().ok())
        .ok_or_else(|| AppError::BadRequest("Missing x-uuid header".into()))?
        .parse::<Uuid>()
        .map_err(|_| AppError::BadRequest("Invalid UUID format in x-uuid header".into()))?;

    let mut video_metadata: Option<PostVideo> = None;
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
            _ => return Err(AppError::BadRequest("Invalid multipart file".into()))
        }
    }

    let video_metadata = video_metadata
        .ok_or_else(|| AppError::BadRequest("Missing video metadata".into()))?;
    let video_data = video_data
        .ok_or_else(|| AppError::BadRequest("Missing file field".into()))?;
    let file_name = file_name
        .ok_or_else(|| AppError::BadRequest("Missing file name".into()))?;    app_state
        .video_service
        .post_video(video_metadata, posting_user_id, video_data, file_name)
        .await?;
    
    Ok(HttpResponse::Ok().finish())
}

#[utoipa::path(
    put,
    path = "/video/{id}",
    params(
        ("id" = Uuid, Path, description = "ID of the video to update")
    ),
    request_body(
        content = PostVideo,
        content_type = "application/json"
    ),
    responses(
        (status = 200, description = "Video updated successfully", body = Video),
        (status = 404, description = "Video not found"),
        (status = 400, description = "Invalid input")
    ),
    tag = "Video"
)]
#[put("/video/{id}")]
async fn put_video(
    req: HttpRequest,
    video_id: web::Path<Uuid>,
    video: web::Json<PostVideo>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Put: /reel", &app_state.connections);

    let posting_user_id = req
        .headers()
        .get("x-uuid")
        .and_then(|h| h.to_str().ok())
        .ok_or_else(|| AppError::BadRequest("Missing x-uuid header".into()))?
        .parse::<Uuid>()
        .map_err(|_| AppError::BadRequest("Invalid UUID format in x-uuid header".into()))?;

    let video =  app_state
        .video_service
        .put_video(video_id.into_inner(), video.into_inner(), posting_user_id)
        .await?;

    Ok(HttpResponse::Ok().json(video))
}

#[utoipa::path(
    delete,
    path = "/video/{id}",
    params(
        ("id" = Uuid, Path, description = "ID of the video to delete")
    ),
    responses(
        (status = 200, description = "Video deleted successfully", body = String),
        (status = 404, description = "Video not found")
    ),
    tag = "Video"
)]
#[delete("/video/{id}")]
async fn delete_video(
    video_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> Result<impl Responder, AppError> {
    log_request("Delete: /video", &app_state.connections);

    let video = app_state
        .video_service
        .delete_video(video_id.into_inner())
        .await?;

    Ok(HttpResponse::Ok().json(video))
}
