use utoipa::ToSchema;
use uuid::Uuid;

#[derive(serde::Serialize, serde::Deserialize, Clone, ToSchema, Debug)]
pub struct PostReel {
    #[schema(example = "Amazing New Video")]
    pub title: String,

    #[schema(example = "This video shows the best moments.")]
    pub description: String,
}
