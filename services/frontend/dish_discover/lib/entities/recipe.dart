import 'dart:convert';
import 'dart:math';

import 'package:dish_discover/entities/comment.dart';
import 'package:dish_discover/entities/tag.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import 'app_state.dart';
import 'ingredient.dart';

class Recipe extends ChangeNotifier {
  final int id;
  final String author;
  Image? authorAvatar;
  String title;
  String description;
  String steps;
  Image? image;
  bool isBoosted;
  List<Ingredient> ingredients;
  List<Tag> tags;
  List<Comment> comments;
  int likeCount = 0;
  int saveCount = 0;

  Recipe(
      {required this.id,
      required this.author,
      this.authorAvatar,
      this.title = '',
      this.description = '',
      this.steps = '',
      this.image,
      this.isBoosted = false})
      : ingredients = [],
        tags = [],
        comments = [];

  Map<String, dynamic> toJson() {
    return {
      'recipe_id': id,
      'recipe_name': title,
      'content': steps,
      'picture': image?.toString(),
      'description': description,
      'is_boosted': isBoosted,
      'author': author,
    };
  }

  factory Recipe.fromJson(Map<String, dynamic> json) {
    return Recipe(
      id: json['recipe_id'],
      title: json['recipe_name'],
      steps: json['content'],
      image: json['picture'],
      description: json['description'],
      isBoosted: json['is_boosted'],
      author: json['author'],
      // ingredients: List<int>.from(json['ingredients']),
      // tags: List<String>.from(json['tags']),
    );
  }

  void editRecipe(
      {String? title,
      String? description,
      String? content,
      Image? image,
      String? steps,
      List<Ingredient>? ingredients,
      List<Tag>? tags}) {
    this.title = title ?? this.title;
    this.description = description ?? this.description;
    this.image = image ?? this.image;
    this.steps = steps ?? this.steps;
    this.ingredients = ingredients ?? this.ingredients;
    this.tags = tags ?? this.tags;
    notifyListeners();
  }

  void addIngredient(Ingredient ingredient) {
    addTag(Tag(
        isPredefined: false,
        name: ingredient.name,
        category: TagCategory.ingredient));
    ingredients.add(ingredient);
    notifyListeners();
  }

  void removeIngredient(Ingredient ingredient) {
    removeTag(Tag(
        isPredefined: false,
        name: ingredient.name,
        category: TagCategory.ingredient));
    ingredients.remove(ingredient);
    notifyListeners();
  }

  void updateIngredient(int index, Ingredient ingredient) {
    if (index < 0 || index > ingredients.length) {
      return;
    }

    ingredients[index] = ingredient;
    notifyListeners();
  }

  void addTag(Tag tag) {
    if (tags.any((e) => e.name == tag.name)) {
      return;
    }
    tags.add(tag);
    notifyListeners();
  }

  void removeTag(Tag tag) {
    tags.removeWhere((e) => e.name == tag.name);
    notifyListeners();
  }

  void addComment(Comment comment) {
    comments.add(comment);
    notifyListeners();
  }

  void removeComment(Comment comment) {
    comments.remove(comment);
    notifyListeners();
  }

  void updateLikeCount(bool add) {
    likeCount = likeCount + (add ? 1 : -1);
    likeCount = max(likeCount, 0);
    notifyListeners();
  }

  void updateSaveCount(bool add) {
    saveCount = saveCount + (add ? 1 : -1);
    saveCount = max(saveCount, 0);
    notifyListeners();
  }

  Map<String, dynamic> getAuthor() {
    return {
      'authorId': author,
      'image': image,
    };
  }

  String getUrl() {
    return "http://${AppState.clientDomain}/recipe/$id";
  }

  static Future<List<Recipe>> getRecipes(String? searchPhrase) async {
    // TODO pass the searchPhrase
    final response = await http
        .get(Uri.parse('http://${AppState.serverDomain}/api/recipes/recipes/'));

    if (response.statusCode == 200) {
      final List data = json.decode(response.body)['recipe'];
      return data.map((item) => Recipe.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load items, status code: ${response.reasonPhrase}');
    }
  }

  static Future<Recipe> getRecipe(int recipeId) async {
    final response = await http.get(Uri.parse(
        'http://${AppState.serverDomain}/api/recipes/recipes/$recipeId'));

    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body)['recipe'];
      return Recipe.fromJson(data);
    } else {
      throw Exception(
          'Failed to load recipe, status code: ${response.reasonPhrase}');
    }
  }

  static Future<List<Ingredient>> getIngredientsForRecipe(int recipeId) async {
    final response = await http.get(Uri.parse(
        'http://${AppState.serverDomain}/api/recipes/$recipeId/ingredients'));

    if (response.statusCode == 200) {
      final List<dynamic> data = json.decode(response.body)['ingredients'];
      return data.map((item) => Ingredient.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load ingredients, status code: ${response.reasonPhrase}');
    }
  }

  static Future<void> saveRecipe(Recipe recipe) async {
    try {
      final response = await http.post(
        Uri.parse(
            'http://${AppState.serverDomain}/api/recipes/recipes/${recipe.id}'),
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: recipe.toJson(),
      );

      if (response.statusCode == 201) {
        if (kDebugMode) {
          print('Recipe saved successfully');
        }
      } else {
        throw Exception(
            'Failed to save recipe, status code: ${response.statusCode}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Error saving recipe: $e');
      }
    }
  }

  static List<Recipe> filterRecipes(List<Recipe> recipes, List<Tag> filter) {
    List<Recipe> filtered = [];

    for (Recipe recipe in recipes) {
      bool add = false;
      for (Tag tag in recipe.tags) {
        if (filter.contains(tag)) {
          add = true;
        }
      }

      if (add) {
        filtered.add(recipe);
      }
    }

    return filtered;
  }
}
