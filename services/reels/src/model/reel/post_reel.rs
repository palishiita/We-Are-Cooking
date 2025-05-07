use utoipa::ToSchema;
use uuid::Uuid;

#[derive(serde::Serialize, serde::Deserialize, Clone, ToSchema, Debug)]
pub struct PostReel {
    #[schema(example = "222e8400-e29b-41d4-a716-446655440000")]
    pub posting_user_id: Uuid,

    #[schema(example = "Amazing New Video")]
    pub title: String,

    #[schema(example = "This video shows the best moments.")]
    pub description: String,
}
