# app/models.py

from pydantic import BaseModel
from typing import List, Optional

class RecommendationRequest(BaseModel):
    user_id: str

class Recipe(BaseModel):
    id: str
    name: str
    score: float
    missing_ingredients: Optional[List[str]] = []

class RecommendationResponse(BaseModel):
    recommendations: List[Recipe]