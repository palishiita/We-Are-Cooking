import pandas as pd
import numpy as np
from datetime import datetime, timedelta

class PopularityRecommender:
    """
    Popularity-based recommender system.

    How it works:
    - Calculates a Bayesian average rating per recipe to avoid overrating small-sample recipes.
    - Adds a recency boost to highlight recipes that are trending in the last N days.
    - Optionally adds a save boost from cookbook saves (if provided).
    - Supports filtering by cuisine if metadata is available.

    Parameters:
    - reviews_df: DataFrame with recipe_id, rating, timestamp.
    - cookbook_df: DataFrame with user_id, recipe_id (optional, for saves).
    - recipe_metadata_df: DataFrame with recipe_id, cuisine (optional).
    - m: Smoothing constant for Bayesian average.
    - recency_days: Number of days to consider for recency boost.
    - use_cuisine: If True, allows recommending top recipes by cuisine.
    """
    def __init__(self, reviews_df, cookbook_df=None, recipe_metadata_df=None,
                 m=5, recency_days=30, use_cuisine=False):

        # Calculate average rating and review count per recipe
        review_stats = reviews_df.groupby('recipe_id').agg(
            avg_rating=('rating', 'mean'),
            count=('rating', 'count')
        )

        # Calculate global average rating (μ)
        mu = reviews_df['rating'].mean()

        # Apply Bayesian average smoothing
        # Formula: (avg_rating * n + mu * m) / (n + m)
        review_stats['bayesian_score'] = (
            (review_stats['avg_rating'] * review_stats['count'] + mu * m) / (review_stats['count'] + m)
        )

        # Add recency boost (if timestamp exists)
        if 'timestamp' in reviews_df.columns:
            cutoff = datetime.now() - timedelta(days=recency_days)
            recent_reviews = reviews_df[pd.to_datetime(reviews_df['timestamp']) >= cutoff]
            recent_counts = recent_reviews.groupby('recipe_id').size()
            review_stats['recent_count'] = review_stats.index.map(recent_counts).fillna(0)
            review_stats['recency_boost'] = np.log1p(review_stats['recent_count'])
            review_stats['final_score'] = review_stats['bayesian_score'] + review_stats['recency_boost']
        else:
            review_stats['final_score'] = review_stats['bayesian_score']

        # Add saves from cookbook
        if cookbook_df is not None:
            save_counts = cookbook_df.groupby('recipe_id').size()
            review_stats['save_count'] = review_stats.index.map(save_counts).fillna(0)
            review_stats['final_score'] += np.log1p(review_stats['save_count'])

        # Store recipe scores and optional cuisine labels
        self.recipe_scores = review_stats.sort_values('final_score', ascending=False)
        if use_cuisine and recipe_metadata_df is not None:
            self.cuisine_map = recipe_metadata_df.set_index('recipe_id')['cuisine'].to_dict()
        else:
            self.cuisine_map = None

    def recommend(self, top_n=5, cuisine=None):
        """
        Returns the top-N recommended recipe IDs.

        Parameters:
        - top_n: Number of recipes to return.
        - cuisine: Optional cuisine filter.

        Returns:
        - List of recipe_ids.
        """
        if cuisine and self.cuisine_map:
            filtered = self.recipe_scores[
                self.recipe_scores.index.isin([
                    rid for rid, cui in self.cuisine_map.items() if cui == cuisine
                ])
            ]
            return filtered.head(top_n).index.tolist()
        else:
            return self.recipe_scores.head(top_n).index.tolist()