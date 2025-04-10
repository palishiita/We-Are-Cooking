use actix_web::{get, web, HttpResponse, Responder};

pub fn init(cfg: &mut web::ServiceConfig) {
    cfg.service(health_check);
}

#[get("/health")]
async fn health_check() -> impl Responder {
    HttpResponse::Ok().json(
        serde_json::json!({
            "status": "OK"
        })
    )
}
