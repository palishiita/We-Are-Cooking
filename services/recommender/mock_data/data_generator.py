import pandas as pd
import numpy as np
from faker import Faker
import random
import uuid

# Configurable Parameters
NUM_RECIPES = 5000
NUM_USERS = 1000
RATINGS_PER_USER = 15
FAVORITES_PER_USER = 8

CATEGORY_RULES = {
    'Vegan': ['tofu', 'lentils', 'beans', 'spinach', 'broccoli', 'carrot', 'rice', 'onion', 'garlic', 'olive oil'],
    'Dessert': ['sugar', 'flour', 'egg', 'butter', 'milk', 'vanilla', 'chocolate', 'cream'],
    'Quick Meals': ['pasta', 'egg', 'cheese', 'spinach', 'tomato', 'onion', 'garlic', 'bread'],
    'Healthy': ['quinoa', 'broccoli', 'carrot', 'spinach', 'chicken', 'olive oil', 'avocado'],
    'Comfort Food': ['beef', 'cheese', 'butter', 'potato', 'flour', 'milk'],
    'Breakfast': ['egg', 'milk', 'bread', 'butter', 'banana', 'yogurt', 'oats'],
    'Dinner': ['chicken', 'rice', 'beans', 'tomato', 'cheese', 'onion', 'pepper'],
    'Gluten-Free': ['rice', 'corn', 'chicken', 'potato', 'avocado', 'spinach']
}

faker = Faker()
random.seed(42)
np.random.seed(42)

def generate_uuid():
    return str(uuid.uuid4())

# Ingredients
all_ingredients = sorted(list(set(ing for lst in CATEGORY_RULES.values() for ing in lst)))
ingredients_df = pd.DataFrame({
    'id': [generate_uuid() for _ in all_ingredients],
    'name': all_ingredients
})
ingredient_name_to_id = dict(zip(ingredients_df['name'], ingredients_df['id']))

# Categories
category_names = list(CATEGORY_RULES.keys())
categories_df = pd.DataFrame({
    'id': [generate_uuid() for _ in category_names],
    'name': category_names,
    'description': [faker.text(max_nb_chars=100) for _ in category_names]
})
category_name_to_id = dict(zip(categories_df['name'], categories_df['id']))

# Ingredients-Categories connection
ingredient_cat_conn = []
for category, ingredients in CATEGORY_RULES.items():
    cat_id = category_name_to_id[category]
    for ing in ingredients:
        ingredient_cat_conn.append({
            'id': generate_uuid(),
            'ingredient_id': ingredient_name_to_id[ing],
            'category_id': cat_id
        })
ingredient_cat_df = pd.DataFrame(ingredient_cat_conn)

# Recipes
recipes = []
recipe_ingredients = []
recipe_categories = []

for _ in range(NUM_RECIPES):
    recipe_id = generate_uuid()
    category = random.choice(category_names)
    category_id = category_name_to_id[category]
    allowed_ingredients = CATEGORY_RULES[category]

    name = faker.catch_phrase()
    description = faker.text(max_nb_chars=300)
    total_time = np.random.randint(10, 45) if category == 'Quick Meals' else np.random.randint(20, 120)

    recipes.append({
        'id': recipe_id,
        'name': name,
        'description': description
    })

    recipe_categories.append({
        'id': generate_uuid(),
        'recipe_id': recipe_id,
        'category_id': category_id
    })

    selected_ingredients = random.sample(allowed_ingredients, k=random.randint(3, 6))
    for ing in selected_ingredients:
        recipe_ingredients.append({
            'id': generate_uuid(),
            'recipe_id': recipe_id,
            'ingredient_id': ingredient_name_to_id[ing]
        })

recipes_df = pd.DataFrame(recipes)
recipe_ingredients_df = pd.DataFrame(recipe_ingredients)
recipe_categories_df = pd.DataFrame(recipe_categories)

# Users
users_df = pd.DataFrame({
    'id': [generate_uuid() for _ in range(NUM_USERS)],
    'username': [faker.user_name() for _ in range(NUM_USERS)]
})

# Ratings
ratings_data = []
for user_id in users_df['id']:
    rated = np.random.choice(recipes_df['id'], size=RATINGS_PER_USER, replace=False)
    for recipe_id in rated:
        ratings_data.append({
            'id': generate_uuid(),
            'user_id': user_id,
            'recipe_id': recipe_id,
            'rating': np.random.randint(1, 6)
        })
ratings_df = pd.DataFrame(ratings_data)

# Favorites
favorites_data = []
for user_id in users_df['id']:
    favorited = np.random.choice(recipes_df['id'], size=FAVORITES_PER_USER, replace=False)
    for recipe_id in favorited:
        favorites_data.append({
            'id': generate_uuid(),
            'user_id': user_id,
            'recipe_id': recipe_id,
            'is_favorite': True
        })
favorites_df = pd.DataFrame(favorites_data)

# Dietary Restrictions
dietary_data = []
for user_id in users_df['id']:
    restricted = random.sample(categories_df['id'].tolist(), k=random.randint(0, 2))
    for cat_id in restricted:
        dietary_data.append({
            'id': generate_uuid(),
            'user_id': user_id,
            'ingredient_category_id': cat_id
        })
dietary_df = pd.DataFrame(dietary_data)

# === Output Files ===
output_path = "C:/Users/ishii/Desktop/We-Are-Cooking/services/recommender/mock_data/"

recipes_df.to_csv(output_path + "recipes.csv", index=False)
ingredients_df.to_csv(output_path + "ingredients.csv", index=False)
recipe_ingredients_df.to_csv(output_path + "recipe_ingredients.csv", index=False)
users_df.to_csv(output_path + "users.csv", index=False)
ratings_df.to_csv(output_path + "ratings.csv", index=False)
favorites_df.to_csv(output_path + "favorites.csv", index=False)
categories_df.to_csv(output_path + "categories.csv", index=False)
recipe_categories_df.to_csv(output_path + "recipe_categories.csv", index=False)
ingredient_cat_df.to_csv(output_path + "ingredient_categories_connection.csv", index=False)
dietary_df.to_csv(output_path + "user_dietary_restrictions.csv", index=False)

print("✅ Mock data generation complete!")
print(f"Recipes: {len(recipes_df)}, Ingredients: {len(ingredients_df)}, Users: {len(users_df)}")