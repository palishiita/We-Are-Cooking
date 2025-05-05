use actix_web::web::Data;
use actix_web::{App, HttpServer, web};
use actix_files::Files;
use reels_microservice::config::{Settings, get_configuration};
use reels_microservice::dao::database_context::Database;
use reels_microservice::openapi::ApiDoc;
use reels_microservice::service::reel_service::{ReelRepository, ReelService};
use reels_microservice::service::video_service::{VideoRepository, VideoService};
use reels_microservice::{AppState, controller};
use std::sync::{Arc, Mutex};
use utoipa::OpenApi;
use utoipa_swagger_ui::SwaggerUi;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    env_logger::init();

    let configuration: Settings = get_configuration().expect("Failed to load configuration.");
    let adress = format!("{}:{}", configuration.app.url, configuration.app.port);

    let db_context: Arc<Database<'_>> =
        Arc::new(Database::new(&configuration.database.connection_string()).await);
    let reel_service: ReelService<'_> = ReelService::new(db_context.clone());
    let video_service: VideoService<'_> = VideoService::new(db_context);

    let app_state: Data<AppState<'_>> = web::Data::new(AppState {
        connections: Mutex::new(0),
        reels_service: reel_service,
        video_service: video_service,
    });

    let app = HttpServer::new(move || {
        App::new()
            .app_data(app_state.clone())
            .configure(controller::init_health_controller)
            .configure(controller::init_reel_controller)
            .configure(controller::init_video_controller)
            .service(Files::new("/static/videos", "./upload").show_files_listing())
            .service(
                SwaggerUi::new("/swagger-ui/{_:.*}")
                    .url("/api-docs/openapi.json", ApiDoc::openapi()),
            )
    })
    .bind(adress)?;

    app.run().await
}
