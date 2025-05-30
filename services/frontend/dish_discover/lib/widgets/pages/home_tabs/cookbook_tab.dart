import 'package:dish_discover/entities/ingredient.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../../entities/app_state.dart';
import '../../../entities/recipe.dart';
import '../../../entities/tag.dart';
import '../../dialogs/custom_dialog.dart';
import '../../inputs/custom_text_field.dart';
import '../edit_recipe.dart';
import '../../display/recipe_list.dart';
import '../reels_page.dart';

class CookbookTab extends StatelessWidget {
  const CookbookTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: DefaultTabController(
        length: 2, // 2 tabs: Saved and Liked
        child: Column(
          children: [
            const TabBar(
              labelColor: Colors.brown, // optional styling
              tabs: [
                Tab(text: 'Saved'),
                Tab(text: 'Liked'),
              ],
            ),
            Expanded(
              child: TabBarView(
                children: [
                  // --- Saved Recipes ---
                  RecipeList(
                    getRecipes: () => Future<List<Recipe>>(() {
                      List<Recipe> recipes = AppState.currentUser!.savedRecipes;
                      if (kDebugMode && recipes.isEmpty) {
                        recipes = _generateDummyRecipes();
                      }
                      return recipes;
                    }),
                  ),

                  // --- Liked Recipes ---
                  RecipeList(
                    getRecipes: () => Future<List<Recipe>>(() {
                      List<Recipe> recipes = AppState.currentUser!.likedRecipes;
                      if (kDebugMode && recipes.isEmpty) {
                        recipes = _generateDummyRecipes();
                      }
                      return recipes;
                    }),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
      floatingActionButton: Column(
        mainAxisAlignment: MainAxisAlignment.end,
        children: [
          FloatingActionButton(
            shape: const CircleBorder(),
            mini: true,
            child: const Icon(Icons.add),
            onPressed: () {
              TextEditingController titleController = TextEditingController();

              CustomDialog.callDialog(
                  context,
                  'Create recipe',
                  '',
                  null,
                  CustomTextField(
                      controller: titleController, hintText: 'Title'),
                  'Create', () {
                if (titleController.text.trim().isNotEmpty) {
                  Recipe newRecipe = Recipe(
                      id: 0,
                      title: titleController.text,
                      author: AppState.currentUser!.username);
                  AppState.currentUser!.addRecipe(newRecipe);
                  Future.microtask(() => Navigator.of(context).push(
                      MaterialPageRoute(
                          builder: (context) =>
                              EditRecipePage(recipeId: newRecipe.id))));
                  return null;
                } else {
                  return "Title cannot be empty";
                }
              });
            },
          ),
          const SizedBox(height: 8),
          FloatingActionButton(
            shape: const CircleBorder(),
            mini: true,
            child: const Icon(Icons.video_library),
            onPressed: () {
              Navigator.of(context).push(
                MaterialPageRoute(builder: (context) => const ReelsPage()),
              );
            },
          ),
        ],
      ),
    );
  }

  /// Helper to generate dummy recipes
  List<Recipe> _generateDummyRecipes() {
    final recipes = [
      Recipe(
        author: AppState.currentUser!.username,
        title: 'My test recipe',
        description: 'Lorem ipsum...',
        steps: AppState.markdownTestText,
        id: 1,
        image: Image.asset('assets/images/logo.png'),
      ),
      Recipe(
        author: 'test_dummy',
        title: 'My test recipe',
        description: 'Lorem ipsum...',
        steps: AppState.markdownTestText,
        id: 2,
        image: Image.asset('assets/images/logo.png'),
      ),
    ];
    recipes.forEach((recipe) {
      recipe.editRecipe(
        ingredients: [
          Ingredient(id: 0, name: 'tomato', quantity: 2),
          Ingredient(id: 0, name: 'basil', quantity: 2, unit: 'teaspoons'),
          Ingredient(id: 0, name: 'flatbread', quantity: 3, unit: 'pieces'),
          Ingredient(id: 0, name: 'olive oil', quantity: 1, unit: 'tablespoon'),
          Ingredient(id: 0, name: 'salt', quantity: 1, unit: 'pinch'),
          Ingredient(id: 0, name: 'garlic', quantity: 0.5, unit: 'teaspoons'),
        ],
        tags: [
          Tag(isPredefined: true, name: 'basil', category: TagCategory.ingredient),
          Tag(isPredefined: true, name: 'bread', category: TagCategory.ingredient),
          Tag(isPredefined: true, name: 'tomato', category: TagCategory.ingredient),
          Tag(isPredefined: true, name: 'Italian', category: TagCategory.cuisine),
          Tag(isPredefined: false, name: 'no-bake', category: null),
          Tag(isPredefined: false, name: 'cheap', category: TagCategory.expense),
          Tag(isPredefined: true, name: 'fast', category: TagCategory.time),
        ],
      );
    });
    return recipes;
  }
}