import 'recipe.dart';

class RecipeResponse {
  final List<Recipe> data;
  final int totalElements;
  final int totalPages;
  final int page;
  final int pageSize;

  RecipeResponse({
    required this.data,
    required this.totalElements,
    required this.totalPages,
    required this.page,
    required this.pageSize,
  });

  factory RecipeResponse.fromJson(Map<String, dynamic> json) {
    return RecipeResponse(
      data: (json['data'] as List)
          .map((item) => Recipe.fromJson(item))
          .toList(),
      totalElements: json['totalElements'] ?? 0,
      totalPages: json['totalPages'] ?? 1,
      page: json['page'] ?? 1,
      pageSize: json['pageSize'] ?? 1,
    );
  }

  factory RecipeResponse.empty(int topN) {
    return RecipeResponse(
      data: [],
      totalElements: 0,
      totalPages: 1,
      page: 0,
      pageSize: topN,
    );
  }
}