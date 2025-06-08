import 'package:dish_discover/widgets/display/recipe_cover.dart';
import 'package:dish_discover/widgets/display/user_avatar.dart';
import 'package:dish_discover/widgets/inputs/popup_menu.dart';
import 'package:dish_discover/widgets/pages/view_recipe.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/app_state.dart';
import '../../entities/new_recipe.dart';
import '../pages/user.dart';

class RecipeCard extends ConsumerWidget {
  final ChangeNotifierProvider<Recipe> recipeProvider;
  const RecipeCard({super.key, required this.recipeProvider});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    //print('Recipe card build start.');
    Recipe recipe = ref.watch(recipeProvider);
    //print('Recipe on card: $recipe');
    
    // Updated to use new Recipe model properties
    String authorUsername = recipe.userData?.username ?? 'Unknown';
    String recipeTitle = recipe.name;

    //print('Recipe by $authorUsername named $recipeTitle');

    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: GestureDetector(
            onTap: () => Navigator.of(context).push(MaterialPageRoute(
                builder: (context) => ViewRecipePage(
                    recipeId: recipe.id, recipeProvider: recipeProvider))),
            child: AspectRatio(
                aspectRatio: 1.2,
                child: Card(
                    child: Flex(
                        direction: Axis.vertical,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                      ListTile(
                        leading: CircleAvatar(
                            radius: 15,
                            backgroundImage: recipe.authorAvatar?.image,
                            child: recipe.authorAvatar?.image == null 
                                ? Text(authorUsername.isNotEmpty ? authorUsername[0].toUpperCase() : 'U')
                                : null),
                          title: Text(recipeTitle,
                              softWrap: true, overflow: TextOverflow.ellipsis),
                          subtitle: GestureDetector(
                              onTap: () => Navigator.of(context).push(MaterialPageRoute(
                                  builder: (context) =>
                                      UserPage(username: authorUsername))),
                              child: Text(authorUsername, softWrap: true)),
                          trailing: PopupMenu(
                            action1: PopupMenuAction.share,
                            onPressed1: () => PopupMenuAction.shareAction(
                                context,
                                "Sharing recipe",
                                "Have a look at this recipe: ",
                                recipe.getUrl()),
                            action2: PopupMenuAction.edit,
                            onPressed2: authorUsername == (AppState.currentUser?.username ?? '')
                                ? () => PopupMenuAction.editAction(context, recipe.id, recipeProvider)
                                : () => print('Author is: $authorUsername and current user is: ${AppState.currentUser?.username}'))),
                      const Divider(height: 1.0),
                      Flexible(child: RecipeCover(cover: recipe.image)),
                      // Removed like/save/tags section
                    ])))));
  }
}