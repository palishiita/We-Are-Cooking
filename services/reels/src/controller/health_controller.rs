use crate::{AppState, controller::log_request, model::HealthResponse};
use actix_web::{HttpResponse, Responder, get, web};

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(health_check);
}

#[utoipa::path(
    get,
    path = "/health",
    responses(
        (status = 200, description = "Health check OK", body = HealthResponse)
    ),
    tag = "Health"
)]
#[get("/health")]
async fn health_check(app_state: web::Data<AppState<'_>>) -> impl Responder {
    log_request("State check: ", &app_state.connections);
    HttpResponse::Ok().json(HealthResponse { status: "OK" })
}
