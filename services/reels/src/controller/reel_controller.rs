use actix_web::{get, post, web, HttpResponse, Responder};
use uuid::Uuid;
use crate::{model::Reel, AppState};

use super::log_request;

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(get_reel);
    cfg.service(post_reel);
    //cfg.service(get_reels);
    //cfg.service(delete_reel);
    //cfg.service(update_reel);
}

#[get("/reel/{id}")]
async fn get_reel(
    reel_id: web::Path<Uuid>,
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("Get: /reel", &app_state.connections);
    let reel = app_state.context.reels.get_reel_by_id(&reel_id).await;

    match reel {
        Err(_) => HttpResponse::NotFound().finish(),
        Ok(reel) => HttpResponse::Ok().json(reel)
    }
}

#[post("/reel/post")]
async fn post_reel(
    reel: web::Json<Reel>,
    app_state: web::Data<AppState<'_>>
) -> impl Responder {
    log_request("Post: /reel", &app_state.connections);
 
    let mut reel = reel.into_inner();
    reel.id = Uuid::new_v4();

    let x = app_state.context.reels.post_reel(&reel).await;

    match x {
        Ok(_) => {
            HttpResponse::Created().body(reel.id.to_string())
        }
        Err(_) => HttpResponse::InternalServerError().finish()
    }
}

async fn get_reels() {}

async fn delete_reel() {}

async fn update_reel() {}


