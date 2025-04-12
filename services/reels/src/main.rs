//use std::net::TcpListener;
//use sqlx::{Connection, PgConnection};
//
//use reels_microservice::config::get_configuration;
//use reels_microservice::startup::run;
//
//
//#[actix_web::main]
//async fn main() -> std::io::Result<()> {
//    env_logger::init();
//    let configuration = get_configuration().expect("Failed to load configuration.");
//    let connection = PgConnection::connect(&configuration.database.connection_string())
//        .await
//        .expect("Failed to connect to Postgres.");
//
//    let adress = format!("{}:{}", configuration.app.url, configuration.app.port);
//    let listener = TcpListener::bind(adress)?;
//    run(listener, connection)?.await
//}


use std::net::TcpListener;
use std::sync::{Arc, Mutex};
use actix_web::{web, App, HttpServer};
use reels_microservice::{controller, AppState};
use reels_microservice::dao::database_context::Database;
use sqlx::{Connection, PgConnection};

use reels_microservice::config::get_configuration;
use reels_microservice::startup::run;


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

    println!("????/");

    let app = HttpServer::new(move || {
        App::new()
            .app_data(app_state.clone())
            .configure(controller::init_health_controller)
            .configure(controller::init_health_controller)
    })
    .bind(adress)?;
    
    app.run().await
}


