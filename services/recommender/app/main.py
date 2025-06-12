import os
import time
import pandas as pd
import psycopg2
import logging
from psycopg2 import OperationalError
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel

# Recommendation imports
from recommendation_system.fridge import FridgeBasedRecommender
from recommendation_system.content import ContentBasedRecommender
from recommendation_system.collaborative import CollaborativeRecommender
from recommendation_system.popularity import PopularityRecommender
from recommendation_system.hybrid import HybridRecommender

# Setup logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Database configuration
DB_CONFIG = {
    "dbname": "wrc",
    "user": "admin",
    "password": "admin",
    "host": "wrc-db",
    "port": 5432
}

def load_table(table_name, retries=30, delay=10):
    for attempt in range(retries):
        try:
            conn = psycopg2.connect(**DB_CONFIG)
            df = pd.read_sql_query(f"SELECT * FROM {table_name}", conn)
            conn.close()
            print(f"Loaded table: {table_name}")
            return df
        except OperationalError as e:
            print(f"DB not ready (attempt {attempt + 1}/{retries}): {e}")
            time.sleep(delay)
        except Exception as e:
            print(f"Error loading table '{table_name}':", e)
            raise
    raise Exception(f"Failed to connect to DB after {retries} attempts")

# Load datasets from PostgreSQL
users_df = load_table('user_profiles')
recipes_df = load_table('recipes')
ingredients_df = load_table('ingredients')
recipe_ingredients_df = load_table('recipe_ingredients')
reviews_df = load_table('reviews')
user_fridge_df = load_table('user_fridge_ingredients')
cookbook_df = load_table('user_cookbook_recipes')

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
hybrid_rec = HybridRecommender({
    'content': content_rec,
    'collaborative': collab_rec,
    'popularity': pop_rec
})

# Recipe ID to title map
recipe_id_to_title = recipes_df.set_index('id')['name'].to_dict()

def map_ids_to_titles(recipe_ids):
    return [recipe_id_to_title.get(rid, f"(missing title: {rid})") for rid in recipe_ids]

def map_ids_to_details(recipe_ids):
    # Returns a list of dicts with id, title, and author for each recipe
    details = []
    for rid in recipe_ids:
        title = recipe_id_to_title.get(rid, f"(missing title: {rid})")
        # Try to get author from recipes_df
        author = None
        if rid in recipes_df['id'].values:
            author = recipes_df.loc[recipes_df['id'] == rid, 'posting_user_id'].values[0]
        details.append({
            'id': rid,
            'title': title,
            'author': author
        })
    return details

# FastAPI app setup
app = FastAPI()

# CORS middleware to allow frontend to talk to backend
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:8020", "http://127.0.0.1:8020"],  # Replace with frontend URL in production
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Request schema
class RecommendationRequest(BaseModel):
    user_id: str
    top_n: int = 5

# Endpoints
@app.get("/")
def read_root():
    return {
        "message": "Welcome to the We-Are-Cooking Recommendation API. Visit /docs to test endpoints."
    }

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
    titles = map_ids_to_titles(ids)
    logger.info(f"[RECOMMENDER OUTPUT] Content-based: {titles}")
    return {"recipes": titles}

@app.post("/recommend/collaborative")
def recommend_collaborative(req: RecommendationRequest):
    ids = collab_rec.recommend(req.user_id, req.top_n)
    titles = map_ids_to_titles(ids)
    logger.info(f"[RECOMMENDER OUTPUT] Collaborative: {titles}")
    return {"recipes": titles}

@app.get("/recommend/popularity")
def recommend_popularity(top_n: int = 5):
    ids = pop_rec.recommend(top_n)
    details = map_ids_to_details(ids)
    logger.info(f"[RECOMMENDER OUTPUT] Popularity-based: {details}")
    return {"recipes": details}

@app.post("/recommend/hybrid")
def recommend_hybrid(req: RecommendationRequest):
    ids = hybrid_rec.recommend(req.user_id, req.top_n)
    details = map_ids_to_details(ids)
    logger.info(f"[RECOMMENDER OUTPUT] Hybrid: {details}")
    return {"recipes": details}

@app.post("/recommend/fridge")
def recommend_fridge(req: RecommendationRequest):
    ids = fridge_rec.recommend(req.user_id, req.top_n)
    details = map_ids_to_details(ids)
    logger.info(f"[RECOMMENDER OUTPUT] Fridge-based: {details}")
    return {"recipes": details}