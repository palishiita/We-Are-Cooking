import 'package:dish_discover/entities/ingredient.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../../entities/app_state.dart';
import '../../../entities/recipe.dart';
import '../../../entities/tag.dart';
import '../../display/recipe_list.dart';
import '../../display/tab_title.dart';

class SavedTab extends StatelessWidget {
  const SavedTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Flex(
        direction: Axis.vertical,
        mainAxisSize: MainAxisSize.min, children: [
      const TabTitle(title: 'Saved'),
      RecipeList(
          getRecipes: () => Future<List<Recipe>>(() {
                List<Recipe> recipes = AppState.currentUser!.savedRecipes;
                if (kDebugMode && recipes.isEmpty) {
                  recipes = [
                    Recipe(
                        author: AppState.currentUser!.username, // 'test_dummy',
                        title: 'My test recipe',
                        description:
                            'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam scelerisque consectetur massa nec pretium. Vivamus faucibus ipsum dolor. Donec tempus lacus id arcu sagittis, sollicitudin pulvinar orci pretium.',
                        steps: AppState.markdownTestText,
                        id: 1,
                        image: Image.asset('assets/images/logo.png')),
                    Recipe(
                        author: 'test_dummy',
                        title: 'My test recipe',
                        description:
                            'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam scelerisque consectetur massa nec pretium. Vivamus faucibus ipsum dolor. Donec tempus lacus id arcu sagittis, sollicitudin pulvinar orci pretium.',
                        steps: AppState.markdownTestText,
                        id: 2,
                        image: Image.asset('assets/images/logo.png'))
                  ];
                  recipes[0].editRecipe(ingredients: [
                    Ingredient(id: 0, name: 'tomato', quantity: 2),
                    Ingredient(
                        id: 0, name: 'basil', quantity: 2, unit: 'teaspoons'),
                    Ingredient(
                        id: 0, name: 'flatbread', quantity: 3, unit: 'pieces'),
                    Ingredient(
                        id: 0,
                        name: 'olive oil',
                        quantity: 1,
                        unit: 'tablespoon'),
                    Ingredient(id: 0, name: 'salt', quantity: 1, unit: 'pinch'),
                    Ingredient(
                        id: 0, name: 'garlic', quantity: 0.5, unit: 'teaspoons')
                  ], tags: [
                    Tag(
                        isPredefined: true,
                        name: 'basil',
                        category: TagCategory.ingredient),
                    Tag(
                        isPredefined: true,
                        name: 'bread',
                        category: TagCategory.ingredient),
                    Tag(
                        isPredefined: true,
                        name: 'tomato',
                        category: TagCategory.ingredient),
                    Tag(
                        isPredefined: true,
                        name: 'Italian',
                        category: TagCategory.cuisine),
                    Tag(isPredefined: false, name: 'no-bake', category: null),
                    Tag(
                        isPredefined: false,
                        name: 'cheap',
                        category: TagCategory.expense),
                    Tag(
                        isPredefined: true,
                        name: 'fast',
                        category: TagCategory.time)
                  ]);

                  recipes[1].editRecipe(ingredients: [
                    Ingredient(id: 0, name: 'tomato', quantity: 2),
                    Ingredient(
                        id: 0, name: 'basil', quantity: 2, unit: 'teaspoons'),
                    Ingredient(
                        id: 0, name: 'flatbread', quantity: 3, unit: 'pieces'),
                    Ingredient(
                        id: 0,
                        name: 'olive oil',
                        quantity: 1,
                        unit: 'tablespoon'),
                    Ingredient(id: 0, name: 'salt', quantity: 1, unit: 'pinch'),
                    Ingredient(
                        id: 0, name: 'garlic', quantity: 0.5, unit: 'teaspoons')
                  ], tags: [
                    Tag(
                        isPredefined: true,
                        name: 'basil',
                        category: TagCategory.ingredient),
                    Tag(
                        isPredefined: true,
                        name: 'bread',
                        category: TagCategory.ingredient),
                    Tag(
                        isPredefined: true,
                        name: 'tomato',
                        category: TagCategory.ingredient),
                    Tag(
                        isPredefined: true,
                        name: 'Italian',
                        category: TagCategory.cuisine),
                    Tag(isPredefined: false, name: 'no-bake', category: null),
                    Tag(
                        isPredefined: false,
                        name: 'cheap',
                        category: TagCategory.expense),
                    Tag(
                        isPredefined: true,
                        name: 'fast',
                        category: TagCategory.time)
                  ]);
                }
                return recipes;
              }))
    ]);
  }
}
