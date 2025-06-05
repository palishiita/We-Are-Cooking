import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import 'app_state.dart';

class Ingredient {
  final String id; // guid = string
  final String name;
  final String description

  Ingredient({
    required this.id,
    required this.name,
    this.description
    });

  factory Ingredient.fromJson(Map<String, dynamic> json) {
    return Ingredient(
      id: json['id'],
      name: json['name'],
      description: json['description']
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': quantity
    };
  }

  static Future<void> addIngredient(Ingredient ingredient) async {
    final response = await http.post(
      Uri.parse('http://${AppState.serverDomain}/api/ingredients'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(ingredient.toJson()),
    );

    if (response.statusCode == 201) {
      if (kDebugMode) {
        print('Ingredient added successfully');
      }
    } else {
      throw Exception(
          'Failed to add ingredient, status code: ${response.statusCode}');
    }
  }

  static Future<List<Ingredient>> getIngredients() async {
    final response = await http
        .get(Uri.parse('http://${AppState.serverDomain}/api/ingredients'));

    if (response.statusCode == 200) {
      final List data = json.decode(response.body)['ingredients'];
      return data.map((item) => Ingredient.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load ingredients, status code: ${response.statusCode}');
    }
  }
}
