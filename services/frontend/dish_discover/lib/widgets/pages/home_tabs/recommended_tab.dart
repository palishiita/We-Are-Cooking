import 'package:flutter/material.dart';
import '../../../entities/app_state.dart';
import '../../../entities/new_recipe.dart';
import '../../../widgets/display/recipe_list.dart';

class RecommendedTab extends StatefulWidget {
  const RecommendedTab({super.key});

  @override
  State<RecommendedTab> createState() => _RecommendedTabState();
}

class _RecommendedTabState extends State<RecommendedTab> {
 
  //final String userId = 'd33f718d-97b6-4d42-adc1-856b31ded5b4';
final String userId = '';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Recommended'),
        centerTitle: true,
      ),
      body: RecipeList(
        searchQuery: null,
        getRecipes: (page) async {
          // Only load on first page — FastAPI doesn't support pagination
          if (page > 0) {
            return RecipeResponse(
              data: [],
              totalElements: 0,
              totalPages: 1,
              page: 0,
              pageSize: 10,
            );
          }
          return await Recipe.getRecommendedRecipesForUser(
            userId: userId,
            topN: 5,
          );
        },
      ),
    );
  }
}