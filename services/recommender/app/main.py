import os
import pandas as pd
from recommendation_system.fridge import FridgeBasedRecommender
from recommendation_system.content import ContentBasedRecommender
from recommendation_system.collaborative import CollaborativeRecommender
from recommendation_system.popularity import PopularityRecommender
from recommendation_system.hybrid import HybridRecommender
from fastapi import FastAPI
from pydantic import BaseModel

# Set relative path inside container
base_path = 'data'

def load_csv(file_name):
    return pd.read_csv(os.path.join(base_path, file_name), encoding='latin1')

# Load datasets
users_df = load_csv('users.csv')
recipes_df = load_csv('recipes.csv')
ingredients_df = load_csv('ingredients.csv')
recipe_ingredients_df = load_csv('recipe_ingredients.csv')
reviews_df = load_csv('reviews.csv')
user_fridge_df = load_csv('user_fridge_ingredients.csv')
cookbook_df = load_csv('user_cookbook_recipes.csv')

# Ingredient importance fallback
ingredient_info_df = pd.DataFrame({
    'ingredient_id': ingredients_df['id'],
    'importance': 1
})

# Initialize recommenders
fridge_rec = FridgeBasedRecommender(user_fridge_df, recipe_ingredients_df, ingredient_info_df)
content_rec = ContentBasedRecommender(recipe_ingredients_df, reviews_df)
collab_rec = CollaborativeRecommender(reviews_df, cookbook_df)
pop_rec = PopularityRecommender(reviews_df, cookbook_df)
hybrid_rec = HybridRecommender({'content': content_rec, 'collaborative': collab_rec, 'popularity': pop_rec})

# Recipe ID to title map
recipe_id_to_title = recipes_df.set_index('id')['name'].to_dict()

def map_ids_to_titles(recipe_ids):
    return [recipe_id_to_title.get(rid, f"(missing title: {rid})") for rid in recipe_ids]

# FastAPI app
app = FastAPI()

class RecommendationRequest(BaseModel):
    user_id: str
    top_n: int = 5

@app.get("/")
def read_root():
    return {"message": "Welcome to the We-Are-Cooking Recommendation API. Visit /docs to test endpoints."}

@app.get("/health")
def health_check():
    return {"status": "ok"}

@app.get("/recipes")
def get_all_recipes():
    return recipes_df.to_dict(orient='records')

@app.get("/reviews")
def get_all_reviews():
    return reviews_df.to_dict(orient='records')

@app.post("/recommend/content")
def recommend_content(req: RecommendationRequest):
    ids = content_rec.recommend(req.user_id, req.top_n)
    return {"recipes": map_ids_to_titles(ids)}

@app.post("/recommend/collaborative")
def recommend_collaborative(req: RecommendationRequest):
    ids = collab_rec.recommend(req.user_id, req.top_n)
    return {"recipes": map_ids_to_titles(ids)}

@app.post("/recommend/popularity")
def recommend_popularity(req: RecommendationRequest):
    ids = pop_rec.recommend(req.top_n)
    return {"recipes": map_ids_to_titles(ids)}

@app.post("/recommend/hybrid")
def recommend_hybrid(req: RecommendationRequest):
    ids = hybrid_rec.recommend(req.user_id, req.top_n)
    return {"recipes": map_ids_to_titles(ids)}

@app.post("/recommend/fridge")
def recommend_fridge(req: RecommendationRequest):
    ids = fridge_rec.recommend(req.user_id, req.top_n)
    return {"recipes": map_ids_to_titles(ids)}