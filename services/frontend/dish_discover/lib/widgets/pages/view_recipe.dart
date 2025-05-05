import 'package:dish_discover/widgets/inputs/popup_menu.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/app_state.dart';
import '../../entities/recipe.dart';
import '../display/loading_indicator.dart';
import '../display/recipe_cover.dart';
import '../display_with_input/comments_box.dart';
import '../display_with_input/ingredients_box.dart';
import '../display_with_input/recipe_header.dart';
import '../display_with_input/steps_box.dart';
import '../display_with_input/tags_box.dart';

class ViewRecipePage extends ConsumerStatefulWidget {
  static const routeName = "/recipe";
  final int recipeId;
  final ChangeNotifierProvider<Recipe>? recipeProvider;

  const ViewRecipePage(
      {super.key, required this.recipeId, this.recipeProvider});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() => _ViewRecipePageState();
}

class _ViewRecipePageState extends ConsumerState<ViewRecipePage> {
  ChangeNotifierProvider<Recipe>? recipeProvider;

  @override
  void initState() {
    super.initState();
    recipeProvider = widget.recipeProvider;
  }

  @override
  Widget build(BuildContext context) {
    return recipeProvider == null ? loading() : done();
  }

  Widget loading() {
    return FutureBuilder(
        future: Future<Recipe>(() => Recipe.getRecipe(widget.recipeId)),
        builder: (context, recipeData) {
          if (recipeData.connectionState != ConnectionState.done) {
            return LoadingIndicator(title: "Recipe #${widget.recipeId}");
          }

          Recipe recipe;
          if (recipeData.data == null) {
            if (kDebugMode) {
              recipe = Recipe(
                  id: widget.recipeId,
                  title: "recipe_${widget.recipeId}_debug",
                  author: "debug",
                  description:
                      "Testing testing testing testing testing testing testing.",
                  image: Image.asset("assets/images/logo.png"));
            } else {
              return LoadingErrorIndicator(title: "Recipe #${widget.recipeId}");
            }
          } else {
            recipe = recipeData.data!;
          }

          recipeProvider = ChangeNotifierProvider<Recipe>((ref) => recipe);

          return done();
        });
  }

  Widget done() {
    Recipe recipe = ref.watch(recipeProvider!);

    return Scaffold(
        appBar: AppBar(
            toolbarHeight: 200,
            scrolledUnderElevation: 0.0,
            leading: const BackButton(),
            actions: [
              PopupMenu(
                  action1: PopupMenuAction.share,
                  onPressed1: () => PopupMenuAction.shareAction(
                      context,
                      "Sharing recipe",
                      "Have a look at this recipe: ",
                      recipe.getUrl()),
                  action2:
                      recipe.author.compareTo(AppState.currentUser!.username) ==
                              0
                          ? PopupMenuAction.edit
                          : AppState.currentUser!.isModerator
                              ? PopupMenuAction.ban
                              : PopupMenuAction.report,
                  onPressed2: () => recipe.author
                              .compareTo(AppState.currentUser!.username) ==
                          0
                      ? PopupMenuAction.editAction(
                          context, recipe.id, recipeProvider!)
                      : AppState.currentUser!.isModerator
                          ? PopupMenuAction.banAction(
                              context, recipe.id, recipe.title, null, null, () {
                              Navigator.of(context)
                                  .popUntil((route) => route.isFirst);
                            })
                          : PopupMenuAction.reportAction(
                              context, recipe.id, recipe.title, null, null)),
            ],
            flexibleSpace: AspectRatio(
                aspectRatio: 4 / 3, child: RecipeCover(cover: recipe.image))),
        body: ListView(children: [
          RecipeHeader(recipeProvider: recipeProvider!),
          IngredientsBox(recipeProvider: recipeProvider!),
          StepsBox(recipeProvider: recipeProvider!),
          TagsBox(recipeProvider: recipeProvider!),
          CommentsBox(recipeProvider: recipeProvider!)
        ]));
  }
}
