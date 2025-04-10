use actix_web::{get, web, HttpResponse, Responder};

use crate::AppState;

use super::log_request;

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(get_reel);
    //cfg.service(post_reel);
    //cfg.service(get_reels);
    //cfg.service(delete_reel);
    //cfg.service(update_reel);
}

#[get("/reel/{id}")]
async fn get_reel(
    reel_id: web::Path<String>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);

    let reel = app_state.context.reels.get_reel_by_id(&reel_id).await;

    match reel {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(reel) => HttpResponse::Ok().json(reel)
    }
}

async fn post_reel() {}

async fn get_reels() {}

async fn delete_reel() {}

async fn update_reel() {}


