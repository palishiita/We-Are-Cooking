import 'package:dish_discover/widgets/display/recipe_cover.dart';
//import 'package:dish_discover/widgets/display/user_avatar.dart';
import 'package:dish_discover/widgets/inputs/popup_menu.dart';
import 'package:dish_discover/widgets/pages/view_recipe.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/app_state.dart';
import '../../entities/new_recipe.dart';
import '../pages/user.dart';

class RecipeCard extends ConsumerStatefulWidget {
  final ChangeNotifierProvider<Recipe> recipeProvider;
  const RecipeCard({super.key, required this.recipeProvider});

  @override
  ConsumerState<RecipeCard> createState() => _RecipeCardState();
}

class _RecipeCardState extends ConsumerState<RecipeCard> {
  bool _isAddingToCookbook = false;
  bool _isInCookbook = false;
  bool _isLoadingCookbook = false;

  @override
  void initState() {
    super.initState();
    // Check cookbook status when the card is created
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _checkCookbookStatus();
    });
  }

  Future<void> _checkCookbookStatus() async {
    if (!mounted) return;
    
    print('Checking if in cookbook.');

    setState(() {
      _isLoadingCookbook = true;
    });

    try {
      Recipe recipe = ref.read(widget.recipeProvider);
      print('Checking if recipe ${recipe.name} in cookbook.');
      List<String> idsInCookbook = await Recipe.getPresentRecipeIdsForGivenIdsCookbook([recipe.id]);
      

      if (mounted) {
        setState(() {
          _isInCookbook = idsInCookbook.contains(recipe.id);
          print('The recipe ${recipe.name} ${_isInCookbook ? 'IS' : 'IS NOT'} in cookbook.');
          _isLoadingCookbook = false;
        });
      }
    } catch (e) {
      print('Error checking cookbook status: $e');
      if (mounted) {
        setState(() {
          _isLoadingCookbook = false;
        });
      }
    }
  }

  Future<void> _removeFromCookbook(String recipeId) async {
    try {
      bool success = await Recipe.removeRecipeFromCookbook(recipeId);
      if (success){
        await _checkCookbookStatus();
      }

      setState(() {
        _isAddingToCookbook = true;
      });

      if (mounted) {
        if (success) {
          // Show success message
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Recipe removed from cookbook!'),
              backgroundColor: Colors.green,
              duration: Duration(seconds: 2),
            ),
          );
        } 
        else {
          // Show error message
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Failed to remove recipe from cookbook'),
              backgroundColor: Colors.red,
              duration: Duration(seconds: 2),
            ),
          );
        }
      }
    } 
    catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Error removing recipe from cookbook'),
            backgroundColor: Colors.red,
            duration: Duration(seconds: 2),
          ),
        );
      }
    } 
    finally {
      if (mounted) {
        setState(() {
          _isAddingToCookbook = false;
        });
      }
    }
  }

  Future<void> _addToCookbook(Recipe recipe) async {
    if (_isAddingToCookbook) return; // Prevent multiple simultaneous calls
    
    setState(() {
      _isAddingToCookbook = true;
    });

    try {
      bool success = await Recipe.addRecipeToCookbook(recipe.id);
      if (success){
        await _checkCookbookStatus();
      }
      
      if (mounted) {
        if (success) {
          // Show success message
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Recipe added to cookbook!'),
              backgroundColor: Colors.green,
              duration: Duration(seconds: 2),
            ),
          );
        } else {
          // Show error message
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Failed to add recipe to cookbook'),
              backgroundColor: Colors.red,
              duration: Duration(seconds: 2),
            ),
          );
        }
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Error adding recipe to cookbook'),
            backgroundColor: Colors.red,
            duration: Duration(seconds: 2),
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isAddingToCookbook = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    Recipe recipe = ref.watch(widget.recipeProvider);

    // Updated to use new Recipe model properties
    String authorUsername = recipe.userData?.username ?? 'Not Found';
    String recipeTitle = recipe.name;

    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: GestureDetector(
            onTap: () => Navigator.of(context).push(MaterialPageRoute(
                builder: (context) => ViewRecipePage(
                    recipeId: recipe.id, recipeProvider: widget.recipeProvider))),
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
                                ? () => PopupMenuAction.editAction(context, recipe.id, widget.recipeProvider)
                                : () => print('Author is: $authorUsername and current user is: ${AppState.currentUser?.username}'))),
                      const Divider(height: 1.0),
                      Flexible(child: RecipeCover(cover: recipe.image)),
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 8.0, vertical: 4.0),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            IconButton(
                              onPressed: (_isAddingToCookbook || _isLoadingCookbook) 
                                ? null // Disable button while loading
                                : _isInCookbook 
                                  ? () => _removeFromCookbook(recipe.id)
                                  : () => _addToCookbook(recipe),
                              icon: _isAddingToCookbook || _isLoadingCookbook
                                ? SizedBox(
                                    width: 20,
                                    height: 20,
                                    child: CircularProgressIndicator(
                                      strokeWidth: 2,
                                      valueColor: AlwaysStoppedAnimation<Color>(
                                        Theme.of(context).primaryColor
                                      ),
                                    ),
                                  )
                                : Icon(
                                    _isInCookbook ? Icons.bookmark : Icons.bookmark_add,
                                    color: _isInCookbook 
                                      ? Theme.of(context).primaryColor 
                                      : Theme.of(context).primaryColor,
                                  ),
                              tooltip: _isAddingToCookbook 
                                ? 'Processing...' 
                                : _isLoadingCookbook
                                  ? 'Loading...'
                                  : _isInCookbook 
                                    ? 'Remove from cookbook' 
                                    : 'Add to cookbook',
                              iconSize: 24,
                            )
                          ],
                        ),
                      ),
                    ]
                  )
                )
              )
            )
          );
  }
}