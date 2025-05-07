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

        for name, rec in self.recommenders.items():
            try:
                recs = rec.recommend(user_id, top_n * 2)  # fetch extra to mix better
                # Repeat recommendations based on weight
                weighted_recs = [r for r in recs for _ in range(int(self.weights.get(name, 1)))]
                combined.extend(weighted_recs)
            except Exception as e:
                print(f"[HybridRecommender] Warning: {name} failed with error → {e}")
                continue

        # Count recipe frequencies and sort
        counter = Counter(combined)
        top_recs = [recipe_id for recipe_id, _ in counter.most_common(top_n)]

        return top_recs