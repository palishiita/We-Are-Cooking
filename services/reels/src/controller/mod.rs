use std::sync::Mutex;

pub mod reel_controller;
pub use reel_controller::init as init_reel_controller;

pub mod health_controller;
pub use health_controller::init as init_health_controller;

fn log_request(route: &'static str, connections: &Mutex<u32>) {
    println!("Logging request");
    let mut con = connections.lock().unwrap();
    *con += 1;
    println!("{}\n\tconnections: {}", route, con);
}
