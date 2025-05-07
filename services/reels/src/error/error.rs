use actix_web::{ResponseError, HttpResponse};
use thiserror::Error;

#[derive(Debug, Error)]
pub enum AppError {
    #[error("Not found: {0}")]
    NotFound(String),
    #[error("Bad request: {0}")]
    BadRequest(String),
    #[error("Internal error: {0}")]
    InternalError(String),
}

impl ResponseError for AppError {
    fn error_response(&self) -> HttpResponse {
        match self {
            AppError::NotFound(msg) => HttpResponse::NotFound().body(msg.to_string()),
            AppError::BadRequest(msg) => HttpResponse::BadRequest().body(msg.to_string()),
            AppError::InternalError(msg) => HttpResponse::InternalServerError().body(msg.to_string()),
        }
    }
}