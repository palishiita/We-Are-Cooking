use std::net::TcpListener;

use actix_web::{dev::Server, web, App, HttpServer};
use sqlx::PgConnection;

pub fn run(
    listener: TcpListener,
    connection: PgConnection 
) -> Result<Server, std::io::Error> {
    let connection = web::Data::new(connection);
    let server = HttpServer::new(move || {
        App::new()
        .app_data(connection.clone())
    })
    .listen(listener)?
    .run();

    Ok(server)
}
