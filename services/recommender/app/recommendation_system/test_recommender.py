import os
import pandas as pd
from fridge import FridgeBasedRecommender
from content import ContentBasedRecommender
from collaborative import CollaborativeRecommender  # , SVDRecommender, ALSRecommender
from popularity import PopularityRecommender
from hybrid import HybridRecommender

# Safe CSV loader with latin1 encoding
def load_csv(file_name, base_path):
    path = os.path.join(base_path, file_name)
    return pd.read_csv(path, encoding='latin1')

# Base data folder
base_path = r'C:\Users\ishii\Desktop\We-Are-Cooking\services\recommender\app\data'

# Load all datasets
users_df = load_csv('users.csv', base_path)
recipes_df = load_csv('recipes.csv', base_path)
ingredients_df = load_csv('ingredients.csv', base_path)
recipe_ingredients_df = load_csv('recipe_ingredients.csv', base_path)
reviews_df = load_csv('reviews.csv', base_path)
user_fridge_df = load_csv('user_fridge_ingredients.csv', base_path)
cookbook_df = load_csv('user_cookbook_recipes.csv', base_path)

# Ingredient importance fallback
ingredient_info_df = pd.DataFrame({
    'ingredient_id': ingredients_df['id'],
    'importance': 1  # assume equal importance
})

# Initialize individual recommenders

# Fridge-based
fridge_rec = FridgeBasedRecommender(
    user_fridge_df=user_fridge_df,
    recipe_ingredients_df=recipe_ingredients_df,
    ingredient_info_df=ingredient_info_df
)

# Content-based
content_rec = ContentBasedRecommender(
    recipe_ingredients_df=recipe_ingredients_df,
    reviews_df=reviews_df
)

# Collaborative filtering (basic)
collab_rec = CollaborativeRecommender(
    reviews_df=reviews_df,
    cookbook_df=cookbook_df
)

# Popularity-based
pop_rec = PopularityRecommender(
    reviews_df=reviews_df,
    cookbook_df=cookbook_df
)

# Hybrid recommender (combining content, collaborative, popularity)
hybrid_rec = HybridRecommender({
    'content': content_rec,
    'collaborative': collab_rec,
    # 'svd': svd_rec,
    # 'als': als_rec,
    'popularity': pop_rec
})

# Select test user
test_user_id = users_df['id'].iloc[0]

# Create recipe ID → title map
recipe_id_to_title = recipes_df.set_index('id')['name'].to_dict()

# Define helper to map IDs to titles
def map_ids_to_titles(recipe_ids, id_to_title_map):
    return [id_to_title_map.get(rid, f"(missing title: {rid})") for rid in recipe_ids]

# Run recommenders and print outputs with titles
print("\n=== Content-Based Recommendations ===")
content_ids = content_rec.recommend(user_id=test_user_id, top_n=5)
print(map_ids_to_titles(content_ids, recipe_id_to_title))

print("\n=== Collaborative Recommendations ===")
collab_ids = collab_rec.recommend(user_id=test_user_id, top_n=5)
print(map_ids_to_titles(collab_ids, recipe_id_to_title))

print("\n=== Popularity Recommendations ===")
pop_ids = pop_rec.recommend(top_n=5)
print(map_ids_to_titles(pop_ids, recipe_id_to_title))

print("\n=== Hybrid Recommendations ===")
hybrid_ids = hybrid_rec.recommend(user_id=test_user_id, top_n=5)
print(map_ids_to_titles(hybrid_ids, recipe_id_to_title))

print("\n=== Fridge-Based Recommendations ===")
fridge_ids = fridge_rec.recommend(user_id=test_user_id, top_n=5)
print(map_ids_to_titles(fridge_ids, recipe_id_to_title))
