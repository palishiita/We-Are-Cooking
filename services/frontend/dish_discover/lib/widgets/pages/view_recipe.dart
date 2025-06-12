import 'package:dish_discover/widgets/inputs/popup_menu.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/app_state.dart';
import '../../entities/new_recipe.dart';
import '../display/loading_indicator.dart';
import '../display/recipe_cover.dart';
import '../display_with_input/ingredients_box.dart';
import '../display_with_input/recipe_header.dart';

class ViewRecipePage extends ConsumerStatefulWidget {
  static const routeName = "/recipe";
  final String recipeId;
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
    print('Initializing recipe view.');
  }

  @override
  Widget build(BuildContext context) {
    return recipeProvider == null ? loading() : done();
  }

  Widget loading() {
    print('Loading recipe view.');
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
                  name: "recipe_${widget.recipeId}_debug",
                  userData: AppState.currentUser == null 
                    ? UserData(userId: '00000000-0000-0000-0000-000000000000', username: 'Debug') 
                    : UserData(userId: AppState.currentUser!.userId, username: AppState.currentUser!.username),
                  description: "Testing testing testing testing testing testing testing.",
                  isReadFromDB: false);
            } else {
              return LoadingErrorIndicator(title: "Recipe #${widget.recipeId}");
            }
          } 
          else 
          {
            print('Recipe data is ${recipeData.data.toString()}');
            recipe = recipeData.data!;
            recipe.isReadFromDB = true;
          }

          recipeProvider = ChangeNotifierProvider<Recipe>((ref) => recipe);

          // Use setState to update the widget after provider is set
          WidgetsBinding.instance.addPostFrameCallback((_) {
            setState(() {});
          });

          return const SizedBox.shrink(); // Return an empty widget while setState triggers rebuild
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
                IconButton(
              icon: const Icon(Icons.share),
              onPressed: () => PopupMenuAction.shareAction(
                  context,
                  "Sharing recipe",
                  "Have a look at this recipe: ",
                  recipe.getUrl()))
            ],
            flexibleSpace: AspectRatio(
                aspectRatio: 4 / 3, child: RecipeCover(cover: recipe.image))),
        body: ListView(children: [
          RecipeHeader(recipeProvider: recipeProvider!),
          IngredientsBox(recipeProvider: recipeProvider!),
          //StepsBox(recipeProvider: recipeProvider!),
          //TagsBox(recipeProvider: recipeProvider!),
          //CommentsBox(recipeProvider: recipeProvider!)
        ]));
  }
}
