import pandas as pd
import os

    # Fridge-Based Recipe Recommendation

    # This function recommends recipes based on the ingredients currently available in a user's fridge.

    # Workflow:
    # 1. Load the list of ingredients in the user's fridge.
    # 2. For each recipe:
    #     - Check which ingredients match the fridge items.
    #     - Calculate a weighted match score based on ingredient frequency (rarer ingredients are weighted higher).
    #     - Apply a penalty for each missing ingredient.
    #     - Skip recipes containing banned ingredients based on dietary restrictions (if enabled).
    # 3. Return top K recipes with:
    #     - Final match score (normalized and penalized)
    #     - List of missing ingredients

    # Scoring Formula:
    #     score = (matched_weight / total_weight) - (penalty_per_missing * num_missing)

    # Returns:
    #     A list of recommended recipes, each with ID, name, score, and missing ingredients.


DATA_PATH = os.environ.get("MOCK_DATA_PATH", "./mock_data/")

# Load CSVs
recipes_df = pd.read_csv(os.path.join(DATA_PATH, "recipes.csv"))
recipe_ingredients_df = pd.read_csv(os.path.join(DATA_PATH, "recipe_ingredients.csv"))
user_fridge_df = pd.read_csv(os.path.join(DATA_PATH, "user_fridge_ingredients.csv"))
ingredients_df = pd.read_csv(os.path.join(DATA_PATH, "ingredients.csv"))

# Optional (if using dietary filters)
try:
    restrictions_df = pd.read_csv(os.path.join(DATA_PATH, "user_dietary_restrictions.csv"))
    category_map_df = pd.read_csv(os.path.join(DATA_PATH, "ingredient_categories_connection.csv"))
    use_diet_filtering = True
except:
    use_diet_filtering = False

# Calculate simple frequency-based weights for ingredients
ingredient_freq = recipe_ingredients_df['ingredient_id'].value_counts().to_dict()
max_freq = max(ingredient_freq.values())

ingredient_weights = {
    ing_id: round(1 - (freq / max_freq), 2) + 0.1  # 0.1 minimum weight
    for ing_id, freq in ingredient_freq.items()
}

def recommend_from_fridge(user_id: str, top_k: int = 10):
    # Get user's fridge ingredients
    fridge_ingredients = set(
        user_fridge_df[user_fridge_df['user_id'] == user_id]['ingredient_id']
    )
    if not fridge_ingredients:
        return {"recommendations": []}

    # Optionally filter banned ingredients
    if use_diet_filtering:
        restricted_cats = restrictions_df[restrictions_df['user_id'] == user_id]['ingredient_category_id']
        banned_ings = category_map_df[category_map_df['category_id'].isin(restricted_cats)]['ingredient_id']
        banned_ings_set = set(banned_ings)
    else:
        banned_ings_set = set()

    scored_recipes = []

    for recipe_id in recipes_df['id']:
        recipe_ings = recipe_ingredients_df[
            recipe_ingredients_df['recipe_id'] == recipe_id
        ]['ingredient_id'].tolist()

        if not recipe_ings:
            continue

        recipe_ing_set = set(recipe_ings)

        # Dietary filtering
        if recipe_ing_set.intersection(banned_ings_set):
            continue

        matches = fridge_ingredients.intersection(recipe_ing_set)
        missing = recipe_ing_set - fridge_ingredients

        if not matches:
            continue

        # Weighted scoring
        matched_weight = sum(ingredient_weights.get(i, 1.0) for i in matches)
        total_weight = sum(ingredient_weights.get(i, 1.0) for i in recipe_ing_set)

        score = matched_weight / total_weight

        # Add a penalty for too many missing ingredients
        penalty = 0.05 * len(missing)  # tweakable
        final_score = round(max(score - penalty, 0), 2)

        recipe_name = recipes_df[recipes_df['id'] == recipe_id]['name'].values[0]
        missing_names = ingredients_df[ingredients_df['id'].isin(missing)]['name'].tolist()

        scored_recipes.append({
            "id": recipe_id,
            "name": recipe_name,
            "score": final_score,
            "missing_ingredients": missing_names
        })

    scored_recipes.sort(key=lambda x: x['score'], reverse=True)
    return {"recommendations": scored_recipes[:top_k]}