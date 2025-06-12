from collections import Counter

class HybridRecommender:
    def __init__(self, recommenders, weights=None):
        """
        Parameters:
        - recommenders: dict → name → recommender instance
        - weights: dict → name → float weight (optional, default = 1.0)
        """
        self.recommenders = recommenders
        self.weights = weights if weights else {name: 1.0 for name in recommenders}

    def recommend(self, user_id, top_n=10):
        """
        Combine recommendations from multiple recommenders.

        Parameters:
        - user_id: str → user identifier
        - top_n: int → number of final recommendations

        Returns:
        - list of top_n recipe_ids
        """
        combined = []

        for name, recommender in self.recommenders.items():
            try:
                if name == "popularity":
                    # Popularity does not require user_id
                    recs = recommender.recommend(top_n * 2)
                else:
                    recs = recommender.recommend(user_id, top_n * 2)

                weight = int(self.weights.get(name, 1))
                weighted_recs = [r for r in recs for _ in range(weight)]
                combined.extend(weighted_recs)
            except Exception as e:
                print(f"[HybridRecommender] Warning: {name} failed with error → {e}")

        # Count frequency, sort, and return top_n
        counter = Counter(combined)
        top_recs = [recipe_id for recipe_id, _ in counter.most_common(top_n)]

        return top_recs