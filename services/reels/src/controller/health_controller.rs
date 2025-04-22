use actix_web::{get, web, HttpResponse, Responder};

use crate::{controller::log_request, AppState};

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(health_check);
    cfg.service(state_check);
}

#[get("/health")]
async fn health_check() -> impl Responder {
    HttpResponse::Ok().json(
        serde_json::json!({
            "status": "OK"
        })
    )
}

#[get("/health/app-state")]
async fn state_check(
    app_state: web::Data<AppState<'_>>,
) -> impl Responder {
    log_request("State check: ", &app_state.connections);
    HttpResponse::Ok().json(
        serde_json::json!({
            "status": "OK"
        })
    )
    
}
