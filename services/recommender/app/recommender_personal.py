"""
Personalized Recommendation Based on Ratings + Favorites

Uses:
- Explicit ratings (1 to 5)
- Implicit favorites (treated as rating = 4.5)
Now includes Redis caching per user.
"""

import pandas as pd
import numpy as np
import os
from sklearn.metrics.pairwise import cosine_similarity
from app.redis_cache import get_cached_recommendations, cache_recommendations

DATA_PATH = os.getenv("MOCK_DATA_PATH", "./mock_data/")

# Load ratings and favorites
ratings_df = pd.read_csv(os.path.join(DATA_PATH, "ratings.csv"))
favorites_df = pd.read_csv(os.path.join(DATA_PATH, "favorites.csv"))
recipes_df = pd.read_csv(os.path.join(DATA_PATH, "recipes.csv"))

# Treat favorites as implicit positive ratings
favorites_df = favorites_df[['user_id', 'recipe_id']].copy()
favorites_df['rating'] = 4.5

# Merge ratings and favorites
combined_df = pd.concat([ratings_df[['user_id', 'recipe_id', 'rating']], favorites_df], ignore_index=True)

# Create rating matrix
ratings_matrix = combined_df.pivot(index='user_id', columns='recipe_id', values='rating').fillna(0)

def recommend_from_ratings(user_id: str, top_k: int = 10):
    # Check Redis cache
    cached = get_cached_recommendations(user_id, rec_type="personal")
    if cached:
        return {"recommendations": cached}

    if user_id not in ratings_matrix.index:
        return {"recommendations": []}

    user_vector = [ratings_matrix.loc[user_id]]
    similarities = cosine_similarity(user_vector, ratings_matrix)[0]

    weighted_scores = np.dot(similarities, ratings_matrix.values)
    predictions = pd.Series(weighted_scores, index=ratings_matrix.columns)

    seen = combined_df[combined_df['user_id'] == user_id]['recipe_id'].tolist()
    predictions = predictions.drop(seen, errors='ignore')

    top = predictions.sort_values(ascending=False).head(top_k)

    results = []
    for recipe_id, score in top.items():
        name = recipes_df[recipes_df['id'] == recipe_id]['name'].values[0]
        results.append({
            "id": recipe_id,
            "name": name,
            "score": round(score, 2),
            "missing_ingredients": []
        })

    # Cache results in Redis
    cache_recommendations(user_id, results, rec_type="personal")
    return {"recommendations": results}
