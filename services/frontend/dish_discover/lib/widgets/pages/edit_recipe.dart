import 'dart:io';

import 'package:dish_discover/entities/app_state.dart';
import 'package:dish_discover/widgets/inputs/popup_menu.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/new_recipe.dart';
import '../display/loading_indicator.dart';
import '../display/recipe_cover.dart';
import '../display_with_input/ingredients_box.dart';
import '../display_with_input/recipe_header.dart';
import '../display_with_input/steps_box.dart';
import '../display_with_input/tags_box.dart';

class EditRecipePage extends ConsumerStatefulWidget {
  static const routeName = "/edit";
  final String recipeId;
  final ChangeNotifierProvider<Recipe>? recipeProvider;

  const EditRecipePage(
      {super.key, required this.recipeId, this.recipeProvider});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() => _EditRecipePageState();
}

class _EditRecipePageState extends ConsumerState<EditRecipePage> {
  ChangeNotifierProvider<Recipe>? recipeProvider;

Future<void> _saveRecipe() async {
  Recipe recipe = ref.read(recipeProvider!);
  
  if(recipe.id == null || recipe.id.isEmpty || recipe.id == '00000000-0000-0000-0000-000000000000') {
    recipe.isReadFromDB = false;
  }
  else
  {
    print('Recipe id: ${recipe.id}');
    recipe.isReadFromDB = true;
  }
  // Validate that recipe has required fields
  if (recipe.name.trim().isEmpty) {
    _showErrorDialog('Recipe name is required');
    return;
  }
  
  if (recipe.ingredients.isEmpty) {
    _showErrorDialog('At least one ingredient is required');
    return;
  }
  
  // Show loading indicator
  showDialog(
    context: context,
    barrierDismissible: false,
    builder: (BuildContext context) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    },
  );
  
  try {
      bool success = await Recipe.saveRecipe(recipe);
      
      // Close loading dialog
      Navigator.of(context).pop();
      
      if (success) {
        _showSuccessDialog('Recipe saved successfully!');
      } else {
        _showErrorDialog('Failed to save recipe. Please try again.');
      }
    } catch (e) {
      // Close loading dialog
      Navigator.of(context).pop();
      _showErrorDialog('An error occurred while saving the recipe.');
    }
  }


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
                  name: "recipe_${widget.recipeId}_debug",
                  userData: AppState.currentUser == null 
                    ? UserData(userId: '00000000-0000-0000-0000-000000000000', username: 'Debug') 
                    : UserData(userId: AppState.currentUser!.userId, username: AppState.currentUser!.username),
                  description: "Testing testing testing testing testing testing testing.",
                  isReadFromDB: false);
            print('Recipe is NOT read from DB.');
            } else {
              return LoadingErrorIndicator(title: "Recipe #${widget.recipeId}");
            }
          } else {
            recipe = recipeData.data!;
            recipe.isReadFromDB = true;
            print('Recipe is read from DB.');
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
              IconButton(
              icon: const Icon(Icons.save),
              onPressed: _saveRecipe,  // Add this line
              ),
              IconButton(
                icon: const Icon(Icons.delete),
                onPressed: () => PopupMenuAction.deleteAction(context, recipe.id),
  )
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
          //StepsBox(recipeProvider: recipeProvider!, forEditing: true),
          //TagsBox(recipeProvider: recipeProvider!, forEditing: true)
        ]));
  }

  void _showErrorDialog(String message) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Error'),
          content: Text(message),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('OK'),
            ),
          ],
        );
      },
    );
  }

  void _showSuccessDialog(String message) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Success'),
          content: Text(message),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop(); // Close dialog
                Navigator.of(context).pop(); // Go back to previous screen
              },
              child: const Text('OK'),
            ),
          ],
        );
      },
    );
  }
}
