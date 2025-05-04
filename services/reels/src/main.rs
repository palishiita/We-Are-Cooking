use actix_web::{App, HttpServer, web};
use reels_microservice::dao::database_context::Database;
use reels_microservice::openapi::ApiDoc;
use reels_microservice::service::reel_service::ReelService;
use reels_microservice::{AppState, controller};
use utoipa_swagger_ui::SwaggerUi;
use std::sync::{Arc, Mutex};
use utoipa::OpenApi;
use reels_microservice::config::get_configuration;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    env_logger::init();

    let configuration = get_configuration().expect("Failed to load configuration.");
    let db_context = Database::new(&configuration.database.connection_string()).await;
    let adress = format!("{}:{}", configuration.app.url, configuration.app.port);

    let app_state = web::Data::new(AppState {
        connections: Mutex::new(0),
        context: Arc::new(db_context),
    });

    let app = HttpServer::new(move || {
        App::new()
            .service(
                SwaggerUi::new("/swagger-ui/{_:.*}")
                    .url("/api-docs/openapi.json", ApiDoc::openapi()),
            )
            .app_data(app_state.clone())
            .configure(controller::init_health_controller)
            .configure(controller::init_reel_controller)
            .configure(controller::init_video_controller)
    })
    .bind(adress)?;

    app.run().await
}
