import 'dart:convert';

import 'package:dish_discover/entities/tag.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import 'app_state.dart';
//import 'ingredient.dart';
//import 'recipe_ingredient_data.dart';

// DTO classes for saving
class AddRecipeWithIngredientsDTO {
  final String name;
  final String description;
  final List<AddIngredientToRecipeDTO> ingredients;

  AddRecipeWithIngredientsDTO({
    required this.name,
    required this.description,
    required this.ingredients,
  });

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'description': description,
      'ingredients': ingredients.map((ingredient) => ingredient.toJson()).toList(),
    };
  }
}

class AddIngredientToRecipeDTO {
  final String ingredientId;
  final double quantity;
  final String unitId;

  AddIngredientToRecipeDTO({
    required this.ingredientId,
    required this.quantity,
    required this.unitId,
  });

  Map<String, dynamic> toJson() {
    return {
      'ingredientId': ingredientId,
      'quantity': quantity,
      'unitId': unitId,
    };
  }
}

// Response wrapper for paginated recipe data
class RecipeResponse {
  final List<Recipe> data;
  final int totalElements;
  final int totalPages;
  final int page;
  final int pageSize;

  RecipeResponse({
    required this.data,
    required this.totalElements,
    required this.totalPages,
    required this.page,
    required this.pageSize,
  });

  factory RecipeResponse.fromJson(Map<String, dynamic> json) {
    //print('Json data: $json');
    return RecipeResponse(
      data: (json['data'] as List)
          .map((item) => Recipe.fromJson(item))
          .toList(),
      totalElements: json['totalElements'] ?? 0,
      totalPages: json['totalPages'] ?? 1,
      page: json['page'] ?? 1,
      pageSize: json['pageSize'] ?? 1,
    );
  }
}

// User data class for recipe author information
class UserData {
  final String userId;
  final String username;
  final String? imageUrl;

  UserData({
    required this.userId,
    required this.username,
    this.imageUrl,
  });

  factory UserData.fromJson(Map<String, dynamic> json) {
    return UserData(
      userId: json['userId']?.toString() ?? '',
    username: json['username']?.toString() ?? '',
    imageUrl: json['imageUrl']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'userId': userId,
      'username': username,
      'imageUrl': imageUrl,
    };
  }
}

// Enhanced ingredient class for recipe ingredients
class RecipeIngredient {
  final String ingredientId;
  final String name;
  String? description;
  final double quantity;
  final String unitId;
  final String unit;

  RecipeIngredient({
    required this.ingredientId,
    required this.name,
    this.description,
    required this.quantity,
    required this.unitId,
    required this.unit,
  });

  factory RecipeIngredient.fromJson(Map<String, dynamic> json) {
    return RecipeIngredient(
      ingredientId: json['ingredientId'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      quantity: (json['quantity'] ?? 0).toDouble(),
      unitId: json['unitId'] ?? '',
      unit: json['unit'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'ingredientId': ingredientId,
      'name': name,
      'description': description,
      'quantity': quantity,
      'unitId': unitId,
      'unit': unit,
    };
  }
}

class Recipe extends ChangeNotifier {
  final String id;
  String name;
  String description;
  Image? image;
  List<RecipeIngredient> ingredients;
  UserData? userData;
  bool? isReadFromDB;

  Recipe({
    required this.id,
    this.name = '',
    this.description = '',
    this.userData,
    this.isReadFromDB
  }) : ingredients = [];

  // Legacy getter for backward compatibility
  String get author => userData?.username ?? '';
  Image? get authorAvatar => null; // This would need to be loaded from userData.imageUrl
  String get title => name;

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'ingredients': ingredients.map((e) => e.toJson()).toList(),
      'userData': userData?.toJson(),
      // Legacy fields for backward compatibility
    };
  }

  factory Recipe.fromJson(Map<String, dynamic> json) {
    final recipe = Recipe(
      id: json['id'] ?? json['recipe_id'] ?? '',
      name: json['name'] ?? json['recipe_name'] ?? '',
      description: json['description'] ?? '',
      userData: json['userData'] != null 
          ? UserData.fromJson(json['userData']) 
          : UserData(userId: '00000000-0000-0000-0000-000000000000', username: 'User not found'),
      isReadFromDB: true
    );
    // Parse ingredients
    if (json['ingredients'] != null) {
      recipe.ingredients = (json['ingredients'] as List)
          .map((item) => RecipeIngredient.fromJson(item))
          .toList();
      
      // Auto-generate ingredient tags
      //for (var ingredient in recipe.ingredients) {
      //  recipe.tags.add(Tag(
      //    isPredefined: false,
      //    name: ingredient.name,
      //    category: TagCategory.ingredient,
      //  ));
      //}
    }

    return recipe;
  }

  void editRecipe({
    String? name,
    String? title, // Legacy parameter
    String? description,
    String? content,
    Image? image,
    String? steps,
    List<RecipeIngredient>? ingredients,
    List<Tag>? tags,
  }) {
    this.name = name ?? title ?? this.name;
    this.description = description ?? this.description;
    this.image = image ?? this.image;
    this.ingredients = ingredients ?? this.ingredients;
    notifyListeners();
  }

  void addIngredient(RecipeIngredient ingredient) {
    ingredients.add(ingredient);
    notifyListeners();
  }

  void removeIngredient(RecipeIngredient ingredient) {
    ingredients.remove(ingredient);
    notifyListeners();
  }

  void updateIngredient(int index, RecipeIngredient ingredient) {
    if (index < 0 || index >= ingredients.length) {
      return;
    }

    ingredients[index] = ingredient;
    notifyListeners();
  }

  Map<String, dynamic> getAuthor() {
    return {
      'authorId': userData?.userId ?? '',
      'username': userData?.username ?? '',
      'image': userData?.imageUrl,
    };
  }

  String getUrl() {
    return "http://${AppState.clientDomain}/recipes/recipe/$id/full";
  }

  // Enhanced getRecipes method with proper parameter handling
  static Future<RecipeResponse> getRecipes({
    String? query,
    int count = 10,
    int page = 0,
    String? sortBy,
    bool orderByAsc = true,
  }) async {
    try {
      // Build query parameters
      final Map<String, String> queryParams = {
        'count': count.toString(),
        'page': page.toString(),
      };

      Map<String, String> requestHeaders = {
       'Content-type': 'application/json',
       'Accept': 'application/json',
       'X-Uuid' : AppState.currentUser == null ? '00000000-0000-0000-0000-000000000000' : AppState.currentUser!.userId
     };

      if (query != null && query.isNotEmpty) {
        queryParams['query'] = query;
      }
      if (sortBy != null && sortBy.isNotEmpty) {
        queryParams['sortBy'] = sortBy;
      }
      queryParams['orderByAsc'] = orderByAsc.toString();
      //final uri = Uri.http(
      //  AppState.serverDomain,
      //  '/api/recipes/recipes/full',
      //  queryParams
      //);

      final uri = Uri.http(
        'localhost:7140',
        '/api/recipes/recipes/full',
        queryParams
      );

      final response = await http.get(uri);
      //final response = await http.get(uri, headers: requestHeaders);

      if (response.statusCode == 200) {
        final jsonData = json.decode(response.body);
        return RecipeResponse.fromJson(jsonData);
      } else {
        throw Exception(
            'Failed to load recipes, status code: ${response.statusCode} - ${response.reasonPhrase}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Error fetching recipes: $e');
      }
      throw Exception('Failed to load recipes: $e');
    }
  }

  // Legacy method for backward compatibility
  static Future<List<Recipe>> getRecipesLegacy(
      String? searchPhrase, String? query, int count, int page) async {
    final response = await getRecipes(query: query ?? searchPhrase, count: count, page: page);
    return response.data;
  }

  static Future<Recipe> getRecipe(String recipeId) async {
    try {

      Map<String, String> requestHeaders = {
       'Content-type': 'application/json',
       'Accept': 'application/json',
       'X-Uuid' : AppState.currentUser == null ? '00000000-0000-0000-0000-000000000000' : AppState.currentUser!.userId
     };

      final response = await http.get(Uri.parse(
          'http://localhost:7140/api/recipes/recipes/$recipeId/full'));
          //'http://${AppState.serverDomain}/api/recipes/recipes/$recipeId'));

      if (response.statusCode == 200) {
        final Map<String, dynamic> data = json.decode(response.body);
        // Handle both new format and legacy format
        if (data.containsKey('data')) {
          return Recipe.fromJson(data['data']);
        } else if (data.containsKey('recipe')) {
          return Recipe.fromJson(data['recipe']);
        } else {
          return Recipe.fromJson(data);
        }
      } else {
        throw Exception(
            'Failed to load recipe, status code: ${response.statusCode} - ${response.reasonPhrase}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Error fetching recipe: $e');
      }
      throw Exception('Failed to load recipe: $e');
    }
  }

  //static Future<void> saveRecipe(Recipe recipe) async {
  //  try {
  //    final response = await http.post(
  //      Uri.parse(
  //          'http://${AppState.serverDomain}/api/recipes/recipes'),
  //      headers: <String, String>{
  //        'Content-Type': 'application/json; charset=UTF-8',
  //      },
  //      body: jsonEncode(recipe.toJson()),
  //    );

  //    if (response.statusCode == 201 || response.statusCode == 200) {
  //      if (kDebugMode) {
  //        print('Recipe saved successfully');
  //      }
  //    } else {
  //      throw Exception(
  //          'Failed to save recipe, status code: ${response.statusCode} - ${response.reasonPhrase}');
  //    }
  //  } catch (e) {
  //    if (kDebugMode) {
  //      print('Error saving recipe: $e');
  //    }
  //    throw Exception('Failed to save recipe: $e');
  //  }
  //}

  static Future<void> deleteRecipe(String recipeId) async {
    try
    {
      var finalUrl = Uri.parse('http://localhost:7140/api/recipes/recipe/$recipeId');

      await http.delete(
        //Uri.parse('http://${AppState.serverDomain}/api/recipes/recipes'),
        finalUrl,
        headers: {
          'Content-Type': 'application/json',
          'X-Uuid' : AppState.currentUser?.userId ?? '00000000-0000-0000-0000-000000000000'
        });
    } catch (e) {
      print('Error deleting recipe: $e');
    }
  }

  static Future<bool> saveRecipe(Recipe recipe) async {
    try {

      http.Response response;

      if (recipe.isReadFromDB!){
        print('Update recipe.');
        // var finalUrl = Uri.parse('http://${AppState.serverDomain}/api/recipes/recipe/${recipe.id}'),
        var finalUrl = Uri.parse('http://localhost:7140/api/recipes/recipe/${recipe.id}');
        response = await _addRecipe(finalUrl, recipe, false);
      }
      else{
        // var finalUrl = Uri.parse('http://${AppState.serverDomain}/api/recipes/recipes'),
        print('Add recipe.');
        var finalUrl = Uri.parse('http://localhost:7140/api/recipes/recipe');
        response = await _addRecipe(finalUrl, recipe, true);
      }

      if (response.statusCode == 200 || response.statusCode == 201) {
        return true;
      } else {
        print('Failed to save recipe: ${response.statusCode}');
        print('Response body: ${response.body}');
        return false;
      }
    } catch (e) {
      print('Error saving recipe: $e');
      return false;
    }
  }

  // return status code
  static Future<http.Response> _addRecipe(Uri finalUrl, Recipe recipe, bool isPost) async {
    // Convert RecipeIngredients to AddIngredientToRecipeDTO
    List<AddIngredientToRecipeDTO> ingredientDTOs = recipe.ingredients
        .map((ingredient) => AddIngredientToRecipeDTO(
              ingredientId: ingredient.ingredientId,
              quantity: ingredient.quantity,
              unitId: ingredient.unitId,
            ))
        .toList();

    // Create the main DTO
    AddRecipeWithIngredientsDTO recipeDTO = AddRecipeWithIngredientsDTO(
      name: recipe.name,
      description: recipe.description,
      ingredients: ingredientDTOs,
    );

    // Make the API call
    if (isPost){
      return await http.post(
        //Uri.parse('http://${AppState.serverDomain}/api/recipes/recipes'),
        finalUrl,
        headers: {
          'Content-Type': 'application/json'
        },
        body: jsonEncode(recipeDTO.toJson()),
      );
    }else{
      print('JSON sent to update: ${jsonEncode(recipeDTO.toJson())}');
      return await http.put(
        //Uri.parse('http://${AppState.serverDomain}/api/recipes/recipes'),
        finalUrl,
        headers: {
          'Content-Type': 'application/json'
        },
        body: jsonEncode(recipeDTO.toJson()),
      );
    }

  }

  //static Future<http.Response> _editRecipe(Uri finalUrl, Recipe recipe) async {
  //  // Convert RecipeIngredients to AddIngredientToRecipeDTO
  //  List<AddIngredientToRecipeDTO> ingredientDTOs = recipe.ingredients
  //      .map((ingredient) => AddIngredientToRecipeDTO(
  //            ingredientId: ingredient.ingredientId,
  //            quantity: ingredient.quantity,
  //            unitId: ingredient.unitId,
  //          ))
  //      .toList();
//
  //  // Create the main DTO
  //  AddRecipeWithIngredientsDTO recipeDTO = AddRecipeWithIngredientsDTO(
  //    name: recipe.name,
  //    description: recipe.description,
  //    ingredients: ingredientDTOs,
  //  );
//
  //  // Make the API call
  //  final response = await http.post(
  //    finalUrl,
  //    headers: {
  //      'Content-Type': 'application/json'
  //    },
  //    body: jsonEncode(recipeDTO.toJson()),
  //  );
//
  //  return response;
  //}
}