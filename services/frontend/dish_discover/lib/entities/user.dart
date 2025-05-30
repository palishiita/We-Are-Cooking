import 'dart:convert';

import 'package:dish_discover/entities/comment.dart';
import 'package:dish_discover/entities/ingredient.dart';
import 'package:dish_discover/entities/recipe.dart';
import 'package:dish_discover/entities/ticket.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

import 'app_state.dart';

class User extends ChangeNotifier {
  final String username;
  String password;
  String email;
  bool isPremium;
  Image? image;
  String description;
  DateTime? unbanDate;
  final bool isModerator;
  late List<Recipe> likedRecipes;
  late List<Recipe> savedRecipes;
  late List<Recipe> addedRecipes;
  int likesTotal;
  int savesTotal;

    // Fridge ingredients
  final List<Ingredient> _fridgeIngredients = [];
  List<Ingredient> get fridgeIngredients => _fridgeIngredients;

  User(
      {required this.username,
      required this.password,
      required this.email,
      this.isPremium = false,
      this.image,
      this.description = '',
      this.unbanDate,
      this.isModerator = false,
      this.likesTotal = 0,
      this.savesTotal = 0})
      : likedRecipes = [],
        savedRecipes = [],
        addedRecipes = [];

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      username: json['username'],
      isModerator: json['has_mod_rights'],
      email: json['email'],
      password: json['password'],
      image: json['picture'] != null ? Image.network(json['picture']) : null,
      description: json['description'],
      isPremium: json['is_premium'],
      unbanDate: json['unban_date'] != null
          ? DateTime.parse(json['unban_date'])
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'username': username,
      'has_mod_rights': isModerator,
      'email': email,
      'password': password,
      'picture': image?.toString(),
      'description': description,
      'is_premium': isPremium,
      'unban_date': unbanDate?.toIso8601String(),
    };
  }

  void banUser(User user, DateTime date) {
    if (isModerator == true) {
      user.unbanDate = date;
    }
    notifyListeners();
  }

  void switchLikeRecipe(Recipe recipe, bool add) {
    add
        ? likedRecipes.add(recipe)
        : likedRecipes.removeWhere((e) => e.id == recipe.id);
    recipe.updateLikeCount(add);
    notifyListeners();
  }

  void switchSaveRecipe(Recipe recipe, bool add) {
    add
        ? savedRecipes.add(recipe)
        : savedRecipes.removeWhere((e) => e.id == recipe.id);
    recipe.updateSaveCount(add);
    notifyListeners();
  }

  Recipe addRecipe(Recipe recipe) {
    addedRecipes.add(recipe);
    notifyListeners();
    return recipe;
  }

  void editProfile(
      {String? email, String? password, Image? image, String? description}) {
    this.email = email ?? this.email;
    this.password = password ?? this.password;
    this.image = image ?? this.image;
    this.description = description ?? this.description;
    notifyListeners();
  }

  void getPremium() {
    isPremium = true;
    notifyListeners();
  }

  // Add ingredient to fridge
  void addFridgeIngredient(Ingredient ingredient) {
    _fridgeIngredients.add(ingredient);
    notifyListeners();
  }

  // Remove ingredient from fridge
  void removeFridgeIngredient(Ingredient ingredient) {
    _fridgeIngredients.removeWhere((i) => i.name == ingredient.name);
    notifyListeners();
  }

  // Add a recipe to user's added recipes (void version, renamed to avoid conflict)
  void addRecipeToUser(Recipe recipe) {
    addedRecipes.add(recipe);
    notifyListeners();
  }

  Ticket reportUser(int reportId, Recipe? recipe, User issuer, User violator,
      Comment? comment, String reason) {
    Ticket ticket = Ticket(
        reportId: reportId,
        recipeId: recipe?.id,
        violatorId: violator.username,
        issuerId: issuer.username,
        commentId: comment?.id,
        reason: reason);
    notifyListeners();
    return ticket;
  }

  Comment addComment(int commentId, Recipe recipe, String content) {
    Comment comment = Comment(
        author: username, recipeId: recipe.id, id: commentId, content: content);
    recipe.comments.add(comment);
    notifyListeners();
    recipe.notifyListeners();
    return comment;
  }

  void editComment(Comment comment, String content) {
    if (username == comment.author) {
      comment.content = content;
      notifyListeners();
    }
  }

  Future<List<Recipe>> getRecommendations() async {
    // TODO
    return [];
  }

  String getUrl() {
    return "http://${AppState.clientDomain}/user/$username";
  }

  static Future<String?> register(
      String username, String password, String email) async {
    try {
      final http.Response response = await http.post(
        Uri.parse('http://${AppState.serverDomain}/api/auth/register'),
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode({
          'username': username,
          'password': password,
          'email': email,
        }),
      );

      if (response.statusCode == 200) {
        if (kDebugMode) {
          print("User registered successfully");
        }
        return null;
      } else {
        if (kDebugMode) {
          print('Failed to register user, status code: ${response.statusCode}');
        }

        return response.reasonPhrase ?? 'No reason given.';
      }
    } catch (e) {
      if (kDebugMode) {
        print('Failed to login user: ${e.toString()}');
      }

      return e.toString();
    }
  }

  static Future<String?> login(String username, String password) async {
    try {
      final http.Response response = await http.post(
        Uri.parse('http://${AppState.serverDomain}/api/auth/login'),
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode({'username': username, 'password': password}),
      );

      if (response.statusCode == 200) {
        if (kDebugMode) {
          print("User logged in successfully");
        }

        AppState.loginToken = jsonDecode(response.body)["token"];
        return null;
      } else {
        if (kDebugMode) {
          print('Failed to login user, status code: ${response.statusCode}');
        }

        return response.reasonPhrase ?? 'No reason given.';
      }
    } catch (e) {
      if (kDebugMode) {
        print('Failed to login user: ${e.toString()}');
      }

      return e.toString();
    }
  }

  static void logout() {
    AppState.currentUser = null;
    AppState.loginToken = null;
    AppState.currentTicket = null;
    AppState.userDataLoaded = false;
  }

  static bool? checkPassword(String pass) {
    if (AppState.currentUser == null) {
      return null;
    }
    return pass.compareTo(AppState.currentUser!.password) == 0;
  }

  static Future<void> addUser(User user) async {
    try {
      final response = await http.post(
        Uri.parse('http://${AppState.serverDomain}/api/recipes/users'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(user.toJson()),
      );

      if (response.statusCode == 201) {
        if (kDebugMode) {
          print('User added successfully');
        }
      } else {
        throw Exception(
            'Failed to add user, status code: ${response.statusCode}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Failed to login user: ${e.toString()}');
      }
    }
  }

  static Future<void> removeUser(User user) async {
    // TODO implement
  }

  static Future<List<User>> getAllUsers() async {
    try {
      final response = await http
          .get(Uri.parse('http://${AppState.serverDomain}/api/user/users'));

      if (response.statusCode == 200) {
        final List data = json.decode(response.body)['user'];
        return data.map((item) => User.fromJson(item)).toList();
      } else {
        throw Exception(
            'Failed to load users, status code: ${response.statusCode}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Failed to login user: ${e.toString()}');
      }
      return [];
    }
  }

  static Future<User> getUser(String username) async {
    // TODO load liked, saved, added, savesTotal and  likesTotal too
    final response = await http.get(
        Uri.parse('http://${AppState.serverDomain}/api/user/users/$username'));

    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body)['user'];
      return User.fromJson(data);
    } else {
      throw Exception(
          'Failed to load recipe, status code: ${response.reasonPhrase}');
    }
  }

  static Future<List<Recipe>> getLikedRecipes() async {
    // recipes liked by current user
    final response = await http
        .get(Uri.parse('http://${AppState.serverDomain}/api/recipes/liked/'));

    if (response.statusCode == 200) {
      final List data = json.decode(response.body)['recipe'];
      return data.map((item) => Recipe.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load recipes, status code: ${response.statusCode}');
    }
  }

  static Future<List<Recipe>> getSavedRecipes() async {
    // recipes saved by current user
    final response = await http
        .get(Uri.parse('http://${AppState.serverDomain}/api/recipes/saved/'));

    if (response.statusCode == 200) {
      final List data = json.decode(response.body)['recipe'];
      return data.map((item) => Recipe.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load recipes, status code: ${response.statusCode}');
    }
  }
}
