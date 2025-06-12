import 'package:dio/dio.dart';
import 'api_recommender.dart'; // Use the new recommender client

class RecommendationService {
  static final ApiRecommender _client = ApiRecommender();

  /// Fetches only the recipe titles from the hybrid recommender.
  static Future<List<String>> getRecommendedRecipeTitles({
    required String userId,
    int topN = 5,
  }) async {
    try {
      // Correct: hybrid recommender is a POST endpoint
      final Response hybridRes = await _client.post(
        'recommend/hybrid',
        data: {'user_id': userId, 'top_n': topN},
      );

      final hybridRecipes = (hybridRes.statusCode == 200 &&
              hybridRes.data != null &&
              hybridRes.data['recipes'] is List)
          ? List<String>.from(hybridRes.data['recipes'])
          : [];

      // Check if hybrid is non-empty before returning
      if (hybridRecipes.isNotEmpty) {
        return List<String>.from(hybridRecipes);
      }

      // Fallback to popularity recommender (this is GET)
      final Response popRes = await _client.get(
        'recommend/popularity',
        queryParameters: {'top_n': topN},
      );

      if (popRes.statusCode == 200 &&
          popRes.data != null &&
          popRes.data['recipes'] is List) {
        return List<String>.from(popRes.data['recipes']);
      }

      return [];
    } catch (e) {
      print('Recommendation error: $e');
      return [];
    }
  }

  /// Fetches recipe details (id, title, author) from the hybrid recommender.
  static Future<List<Map<String, dynamic>>> getRecommendedRecipeDetails({
    required String userId,
    int topN = 5,
  }) async {
    try {
      // Try hybrid recommender (POST)
      final Response hybridRes = await _client.post(
        'recommend/hybrid',
        data: {'user_id': userId, 'top_n': topN},
      );
      final hybridRecipes = (hybridRes.statusCode == 200 &&
              hybridRes.data != null &&
              hybridRes.data['recipes'] is List)
          ? List<Map<String, dynamic>>.from(hybridRes.data['recipes'])
          : [];
      if (hybridRecipes.isNotEmpty) {
        return hybridRecipes.map((e) => Map<String, dynamic>.from(e)).toList();
      }
      // Fallback to popularity recommender (GET)
      final Response popRes = await _client.get(
        'recommend/popularity',
        queryParameters: {'top_n': topN},
      );
      if (popRes.statusCode == 200 &&
          popRes.data != null &&
          popRes.data['recipes'] is List) {
        return List<Map<String, dynamic>>.from(
          (popRes.data['recipes'] as List).map((e) => Map<String, dynamic>.from(e)),
        );
      }
      return [];
    } catch (e) {
      print('Recommendation error: $e');
      return [];
    }
  }
}
