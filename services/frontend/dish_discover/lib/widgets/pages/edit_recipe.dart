import 'dart:io';

import 'package:dish_discover/widgets/inputs/popup_menu.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/recipe.dart';
import '../display/loading_indicator.dart';
import '../display/recipe_cover.dart';
import '../display_with_input/ingredients_box.dart';
import '../display_with_input/recipe_header.dart';
import '../display_with_input/steps_box.dart';
import '../display_with_input/tags_box.dart';

class EditRecipePage extends ConsumerStatefulWidget {
  static const routeName = "/edit";
  final int recipeId;
  final ChangeNotifierProvider<Recipe>? recipeProvider;

  const EditRecipePage(
      {super.key, required this.recipeId, this.recipeProvider});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() => _EditRecipePageState();
}

class _EditRecipePageState extends ConsumerState<EditRecipePage> {
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
    Recipe recipe = ref.read(recipeProvider!);

    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            leading: const BackButton(),
            actions: [
              PopupMenu(
                  action1: PopupMenuAction.boost,
                  onPressed1: recipe.isBoosted
                      ? null
                      : () => PopupMenuAction.boostAction(context),
                  action2: PopupMenuAction.delete,
                  onPressed2: () =>
                      PopupMenuAction.deleteAction(context, recipe.id))
            ],
            flexibleSpace: AspectRatio(
                aspectRatio: 4 / 3,
                child: Stack(
                    alignment: AlignmentDirectional.bottomCenter,
                    children: [
                      RecipeCover(cover: recipe.image),
                      FloatingActionButton(
                          shape: const CircleBorder(),
                          mini: true,
                          backgroundColor: containerColor(context),
                          child: Icon(Icons.edit_outlined, color: buttonColor),
                          onPressed: () async {
                            await FilePicker.platform
                                .pickFiles(
                                    dialogTitle: 'Please select an image:',
                                    type: FileType.image)
                                .then((res) {
                              String? path = res?.files[0].path;
                              if (path != null) {
                                setState(() {
                                  recipe.editRecipe(
                                      image: Image.file(File(path)));
                                });
                              }
                            });
                          })
                    ]))),
        body: ListView(children: [
          RecipeHeader(recipeProvider: recipeProvider!, forEditing: true),
          IngredientsBox(recipeProvider: recipeProvider!, forEditing: true),
          StepsBox(recipeProvider: recipeProvider!, forEditing: true),
          TagsBox(recipeProvider: recipeProvider!, forEditing: true)
        ]));
  }
}
