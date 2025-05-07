class FridgeBasedRecommender:
    """
    Fridge-based recipe recommender.

    How it works:
    - Recommends recipes based on the ingredients a user currently has in their fridge.
    - Considers ingredient importance (core vs. optional).
    - Supports ingredient substitutions.
    - Respects dietary restrictions (e.g., vegan, gluten-free).
    - Gives a small bonus to recipes that require minimal shopping (≤2 missing items).

    Parameters:
    - user_fridge_df: DataFrame with user_id, ingredient_id.
    - recipe_ingredients_df: DataFrame with recipe_id, ingredient_id.
    - ingredient_info_df: DataFrame with ingredient_id, importance (weight).
    - substitutions: dict → ingredient_id → list of acceptable substitutes.
    - user_dietary_restrictions: dict → dietary category → set of forbidden ingredient_ids.
    """
    def __init__(self, user_fridge_df, recipe_ingredients_df, ingredient_info_df,
                 substitutions=None, user_dietary_restrictions=None):
        # Map user → set of fridge ingredients
        self.user_fridge = user_fridge_df.groupby('user_id')['ingredient_id'].apply(set).to_dict()

        # Map recipe → set of required ingredients
        self.recipe_ingredients = recipe_ingredients_df.groupby('recipe_id')['ingredient_id'].apply(set).to_dict()

        # Map ingredient → importance weight (e.g., core=2, optional=1)
        self.ingredient_weights = ingredient_info_df.set_index('ingredient_id')['importance'].to_dict()

        # Substitution mapping (optional)
        self.substitutions = substitutions or {}

        # Dietary restrictions mapping (optional)
        self.user_dietary_restrictions = user_dietary_restrictions or {}

    def recommend(self, user_id, top_n=5, user_dietary_category=None):
        """
        Recommend top-N recipes based on the user's fridge and preferences.

        Parameters:
        - user_id: str → ID of the user.
        - top_n: int → number of recommendations to return.
        - user_dietary_category: str → dietary restriction category to exclude (e.g., 'vegan').

        Returns:
        - List of recipe_ids.
        """
        if user_id not in self.user_fridge:
            return []

        fridge = self.user_fridge[user_id]
        scores = {}

        for recipe_id, ingredients in self.recipe_ingredients.items():
            # Apply dietary restrictions (if provided)
            if user_dietary_category:
                forbidden = self.user_dietary_restrictions.get(user_dietary_category, set())
                if ingredients & forbidden:
                    continue

            # Match ingredients (including substitutions)
            matched = set()
            for ing in ingredients:
                if ing in fridge:
                    matched.add(ing)
                elif any(sub in fridge for sub in self.substitutions.get(ing, [])):
                    matched.add(ing)

            # Calculate weighted match score
            total_weight = sum(self.ingredient_weights.get(ing, 1) for ing in ingredients)
            matched_weight = sum(self.ingredient_weights.get(ing, 1) for ing in matched)
            match_score = matched_weight / total_weight if total_weight else 0

            # Boost recipes with minimal shopping effort
            missing_count = len(ingredients - matched)
            if missing_count <= 2:
                match_score += 0.1  # small bonus

            scores[recipe_id] = match_score

        # Return top-N recipes sorted by score
        top_recipes = sorted(scores, key=scores.get, reverse=True)[:top_n]
        return top_recipes