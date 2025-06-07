
// import 'dart:convert';
// import 'package:http/http.dart' as http;
// import 'package:flutter/material.dart';

// import '../../../entities/app_state.dart';
// import '../../../entities/ingredient.dart';
// import '../../../entities/new_recipe.dart';
// import '../../dialogs/custom_dialog.dart';
// import '../../inputs/custom_text_field.dart';
// import '../edit_recipe.dart';
// import '../../display/recipe_list.dart';
// import '../../display/tab_title.dart';

// class FridgeTab extends StatefulWidget {
//   const FridgeTab({super.key});

//   @override
//   State<FridgeTab> createState() => _FridgeTabState();
// }

// class _FridgeTabState extends State<FridgeTab> {
//   final TextEditingController _ingredientController = TextEditingController();

//   void _addIngredient(String ingredientName) {
//     if (ingredientName.trim().isEmpty) return;

//     final exists = AppState.currentUser!.fridgeIngredients.any(
//       (i) => i.name.toLowerCase() == ingredientName.trim().toLowerCase(),
//     );
//     if (exists) return;

//     final newIngredient = Ingredient(
//       id: DateTime.now().millisecondsSinceEpoch,
//       name: ingredientName.trim(),
//       quantity: 1,
//     );

//     setState(() {
//       AppState.currentUser!.addFridgeIngredient(newIngredient);
//       _ingredientController.clear();
//     });
//   }

//   void _removeIngredient(Ingredient ingredient) {
//     setState(() {
//       AppState.currentUser!.removeFridgeIngredient(ingredient);
//     });
//   }

//   // Converted to return Future<List<Recipe>>
//   Future<RecipeResponse> _getRecipesFromFridge({
//   int page = 0,
//   int pageSize = 10,
// }) async {
//   try {
//     // Build query parameters
//     final Map<String, String> queryParams = {
//       'page': page.toString(),
//       'pageSize': pageSize.toString(),
//     };

//     // Make API call to your fridge recipes endpoint
//     final uri = Uri.http(
//       AppState.serverDomain,
//       '/api/recipes/fridge-recipes/', // Adjust this path to your actual endpoint
//       queryParams,
//     );

//     final response = await http.get(uri);

//     if (response.statusCode == 200) {
//       final jsonData = json.decode(response.body);
//       // Parse directly using your existing RecipeResponse.fromJson
//       return RecipeResponse.fromJson(jsonData);
//     } else {
//       throw Exception(
//           'Failed to load fridge recipes, status code: ${response.statusCode} - ${response.reasonPhrase}');
//     }
//   } catch (e) {
//     //if (kDebugMode) {
//     //  print('Error fetching fridge recipes: $e');
//     //}
//     throw Exception('Failed to load fridge recipes: $e');
//   }
// }

//   @override
//   Widget build(BuildContext context) {
//     final fridgeIngredients = AppState.currentUser!.fridgeIngredients;

//     return Scaffold(
//       body: Padding(
//         padding: const EdgeInsets.all(16),
//         child: Column(
//           children: [
//             const TabTitle(title: 'Fridge Ingredients'),
//             Row(
//               children: [
//                 Expanded(
//                   child: TextField(
//                     controller: _ingredientController,
//                     decoration: const InputDecoration(
//                       labelText: 'Add ingredient',
//                       border: OutlineInputBorder(),
//                     ),
//                     onSubmitted: _addIngredient,
//                   ),
//                 ),
//                 const SizedBox(width: 8),
//                 ElevatedButton(
//                   onPressed: () => _addIngredient(_ingredientController.text),
//                   child: const Text('Add'),
//                 ),
//               ],
//             ),
//             const SizedBox(height: 12),
//             SingleChildScrollView(
//               scrollDirection: Axis.horizontal,
//               child: Row(
//                 children: fridgeIngredients
//                     .map((ingredient) => Padding(
//                           padding: const EdgeInsets.symmetric(horizontal: 4),
//                           child: Chip(
//                             label: Text(ingredient.name),
//                             deleteIcon: const Icon(Icons.close),
//                             onDeleted: () => _removeIngredient(ingredient),
//                           ),
//                         ))
//                     .toList(),
//               ),
//             ),
//             const SizedBox(height: 12),
//             // Info message will update when recipes are fetched in RecipeList
//             const Text(
//               'Recipes will appear below based on your fridge ingredients.',
//               style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
//             ),
//             const SizedBox(height: 12),
//             Expanded(
//               child: RecipeList(
//                 getRecipes: _getRecipesFromFridge, // Now correctly async
//               ),
//             ),
//           ],
//         ),
//       ),
//       floatingActionButton: FloatingActionButton(
//         shape: const CircleBorder(),
//         mini: true,
//         child: const Icon(Icons.add),
//         onPressed: () {
//           TextEditingController titleController = TextEditingController();

//               CustomDialog.callDialog(
//                 context,
//                 'Create recipe',
//                 '',
//                 null,
//                 CustomTextField(controller: titleController, hintText: 'Title'),
//                 'Create',
//                 () {
//                   if (titleController.text.trim().isNotEmpty) {
//                     Recipe newRecipe = Recipe(
//                       id: '00000000-0000-0000-0000-000000000000',
//                       name: titleController.text,
//                       userData: UserData(userId: AppState.currentUser!.userId, username: AppState.currentUser!.username)
//                     );
//                     AppState.currentUser!.addRecipe(newRecipe);
//                     Future.microtask(() => Navigator.of(context).push(
//                           MaterialPageRoute(
//                             builder: (context) =>
//                                 EditRecipePage(recipeId: newRecipe.id),
//                           ),
//                         ));
//                     return null;
//                   } else {
//                     return "Title cannot be empty";
//                   }
//                 },
//               );
//             },
//           );
//         },
//       ),
//     );
//   }
// }
