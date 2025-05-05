import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import 'app_state.dart';

enum TagCategory {
  cuisine(name: 'Cuisine'),
  difficulty(name: 'Difficulty'),
  diet(name: 'Diet'),
  expense(name: 'Expense'),
  ingredient(name: 'Ingredient'),
  time(name: 'Time required');

  const TagCategory({required this.name});

  final String name;
}

class Tag {
  final String name;
  bool isPredefined;
  TagCategory? category;

  Tag({required this.isPredefined, required this.name, this.category});

  factory Tag.fromJson(Map<String, dynamic> json) {
    return Tag(
      isPredefined: json['isPredefined'],
      name: json['name'],
      category: TagCategory.values[json['category']],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'isPredefined': isPredefined,
      'name': name,
      'category': category?.index,
    };
  }

  static Future<void> addTag(Tag tag) async {
    final response = await http.post(
      Uri.parse('http://${AppState.serverDomain}/api/recipes/tags'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(tag.toJson()),
    );

    if (response.statusCode == 201) {
      if (kDebugMode) {
        print('Tag added successfully');
      }
    } else {
      throw Exception('Failed to add tag, status code: ${response.statusCode}');
    }
  }

  static Future<List<Tag>> getTags() async {
    final response = await http
        .get(Uri.parse('http://${AppState.serverDomain}/api/recipes/tags'));

    if (response.statusCode == 200) {
      final List data = json.decode(response.body)['tags'];
      return data.map((item) => Tag.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load tags, status code: ${response.statusCode}');
    }
  }

  static Future<List<(Tag, int)>> getPopularNotPredefinedTags() async {
    // TODO get most popular not predefined tags as a list of (tag, #occurrences)
    return [];
  }
}

class PreferredTag {
  final int username;
  final String tagName;
  double weight;

  PreferredTag(
      {required this.username, required this.tagName, required this.weight});
}
