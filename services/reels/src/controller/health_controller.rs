use actix_web::{HttpResponse, Responder, get, web};

use crate::{AppState, controller::log_request};

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(health_check);
    cfg.service(state_check);
}

#[get("/health")]
async fn health_check(app_state: web::Data<AppState<'_>>) -> impl Responder {
    log_request("State check: ", &app_state.connections);
    HttpResponse::Ok().json(serde_json::json!({
        "status": "OK"
    }))
}