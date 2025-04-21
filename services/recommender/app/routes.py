# app/routes.py

from fastapi import APIRouter
from app.models import RecommendationRequest, RecommendationResponse
from app.recommender_fridge import recommend_from_fridge
from app.recommender_personal import recommend_from_ratings
from app.recommender_hybrid import recommend_hybrid
from app.recommender_strategy import get_recommendations

router = APIRouter(prefix="/recommend", tags=["Recommendation"])

@router.post("/fridge", response_model=RecommendationResponse)
def fridge_route(req: RecommendationRequest):
    return recommend_from_fridge(req.user_id)

@router.post("/personal", response_model=RecommendationResponse)
def personal_route(req: RecommendationRequest):
    return recommend_from_ratings(req.user_id)

@router.post("/hybrid", response_model=RecommendationResponse)
def hybrid_route(req: RecommendationRequest):
    return recommend_hybrid(req.user_id)

@router.post("/auto", response_model=RecommendationResponse)
def auto_route(req: RecommendationRequest):
    return get_recommendations(req.user_id)