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
    Recipe recipe = ref.watch(recipeProvider);
    
    // Updated to use new Recipe model properties
    String authorUsername = recipe.userData?.username ?? 'Unknown';
    String recipeTitle = recipe.name;

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
                          leading: UserAvatar(
                              username: authorUsername,
                              image: recipe.authorAvatar, // This might need updating too
                              diameter: 30),
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
                              action2: authorUsername ==
                                      AppState.currentUser!.username
                                  ? PopupMenuAction.edit
                                  : AppState.currentUser!.isModerator
                                      ? PopupMenuAction.ban
                                      : PopupMenuAction.report,
                              onPressed2: () => authorUsername == AppState.currentUser!.username
                                  ? PopupMenuAction.editAction(context, recipe.id, recipeProvider)
                                  : {})),
                                  //: AppState.currentUser!.isModerator
                                  //    ? PopupMenuAction.banAction(
                                  //        context,
                                  //        recipe.id,
                                  //        recipeTitle,
                                  //        null,
                                  //        null,
                                  //        () { // TODO
                                  //          })
                                  //    : PopupMenuAction.reportAction(
                                  //        context, recipe.id, recipeTitle, null, null))),
                      const Divider(height: 1.0),
                      Flexible(child: RecipeCover(cover: recipe.image)),
                      // Removed like/save/tags section
                    ])))));
  }
}