import pandas as pd
import numpy as np
from sklearn.metrics.pairwise import cosine_similarity

class ContentBasedRecommender:
    """
    Content-based recommender system.

    How it works:
    - Builds a recipe × ingredient matrix using TF-IDF-like weighting.
    - Computes recipe-to-recipe cosine similarity.
    - For a given user, recommends recipes most similar to those they rated highly.
    - If no user history, falls back to general similarity ranking.

    Parameters:
    - recipe_ingredients_df: DataFrame with recipe_id, ingredient_id.
    - reviews_df: DataFrame with user_id, recipe_id, rating (optional, for personalization).
    """
    def __init__(self, recipe_ingredients_df, reviews_df=None):
        # Calculate ingredient frequency across all recipes (for inverse weighting)
        ingredient_counts = recipe_ingredients_df['ingredient_id'].value_counts()
        self.ingredient_weights = 1 / np.log1p(ingredient_counts)  # like TF-IDF weighting
        
        # Build recipe × ingredient weighted matrix
        recipe_ingredients_df['weight'] = recipe_ingredients_df['ingredient_id'].map(self.ingredient_weights)
        self.recipe_ingredient_matrix = recipe_ingredients_df.pivot_table(
            index='recipe_id',
            columns='ingredient_id',
            values='weight',
            aggfunc='sum',
            fill_value=0
        )

        # Calculate recipe-to-recipe cosine similarity
        self.recipe_ids = self.recipe_ingredient_matrix.index.tolist()
        self.similarity_matrix = cosine_similarity(self.recipe_ingredient_matrix)

        # Store user-recipe ratings (optional, for personalization)
        self.reviews_df = reviews_df

    def recommend(self, user_id=None, top_n=5):
        """
        Recommend top-N recipes for a user or general audience.

        Parameters:
        - user_id: str (optional) → if provided, use their liked recipes as anchors.
        - top_n: int → number of recommendations to return.

        Returns:
        - List of recipe_ids.
        """
        # If user history is available, use recipes they rated highly (≥4)
        if self.reviews_df is not None and user_id in self.reviews_df['user_id'].unique():
            user_recipes = self.reviews_df[
                (self.reviews_df['user_id'] == user_id) & 
                (self.reviews_df['rating'] >= 4)
            ]['recipe_id'].tolist()
        else:
            # Fallback: use first recipe as anchor (typically not ideal but ensures output)
            user_recipes = self.recipe_ids[:1]

        # Calculate average similarity across liked recipes
        avg_similarity = np.zeros(len(self.recipe_ids))
        for recipe_id in user_recipes:
            if recipe_id in self.recipe_ids:
                idx = self.recipe_ids.index(recipe_id)
                avg_similarity += self.similarity_matrix[idx]
        avg_similarity /= len(user_recipes)

        # Get top recommendations (excluding already liked recipes)
        recommendations = sorted(
            [(self.recipe_ids[i], score) for i, score in enumerate(avg_similarity)
             if self.recipe_ids[i] not in user_recipes],
            key=lambda x: x[1],
            reverse=True
        )
        top_recipes = [rec[0] for rec in recommendations[:top_n]]
        return top_recipes