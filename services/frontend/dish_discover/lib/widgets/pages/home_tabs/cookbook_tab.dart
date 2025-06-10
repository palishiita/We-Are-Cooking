//import 'package:dish_discover/entities/ingredient.dart';
//import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../../entities/app_state.dart';
import '../../../entities/new_recipe.dart';
import '../../../widgets/display/recipe_list.dart';

class CookbookTab extends StatefulWidget {
  const CookbookTab({super.key});

  @override
  State<CookbookTab> createState() => _CookbookTabState();
}

class _CookbookTabState extends State<CookbookTab> {
  final double appBarHeight = 56.0; // Adjust as needed

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        toolbarHeight: appBarHeight,
        scrolledUnderElevation: 0.0,
        title: const Text('Search'),
        centerTitle: true,
        leading: const BackButton(),
      ),
      body: RecipeList(
        searchQuery: null,
        getRecipes: (page) => Recipe.getCookbookRecipes(
          query: null,
          count: 10,
          page: page,
          sortBy: null,
          orderByAsc: true,
        ),
      ),
    );
  }

  /// Helper to generate dummy recipes
  //List<Recipe> _generateDummyRecipes() {
  //  final recipes = [
  //    Recipe(
  //      author: AppState.currentUser!.username,
  //      title: 'My test recipe',
  //      description: 'Lorem ipsum...',
  //      steps: AppState.markdownTestText,
  //      id: 1,
  //      image: Image.asset('assets/images/logo.png'),
  //    ),
  //    Recipe(
  //      author: 'test_dummy',
  //      title: 'My test recipe',
  //      description: 'Lorem ipsum...',
  //      steps: AppState.markdownTestText,
  //      id: 2,
  //      image: Image.asset('assets/images/logo.png'),
  //    ),
  //  ];
  //  recipes.forEach((recipe) {
  //    recipe.editRecipe(
  //      ingredients: [
  //        Ingredient(id: 0, name: 'tomato', quantity: 2),
  //        Ingredient(id: 0, name: 'basil', quantity: 2, unit: 'teaspoons'),
  //        Ingredient(id: 0, name: 'flatbread', quantity: 3, unit: 'pieces'),
  //        Ingredient(id: 0, name: 'olive oil', quantity: 1, unit: 'tablespoon'),
  //        Ingredient(id: 0, name: 'salt', quantity: 1, unit: 'pinch'),
  //        Ingredient(id: 0, name: 'garlic', quantity: 0.5, unit: 'teaspoons'),
  //      ],
  //      tags: [
  //        Tag(isPredefined: true, name: 'basil', category: TagCategory.ingredient),
  //        Tag(isPredefined: true, name: 'bread', category: TagCategory.ingredient),
  //        Tag(isPredefined: true, name: 'tomato', category: TagCategory.ingredient),
  //        Tag(isPredefined: true, name: 'Italian', category: TagCategory.cuisine),
  //        Tag(isPredefined: false, name: 'no-bake', category: null),
  //        Tag(isPredefined: false, name: 'cheap', category: TagCategory.expense),
  //        Tag(isPredefined: true, name: 'fast', category: TagCategory.time),
  //      ],
  //    );
  //  });
  //  return recipes;
  //}
}
