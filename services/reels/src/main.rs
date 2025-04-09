use std::net::TcpListener;
use sqlx::{Connection, PgConnection};

use reels_microservice::config::get_configuration;
use reels_microservice::startup::run;


#[actix_web::main]
async fn main() -> std::io::Result<()> {
    let configuration = get_configuration().expect("Failed to load configuration.");
    let connection = PgConnection::connect(&configuration.database.connection_string())
        .await
        .expect("Failed to connect to Postgres.");

    let adress = format!("{}:{}", configuration.app.url, configuration.app.port);
    let listener = TcpListener::bind(adress)?;
    run(listener, connection)?.await
}

