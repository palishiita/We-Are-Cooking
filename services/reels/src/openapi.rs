use utoipa::OpenApi;

use crate::model::HealthResponse;
use crate::controller;

#[derive(OpenApi)]
#[openapi(paths(controller::health_controller::health_check), components(schemas(HealthResponse)))]
pub struct ApiDoc;
