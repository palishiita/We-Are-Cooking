import pandas as pd
import numpy as np
import implicit
import scipy.sparse as sparse
from sklearn.metrics.pairwise import cosine_similarity
from surprise import SVD, Dataset, Reader

# Basic user-user or item-item similarity recommender
class CollaborativeRecommender:
    """
    Collaborative filtering using user-user or item-item similarity.

    How it works:
    - Builds a user × recipe interaction matrix from ratings and saves.
    - Calculates recipe-to-recipe cosine similarity.
    - For a given user, recommends recipes most similar to those they interacted with.

    """
    def __init__(self, reviews_df, cookbook_df=None):
        reviews_df['interaction'] = reviews_df['rating'].fillna(3)
        reviews_df.loc[reviews_df['has_photos'], 'interaction'] += 1

        if cookbook_df is not None:
            saves_df = cookbook_df.copy()
            saves_df['interaction'] = 1.5 + saves_df['is_favorite'].astype(float)
            reviews_df = pd.concat([reviews_df[['user_id', 'recipe_id', 'interaction']],
                                    saves_df[['user_id', 'recipe_id', 'interaction']]])

        self.user_recipe_matrix = reviews_df.pivot_table(index='user_id', columns='recipe_id',
                                                         values='interaction', aggfunc='sum', fill_value=0)
        self.recipe_recipe_similarity = cosine_similarity(self.user_recipe_matrix.T)
        self.recipe_ids = self.user_recipe_matrix.columns.tolist()
        self.user_ids = self.user_recipe_matrix.index.tolist()

    def recommend(self, user_id, top_n=5):
        if user_id not in self.user_ids:
            return []
        user_vector = self.user_recipe_matrix.loc[user_id].values
        scores = np.dot(self.recipe_recipe_similarity, user_vector)
        seen_recipes = self.user_recipe_matrix.loc[user_id] > 0
        scores[seen_recipes.values] = -np.inf
        top_indices = np.argpartition(-scores, range(top_n))[:top_n]
        top_recipes = [self.recipe_ids[i] for i in top_indices[np.argsort(-scores[top_indices])]]
        return top_recipes


# SVD with surprise (explicit ratings)
class SVDRecommender:
    """
    Collaborative filtering using matrix factorization (SVD) on explicit ratings.

    How it works:
    - Learns latent factors from user-recipe ratings using the surprise library.
    - Predicts user preference for unrated recipes.
    - Recommends top-rated predicted recipes.
    """
    def __init__(self, reviews_df):
        reader = Reader(rating_scale=(1, 5))
        data = Dataset.load_from_df(reviews_df[['user_id', 'recipe_id', 'rating']], reader)
        trainset = data.build_full_trainset()
        self.algo = SVD()
        self.algo.fit(trainset)
        self.recipe_ids = reviews_df['recipe_id'].unique()

    def recommend(self, user_id, top_n=5):
        predictions = []
        for recipe_id in self.recipe_ids:
            pred = self.algo.predict(user_id, recipe_id)
            predictions.append((recipe_id, pred.est))
        predictions.sort(key=lambda x: x[1], reverse=True)
        return [recipe for recipe, score in predictions[:top_n]]


# ALS with implicit (implicit feedback)
class ALSRecommender:
    """
    Collaborative filtering using Alternating Least Squares (ALS) on implicit feedback.

    How it works:
    - Builds a user × recipe sparse interaction matrix (views, saves, clicks).
    - Learns user and recipe latent factors using the implicit library.
    - Recommends recipes with highest predicted preference for each user.

    """
    def __init__(self, interactions_df):
        user_ids = interactions_df['user_id'].unique()
        recipe_ids = interactions_df['recipe_id'].unique()
        self.user_to_idx = {u: i for i, u in enumerate(user_ids)}
        self.recipe_to_idx = {r: i for i, r in enumerate(recipe_ids)}
        self.idx_to_recipe = {i: r for r, i in self.recipe_to_idx.items()}

        rows = interactions_df['user_id'].map(self.user_to_idx)
        cols = interactions_df['recipe_id'].map(self.recipe_to_idx)
        data = interactions_df['interaction_score']
        matrix = sparse.coo_matrix((data, (rows, cols)), shape=(len(user_ids), len(recipe_ids)))

        self.model = implicit.als.AlternatingLeastSquares(factors=20, iterations=15)
        self.model.fit(matrix.tocsr())
        self.matrix = matrix.tocsr()

    def recommend(self, user_id, top_n=5):
        if user_id not in self.user_to_idx:
            return []
        user_idx = self.user_to_idx[user_id]
        recs = self.model.recommend(user_idx, self.matrix, N=top_n)
        return [self.idx_to_recipe[i] for i, score in recs]
