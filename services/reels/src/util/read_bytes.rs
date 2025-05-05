use actix_multipart::Field;
use bytes::BytesMut;
use futures_util::StreamExt;

use crate::error::error::AppError;

pub async fn read_bytes(field: &mut Field) -> Result<BytesMut, AppError> {
    let mut bytes = BytesMut::new();
    while let Some(chunk) = field.next().await {
        let chunk = chunk.map_err(|_| AppError::InternalError("Error reading multipart chunk".into()))?;
        bytes.extend_from_slice(&chunk);
    }
    Ok(bytes)
}
