use config::File;

#[derive(serde::Deserialize)]
pub struct AppSettings {
    pub url: String,
    pub port: u16,
}

#[derive(serde::Deserialize)]
pub struct DatabaseSettings {
    username: String,
    password: String,
    database_name: String,
    port: u16,
    host: String,
}

#[derive(serde::Deserialize)]
pub struct Settings {
    pub app: AppSettings,
    pub database: DatabaseSettings,
}

// implement this function as settings method
pub fn get_configuration() -> Result<Settings, config::ConfigError> {
    let cf = config::Config::builder()
        .add_source(File::with_name("config"))
        .build()?;

    cf.try_deserialize()
}

impl DatabaseSettings {
    pub fn connection_string(&self) -> String {
        format!(
            "postgres://{}:{}@{}:{}/{}",
            self.username, self.password, self.host, self.port, self.database_name
        )
    }
}
//impl Config {
//    pub fn from_file(path: &'static str) -> Self {
//        let config = fs::read_to_string(path).unwrap();
//        serde_json::from_str(&config).unwrap()
//    }
//
//    pub fn get_app_url(&self) -> String {
//        format!("{0}:{1}", self.app.url, self.app.port)
//    }
//
//    pub fn get_database_url(&self) -> String {
//        todo!()
//    }
//}
